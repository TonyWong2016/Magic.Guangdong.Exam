using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;

namespace Magic.Guangdong.DbServices.Methods
{
    internal class CertRepo :ExaminationRepository<Cert>,ICertRepo
    {
        private IdleBus<IFreeSql> fsql;
        const string conn_str = "db_exam";
        public CertRepo(IdleBus<IFreeSql> fsql)
            : base(fsql)
        {
            this.fsql = fsql;
        }

        public async Task<int> InsertCertBatch(List<Cert> certs)
        {
            var certNos = certs.Select(u=>u.CertNo).ToList();
            var idNumbers = certs.Select(u => u.IdNumber).ToList();
            await fsql.Get(conn_str).Select<Cert>()
                    .Where(u => certNos.Contains(u.CertNo) || idNumbers.Contains(u.IdNumber))
                    .ToUpdate()
                    .Set(u => u.IsDeleted == 1)
                    .Set(u => u.UpdatedAt == DateTime.Now)
                    .ExecuteAffrowsAsync();

            return await addItemsAsync(certs);
        }
    }
}
