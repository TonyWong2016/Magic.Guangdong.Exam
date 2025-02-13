﻿using FreeSql.Internal.Model;
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
        public async Task<int> BulkUpdateCerts(string whereJsonStr, int status, int isDeleted=0)
        {
            DynamicFilterInfo dyfilter = JsonConvert.DeserializeObject<DynamicFilterInfo>(whereJsonStr);
            return await fsql.Get(conn_str).Select<Cert>()
                //.Where(u=>u.IsDeleted==0)
                .WhereDynamicFilter(dyfilter)
                .ToUpdate()
                .Set(u => u.Status, status)
                .SetIf(isDeleted != 0, u => u.IsDeleted, isDeleted)
                .ExecuteAffrowsAsync();
        }

        /// <summary>
        /// 批量更新证书状态
        /// </summary>
        /// <param name="whereJsonStr"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public async Task<int> BulkUpdateCerts(long[] certIds, int status, int isDeleted = 0)
        {
            return await fsql.Get(conn_str).Select<Cert>()
                .Where(u => certIds.Contains(u.Id))
                .ToUpdate()
                .Set(u => u.Status, status)
                .SetIf(isDeleted!=0, u => u.IsDeleted, isDeleted)
                .ExecuteAffrowsAsync();
        }

        public async Task<CertDtoForApi> GetCertRecordsForApi(CertRequestDto dto)
        {
            var sql = fsql.Get(conn_str).Select<Cert>()
                .Where(u => u.IsDeleted == 0)
                .WhereIf(!string.IsNullOrEmpty(dto.AwardName), u => u.AwardName.Equals(dto.AwardName))
                .WhereIf(!string.IsNullOrEmpty(dto.CertTitle), u => u.Title.Equals(dto.CertTitle))
                .WhereIf(!string.IsNullOrEmpty(dto.CertNo), u => u.CertNo.Equals(dto.CertNo))
                ;

            var ret = new CertDtoForApi();
            ret.total = await sql.CountAsync();
            ret.items = (await sql
                .OrderByDescending(dto.IsDesc == 1, u => u.Id)
                .Page(dto.PageIndex, dto.PageSize)
                .ToListAsync())
                .Adapt<List<CertDto>>();
            return ret;
        }
    }
}
