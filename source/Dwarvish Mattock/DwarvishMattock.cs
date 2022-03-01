/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AHilyard/DwarvishMattock
**
*************************************************/

using System;
using System.Reflection.Emit;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using HarmonyLib;
using xTile.Tiles;
using System.Xml.Serialization;
using System.IO;
using StardewValley.Objects;
using StardewValley.Buildings;
using StardewValley.Locations;

namespace DwarvishMattock
{
	/// <summary>The mod entry point.</summary>
	public class ModEntry : Mod, IAssetLoader, IAssetEditor
	{
		public static IMonitor M = null;
		public static Texture2D mattockIcons = null;

		public static Texture2D clintSheet = null;

		private static bool providesBlacksmithEvents = false;

		
		/*********
		** Public methods
		*********/

		/// <summary>Get whether this instance can load the initial version of the given asset.</summary>
		/// <param name="asset">Basic metadata about the asset being loaded.</param>
		public bool CanLoad<T>(IAssetInfo asset)
		{
			// Ensure we can load the new mattock graphics.
			return asset.AssetNameEquals(Helper.Content.GetActualAssetKey("assets/mattocks.png", ContentSource.ModFolder)) ||
				   asset.AssetNameEquals(Helper.Content.GetActualAssetKey("Characters/Clint2", ContentSource.GameContent));
		}

		/// <summary>Load a matched asset.</summary>
		/// <param name="asset">Basic metadata about the asset being loaded.</param>
		public T Load<T>(IAssetInfo asset)
		{
			// Load the new mattock graphics!
			if (asset.AssetNameEquals(Helper.Content.GetActualAssetKey("assets/mattocks.png", ContentSource.ModFolder)))
			{
				return (T) Convert.ChangeType(mattockIcons, typeof(T));
			}

			if (asset.AssetNameEquals(Helper.Content.GetActualAssetKey("Characters/Clint2", ContentSource.GameContent)))
			{
				return (T) Convert.ChangeType(clintSheet, typeof(T));
			}

			throw new InvalidOperationException($"Unexpected asset '{asset.AssetName}'.");
		}

		/// <summary>Get whether this instance can edit the given asset.</summary>
		/// <param name="asset">Basic metadata about the asset being loaded.</param>
		public bool CanEdit<T>(IAssetInfo asset)
		{
			return (asset.AssetNameEquals("Data/weapons") || 
					asset.AssetNameEquals("Data/ObjectInformation") ||
					asset.AssetNameEquals("Data/NPCGiftTastes") ||
					asset.AssetNameEquals("Maps/springobjects") ||
					asset.AssetNameEquals("Maps/Blacksmith") ||
					asset.AssetNameEquals("TileSheets/weapons"));
		}

