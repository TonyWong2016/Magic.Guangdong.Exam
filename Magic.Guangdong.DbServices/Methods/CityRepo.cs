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
    internal class CityRepo : ExaminationRepository<City>, ICityRepo
    {
        private IdleBus<IFreeSql> fsql;
        const string conn_str = "db_exam";
        public CityRepo(IdleBus<IFreeSql> fsql)
            : base(fsql)
        {
            this.fsql = fsql;
        }

        public dynamic GetCities(Dtos.PageDto dto,out long total)
        {
            //var cityViewRepo = fsql.Get(conn_str).GetRepository<CityView>();
            if (string.IsNullOrEmpty(dto.whereJsonStr))
            {
                return fsql.Get(conn_str).Select<CityView>()
                .OrderByPropertyName(dto.orderby, dto.isAsc)
                .Count(out total)
                .Page(dto.pageindex, dto.pagesize)
                .ToList();
            }
            DynamicFilterInfo dyfilter = JsonConvert.DeserializeObject<DynamicFilterInfo>(dto.whereJsonStr);
            return fsql.Get(conn_str).Select<CityView>()
                .WhereDynamicFilter(dyfilter)
                .OrderByPropertyName(dto.orderby,dto.isAsc)
                .Count(out total)
                .Page(dto.pageindex, dto.pagesize)                
                .ToList();
            
        }

        public async Task<dynamic> GetCitiyDropsAsync(int provinceId)
        {
            return await fsql.Get(conn_str).Select<CityView>()
                .Where(u=>u.IsDeleted==0)
                .WhereIf(provinceId > 0, u => u.ProvinceId == provinceId)
                .ToListAsync(u => new
                {
                    value = u.Id,
                    name = u.CityName,
                    text = u.CityName,
                    province = u.ProvinceShortName
                });
        }
    }
}
