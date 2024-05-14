using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Methods
{
    internal class SyncRecordRepo:ExaminationRepository<SyncRecord>, ISyncRecordRepo
    {
        private IdleBus<IFreeSql> fsql;
        const string conn_str = "db_exam";
        public SyncRecordRepo(IdleBus<IFreeSql> fsql)
            : base(fsql)
        {
            this.fsql = fsql;
        }

        public async Task<int> GetLastRecordByPlatform(string platform)
        {
            var syncRepo = fsql.Get(conn_str).GetRepository<SyncRecord>();
            if(await syncRepo.Where(u=>u.Platform == platform).AnyAsync())
            {
                return await syncRepo.Where(u=>u.Platform == platform)
                    .OrderByDescending(u=>u.Id)
                    .ToOneAsync(u=>u.Times);
            }
            return 0;
        }
    }
}
