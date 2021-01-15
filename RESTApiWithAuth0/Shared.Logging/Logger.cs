using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Shared.Logging
{
    public class Logger<T> where T : class
    {
        private readonly string moduleName;
        private readonly string className;

        public Logger()
        {
            this.moduleName = Path.GetFileNameWithoutExtension(typeof(T).Module.ToString());
            this.className = typeof(T).Name;
        }

        public void LogException(Exception ex, string message = "", [CallerMemberName] string methodName = "")
        {
        }

        public void LogInfo(string message, [CallerMemberName] string methodName = "")
        {
        }

        public void LogWarning(string message, [CallerMemberName] string methodName = "")
        {
        }

        public void LogError(string message, [CallerMemberName] string methodName = "")
        {
        }

        public void LogError(Exception ex, string message = "", [CallerMemberName] string methodName = "")
        {
        }

        public void LogDebug(string message, [CallerMemberName] string methodName = "")
        {
        }

        public void LogAlways(string message, [CallerMemberName] string methodName = "")
        {
        }

    }
}
