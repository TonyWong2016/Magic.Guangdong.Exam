using FreeSql.Internal.Model;
using Magic.Guangdong.DbServices.Dto;
using Magic.Guangdong.DbServices.Dtos.Admins;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using Mapster;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public List<AdminListDto> GetAdminList(PageDto dto, long[] roleIds, out long total)
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
                .WhereIf(roleIds != null && roleIds.Length > 0, (a, b) => roleIds.Contains(b.RoleId))
                .Where((a, b) => a.IsDeleted == 0 && b.IsDeleted == 0)
                .ToList((a, b) => new
                {
                    b.AdminId,
                    b.RoleId,
                    RoleName = a.Name,
                })
                .Adapt<List<AdminRoleList>>();
            list = admins.Adapt<List<AdminListDto>>();
            foreach (var item in list)
            {
                if (roles.Where(u => u.AdminId == item.Id).Any())
                    item.adminRoles = roles.Where(u => u.AdminId == item.Id).ToList();
                
            }

            return list.Where(u=>u.adminRoles.Any()).ToList();

        }
    }

}
