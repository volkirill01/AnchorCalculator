using NLog;

namespace UI.AnchorCalculator.Extensions;

public class LoggerManager
{
	private static NLog.ILogger m_Logger = LogManager.GetCurrentClassLogger();

	public void LogDebug(string message) => m_Logger.Debug(message);
	public void LogError(string message) => m_Logger.Error(message);
	public void LogInfo(string message) => m_Logger.Info(message);
	public void LogWarn(string message) => m_Logger.Warn(message);
}
