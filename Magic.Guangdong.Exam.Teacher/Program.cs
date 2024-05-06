using Autofac;
using Autofac.Extensions.DependencyInjection;
using Magic.Guangdong.Exam.Teacher.Extensions;

var builder = WebApplication.CreateBuilder(args).SetupServices();
builder.Services.AddControllersWithViews();

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureContainer<ContainerBuilder>(containerBuilder =>
    {
        containerBuilder.RegisterModule<ConfigureAutofac>();
        //containerBuilder.Build();
    });
var app = builder.Build().SetupMiddlewares();

app.Run();