		/// <summary>Edit a matched asset.</summary>
		/// <param name="asset">A helper which encapsulates metadata about an asset and enables changes to it.</param>
		public void Edit<T>(IAssetData asset)
		{
			// Add info about the mattock to the weapons data.
			if (asset.AssetNameEquals("Data/weapons"))
			{
				IDictionary<int, string> data = asset.AsDictionary<int, string>().Data;
				data[70] = "Mattock/A piece of dwarf history, it's a very versatile tool./16/16/1/0/0/0/0/-1/-1/2/.02/4";
			}

			// Add info about the mattock artifact to the object info data.
			if (asset.AssetNameEquals("Data/ObjectInformation"))
			{
				IDictionary<int, string> data = asset.AsDictionary<int, string>().Data;
				data[934] = "Dwarf Mattock/250/-300/Arch/Dwarf Mattock/It's an ancient dwarf tool, used for mining and clearing rubble. It's in bad shape, maybe it could be repaired?/Volcano .01/Money 1 500";
			}

			// Add the dwarf mattock artifact as a liked gift for the dwarf.
			if (asset.AssetNameEquals("Data/NPCGiftTastes"))
			{
				IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
				string[] currentTastes = data["Dwarf"].Split('/');
				currentTastes[3] += " 934";
				data["dwarf"] = String.Join("/", currentTastes);
			}

			// Add the mattock artifact to the item sprite sheet.
			if (asset.AssetNameEquals("Maps/springobjects"))
			{
				var editor = asset.AsImage();
				editor.PatchImage(mattockIcons, sourceArea: new Rectangle(0, 0, 32, 16), targetArea: new Rectangle(352, 608, 32, 16));
			}

			if (asset.AssetNameEquals("Maps/Blacksmith"))
			{
				var map = asset.AsMap();
				// Add an invisible tile to the front layer so later we can replace it.
				map.Data.GetLayer("Front").Tiles[6, 12] = new StaticTile(map.Data.GetLayer("Front"), map.Data.TileSheets[0], BlendMode.Alpha, 48);
				map.Data.GetLayer("Front").Tiles[5, 10] = new StaticTile(map.Data.GetLayer("Front"), map.Data.TileSheets[0], BlendMode.Alpha, 48);
				map.Data.GetLayer("Front").Tiles[6, 10] = new StaticTile(map.Data.GetLayer("Front"), map.Data.TileSheets[0], BlendMode.Alpha, 48);
				map.Data.GetLayer("Front").Tiles[5, 11] = new StaticTile(map.Data.GetLayer("Front"), map.Data.TileSheets[0], BlendMode.Alpha, 48);
				map.Data.GetLayer("Front").Tiles[6, 11] = new StaticTile(map.Data.GetLayer("Front"), map.Data.TileSheets[0], BlendMode.Alpha, 48);
			}

			// Add the mattock weapon icons to the weapon sprite sheet.
			if (asset.AssetNameEquals("TileSheets/weapons"))
			{
				var editor = asset.AsImage();
				editor.PatchImage(mattockIcons, sourceArea: new Rectangle(16, 0, 32, 16), targetArea: new Rectangle(96, 128, 32, 16));
			}
		}

