using Magic.Guangdong.Assistant;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace Magic.Guangdong.DbServices.Methods
{
    internal class CertTemplateRepo: ExaminationRepository<CertTemplate>, ICertTemplateRepo
    {
        private IdleBus<IFreeSql> fsql;
        const string conn_str = "db_exam";
        public CertTemplateRepo(IdleBus<IFreeSql> fsql)
            : base(fsql)
        {
            this.fsql = fsql;
        }

        public async Task<bool> CloneTemplate(long templateId, string adminId)
        {
            var templateRepo = fsql.Get(conn_str).GetRepository<CertTemplate>();

            if (!await templateRepo.Where(x => x.Id == templateId).AnyAsync())
            {
                return false;
            }

            var template = await templateRepo.Where(u => u.Id == templateId)
                .ToOneAsync();
            template.Id = YitIdHelper.NextId();
            template.Remark = "快速克隆";
            template.Title = $"克隆_{template.Title}_{Utils.GenerateRandomCodePro(4)}";
            template.UpdatedAt = DateTime.Now;
            template.CreatedAt = DateTime.Now;
            template.CreatedBy = adminId;
            await templateRepo.InsertAsync(template);
            return true;
        }
    }
}
