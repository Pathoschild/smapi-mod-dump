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
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using WarpNetwork.api;
using WarpNetwork.models;

namespace WarpNetwork
{
	class WarpHandler
	{
		internal static readonly PerScreen<string> wandLocation = new();
		internal static readonly PerScreen<Point> wandTile = new();
		private static readonly WarpNetHandler returnHandler = new(
			() => wandLocation.Value is not null, 
			() => "RETURN", 
			() => ModEntry.i18n.Get("dest.return"), 
			ReturnToPrev);
		
		internal static void Init()
		{
			GameLocation.RegisterTileAction("warpnetwork", WarpNetAction);
			GameLocation.RegisterTileAction("warpnetworkto", WarpToAction);
			GameLocation.RegisterTouchAction("warpnetworkto", (w, s, f, l) => DirectWarp(s.Length is 0 ? string.Empty : s[0]));
			ModEntry.helper.Events.GameLoop.DayEnding += Cleanup;
			ModEntry.helper.Events.GameLoop.ReturnedToTitle += Cleanup;
		}
		private static void Cleanup(object sender, object ev){
			wandTile.ResetAllScreens();
			wandLocation.ResetAllScreens();
		}
		private static bool WarpNetAction(GameLocation where, string[] split, Farmer who, Point tile)
		{
			var id = split.Length is 0 ? string.Empty : split[0];
			ShowWarpMenu(id);
			return true;
		}
		private static bool WarpToAction(GameLocation where, string[] split, Farmer who, Point tile)
		{
			var id = split.Length is 0 ? string.Empty : split[0];
			DirectWarp(id);
			return true;
		}
		public static void ShowWarpMenu(string exclude = "", bool consume = false)
		{
			ModEntry.monitor.Log($"Activating menu; exclude: {exclude}, consume: {consume}");
			if (!ModEntry.config.MenuEnabled)
			{
				ShowFailureText();
				ModEntry.monitor.Log("Warp menu is disabled in config, menu not displayed.");
				return;
			}

			List<WarpLocation> dests = new();
			Dictionary<string, WarpLocation> locs = Utils.GetWarpLocations();

			if (locs.ContainsKey(exclude))
			{
				if (!locs.IsAccessible(exclude) && !ModEntry.config.AccessFromDisabled)
				{
					ShowFailureText();
					ModEntry.monitor.Log("Access from locked locations is disabled, menu not displayed.");
					return;
				}
			}
			if (exclude.Equals("_wand", StringComparison.OrdinalIgnoreCase) && ModEntry.config.WandReturnEnabled && wandLocation.Value is not null)
			{
				var dest = Game1.getLocationFromName(wandLocation.Value);
				if (dest is not null)
					dests.Add(new CustomWarpLocation(returnHandler));
			}
			foreach ((string id, WarpLocation loc) in locs)
			{
				if (
					!loc.AlwaysHide && (
					exclude.Equals("_force", StringComparison.OrdinalIgnoreCase) ||
					(locs.IsAccessible(id) && !id.Equals(exclude, StringComparison.OrdinalIgnoreCase)) ||
					(id.Equals("farm", StringComparison.OrdinalIgnoreCase) && exclude.Equals("_wand", StringComparison.OrdinalIgnoreCase))
					)
				)
				{
					if (loc is CustomWarpLocation || Game1.getLocationFromName(loc.Location) != null)
						dests.Add(locs[id]);
					else
						ModEntry.monitor.Log("Invalid Location name '" + loc.Location + "'; skipping entry.", LogLevel.Warn);
				}
			}
			if (dests.Count == 0)
			{
				ModEntry.monitor.Log("No valid warp destinations, menu not displayed.");
				ShowFailureText();
				return;
			}
			Game1.activeClickableMenu = new WarpMenu(dests, (WarpLocation where) =>
			{
				WarpToLocation(where, exclude.Equals("_wand", StringComparison.OrdinalIgnoreCase));
				if (consume)
					Game1.player.reduceActiveItemByOne();
			});
			return;
		}
		internal static void ShowFailureText()
			=> Game1.drawObjectDialogue(Game1.parseText(ModEntry.i18n.Get("ui-fail")));
		internal static void ShowFestivalNotReady()
			=> Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2973"));
		private static void ReturnToPrev()
		{
			(int x, int y) = wandTile.Value;
			string loc = wandLocation.Value;
			//MUST copy to preserve. warp is called on delay and values may change.
			DoWarpEffects(() => Game1.warpFarmer(loc, x, y, false));
		}
		public static bool DirectWarp(string prop)
		{
			var args = prop.Split(' ');
			if (args.Length > 0)
				return DirectWarp(args[0], args.Length > 1);

			ModEntry.monitor.Log("Warning! Map '" + Game1.currentLocation.Name + "' has invalid WarpNetworkTo property! Location MUST be specified!", LogLevel.Warn);
			return false;
		}
		public static bool DirectWarp(string location, bool force)
		{
			if (location is null)
			{
				ModEntry.monitor.Log("Destination is null! Cannot warp!", LogLevel.Error);
				ShowFailureText();
				return false;
			}
			if (location.ToLowerInvariant() == "_return")
			{
				if (returnHandler.getEnabled())
					returnHandler.Warp();
				else
					return false;
				return true;
			}
			Dictionary<string, WarpLocation> locs = Utils.GetWarpLocations();
			if (locs.TryGetValue(location, out var loc))
			{
				if (Game1.getLocationFromName(loc.Location) is not null)
				{
					if (Game1.whereIsTodaysFest != loc.Location || Utility.getStartTimeOfFestival() < Game1.timeOfDay)
					{
						if (force || loc.Enabled)
						{
							WarpToLocation(loc);
							return true;
						}
						else
						{
							ShowFailureText();
							return false;
						}
					}
					else
					{
						ModEntry.monitor.Log($"Failed to warp to '{loc.Location}': Festival at location not ready.", LogLevel.Debug);
						ShowFestivalNotReady();
						return false;
					}
				}
				else
				{
					ModEntry.monitor.Log($"Failed to warp to '{loc.Location}': Location with that name does not exist!", LogLevel.Error);
					ShowFailureText();
					return false;
				}
			}
			else
			{
				ModEntry.monitor.Log($"Warp to '{location}' failed: warp network location not registered with that name", LogLevel.Warn);
				ShowFailureText();
				return false;
			}
		}
		internal static void WarpToLocation(WarpLocation where, bool fromWand = false)
		{
			if (where is CustomWarpLocation custom)
			{
				var name = Game1.currentLocation.NameOrUniqueName;
				var tile = Game1.player.TilePoint;
				custom.handler.Warp();
				if (custom.handler == returnHandler)
				{
					wandLocation.Value = null;
				}
				else
				{
					wandLocation.Value = name;
					wandTile.Value = tile;
				}
				return;
			}
			if (Game1.currentLocation.Name != "Temp")
			{
				wandLocation.Value = Game1.currentLocation.NameOrUniqueName;
				wandTile.Value = Game1.player.TilePoint;
			}
			int x = where.X;
			int y = where.Y;
			if (where.Location == "Farm")
			{
				if (fromWand)
				{
					Point dest = GetFrontDoor(Game1.player);
					DoWarpEffects(() => Game1.warpFarmer("Farm", dest.X, dest.Y, false));
					return;
				}
				Point farmTotem = Utils.GetActualFarmPoint(x, y);
				x = farmTotem.X;
				y = farmTotem.Y;
			}
			if (!where.OverrideMapProperty)
			{
				Point coords = Game1.getLocationFromName(where.Location).GetPropertyPosition("WarpNetworkEntry", new(x, y));
				DoWarpEffects(() => Game1.warpFarmer(where.Location, coords.X, coords.Y, false));
			}
			else
			{
				DoWarpEffects(() => Game1.warpFarmer(where.Location, x, y, false));
			}
		}
		private static Point GetFrontDoor(Farmer who)
		{
			FarmHouse home = Utility.getHomeOfFarmer(who);
			if (home is not null)
				return home.getFrontDoorSpot();
			return Game1.getLocationFromName("Farm").GetPropertyPosition("FarmHouseEntry", new(64, 15));
		}
		private static void DoWarpEffects(Action action)
		{
			Farmer who = Game1.player;
			// reflection
			Multiplayer mp = ModEntry.helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();
			// --
			for (int index = 0; index < 12; ++index)
				mp.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite(
					354,
					Game1.random.Next(25, 75), 6, 1,
					new Vector2(Game1.random.Next((int)who.Position.X - 256, (int)who.Position.X + 192), Game1.random.Next((int)who.Position.Y - 256, (int)who.Position.Y + 192)),
					false,
					Game1.random.NextDouble() < 0.5)
					);
			who.currentLocation.playSound("wand", who.Position);
			Game1.displayFarmer = false;
			who.temporarilyInvincible = true;
			who.temporaryInvincibilityTimer = -2000;
			who.freezePause = 1000;
			Game1.flashAlpha = 1f;
			DelayedAction.fadeAfterDelay(new Game1.afterFadeFunction(() =>
			{
				action();
				Game1.changeMusicTrack("none");
				Game1.fadeToBlackAlpha = 0.99f;
				Game1.screenGlow = false;
				Game1.player.temporarilyInvincible = false;
				Game1.player.temporaryInvincibilityTimer = 0;
				Game1.displayFarmer = true;
			}), 1000);
			new Rectangle(who.GetBoundingBox().X, who.GetBoundingBox().Y, 64, 64).Inflate(192, 192);
			int num = 0;
			var tile = who.TilePoint;
			for (int index = tile.X + 8; index >= tile.X - 8; --index)
			{
				mp.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite(6, new Vector2(index, tile.Y) * 64f, Color.White, 8, false, 50f, 0, -1, -1f, -1, 0)
				{
					layerDepth = 1f,
					delayBeforeAnimationStart = num * 25,
					motion = new Vector2(-0.25f, 0.0f)
				});
				++num;
			}
		}
	}
}
