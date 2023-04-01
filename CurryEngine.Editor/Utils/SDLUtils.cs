using System.Runtime.InteropServices;

namespace CurryEngine.Editor.Utils;

public static class SDLUtils
{
    [DllImport("SDL2.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_MaximizeWindow(IntPtr window);
}