using Autofac;
using Magic.Guangdong.Exam.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System;



var builder = WebApplication.CreateBuilder(args).SetupServices();
builder.Services.AddControllersWithViews();
ContainerBuilder containerBuilder = new ContainerBuilder();
//static void ConfigureContainer(ContainerBuilder builder)
//{
//    containerBuilder.RegisterModule<ConfigureAutofac>();
//}
containerBuilder.RegisterModule<ConfigureAutofac>();
var app = builder.Build().SetupMiddlewares();
app.Run();

