/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ilyaki/BattleRoyalley
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BattleRoyale
{
    class MiniPortraitOnMapDisabler : Patch
    {
        protected override PatchDescriptor GetPatchDescriptor() => new PatchDescriptor(typeof(MapPage), "drawMiniPortraits");
		
		public static bool Prefix(MapPage __instance, SpriteBatch b)
		{
			List<Farmer> playersToShow = new List<Farmer>();
			if (ModEntry.BRGame.IsGameInProgress && ModEntry.BRGame.alivePlayers != null && Game1.player != null && ModEntry.BRGame.alivePlayers.Contains(Game1.player.UniqueMultiplayerID))
			{
				//As an alive player, only show the local player
				playersToShow.Add(Game1.player);
			}
			else if (ModEntry.BRGame.IsGameInProgress && SpectatorMode.InSpectatorMode && ModEntry.BRGame.alivePlayers != null)
			{
				//As a spectator, only show alive players

				foreach (long id in ModEntry.BRGame.alivePlayers)
				{
					try
					{
						var f = Game1.getOnlineFarmers().First(x => x.UniqueMultiplayerID == id);
						playersToShow.Add(f);
					}
					catch(Exception) { }
				}
			}

			//

			Dictionary<Vector2, int> usedPositions = new Dictionary<Vector2, int>();
			foreach (Farmer onlineFarmer in playersToShow)
			{
				Vector2 pos2 = __instance.getPlayerMapPosition(onlineFarmer) - new Vector2(32f, 32f);
				int count = 0;
				usedPositions.TryGetValue(pos2, out count);
				usedPositions[pos2] = count + 1;
				pos2 += new Vector2((float)(48 * (count % 2)), (float)(48 * (count / 2)));
				onlineFarmer.FarmerRenderer.drawMiniPortrat(b, pos2, 0.00011f, 4f, 2, onlineFarmer);
			}

			return false;
		}
    }
}
