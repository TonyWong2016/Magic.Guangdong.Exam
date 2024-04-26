using Magic.Guangdong.Assistant.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.Assistant.IService
{

    public interface ISixLaborHelper
    {
        Task<string> MakeCertPic(string savePath, CertTemplateDto model, string filename);
    }
}
