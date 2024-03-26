using Magic.Guangdong.DbServices.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Interfaces
{
    public interface IAdminLoginLogRepo : IExaminationRepository<AdminLoginLog>
    {
        Task<int> InsertLoginLog(Guid adminId, string jwt, string exp);
    }
}
