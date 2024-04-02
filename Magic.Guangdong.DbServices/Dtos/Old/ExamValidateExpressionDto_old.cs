using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Dtos.Old
{
    public class ExamValidateExpressionDto_old
    {
        public Guid ExamId { get; set; }

        public Guid[] PaperIds { get; set; }

        public string ColumnId { get; set; }

        public string Expression { get; set; }

        public string AdminId { get; set; } = "";
    }
}
