using DotNetCore.CAP;
using Magic.Guangdong.Assistant;
using Magic.Guangdong.Assistant.Contracts;
using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.DbServices.Dtos;
using Magic.Guangdong.DbServices.Dtos.Teacher;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using Magic.Guangdong.Exam.Extensions;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using System.Linq.Expressions;
using System.Text;

namespace Magic.Guangdong.Exam.Areas.Teacher.Controllers
{
    [Area("Teacher")]
    public class TeacherController : Controller
    {
        private readonly IResponseHelper _resp;
        private readonly ITeacherRepo _teacherRepo;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ICapPublisher _capPublisher;
        private string adminId = "system";
        public TeacherController(IResponseHelper resp, ITeacherRepo teacherRepo, IHttpContextAccessor contextAccessor, ICapPublisher capPublisher)
        {
            _resp = resp;
            _teacherRepo = teacherRepo;
            _contextAccessor = contextAccessor;
            _capPublisher = capPublisher;
            adminId = _contextAccessor.HttpContext != null && _contextAccessor.HttpContext.Request.Cookies.Where(u => u.Key == "userId").Any() ?
               Utils.FromBase64Str(_contextAccessor.HttpContext.Request.Cookies.Where(u => u.Key == "userId").First().Value) : "system";

        }

        [RouteMark("教师管理")]
        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 60, VaryByQueryKeys = new string[] { "whereJsonStr", "pageIndex", "pageSize", "orderby", "isAsc" })]
        public IActionResult GetTeachers(PageDto dto)
        {
            long total;
            return Json(_resp.success(new { items = _teacherRepo.getList(dto, out total), total }));
        }

        [ResponseCache(Duration = 60,VaryByQueryKeys =new string[] {"keyword","rd"})]
        public async Task<IActionResult> GetTeacherDrops(string keyword)
        {
            Expression<Func<DbServices.Entities.Teacher, bool>> where = p => p.IsDeleted == 0;
            if (!string.IsNullOrEmpty(keyword))
            {
                where = where.And(p => p.Name.Contains(keyword)||p.TeachNo.Contains(keyword));
            }
            var teachers = await _teacherRepo.getListAsync(where);
            return Json(_resp.success(
               teachers.Select(u => new
               {
                   value = u.Id,
                   text = $"{u.Name}({u.TeachNo})"
               })
           ));
        }


        [RouteMark("新增教师")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TeacherDto dto)
        {
            if (!ModelState.IsValid)
            {
                return Json(_resp.error("姓名，邮箱，电话为必填"));
            }
            if (await _teacherRepo.getAnyAsync(u => u.Email == dto.Email || u.Mobile == dto.Mobile))
            {
                return Json(_resp.error("邮箱或电话已存在"));
            }
            var item = dto.Adapt<DbServices.Entities.Teacher>();

            var tCnt = await _teacherRepo.getCountAsync(u => u.TeachNo != string.Empty);
            //教师编号，1位随机固定大写字母，2位随机字符（字母或数字），5位顺序编号，如：A1A00001
            item.TeachNo = $"{Utils.GenerateRandomCodePro(1, 1)}{Utils.GenerateRandomCodePro(2)}{tCnt.ToString().PadLeft(5, '0')}";
            item.KeyId = Utils.GenerateRandomCodePro(16);
            item.KeySecret = Utils.GenerateRandomCodePro(16);

            if (string.IsNullOrEmpty(dto.AuthToken))
                dto.AuthToken = item.TeachNo;

            item.Password = Security.Encrypt(dto.AuthToken, Encoding.UTF8.GetBytes(item.KeyId), Encoding.UTF8.GetBytes(item.KeySecret));
            item.CreatedBy = adminId;
            return Json(_resp.success(await _teacherRepo.addItemAsync(item)));
        }

        [RouteMark("编辑教师")]
        public async Task<IActionResult> Edit(Guid id)
        {
            if (!await _teacherRepo.getAnyAsync(u => u.Id == id))
            {
                return Json(_resp.error("教师不存在"));
            }
            var item = await _teacherRepo.getOneAsync(u => u.Id == id);

            return View(item.Adapt<TeacherDto>());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TeacherDto dto)
        {
            var item = await _teacherRepo.getOneAsync(u => u.Id == dto.Id);
            string pwd = item.Password;
            item = dto.Adapt(item);
            item.UpdatedAt = DateTime.Now;
            item.Password = pwd;//这个不能再这里改

            return Json(_resp.success(await _teacherRepo.updateItemAsync(item)));
        }

        [RouteMark("修改教师登录密码")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetTeacherPwd(TeacherPwdChangedDto dto)
        {
            var item = await _teacherRepo.getOneAsync(u => u.Id == dto.Id);
            if (item == null)
            {
                return Json(_resp.error("教师不存在"));
            }

            item.Password = Security.Encrypt(dto.AuthToken, Encoding.UTF8.GetBytes(item.KeyId), Encoding.UTF8.GetBytes(item.KeySecret));
            item.UpdatedAt = DateTime.Now;
            if (dto.Notice == 1)
            {
                var teacheDto = item.Adapt<TeacherDto>();
                teacheDto.AuthToken = dto.AuthToken;
                await _capPublisher.PublishAsync(CapConsts.PREFIX + "NoticeTeacherPwdChanged", teacheDto);
            }
            return Json(_resp.success(await _teacherRepo.updateItemAsync(item)));
        }

        [NonAction]
        [CapSubscribe(CapConsts.PREFIX + "NoticeTeacherPwdChanged")]
        public async Task NoticeTeacherPwdChanged(TeacherDto dto)
        {
            var teacher = await _teacherRepo.getOneAsync(u => u.Id == dto.Id);
            List<MailboxAddress> noticeTo = new List<MailboxAddress>()
                {
                     new MailboxAddress(teacher.Name, teacher.Email)
                };
            await EmailKitHelper.SendEMailAsync("密码修改通知", $"您在{ConfigurationHelper.GetSectionValue("providerName")}-主观题判卷系统的登录密码已被修改，修改后密码为【{dto.AuthToken}】。",
                noticeTo);
        }

        [RouteMark("删除教师")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(Guid id)
        {
            var item = await _teacherRepo.getOneAsync(u => u.Id == id);
            item.IsDeleted = 1;
            item.UpdatedAt = DateTime.Now;
            return Json(_resp.success(await _teacherRepo.updateItemAsync(item)));
        }
    }
}
