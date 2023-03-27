namespace nvm.Logging;

public interface ILogger
{
    void Log(LogLevel level, string message);
    void Log(LogLevel level, string message, params object[] args);
}