using Magic.Guangdong.DbServices.Dto.Role;
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
    }
}
