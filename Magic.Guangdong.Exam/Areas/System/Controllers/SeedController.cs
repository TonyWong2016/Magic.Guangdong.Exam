using FreeSql.Internal;
using Magic.Guangdong.Assistant;
using Magic.Guangdong.Assistant.IService;
using Magic.Guangdong.DbServices.Dtos.System.Routers;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using Magic.Guangdong.Exam.Extensions;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Reflection;
using System.Text;
using Yitter.IdGenerator;

namespace Magic.Guangdong.Exam.Areas.System.Controllers
{
    [Area("System")]
    public class SeedController : Controller
    {
        private IMenuRepo _menuRepo;
        private IAdminRepo _adminrepo;
        private IAdminRoleRepo _adminRoleRepo;
        private IPermissionRepo _permissionRepo;
        private IRoleRepo _roleRepo;
        private IRolePermissionRepo _rolePermissionRepo;
        private IResponseHelper _resp;
        public SeedController(IResponseHelper responseHelper, IMenuRepo menuRepo, IAdminRepo adminRepo, IAdminRoleRepo adminRoleRepo, IPermissionRepo permissionRepo, IRolePermissionRepo rolePermissionRepo, IRoleRepo roleRepo)
        {
            // _menuRepo = menuRepo;
            _resp = responseHelper;
            _menuRepo = menuRepo;
            _adminrepo = adminRepo;
            _permissionRepo = permissionRepo;
            _roleRepo = roleRepo;
            _adminRoleRepo = adminRoleRepo;
            _rolePermissionRepo = rolePermissionRepo;
        }
        public IActionResult Index()
        {
            // _menuRepo.getList();

            return View();
        }

        /// <summary>
        /// 初始化顶级菜单
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> InitTopMenuData()
        {
            if (await _menuRepo.getAnyAsync(u => u.IsDeleted == 0 && u.ParentId == 0))
            {
                return Json(_resp.success("success", "无需插入"));
            }
            var topMenu = new List<DbServices.Entities.Menu>() {
                new DbServices.Entities.Menu()
                {
                    Name="考试管理",
                    Router="",
                    ParentId = 0,
                    Depth=0,
                    IsLeef = 0,
                    Description ="考试管理有关操作"
                },
                new DbServices.Entities.Menu()
                {
                    Name="报名管理",
                    Router="",
                    ParentId = 0,
                    Depth=0,
                    IsLeef = 0,
                    Description ="报名管理有关操作"
                },
                new DbServices.Entities.Menu()
                {
                    Name="账单管理",
                    Router="",
                    ParentId = 0,
                    Depth=0,
                    IsLeef = 0,
                    Description = "账单管理有关操作"
                },
                new DbServices.Entities.Menu()
                {
                    Name="发票管理",
                    Router="",
                    ParentId = 0,
                    Depth=0,
                    IsLeef = 0,
                    Description ="发票管理有关操作"
                },
                new DbServices.Entities.Menu()
                {
                    Name="系统管理",
                    Router="",
                    ParentId = 0,
                    Depth=0,
                    IsLeef = 0,
                    Description ="系统管理有关操作"
                },
            };
            await _menuRepo.addItemsBulkAsync(topMenu);
            return Json(_resp.success("success", "种子数据插入完成"));
        }

        /// <summary>
        /// 初始化二级菜单
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> InitSubMenuData()
        {
            if (await _menuRepo.getAnyAsync(u => u.IsDeleted == 0 && u.ParentId != 0))
            {
                return Json(_resp.success("success", "无需插入"));
            }
            var tops = await _menuRepo.getListAsync(u => u.ParentId == 0);
            var subMenus = new List<DbServices.Entities.Menu>();

            foreach (var item in tops)
            {
                int cnt = 0;
                while (cnt < 3)
                {
                    string name = $"{item.Name}-子菜单{cnt + 1}";
                    subMenus.Add(new DbServices.Entities.Menu()
                    {
                        Name = name,
                        Router = "",
                        ParentId = item.Id,
                        Depth = item.Depth + 1,
                        IsLeef = 0,
                        Description = $"{name}有关操作",
                        OrderIndex = (item.Depth + 1) * 10
                    });
                    cnt++;
                }

            }

            await _menuRepo.addItemsBulkAsync(subMenus);
            return Json(_resp.success("success", "种子数据插入完成"));
        }

        /// <summary>
        /// 初始化管理员
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> InitAdminData()
        {
            if (await _adminrepo.getAnyAsync(u => u.IsDeleted == 0))
            {
                return Json(_resp.success("success", "无需插入"));
            }
            long roleId = 0;
            var topRole = await _roleRepo.getOneAsync(u => u.Type == 1);

            if (topRole == null)
            {
                roleId = YitIdHelper.NextId();
                await _roleRepo.addItemAsync(new DbServices.Entities.Role()
                {
                    Id = roleId,
                    Name = "系统管理角色",
                    Description = "初始化管理角色，系统顶级权限",
                    Type = 1
                });
            }
            else
                roleId = topRole.Id;
            Guid adminId = NewId.NextGuid();
            string keySecret = Assistant.Utils.GenerateRandomCodePro(16);
            string keyId = Assistant.Utils.GenerateRandomCodePro(16);
            string password = Security.Encrypt("123456", Encoding.UTF8.GetBytes(keyId), Encoding.UTF8.GetBytes(keySecret));
            await _adminrepo.addItemAsync(new DbServices.Entities.Admin()
            {
                Id = adminId,
                Name = "sa",
                Email = "wtlemon@126.com",
                Mobile = "18888888888",
                Description = "系统管理员",
                KeyId = keyId,
                KeySecret = keySecret,
                Password = password,
            });
            await _adminRoleRepo.addItemAsync(new DbServices.Entities.AdminRole()
            {
                AdminId = adminId,
                RoleId = roleId,
            });

            return Json(_resp.success("success", "种子数据插入完成"));

        }

