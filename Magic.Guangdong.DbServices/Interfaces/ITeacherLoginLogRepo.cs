using Magic.Guangdong.DbServices.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Interfaces
{
    public interface ITeacherLoginLogRepo:IExaminationRepository<TeacherLoginLog>
    {
        Task<int> InsertTeacherLoginLog(Guid teacherId, string jwt, string exp);
    }
}
