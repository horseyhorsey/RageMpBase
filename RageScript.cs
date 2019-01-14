using GTANetworkAPI;
using RageMpBase.Attributes;
using Serilog;
using System;
using System.IO;

namespace RageMpBase
{
    /// <summary>
    /// Base RageMp with logging
    /// </summary>
    public abstract class RageScript : Script
    {
        #region Fields
        static ILogger _logger;
        bool _loggerEnabled = false;

        /// <summary>
        /// Holds the name to this loaded resource
        /// </summary>
        public string ResourceName { get; }
        #endregion

        #region Constructors
        public RageScript()
        {
            ResourceName = this.GetType().Name;

            //Add logging if script attribute has LoggerAttribute
            AddLogger();
        }
        #endregion

        #region Virtual

        /// <summary>
        /// Gets all the commands used by this resource
        /// </summary>
        protected virtual string[] GetResourceCommands(Client client)
        {
            var name = this.GetType().Name;
            client.SendChatMessage("Commands", $"{name} Commands:");
            return NAPI.Resource.GetResourceCommands(name);
        }

        /// <summary>
        /// Returns all available settings names from this resources meta.xml
        /// </summary>
        /// <returns></returns>
        public virtual string[] GetResourceSettings()
        {
            var thisResource = NAPI.Resource.GetThisResource(this);
            return NAPI.Resource.GetResourceSettings(thisResource);
        }

        /// <summary>
        /// Logs with Serilog to file if this script is using <see cref="LoggerAttribute"/>. Also prints to console
        /// </summary>
        /// <param name="message">Can be template "My message {variableName}"</param>
        /// <param name="loglvl"></param>
        /// <param name="console">Log to console?</param>
        /// <param name="args"></param>
        protected virtual void Log(string message, bool console = true, RageLogLevel loglvl = RageLogLevel.Debug, params object[] args)
        {
            if (!_loggerEnabled) return;

            message = this.GetType().Name + ":" + message;            
            switch (loglvl)
            {
                case RageLogLevel.Verbose:
                    _logger.Verbose(message, args);
                    break;
                case RageLogLevel.Debug:
                    _logger.Debug(message, args);
                    break;
                case RageLogLevel.Information:
                    _logger.Information(message, args);
                    break;
                case RageLogLevel.Warning:
                    _logger.Warning(message, args);
                    break;
                case RageLogLevel.Error:
                    _logger.Error(message, args);
                    break;
                case RageLogLevel.Fatal:
                    _logger.Fatal(message, args);
                    break;
                default:
                    break;
            }

            if (console)
                NAPI.Util.ConsoleOutput(message);
        }

        /// <summary>
        /// Logs an exception, should use <see cref="Log(string, RageLogLevel, object[])"/>
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="msgTemplate"></param>
        protected virtual void LogEx(Exception exception, string msgTemplate)
        {
            if (_loggerEnabled)
                _logger.Error(exception, msgTemplate);            
        }

        /// <summary>
        /// Logs an exception, should use <see cref="Log(string, RageLogLevel, object[])"/>
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="msgTemplate"></param>
        /// <param name="console"></param>
        protected virtual void LogEx(Exception exception, string msgTemplate, bool console = true)
        {
            if (_loggerEnabled)
            {
                _logger.Error(exception, msgTemplate);

                if (console)
                    NAPI.Util.ConsoleOutput(msgTemplate + " " + exception?.Message);
            }                
        }

        #endregion

        #region Private Methods
        /// <summary>
        /// Checks attributes to see if LoggerAttribute is available
        /// </summary>
        private void AddLogger()
        {
            var attributes = this.GetType().GetCustomAttributes(true);
            foreach (var attribute in attributes)
            {
                if (attribute.GetType() == typeof(LoggerAttribute))
                {
                    var att = attribute as LoggerAttribute;                    
                    _loggerEnabled = true;                    
                    StartLogger(att.RageLogLevel);
                    break;
                }
            }
        }

        private void StartLogger(RageLogLevel rageLogLevel = RageLogLevel.Debug)
        {
            if (_logger == null)
            {
                var logPath = Path.Combine(Directory.GetCurrentDirectory(), "Logs", "server.log");
                _logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.RollingFile(logPath, restrictedToMinimumLevel: (Serilog.Events.LogEventLevel)rageLogLevel)
                .CreateLogger();
            }

            Log($"starting logger: {ResourceName}", loglvl: RageLogLevel.Information);
        }
        #endregion
    }
}
