using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Methods
{
    internal class QuestionTagRepo : ExaminationRepository<QuestionTag>, IQuestionTagRepo
    {
        private IdleBus<IFreeSql> fsql;
        const string conn_str = "db_exam";
        public QuestionTagRepo(IdleBus<IFreeSql> fsql)
            : base(fsql)
        {
            this.fsql = fsql;
        }

    }
}
