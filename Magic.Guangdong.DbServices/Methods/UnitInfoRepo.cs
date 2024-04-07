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

        public async Task<dynamic> GetUnitDropsAsync(string keyword, int districtId, int cityId, int provinceId)
        {
            return await fsql.Get(conn_str).Select<UnitInfoView>()
                .Where(u => u.IsDeleted == 0)
                .WhereIf(!string.IsNullOrWhiteSpace(keyword),u=>u.UnitCaption.Contains(keyword))
                .WhereIf(districtId>0, u=>u.DistrictId==districtId)
                .WhereIf(cityId>0,u=>u.CityId==cityId)
                .WhereIf(provinceId > 0, u => u.ProvinceId == provinceId)
                .ToListAsync(u => new
                {
                    value = u.Id,
                    name = u.UnitCaption,
                    code = u.OrganizationCode,
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
