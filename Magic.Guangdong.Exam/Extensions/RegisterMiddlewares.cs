using AspNetCoreRateLimit;
using Autofac.Extensions.DependencyInjection;
using Coravel;
using Magic.Guangdong.Exam.AutoJobs.MiddleWare;
using Magic.Guangdong.Exam.AutoJobs.SyncUnitInfo;
using Microsoft.AspNetCore.Builder;
using SixLabors.ImageSharp.Web.DependencyInjection;

namespace Magic.Guangdong.Exam.Extensions
{
    public static class RegisterMiddlewares
    {
        public static WebApplication SetupMiddlewares(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseImageSharp();
            app.UseStaticFiles();
            app.UseRouting();
            //app.UseSession();
            //大多数中间件都是在UseRouting之后引用，注意顺序

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCors("any");
            //app.UseCors();           
            app.UseCookiePolicy();

            app.UseResponseCompression();
            app.UseDefaultFiles();
            app.UseResponseCaching();
            app.MapRazorPages();

            //注入ip限流中间件
            app.UseIpRateLimiting();

            //注入客户端限流中间件（需要自己在请求客户端实现X-ClientId）
            //app.UseClientRateLimiting();

            app.MapControllerRoute(
               name: "area",
               pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
            );
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}"
            );


            // app.UseWebSockets();
            app.Services.UseScheduler(scheduler =>
            {
                scheduler.OnWorker("SyncUnitDataFromXXT");
                scheduler
                    .Schedule<SyncUnitDataFromXXT>()
                    .Cron("00 12 * * 5")// 每周五中午12点同步
                    .Zoned(TimeZoneInfo.Local)
                    .PreventOverlapping(nameof(SyncUnitDataFromXXT))
                ;

                scheduler.OnWorker(nameof(CacheHandle));
                scheduler
                    .Schedule<CacheHandle>()
                    .DailyAt(3, 0)// 每天3点整执行一次
                    .Zoned(TimeZoneInfo.Local)
                    .PreventOverlapping(nameof(CacheHandle));

                scheduler.OnWorker(nameof(ClearCapMsgId));
                scheduler
                    .Schedule<ClearCapMsgId>()
                    .Hourly()
                    .PreventOverlapping(nameof(ClearCapMsgId));
            });

            return app;
        }



    }

}
