/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tlitookilakin/WarpNetwork
**
*************************************************/

using StardewModdingAPI;

namespace WarpNetwork.api
{
	class CPIntegration
	{
		public static void AddTokens(IManifest manifest)
		{
			if (!ModEntry.helper.ModRegistry.IsLoaded("pathoschild.ContentPatcher"))
				return;
			var api = ModEntry.helper.ModRegistry.GetApi<IContentPatcherAPI>("Pathoschild.ContentPatcher");
			api.RegisterToken(manifest, "MenuEnabled", () => new[] { ModEntry.config.MenuEnabled.ToString() });
		}
	}
}
