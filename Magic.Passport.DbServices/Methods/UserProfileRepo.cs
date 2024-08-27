
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Passport.DbServices.Methods
{
    public class UserProfileRepo : PassportRepository<Entities.User_Profile>, Interfaces.IUserProfileRepo
    {
        private IdleBus<IFreeSql> fsql;
        const string conn_str = "db_passport";
        public UserProfileRepo(IdleBus<IFreeSql> fsql) : base(fsql)
        {
            this.fsql = fsql;
        }
    }
}
