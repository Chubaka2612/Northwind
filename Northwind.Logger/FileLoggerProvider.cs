using Microsoft.Extensions.Logging;
using System;

public class FileLoggerProvider : ILoggerProvider
{
    private readonly string _filePath;
    private readonly LogLevel _minLogLevel;

    public FileLoggerProvider(string filePath, LogLevel minLogLevel)
    {
        _filePath = filePath;
        _minLogLevel = minLogLevel;
    }

    public ILogger CreateLogger(string categoryName) => new FileLogger(_filePath, _minLogLevel);

    public void Dispose() { }
}