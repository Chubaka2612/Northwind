using Microsoft.Extensions.Logging;
using System;
using System.IO;

public class FileLogger : ILogger
{
    private readonly string _filePath;
    private readonly LogLevel _minLogLevel;

    public FileLogger(string filePath, LogLevel minLogLevel)
    {
        _filePath = filePath;
        _minLogLevel = minLogLevel;
    }

    public IDisposable BeginScope<TState>(TState state) => null;

    public bool IsEnabled(LogLevel logLevel) => logLevel >= _minLogLevel;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        if (!IsEnabled(logLevel)) return;

        var message = formatter(state, exception);
        if (!string.IsNullOrWhiteSpace(message))
        {
            var logMessage = $"{DateTime.UtcNow:O} [{logLevel}] {message}";
            File.AppendAllText(_filePath, logMessage + Environment.NewLine);
        }
    }
}
