using System;
using Core.Interface;
using Core.Models;
using DIContainer;
using Options.DIModule;
using Ninject;
using Services.Interfaces;

namespace Options
{
    internal class Program
    {
        #region initialize DI container

        private static void InitializeDiContainer()
        {
            NinjectSettings settings = new NinjectSettings
                {
                    LoadExtensions = false
                };

            // change DesignTimeModule to run other implementation ProductionModule || DebugModule
            IOCContainer.Instance.Initialize(settings, new DebugModule());
        }

        #endregion Initialize DI Container

        private static void Main(string[] args)
        {
            Console.WriteLine("{0}: Program startup...", DateTime.Now);

            try
            {
                InitializeDiContainer();

                IOCContainer.Instance.Get<ILogger>()
                            .InfoFormat("{0}********************************************************************************{0}", Environment.NewLine);
                IOCContainer.Instance.Get<ILogger>().InfoFormat("{0}Main's runnin'...{0}", Environment.NewLine);

                var sup = IOCContainer.Instance.Get<IOptionService>().GetOptions();

                IOCContainer.Instance.Get<ILogger>().InfoFormat("{0}Data collecting complete...{0}", Environment.NewLine);

            }
            catch (Exception ex)
            {
                IOCContainer.Instance.Get<ILogger>().Fatal("Sucker blew up: {0}", ex);
            }
            finally
            {
                IOCContainer.Instance.Get<ILogger>().InfoFormat("{0}Main's done'...{0}", Environment.NewLine);
                IOCContainer.Instance.Get<ILogger>()
                            .InfoFormat(
                                "{0}********************************************************************************{0}",
                                Environment.NewLine);

               Console.ReadKey();
            }
        }
    }
}
