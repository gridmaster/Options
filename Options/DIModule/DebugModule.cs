// -----------------------------------------------------------------------
// <copyright file="DebugModule.cs" company="Magic FireFly">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using Core.Interface;
using Logger;
using Ninject.Modules;
using Services.Interfaces;
using Services.Services;

namespace Options.DIModule
{
    class DebugModule : NinjectModule
    {
        public override void Load()
        {
            Bind<ILogger>().To<Log4NetLogger>().InSingletonScope()
                .WithConstructorArgument("loglevel", LogLevelEnum.Debug);

            Bind<IOptionService>().To<OptionService>().InSingletonScope();
        }
    }
}
