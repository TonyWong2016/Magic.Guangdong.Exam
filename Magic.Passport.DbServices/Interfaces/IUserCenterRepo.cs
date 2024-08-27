using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Passport.DbServices.Interfaces
{
    public interface IUserCenterRepo : IPassportRepository<Entities.Viw_UserMain>
    {
        dynamic getUserList(string keyword, string provinceID, int pageIndex, int pageSize, out long total, int type=0);

        Task<Entities.UserModel> getUser(int userId);
    }
}
