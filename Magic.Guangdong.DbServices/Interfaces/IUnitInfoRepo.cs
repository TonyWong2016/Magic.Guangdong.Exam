using Magic.Guangdong.DbServices.Dtos;
using Magic.Guangdong.DbServices.Entities;

namespace Magic.Guangdong.DbServices.Interfaces
{
    public interface IUnitInfoRepo : IExaminationRepository<UnitInfo>
    {
        dynamic GetUnitInfos(PageDto dto, out long total);

        Task<dynamic> GetUnitDropsAsync(string keyword, int unitType, int provinceId, int cityId, int districtId, int limit=1000);

        Task<UnitInfoView> GetUnitInfoView(int id);

        Task<bool> SyncUnitInfos(List<UnitInfo> unitInfos);
    }
}
