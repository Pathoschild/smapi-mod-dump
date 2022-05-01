/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using StardewValley;

namespace SpriteMaster;

static class GameState {
	internal static bool IsLoading => Game1.currentLoader is not null || Game1.gameMode == Game1.loadingMode;
	internal static volatile string CurrentSeason = "";
}
