using Autofac;
using Magic.Guangdong.Exam.Extensions;

namespace Magic.Guangdong.Exam
{
    public class Program
    {
        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule<ConfigureAutofac>();
        }
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args).SetupServices();
            var app = builder.Build().SetupMiddlewares();
            app.Run();
        }
    }
}