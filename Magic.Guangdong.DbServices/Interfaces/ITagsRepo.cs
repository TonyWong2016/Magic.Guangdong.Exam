using Magic.Guangdong.DbServices.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Interfaces
{
    public interface ITagsRepo : IExaminationRepository<Tags>
    {
        Task<bool> CreateTagRelations(List<Dtos.System.Tags.TagDto> dtos);
        Task<int> RemoveRelationsByTagId(long id, bool force = false);
    }
}
