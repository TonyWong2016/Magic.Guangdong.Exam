using Serilog;
using Serilog.Filters;
using Serilog.Events;

namespace Magic.Guangdong.Assistant
{
    public class Logger
    {
        //static CSRedisClient csRedis = new CSRedisClient("10.185.3.130:6379,defaultDatabase=7");
        //static CSRedisClient csRedis = new CSRedisClient("120.48.17.153:6400,pass=tonydf123,defaultDatabase=7,poolsize=10");

        //static CSRedisClient csRedis = new CSRedisClient("127.0.0.1:6380,pass=wt123456,defaultDatabase=7,poolsize=50,ssl=false,writeBuffer=10240", new string[] {"127.0.0.1:26380", "127.0.0.1:26381", "127.0.0.1:26382" });
        const string log1Name = "ApiLog";
        const string log2Name = "WebLog";
        const string log3Name = "ErrorLog";
        const string log4Name = "DebugLog";
        const string log5Name = "WarningLog";
        
        /// <summary>
        /// 初始化日志
        /// </summary>
        public static void InitLog()
        {
            //RedisHelper.Initialization(new CSRedisClient("127.0.0.1:6380,pass=wt123456,defaultDatabase=7,poolsize=50,ssl=false,writeBuffer=10240"));
            string LogFilePath(string FileName) => $@"{AppContext.BaseDirectory}ALL_Logs\{FileName}\log.log";
            string SerilogOutputTemplate = "{NewLine}Date：{Timestamp:yyyy-MM-dd HH:mm:ss.fff}{NewLine}LogLevel：{Level}{NewLine}Message：{Message}{NewLine}{Exception}" + new string('-', 100);
            Log.Logger = new LoggerConfiguration()
#if DEBUG
            .MinimumLevel.Debug()
#else
            .MinimumLevel.Information()
#endif
                        .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                        .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
                         .Enrich.FromLogContext()
                        .WriteTo.Logger(lg => lg.Filter.ByIncludingOnly(Matching.WithProperty<string>("position", p => p == log1Name)).WriteTo.Async(a => a.File(LogFilePath(log1Name), rollingInterval: RollingInterval.Day, outputTemplate: SerilogOutputTemplate)))
                        .WriteTo.Logger(lg => lg.Filter.ByIncludingOnly(Matching.WithProperty<string>("position", p => p == log2Name)).WriteTo.Async(a => a.File(LogFilePath(log2Name), rollingInterval: RollingInterval.Day, outputTemplate: SerilogOutputTemplate)))
                        .WriteTo.Logger(lg => lg.Filter.ByIncludingOnly(Matching.WithProperty<string>("position", p => p == log3Name)).WriteTo.Async(a => a.File(LogFilePath(log3Name), rollingInterval: RollingInterval.Day, outputTemplate: SerilogOutputTemplate)))
                        .WriteTo.Logger(lg => lg.Filter.ByIncludingOnly(Matching.WithProperty<string>("position", p => p == log4Name)).WriteTo.Async(a => a.File(LogFilePath(log4Name), rollingInterval: RollingInterval.Day, outputTemplate: SerilogOutputTemplate)))
                        .WriteTo.Logger(lg => lg.Filter.ByIncludingOnly(Matching.WithProperty<string>("position", p => p == log5Name)).WriteTo.Async(a => a.File(LogFilePath(log5Name), rollingInterval: RollingInterval.Day, outputTemplate: SerilogOutputTemplate)))
                        //.WriteTo.Async(a => a.Console())
                        .CreateLogger();
            
        }

        /*****************************下面是不同日志级别*********************************************/
        // FATAL(致命错误) > ERROR（一般错误） > Warning（警告） > Information（一般信息） > DEBUG（调试信息）>Verbose（详细模式，即全部）



        /// <summary>
        /// 普通日志
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="fileName"></param>
        public static void Info(string msg, string fileName = "")
        {
            if (fileName == "" || fileName == log1Name)
            {
                Log.Information($"{{position}}:{msg}", log1Name);
                
            }
            else if (fileName == log2Name)
            {
                Log.Information($"{{position}}:{msg}", log2Name);
            }
            else
            {
                //输入其他的话，还是存放到第一个文件夹
                Log.Information($"{{position}}:{msg}", log1Name);
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Info:{msg}....{DateTime.Now}");
            Console.ResetColor();
            //Task.Run(() => writeLogToRedis(msg, "info"));
        }

        /// <summary>
        /// 调试日志
        /// </summary>
        /// <param name="msg"></param>
        public static void Debug(string msg)
        {            

            if (ConfigurationHelper.GetSectionValue("env") != "dev")
                return;
            Log.Debug($"{{position}}:{msg}", log4Name);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"Debug:{msg}....{DateTime.Now}");
            Console.ResetColor();
            //Task.Run(() => writeLogToRedis(msg, "debug"));
        }
        /// <summary>
        /// 冗余日志
        /// </summary>
        /// <param name="msg"></param>
        public static void Verbose(string msg)
        {
            if (ConfigurationHelper.GetSectionValue("env") != "dev")
                return;
            Log.Debug($"{{position}}:{msg}", log2Name);
        }
        /// <summary>
        /// 警告日志或一些关键日志
        /// </summary>
        /// <param name="msg"></param>
        public static void Warning(string msg)
        {
            Log.Warning($"{{position}}:{msg}", log5Name);
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine($"Warning:{msg}....{DateTime.Now}");
            Console.ResetColor();
            //Task.Run(() => writeLogToRedis(msg, "warning"));
        }
        /// <summary>
        /// Error方法统一存放到ErrorLog文件夹
        /// </summary>
        /// <param name="msg"></param>
        public static void Error(Exception ex)
        {
            Log.Error(ex, "{position}:" + ex.Message, log3Name);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error:{ex.Message}....{DateTime.Now}");
            Console.ResetColor();
            //Task.Run(() => writeLogToRedis(ex.Message, "error"));
        }

        public static void Error(string msg)
        {
            Log.Error("{position}:" + msg, log3Name);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error:{msg}....{DateTime.Now}");
            Console.ResetColor();
            //Task.Run(() => writeLogToRedis(msg, "error"));
        }


        /// <summary>
        /// 日志写入到redis队列
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="logLevel"></param>
        /// <returns></returns>
        public static async Task writeLogToRedis(string msg, string logLevel,string system="magicExam")
        {            
            string ret = $"{system} {logLevel} {msg}";
            //Console.WriteLine(msg);
            Console.ForegroundColor = ConsoleColor.Blue;
           // Console.WriteLine($"log:{msg}....{DateTime.Now}");
            Console.ResetColor();
            await RedisHelper.LPushAsync(ConfigurationHelper.GetSectionValue("redislogkey"), ret);
        }
    }
}
