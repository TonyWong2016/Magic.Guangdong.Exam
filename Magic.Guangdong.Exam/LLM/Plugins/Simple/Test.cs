using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace Magic.Guangdong.Exam.LLM.Plugins.Simple
{
    public class Test
    {
    }

    /// <summary>
    /// Interface for a service to get the current user id.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Return the user id for the current user.
        /// </summary>
        string GetCurrentUsername();
    }
    public class FakeUserService : IUserService
    {
        public string GetCurrentUsername() => "Bob";
    }

    /// <summary>
    /// A plugin that returns the current time.
    /// </summary>
    public class TimeInformation(ILoggerFactory loggerFactory)
    {
        private readonly ILogger _logger = loggerFactory.CreateLogger<TimeInformation>();

        [KernelFunction]
        [Description("Retrieves the current time in UTC.")]
        public string GetCurrentUtcTime()
        {
            var utcNow = DateTime.UtcNow.ToString("R");
            _logger.LogInformation("Returning current time {0}", utcNow);
            return utcNow;
        }
    }

    /// <summary>
    /// A plugin that returns the current time.
    /// </summary>
    public class UserInformation(IUserService userService)
    {
        [KernelFunction]
        [Description("Retrieves the current users name.")]
        public string GetUsername()
        {
            return userService.GetCurrentUsername();
        }
    }
}