		/// <summary>The mod entry point, called after the mod is first loaded.</summary>
		/// <param name="helper">Provides simplified APIs for writing mods.</param>
		public override void Entry(IModHelper helper)
		{
			M = Monitor;

			// Load the custom mattock graphics.
			mattockIcons = Helper.Content.Load<Texture2D>("assets/mattocks.png");

			clintSheet = Helper.Content.Load<Texture2D>("Characters/Clint", ContentSource.GameContent);

			Harmony harmony = new Harmony(ModManifest.UniqueID);

			// Get copies of the performToolAction methods prior to patching so we can call them without causing a stack overflow.
			TerrainFeaturePatches.performToolActionOriginal = (DynamicMethod) harmony.Patch(AccessTools.Method(typeof(StardewValley.TerrainFeatures.TerrainFeature), nameof(StardewValley.TerrainFeatures.TerrainFeature.performToolAction)));
			ResourceClumpPatches.performToolActionOriginal = (DynamicMethod) harmony.Patch(AccessTools.Method(typeof(StardewValley.TerrainFeatures.ResourceClump), nameof(StardewValley.TerrainFeatures.ResourceClump.performToolAction)));
			FruitTreePatches.performToolActionOriginal = (DynamicMethod) harmony.Patch(AccessTools.Method(typeof(StardewValley.TerrainFeatures.FruitTree), nameof(StardewValley.TerrainFeatures.FruitTree.performToolAction)));
			TreePatches.performToolActionOriginal = (DynamicMethod) harmony.Patch(AccessTools.Method(typeof(StardewValley.TerrainFeatures.Tree), nameof(StardewValley.TerrainFeatures.Tree.performToolAction)));
			WoodsPatches.performToolActionOriginal = (DynamicMethod) harmony.Patch(AccessTools.Method(typeof(StardewValley.Locations.Woods), nameof(StardewValley.Locations.Woods.performToolAction)));

			// Patch the isScythe method to allow mattocks to be considered scythes (So they'll drop hay when grass is cut, and so on).
			harmony.Patch(original: AccessTools.Method(typeof(StardewValley.Tools.MeleeWeapon), nameof(StardewValley.Tools.MeleeWeapon.isScythe)),
						  prefix: new HarmonyMethod(typeof(MeleeWeaponPatches), nameof(MeleeWeaponPatches.isScythe_Prefix)));

			// Path the DoDamage method to allow mattocks to damage stumps and boulders.
			harmony.Patch(original: AccessTools.Method(typeof(StardewValley.Tools.MeleeWeapon), nameof(StardewValley.Tools.MeleeWeapon.DoDamage)),
						  prefix: new HarmonyMethod(typeof(MeleeWeaponPatches), nameof(MeleeWeaponPatches.DoDamage_Prefix)));

			// Patch the Object.performToolAction method to allow mattocks to apply to both stone and twig objects.
			harmony.Patch(original: AccessTools.Method(typeof(StardewValley.Object), nameof(StardewValley.Object.performToolAction)),
						  prefix: new HarmonyMethod(typeof(ObjectPatches), nameof(ObjectPatches.performToolAction_Prefix)));

			// Patch the ResourceClump.performToolAction method to allow mattocks to properly interact with stumps and boulders.
			harmony.Patch(original: AccessTools.Method(typeof(StardewValley.TerrainFeatures.ResourceClump), nameof(StardewValley.TerrainFeatures.ResourceClump.performToolAction)),
						  prefix: new HarmonyMethod(typeof(ResourceClumpPatches), nameof(ResourceClumpPatches.performToolAction_Prefix)));

			// Patch the FruitTree.performToolAction method to allow mattocks to chop fruit trees.
			harmony.Patch(original: AccessTools.Method(typeof(StardewValley.TerrainFeatures.FruitTree), nameof(StardewValley.TerrainFeatures.FruitTree.performToolAction)),
						  prefix: new HarmonyMethod(typeof(FruitTreePatches), nameof(FruitTreePatches.performToolAction_Prefix)));

			// Patch the Tree.performToolAction method to allow mattocks to chop trees.
			harmony.Patch(original: AccessTools.Method(typeof(StardewValley.TerrainFeatures.Tree), nameof(StardewValley.TerrainFeatures.Tree.performToolAction)),
						  prefix: new HarmonyMethod(typeof(TreePatches), nameof(TreePatches.performToolAction_Prefix)));
			
			// Patch the IslandLocation.checkForBuriedItem method to allow the mattock artifact to be dug up in the volcano dungeon.
			harmony.Patch(original: AccessTools.Method(typeof(StardewValley.Locations.IslandLocation), nameof(StardewValley.Locations.IslandLocation.checkForBuriedItem)),
						  prefix: new HarmonyMethod(typeof(IslandLocationPatches), nameof(IslandLocationPatches.checkForBuriedItem_Prefix)));

			// Patch the Woods.performToolAction method so stumps in the hidden woods work with mattocks as expected.
			harmony.Patch(original: AccessTools.Method(typeof(StardewValley.Locations.Woods), nameof(StardewValley.Locations.Woods.performToolAction)),
						  prefix: new HarmonyMethod(typeof(WoodsPatches), nameof(WoodsPatches.performToolAction_Prefix)));

			helper.ConsoleCommands.Add("mattock", "Spawns a mattock in your inventory.\n\nUsage: mattock", SpawnMattockCommand);

			// // Save handlers to prevent custom objects from being saved to file.
			helper.Events.GameLoop.Saving += (s, e) => makePlaceholderMattocks();
			helper.Events.GameLoop.Saved += (s, e) => restorePlaceholderMattocks();
			helper.Events.GameLoop.SaveLoaded += (s, e) => restorePlaceholderMattocks();
			helper.Events.Player.Warped += OnWarped;
		}

