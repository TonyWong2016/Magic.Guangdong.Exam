using DotNetCore.CAP;
using EasyCaching.Core;
using Magic.Guangdong.Assistant;
using Magic.Guangdong.Assistant.Contracts;
using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.DbServices.Dtos.Account;
using Magic.Guangdong.DbServices.Dtos.Report.ReportInfo;
using Magic.Guangdong.DbServices.Dtos.System.Admins;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using Magic.Guangdong.Exam.Areas.WebApi.Models;
using Magic.Guangdong.Exam.Extensions;
using Magic.Passport.DbServices.Interfaces;
using Mapster;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Yitter.IdGenerator;

namespace Magic.Guangdong.Exam.Areas.WebApi.Controllers
{
    /// <summary>
    /// 外部开放接口
    /// 目前接口数量不多，所以所有的接口都放在一个控制器里面，
    /// 后续开发中，应该根据业务拆分不同的控制器，提高可读性，扩展性和一定程度的性能 
    /// </summary>
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
        private readonly IUserBaseRepo _userBaseRepo;
        private readonly IReportInfoRepo _reportInfoRepo;
        private readonly ITagsRepo _tagsRepo;
        private readonly IUserAnswerRecordViewRepo _userAnswerRecordViewRepo;
        private readonly IUserCenterRepo _userCenterRepo;
        private readonly IReportAttributeRepo _reportAttributeRepo;

