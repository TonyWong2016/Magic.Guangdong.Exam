using Magic.Guangdong.DbServices.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Interfaces
{
    public interface ICertTemplateRepo : IExaminationRepository<CertTemplate>
    {
        Task<bool> CloneTemplate(long templateId, string adminId);
    }
}
