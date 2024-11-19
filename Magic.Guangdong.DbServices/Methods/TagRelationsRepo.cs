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

        public async Task<int> AddTagRelations(List<TagRelations> tags)
        {
            if (tags.Count==0)
            {
                return 0;
            }
            var hashs = tags.Select(u=>u.HashRelation).ToList();
            var exitHashs = await fsql.Get(conn_str).Select<TagRelations>()
                .Where(u => hashs.Contains(u.HashRelation)).ToListAsync(u=>u.HashRelation);
            var addTags = tags.Where(u => !exitHashs.Contains(u.HashRelation)).ToList();
            if (addTags.Count > 0)
            {
               return await addItemsAsync(addTags);
            }
            return 0;
        }
    
    }
}
