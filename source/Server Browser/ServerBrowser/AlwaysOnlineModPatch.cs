using StardewValley;

namespace ServerBrowser
{
	//AlwaysOnlineServer frequently calls Game1.options.setServerMode("online" or "offline"). "offline" will reset the server privacy to friends only
	class AlwaysOnlineModPatch : Patch
	{
		protected override PatchDescriptor GetPatchDescriptor() => new PatchDescriptor(typeof(StardewValley.Options), "setServerMode");

		public static void Postfix(string setting)
		{
			if (setting != "offline" && CoopMenuHolder.PublicCheckBox.IsChecked)
				Game1.server.setPrivacy(ServerPrivacy.Public);
		}
	}
}
