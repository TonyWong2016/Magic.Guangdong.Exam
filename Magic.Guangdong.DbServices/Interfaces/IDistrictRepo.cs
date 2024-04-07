using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Methods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Interfaces
{
    public interface IDistrictRepo : IExaminationRepository<District>
    {
        dynamic GetDictricts(Dtos.PageDto dto, out long total);

        Task<dynamic> GetDistrictDropsAsync(int cityId, int provinceId = 0);

        Task<dynamic> GetDistrictView(int Id);
    }
}
