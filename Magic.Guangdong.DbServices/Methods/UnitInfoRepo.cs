using FreeSql.Internal.Model;
using Magic.Guangdong.DbServices.Dtos;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Mozilla;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Methods
{
    internal class UnitInfoRepo : ExaminationRepository<UnitInfo>, IUnitInfoRepo
    {
        private IdleBus<IFreeSql> fsql;
        const string conn_str = "db_exam";
        public UnitInfoRepo(IdleBus<IFreeSql> fsql)
            : base(fsql)
        {
            this.fsql = fsql;
        }

        public dynamic GetUnitInfos(PageDto dto, out long total)
        {
            //var cityViewRepo = fsql.Get(conn_str).GetRepository<CityView>();
            if (string.IsNullOrEmpty(dto.whereJsonStr))
            {
                return fsql.Get(conn_str).Select<UnitInfoView>()
                 .Where(u => u.IsDeleted == 0)
                .OrderByPropertyName(dto.orderby, dto.isAsc)
                .Count(out total)
                .Page(dto.pageindex, dto.pagesize)
                .ToList();
            }
            DynamicFilterInfo dyfilter = JsonConvert.DeserializeObject<DynamicFilterInfo>(dto.whereJsonStr);
            return fsql.Get(conn_str).Select<UnitInfoView>()
                .Where(u => u.IsDeleted == 0)
                .WhereDynamicFilter(dyfilter)
                .OrderByPropertyName(dto.orderby, dto.isAsc)
                .Count(out total)
                .Page(dto.pageindex, dto.pagesize)
                .ToList();
        }

        public async Task<dynamic> GetUnitDropsAsync(string keyword, int provinceId, int cityId, int districtId, int limit=1000)
        {
            if (limit <= 0)
                limit = 1000;
            return await fsql.Get(conn_str).Select<UnitInfoView>()
                .Where(u => u.IsDeleted == 0 && u.Status == 0)//下拉列表里只能选通过审核的
                .WhereIf(!string.IsNullOrWhiteSpace(keyword), u => u.UnitCaption.Contains(keyword))
                .WhereIf(provinceId > 0, u => u.ProvinceId == provinceId)
                .WhereIf(cityId > 0, u => u.CityId == cityId)                
                .WhereIf(districtId > 0, u => u.DistrictId == districtId)
                .Take(limit)
                .ToListAsync(u => new
                {
                    value = u.Id,
                    name = u.UnitCaption,
                    code = u.OrganizationCode,
                    u.Address,
                    //u.Status,
                    u.UnitStatus
                    //province = u.ProvinceShortName
                });
        }
        public async Task<UnitInfoView> GetUnitInfoView(int id)
        {
            return await fsql.Get(conn_str).Select<UnitInfoView>()
                .Where(u => u.IsDeleted == 0)
                .Where(u => u.Id == id)
                .ToOneAsync();
        }
    }
}
