using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Methods
{
    internal class TeacherRecordScoringRepo:ExaminationRepository<TeacherRecordScoring>, ITeacherRecordScoringRepo
    {
        private IdleBus<IFreeSql> fsql;
        const string conn_str = "db_exam";
        public TeacherRecordScoringRepo(IdleBus<IFreeSql> fsql)
            : base(fsql)
        {
            this.fsql = fsql;
        }

        public async Task<TeacherRecordScoring> GetLastScoreDetail(long submitRecoredId)
        {
            if(!await getAnyAsync(x => x.SubmitRecordId == submitRecoredId))
            {
                return new TeacherRecordScoring() { QuestionId = -1, RecordId = -1, SubmitRecordId = -1 };
            }
            
            return await fsql.Get(conn_str).Select<TeacherRecordScoring>()
                .Where(x => x.SubmitRecordId == submitRecoredId)
                .OrderByDescending(x => x.Id)
                .FirstAsync();
        }
    }
}
