using Serilog.Events;

namespace CurryEngine.Editor.UI;

public class VLogger
{
    public List<KeyValuePair<LogEventLevel, string>> Logs = new List<KeyValuePair<LogEventLevel, string>>();

    public void Log(LogEventLevel level, string message)
    {
        Logs.Add(new KeyValuePair<LogEventLevel, string>(level, message));
    }    
}