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

namespace BattleRoyale
{
	class WarpFarmerListener : Patch
	{
		protected override PatchDescriptor GetPatchDescriptor() => new PatchDescriptor(typeof(Game1), "warpFarmer", new Type[] { typeof(LocationRequest), typeof(int), typeof(int), typeof(int) });

		public static bool Prefix(ref LocationRequest locationRequest, ref int tileX, ref int tileY)
		{
			Console.WriteLine($"warpFarmer called to {locationRequest?.Location.Name ?? "<NULL>"}");
			SpectatorMode.OnWarped(locationRequest);
			ModEntry.BRGame.AddKnockbackImmunity();

			if (!SpectatorMode.InSpectatorMode)
			{
				if (locationRequest.Location.Name == "Tunnel")
				{
					

					var m = Game1.player.mount;
					Game1.player.mount = null;
					Game1.warpFarmer("Desert", 10000, 45, false);

					

					Game1.player.canMove = false;
					System.Threading.Tasks.Task.Factory.StartNew(() =>
					{
						System.Threading.Thread.Sleep(1000);
						Game1.player.position.X = 46.5f * Game1.tileSize;
						Game1.player.position.Y = 27 * Game1.tileSize;
						Game1.player.canMove = true;

						m.position.X = Game1.player.Position.X;
						m.position.Y = Game1.player.Position.Y;
						Game1.player.mount = m;
					});
					
					return false;
				}
			}

			return true;
		}

		public static void Postfix()
		{
			
		}
	}
}
