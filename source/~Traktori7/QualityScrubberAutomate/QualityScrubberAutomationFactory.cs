using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;
using SObject = StardewValley.Object;
using Pathoschild.Stardew.Automate;
using QualityScrubber;


namespace QualityScrubberAutomate
{
	class QualityScrubberAutomationFactory : IAutomationFactory
	{
		private QualityScrubberController controller { get; set; }


		public QualityScrubberAutomationFactory(QualityScrubberController controller)
		{
			this.controller = controller;
		}


		public IAutomatable GetFor(SObject obj, GameLocation location, in Vector2 tile)
		{
			if (obj.Name == "Quality Scrubber")
			{
				return new QualityScrubberMachine(controller, obj, location, tile);
			}

			return null;
		}


		public IAutomatable GetFor(TerrainFeature feature, GameLocation location, in Vector2 tile)
		{
			return null;
		}


		public IAutomatable GetFor(Building building, BuildableGameLocation location, in Vector2 tile)
		{
			return null;
		}


		public IAutomatable GetForTile(GameLocation location, in Vector2 tile)
		{
			return null;
		}
	}
}
