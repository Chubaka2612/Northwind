using Microsoft.Extensions.Logging;

public static class FileLoggerFactoryExtensions
{
    public static ILoggingBuilder AddFile(this ILoggingBuilder builder, string filePath, LogLevel minLogLevel = LogLevel.Information)
    {
        builder.AddProvider(new FileLoggerProvider(filePath, minLogLevel));
        return builder;
    }
}
