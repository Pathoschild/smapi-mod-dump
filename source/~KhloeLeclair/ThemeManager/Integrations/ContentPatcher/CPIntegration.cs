/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

#nullable enable

using Leclair.Stardew.Common.Integrations;

using StardewValley;
using StardewModdingAPI;

using ContentPatcher;

namespace Leclair.Stardew.ThemeManager.Integrations.ContentPatcher;

public class CPIntegration : BaseAPIIntegration<IContentPatcherAPI, ModEntry> {

	public CPIntegration(ModEntry mod)
	: base(mod, "Pathoschild.ContentPatcher", "1.25") {
		if (!IsLoaded)
			return;

		API.RegisterToken(Self.ModManifest, "GameTheme", () => {
			if (Self.BaseThemeManager is null)
				return new[] {
					"default"
				};

			return new[] {
				Self.BaseThemeManager.ActiveThemeId
			};
		});
	}
}
