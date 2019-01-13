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
        #endregion

        #region Constructors
        public RageScript()
        {
            //Add logging if script attribute has LoggerAttribute
            AddLogger();
        }
        #endregion

        #region Virtual
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
        /// <param name="rageScriptLogLevel"></param>
        /// <param name="args"></param>
        public virtual void Log(string message, RageLogLevel rageScriptLogLevel = RageLogLevel.Debug, params object[] args)
        {
            if (!_loggerEnabled) return;

            message = this.GetType().Name + ":" + message;            
            switch (rageScriptLogLevel)
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
        }

        /// <summary>
        /// Logs an exception, should use <see cref="Log(string, RageLogLevel, object[])"/>
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="msgTemplate"></param>
        public virtual void LogEx(Exception exception, string msgTemplate)
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
        public virtual void LogEx(Exception exception, string msgTemplate, bool console = true)
        {
            if (_loggerEnabled)
            {
                _logger.Error(exception, msgTemplate);

                if (console)
                    NAPI.Util.ConsoleOutput(msgTemplate + " " + exception?.Message);
            }                
        }

        public virtual void LogIncludeConsole(string message, bool logConsole = true, RageLogLevel rageScriptLogLevel = RageLogLevel.Debug, params object[] args)
        {
            if (logConsole)
                NAPI.Util.ConsoleOutput(message);

            Log(message, rageScriptLogLevel, args);
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

            Log($"starting logger: {this.GetType().Name}", RageLogLevel.Information);
        }
        #endregion
    }
}
