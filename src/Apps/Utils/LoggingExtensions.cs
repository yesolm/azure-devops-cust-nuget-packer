using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yesolm.DevOps.Utils
{
    /// <summary>
    /// Just calls actual serilog methods. Just for a short writing.
    /// </summary>
    public static class LoggingExtensions
    { 
        public static void Log(this string str) => Serilog.Log.Logger.Information(str);
        public static void Log<T>(this string str, T propertyValue) => Serilog.Log.Logger.Information<T>(str, propertyValue);
        public static void Debug(this string str) => Serilog.Log.Logger.Debug(str);
        public static void Debug<T>(this string str, T propertyValue) => Serilog.Log.Logger.Debug<T>(str, propertyValue);
        public static void Warn(this string str) => Serilog.Log.Logger.Warning(str);
        public static void Warm<T>(this string str, T propertyValue) => Serilog.Log.Logger.Warning<T>(str, propertyValue);
        public static void Error(this string str) => Serilog.Log.Logger.Error(str);
        public static void Error<T>(this string str, T propertyValue) => Serilog.Log.Logger.Error<T>(str, propertyValue);
        public static void Log(this Exception ex,string str) => Serilog.Log.Logger.Error(ex, str);
    }
}
