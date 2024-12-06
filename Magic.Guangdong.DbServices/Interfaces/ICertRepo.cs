using Magic.Guangdong.DbServices.Dtos.Cert;
using Magic.Guangdong.DbServices.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Interfaces
{
    public  interface ICertRepo: IExaminationRepository<Cert>
    {
        Task<int> InsertCertBatch(List<Cert> certs);

        Task<List<CertDto>> GetCertRecordsForExcel(string whereJsonStr);

        //Task<int> BulkRemoveCerts(string whereJsonStr);

        //Task<int> BulkRemoveCerts(long[] ids);

        Task<int> BulkUpdateCerts(string whereJsonStr, int status, int isDeleted = 0);
        Task<int> BulkUpdateCerts(long[] certIds, int status, int isDeleted = 0);
    }
}
