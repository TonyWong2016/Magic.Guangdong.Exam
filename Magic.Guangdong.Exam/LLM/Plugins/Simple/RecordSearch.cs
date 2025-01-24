//using Magic.Guangdong.DbServices.AgentBases;
using Magic.Guangdong.DbServices.AgentBases;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace Magic.Guangdong.Exam.LLM.Plugins.Simple
{
    public class RecordSearch(IServiceProvider serviceProvider)
    {
        private IServiceProvider _serviceProvider = serviceProvider;

        [KernelFunction("get_userrecord_by_name")]
        [Description("Get the record by name")]
        [return: Description("The record of the user; will return null if the user dose not exist")]
        public async Task<DbServices.Dtos.Exam.UserAnswerRecord.UserAnswerSubmitRecordDto?> GetUserRecordByNameAsync(string name)
        {
            var _recordRepo = _serviceProvider.GetRequiredService<IRecordBase>();
            //var plugbase = new RecordPlugBase();
            if (!await _recordRepo.getAnyAsync(u => u.UserName == name))
            {
                return null;
            }
            return (await _recordRepo.getOneAsync(u => u.UserName == name))
                .Adapt<DbServices.Dtos.Exam.UserAnswerRecord.UserAnswerSubmitRecordDto>();
        }

        [KernelFunction("get_userrecord_by_id")]
        [Description("Get the record by id")]
        [return: Description("The record of the user; will return null if the user dose not exist")]
        public async Task<DbServices.Dtos.Exam.UserAnswerRecord.UserAnswerSubmitRecordDto?> GetUserRecordByIdAsync(long id)
        {
            var _recordRepo = _serviceProvider.GetRequiredService<IRecordBase>();
            //IdleBus<IFreeSql> idleBus = new IdleBus<IFreeSql>();
            //var _recordRepo = new RecordPlugBase(idleBus);
            if (!await _recordRepo.getAnyAsync(u => u.Id == id))
            {
                return null;
            }
            return (await _recordRepo.getOneAsync(u => u.Id == id))
                .Adapt<DbServices.Dtos.Exam.UserAnswerRecord.UserAnswerSubmitRecordDto>();
        }
    }


}
