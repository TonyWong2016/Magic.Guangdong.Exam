using Magic.Guangdong.DbServices.Dtos.Roles;
using Magic.Guangdong.DbServices.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Interfaces
{
    public interface IRoleRepo:IExaminationRepository<Role>
    {
        Task<bool> CreateRole(RoleDto dto);

        Task<bool> UpdateRole(RoleDto dto);

        Task<bool> RemoveRole(long roleId);
    }
}
