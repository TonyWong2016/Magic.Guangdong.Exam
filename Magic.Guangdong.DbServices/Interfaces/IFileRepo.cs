using Magic.Guangdong.DbServices.Dtos.Material;

namespace Magic.Guangdong.DbServices.Interfaces
{
    public interface IFileRepo : IExaminationRepository<Entities.File>
    {
        Task<List<MaterialResponseDto>> GetMaterials(string connId, string connName);    }
}
