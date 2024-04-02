using Magic.Guangdong.DbServices.Dtos.Exam.Examinations;
using Magic.Guangdong.DbServices.Entities;
using Magic.Guangdong.DbServices.Interfaces;

namespace Magic.Guangdong.DbServices.Methods
{
    internal class ValidateExpressionRepo : ExaminationRepository<ValidateExpression>, IValidateExpressionRepo
    {
        private IdleBus<IFreeSql> fsql;
        const string conn_str = "db_exam";
        public ValidateExpressionRepo(IdleBus<IFreeSql> fsql)
            : base(fsql)
        {
            this.fsql = fsql;
        }

        public async Task<bool> InsertExamValidateExpressions(ExamValidateExpressionDto dto)
        {
            try
            {
                var repo = fsql.Get(conn_str).GetRepository<ValidateExpression>();
                if (dto.PaperIds.Any())
                {
                    foreach (var paperId in dto.PaperIds)
                    {
                        if (!await repo.Where(u => u.PaperId == paperId).AnyAsync())
                        {
                            continue;
                        }
                        await repo.Where(u => u.PaperId == paperId && u.ColumnId == dto.ColumnId)
                             .ToUpdate()
                             .Set(u => u.IsDeleted, 1)
                             .Set(u => u.UpdatedAt, DateTime.Now)
                             .Set(u => u.Remark, $"于{DateTime.Now}被管理员{dto.AdminId}删除")
                             .ExecuteAffrowsAsync();
                    }
                }
                //await repo.Where(u => u.ExamId == dto.ExamId &&  dto.PaperIds.Contains(u.PaperId) && u.ColumnId == dto.ColumnId)
                //    .ToUpdate()
                //    .Set(u => u.IsDeleted, 1)
                //    .Set(u => u.UpdatedAt, DateTime.Now)
                //    .Set(u => u.Remark, $"于{DateTime.Now}被管理员{dto.AdminId}删除")
                //    .ExecuteAffrowsAsync();

                List<ValidateExpression> validateExpressions = new List<ValidateExpression>();
                foreach (Guid paperId in dto.PaperIds)
                {
                    validateExpressions.Add(new ValidateExpression
                    {
                        IsDeleted = 0,
                        Status = 1,
                        PaperId = paperId,
                        ExamId = dto.ExamId,
                        Expression = dto.Expression,
                        CreatedBy = dto.AdminId,
                        UpdatedBy = dto.AdminId,
                        ColumnId = dto.ColumnId
                    });
                }
                await repo.InsertAsync(validateExpressions);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
