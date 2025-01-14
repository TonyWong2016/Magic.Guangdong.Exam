using Autofac;
using Autofac.Extensions.DependencyInjection;
using Magic.Guangdong.Exam.Extensions;

public class Program
{
    static void Main(string[] args)
    {

        //ContainerBuilder containerBuilder = new ContainerBuilder();
        //containerBuilder.RegisterModule<ConfigureAutofac>();
        //containerBuilder.Build();
       
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
    }
}



