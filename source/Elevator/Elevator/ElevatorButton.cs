using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;
using System;

namespace Elevator
{
	internal class ElevatorButton : BaseTextButton
	{
		readonly Farmer player;

		public ElevatorButton(int x, int y, Farmer player) : base(x, y, player.Name)
		{
			this.player = player;

			if (player == Game1.player)
				baseColor = Color.LawnGreen;
		}

		public override void OnClicked()
		{
			Console.WriteLine($"Warping to {player.Name}'s cabin");
			
			Cabin cabin = CabinHelper.FindCabinInside(player);
			
			Game1.warpFarmer(cabin.uniqueName.Value, cabin.warps[0].X, cabin.warps[0].Y -1, 0);

			Game1.activeClickableMenu.exitThisMenu(false);
		}
	}
}
