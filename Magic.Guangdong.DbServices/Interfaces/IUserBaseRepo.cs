using Magic.Guangdong.DbServices.Entities;

namespace Magic.Guangdong.DbServices.Interfaces
{
    public interface IUserBaseRepo : IExaminationRepository<UserBase>
    {
        Task<bool> InsertUserBaseSecurity(UserBase user);
    }
}
