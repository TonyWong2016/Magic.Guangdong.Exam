using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Dtos.Cert
{
    public class ImportDto
    {
        /// <summary>
        /// 路径
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// 模板id
        /// </summary>
        public long TemplateId { get; set; }


        /// <summary>
        /// 证书名称
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 编号长度
        /// </summary>
        public int CertNumLength { get; set; }
        /// <summary>
        /// 参数类型
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// 证书状态
        /// </summary>
        public int Status { get; set; } = 0;
        /// <summary>
        /// 活动id
        /// </summary>
        public long ActivityId { get; set; } = 0;
        /// <summary>
        /// 考试id
        /// </summary>
        public Guid ExamId { get; set; } = Guid.Empty;
        /// <summary>
        /// 编号前缀
        /// </summary>
        public string CertNumPrefix { get; set; } = "";
        /// <summary>
        /// 是否覆盖
        /// </summary>
        public int IsOverwrite { get; set; } = 0;
        /// <summary>
        /// 证书类型
        /// </summary>
        public int CertType { get; set; } = 0;

        /// <summary>
        /// 当certType==1时生效
        /// 1-表示不验证任何颁发条件，直接颁发证书
        /// 0-表示按正常流程颁发证书
        /// </summary>
        public int IsForce { get; set; } = 0;

        public string AccountId { get; set; } = "";
    }
}
