using Magic.Guangdong.Assistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace Magic.Guangdong.DbServices.Dtos.System.Tags
{
    public class TagDto
    {
        public long Id { get; set; } = YitIdHelper.NextId();
        public string Title { get; set; }

        public string AssociationId { get; set; }

        public string TableName { get; set; }

        public string OriginalId { get; set; }

        public string HashRelation
        {
            get
            {
                return Security.GenerateMD5Hash(Id.ToString() + Title + AssociationId + TableName+ OriginalId);

            }
        }
    }

    public class TagPaperDto
    {
        public string Tags { get; set; }
       
        public Guid[] PaperIds { get; set; }

    }
}
