using DotNetCore.CAP;
using EasyCaching.Core;
using Magic.Guangdong.Assistant.Contracts;
using Magic.Guangdong.Assistant;
using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.DbServices.Dtos.System.Admins;
using Magic.Guangdong.DbServices.Interfaces;
using Magic.Guangdong.Exam.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Magic.Guangdong.DbServices.Dtos.Report.Activities;
using Magic.Guangdong.DbServices.Entities;
using Mapster;
using Magic.Guangdong.Exam.Areas.WebApi.Models;

namespace Magic.Guangdong.Exam.Areas.WebApi.Controllers
{
    [Area("webapi")]
    public class ExposedApiController : Controller
    {
        private readonly IResponseHelper _resp;
        private readonly IRedisCachingProvider _redisCachingProvider;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ICapPublisher _capPublisher;
        private readonly IAdminRepo _adminRepo;
        private readonly IAdminRoleRepo _adminRoleRepo;
        private readonly IRoleRepo _roleRepo;
        private readonly IJwtService _jwtService;
        private readonly IActivityRepo _activityRepo;
        private readonly IExaminationRepo _examinationRepo;
        public ExposedApiController(IResponseHelper resp,
            IRedisCachingProvider redisCachingProvider, 
            IWebHostEnvironment webHostEnvironment, 
            ICapPublisher capPublisher,
            IAdminRepo adminRepo,
            IRoleRepo roleRepo,
            IAdminRoleRepo adminRoleRepo,
            IActivityRepo activityRepo,
            IExaminationRepo examinationRepo,
            IJwtService jwtService)
        {
            _resp = resp;
            _redisCachingProvider = redisCachingProvider;
            _webHostEnvironment = webHostEnvironment;
            _capPublisher = capPublisher;
            _adminRepo = adminRepo;
            _roleRepo = roleRepo;
            _adminRoleRepo= adminRoleRepo;
            _activityRepo = activityRepo;
            _examinationRepo = examinationRepo;
            _jwtService = jwtService;
        }

        [HttpPost]
        [WebApiModule]
        public async Task<IActionResult> Test()
        {
            return Json(_resp.success(Guid.NewGuid()));
        }

