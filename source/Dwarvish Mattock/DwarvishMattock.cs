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

		/*********
		** Public methods
		*********/

		/// <summary>Get whether this instance can load the initial version of the given asset.</summary>
		/// <param name="asset">Basic metadata about the asset being loaded.</param>
		public bool CanLoad<T>(IAssetInfo asset)
		{
			// Ensure we can load the new mattock graphics.
			return asset.AssetNameEquals(Helper.Content.GetActualAssetKey("assets/mattocks.png", ContentSource.ModFolder)) ||
				   asset.AssetNameEquals("Data/Events/Blacksmith") || 
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

			if (asset.AssetNameEquals("Data/Events/Blacksmith"))
			{
				return (T)(object)new Dictionary<string, string>
				{
					["9684001/r 0"] = String.Join("/", new string[] {
						"playful",
						"8 15",
						"farmer 6 15 1 Clint 8 12 2 Dwarf 10 13 3",
						"addObject 9 11 934 0",
						"addTemporaryActor Clint2 16 32 -1 -1 4 false Character",
						"pause 800",
						"emote Clint 16",
						"jump Clint 4",
						"speak Clint \"There you are, @! Gunther just brought this over, he said you donated it?  Where did you find this?$l\"",
						"advancedMove farmer false 2 0 0 -1",
						"faceDirection farmer 1",
						"pause 1600",
						"stopAnimation farmer",
						"speak Clint \"I've read about these in Metallurgist Monthly, this is quite a find!  It's called a mattock, they haven't been seen in years!$h\"",
						"faceDirection Clint 1",
						"jump Clint 4",
						"showFrame Clint 7",
						"speak Clint \"Look over here, one end has a sharp blade to cut through thick roots and plants and the other end is a sturdy point, strong as any pickaxe!  What exquisite craftsmanship, you could probably chop a tree down with this thing!$h\"",
						"emote Clint 20",
						"faceDirection Clint 2",
						"showFrame Clint 0",
						"pause 800",
						"faceDirection farmer 1",
						"speak Dwarf \"This is all that remains of an ancient tool used by my ancestors. It is said that every dwarf was given one when they came of age, and this is what gave us our reputation as the world's best miners.  Unfortunately the tradition was lost over the ages...\"",
						"speak Dwarf \"It has been a long time since tools like this were made, seeing this brings happiness to my heart.  Thank you, human.\"",
						"pause 1000",
						"jump Clint 8",
						"speak Clint \"Wait a second, I've been stockpiling some rare materials for a special occasion... @ is the best miner I know, why don't we bring the tradition back to life!$h\"",
						"emote Dwarf 32",
						"friendship Dwarf 250",

						// *** Fade out ***
						"globalFade",
						"viewport -1000 -1000",
						// ***          ***

						"playMusic none",

						// Move the mattock to the table.
						"removeObject 9 11",
						"addObject 10 16 934 0",

						// Remove the bowl from the table.
						"removeTile 10 16 Front",

						// Warp everyone around the table to examine.
						"warp Clint 10 15",
						"faceDirection Clint 2",
						"warp Dwarf 8 17",
						"faceDirection Dwarf 1",
						"warp farmer 8 16",
						"faceDirection farmer 1",

						"playMusic sappypiano",

						// *** Fade in ***
						"pause 400",
						"viewport 6 15 true",
						"globalFadeToClear 0.015",
						// ***         ***

						"showFrame Clint 43",
						"textAboveHead Clint \"If I attach the head here...\"",
						"pause 4000",
						"showFrame Clint 0",
						"pause 3000",
						"textAboveHead Clint \"... I could forge the edges with dragontooth...\"",
						"pause 2000",
						"jump Dwarf 4",
						"pause 250",
						"faceDirection farmer 2",
						"pause 150",
						"faceDirection Dwarf 0",
						"pause 1750",
						"faceDirection Dwarf 1",
						"pause 100",
						"faceDirection farmer 1",
						"pause 1250",
						"shake Clint 2000",
						"textAboveHead Clint \"Let's get started!\"",
						"pause 200",
						"jump Dwarf 4",
						"pause 50",
						"jump farmer 4",
						"pause 750",
						"jump Dwarf 4",
						"pause 1000",
						"jump Dwarf 6",
						"pause 1500",

						// *** Fade out ***
						"globalFade 0.015",
						"viewport -1000 -1000",
						// ***          ***

						"warp Dwarf 9 15",
						"warp farmer 7 15",
						"warp Clint 8 12",
						"warp Clint2 9 12",
						"addObject 9 11 852 0",
						"addObject 11 12 337 0",
						"faceDirection Dwarf 3",
						"faceDirection farmer 1",
						"showFrame Clint 16",
						"showFrame Clint2 17",
						"pause 400",

						// Fade in
						"viewport 6 15 true",
						"globalFadeToClear 0.015",

						"pause 600",
						"showFrame Clint 22", "showFrame Clint2 23",
						
						"pause 150",
						"showFrame Clint 20", "showFrame Clint2 21",
						"pause 250",
						"showFrame Clint 22", "showFrame Clint2 23",
						"pause 150",
						"showFrame Clint 16", "showFrame Clint2 17",
						"pause 150",
						"playSound clank",
						"showFrame Clint 18", "showFrame Clint2 19",
						"pause 150",
						"showFrame Clint 16", "showFrame Clint2 17",

						"emote farmer 8",
						
						"pause 750",

						"emote Dwarf 40",

						"showFrame Clint 22", "showFrame Clint2 23",
						"pause 150",
						"showFrame Clint 20", "showFrame Clint2 21",
						"pause 250",
						"showFrame Clint 22", "showFrame Clint2 23",
						"pause 150",
						"showFrame Clint 16", "showFrame Clint2 17",
						"pause 150",
						"playSound clank",
						"showFrame Clint 18", "showFrame Clint2 19",
						"pause 150",
						"showFrame Clint 16", "showFrame Clint2 17",

						"emote Dwarf 52",

						"pause 200",
						"jump farmer 4",
						"emote farmer 16",
						"pause 400",

						"showFrame Clint 22", "showFrame Clint2 23",
						"pause 150",
						"showFrame Clint 20", "showFrame Clint2 21",
						"pause 250",
						"showFrame Clint 22", "showFrame Clint2 23",
						"pause 100",
						"showFrame Clint 16", "showFrame Clint2 17",
						"pause 100",
						"playSound clank",
						"showFrame Clint 18", "showFrame Clint2 19",
						"pause 250",
						"showFrame Clint 16", "showFrame Clint2 17",

						"emote Dwarf 32",
						"emote farmer 32",

						"pause 200",
						"showFrame Clint 22", "showFrame Clint2 23",
						"pause 100",
						"showFrame Clint 20", "showFrame Clint2 21",

						// *** Fade out ***
						"globalFade 0.015",
						"viewport -1000 -1000",
						// ***          ***
						"playSound clank",

						"addProp 690 5 10 2 1 1",
						"addProp 692 5 11 2 1 1",
						"addProp 694 5 11 2 1 1",
						
						"changeMapTile Front 6 12 815",

						"removeObject 9 11",
						"removeObject 11 12",
						"addObject 9 11 788 0",
						"addObject 9 11 67 1",
						"addObject 11 16 656 1",

						"warp Dwarf 6 14",
						"faceDirection Dwarf 0",
						"positionOffset Dwarf 2 -8",
						"warp farmer 5 14",
						"faceDirection farmer 0",
						"positionOffset farmer 10 0",
						"warp Clint2 -1 -1",
						"faceDirection Clint 2",

						"pause 450",
						"viewport 6 15 true",
						"globalFadeToClear 0.015",

						"advancedMove Clint false 0 2 4 0 0 2",
						"pause 600",
						"temporarySprite 5 11 3 3 250 false",
						"playSound shwip",
						"pause 1200",
						"temporarySprite 6 11 1 4 200 false",
						"playSound debuffSpell",
						"shake Dwarf 1150",
						"pause 1000",
						"showFrame farmer 15",
						"jump farmer 4",
						"pause 1500",
						"showFrame farmer 12",
						"addProp 694 5 11 2 1 1",
						"temporarySprite 6 11 3 2 300 false",
						"playSound grunt",
						"positionOffset Dwarf 0 -1",
						"faceDirection Clint 3",
						"pause 1250",
						"addProp 694 5 11 2 1 1",
						"temporarySprite 5 11 1 4 200 false",
						"playSound fireball",
						"shake Dwarf 1200",
						"pause 1500",
						"positionOffset Dwarf 0 -1",
						"pause 600",
						"showFrame farmer 65",
						"pause 500",
						"showFrame Clint 13",
						"jump farmer 4",
						"pause 750",
						"positionOffset Dwarf 0 1",
						"pause 950",
						"temporarySprite 5 11 1 4 200 false",
						"playSound grunt",
						"pause 1000",
						"showFrame Clint 12",
						"jump Dwarf 2",
						"pause 250",
						"showFrame farmer 12",
						"temporarySprite 6 11 5 2 250 false",
						"playSound explosion",
						"pause 1000",

						// *** Fade out ***
						"globalFade",
						"viewport -1000 -1000",
						// ***          ***

						"removeObject 9 11",
						"removeObject 9 11",
						"addObject 9 11 935 0",
						"pause 500",
						"warp Clint 8 12",
						"warp Clint2 9 12",
						"warp Dwarf 10 14",
						"warp farmer 9 14",

						"faceDirection Dwarf 0",
						"faceDirection farmer 0",
						"showFrame Clint 16",
						"showFrame Clint2 17",
						"positionOffset Clint 0 -14",
						"positionOffset Clint2 0 -14",

						"changeMapTile Front 6 12 48",
						"changeMapTile Front 5 10 218",
						"changeMapTile Front 6 10 112",
						"changeMapTile Front 5 11 994",
						"changeMapTile Front 6 11 997",
						"removeObject 11 16",

						// *** Fade in ***
						"pause 800",
						"viewport 6 15 true",
						"globalFadeToClear 0.005",
						// ***         ***
						
						"pause 600",
						"showFrame Clint 22", "showFrame Clint2 23",
						
						"pause 100",
						"showFrame Clint 20", "showFrame Clint2 21",
						"pause 200",
						"showFrame Clint 22", "showFrame Clint2 23",
						"pause 100",
						"showFrame Clint 16", "showFrame Clint2 17",
						"pause 100",
						"playSound clank",
						"showFrame Clint 18", "showFrame Clint2 19",
						"pause 100",
						"showFrame Clint 16", "showFrame Clint2 17",
						"textAboveHead Clint \"Just a few more finishing touches...\"",
						"showFrame Clint 22", "showFrame Clint2 23",
						
						"pause 1100",
						"showFrame Clint 20", "showFrame Clint2 21",
						"pause 200",
						"showFrame Clint 22", "showFrame Clint2 23",
						"pause 100",
						"showFrame Clint 16", "showFrame Clint2 17",
						"pause 100",
						"playSound clank",
						"showFrame Clint 18", "showFrame Clint2 19",
						"pause 100",
						"showFrame Clint 16", "showFrame Clint2 17",
						"pause 200",
						"showFrame Clint 22", "showFrame Clint2 23",
						
						"pause 200",
						"showFrame Clint 20", "showFrame Clint2 21",
						"pause 800",
						"showFrame Clint 22", "showFrame Clint2 23",
						"pause 100",
						"showFrame Clint 16", "showFrame Clint2 17",
						"pause 100",
						"playSound clank",
						"showFrame Clint 18", "showFrame Clint2 19",
						"pause 100",
						"showFrame Clint 16", "showFrame Clint2 17",
						"pause 200",
						"showFrame Clint 22", "showFrame Clint2 23",

						"pause 200",
						"showFrame Clint 20", "showFrame Clint2 21",
						"pause 600",
						"showFrame Clint 22", "showFrame Clint2 23",
						"pause 100",
						"showFrame Clint 16", "showFrame Clint2 17",
						"pause 100",
						"playSound clank",
						"showFrame Clint 18", "showFrame Clint2 19",
						"pause 100",
						"showFrame Clint 16", "showFrame Clint2 17",
						"pause 200",
						"showFrame Clint 22", "showFrame Clint2 23",

						"pause 200",
						"showFrame Clint 20", "showFrame Clint2 21",
						"pause 600",
						"showFrame Clint 22", "showFrame Clint2 23",
						"pause 100",
						"showFrame Clint 16", "showFrame Clint2 17",
						"pause 100",
						"playSound clank",
						"showFrame Clint 18", "showFrame Clint2 19",
						"pause 100",
						"showFrame Clint 16", "showFrame Clint2 17",
						"pause 200",
						"showFrame Clint 22", "showFrame Clint2 23",

						"pause 200",
						"showFrame Clint 20", "showFrame Clint2 21",
						"pause 1400",
						"showFrame Clint 22", "showFrame Clint2 23",
						"pause 100",
						"showFrame Clint 16", "showFrame Clint2 17",
						"pause 100",
						"playSound clank",
						"showFrame Clint 18", "showFrame Clint2 19",
						"pause 100",
						"showFrame Clint 16", "showFrame Clint2 17",
						"pause 200",
						"showFrame Clint 22", "showFrame Clint2 23",

						"pause 200",
						"showFrame Clint 20", "showFrame Clint2 21",
						"pause 600",
						"showFrame Clint 22", "showFrame Clint2 23",
						"pause 100",
						"showFrame Clint 16", "showFrame Clint2 17",
						"pause 100",
						"playSound clank",
						"showFrame Clint 18", "showFrame Clint2 19",
						"pause 100",
						"showFrame Clint 16", "showFrame Clint2 17",
						"pause 1500",

						"warp Clint2 -1 -1",
						"faceDirection Clint 2",
						"showFrame Clint 0",
						"pause 500",
						"textAboveHead Clint \"It's done!\"",
						"removeObject 9 11",
						"advancedMove Clint false 0 2",
						"pause 500",
						"faceDirection Dwarf 3",
						"pause 500",
						"playMusic none",
						"faceDirection Clint 1",
						"showFrame Clint 7",
						"pause 400",
						"playSound getNewSpecialItem",
						"faceDirection farmer 2",
						"showFrame farmer 57",
						"addObject 9 12 935 1",
						"message \"You received the Dwarven Mattock!\"",
						"removeObject 9 12",
						"showFrame Clint 4",
						"faceDirection farmer 3",

						"speak Clint \"This has to be some of my best work, I don't think I'll be able to upgrade it any further. I stayed true to the original dwarf design, so this tool can destroy plants, trees and rocks.  Plus I used a dragontooth alloy so it's practically indestructible!$h\"",
						"pause 500",
						"faceDirection farmer 1",
						"pause 750",
						"speak Dwarf \"You have done dwarf kind a service. Thank you, both. Wield it with pride, @!\"",
						"pause 2500",
						"globalFade",
						"viewport -1000 -1000",
						"end"
					})
				};
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
					args.Player.mailReceived.Contains("gotGoldenScythe") &&
					args.Player.getToolFromName("Axe").UpgradeLevel == 4 &&
					args.Player.getToolFromName("Pickaxe").UpgradeLevel == 4 &&
					args.Player.canUnderstandDwarves &&
					args.Player.getFriendshipHeartLevelForNPC("Clint") >= 6 &&
					!args.Player.eventsSeen.Contains(9684001) &&
					!args.Player.isInventoryFull())
				{
					Dictionary<string, string> location_events = null;
					try
					{
						location_events = Game1.content.Load<Dictionary<string, string>>("Data\\Events\\Blacksmith");
					}
					catch (Exception)
					{
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