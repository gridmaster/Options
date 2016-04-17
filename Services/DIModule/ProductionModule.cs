// -----------------------------------------------------------------------
// <copyright file="ProductionModule.cs" company="Magic FireFly">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using Core.Interface;
using Logger;
using Ninject.Modules;

namespace Services.DIModule
{
    public class ProductionModule : NinjectModule
    {
        public override void Load()
        {
            Bind<ILogger>().To<ConsoleLogger>().InSingletonScope()
                .WithConstructorArgument("loglevel", LogLevelEnum.Fatal);
        }
    }
}
