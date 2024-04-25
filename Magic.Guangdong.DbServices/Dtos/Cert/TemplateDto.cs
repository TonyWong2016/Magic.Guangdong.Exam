using Magic.Guangdong.DbServices.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace Magic.Guangdong.DbServices.Dtos.Cert
{
    public class TemplateDto
    {
        public long Id { get; set; } = YitIdHelper.NextId();

        public string ConfigJsonStrForImg { get; set; }

        public string ConfigJsonStrForPdf { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string CreatedBy { get; set; }

        public string Remark { get; set; }

        public string Title { get; set; }

        public string Url { get; set; } = "";

        public CertTemplateType Type { get; set; }

        public CertTemplateStatus Status { get; set; }

        public long ActivityId { get; set; } = 0;

        public CertTemplateLockStatus IsLock { get; set; }

    }
}
