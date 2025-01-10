using Magic.Guangdong.Assistant;
using Magic.Guangdong.DbServices.Interfaces;
using Magic.Guangdong.Exam.Areas.AI.Models;

namespace Magic.Guangdong.Exam.Areas.AI.Functions
{
    public interface ITest
    {
        string GetWeather(WeatherInput input);
        Task<string> GetUserRecord(UserRecordInput input);
    }
    public class Test: ITest
    {

        private IUserAnswerRecordViewRepo _userAnswerRecordViewRepo;
        public Test(IUserAnswerRecordViewRepo userAnswerRecordViewRepo)
        {
            _userAnswerRecordViewRepo = userAnswerRecordViewRepo;
        }
        public string GetWeather(WeatherInput input)
        {
            // 这里应替换为实际获取天气信息的逻辑
            return $"The current temperature in {input.location} is 24°C.";
        }

        public async Task<string> GetUserRecord(UserRecordInput input)
        {
            if (await _userAnswerRecordViewRepo.getAnyAsync(u => u.Name == input.UserName))
            {
                return JsonHelper.JsonSerialize(await _userAnswerRecordViewRepo.getOneAsync(u => u.Name == input.UserName));
            }
            return "";
        }
    }
}
