using System.Reflection;

namespace MediatR.Examples.Autofac
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using global::Autofac;
    using global::Autofac.Features.Variance;

    internal static class Program
    {
        private static void Main(string[] args)
        {
            var mediator = BuildMediator();

            Runner.Run(mediator, Console.Out);

            Console.ReadKey();
        }

        private static IMediator BuildMediator()
        {
            var builder = new ContainerBuilder();
            builder.RegisterSource(new ContravariantRegistrationSource());
            builder.RegisterAssemblyTypes(typeof (IMediator).GetTypeInfo().Assembly).AsImplementedInterfaces();
            builder.RegisterAssemblyTypes(typeof (Ping).GetTypeInfo().Assembly).AsImplementedInterfaces();
            builder.RegisterInstance(Console.Out).As<TextWriter>();
            builder.Register<SingleInstanceFactory>(ctx =>
            {
                var c = ctx.Resolve<IComponentContext>();
                return t => c.Resolve(t);
            });
            builder.Register<MultiInstanceFactory>(ctx =>
            {
                var c = ctx.Resolve<IComponentContext>();
                return t => (IEnumerable<object>) c.Resolve(typeof (IEnumerable<>).MakeGenericType(t));
            });

            var mediator = builder.Build().Resolve<IMediator>();

            return mediator;
        }
    }
}