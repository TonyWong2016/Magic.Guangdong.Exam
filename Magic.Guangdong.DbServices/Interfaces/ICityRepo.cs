using Magic.Guangdong.DbServices.Entities;

namespace Magic.Guangdong.DbServices.Interfaces
{
    public interface ICityRepo : IExaminationRepository<City>
    {
        dynamic GetCities(Dtos.PageDto dto, out long total);

        Task<dynamic> GetCitiyDropsAsync(int provinceId);
    }
}
