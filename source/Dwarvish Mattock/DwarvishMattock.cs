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
using StardewValley.GameData.Objects;
using StardewValley.GameData.Weapons;

namespace DwarvishMattock
{
	/// <summary>The mod entry point.</summary>
	public class ModEntry : Mod
	{
		public static IMonitor M = null;
		public static readonly string MATTOCK_WEAPON_ID = "DwarvishMattock";
		public static readonly string MATTOCK_ARTIFACT_ID = "MattockArtifact";
		public static readonly string MATTOCK_EVENT_ID = "9684001";


		/*********
		** Public methods
		*********/

		private void onAssetRequested(object sender, AssetRequestedEventArgs e)
		{
			// Add info about the mattock to the weapons data.
			if (e.NameWithoutLocale.IsEquivalentTo("Data/Weapons"))
			{
				e.Edit(asset =>
				{
					IAssetDataForDictionary<string, WeaponData> editor = asset.AsDictionary<string, WeaponData>();

					editor.Data[MATTOCK_WEAPON_ID] = new WeaponData {
						Name = "Mattock",
						DisplayName = "Mattock",
						Description = "A piece of dwarf history, it's a very versatile tool.",
						Type = 3,
						SpriteIndex = 70,
						Texture = "TileSheets/weapons",
						MinDamage = 30,
						MaxDamage = 40,
						Speed = -1,
						CanBeLostOnDeath = false,
						AreaOfEffect = 3,
						CritChance = 0.02f,
						CritMultiplier = 4.0f,
						Precision = 10
					};
				});
			}

			// Add info about the mattock artifact to the object info data.
			if (e.NameWithoutLocale.IsEquivalentTo("Data/Objects"))
			{
				e.Edit(asset =>
				{
					IAssetDataForDictionary<string, ObjectData> editor = asset.AsDictionary<string, ObjectData>();
					editor.Data[MATTOCK_ARTIFACT_ID] = new ObjectData {
						Name = "Dwarf Mattock",
						DisplayName = "Dwarf Mattock",
						Description = "It's an ancient dwarf tool, used for mining and clearing rubble. It's in bad shape, maybe it could be repaired?",
						SpriteIndex = 933,
						Price = 250,
						Type = "Arch",
						ArtifactSpotChances = new Dictionary<string, float> {{ "Volcano", 0.01f }}
					};
				});
			}

			// Add the dwarf mattock artifact as a liked gift for the dwarf.
			if (e.NameWithoutLocale.IsEquivalentTo("Data/NPCGiftTastes"))
			{
				e.Edit(asset =>
				{
					IAssetDataForDictionary<string, string> editor = asset.AsDictionary<string, string>();
					string[] currentTastes = editor.Data["Dwarf"].Split('/');
					currentTastes[3] += " " + MATTOCK_ARTIFACT_ID;
					editor.Data["Dwarf"] = String.Join("/", currentTastes);
				});
			}

			// Add the mattock artifact to the item sprite sheet.
			if (e.NameWithoutLocale.IsEquivalentTo("Maps/springobjects"))
			{
				e.Edit(asset =>
				{
					IRawTextureData sourceImage = Helper.ModContent.Load<IRawTextureData>("assets/mattocks.png");

					IAssetDataForImage editor = asset.AsImage();
					editor.PatchImage(sourceImage, sourceArea: new Rectangle(0, 0, 32, 16), targetArea: new Rectangle(336, 608, 32, 16));
				});
			}

			// Add the mattock weapon icons to the weapon sprite sheet.
			if (e.NameWithoutLocale.IsEquivalentTo("TileSheets/weapons"))
			{
				e.Edit(asset =>
				{
					IRawTextureData sourceImage = Helper.ModContent.Load<IRawTextureData>("assets/mattocks.png");

					IAssetDataForImage editor = asset.AsImage();
					editor.PatchImage(sourceImage, sourceArea: new Rectangle(16, 0, 32, 16), targetArea: new Rectangle(96, 128, 32, 16));
				});
			}

			// Alter the blacksmith map to support the cutscene.
			if (e.NameWithoutLocale.IsEquivalentTo("Maps/Blacksmith"))
			{
				e.Edit(asset =>
				{
					IAssetDataForMap map = asset.AsMap();

					// Add an invisible tile to the front layer so later we can replace it.
					map.Data.GetLayer("Front").Tiles[6, 12] = new StaticTile(map.Data.GetLayer("Front"), map.Data.TileSheets[0], BlendMode.Alpha, 48);
					map.Data.GetLayer("Front").Tiles[5, 10] = new StaticTile(map.Data.GetLayer("Front"), map.Data.TileSheets[0], BlendMode.Alpha, 48);
					map.Data.GetLayer("Front").Tiles[6, 10] = new StaticTile(map.Data.GetLayer("Front"), map.Data.TileSheets[0], BlendMode.Alpha, 48);
					map.Data.GetLayer("Front").Tiles[5, 11] = new StaticTile(map.Data.GetLayer("Front"), map.Data.TileSheets[0], BlendMode.Alpha, 48);
					map.Data.GetLayer("Front").Tiles[6, 11] = new StaticTile(map.Data.GetLayer("Front"), map.Data.TileSheets[0], BlendMode.Alpha, 48);
				});
			}
		}

