using Magic.Guangdong.DbServices.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Interfaces
{
    public interface IAdminRoleRepo : IExaminationRepository<AdminRole>
    {
        Task<bool> Grant(Guid adminId, long[] roleIds);
    }
}
