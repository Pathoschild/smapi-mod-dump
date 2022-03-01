/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/


using Leclair.Stardew.BetterCrafting;
using Leclair.Stardew.Common.Events;

using StardewModdingAPI.Events;

using StardewValley;

namespace Leclair.Stardew.BCSpaceCore {
	class ModEntry : ModSubscriber {

		[Subscriber]
		private void OnGameLaunched(object sender, GameLaunchedEventArgs e) {
			var api = Helper.ModRegistry.GetApi<IBetterCrafting>("leclair.bettercrafting");
			api.AddRecipeProvider(new SpaceCoreProvider(this));
		}

	}
}
