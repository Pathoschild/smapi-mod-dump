/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Traktori7/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
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
		private readonly IMonitor monitor;
		private readonly QualityScrubberController controller;


		public QualityScrubberAutomationFactory(QualityScrubberController controller, IMonitor monitor)
		{
			this.monitor = monitor;
			this.controller = controller;
		}


		public IAutomatable? GetFor(SObject obj, GameLocation location, in Vector2 tile)
		{
			if (obj.Name == "Quality Scrubber")
			{
				return new QualityScrubberMachine(controller, monitor, obj, location, tile);
			}

			return null;
		}


		public IAutomatable? GetFor(TerrainFeature feature, GameLocation location, in Vector2 tile)
		{
			return null;
		}


		public IAutomatable? GetFor(Building building, BuildableGameLocation location, in Vector2 tile)
		{
			return null;
		}


		public IAutomatable? GetForTile(GameLocation location, in Vector2 tile)
		{
			return null;
		}
	}
}
