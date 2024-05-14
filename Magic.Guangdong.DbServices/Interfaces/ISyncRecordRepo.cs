using Magic.Guangdong.DbServices.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Interfaces
{
    public interface ISyncRecordRepo : IExaminationRepository<SyncRecord>
    {
        Task<int> GetLastRecordByPlatform(string platform);
    }
}
