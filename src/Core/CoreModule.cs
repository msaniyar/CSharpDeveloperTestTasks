using Autofac;
using Core.Interfaces;
using Core.Services;

namespace Core
{
    public class CoreModule : Module
    {

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<NumberProcessorService>()
                .As<INumberProcessorService>()
                .InstancePerLifetimeScope();

            builder.RegisterType<StringReverseService>()
                .As<IStringReverseService>()
                .InstancePerLifetimeScope();

            builder.RegisterType<FileHashService>()
                .As<IFileHashService>()
                .InstancePerLifetimeScope();

            builder.RegisterType<PriceGetterService>()
                .As<IPriceGetterService>()
                .InstancePerLifetimeScope();
        }
    }
}
