using StardewModdingAPI;

namespace BlueberryMushroomMachine
{
	class Config
	{
		public int MaximumDaysToMature { get; set; } = 4;

		public bool WorksInCellar { get; set; } = true;
		public bool WorksInFarmCave { get; set; } = true;
		public bool WorksInBuildings { get; set; } = false;
		public bool WorksInFarmHouse { get; set; } = false;
		public bool WorksInGreenhouse { get; set; } = false;
		public bool WorksOutdoors { get; set; } = false;
		public bool DisabledForFruitCave { get; set; } = true;
		public bool RecipeAlwaysAvailable { get; set; } = false;

		public bool Debugging { get; set; } = false;
		public SButton GivePropagatorKey { get; set; } = SButton.F9;
	}
}