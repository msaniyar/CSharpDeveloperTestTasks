using Autofac;
using Core.Interfaces;
using Core.Services;

namespace Core
{
    public class CoreModule : Module
    {
        private readonly string _url;
        public CoreModule(string url)
        {
            _url = url;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<NumberProcessorService>()
                .As<INumberProcessorService>()
                .WithParameter("url", _url)
                .InstancePerLifetimeScope();

            builder.RegisterType<StringReverseService>()
                .As<IStringReverseService>()
                .InstancePerLifetimeScope();
        }
    }
}
