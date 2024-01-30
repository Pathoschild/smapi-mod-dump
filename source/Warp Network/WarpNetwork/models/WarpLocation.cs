/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tlitookilakin/WarpNetwork
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using StardewModdingAPI;
using StardewValley;
using WarpNetwork.api;
using WarpNetwork.framework;
using xTile.Dimensions;

namespace WarpNetwork.models
{
	public class WarpLocation : IWarpNetAPI.IDestinationHandler
	{
		public string Location { set; get; }
		public Point Position { set; get; }
		public string Condition { set; get; } = "TRUE";
		public string Label { set; get; } = "Unnamed";
		public bool OverrideMapProperty { set; get; } = false;
		public string DisplayCondition { get; set; }
		public string IconPath { set; get; } = "";

		[JsonIgnore]
		public Texture2D Icon
			=> ModEntry.helper.GameContent.Load<Texture2D>(IconPath);

		public bool IsAccessible(GameLocation context, Farmer who)
			=> Condition is not null && GameStateQuery.CheckConditions(Condition, context, who);

		public bool IsVisible(GameLocation context, Farmer who)
			=> DisplayCondition is null || GameStateQuery.CheckConditions(DisplayCondition, context, who);

		public bool Activate(GameLocation location, Farmer who)
		{
			if (Game1.getLocationFromName(Location) is not null)
				if (!Utility.isFestivalDay() || Game1.whereIsTodaysFest != Location || Utility.getStartTimeOfFestival() < Game1.timeOfDay)
					return DoWarp(location, who);
				else
					ModEntry.monitor.Log($"Failed to warp to '{Location}': Festival at location not ready.", LogLevel.Debug);
			else
				ModEntry.monitor.Log($"Failed to warp to '{Location}': Location with that name does not exist!", LogLevel.Error);

			return false;
		}

		private bool DoWarp(GameLocation where, Farmer who)
		{
			if (!IsAccessible(where, who))
				return false;

			Point tile = Position;
			if (Location is "Farm")
			{
				if (WarpHandler.fromWand.Value)
				{
					Point dest = Utility.getHomeOfFarmer(who).getFrontDoorSpot();
					API.api.DoWarpEffects(() => Game1.warpFarmer("Farm", dest.X, dest.Y, false), who, where);
					return true;
				}
				Utils.TryGetActualFarmPoint(ref tile);
			}

			if (!OverrideMapProperty || tile == default)
			{
				var loc = Game1.getLocationFromName(Location);
				if (loc.TryGetMapPropertyAs("WarpNetworkEntry", out Point c) || Utils.TryGetDefaultPosition(Location, out c))
					tile = c;
			}

			if (tile == default)
			{
				ModEntry.monitor.Log($"Failed to warp to '{Location}': could not find landing point!", LogLevel.Warn);
				return false;
			}

			API.api.DoWarpEffects(() => Game1.warpFarmer(Location, tile.X, tile.Y, false), who, where);
			return true;
		}

		public void AfterWarp(string location, Point tile, IWarpNetAPI.IDestinationHandler handler)
		{
			// do nothing
		}
	}
}
