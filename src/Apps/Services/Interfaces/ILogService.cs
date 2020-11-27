using Microsoft.Extensions.Configuration;
using Serilog;
using System;

namespace Yesolm.DevOps.Services
{
    public interface ILogService
    {
        ILogger Log { get; }
        void LogObject(object obj);
        void Create(Func<ILogger> func);
        void CloseAndFlush();
    }
}