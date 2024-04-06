using FreeSql;
using Magic.Guangdong.Assistant;
using Microsoft.AspNetCore.ResponseCompression;
using Yitter.IdGenerator;

namespace Magic.Guangdong.Exam.Client.Extensions
{
    public static class RegisterServices
    {
        private static IConfiguration _configuration;

        public static WebApplicationBuilder SetupServices(this WebApplicationBuilder builder)
        {
            _configuration = builder.Configuration;
            ConfigurationHelper.Initialize(_configuration);

            Logger.InitLog();

            builder.Services.ConfigureOrm(_configuration);
            builder.Services.ConfigureRazorPages();
            builder.Services.ConfigureRedis(_configuration);
            builder.Services.ConfigurePlug(_configuration);

            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            return builder;
        }

        private static void ConfigureRazorPages(this IServiceCollection services)
        {
            services.AddRazorPages()
                .AddMvcOptions(option =>
                {
                    option.Filters.Add(typeof(Filters.AuthorizeFilter));
                    option.Filters.Add(typeof(Filters.GlobalActionFilter));
                    option.Filters.Add(typeof(Filters.GlobalExceptionFilter));

                });
            //数据压缩
            services.AddResponseCompression(options =>
            {
                options.Providers.Add<BrotliCompressionProvider>();
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "image/svg+xml" });
            });
        }

        static IdleBus<IFreeSql> ib = new IdleBus<IFreeSql>(TimeSpan.FromMinutes(10));
        /// <summary>
        /// 配置orm
        /// </summary>
        private static void ConfigureOrm(this IServiceCollection services, IConfiguration configuration)
        {
            #region orm框架
            //IFreeSql fsql = new FreeSqlBuilder()
            //.UseConnectionString(DataType.SqlServer, configuration.GetConnectionString("ExamConnString"))
            ////.UseAutoSyncStructure(true) //自动同步实体结构到数据库，FreeSql不会扫描程序集，只有CRUD时才会生成表。
            ////.UseMonitorCommand(cmd => Console.Write(cmd.CommandText))
            //.Build(); //请务必定义成 Singleton 单例模式

            //services.AddSingleton<IFreeSql>(fsql);
            if (configuration.GetSection("env").Value == "dev")
            {
                ib.Register("db_exam", () =>
                    new FreeSqlBuilder()
                    .UseConnectionString(DataType.SqlServer, configuration.GetConnectionString("ExamConnString"))
                    .UseMonitorCommand(cmd =>
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"Sql：{cmd.CommandText}");
                        Console.ResetColor();
                    })//监听SQL语句
                    .Build()
                );
            }
            else
            {
                ib.Register("db_exam", () =>
                    new FreeSqlBuilder()
                    .UseConnectionString(DataType.SqlServer, configuration.GetConnectionString("ExamConnString"))
                    .Build()
                );
            }


            services.AddSingleton(ib);
            #endregion
        }


        #region 配置redis
        private static void ConfigureRedis(this IServiceCollection services, IConfiguration configuration)
        {
            string redisMode = configuration.GetSection("RedisMode").Value;
            if (redisMode == "cluser")
                ConfigureRedisCluser(services, configuration);
            else
                ConfigureRedisSingle(services, configuration);
        }

        /// <summary>
        /// 哨兵集群配置
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        private static void ConfigureRedisCluser(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMemoryCache();
            services.AddResponseCaching();
            services.AddEasyCaching(options =>
            {
                object value = options.UseCSRedis(configuration, "mymsgpack", "easycaching:csredis").WithMessagePack("mymsgpack");
                //options.UseRedis(configuration, "mymsgpack", "easycaching:csredis").WithMessagePack("mymsgpack");
            });
            //RedisHelper.Initialization(new CSRedis.CSRedisClient("10.185.3.130:6379,defaultDatabase=7,poolsize=50,ssl=false,writeBuffer=10240"));
            RedisHelper.Initialization(new CSRedis.CSRedisClient(configuration.GetSection("CsredisStr:ConnectionString").Value, configuration.GetSection("CsredisStr:Sentinels").GetChildren().Select(p => p.Value).ToArray()));
        }

        /// <summary>
        /// 单节点使用缓存
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        private static void ConfigureRedisSingle(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMemoryCache();
            services.AddResponseCaching();
            services.AddEasyCaching(options =>
            {
                options.UseCSRedis(configuration, "magicRedis", "easycaching:csredisSingle").WithMessagePack("mymsgpack");
            });
            RedisHelper.Initialization(new CSRedis.CSRedisClient(configuration.GetSection("CsredisStrSingle:ConnectionString").Value));
            //RedisHelper.Initialization(new CSRedis.CSRedisClient(configuration.GetSection("CsredisStr:ConnectionString").Value, configuration.GetSection("CsredisStr:Sentinels").GetChildren().Select(p => p.Value).ToArray()));
        }
        #endregion

        private static void ConfigurePlug(this IServiceCollection services, IConfiguration configuration)
        {
            //配置雪花漂移
            var options = new IdGeneratorOptions(6);
            YitIdHelper.SetIdGenerator(options);

            //配置CAP
            services.AddCap(x =>
            {
                x.UseSqlServer(x => x.ConnectionString = configuration.GetConnectionString("ExamConnString"));
                x.UseRabbitMQ(opt =>
                {
                    opt.HostName = configuration.GetSection("RabbitMQ")["HostName"];
                    opt.UserName = configuration.GetSection("RabbitMQ")["UserName"];
                    opt.Password = configuration.GetSection("RabbitMQ")["Password"];
                    opt.VirtualHost = configuration.GetSection("RabbitMQ")["VirtualHost"];
                    opt.Port = Convert.ToInt32(configuration.GetSection("RabbitMQ")["Port"]);
                    //指定Topic exchange名称，不指定的话会用默认的
                    opt.ExchangeName = configuration.GetSection("RabbitMQ")["ExchangeName"];
                });
                //设置处理成功的数据在数据库中保存的时间（秒），为保证系统新能，数据会定期清理。
                x.SucceedMessageExpiredAfter = 24 * 3600;

                //设置失败重试次数
                x.FailedRetryCount = 5;

            });

        }

    }
}
