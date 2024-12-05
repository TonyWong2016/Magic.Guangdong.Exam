using FreeSql.Internal.Model;
using Magic.Guangdong.DbServices.Dtos.Cert;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using Mapster;
using Newtonsoft.Json;

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

        public async Task<List<CertDto>> GetCertRecordsForExcel(string whereJsonStr)
        {
            DynamicFilterInfo dyfilter = JsonConvert.DeserializeObject<DynamicFilterInfo>(whereJsonStr);
            var list = await fsql.Get(conn_str).Select<Cert>()
                .Where(u=>u.IsDeleted==0)
                .WhereDynamicFilter(dyfilter)
                .ToListAsync();

           
            return list.Adapt<List<CertDto>>();
        }

        /// <summary>
        /// 批量删除(按检索条件)
        /// </summary>
        /// <param name="whereJsonStr"></param>
        /// <returns></returns>
        public async Task<int> BulkRemoveCerts(string whereJsonStr)
        {
            DynamicFilterInfo dyfilter = JsonConvert.DeserializeObject<DynamicFilterInfo>(whereJsonStr);
            return await fsql.Get(conn_str).Select<Cert>()
                .WhereDynamicFilter(dyfilter)
                .ToUpdate()
                .Set(u => u.IsDeleted, 1)
                .ExecuteAffrowsAsync();
        }

        /// <summary>
        /// 批量删除（按勾选记录）
        /// </summary>
        /// <param name="recordIds"></param>
        /// <returns></returns>
        public async Task<int> BulkRemoveCerts(long[] ids)
        {
            return await fsql.Get(conn_str).Select<Cert>()
                .Where(u => ids.Contains(u.Id))
                .ToUpdate()
                .Set(u => u.IsDeleted, 1)
                .ExecuteAffrowsAsync();
        }

        /// <summary>
        /// 批量更新证书状态
        /// </summary>
        /// <param name="whereJsonStr"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public async Task<int> BulkUpdateCerts(string whereJsonStr, int status)
        {
            DynamicFilterInfo dyfilter = JsonConvert.DeserializeObject<DynamicFilterInfo>(whereJsonStr);
            return await fsql.Get(conn_str).Select<Cert>()
                .WhereDynamicFilter(dyfilter)
                .ToUpdate()
                .Set(u => u.Status, status)
                .ExecuteAffrowsAsync();
        }

        /// <summary>
        /// 批量更新证书状态
        /// </summary>
        /// <param name="whereJsonStr"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public async Task<int> BulkUpdateCerts(long[] certIds, int status)
        {
            return await fsql.Get(conn_str).Select<Cert>()
                .Where(u => certIds.Contains(u.Id))
                .ToUpdate()
                .Set(u => u.Status, status)
                .ExecuteAffrowsAsync();
        }
    }
}
