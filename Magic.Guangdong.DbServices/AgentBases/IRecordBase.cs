using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.AgentBases
{
    public interface IRecordBase : IExaminationRepository<UserAnswerRecord>
    {
    }
}
