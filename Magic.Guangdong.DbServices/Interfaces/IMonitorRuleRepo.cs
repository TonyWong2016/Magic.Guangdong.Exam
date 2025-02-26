using Magic.Guangdong.DbServices.Dtos.Monitor;
using Magic.Guangdong.DbServices.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Interfaces
{
    public interface IMonitorRuleRepo : IExaminationRepository<MonitorRule>
    {
        Task<dynamic> GetRuleMini();

        Task<bool> CreateMonitorRole(MainMonitorDto dto);
    }
}
