using Autofac.Extensions.DependencyInjection;
using Autofac;
using Magic.Guangdong.Exam.Client.Extensions;
using Microsoft.AspNetCore.Hosting;

var builder = WebApplication.CreateBuilder(args)
    .SetupServices();

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureContainer<ContainerBuilder>(containerBuilder =>
            {
                containerBuilder.RegisterModule<ConfigureAutofac>();
                //containerBuilder.Build();
            });
//builder.Services.AddSession();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    //app.UseHsts();
    app.Use(async (context, next) =>
    {
        try
        {
            await next();
        }
        catch (Exception ex)
        {
            
            Magic.Guangdong.Assistant.Logger.Error(context.Request.Path);
            Magic.Guangdong.Assistant.Logger.Error(ex);
            // 如果你想重定向到一个特定的错误页
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync($"An error occurred while processing your request,last request path {context.Request.Path},errormsg:{ex.Message}" );
        }
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseCors("any");
app.UseCookiePolicy();

app.UseResponseCompression();
app.UseResponseCaching();
//app.UseSession();
app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}"
             );
app.MapRazorPages();
app.Run();