		/// <summary>The mod entry point, called after the mod is first loaded.</summary>
		/// <param name="helper">Provides simplified APIs for writing mods.</param>
		public override void Entry(IModHelper helper)
		{
			M = Monitor;

			Harmony harmony = new Harmony(ModManifest.UniqueID);

			// Get copies of the performToolAction methods prior to patching so we can call them without causing a stack overflow.
			TerrainFeaturePatches.performToolActionOriginal = (DynamicMethod) harmony.Patch(AccessTools.Method(typeof(StardewValley.TerrainFeatures.TerrainFeature), nameof(StardewValley.TerrainFeatures.TerrainFeature.performToolAction)));
			ResourceClumpPatches.performToolActionOriginal = (DynamicMethod) harmony.Patch(AccessTools.Method(typeof(StardewValley.TerrainFeatures.ResourceClump), nameof(StardewValley.TerrainFeatures.ResourceClump.performToolAction)));
			FruitTreePatches.performToolActionOriginal = (DynamicMethod) harmony.Patch(AccessTools.Method(typeof(StardewValley.TerrainFeatures.FruitTree), nameof(StardewValley.TerrainFeatures.FruitTree.performToolAction)));
			TreePatches.performToolActionOriginal = (DynamicMethod) harmony.Patch(AccessTools.Method(typeof(StardewValley.TerrainFeatures.Tree), nameof(StardewValley.TerrainFeatures.Tree.performToolAction)));

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

			// Patch the RockCrab.hitWithTool method to allow the mattock to affect them as a pickaxe.
			harmony.Patch(original: AccessTools.Method(typeof(StardewValley.Monsters.RockCrab), nameof(StardewValley.Monsters.RockCrab.hitWithTool)),
						  prefix: new HarmonyMethod(typeof(RockCrabPatches), nameof(RockCrabPatches.hitWithTool_Prefix)));


			helper.ConsoleCommands.Add("mattock", "Spawns a mattock in your inventory.\n\nUsage: mattock", SpawnMattockCommand);

			// Handle content events.
			helper.Events.Content.AssetRequested += onAssetRequested;
			
			// Save handlers to prevent custom objects from being saved to file.
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
						for (int i = 0; i < chest.Items.Count; i++)
						{
							if (chest.Items[i] is Mattock)
							{
								chest.Items[i] = placeholder;
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
							for (int i = 0; i < chest.Items.Count; i++)
							{
								if (chest.Items[i] is Mattock)
								{
									chest.Items[i] = placeholder;
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
						for (int i = 0; i < chest.Items.Count; i++)
						{
							if (chest.Items[i] != null && chest.Items[i].Name.Equals("Mattock"))
							{
								chest.Items[i] = new Mattock();
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
							for (int i = 0; i < chest.Items.Count; i++)
							{
								if (chest.Items[i] != null && chest.Items[i].Name.Equals("Mattock"))
								{
									chest.Items[i] = new Mattock();
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
				//  Fully upgraded pickaxe and axe
				//  Have Dwarvish translation guide
				//  Six hearts with Clint
				//  Don't already have a mattock
				//  Have at least one spot free in inventory
				if (LibraryMuseum.HasDonatedArtifact(MATTOCK_ARTIFACT_ID) &&
					args.Player.getToolFromName("Axe").UpgradeLevel >= 4 &&
					args.Player.getToolFromName("Pickaxe").UpgradeLevel >= 4 &&
					args.Player.canUnderstandDwarves &&
					args.Player.getFriendshipHeartLevelForNPC("Clint") >= 6 &&
					!args.Player.eventsSeen.Contains(MATTOCK_EVENT_ID) &&
					!args.Player.isInventoryFull())
				{
					// Check if another mod is providing the blacksmith events file.
					// try
					// {
					// 	Game1.content.Load<Dictionary<string, string>>("Data\\Events\\Blacksmith");
					// }
					// catch (Exception)
					// {
					// 	// Failed to load the content, so it's already being loaded somewhere else!
					// 	providesBlacksmithEvents = true;
					// }

					// Helper.GameContent.InvalidateCache("Data/Events/Blacksmith");
					// Dictionary<string, string> location_events = null;
					// try
					// {
					// 	location_events = Game1.content.Load<Dictionary<string, string>>("Data\\Events\\Blacksmith");
					// }
					// catch (Exception)
					// {
					// 	M.Log("COULD NOT LOAD EVENT", LogLevel.Error);
					// 	return;
					// }

					Event mattockEvent = new Event(MattockEvent.eventString, null, MATTOCK_EVENT_ID, args.Player);

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