        public ExposedApiController(IResponseHelper resp,
            IRedisCachingProvider redisCachingProvider,
            IWebHostEnvironment webHostEnvironment,
            ICapPublisher capPublisher,
            IAdminRepo adminRepo,
            IRoleRepo roleRepo,
            IAdminRoleRepo adminRoleRepo,
            IActivityRepo activityRepo,
            IExaminationRepo examinationRepo,
            IUserBaseRepo userBaseRepo,
            IReportInfoRepo reportInfoRepo,
            ITagsRepo tagsRepo,
            IUserAnswerRecordViewRepo userAnswerRecordViewRepo,
            IUserCenterRepo userCenterRepo,
            IJwtService jwtService,
            IReportAttributeRepo reportAttributeRepo)
        {
            _resp = resp;
            _redisCachingProvider = redisCachingProvider;
            _webHostEnvironment = webHostEnvironment;
            _capPublisher = capPublisher;
            _adminRepo = adminRepo;
            _roleRepo = roleRepo;
            _adminRoleRepo = adminRoleRepo;
            _activityRepo = activityRepo;
            _examinationRepo = examinationRepo;
            _userBaseRepo = userBaseRepo;
            _reportInfoRepo = reportInfoRepo;
            _jwtService = jwtService;
            _tagsRepo = tagsRepo;
            _userCenterRepo = userCenterRepo;
            _userAnswerRecordViewRepo = userAnswerRecordViewRepo;
            _reportAttributeRepo = reportAttributeRepo;
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
        public async Task<IActionResult> GetAccessToken([FromBody] TokenParam model)
        {
            TimeSpan ts = DateTime.Now - Utils.TimeStampToDateTime(model.timestamp);
            if (ts.TotalSeconds > 300)
            {
                return Json(_resp.error("时间戳已过期"));
            }
            if (!await _adminRepo.getAnyAsync(
                u =>
                u.KeyId.Equals(model.keyId) &&
                u.IsDeleted == 0))
            {
                return Json(_resp.error("用户不存在"));
            }
            var admin = await _adminRepo.getOneAsync(
                u =>
                u.KeyId.Equals(model.keyId) &&
                u.IsDeleted == 0);


            string tt = Security.GenerateMD5Hash(admin.KeyId + admin.KeySecret + model.timestamp);
            if (tt == model.sign.ToLower())
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
        /// 创建活动
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [WebApiModule]
        public async Task<IActionResult> CreateOrModifyActivity([FromBody] ActivityApiDto dto)
        {
            try
            {
                if (dto.St > dto.Et)
                {
                    return Json(_resp.error("开始时间不能大于结束时间"));
                }
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
                //newActivity.StartTime = Utils.TimeStampToDateTime(dto.St);
                //newActivity.EndTime = Utils.TimeStampToDateTime(dto.Et);
                newActivity.CreatedAt = DateTime.Now;
                newActivity.Remark = "接口创建";
                await _activityRepo.addItemAsync(newActivity);
                if (dto.CreatedExam == 1 && dto.ExamDto != null)
                {
                    dto.ExamDto.Id = NewId.NextGuid();
                    if (dto.ExamDto.AssociationId == 0)
                        dto.ExamDto.AssociationId = dto.Id;
                    if (string.IsNullOrEmpty(dto.ExamDto.AssociationTitle))
                        dto.ExamDto.AssociationTitle = dto.Title;
                    if (await CreateOrModifyExam(dto.ExamDto))
                        return Json(_resp.success(new { examId = dto.ExamDto.Id, activityId = dto.Id }, "操作成功"));

                    return Json(_resp.error("创建失败"));
                }
            }
            catch (Exception ex)
            {
                return Json(_resp.error($"创建失败，{ex.Message}"));
            }
            return Json(_resp.success(new { examId = "未提交创建考试的参数", activityId = dto.Id }, "操作成功"));
        }

        /// <summary>
        /// 修改活动
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [WebApiModule]
        public async Task<IActionResult> ModifyActivity(ActivityApiDto dto)
        {
            if (await _activityRepo.getAnyAsync(u => u.Title == dto.Title && u.Id != dto.Id))
            {
                return Json(_resp.error("活动名称已存在"));
            }
            var activity = dto.Adapt<Activity>();
            activity.UpdatedAt = DateTime.Now;
            return Json(_resp.success(await _activityRepo.updateItemAsync(activity)));
        }

        /// <summary>
        /// 创建或修改考试
        /// </summary>
        /// <param name="examApiDto"></param>
        /// <returns></returns>
        [HttpPost]
        [WebApiModule]
        public async Task<IActionResult> CreateOrModifyExamination(ExamApiDto examApiDto)
        {
            if (await CreateOrModifyExam(examApiDto))
                return Json(_resp.success(true, "操作成功"));
            return Json(_resp.error("创建失败"));
        }

        /// <summary>
        /// 提交报名信息
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [WebApiModule]
        public async Task<IActionResult> SubmitReportInfo([FromBody]ReportInfoApiDto dto)
        {
            try
            {
                if (!dto.isValid) 
                {
                    return Json(_resp.error("关键信息不可为空，且中国大陆的证件号和手机号要符合格式要求"));
                }

                if (!await _userBaseRepo.getAnyAsync(u => u.AccountId == dto.AccountId)
                    && !await SyncAccountFromUserCenter(Convert.ToInt32(dto.AccountId)))
                {
                    return Json(_resp.error("用户不存在"));
                }

                if (!await _activityRepo.getAnyAsync(a => a.Id == dto.ActivityId))
                {
                    return Json(_resp.error("活动不存在"));
                }

                //证件号作为唯一报名活动的重复性判定元素
                if (await _reportInfoRepo.getAnyAsync(r => r.HashIdcard == dto.HashIdCard && (r.ActivityId == dto.ActivityId || r.ExamId == dto.ExamId)))
                {
                    return Json(_resp.error("您已经报名该活动了"));
                }
                dto.Id = YitIdHelper.NextId();
                //var reportInfo = dto.Adapt<ReportInfo>();
                dto.OrderTradeNumber = $"{dto.ExamId.ToString().Substring(19, 4).ToUpper()}{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}{Assistant.Utils.GenerateRandomCodePro(15)}";
                //准考证号：身份证后4位+10位时间戳+考试id4位+随机字符2位（如果考试id为空，则随机字符为6位）
                dto.ReportNumber =
                                dto.IdCard.Substring(dto.IdCard.Length - 4, 4) +
                                Utils.DateTimeToTimeStamp(DateTime.Now) +
                               (dto.ExamId == Guid.Empty ? Utils.GenerateRandomCodePro(6) : dto.ExamId.ToString().Substring(19, 4).ToUpper() + Utils.GenerateRandomCodePro(2));
                var reportModel = dto.Adapt<ReportInfoDto>();

                if (!string.IsNullOrEmpty(dto.Tag) && !await _tagsRepo.getAnyAsync(u=>u.Title==dto.Tag))
                {
                    var newTag = new Tags()
                    {
                        Title = dto.Tag,                        
                    };
                    await _tagsRepo.addItemAsync(newTag);
                    reportModel.TagId = newTag.Id;
                }
                if(dto.TagId > 0 && await _tagsRepo.getAnyAsync(u => u.Id == dto.TagId))
                {
                    reportModel.TagId= dto.TagId;
                }

                if (!await _reportInfoRepo.ReportActivity(reportModel))
                {
                    return Json(_resp.error("报名失败"));

                }
                if (dto.reportAttributes != null)
                {
                    dto.reportAttributes.ReportId = dto.Id;
                    await _reportAttributeRepo.SyncReportAttribute(dto.reportAttributes);
                }
                await _capPublisher.PublishAsync(CapConsts.PREFIX + "CheckReportStatus", dto.Id);
                return Json(_resp.success(new
                {
                    tradeNo = dto.OrderTradeNumber,
                    reportId = dto.Id,
                    dto.ReportNumber
                }));
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Json(_resp.error("报名失败"));
            }
        }


        /// <summary>
        /// 绑定标签
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        [WebApiModule]
        [HttpPost]
        public async Task<IActionResult> BindTagForReporter([FromBody]BindTagForReportParam dto)
        {
            try
            {
                if (!await _reportInfoRepo.getAnyAsync(u => u.Id == dto.reportId && u.IsDeleted == 0))
                {
                    return Json(_resp.error("报名信息不存在"));
                }
                long tagId = 0;
                if (!await _tagsRepo.getAnyAsync(u => u.Title == dto.tagName))
                {
                    var newTag = new Tags()
                    {
                        Title = dto.tagName
                    };
                    await _tagsRepo.addItemAsync(newTag);
                    tagId = newTag.Id;
                }
                var tag = await _tagsRepo.getOneAsync(u => u.Title == dto.tagName);
                tagId = tag.Id;

                var reportInfo = await _reportInfoRepo.getOneAsync(u => u.Id == dto.reportId);
                reportInfo.TagId = tagId;
                reportInfo.UpdatedAt = DateTime.Now;
                await _reportInfoRepo.updateItemAsync(reportInfo);
                return Json(_resp.success(true));

            }
            catch (Exception ex)
            {
                Assistant.Logger.Error(ex);
                return Json(_resp.error("绑定失败，"+ex.Message));
            }
            
        }

        /// <summary>
        /// 获取答题记录
        /// </summary>
        /// <param name="recordParam"></param>
        /// <returns></returns>
        [HttpPost]
        [WebApiModule]
        public async Task<IActionResult> GetUserAnswerRecords([FromBody] RecordParam recordParam)
        {
            if (recordParam.reportIds.Length > 50)
            {
                return Json(_resp.error("最多可以查询50条记录"));
            }
            return Json(_resp.success(await _userAnswerRecordViewRepo.GetUserAnswerRecordApi(recordParam.reportIds, recordParam.examType, recordParam.isDeleted)));
        }

        /// <summary>
        /// 审核接口
        /// </summary>
        /// <param name="checkParam"></param>
        /// <returns></returns>
        [HttpPost]
        [WebApiModule]        
        public async Task<IActionResult> CheckReportInfo([FromBody] CheckParam checkParam)
        {
            if (!await _reportInfoRepo.getAnyAsync(u => u.Id == checkParam.reportId))
            {
                return Json(_resp.error("报名信息不存在"));
            }


            if (!Enum.IsDefined(typeof(CheckStatus), checkParam.checkResult))
            {
                return Json(_resp.error("审核结果不正确"));
            }
            var dto = new ReportCheckHistoryDto()
            {
                reportIds = new long[] { checkParam.reportId },
                checkStatus = (CheckStatus)checkParam.checkResult,
                adminId = "api",
                checkRemark = checkParam.checkRemark
            };
            bool result = await _reportInfoRepo.CheckReportInfo(dto);
            if (checkParam.notice == 1)
                await _capPublisher.PublishAsync(CapConsts.PREFIX + "ReportNotice", dto.reportIds);
            if (result)
            {
                return Json(_resp.success("审核成功"));
            }
            return Json(_resp.error("审核失败"));
            

        }

        /// <summary>
        /// 同步申报属性，参与人，项目编号，指导老师，学校，赛队名等
        /// </summary>
        /// <param name="reportAttribute"></param>
        /// <returns></returns>
        [HttpPost]
        [WebApiModule]
        public async Task<IActionResult> SyncReportInfo([FromBody] ReportAttributeParam reportAttribute)
        {
            if(reportAttribute.ReportId==0 || !await _reportInfoRepo.getAnyAsync(u => u.Id == reportAttribute.ReportId && u.IsDeleted==0))
            {
                return Json(_resp.error("申报信息不存在或已被删除"));
            }
            if(await _reportAttributeRepo.SyncReportAttribute(reportAttribute))
            {
                return Json(_resp.success("同步成功"));
            }
            return Json(_resp.error("同步失败"));
        }

        public async Task<bool> CreateOrModifyExam(ExamApiDto examApiDto)
        {
            var exam = examApiDto.Adapt<Examination>();
            if (await _examinationRepo.getAnyAsync(u => u.Id == examApiDto.Id))
            {
                return await _examinationRepo.updateItemAsync(exam) == 1;
            }

            if (await _examinationRepo.getAnyAsync(u => u.Title == exam.Title))
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

        private async Task<bool> SyncAccountFromUserCenter(int uid)
        {
            if (!await _userCenterRepo.getAnyAsync(u => u.UID == uid))
            {
                return false;
            }
            Assistant.Logger.Debug("获取成功");
            var passportUser = await _userCenterRepo.getOneAsync(u => u.UID == uid);
            if (await _userBaseRepo.getAnyAsync(u => u.AccountId == uid.ToString()))
            {
                var userBase = await _userBaseRepo.getOneAsync(u => u.AccountId == uid.ToString());
                return Json(_resp.success(userBase.Adapt<AccountDto>()));
            }
            var newUserBase = new UserBase()
            {
                AccountId = uid.ToString(),
                Name = passportUser.Name,
                Password = Utils.GenerateRandomCodePro(8, 2),
                IdCard = passportUser.IdentityNo,
                Sex = string.IsNullOrEmpty(passportUser.IdentityNo) ? 0
                : passportUser.IdentityNo.Length == 18 ? Convert.ToInt32(passportUser.IdentityNo.Substring(16, 1)) % 2 : 0,
                Email = passportUser.Email,
                Mobile = passportUser.Mobile,
            };

            //await _userBaseRepo.addItemAsync(newUserBase);
            await _userBaseRepo.InsertUserBaseSecurity(newUserBase);
            return true;
        }

        
    }

    public class TokenParam
    {
        public string keyId { get; set; }

        public string sign { get; set; }

        public long timestamp { get; set; }
    }

    public class RecordParam
    {
        public string[] reportIds { get; set; }

        public int? examType { get; set; }

        public int? isDeleted { get; set; }
    }

    public class CheckParam
    {
        public long reportId { get; set; }
        public int checkResult { get; set; }
        public string checkRemark { get; set; }

        public int notice { get; set; } = 1;
    }

    public class  BindTagForReportParam
    {
        public long reportId { get; set; }

        public string tagName { get; set; }
    }

   
}
