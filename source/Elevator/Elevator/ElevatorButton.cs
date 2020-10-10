/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ilyaki/Elevator
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;
using System;

namespace Elevator
{
	internal class ElevatorButton : BaseTextButton
	{
		readonly Farmer player;
		readonly bool mailButton;

		public ElevatorButton(int x, int y, Farmer player, bool mailButton) : base(x, y, mailButton ?  "Open mailbox" : player.Name)
		{
			this.player = player;
			this.mailButton = mailButton;

			if (player == Game1.player || mailButton)
				baseColor = Color.LawnGreen;
		}

		public override void OnClicked()
		{
			if (mailButton)
			{
				Game1.getFarm().mailbox();
			}
			else
			{
				Console.WriteLine($"Warping to {player.Name}'s cabin");

				Cabin cabin = CabinHelper.FindCabinInside(player);

				Game1.warpFarmer(cabin.uniqueName.Value, cabin.warps[0].X, cabin.warps[0].Y - 1, 0);

				Game1.activeClickableMenu.exitThisMenu(false);
			}
		}
	}
}
