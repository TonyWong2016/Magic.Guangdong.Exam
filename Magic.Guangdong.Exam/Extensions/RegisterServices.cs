using Coravel;
using Essensoft.Paylink.Alipay;
using Essensoft.Paylink.WeChatPay;
using FreeSql;
using Magic.Guangdong.Assistant;
using Magic.Guangdong.Exam.Filters;
using Mapster;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Web.Caching;
using SixLabors.ImageSharp.Web.Commands;
using SixLabors.ImageSharp.Web.DependencyInjection;
using SixLabors.ImageSharp.Web.Providers;
using Yitter.IdGenerator;

namespace Magic.Guangdong.Exam.Extensions
{
    public static class RegisterServices
    {
        private static IConfiguration _configuration;
        public static WebApplicationBuilder SetupServices(this WebApplicationBuilder builder)
        {
            builder.Configuration
                 .AddJsonFile("Configs/cachesetting.json", optional: true, reloadOnChange: true)
                 .AddJsonFile("Configs/filesetting.json", optional: true, reloadOnChange: true)
                 .AddJsonFile("Configs/mqsetting.json", optional: true, reloadOnChange: true)
                 .AddJsonFile("Configs/paysetting.json", optional: true, reloadOnChange: true)
                ;
            _configuration = builder.Configuration;
            ConfigurationHelper.Initialize(_configuration);
            Logger.InitLog();
            builder.Services.ConfigureMvc();
            builder.Services.ConfigureOrm(_configuration);
            builder.Services.ConfigureRedis(_configuration);
            builder.Services.ConfigurePolicy(_configuration);
            builder.Services.ConfigurePlug(_configuration);
            // builder.Services.BuildServiceProvider();
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // 添加Paylink依赖注入
            builder.Services.AddAlipay();
            builder.Services.AddWeChatPay();
            // 在 appsettings.json(开发环境：appsettings.Development.json) 中 配置选项
            builder.Services.Configure<AlipayOptions>(_configuration.GetSection("Alipay"));
            builder.Services.Configure<WeChatPayOptions>(_configuration.GetSection("WeChatPay"));
            builder.Services.ConfigureImageSharp(_configuration);
            return builder;
        }

       
        /// <summary>
        /// 配置mvc
        /// </summary>
        /// <param name="services"></param>
        private static void ConfigureMvc(this IServiceCollection services)
        {
            #region mvc,过滤器，拦截器配置
            //services.AddControllersWithViews();

            services.AddMvc(option =>
            {
                option.Filters.Add(typeof(Filters.AuthorizeFilter));
                ////option.Filters.Add(typeof(PermissionActionFilter));
                option.Filters.Add(typeof(GlobalExceptionFilter));
                option.Filters.Add(typeof(GlobalActionFilter));

            }).AddJsonOptions(option =>
            {
                option.JsonSerializerOptions.Converters.Add(new DatetimeJsonConverter());
            });
            services.AddRazorPages().AddRazorRuntimeCompilation();
            #endregion
            //引入signalr
            services.AddSignalR().AddMessagePackProtocol();
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
        private static void ConfigureOrm(this IServiceCollection services,IConfiguration configuration)
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

        /// <summary>
        /// 配置登录策略，跨域等
        /// </summary>
        /// <param name="services"></param>
        private static void ConfigurePolicy(this IServiceCollection services, IConfiguration configuration)
        {
            //services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            //    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            //    {
            //        options.LoginPath = "/system/account/login";
            //        options.Cookie.HttpOnly = true;
            //        options.Cookie.SameSite = SameSiteMode.Lax;
            //        options.ExpireTimeSpan = TimeSpan.FromHours(2);
            //    })
            //    ;

            services.AddCors(options =>
            {
                options.AddPolicy("api", builder =>
                {
                    builder.WithOrigins("https://localhost:5001,https://localhost:7296").AllowAnyHeader().AllowCredentials().WithExposedHeaders("cyscc");

                    builder.SetIsOriginAllowed(orgin => true).AllowCredentials().AllowAnyHeader();
                });
                //登录用户使用
                options.AddPolicy("any", builder =>
                {
                    builder.WithOrigins(configuration.GetSection("corsHosts").Value.Split(","))
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
                });
            });

            //services.AddSession(options =>
            //{
            //    options.IdleTimeout = TimeSpan.FromHours(2);
            //    options.Cookie.HttpOnly = true;
            //    options.Cookie.IsEssential = true;
            //});
            //services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "DataProtection"));
            
            ////这里以后要增加日志拦截器和权限拦截器
            //services.AddAntiforgery(options =>
            //{
            //    options.HeaderName = "X-CSRF-TOKEN";
            //});
        }

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
                //设置处理成功的数据在数据库中保存的时间（秒），为保证系统性能，数据会定期清理。
                x.SucceedMessageExpiredAfter = 24 * 3600 * 3;

                //设置失败重试次数
                x.FailedRetryCount = 5;

                //设置处理失败的数据在数据库中保存的时间（秒），为保证系统性能，数据会定期清理。
                x.FailedMessageExpiredAfter = 24 * 3600 * 30;

            });            
        }

        /// <summary>
        /// 配置Coravel
        /// </summary>
        /// <param name="services"></param>
        private static void ConfigureCoravel(this IServiceCollection services)
        {
            
            services.AddScheduler();

        }

        private static void ConfigureImageSharp(this IServiceCollection services, IConfiguration configuration)
        {
            // Add the default service and custom options.
            services.AddImageSharp(
                options =>
                {
                    // You only need to set the options you want to change here
                    // All properties have been listed for demonstration purposes
                    // only.
                    options.Configuration = Configuration.Default;
                    options.MemoryStreamManager = new RecyclableMemoryStreamManager();
                    options.BrowserMaxAge = TimeSpan.FromDays(7);
                    options.CacheMaxAge = TimeSpan.FromDays(365);
                    options.CacheHashLength = 8;
                    options.OnParseCommandsAsync = _ => Task.CompletedTask;
                    options.OnBeforeSaveAsync = _ => Task.CompletedTask;
                    options.OnProcessedAsync = _ => Task.CompletedTask;
                    options.OnPrepareResponseAsync = _ => Task.CompletedTask;
                });
        }
    
        
    }
}
