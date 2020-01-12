using StardewModdingAPI;

namespace SailorStyles_Hair
{
	public class ModEntry : Mod
	{
		internal static IModHelper SHelper;
		internal static IMonitor SMonitor;

		public override void Entry(IModHelper helper)
		{
			SMonitor = Monitor;
			SHelper = helper;
			helper.Content.AssetEditors.Add(new Editors.HairstylesEditor());
		}
	}
}
