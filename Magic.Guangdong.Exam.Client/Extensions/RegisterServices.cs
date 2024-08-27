//using Authing.ApiClient.Auth;
using Essensoft.Paylink.Alipay;
using Essensoft.Paylink.WeChatPay;
using FreeSql;
using IdentityModel;
using Magic.Guangdong.Assistant;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
//using NetDevPack.Security.JwtExtensions;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Yitter.IdGenerator;

namespace Magic.Guangdong.Exam.Client.Extensions
{
    public static class RegisterServices
    {
        private static IConfiguration _configuration;

        public static WebApplicationBuilder SetupServices(this WebApplicationBuilder builder)
        {
            builder.Configuration
                .AddJsonFile("Configs/cachesetting.json", optional: true, reloadOnChange: true)
                .AddJsonFile("Configs/mqsetting.json", optional: true, reloadOnChange: true)
                .AddJsonFile("Configs/paysetting.json", optional: true, reloadOnChange: true)
               ;

            _configuration = builder.Configuration;
            ConfigurationHelper.Initialize(_configuration);

            

            Logger.InitLog();

            builder.Services.ConfigureOrm(_configuration);
            builder.Services.ConfigureRazorPages();
            builder.Services.ConfigureRedis(_configuration);
            builder.Services.ConfigurePlug(_configuration);
            //builder.Services.ConfigureAuthing();
            builder.Services.ConfigureSelfAuthing(_configuration);
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // 添加Paylink依赖注入
            builder.Services.AddAlipay();
            builder.Services.AddWeChatPay();

            // 在 appsettings.json(开发环境：appsettings.Development.json) 中 配置选项
            builder.Services.Configure<AlipayOptions>(_configuration.GetSection("Alipay"));
            builder.Services.Configure<WeChatPayOptions>(_configuration.GetSection("WeChatPay"));
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

        //private static void ConfigureAuthing(this IServiceCollection services)
        //{
        //    var authenticationClient = new AuthenticationClient(options =>
        //    {

        //        options.AppId = _configuration["Authing.Config:AppId"];
        //        options.Host = _configuration["Authing.Config:Host"];
        //        options.UserPoolId = _configuration["Authing.Config:UserPoolId"];
        //        options.Secret = _configuration["Authing.Config:Secret"];
        //        options.RedirectUri = _configuration["Authing.Config:RedirectUri"];
        //    });
        //    services.AddSingleton(typeof(AuthenticationClient), authenticationClient);
        //    var jwtSettings = new JwtSettings();
        //    _configuration.Bind("JwtSettings", jwtSettings);
        //    services.AddAuthentication(options =>
        //    {
        //        //认证middleware配置
        //        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        //        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        //    })
        //   .AddJwtBearer(o =>
        //   {
        //       //主要是jwt  token参数设置
        //       o.TokenValidationParameters = new TokenValidationParameters
        //       {
        //           //Token颁发机构
        //           ValidIssuer = jwtSettings.Issuer,
        //           //颁发给谁
        //           ValidAudience = jwtSettings.Audience,
        //           //这里的key要进行加密，需要引用Microsoft.IdentityModel.Tokens
        //           // IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
        //           // ValidateIssuerSigningKey = true,
        //           //是否验证Token有效期，使用当前时间与Token的Claims中的NotBefore和Expires对比
        //           // ValidateLifetime = true,
        //           //允许的服务器时间偏移量
        //           // ClockSkew = TimeSpan.Zero,
        //           ValidAlgorithms = new string[] { "RS256" },
        //           // IssuerSigningKeys = signingKeys,
        //       };
        //       o.RequireHttpsMetadata = false;
        //       o.SaveToken = false;
        //       o.IncludeErrorDetails = true;
        //       o.SetJwksOptions(new JwkOptions(jwtSettings.JwksUri, jwtSettings.Issuer, new TimeSpan(TimeSpan.TicksPerDay)));
        //   });
        //}

        private static void ConfigureSelfAuthing(this IServiceCollection services, IConfiguration configuration)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            //JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.Clear();
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
                .AddCookie("Cookies", options =>
                {
                    options.AccessDeniedPath = new PathString("/Error?msg=" + Assistant.Utils.EncodeUrlParam("未登录"));
                
                })
                .AddJwtBearer(cfg =>
                {
                    cfg.RequireHttpsMetadata = false;
                    cfg.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false,
                        ValidateIssuer = false,
                        ValidateIssuerSigningKey = false
                    };
                })
                .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
                {

                    //samesite 设置
                    CookieBuilder cb = new CookieBuilder();
                    cb.Name = OpenIdConnectDefaults.CookieNoncePrefix;
                    cb.SameSite = SameSiteMode.Unspecified;
                    cb.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                    cb.HttpOnly = true;
                    cb.IsEssential = true;
                    
                    //options.Authority = ConfigurationHelper.GetSectionValue("authHost");
                    options.Authority = "http://login.xiaoxiaotong.org";
                    //options.NonceCookie = cb;
                    //options.CorrelationCookie = cb;

                    options.ClientId = "MagicExam2024";
                    options.ClientSecret = "MagicGDExam14cFGp";

                    options.ResponseType = OpenIdConnectResponseType.CodeIdToken;
                    options.Scope.Clear();
                    options.Scope.Add("api1");
                    options.Scope.Add("openid");
                    options.Scope.Add("profile");
                    options.SaveTokens = true;
                    options.RequireHttpsMetadata = false;
                    options.GetClaimsFromUserInfoEndpoint = true;
                    options.ClaimActions.MapUniqueJsonKey("role", "role");
                    //options.MapInboundClaims = false;
                    // options.UseSecurityTokenValidator = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = JwtClaimTypes.GivenName,
                        RoleClaimType = JwtClaimTypes.Role,
                    };
                    
                });
        }

       
        /// <summary>
        /// 配置orm
        /// </summary>
        private static void ConfigureOrm(this IServiceCollection services, IConfiguration configuration)
        {
            IdleBus<IFreeSql> ib = new IdleBus<IFreeSql>(TimeSpan.FromMinutes(10));
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
                ib.Register("db_passport", () =>
                    new FreeSqlBuilder()
                    .UseConnectionString(DataType.SqlServer, configuration.GetConnectionString("PassportConnString"))
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
                ib.Register("db_passport", () =>
                    new FreeSqlBuilder()
                    .UseConnectionString(DataType.SqlServer, configuration.GetConnectionString("PassportConnString"))
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
                x.SucceedMessageExpiredAfter = 24 * 3600 * 3;
                x.FailedMessageExpiredAfter = 24 * 3600 * 30;
                //设置失败重试次数
                x.FailedRetryCount = 5;

            });

        }

    }

    public class JwtSettings
    {
        //token是谁颁发的
        public string Issuer { get; set; }
        //token可以给哪些客户端使用
        public string Audience { get; set; }
        //加密的key
        public string SecretKey { get; set; }
        // JwksUri
        public string JwksUri { get; set; }
    }
}
