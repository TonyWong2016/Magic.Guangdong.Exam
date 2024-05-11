using Magic.Guangdong.Assistant;
using Magic.Guangdong.DbServices.Dtos.System.AdminRoles;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Methods
{
    internal class AdminRoleRepo : ExaminationRepository<AdminRole>, IAdminRoleRepo
    {
        private IdleBus<IFreeSql> fsql;
        const string conn_str = "db_exam";
        public AdminRoleRepo(IdleBus<IFreeSql> fsql)
            : base(fsql)
        {
            this.fsql = fsql;
        }

        public async Task<bool> Grant(Guid adminId, long[] roleIds)
        {
            using (var uow = fsql.Get(conn_str).CreateUnitOfWork())
            {
                try
                {
                    var adminRoleRepo = fsql.Get(conn_str).GetRepository<AdminRole>();
                    adminRoleRepo.UnitOfWork = uow;
                    if (roleIds == null || roleIds.Length == 0)
                    {
                        return false;
                    }
                    var roleRepo = fsql.Get(conn_str).GetRepository<Role>();
                    roleRepo.UnitOfWork = uow;
                    if (!await roleRepo.Where(u => roleIds.Contains(u.Id)).AnyAsync())
                    {
                        return false;
                    }
                    var adminRepo = fsql.Get(conn_str).GetRepository<Admin>();
                    adminRepo.UnitOfWork = uow;
                    if(!await adminRepo.Where(u=>u.Id == adminId).AnyAsync())
                    {
                        return false;
                    }
                    List<AdminRole> ads = new List<AdminRole>();
                    if (!await adminRoleRepo.Where(x => x.AdminId == adminId).AnyAsync())
                    {
                        foreach (var roleId in roleIds)
                        {
                            ads.Add(new AdminRole()
                            {
                                AdminId = adminId,
                                RoleId = roleId
                            });
                        }
                        await adminRoleRepo.InsertAsync(ads);
                    }
                    else
                    {
                        var originRoles = await adminRoleRepo.Where(x => x.AdminId == adminId).ToListAsync();
                        //找到原来有，后来没有的角色，删掉
                        var needDeleteRoleIds = originRoles.Select(u => u.RoleId).Except(roleIds);
                        if (originRoles.Select(x => x.RoleId).Except(roleIds).Any())
                        {
                            foreach (var roleId in needDeleteRoleIds)
                            {
                                var delAr = await adminRoleRepo.Where(x => x.AdminId == adminId && x.RoleId == roleId).FirstAsync();
                                delAr.IsDeleted = 1;
                                delAr.UpdatedAt = DateTime.Now;
                                await adminRoleRepo.UpdateAsync(delAr);
                            }
                        }
                        //找到原来没有的角色，加上
                        var notExistRoleIds = roleIds.Except(originRoles.Select(u => u.RoleId));
                        if (notExistRoleIds.Any())
                        {
                            foreach (var roleId in notExistRoleIds)
                            {
                                ads.Add(new AdminRole()
                                {
                                    AdminId = adminId,
                                    RoleId = roleId
                                });
                            }
                            await adminRoleRepo.InsertAsync(ads);
                        }
                    }
                    
                    var admin = await adminRepo.Where(x => x.Id == adminId).FirstAsync();
                    admin.UpdatedAt = DateTime.Now;
                    admin.Version += 1;
                    await adminRepo.UpdateAsync(admin);

                    uow.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    uow.Rollback();
                    Logger.Error(ex.Message);
                    return true;
                }
            }
        }

        public async Task<List<AdminPermissionDto>> GetMyPermission(Guid adminId)
        {
            return await fsql.Get(conn_str).Select<AdminRole, RolePermission, Permission>()
                .LeftJoin((a, b, c) => a.RoleId == b.RoleId)
                .LeftJoin((a, b, c) => b.PremissionId == c.Id)
                .Where((a, b, c) => a.AdminId == adminId)
                .ToListAsync((a, b, c) => new AdminPermissionDto()
                {
                    RoleId = a.RoleId,
                    AdminId = adminId,
                    PermissionId = b.PremissionId,
                    area = c.Area,
                    controller = c.Controller,
                    action = c.Action,
                    dataFilterJson = c.DataFilterJson,
                });
                
        }
    }


}
