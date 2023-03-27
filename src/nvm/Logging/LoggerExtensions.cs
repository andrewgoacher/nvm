namespace nvm.Logging;

public static class LoggerExtensions
{
    public static void LogDiagnostic(this ILogger logger, string message)
    {
        logger.Log(LogLevel.Diagnostic, message);
    }

    public static void LogDiagnostic(this ILogger logger, string message, params object[] args)
    {
        logger.Log(LogLevel.Diagnostic, message, args);
    }

    public static void LogInformation(this ILogger logger, string message)
    {
        logger.Log(LogLevel.Information, message);
    }

    public static void LogInformation(this ILogger logger, string message, params object[] args)
    {
        logger.Log(LogLevel.Information, message, args);
    }

    public static void LogWarning(this ILogger logger, string message)
    {
        logger.Log(LogLevel.Warning, message);
    }

    public static void LogWarning(this ILogger logger, string message, params object[] args)
    {
        logger.Log(LogLevel.Warning, message, args);
    }

    public static void LogError(this ILogger logger, string message)
    {
        logger.Log(LogLevel.Error, message);
    }

    public static void LogError(this ILogger logger, string message, params object[] args)
    {
        logger.Log(LogLevel.Error, message, args);
    }

    public static void LogCritical(this ILogger logger, string message)
    {
        logger.Log(LogLevel.Critical, message);
    }

    public static void LogCritical(this ILogger logger, string message, params object[] args)
    {
        logger.Log(LogLevel.Critical, message, args);
    }
}