        /// <summary>
        /// 为调用api获取token
        /// </summary>
        /// <param name="keyId"></param>
        /// <param name="sign"></param>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> GetAccessToken(string keyId, string sign, long timestamp)
        {
            TimeSpan ts = DateTime.Now - Utils.TimeStampToDateTime(timestamp);
            if (ts.TotalSeconds > 300)
            {
                return Json(_resp.error("时间戳已过期"));
            }
            if (!await _adminRepo.getAnyAsync(
                u =>
                u.KeyId.Equals(keyId) &&
                u.IsDeleted == 0))
            {
                return Json(_resp.error("用户不存在"));
            }
            var admin = await _adminRepo.getOneAsync(
                u =>
                u.KeyId.Equals(keyId) &&
                u.IsDeleted == 0);


            string tt = Security.GenerateMD5Hash(admin.KeyId + admin.KeySecret + timestamp);
            if (tt == sign.ToLower())
            {
                DateTime expires = DateTime.Now.AddHours(6);
                await CacheMyPermission(new AfterLoginDto() { adminId = admin.Id, exp = expires });
                string jwt = _jwtService.Make(Utils.ToBase64Str(admin.Id.ToString()), admin.Name, expires);
                await _capPublisher.PublishAsync(CapConsts.PREFIX + "SubmitLoginLog", $"{admin.Id}|{jwt}|{Utils.DateTimeToTimeStamp(expires)}");
                return Json(_resp.success(
                    new
                    {
                        access_token = jwt
                    }));
            }
            return Json(_resp.error("获取Token失败，请检查传入参数是否正确"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [WebApiModule]
        public async Task<IActionResult> CreateOrModifyActivity(ActivityApiDto dto)
        {
            try
            {
                if (await _activityRepo.getAnyAsync(u => u.Id == dto.Id && u.Title != dto.Title))
                {
                    var modifyActivity = dto.Adapt<Activity>();
                    modifyActivity.UpdatedAt = DateTime.Now;
                    await _activityRepo.updateItemAsync(modifyActivity);
                }
                else if (await _activityRepo.getAnyAsync(u => u.Title == dto.Title))
                {
                    return Json(_resp.error("活动名称已存在"));
                }
                var newActivity = dto.Adapt<Activity>();
                newActivity.Remark = "接口创建";
                await _activityRepo.addItemAsync(newActivity);
                if (dto.CreatedExam == 1 && dto.ExamDto != null)
                {
                    if (dto.ExamDto.AssociationId == 0)
                        dto.ExamDto.AssociationId = dto.Id;
                    if (string.IsNullOrEmpty(dto.ExamDto.AssociationTitle))
                        dto.ExamDto.AssociationTitle = dto.Title;
                    await CreateOrModifyExam(dto.ExamDto);
                }
            }
            catch(Exception ex)
            {
                return Json(_resp.error($"创建失败，{ex.Message}"));
            }
            return Json(_resp.success(true,"操作成功"));
        }

        [HttpPost]
        [WebApiModule]
        public async Task<IActivityRepo> ModifyActivity(ActivityApiDto dto)
        {
            if (await _activityRepo.getAnyAsync(u => u.Title == dto.Title && u.Id != dto.Id))
            {
                return Json(_resp.error("活动名称已存在"));
            }
            var activity = dto.Adapt<Activity>();
            activity.UpdatedAt = DateTime.Now;
            return Json(_resp.success(await _activityRepo.updateItemAsync(activity)));
        }

        [HttpPost]
        [WebApiModule]
        public async Task<IActivityRepo> CreateOrModifyExamination(ExamApiDto examApiDto)
        {
            if (await CreateOrModifyExam(examApiDto))
                return Json(_resp.success(true, "操作成功"));
            return Json(_resp.error("创建失败"));
        }

        public async Task<bool> CreateOrModifyExam(ExamApiDto examApiDto)
        {
            var exam = examApiDto.Adapt<Examination>();
            if (await _examinationRepo.getAnyAsync(u => u.Id == examApiDto.Id))
            {                
                return await _examinationRepo.updateItemAsync(exam) == 1;
            }

            if(await _examinationRepo.getAnyAsync(u => u.Title == exam.Title))
            {
                return false;
            }
            return await _examinationRepo.addItemAsync(exam) == 1;
        }

        private async Task CacheMyPermission(AfterLoginDto dto)
        {
            await _redisCachingProvider.KeyDelAsync("GD.Exam.Permissions_" + dto.adminId.ToString());
            var myPermissions = await _adminRoleRepo.GetMyPermission(dto.adminId);
            var superRoles = await _roleRepo.getListAsync(u => u.IsDeleted == 0 && u.Type == 1);
            List<long> superRoleIds = new List<long>();
            if (superRoles.Any())
                superRoleIds = superRoles.Select(u => u.Id).ToList();
            if (superRoleIds.Any() && myPermissions.Where(u => superRoleIds.Contains(u.RoleId)).Any())
                await _redisCachingProvider.HSetAsync("GD.Exam.Permissions_" + dto.adminId.ToString(), "super", "super");

            //await _redisCachingProvider.ZAddAsync(adminId.ToString(), myPermissions, Utils.TimeStampToDateTime(exp) - DateTime.Now);
            foreach (var myPermission in myPermissions)
            {
                await _redisCachingProvider.HSetAsync("GD.Exam.Permissions_" + dto.adminId.ToString(), myPermission.router, JsonHelper.JsonSerialize(myPermission));
            }
            await _redisCachingProvider.KeyExpireAsync("GD.Exam.Permissions_" + dto.adminId.ToString(), Convert.ToInt32((dto.exp - DateTime.Now).TotalSeconds));
        }
    }
}
