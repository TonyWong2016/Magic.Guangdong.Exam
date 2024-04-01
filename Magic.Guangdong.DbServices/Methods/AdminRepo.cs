using FreeSql.Internal.Model;
using Magic.Guangdong.Assistant;
using Magic.Guangdong.DbServices.Dto;
using Magic.Guangdong.DbServices.Dtos.System.Roles;
using Magic.Guangdong.DbServices.Dtos.System.Admins;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using Mapster;
using MassTransit;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace Magic.Guangdong.DbServices.Methods
{
    internal class AdminRepo : ExaminationRepository<Admin>, IAdminRepo
    {
        private IdleBus<IFreeSql> fsql;
        const string conn_str = "db_exam";
        public AdminRepo(IdleBus<IFreeSql> fsql)
            : base(fsql)
        {
            this.fsql = fsql;
        }
        /// <summary>
        /// 获取管理员列表
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public List<AdminListDto> GetAdminList(AdminListPageDto dto, out long total)
        {

            var adminRepo = fsql.Get(conn_str).GetRepository<Admin>();
            List<AdminListDto> list = new List<AdminListDto>();
            List<Admin> admins = new List<Admin>();


            if (string.IsNullOrEmpty(dto.whereJsonStr))
            {
                admins = adminRepo
                    .Where(u => u.IsDeleted == 0)
                    .Count(out total)
                    .Page(dto.pageindex, dto.pagesize)
                    .ToList();
            }
            else
            {
                DynamicFilterInfo dyfilter = JsonConvert.DeserializeObject<DynamicFilterInfo>(dto.whereJsonStr);

                admins = adminRepo
                    .Where(u => u.IsDeleted == 0)
                    .WhereDynamicFilter(dyfilter)
                    .Count(out total)
                    .Page(dto.pageindex, dto.pagesize)
                    .ToList();
            }

            var adminIds = admins.Select(u => u.Id);
            var roles = fsql.Get(conn_str).Select<Role, AdminRole>()
                .LeftJoin((a, b) => a.Id == b.RoleId)
                .WhereIf(dto.roleIds != null && dto.roleIds.Length > 0, (a, b) => dto.roleIds.Contains(b.RoleId))
                .Where((a, b) => a.IsDeleted == 0 && b.IsDeleted == 0)
                .ToList((a, b) => new
                {
                    b.AdminId,
                    b.RoleId,
                    RoleName = a.Name,
                    RoleType = a.Type,
                })
                .Adapt<List<AdminRoleList>>();
            if (!roles.Any())
            {
                return list;
            }
            list = admins.Adapt<List<AdminListDto>>();
            foreach (var item in list)
            {
                if (roles.Where(u => u.AdminId == item.Id).Any())
                    item.adminRoles = roles.Where(u => u.AdminId == item.Id).ToList();
                
            }
            if(dto.roleIds != null && dto.roleIds.Length > 0)
                return list.Where(u=> u.adminRoles!=null && u.adminRoles.Any()).ToList();
            return list;
        }

        /// <summary>
        /// 后台创建管理员
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<bool> CreateAdmin(AdminDto dto)
        {
            using (var uow = fsql.Get(conn_str).CreateUnitOfWork())
            {
                try
                {
                    var adminRepo = fsql.Get(conn_str).GetRepository<Admin>();
                    if (await adminRepo
                        .Where(u => u.Name == dto.Name ||
                        u.Email == dto.Email ||
                        u.Mobile == dto.Mobile).AnyAsync())
                    {
                        return false;
                    }
                    string keySecret = Utils.GenerateRandomCodePro(16);
                    string keyId = Utils.GenerateRandomCodePro(16);
                    string password = Security.Encrypt(dto.Password, Encoding.UTF8.GetBytes(keyId), Encoding.UTF8.GetBytes(keySecret));
                    Guid adminId = NewId.NextGuid();
                    var admin = dto.Adapt<Admin>();
                    admin.Id = adminId;
                    admin.Password = password;
                    admin.KeyId = keyId;
                    admin.KeySecret = keySecret;
                    admin.Version = Utils.GetTimeStampUint(DateTime.Now).ToString();
                    await adminRepo.InsertAsync(admin);
                    //await adminRepo.InsertAsync(new Admin
                    //{
                    //    Id= adminId,
                    //    Name = dto.Name,
                    //    Email = dto.Email,
                    //    Mobile = dto.Mobile,
                    //    Password = password,
                    //    KeySecret = keySecret,
                    //    KeyId = keyId,
                    //    NickName = dto.NickName,
                    //    Status = dto.Status,
                    //    Description = dto.Description,
                    //    AccountAvailable = dto.AccountAvailable,
                    //});

                    if (dto.RoleIds.Any())
                    {
                        var adminRoleRepo = fsql.Get(conn_str).GetRepository<AdminRole>();
                        List<AdminRole> adminRoles = new List<AdminRole>();
                        foreach (var roleId in dto.RoleIds)
                        {
                            adminRoles.Add(new AdminRole { RoleId = roleId, AdminId = adminId });
                        }
                        await adminRoleRepo.InsertAsync(adminRoles);
                    }
                    uow.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    uow.Rollback();
                    Logger.Error(ex.Message);
                    return false;
                }
            }
        }

        public async Task<bool> UpdateAdmin(AdminDto dto)
        {
            using (var uow = fsql.Get(conn_str).CreateUnitOfWork())
            {
                try
                {
                    var adminRepo = fsql.Get(conn_str).GetRepository<Admin>();
                    if (!await adminRepo.Where(u => u.Id == dto.Id && u.IsDeleted == 0).AnyAsync())
                        return false;
                    var admin = await adminRepo.Where(u => u.Id == dto.Id).FirstAsync();

                    if (!string.IsNullOrWhiteSpace(dto.Password))
                    {
                        string keySecret = admin.KeySecret;
                        string keyId = admin.KeyId;
                        string password = Security.Encrypt(dto.Password, Encoding.UTF8.GetBytes(keyId), Encoding.UTF8.GetBytes(keySecret));
                        admin.Password = password;
                    }
                    admin.Name = dto.Name;
                    admin.Email = dto.Email;
                    admin.Mobile = dto.Mobile;
                    admin.NickName = dto.NickName;
                    admin.Status = dto.Status;
                    admin.Description = dto.Description;
                    admin.UpdatedAt = DateTime.Now;
                    admin.Version = Utils.GetTimeStampUint(DateTime.Now).ToString();
                    await adminRepo.UpdateAsync(admin);

                    if (dto.RoleIds.Any())
                    {
                        var adminRoleRepo = fsql.Get(conn_str).GetRepository<AdminRole>();
                        var originRoles = await adminRoleRepo.Where(u => u.AdminId == admin.Id).ToListAsync();
                        var needDelRoles = originRoles.Select(u => u.RoleId).Except(dto.RoleIds);
                        if (needDelRoles.Any())
                        {
                            List<AdminRole> delArs = new List<AdminRole>();
                            //await adminRoleRepo.DeleteAsync(u => u.AdminId == admin.Id && needDelRoles.Contains(u.RoleId));
                            foreach (var roleId in needDelRoles)
                            {
                                var delAr = await adminRoleRepo.Where(u => u.AdminId == admin.Id && u.RoleId == roleId).FirstAsync();
                                delAr.IsDeleted = 1;
                                delAr.UpdatedAt = DateTime.Now;
                                delArs.Add(delAr);
                            }
                            await adminRoleRepo.UpdateAsync(delArs);
                        }

                        var needAddRoles = dto.RoleIds.Except(originRoles.Select(u => u.RoleId));
                        if (needAddRoles.Any())
                        {
                            List<AdminRole> addArs = new List<AdminRole>();
                            foreach (var roleId in needAddRoles)
                            {
                                addArs.Add(new AdminRole { RoleId = roleId, AdminId = admin.Id });
                            }
                            await adminRoleRepo.InsertAsync(addArs);
                        }
                    }
                    uow.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    uow.Rollback();
                    Logger.Error(ex.Message);
                    return false;
                }

            }
        }

    }

}
