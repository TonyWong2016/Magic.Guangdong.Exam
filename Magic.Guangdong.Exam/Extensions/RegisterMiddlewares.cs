using Autofac.Extensions.DependencyInjection;
using Coravel;
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
                    .DailyAt(12,00)
                    .Zoned(TimeZoneInfo.Local)
                    .PreventOverlapping(nameof(SyncUnitDataFromXXT))
                ;
            });

            return app;
        }
    }
   
}
