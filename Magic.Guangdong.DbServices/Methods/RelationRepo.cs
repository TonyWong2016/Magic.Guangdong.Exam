using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Methods
{
    internal class RelationRepo : ExaminationRepository<Relation>, IRelationRepo
    {
        private IdleBus<IFreeSql> fsql;
        const string conn_str = "db_exam";
        public RelationRepo(IdleBus<IFreeSql> fsql)
            : base(fsql)
        {
            this.fsql = fsql;
        }

        public async Task<bool> AutoShardingRelation(Guid? examId)
        {

            var relationRepo = fsql.Get(conn_str).GetRepository<Relation>();
            var lastRelation = await relationRepo.Where(u => u.IsDeleted == 0).OrderByDescending(u => u.CreatedBy).ToOneAsync();
            var examRepo = fsql.Get(conn_str).GetRepository<Examination>();

            if (!examId.Equals(Guid.Empty) && !await examRepo.Where(u => u.Id == examId).AnyAsync())
            {
                return false;
            }
            //var exam = await examRepo
            //    .WhereIf(examId == null, u => u.IsDeleted == 0)
            //    .WhereIf(examId != null, u => u.Id == examId)
            //    .OrderByDescending(u => u.CreatedBy)
            //    .ToOneAsync();
            if (DateTime.Now.Year > lastRelation.CreatedAt.Year)
            {
                //数据迁移到年份表
                string sql = $"select * into Relation_{lastRelation.CreatedAt.Year} from Relation;";
                //清空现有表
                //sql += "delete from Relation";
                sql += $"update Relation set Remark+=',分表标记，可清除',IsDeleted=1 where CreatedAt<'{DateTime.Now.Year}-01-01'";
                await fsql.Get(conn_str).Ado.ExecuteNonQueryAsync(sql);
                return true;
            }
            return false;
        }
    }
}
