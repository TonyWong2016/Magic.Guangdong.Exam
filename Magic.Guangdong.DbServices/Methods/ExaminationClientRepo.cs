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
using Magic.Guangdong.DbServices.Dtos.Exam.Examinations;
using Mapster;
using Magic.Guangdong.DbServices.Dtos.Report.ReportInfo;

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

        /// <summary>
        /// 该方法适用于非独立报名模式，既依托申报系统和用户中心
        /// 获取报名的考试列表执行之前先进行登录检查
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<List<ReportExamView>?> GetReportExamsForClient(ReportExamDto dto)
        {
            //if (string.IsNullOrEmpty(dto.accountId) || dto.examId == null)
            //    return null;
            
            var examReportView = fsql.Get(conn_str).GetRepository<ReportExamView>();
            var query = examReportView
                .Where(u => u.ReportStatus == 0 && u.ReportStep == 1 && u.ExamType == (int)ExamType.Examination && u.Status==0)
                .WhereIf(dto.examId != null, u => u.ExamId == dto.examId)
                .WhereIf(dto.reportId != null && dto.reportId>0, u => u.ReportId == dto.reportId)
                .WhereIf(!string.IsNullOrWhiteSpace(dto.accountId), u => u.AccountId == dto.accountId)
                .WhereIf(!string.IsNullOrWhiteSpace(dto.groupCode) && dto.groupCode!="auto", u => u.GroupCode == dto.groupCode)
                //.WhereIf(!string.IsNullOrWhiteSpace(dto.idCard), u => u.IdCard == dto.idCard)
                .WhereIf(!string.IsNullOrWhiteSpace(dto.HashIdcard), u => u.HashIdcard == dto.HashIdcard)
                .WhereIf(!string.IsNullOrWhiteSpace(dto.reportNumber), u => u.ReportNumber == dto.reportNumber)
                .WhereIf(dto.examType >-1, u => u.ExamType == dto.examType)
                ;
            return await query.ToListAsync();
        }

        /// <summary>
        /// 该方法适用于独立报名模式，既不依托申报系统和用户中心
        /// 获取该报名方法时，不需要进行登录检查
        /// </summary>
        /// <returns></returns>
        public async Task<List<ExaminationDto>?> GetExamsForClient(OnlyGetExamDto dto)
        {
            if(dto.examId == null && string.IsNullOrEmpty(dto.groupCode))
            {
                return null;
            }
            var examRepo = fsql.Get(conn_str).GetRepository<Examination>();
            return (await examRepo
                 .Where(u => u.IsDeleted == 0 && u.Status == ExamStatus.Enabled && u.IndependentAccess==1 && u.LoginRequired==1)
                 .WhereIf(dto.examId != null, u => u.Id == dto.examId)
                 .WhereIf(!string.IsNullOrEmpty(dto.groupCode), u => u.GroupCode == dto.groupCode)
                 .ToListAsync())
                 .Adapt<List<ExaminationDto>>();
            
        }

        /// <summary>
        /// 报名信息验证，需要主动提供reportid，即需要先登录
        /// 只返回验证结果
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<string> InfoVerificationAuto(ReportExamDto dto)
        {
            if (dto.examId == null || dto.reportId == null)
                return "参数不合法";

            if (string.IsNullOrWhiteSpace(dto.HashIdcard) && string.IsNullOrWhiteSpace(dto.reportNumber))
                return "证件号和准考证号不可以同时为空";
            
            var examReportRepo= fsql.Get(conn_str).GetRepository<ReportExamView>();

            var reportCheckQuery = examReportRepo
                .Where(u => u.Status == 0 && u.ReportStatus == (int)ReportStatus.Succeed && u.ReportStep == (int)ReportStep.Paied)
                .Where(u => u.ExamId == dto.examId && u.ReportId == dto.reportId)
                .Where(u => u.ExamType == (int)ExamType.Examination)                
                ;
                

            if (!await reportCheckQuery.AnyAsync())
            {
                return "没有有效的报名记录";
            }
            
            var examReport = await reportCheckQuery.ToOneAsync();
            //if (examReport.ExamType == (int)ExamType.Practice)
            //{
            //    return "请到练习模式完成后续操作";
            //}
            //if (examReport.ReportStep == (int)ReportStep.Tested)//这个单独判定一下
            //{
            //    return "已参加过考试";
            //}
            //if(examReport.ReportStep != (int)ReportStep.Paied)
            //{
            //    return "没有交费";
            //}
            if(!string.IsNullOrWhiteSpace(dto.HashIdcard) && examReport.HashIdcard.Trim() != dto.HashIdcard.Trim())
            {
                return "身份证号核验失败";
            }

            if(!string.IsNullOrEmpty(dto.reportNumber) && examReport.ReportNumber.Trim() != dto.reportNumber.Trim())
            {
                return "准考证号核验失败";
            }

            if (examReport.StartTime > DateTime.Now || examReport.EndTime < DateTime.Now)
            {
                return "未到在线考试时间，请耐心等待";
            }

            if (examReport.Status != 0 || examReport.ReportStatus!=0 || examReport.ReportStep!=1)
            {
                return "考试已关闭";
            }
            var userAnswerRecordRepo = fsql.Get(conn_str).GetRepository<UserAnswerRecord>();
            var recordCheckQuery = userAnswerRecordRepo
                .Where(u => u.Complated != ExamComplated.Cancle)
                .Where(u => u.ExamId == dto.examId && u.ReportId == dto.reportId)
                .Where(u => u.Stage == dto.stage)
                .Where(u => u.IsDeleted == 0)
                //.WhereIf(!string.IsNullOrWhiteSpace(dto.idCard), u => u.IdNumber == dto.idCard)
                //.WhereIf(!string.IsNullOrWhiteSpace(dto.reportNumber), u => u.IdNumber == dto.reportNumber)
                ;
            if(await recordCheckQuery.AnyAsync())
            {
                return "已参加过考试|"+ (await recordCheckQuery.ToOneAsync()).Id.ToString();
            }
            return "ok";
        }

        /// <summary>
        /// 报名信息验证，不需要主动提供reportid，不需要先登录
        /// 返回报名信息，方便后续操作，即补全没有登录的信息
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<ReturnVerifyResult> InfoVerificationByNumber(OnlyGetExamDto dto)
        {
            ReturnVerifyResult returnVerifyResult = new ReturnVerifyResult();
            //到这里，就必须要提供examid了
            if (dto.examId == null)
            {
                returnVerifyResult.verifyMsg = "没有可参加的考试信息";
                returnVerifyResult.verifyCode = -1;
                return returnVerifyResult;
            }

            if (string.IsNullOrWhiteSpace(dto.hashIdcard) && string.IsNullOrWhiteSpace(dto.reportNumber))
            {
                returnVerifyResult.verifyMsg = "证件号和准考证号不可以同时为空";
                returnVerifyResult.verifyCode = -1;
                return returnVerifyResult;
            }
            var examReportRepo = fsql.Get(conn_str).GetRepository<ReportExamView>();

            var reportCheckQuery = examReportRepo
                .Where(u => u.Status == 0 && u.ReportStatus == (int)ReportStatus.Succeed && u.ReportStep == (int)ReportStep.Paied)
                .Where(u => u.ExamId == dto.examId)
                .Where(u => u.ExamType == (int)ExamType.Examination)
                .WhereIf(!string.IsNullOrEmpty(dto.hashIdcard), u => u.HashIdcard == dto.reportNumber)
                .WhereIf(!string.IsNullOrEmpty(dto.reportNumber), u => u.ReportNumber == dto.reportNumber)
                ;

            if (!await reportCheckQuery.AnyAsync())
            {
                returnVerifyResult.verifyMsg = "没有找到您的报名记录，请检查提交的身份证件号码是否正确";
                returnVerifyResult.verifyCode = -1;
                return returnVerifyResult;
            }
            var reportInfo = await reportCheckQuery.FirstAsync();
            var recordRepo = fsql.Get(conn_str).GetRepository<UserAnswerRecord>();
            var recordCheckQuery = recordRepo
                .Where(u => u.Complated != ExamComplated.Cancle)
                .Where(u => u.Stage == 0)
                .Where(u => u.IsDeleted == 0)
                .Where(u => u.ExamId == dto.examId && u.ReportId == reportInfo.ReportId);
          
            returnVerifyResult.reportId = reportInfo.ReportId;
            returnVerifyResult.verifyReportInfo = new VerifyReportInfo()
            {
                reportNumber = reportInfo.ReportNumber,
                email = reportInfo.Email,
                mobile = reportInfo.SecurityMobile,
                secruityIdCard = reportInfo.SecurityIdCard,
                name = reportInfo.Name,
            };
            if (!await recordCheckQuery.AnyAsync())
            {
                returnVerifyResult.verifyMsg = "ok";
                returnVerifyResult.verifyCode = 0;
                returnVerifyResult.recordId = 0;
                return returnVerifyResult;
            }

            var record = await recordCheckQuery.FirstAsync();
            //return "已参加过考试|" + (await recordCheckQuery.ToOneAsync()).Id.ToString();
            returnVerifyResult.verifyMsg = "已经参加过当前考试";
            returnVerifyResult.verifyCode = 1;
            
            returnVerifyResult.complated = record.Complated;
            returnVerifyResult.recordId = record.Id;
            return returnVerifyResult;
        }


        public async Task<ReportExamView> GetReportExamView(long reportId)
        {
            return await fsql.Get(conn_str).Select<ReportExamView>()
                .Where(u => u.ReportId == reportId)
                .ToOneAsync();
                
        }
    }
}
