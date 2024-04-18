using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;
using FreeSql.Internal.Model;
using MassTransit;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Magic.Guangdong.DbServices.Dtos;
using NPOI.POIFS.Dev;
using Magic.Guangdong.DbServices.Dtos.Report.Exams;
using Magicodes.ExporterAndImporter.Core.Extension;
using EasyCaching.CSRedis;

namespace Magic.Guangdong.DbServices.Methods
{
    internal class ExaminationClientRepo : ExaminationRepository<Examination>, IExaminationClientRepo
    {
        private IdleBus<IFreeSql> fsql;
        const string conn_str = "db_exam";
        public ExaminationClientRepo(IdleBus<IFreeSql> fsql)
            : base(fsql)
        {
            this.fsql = fsql;
        }
        
        //获取报名的考试列表
        //执行之前先调用AnyReportExamsForClient进行检查
        public async Task<List<ReportExamView>> GetReportExamsForClient(ReportExamDto dto)
        {
            if (string.IsNullOrEmpty(dto.accountId) || dto.examId == null)
                return null;
            var examReportView = fsql.Get(conn_str).GetRepository<ReportExamView>();
            var query = examReportView
                .Where(u=>u.ReportStatus==0 && u.ReportStep==1)
                .WhereIf(dto.examId != null, u => u.ExamId == dto.examId)
                .WhereIf(dto.reportId != null, u => u.ReportId == dto.reportId)
                .WhereIf(!string.IsNullOrWhiteSpace(dto.accountId), u => u.AccountId == dto.accountId)
                .WhereIf(!string.IsNullOrWhiteSpace(dto.groupCode), u => u.GroupCode == dto.groupCode)
                .WhereIf(!string.IsNullOrWhiteSpace(dto.idCard), u => u.IdCard == dto.idCard)
                .WhereIf(!string.IsNullOrWhiteSpace(dto.reportNumber), u => u.ReportNumber == dto.reportNumber)
                ;
            return await query.ToListAsync();
        }

        public async Task<string> InfoVerificationAuto(ReportExamDto dto)
        {
            if (dto.examId == null || dto.reportId == null)
                return "参数不合法";
            if (string.IsNullOrWhiteSpace(dto.idCard) && string.IsNullOrWhiteSpace(dto.reportNumber))
                return "证件号和准考证号不可以同时为空";
            
            var examReportRepo= fsql.Get(conn_str).GetRepository<ReportExamView>();

            var reportCheckQuery = examReportRepo
                .Where(u => u.Status == 0 && u.ReportStatus == (int)ReportStatus.Succeed && u.ReportStep == (int)ReportStep.Paied)
                .Where(u => u.ExamId == dto.examId && u.ReportId == dto.reportId);
                

            if (!await reportCheckQuery.AnyAsync())
            {
                return "没有报名考试";
            }
            var examReport = await reportCheckQuery.ToOneAsync();

            if(!string.IsNullOrWhiteSpace(dto.idCard) && examReport.IdCard.Trim() != dto.idCard.Trim())
            {
                return "身份证号核验失败";
            }

            if(!string.IsNullOrEmpty(dto.reportNumber) && examReport.ReportNumber.Trim() != dto.reportNumber.Trim())
            {
                return "准考证号核验失败";
            }

            if (examReport.StartTime > DateTime.Now || examReport.EndTime < DateTime.Now)
            {
                return "未在考试要求的时间范围内";
            }

            if (examReport.Status != 0 || examReport.ReportStatus!=0 || examReport.ReportStep!=1)
            {
                return "考试已关闭";
            }
            var userAnswerRecordRepo = fsql.Get(conn_str).GetRepository<UserAnswerRecord>();
            var recordCheckQuery = userAnswerRecordRepo
                .Where(u => u.Complated != ExamComplated.Cancle)
                .Where(u => u.ExamId == dto.examId && u.ReportId == dto.reportId.ToString())
                //.WhereIf(!string.IsNullOrWhiteSpace(dto.idCard), u => u.IdNumber == dto.idCard)
                //.WhereIf(!string.IsNullOrWhiteSpace(dto.reportNumber), u => u.IdNumber == dto.reportNumber)
                ;
            if(await recordCheckQuery.AnyAsync())
            {
                return "已经提交过该考试了";
            }
            return "ok";
        }

    }
}
