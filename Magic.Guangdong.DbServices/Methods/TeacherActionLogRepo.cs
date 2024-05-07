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
    internal class TeacherActionLogRepo : ExaminationRepository<TeacherLoginLog>, ITeacherLoginLogRepo
    {
        private IdleBus<IFreeSql> fsql;
        const string conn_str = "db_exam";
        public TeacherActionLogRepo(IdleBus<IFreeSql> fsql)
            : base(fsql)
        {
            this.fsql = fsql;
        }

        public async Task<int> InsertTeacherLoginLog(Guid teacherId,string jwt,string exp)
        {
            using(var uow = fsql.Get(conn_str).CreateUnitOfWork())
            {
                try
                {
                    var teacherLoginLogRepo = fsql.Get(conn_str).GetRepository<TeacherLoginLog>();
                    string tokenHash = Security.GenerateMD5Hash(jwt);
                    if(teacherLoginLogRepo.Where(u=>u.TeacherId==teacherId && u.TokenHash== tokenHash).Any())
                    {
                        return -1;
                    }
                    await teacherLoginLogRepo.InsertAsync(new TeacherLoginLog()
                    {
                        TeacherId = teacherId,
                        TokenVersion = exp,
                        TokenHash = tokenHash,
                        LoginTime = DateTime.Now
                    });
                    var teacherRepo = fsql.Get(conn_str).GetRepository<Teacher>();
                   
                    var teacher = await teacherRepo.Where(a => a.Id == teacherId).FirstAsync();
                    teacher.UpdatedAt= DateTime.Now;
                    teacher.Version = exp;
                    await teacherRepo.UpdateAsync(teacher);
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
