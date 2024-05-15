using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Methods
{
    internal class ReportExamViewRepo : ExaminationRepository<ReportExamView>, IReportExamViewRepo
    {
        private IdleBus<IFreeSql> fsql;
        const string conn_str = "db_exam";
        public ReportExamViewRepo(IdleBus<IFreeSql> fsql)
            : base(fsql)
        {
            this.fsql = fsql;
        }

        public async Task<dynamic> GetDashboardData()
        {
            decimal orderAmountTotal = await fsql.Get(conn_str).Select<Order>()
                .Where(u => u.Status == 0 && u.IsDeleted == 0)
                .SumAsync(u => u.Expenses);
            long reportNumTotal = await fsql.Get(conn_str).Select<ReportInfo>()
                .Where(u=>u.IsDeleted==0)
                .CountAsync();
            long examNumTotal = await fsql.Get(conn_str).Select<Examination>()
                .Where(u => u.IsDeleted == 0)
                .CountAsync();

            long recordTotal = await fsql.Get(conn_str).Select<UserAnswerRecord>()
                .Where(u => u.IsDeleted == 0)
                .CountAsync();

            long certTotal = await fsql.Get(conn_str).Select<Cert>()
                .Where(u => u.IsDeleted == 0)
                .CountAsync();

            long unMarkedTotal = await fsql.Get(conn_str).Select<UserAnswerRecord>()
                .Where(u => u.IsDeleted == 0 && u.Marked!=ExamMarked.All)
                .CountAsync();

            
            string sql = $"SELECT CONVERT(date, ReportTime) AS reportTime," +
                $" COUNT(*) AS reportCnt " +
                $" FROM " +
                $" ReportExamView " +
                $" WHERE " +
                $" examType=0 and ReportTime >= DATEADD(day, -30, GETDATE()) " +
                $" GROUP BY CONVERT(date, ReportTime) " +
                $" ORDER BY CONVERT(date, ReportTime);";
            var reportExamDateLine = await fsql.Get(conn_str).Ado.QueryAsync<ReportNumDto>(sql);
            var reportPracticeDateLine = await fsql.Get(conn_str).Ado.QueryAsync<ReportNumDto>(sql.Replace("examType=0", "examType=1"));

            string sqlOrder = $"SELECT" +
                $" CONVERT(date, CreatedAt) AS CreatedAt," +
                $" sum(Expenses) AS orderAmount" +
                $" FROM  [Magic.Guangdong.Exam].[dbo].[Order]" +
                $" WHERE " +
                $" CreatedAt >= DATEADD(day, -30, GETDATE()) and Status=0 and IsDeleted=0" +
                $" GROUP BY" +
                $" CONVERT(date, CreatedAt) " +
                $" ORDER BY" +
                $" CONVERT(date, CreatedAt)";
            var orderDateLine = await fsql.Get(conn_str).Ado.QueryAsync<OrderNumDto>(sqlOrder);
        
            return new { orderAmountTotal, reportNumTotal, unMarkedTotal, examNumTotal, recordTotal, certTotal, reportExamDateLine, reportPracticeDateLine, orderDateLine };
        }
    }

    public class ReportNumDto
    {
        public int reportCnt { get; set; }

        public DateTime reportTime { get; set; }

        public string reportTimeFormat { get { return reportTime.ToString("yyyy-MM-dd"); } }
    }

    public class OrderNumDto
    {
        public decimal orderAmount { get; set; }

        public DateTime createdAt { get; set; }

        public string createdAtFormat { get { return createdAt.ToString("yyyy-MM-dd"); } }
    }
}
