using ImGuiNET;
using Microsoft.Xna.Framework;
using Num = System.Numerics;

namespace CurryEngine.Editor.Rendering.ImGUI;

public class FSDialog
{
	public static bool FolderPicker(string name)
	{
		if (!ImGui.IsPopupOpen("###fs-picker-folder"))
		{
			ImGui.OpenPopup($"{name}###fs-picker-folder");
		}

		if (ImGui.BeginPopupModal($"{name}###fs-picker-folder"))
		{
			
			
			ImGui.EndPopup();
		}
		
		return false;
	}
}