using nvm.Logging;

namespace nvm.Console;

public class ConsoleLogger : ILogger
{
    private readonly LogLevel _minimalLevel;

    public ConsoleLogger(LogLevel minimalLevel)
    {
        _minimalLevel = minimalLevel;
    }

    public void Log(LogLevel level, string message)
    {
        Log(level, message, Array.Empty<object>());
    }

    public void Log(LogLevel level, string message, params object[] args)
    {
        if (level < _minimalLevel)
        {
            return;
        }

        WriteHandle(level);
        var text = args?.Length > 0 ?
            string.Format(message, args) :
            message;

        System.Console.WriteLine(text);
    }

    private static void WriteHandle(LogLevel level)
    {
        var color = System.Console.ForegroundColor;

        var handleColor = level switch
        {
            LogLevel.Diagnostic => System.ConsoleColor.Cyan,
            LogLevel.Information => System.ConsoleColor.White,
            LogLevel.Warning => System.ConsoleColor.Yellow,
            LogLevel.Error or LogLevel.Critical => System.ConsoleColor.Red,
            _ => ConsoleColor.White
        };

        var handle = level switch
        {
            LogLevel.Diagnostic => "[Diag]: ",
            LogLevel.Information => "[Info]: ",
            LogLevel.Warning => "[Warn]: ",
            LogLevel.Error => "[Error]: ",
            LogLevel.Critical => "[Critical]: ",
            _ => "[Info]: "
        };

        System.Console.ForegroundColor = handleColor;
        System.Console.Write(handle);
        System.Console.ForegroundColor = color;
    }
}
