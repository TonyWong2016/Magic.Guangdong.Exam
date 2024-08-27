using Autofac.Extensions.DependencyInjection;
using Autofac;
using Magic.Guangdong.Exam.Client.Extensions;

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
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


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
