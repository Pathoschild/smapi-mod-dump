/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/thatnzguy/WindowResize
**
*************************************************/

using System.Reflection;
using Harmony;
using StardewModdingAPI;

namespace WindowResize {
	/// <summary>The mod entry point.</summary>
	public class ModEntry : Mod {
		/*********
        ** Public methods
        *********/
		/// <summary>The mod entry point, called after the mod is first loaded.</summary>
		/// <param name="helper">Provides simplified APIs for writing mods.</param>
		public override void Entry(IModHelper helper) {
			//Program.gamePtr.Window.ClientSizeChanged += WindowOnClientSizeChanged;

			var harmony = HarmonyInstance.Create("com.JoelSchroyen.SDV.WindowResize");
			harmony.PatchAll(Assembly.GetExecutingAssembly());
		}

//		private void WindowOnClientSizeChanged(object sender, EventArgs eventArgs) {
//			this.Monitor.Log("Size changed");
//			LogSize();
//		}
//
//		private void LogSize() {
//			this.Monitor.Log($"Size: {Program.gamePtr.Window.ClientBounds.Width} : {Program.gamePtr.Window.ClientBounds.Height}");
//		}
	}
}