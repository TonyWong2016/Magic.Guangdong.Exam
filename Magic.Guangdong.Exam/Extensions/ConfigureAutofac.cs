using Autofac;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.Reflection;

namespace Magic.Guangdong.Exam.Extensions
{
    /// <summary>
    /// Autofac配置
    /// </summary>
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

            #region 注册模型类
            var assemblySystemModels = Assembly.Load("Magic.Guangdong.DbServices");
            builder.RegisterAssemblyTypes(assemblySystemModels).Where(u => u.Name.EndsWith("Respo") || u.Name.EndsWith("Repo") || u.Name.EndsWith("Base")).AsImplementedInterfaces();

            #endregion

            #region 在控制器中使用属性依赖注入，其中注入属性必须标注为public
            var manager = new ApplicationPartManager();
            manager.ApplicationParts.Add(new AssemblyPart(Assembly.Load("Magic.Guangdong.Exam")));
            manager.FeatureProviders.Add(new ControllerFeatureProvider());
            var feature = new ControllerFeature();
            manager.PopulateFeature(feature);
            builder.RegisterTypes(feature.Controllers.Select(ti => ti.AsType()).ToArray()).PropertiesAutowired();

            //var controllersTypesInAssembly = typeof(Program).Assembly.GetExportedTypes().Where(type => typeof(Microsoft.AspNetCore.Mvc.ControllerBase).IsAssignableFrom(type)).ToArray();
            // builder.RegisterTypes(controllersTypesInAssembly).PropertiesAutowired();
            #endregion
        }
    }
}
