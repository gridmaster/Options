// -----------------------------------------------------------------------
// <copyright file="DesignTimeModule.cs" company="Magic FireFly">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using Core.Interface;
using Logger;
using Ninject.Modules;

namespace Services.DIModule
{
    public class DesignTimeModule : NinjectModule
    {
        public override void Load()
        {
            Bind<ILogger>().To<Log4NetWrapper>().InSingletonScope()
                .WithConstructorArgument("loglevel", LogLevelEnum.Info);
        }
    }
}
