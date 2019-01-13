using System;

namespace RageMpBase.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class LoggerAttribute : Attribute
    {
        public RageLogLevel RageLogLevel { get; }

        public LoggerAttribute(RageLogLevel rageLogLevel = RageLogLevel.Debug)
        {
            RageLogLevel = rageLogLevel;
        }
    }
}
