// -----------------------------------------------------------------------
// <copyright file="BaseService.cs" company="Magic FireFly">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Runtime.CompilerServices;
using Core.Business;
using Core.Interface;
using DIContainer;

namespace Services.Services
{
    public abstract class BaseService
    {
        #region Fields
        protected readonly ILogger logger;
        #endregion

        #region Properties

        public bool IsInitialized { get; protected set; }

        #endregion

        #region Protected/Private Methods
        protected BaseService(ILogger logger)
        {
            this.logger = logger;
        }

        // was protected as a base class
        public string GetThisMethodName([CallerMemberName] string methodName = null)
        {
            return string.Format("{0}.{1}", GetType().Name, (methodName ?? "?"));
        }

        protected void ThrowIfNotInitialized([CallerMemberName] string methodName = null)
        {
            if (IsInitialized)
            {
                return;
            }
            string errorMessage = string.Format(
                "The {0}() method cannot be executed because the {1} has not yet been initialized.",
                methodName,
                GetType().Name);

            logger.Error(errorMessage);
            throw new Exception(errorMessage);
        }

        protected void ThrowIfIsInitialized([CallerMemberName] string methodName = null)
        {
            if (!IsInitialized)
            {
                return;
            }

            string errorMessage = string.Format(
                "The {0}() method cannot be executed because the {1} has already been initialized.",
                methodName,
                GetType().Name);

            logger.Error(errorMessage);
            throw new Exception(errorMessage);
        }

        public string GetWebPage(string uri)
        {
            IOCContainer.Instance.Get<ILogger>().InfoFormat("{0}Optionss is fetching uri: {0}{1}{0}", Environment.NewLine, uri);

            string webData = string.Empty;

            try
            {
                webData = WebWorks.GetResponse(uri);
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Unable to get {1}{0}Error: {2}{0}{3}"
                                        , Environment.NewLine
                                        , uri
                                        , ex.Message
                                        , GetThisMethodName());
                webData = ex.Message;
            }

            return webData;
        }
        #endregion
    }
}
