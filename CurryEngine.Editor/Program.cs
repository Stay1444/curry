
using CurryEngine.Editor;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    #if DEBUG
    .MinimumLevel.Debug()
    #endif
    .CreateLogger();

var editor = new CurryEditor();

editor.Run();