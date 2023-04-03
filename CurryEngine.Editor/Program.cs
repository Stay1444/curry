using System.Reflection;
using CurryEngine.Editor;
using Microsoft.Xna.Framework;
using Serilog;

{
    foreach (var resource in Assembly.GetAssembly(typeof(Program))!.GetManifestResourceNames()
                 .Where(x => x.StartsWith("CurryEngine.Editor.Resources.Lib")))
    {
        var fileName = resource.Replace("CurryEngine.Editor.Resources.Lib.", "");
        if (File.Exists(fileName)) continue;
        await using var fsr = Assembly.GetAssembly(typeof(Program))!.GetManifestResourceStream(resource);
        await using var fsw = File.OpenWrite(fileName);
        await fsr!.CopyToAsync(fsw);
        Console.WriteLine($"Extracted {resource}");
    }
}

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