        /// <summary>
        /// 自动填充权限数据
        /// </summary>
        /// <returns></returns>
        [RouteMark("自动填充权限数据")]
        public async Task<IActionResult> InitPermissionData()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var types = assembly.GetTypes().Where(u => u.Namespace != null).Where(u => u.Namespace.StartsWith("Magic.Guangdong.Exam.") && u.Name.EndsWith("Controller"));

            List<Permission> permissions = new List<Permission>();
            foreach (var type in types)
            {
                string area = "";
                if (type.CustomAttributes.Where(u => u.AttributeType.Name.Contains("Area")).Any())
                {
                    area = type.CustomAttributes.Where(u => u.AttributeType.Name.Contains("Area")).First().ConstructorArguments[0].Value.ToString();
                }

                foreach (var methodInfo in type.GetMethods())
                {
                    string method = "get";
                    if (methodInfo.CustomAttributes.Where(u => u.AttributeType.Name.StartsWith("Http")).Any())
                    {
                        method = methodInfo.CustomAttributes
                            .Where(u => u.AttributeType.Name.StartsWith("Http"))
                            .First()
                            .AttributeType
                            .Name.Replace("Http", "").Replace("Attribute", "").ToLower();
                    }
                    var existPermissions = await _permissionRepo.getListAsync(u=>u.IsDeleted==0);
                    foreach (Attribute attribute in methodInfo.GetCustomAttributes(false))
                    {

                        if (attribute is RouteMark routeMark)
                        {
                            if (
                                existPermissions.Any(
                                    u => u.Name == routeMark.Module.ToLower() &&
                                    u.Area == area.ToLower() &&
                                    u.Controller == type.Name.Replace("Controller", "").ToLower() &&
                                    u.Action == methodInfo.Name.ToLower() &&
                                    u.Method == method))
                            {
                                continue;
                            }

                            string router = $"/{area}/{type.Name.Replace("Controller", "").ToLower()}/{methodInfo.Name}";
                            if (string.IsNullOrEmpty(area))
                                router = router.Substring(1);
                            permissions.Add(new Permission()
                            {
                                Id = YitIdHelper.NextId(),
                                Name = routeMark.Module,
                                Controller = type.Name.Replace("Controller", "").ToLower(),
                                Action = methodInfo.Name.ToLower(),
                                Area = area.ToLower(),
                                Description = $"{routeMark.Module}（{router}）",
                                Method = method,
                            });
                        }
                    }
                }

            }
            
            if (permissions.Any())
            {
                await _permissionRepo.addItemsBulkAsync(permissions);
            }
            return Json(_resp.success(new { items = permissions, total = permissions.Count() }));

        }

        public async Task<IActionResult> InitActivityData([FromServices] IActivityRepo activityRepo)
        {
            if (await activityRepo.getCountAsync(u => u.Id > 0) != 0)
            {
                return Json(_resp.error("已有数据"));
            }

            var list = new List<Activity>(){
                new Activity()
                {
                    Title = "测试活动1",
                    Description = "测试1",
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now.AddDays(45),
                },
                new Activity()
                {
                    Title = "测试活动2",
                    Description = "测试2",
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now.AddDays(60),
                }
            };
            await activityRepo.addItemsBulkAsync(list);
            return Json(_resp.success(true));


        }

        public async Task<IActionResult> InitUnitInfoData([FromServices] IUnitInfoRepo unitInfoRepo)
        {
            if (await unitInfoRepo.getCountAsync(u => u.Id > 0) > 0)
            {
                return Json(_resp.error("已有数据"));
            }
            var list = new List<UnitInfo>(){
                new UnitInfo()
                {
                    UnitNo="6666",
                    UnitCaption="不正常人类研究中心",
                    UnitIntroduction="不正常人类研究中心",
                    UnitType=UnitType.ScientificResearchInstitution,
                    UnitUrl = "https://github.com",
                    Address="火星",
                    ProvinceId=32,
                    CityId=320100,
                    DistrictId=320111,
                    Status=0,
                    OrganizationCode="123456",
                    Telephone="123456"
                },
                new UnitInfo()
                {
                     UnitNo="6666",
                    UnitCaption="正常人类研究中心",
                    UnitIntroduction="正常人类研究中心",
                    UnitType=UnitType.Enterprises,
                    UnitUrl = "https://picocss.com",
                    Address="地球星",
                    ProvinceId=13,
                    CityId=130600,
                    DistrictId=130603,
                    Status=0,
                    OrganizationCode="654321",
                    Telephone="123456789"
                }
            };
            await unitInfoRepo.addItemsBulkAsync(list);
            return Json(_resp.success(true));
        }
    }
}
