using Magic.Guangdong.Assistant;
using Magic.Guangdong.DbServices.Dtos.Roles;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using Microsoft.Extensions.DependencyModel.Resolution;
using NPOI.POIFS.Dev;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace Magic.Guangdong.DbServices.Methods
{
    internal class RoleRepo : ExaminationRepository<Role>, IRoleRepo
    {
        private IdleBus<IFreeSql> fsql;
        const string conn_str = "db_exam";
        public RoleRepo(IdleBus<IFreeSql> fsql)
            : base(fsql)
        {
            this.fsql = fsql;
        }

        public async Task<bool> CreateRole(RoleDto dto)
        {
            
            using (var uow = fsql.Get(conn_str).CreateUnitOfWork())
            {
                try
                {
                    var roleRepo = fsql.Get(conn_str).GetRepository<Role>();
                    //上层排重
                    //if (string.IsNullOrWhiteSpace(dto.Name) || await roleRepo.Where(u => u.Name == dto.Name).AnyAsync())
                    //{
                    //    return false;
                    //}
                    long roleId = YitIdHelper.NextId();
                    await roleRepo.InsertAsync(new Role()
                    {
                        Id= roleId,
                        Name = dto.Name,
                        Description = dto.Description,
                        Type = (int)dto.Type
                    });

                    if (dto.PermissionIds != null && dto.PermissionIds.Any())
                    {
                        var rolePermissionRepo = fsql.Get(conn_str).GetRepository<RolePermission>();
                        List<RolePermission> rolePermissions = new List<RolePermission>();
                        foreach(long permissionId in dto.PermissionIds)
                        {
                            rolePermissions.Add(new RolePermission()
                            {
                                RoleId = roleId,
                                PremissionId = permissionId,
                            });
                        }
                        await rolePermissionRepo.InsertAsync(rolePermissions);
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

        /// <summary>
        /// 更新角色，确保调用之前进行了排重操作
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<bool> UpdateRole(RoleDto dto)
        {
            using (var uow = fsql.Get(conn_str).CreateUnitOfWork())
            {
                try
                {
                    var roleRepo = fsql.Get(conn_str).GetRepository<Role>();
                    var role = await roleRepo.Where(u => u.Id == dto.Id).FirstAsync();
                    role.Name = dto.Name;
                    role.Description = dto.Description;
                    role.UpdatedAt = DateTime.Now;
                    role.Type = (int)dto.Type;
                    //上层要判断roleName是否重复
                    await roleRepo.UpdateAsync(role);

                    var rolePermissionRepo = fsql.Get(conn_str).GetRepository<RolePermission>();
                    if (dto.PermissionIds != null && dto.PermissionIds.Any())
                    {
                        var originPermissions = await rolePermissionRepo
                            .Where(u => u.RoleId == dto.Id)
                            .ToListAsync();

                        //找到原来有，后来没有的权限
                        var needDeletePermissionIds = originPermissions.Select(u => u.PremissionId).Except(dto.PermissionIds);
                        if (needDeletePermissionIds.Any())
                        {
                            foreach (var needDeletePermissionId in needDeletePermissionIds)
                            {
                                var delRp = originPermissions.Where(u => u.PremissionId == needDeletePermissionId && u.RoleId == dto.Id && u.IsDeleted == 0).First();
                                delRp.IsDeleted = 1;
                                delRp.UpdatedAt = DateTime.Now;
                                await rolePermissionRepo.UpdateAsync(delRp);
                            }
                        }


                        //找到原来没有，后来有的权限
                        var notExistPermissionIds = dto.PermissionIds.Except(originPermissions.Select(u => u.PremissionId));
                        if (notExistPermissionIds.Any())
                        {
                            foreach (var notExistPermissionId in notExistPermissionIds)
                            {
                                await rolePermissionRepo.InsertAsync(new RolePermission()
                                {
                                    RoleId = dto.Id,
                                    PremissionId = notExistPermissionId
                                });
                            }
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

        public async Task<bool> RemoveRole(long roleId)
        {
            using (var uow = fsql.Get(conn_str).CreateUnitOfWork())
            {
                try
                {
                    //var keyActionRepo = fsql.Get(conn_str).GetRepository<KeyAction>();
                    var roleRepo = fsql.Get(conn_str).GetRepository<Role>();
                    var rolePermissionRepo = fsql.Get(conn_str).GetRepository<RolePermission>();
                    var role = await roleRepo.Where(u => u.Id == roleId).FirstAsync();
                    role.IsDeleted = 1;
                    role.UpdatedAt = DateTime.Now;

                    await roleRepo.UpdateAsync(role);

                    var rolePermissions = await rolePermissionRepo
                        .Where(u => u.RoleId == roleId && u.IsDeleted == 0)
                        .ToListAsync();

                    if (rolePermissions.Any())
                    {
                        foreach (var rp in rolePermissions)
                        {
                            rp.IsDeleted = 1;
                            rp.UpdatedAt = DateTime.Now;
                            await rolePermissionRepo.UpdateAsync(rp);
                        }
                    }
                    uow.Commit();
                    return true;
                }
                catch(Exception ex)
                {
                    uow.Rollback();
                    Logger.Error(ex.Message);
                    return false;
                }
            }
        }

        
    }
}
