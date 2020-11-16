using System;
using NServiceBus.Logging;

public class NullLogger :
    ILoggerFactory,
    ILog
{
    public static NullLogger Instance = new();

    public ILog GetLogger(Type type)
    {
        return Instance;
    }

    public ILog GetLogger(string name)
    {
        return Instance;
    }

    public void Debug(string message)
    {
    }

    public void Debug(string message, Exception exception)
    {
    }

    public void DebugFormat(string format, params object[] args)
    {
    }

    public void Info(string message)
    {
    }

    public void Info(string message, Exception exception)
    {
    }

    public void InfoFormat(string format, params object[] args)
    {
    }

    public void Warn(string message)
    {
    }

    public void Warn(string message, Exception exception)
    {
    }

    public void WarnFormat(string format, params object[] args)
    {
    }

    public void Error(string message)
    {
    }

    public void Error(string message, Exception exception)
    {
    }

    public void ErrorFormat(string format, params object[] args)
    {
    }

    public void Fatal(string message)
    {
    }

    public void Fatal(string message, Exception exception)
    {
    }

    public void FatalFormat(string format, params object[] args)
    {
    }

    public bool IsDebugEnabled{ get; } = false;
    public bool IsInfoEnabled { get; } = false;
    public bool IsWarnEnabled { get; }= false;
    public bool IsErrorEnabled { get; }= false;
    public bool IsFatalEnabled { get; }= false;
}