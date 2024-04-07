using FreeSql.Internal.Model;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Methods
{
    internal class DistrictRepo : ExaminationRepository<District>, IDistrictRepo
    {
        private IdleBus<IFreeSql> fsql;
        const string conn_str = "db_exam";
        public DistrictRepo(IdleBus<IFreeSql> fsql)
            : base(fsql)
        {
            this.fsql = fsql;
        }

        public dynamic GetDictricts(Dtos.PageDto dto, out long total)
        {
            //var cityViewRepo = fsql.Get(conn_str).GetRepository<CityView>();
            if (string.IsNullOrEmpty(dto.whereJsonStr))
            {
                return fsql.Get(conn_str).Select<DistrictView>()
                .OrderByPropertyName(dto.orderby, dto.isAsc)
                .Count(out total)
                .Page(dto.pageindex, dto.pagesize)
                .ToList();
            }
            DynamicFilterInfo dyfilter = JsonConvert.DeserializeObject<DynamicFilterInfo>(dto.whereJsonStr);
            return fsql.Get(conn_str).Select<DistrictView>()
                .WhereDynamicFilter(dyfilter)
                .OrderByPropertyName(dto.orderby, dto.isAsc)
                .Count(out total)
                .Page(dto.pageindex, dto.pagesize)
                .ToList();

        }

        public async Task<dynamic> GetDistrictDropsAsync(int cityId, int provinceId = 0)
        {
            return await fsql.Get(conn_str).Select<DistrictView>()
                .WhereIf(cityId > 0, u => u.CityId == cityId)
                .WhereIf(provinceId > 0, u => u.ProvinceId == provinceId)
                .ToListAsync(u => new
                {
                    value = u.Id,
                    name = u.DistrictName,
                    text = u.DistrictName,
                    city = u.CityName,
                    province = u.ProvinceShortName
                });
        }

        public async Task<dynamic> GetDistrictView(int Id)
        {
            return await fsql.Get(conn_str).Select<DistrictView>()
                .Where(u => u.Id == Id)
                .ToOneAsync();
        }
    
    }
}
