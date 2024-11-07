using MassTransit;
using Newtonsoft.Json;

namespace Magic.Guangdong.Exam.Areas.WebApi.Models
{
    public class ExamApiDto
    {
        public Guid Id { get; set; } = NewId.NextGuid();
        public string Title { get; set; }

        public string Description { get; set; }

        public long AssociationId { get; set; }

        public string AssociationTitle { get; set; }

        public string Remark { get; set; } = "接口创建";

        public long St { get; set; }

        public long Et { get; set; }

        public DateTime StartTime
        {
            get
            {
                return Assistant.Utils.TimeStampToDateTime(St);
            }
        }

        public DateTime EndTime
        {
            get
            {
                return Assistant.Utils.TimeStampToDateTime(Et);
            }
        }

        public double BaseScore { get; set; }

        public double BaseDuration { get; set; }

        public int IsStrict { get; set; } = 0;

        public decimal Expenses {  get; set; } = decimal.Zero;

        public int Quota { get; set; } = 0;

        public string Address { get; set; } = "线上";

        public int Audit { get; set; } = 2;

        public Guid AttachmentId { get; set; } = Guid.Empty;

        public int MarkStatus {  get; set; } = 0;

        /// <summary>
		/// 是否允许独立访问
		/// </summary>
        public int? IndependentAccess { get; set; } = 0;

        /// <summary>
        /// 绑定的评分标准
        /// </summary>
        public long? SchemeId { get; set; } = 0;
    }
}
