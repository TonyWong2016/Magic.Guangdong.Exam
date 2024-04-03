using Autofac;
using System.Reflection;

namespace Magic.Guangdong.Exam.Client.Extensions
{
    public class ConfigureAutofac : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            #region 注册助手类
            //base.Load(builder);
            //注册助手类（standard2.0）
            var assemblyAssistant = Assembly.Load("Magic.Guangdong.Assistant");
            builder.RegisterAssemblyTypes(assemblyAssistant).Where(t => t.Name.EndsWith("Helper") || t.Name.EndsWith("Service")).AsImplementedInterfaces();
            #endregion

            //#region 注册模型类
            //var assemblyDbServices = Assembly.Load("Magic.Guangdong.DbServices");
            //builder.RegisterAssemblyTypes(assemblyDbServices)
            //    .Where(u => u.Name.EndsWith("Repository") || u.Name.EndsWith("Repo") || u.Name.EndsWith("Base"))
            //    .AsImplementedInterfaces();

            //#endregion

        }
    }
}
