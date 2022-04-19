using Autofac;
using Autofac.Extras.DynamicProxy;
using Business.Abstract;
using Business.Concrete;
using Castle.DynamicProxy;
using Core.Utilities.Interceptors;
using Core.Utilities.Security.JWT.Abstract;
using Core.Utilities.Security.JWT.Concrete;
using DataAccess.Abstract;
using DataAccess.Concrete;

namespace Business.DependencyResolvers
{
    public class AutofacBusinessModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<UserService>().As<IUserService>().SingleInstance();
            builder.RegisterType<EfUserDal>().As<IUserDal>().SingleInstance();
            
            builder.RegisterType<KeyLicenseService>().As<IKeyLicenseService>().SingleInstance();
            builder.RegisterType<EfKeyDal>().As<IKeyLicenseDal>().SingleInstance();

            builder.RegisterType<AuthService>().As<IAuthService>().SingleInstance();
            builder.RegisterType<JwtHelper>().As<ITokenHelper>().SingleInstance();

            builder.RegisterType<PanelService>().As<IPanelService>().SingleInstance();
            builder.RegisterType<EfPanelDal>().As<IPanelDal>().SingleInstance();
            
            builder.RegisterType<LogService>().As<ILogService>().SingleInstance();
            builder.RegisterType<EfLogDal>().As<ILogDal>().SingleInstance();
            
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();

            
            builder.RegisterAssemblyTypes(assembly).AsImplementedInterfaces()
                .EnableInterfaceInterceptors(new ProxyGenerationOptions()
                {
                    Selector = new AspectInterceptorSelector()
                }).SingleInstance();

        }
    }
}