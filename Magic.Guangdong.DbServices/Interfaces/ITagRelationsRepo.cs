using Magic.Guangdong.DbServices.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Interfaces
{
    public interface ITagRelationsRepo : IExaminationRepository<TagRelations>
    {
        Task<dynamic> GetPaperTags(string associationId);

        Task<int> AddTagRelations(List<TagRelations> tags);
    }
}
