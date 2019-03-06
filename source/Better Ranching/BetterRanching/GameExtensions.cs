using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;

namespace BetterRanching
{
	public static class GameExtensions
	{
		public static bool CanBeRanched(this FarmAnimal animal, string toolName)
		{
			return animal.currentProduce.Value > 0
				&& (animal.age.Value >= animal.ageWhenMature.Value
				&& animal.toolUsedForHarvest.Value.Equals(toolName));
		}

		public static FarmAnimal GetSelectedAnimal(this Farm farm, Rectangle rectangle)
		{
			foreach (FarmAnimal farmAnimal in farm.animals.Values)
			{
				if (farmAnimal.GetBoundingBox().Intersects(rectangle))
				{
					return farmAnimal;
				}
			}
			return null;
		}

		public static FarmAnimal GetSelectedAnimal(this AnimalHouse house, Rectangle rectangle)
		{
			foreach (FarmAnimal farmAnimal in house.animals.Values)
			{
				if (farmAnimal.GetBoundingBox().Intersects(rectangle))
				{
					return farmAnimal;
				}
			}
			return null;
		}

		public static void OverwriteState(this IInputHelper input, SButton button, string message = null)
		{
			if (message != null)
				Game1.showRedMessage(message);
			input.Suppress(button);
		}

		public static bool HoldingOverridableTool()
		{
			return Game1.player.CurrentTool?.Name == GameConstants.Tools.MilkPail || Game1.player.CurrentTool?.Name == GameConstants.Tools.Shears;
		}

		public static bool IsClickableArea()
		{
			if (Game1.activeClickableMenu != null)
			{
				return false;
			}
			Point newPosition = Game1.getMousePosition();
			foreach (var screen in Game1.onScreenMenus)
			{
				if (screen.isWithinBounds(newPosition.X, newPosition.Y))
				{
					return false;
				}
			}
			return true;
		}
	}
}
