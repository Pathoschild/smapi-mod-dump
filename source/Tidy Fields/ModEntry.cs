/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/tophatsquid/sdv-tidy-fields
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace TidyFields
{
	public class MapData
	{
		private Dictionary<string, HashSet<Vector2>> lightCollection;

		public MapData()
		{
			lightCollection = new Dictionary<string, HashSet<Vector2>>();
		}

		public void enableLightOnObject(string mapName, Vector2 position)
		{
			if (lightCollection.ContainsKey(mapName))
			{
				if (!lightCollection[mapName].Contains(position))
				{
					lightCollection[mapName].Add(position);
				}
			}
			else
			{
				lightCollection[mapName] = new HashSet<Vector2>();
				lightCollection[mapName].Add(position);
			}
		}
		public void disableLightOnObject(string mapName, Vector2 position)
		{
			if (lightCollection.ContainsKey(mapName))
			{
				if (!lightCollection[mapName].Contains(position))
				{
					lightCollection[mapName].Remove(position);
				}
			}
		}

		public Dictionary<string, HashSet<Vector2>> getLightCollection()
		{
			return lightCollection;
		}
		public void setLightCollection(Dictionary<string, HashSet<Vector2>> newLightCollection)
		{
			lightCollection = newLightCollection;
		}
	}

	public class MapUtils
	{
		public static void saveMapData(string path, MapData md)
		{
			Dictionary<string, HashSet<Vector2>> lights = md.getLightCollection();
			Dictionary<string,Dictionary<string,HashSet<Vector2>>> data = new Dictionary<string,Dictionary<string,HashSet<Vector2>>>();
			data["lights"] = lights;
			ModEntry.SHelper.Data.WriteJsonFile(path, data);
		}
		public static MapData getMapData(string path)
		{
			MapData md = new MapData();
			Dictionary<string, Dictionary<string, HashSet<Vector2>>> data = ModEntry.SHelper.Data.ReadJsonFile<Dictionary<string, Dictionary<string, HashSet<Vector2>>>>(path) ?? new Dictionary<string, Dictionary<string, HashSet<Vector2>>>();
			if (data.ContainsKey("lights"))
			{
				md.setLightCollection(data["lights"]);
			}
			return md;
		}
	}
	public class ModEntry : Mod
	{
		public static ModConfig config_;
		// Dictionary, with ones dictionary called "lights", keyed by map name, containing a list of Vector2s of objects which contain torches
		public static MapData mapDict = new MapData();
		public static IModHelper SHelper;
		public static IMonitor SMonitor;
		/*********
        ** Public methods
        *********/
		/// <summary>The mod entry point, called after the mod is first loaded.</summary>
		/// <param name="helper">Provides simplified APIs for writing mods.</param>

		public override void Entry(IModHelper helper)
		{
			config_ = helper.ReadConfig<ModConfig>();
			// Initialize SMAPI helper and monitor
			SHelper = Helper;
			SMonitor = Monitor;

			mapDict = new MapData();

			// Register events
			helper.Events.Player.Warped += this.OnWarped;
			helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
			helper.Events.GameLoop.Saving += this.OnSaving;


			// Harmony instance
			var harmony = new Harmony(this.ModManifest.UniqueID);

			// Patches for Object.cs
			harmony.Patch(
			   AccessTools.Method(typeof(StardewValley.Object), nameof(StardewValley.Object.performRemoveAction)),
			   prefix: new HarmonyMethod(typeof(ObjectPatches), nameof(ObjectPatches.performRemoveAction_Prefix))
			);
			harmony.Patch(
			   AccessTools.Method(typeof(StardewValley.Object), nameof(StardewValley.Object.DayUpdate)),
			   prefix: new HarmonyMethod(typeof(ObjectPatches), nameof(ObjectPatches.DayUpdate_Prefix))
			);
			ObjectPatches.Initialize(SMonitor, SHelper);


			// Patches for Fence.cs
			harmony.Patch(
			   AccessTools.Method(typeof(StardewValley.Fence), nameof(StardewValley.Fence.performObjectDropInAction)),
			   prefix: new HarmonyMethod(typeof(FencePatches), nameof(FencePatches.performObjectDropInAction_Prefix))
			);
			FencePatches.Initialize(SMonitor, SHelper);
			
			// Patches for Game1.cs
			harmony.Patch(
				AccessTools.Method(typeof(StardewValley.Game1), nameof(StardewValley.Game1.pressUseToolButton)),
				prefix: new HarmonyMethod(typeof(Game1Patches), nameof(Game1Patches.pressUseToolButton_Prefix))
			);
			Game1Patches.Initialize(SMonitor, SHelper);

		}

		private void OnWarped(object sender, WarpedEventArgs e)
		{
			SMonitor.Log($"Warped to {e.NewLocation.Name}, updating lights", LogLevel.Trace);
			SMonitor.Log($"Mapdict: {mapDict}", LogLevel.Trace);
			SMonitor.Log($"Mapdict keys: {String.Join(", ", new List<string>(mapDict.getLightCollection().Keys))}", LogLevel.Trace);
			// Check lights
			if (mapDict.getLightCollection().ContainsKey(e.NewLocation.Name))
			{
				SMonitor.Log($"{mapDict.getLightCollection()[e.NewLocation.Name].Count} lights found", LogLevel.Trace);
				foreach (Vector2 c in mapDict.getLightCollection()[e.NewLocation.Name])
				{
					if (e.NewLocation.Objects.ContainsKey(c))
					{
						if (Game1.currentLocation.Objects[c].Name.Contains("arecrow") && !ModEntry.config_.place_torches_in_scarecrows)
						{
							SMonitor.Log($"Skipping adding light to {e.NewLocation.Name}: {c.X}, {c.Y} as it's a scarecrow and the config option 'place_torches_in_scarecrows' is disabled", LogLevel.Trace);
							continue;
						}
						if (Game1.currentLocation.Objects[c].IsSprinkler() && !ModEntry.config_.place_torches_in_sprinklers)
						{
							SMonitor.Log($"Skipping adding light to {e.NewLocation.Name}: {c.X}, {c.Y} as it's a sprinkler and the config option 'place_torches_in_sprinklers' is disabled", LogLevel.Trace);
							continue;
						}
						SMonitor.Log($"Adding light to {e.NewLocation.Name}: {c.X}, {c.Y}", LogLevel.Trace);
						Game1.currentLocation.Objects[c].lightSource = (new Torch(c, 1)).lightSource;
					}
					else
					{
						SMonitor.Log($"Can't add light to {e.NewLocation.Name}: {c.X}, {c.Y} [NO OBJECT FOUND]", LogLevel.Trace);
					}
				}
			}
			else
			{
				SMonitor.Log($"No lights found", LogLevel.Trace);
			}


			/*
			SMonitor.Log($"Warped to {e.NewLocation.Name}, checking sprinklers", LogLevel.Debug);
			// Check sprinklers and activate animation
			foreach (Vector2 c in e.NewLocation.objects.Keys)
			{
				StardewValley.Object parent_ob = e.NewLocation.objects[c];
				StardewValley.Object obj = parent_ob.heldObject;
				if (obj != null && new[] { 599, 621, 645 }.Contains(obj.ParentSheetIndex))
				{
					SMonitor.Log($"Checking held sprinkler at {c}", LogLevel.Debug);
					if (!Game1.player.team.SpecialOrderRuleActive("NO_SPRINKLER"))
					{
						parent_ob.ApplySprinklerAnimation(e.NewLocation);
					}
				}
			*/



		}
		private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
		{
			// Populate mapDict
			string modPath = SHelper.DirectoryPath;
			if (!Directory.Exists(Path.Combine(modPath, "save_data")))
				Directory.CreateDirectory(Path.Combine(modPath, "save_data"));
			string path = Path.Combine("save_data", Constants.SaveFolderName + "_map_data.json");
			SMonitor.Log($"Loading {modPath}\\{path}", LogLevel.Trace);
			mapDict = MapUtils.getMapData(path);
		}

		private void OnSaving(object sender, SavingEventArgs e)
		{
			// Save mapDict to save specific json file
			string modPath = SHelper.DirectoryPath;
			if (!Directory.Exists(Path.Combine(modPath, "save_data")))
				Directory.CreateDirectory(Path.Combine(modPath, "save_data"));
			string path = Path.Combine("save_data", Constants.SaveFolderName + "_map_data.json");
			SMonitor.Log($"Saving {modPath}\\{path}", LogLevel.Trace);
			MapUtils.saveMapData(path, mapDict);
		}
	}



	public class ObjectPatches
	{
		private static IMonitor Monitor;
		private static IModHelper Helper;

		// call this method from your Entry class
		public static void Initialize(IMonitor monitor, IModHelper helper)
		{
			Monitor = monitor;
			Helper = helper;
		}

		public static bool performRemoveAction_Prefix(StardewValley.Object __instance, Vector2 tileLocation, GameLocation environment)
		{
			if (__instance.lightSource != null && (__instance.Name.Contains("arecrow") || __instance.IsSprinkler()))
			{
				// Get object location (int)
				Vector2 c = tileLocation;
				c.X = (int)c.X;
				c.Y = (int)c.Y;

				// Remove light from map data
				ModEntry.mapDict.disableLightOnObject(Game1.currentLocation.Name, c);

				// Spawn torch
				int oldCount = Game1.currentLocation.debris.Count;
				Game1.createItemDebris(new Torch(__instance.TileLocation, 1), tileLocation * 64f, (Game1.player.FacingDirection + 2) % 4);
				int newCount = Game1.currentLocation.debris.Count;
			}
			return true;
		}

		// Make sprinklers that are held by objects, do sprinkler things overnight
		public static bool DayUpdate_Prefix(StardewValley.Object __instance, GameLocation location)
		{
			StardewValley.Object obj = __instance.heldObject;
			if (obj != null && new[] { 599, 621, 645 }.Contains(obj.ParentSheetIndex))
			{
				location.postFarmEventOvernightActions.Add(delegate
				{
					if (!Game1.player.team.SpecialOrderRuleActive("NO_SPRINKLER"))
					{
						string tiles_string = "";
						foreach (Vector2 current in obj.GetSprinklerTiles())
						{
							Vector2 current_modified = new Vector2(current.X + __instance.TileLocation.X, current.Y + __instance.TileLocation.Y);
							obj.ApplySprinkler(location, current_modified);
							tiles_string += $"({current_modified.X},{current_modified.Y}), ";
						}
						obj.ApplySprinklerAnimation(location);

					}
				});
			}
			return true;
		}
	}


	public class FencePatches
	{
		private static IMonitor Monitor;
		private static IModHelper Helper;

		// call this method from your Entry class
		public static void Initialize(IMonitor monitor, IModHelper helper)
		{
			Monitor = monitor;
			Helper = helper;
		}

		// Allow sprinklers to be "dropped in"
		public static bool performObjectDropInAction_Prefix(StardewValley.Object __instance, Item dropIn, bool probe, Farmer who)
		{
			if (!ModEntry.config_.place_sprinklers_on_fences)
			{
				Monitor.Log($"Skipping dropping sprinkler on fence as config option 'place_sprinklers_on_fences' is disabled", LogLevel.Trace);
				return true;
			}
			if (__instance.isTemporarilyInvisible)
			{
				return false;
			}
			if (dropIn is StardewValley.Object)
			{
				// If putting fence gate on fence with sprinkler then drop sprinkler
				if (__instance.heldObject.Value!=null && new[] { 599, 621, 645 }.Contains((int)__instance.heldObject.Value.ParentSheetIndex) && dropIn.Name=="Gate" )
				{
					Game1.createItemDebris(__instance.heldObject, __instance.TileLocation * 64f, (Game1.player.FacingDirection + 2) % 4);
					__instance.heldObject.Value = null;

				}

				// If putting sprinkler on fence (not fence gate!)
				if (new[] { 599, 621, 645 }.Contains((int)dropIn.ParentSheetIndex) && __instance.heldObject.Value == null && !ModEntry.SHelper.Reflection.GetField<Netcode.NetBool>(__instance, "isGate").GetValue())
				{
					if (!probe)
					{
						__instance.heldObject.Value = (StardewValley.Object)dropIn;
						who.currentLocation.playSound("axe");
						who.reduceActiveItemByOne();
					}
					return false;
				}
			}
			return true;
		}

	}


	public class Game1Patches
	{
		private static IMonitor Monitor;
		private static IModHelper Helper;

		// call this method from your Entry class
		public static void Initialize(IMonitor monitor, IModHelper helper)
		{
			Monitor = monitor;
			Helper = helper;
		}

		// Allow torches to be removed from scarecrows and sprinklers with them in
		public static bool pressUseToolButton_Prefix(StardewValley.Game1 __instance, ref bool __result)
		{
			if (Game1.player.CurrentTool == null && Game1.player.ActiveObject != null && Game1.player.ActiveObject.Name=="Torch")
			{
				Vector2 c = Game1.player.GetToolLocation() / 64f;
				c.X = (int)c.X;
				c.Y = (int)c.Y;
				if (Game1.currentLocation.Objects.ContainsKey(c))
				{
					StardewValley.Object o = Game1.currentLocation.Objects[c];
					if (o.lightSource == null && (o.Name.Contains("arecrow") || o.IsSprinkler()))
                    {
						// If config options are disabled then skip
						if (o.Name.Contains("arecrow") && !ModEntry.config_.place_torches_in_scarecrows)
						{
							Monitor.Log($"Skipping adding light to {Game1.currentLocation.Name}: {c.X}, {c.Y} as it's a scarecrow and the config option 'place_torches_in_scarecrows' is disabled", LogLevel.Trace);
							return true;
						}
						else if (o.IsSprinkler() && !ModEntry.config_.place_torches_in_sprinklers)
						{
							Monitor.Log($"Skipping adding light to {Game1.currentLocation.Name}: {c.X}, {c.Y} as it's a sprinkler and the config option 'place_torches_in_sprinklers' is disabled", LogLevel.Trace);
							return true;
						}
						else
						{
							// Add light to map data
							ModEntry.mapDict.enableLightOnObject(Game1.currentLocation.Name, c);
							// Take torch from inventory
							Game1.player.reduceActiveItemByOne();
							// Make light source in game
							Game1.currentLocation.Objects[c].lightSource = (new Torch(c, 1)).lightSource;
							return false;
						}
					}
					
				}
			}
			return true;
		}
	}


}
