using Magic.Guangdong.Assistant;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Methods
{
    internal class AdminActionLogRepo : ExaminationRepository<AdminLoginLog>, IAdminLoginLogRepo
    {
        private IdleBus<IFreeSql> fsql;
        const string conn_str = "db_exam";
        public AdminActionLogRepo(IdleBus<IFreeSql> fsql)
            : base(fsql)
        {
            this.fsql = fsql;
        }

        public async Task<int> InsertLoginLog(Guid adminId,string jwt,string exp)
        {
            using(var uow = fsql.Get(conn_str).CreateUnitOfWork())
            {
                try
                {
                    var adminLoginLogRepo = fsql.Get(conn_str).GetRepository<AdminLoginLog>();
                    string tokenHash = Security.GenerateMD5Hash(jwt);
                    if(adminLoginLogRepo.Where(u=>u.AdminId==adminId && u.TokenHash== tokenHash).Any())
                    {
                        return -1;
                    }
                    await adminLoginLogRepo.InsertAsync(new AdminLoginLog() { AdminId = adminId, TokenVersion = exp, TokenHash = tokenHash });
                    var adminRepo = fsql.Get(conn_str).GetRepository<Admin>();
                   
                    var admin = await adminRepo.Where(a => a.Id == adminId).FirstAsync();
                    //var claims = JwtService.ValidateJwt(jwt);
                    admin.UpdatedAt= DateTime.Now;
                    admin.Version = exp;
                    await adminRepo.UpdateAsync(admin);
                    uow.Commit();
                    return 1;
                }
                catch (Exception ex)
                {
                    uow.Rollback();
                    Logger.Error(ex.Message);
                    return -1;
                }
            }
        }
    }
}
