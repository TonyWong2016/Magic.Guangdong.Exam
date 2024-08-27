using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Passport.DbServices.Methods
{
    public class UserCenterRepo : PassportRepository<Entities.Viw_UserMain>,Interfaces.IUserCenterRepo
    {

        private IdleBus<IFreeSql> fsql;
        const string conn_str = "db_passport";

        public UserCenterRepo(IdleBus<IFreeSql> fsql) : base(fsql)
        {
            this.fsql = fsql;
        }

        public dynamic getUserList(string keyword, string provinceID,int pageIndex,int pageSize,out long total,int type)
        {            
            string whereStr = "";
            if (!string.IsNullOrEmpty(provinceID))
            {
                whereStr += $" and (SchoolProvinceID={provinceID})";
            }
            if (!string.IsNullOrEmpty(keyword))//效率极低，从老版oa复刻过来的，先这样吧，下来再调，参数已经留好了，就用type来控制下检索条件
            {
                whereStr += $" and (Accounts like '%{keyword}%' or UserName like '%{keyword}%' or IdentityNo like '%{keyword}%'  or Name like '%{keyword}%') ";
            }

            string sql = "select * from Viw_UserMain where 1=1 " + whereStr ;
            return fsql.Get(conn_str).Select<Entities.Viw_UserMain>()
                .WithSql(sql)
                .Count(out total)
                .OrderByDescending(u=>u.IdentityNo)
                .Page(pageIndex, pageSize)
                .ToList();
        }

        /// <summary>
        /// 获取用户信息（用户中心那两个account和main表的设计，真尼玛二逼我靠）
        /// 这里的逻辑是，先判定main表里的username是不是为空，不为空则直接返回
        /// 为空则再次到account表里查account值，不为空则返回account值
        /// 两者都为空则将username的值设定成和userid一样
        /// 但凡设计有点逻辑，都不至于这么弄
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<Entities.UserModel> getUser(int userId)
        {
            //var userAccount = await fsql.Get(conn_str).Select<Models.User_Account>().Where(u => u.UID == userId).FirstAsync();
            //var userMain = await fsql.Get(conn_str).Select<Models.User_Main>().Where(u => u.UID == userId).FirstAsync();
            var ret = new Entities.UserModel();

            ret.userId = userId;
            if (await fsql.Get(conn_str).Select<Entities.User_Main>().AnyAsync(u => u.UID == userId))
            {
                var userMain = await fsql.Get(conn_str).Select<Entities.User_Main>().Where(u => u.UID == userId).FirstAsync();
                if (!string.IsNullOrEmpty(userMain.UserName))
                {
                    ret.userName = userMain.UserName;
                    return ret;
                }
            }
            if (await fsql.Get(conn_str).Select<Entities.User_Account>().AnyAsync(u => u.UID == userId))
            {
                var userAccount = await fsql.Get(conn_str).Select<Entities.User_Account>().Where(u => u.UID == userId).FirstAsync();
                if (!string.IsNullOrEmpty(userAccount.Account))
                    ret.userName = userAccount.Account;
                else
                    ret.userName = ret.userId.ToString();
            }
            return ret;
        }

        
    }
}