		/*********
		** Private methods
		*********/
		private static T XmlDeserialize<T>(string toDeserialize)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
			using(StringReader textReader = new StringReader(toDeserialize))
			{
				return (T)xmlSerializer.Deserialize(textReader);
			}
		}

		private static string XmlSerialize<T>(T toSerialize)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(toSerialize.GetType());
			using(StringWriter textWriter = new StringWriter())
			{
				xmlSerializer.Serialize(textWriter, toSerialize);
				return textWriter.ToString();
			}
		}
		
		/// <summary>Replaces all instances of PanHat with a placeholder.</summary>
		private void makePlaceholderMattocks()
		{
			// Find all instances of Mattock objects and replace them with standard placeholder objects.
			StardewValley.Object placeholder = new StardewValley.Object();
			placeholder.Name = "Mattock";

			// Replace any mattocks here with placeholders.
			Mattock testMattock = new Mattock();

			foreach (GameLocation location in Game1.locations)
			{
				foreach (StardewValley.Object chestObject in location.Objects.Values)
				{
					if (chestObject is Chest chest)
					{
						for (int i = 0; i < chest.items.Count; i++)
						{
							if (chest.items[i] is Mattock)
							{
								chest.items[i] = placeholder;
							}
						}
					}
				}
			}

			// Ensure we check additional buildings like sheds and cabins.
			foreach (Building building in Game1.getFarm().buildings)
			{
				if (building.indoors.Value != null && building.indoors.Value.Objects != null)
				{
					foreach (StardewValley.Object chestObject in building.indoors.Value.Objects.Values)
					{
						if (chestObject is Chest chest)
						{
							for (int i = 0; i < chest.items.Count; i++)
							{
								if (chest.items[i] is Mattock)
								{
									chest.items[i] = placeholder;
								}
							}
						}
					}
				}
			}

			// Replace all mattocks in the inventory with placeholders.
			for (int i = 0; i < Game1.player.Items.Count; i++)
			{
				if (Game1.player.Items[i] is Mattock)
				{
					Game1.player.Items[i] = placeholder;
				}
			}
		}

		/// <summary>Replaces a placeholder object with its PanHat counterpart.</summary>
		private void restorePlaceholderMattocks()
		{
			// Replace any placeholders here with mattocks.
			foreach (GameLocation location in Game1.locations)
			{
				foreach (StardewValley.Object chestObject in location.Objects.Values)
				{
					if (chestObject is Chest chest)
					{
						for (int i = 0; i < chest.items.Count; i++)
						{
							if (chest.items[i] != null && chest.items[i].Name.Equals("Mattock"))
							{
								chest.items[i] = new Mattock();
							}
						}
					}
				}
			}

			// Ensure we check additional buildings like sheds and cabins.
			foreach (Building building in Game1.getFarm().buildings)
			{
				if (building.indoors.Value != null && building.indoors.Value.Objects != null)
				{
					foreach (StardewValley.Object chestObject in building.indoors.Value.Objects.Values)
					{
						if (chestObject is Chest chest)
						{
							for (int i = 0; i < chest.items.Count; i++)
							{
								if (chest.items[i] != null && chest.items[i].Name.Equals("Mattock"))
								{
									chest.items[i] = new Mattock();
								}
							}
						}
					}
				}
			}

			// Replace all mattocks in the inventory with placeholders.
			for (int i = 0; i < Game1.player.Items.Count; i++)
			{
				if (Game1.player.Items[i] != null && Game1.player.Items[i].Name.Equals("Mattock"))
				{
					Game1.player.Items[i] = new Mattock();
				}
			}
		}

		/// <summary>Spawns a mattock in the player's inventory when the command is invoked.</summary>
		/// <param name="command">The name of the command invoked.</param>
		/// <param name="args">The arguments received by the command. Each word after the command name is a separate argument.</param>
		private void SpawnMattockCommand(string command, string[] args)
		{
			Game1.player.addItemToInventory(new Mattock());
		}

		/// <summary>The method called after a player warps somewhere.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="args">The event arguments.</param>
		private void OnWarped(object sender, WarpedEventArgs args)
		{
			// If the local player just entered the blacksmith shop, check all requirements for the mattock event.
			if (args.IsLocalPlayer && args.NewLocation.Name.Equals("Blacksmith") && args.NewLocation.currentEvent == null)
			{
				// Requirements:
				//  Found and donated dwarvish mattock artifact
				//  Have golden scythe
				//  Fully upgraded pickaxe and axe
				//  Have Dwarvish translation guide
				//  Six hearts with Clint
				//  Don't already have a mattock
				//  Have at least one spot free in inventory
				if ((Game1.getLocationFromName("ArchaeologyHouse") as LibraryMuseum).museumAlreadyHasArtifact(934) &&
					args.Player.getToolFromName("Axe").UpgradeLevel >= 4 &&
					args.Player.getToolFromName("Pickaxe").UpgradeLevel >= 4 &&
					args.Player.canUnderstandDwarves &&
					args.Player.getFriendshipHeartLevelForNPC("Clint") >= 6 &&
					!args.Player.eventsSeen.Contains(9684001) &&
					!args.Player.isInventoryFull())
				{
					// Check if another mod is providing the blacksmith events file.
					try
					{
						Game1.content.Load<Dictionary<string, string>>("Data\\Events\\Blacksmith");
					}
					catch (Exception)
					{
						// Failed to load the content, so it's already being loaded somewhere else!
						providesBlacksmithEvents = true;
					}

					if (providesBlacksmithEvents)
					{
						Helper.Content.AssetLoaders.Add(new AssetLoader());
					}
					else
					{
						Helper.Content.AssetEditors.Add(new AssetEditor());
					}

					Helper.Content.InvalidateCache("Data/Events/Blacksmith");
					Dictionary<string, string> location_events = null;
					try
					{
						location_events = Game1.content.Load<Dictionary<string, string>>("Data\\Events\\Blacksmith");
					}
					catch (Exception)
					{
						M.Log("COULD NOT LOAD EVENT", LogLevel.Error);
						return;
					}

					Event mattockEvent = new Event(location_events["9684001/r 0"], 9684001, args.Player);

					// Cleanup after event is finished.
					mattockEvent.onEventFinished = (Action)Delegate.Combine(mattockEvent.onEventFinished, (Action)delegate
					{
						// Add the mattock to inventory.
						Game1.player.addItemToInventory(new Mattock());

						GameLocation blacksmith = Game1.player.currentLocation;
						
						// Clean up the temporary tiles that were added during the event.
						blacksmith.map.GetLayer("Front").Tiles[6, 12] = new StaticTile(blacksmith.map.GetLayer("Front"), blacksmith.map.TileSheets[0], BlendMode.Alpha, 48);
						blacksmith.map.GetLayer("Front").Tiles[5, 10] = new StaticTile(blacksmith.map.GetLayer("Front"), blacksmith.map.TileSheets[0], BlendMode.Alpha, 48);
						blacksmith.map.GetLayer("Front").Tiles[6, 10] = new StaticTile(blacksmith.map.GetLayer("Front"), blacksmith.map.TileSheets[0], BlendMode.Alpha, 48);
						blacksmith.map.GetLayer("Front").Tiles[5, 11] = new StaticTile(blacksmith.map.GetLayer("Front"), blacksmith.map.TileSheets[0], BlendMode.Alpha, 48);
						blacksmith.map.GetLayer("Front").Tiles[6, 11] = new StaticTile(blacksmith.map.GetLayer("Front"), blacksmith.map.TileSheets[0], BlendMode.Alpha, 48);
					});
					args.NewLocation.startEvent(mattockEvent);
				}
			}
		}
	}
}