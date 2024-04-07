using Magic.Guangdong.DbServices.Dtos;
using Magic.Guangdong.DbServices.Entities;

namespace Magic.Guangdong.DbServices.Interfaces
{
    public interface IUnitInfoRepo : IExaminationRepository<UnitInfo>
    {
        dynamic GetUnitInfos(PageDto dto, out long total);

        Task<dynamic> GetUnitDropsAsync(string keyword, int districtId, int cityId, int provinceId);

        Task<UnitInfoView> GetUnitInfoView(int id);
    }
}
