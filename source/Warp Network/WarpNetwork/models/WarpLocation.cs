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
using Microsoft.Xna.Framework.Content;
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
		{
			get
			{
				try
				{
					return ModEntry.helper.GameContent.Load<Texture2D>(IconPath);
				} catch (ContentLoadException ex) 
				{
					ModEntry.monitor.Log(ex.ToString(), LogLevel.Debug);
				}
				return null;
			}
		}

		public bool IsAccessible(GameLocation context, Farmer who)
			=> Condition is not null && GameStateQuery.CheckConditions(Condition, context, who);

		public bool IsVisible(GameLocation context, Farmer who)
			=> DisplayCondition is null || GameStateQuery.CheckConditions(DisplayCondition, context, who);

		public Point GetLandingPoint(Farmer who = null, bool fromWand = false)
		{
			if (Location.Equals("Farm", System.StringComparison.OrdinalIgnoreCase))
				if (fromWand && who is not null)
					return Utility.getHomeOfFarmer(who).getFrontDoorSpot();

			return 
				!OverrideMapProperty || Position == default ?
				Utils.GetTargetTile(Game1.getLocationFromName(Location), Position) : 
				Position;
		}

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

			var tile = GetLandingPoint(who, WarpHandler.fromWand.Value);

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
