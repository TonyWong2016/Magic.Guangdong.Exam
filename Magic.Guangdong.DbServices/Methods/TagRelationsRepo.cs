using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Methods
{
    internal class TagRelationsRepo : ExaminationRepository<TagRelations>, ITagRelationsRepo
    {
        private IdleBus<IFreeSql> fsql;
        const string conn_str = "db_exam";
        public TagRelationsRepo(IdleBus<IFreeSql> fsql)
            : base(fsql)
        {
            this.fsql = fsql;
        }

        public async Task<dynamic> GetPaperTags(string associationId)
        {
            return await fsql.Get(conn_str).Select<Tags, TagRelations>()
                .LeftJoin((a, b) => a.Id == b.TagId)
                .Where((a, b) => b.AssociationId == associationId
                && a.IsDeleted == 0
                && b.IsDeleted == 0)
                .ToListAsync((a, b) => new
                {
                    a.Title,
                    b.TagId
                });
        }
    }
}
