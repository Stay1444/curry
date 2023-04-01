using CurryEngine.Editor;
using Microsoft.Xna.Framework;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    #if DEBUG
    .MinimumLevel.Debug()
    #endif
    .CreateLogger();

FNALoggerEXT.LogInfo = Log.Debug;
FNALoggerEXT.LogError = Log.Error;
FNALoggerEXT.LogWarn = Log.Warning;

var editor = new CurryEditor();

editor.Run();
