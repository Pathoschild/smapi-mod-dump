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
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleRoyale
{
	/* When a menu is open, MovePosition returns immediately. 
	 * In the method is the code for increasing temporaryInvincibilityTimer, hence the player is invincible while the menu is open.
	 * 
	 * Fix by running the code when a menu is open
	 */

	class MenuInvincibilityBugFix : Patch
	{
		protected override PatchDescriptor GetPatchDescriptor() => new PatchDescriptor(typeof(Farmer), "MovePosition");

		public static bool Prefix(ref bool ___temporarilyInvincible, ref int ___temporaryInvincibilityTimer, GameTime time)
		{
			if (Game1.activeClickableMenu != null)
			{
				TimeSpan elapsedGameTime;
				if (___temporarilyInvincible)
				{
					int num11 = ___temporaryInvincibilityTimer;
					elapsedGameTime = time.ElapsedGameTime;
					___temporaryInvincibilityTimer = num11 + elapsedGameTime.Milliseconds;
					if (___temporaryInvincibilityTimer > 1200)
					{
						___temporarilyInvincible = false;
						___temporaryInvincibilityTimer = 0;
					}
				}
			}

			return true;
		}
	}
}
