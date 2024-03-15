using Magic.Guangdong.DbServices.Dto;
using Magic.Guangdong.DbServices.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.DbServices.Interfaces
{
    public interface IUserAnswerRecordViewRepo : IExaminationRepository<UserAnswerRecordView>
    {
        dynamic GetUserRecord(PageDto dto, out long total);

        Task<List<UserAnswerRecordDto>> GetUserRecordForExport(string whereJsonStr);
        Task<dynamic> GetUserAnswer(long urid);

        Task<bool> RemoveUserRecord(long urid, string adminId);

        Task<UserAnswerRecordView> ForceMarking(long urid);

        Task<List<long>> GetNotComplatedList(Guid examId);
    }
}
