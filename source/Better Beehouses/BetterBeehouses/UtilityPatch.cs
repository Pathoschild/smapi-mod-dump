/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tlitookilakin/BetterBeehouses
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BetterBeehouses
{
	[HarmonyPatch(typeof(Utility))]
	class UtilityPatch
	{
		[HarmonyPatch("findCloseFlower", new Type[] { typeof(GameLocation), typeof(Vector2), typeof(int), typeof(Func<Crop, bool>) })]
		[HarmonyPrefix]
		internal static bool preCheck(GameLocation location, Vector2 startTileLocation, int range, Func<Crop, bool> additional_check, ref Crop __result)
		{
			if (ModEntry.config.UseRandomFlower)
			{
				var items = GetAllNearFlowers(location, startTileLocation, range, additional_check).ToArray();
				if (items.Length > 0)
					__result = CropFromIndex(items[Game1.random.Next(items.Length)]);
			} else
			{
				__result = CropFromIndex(GetAllNearFlowers(location, startTileLocation, range, additional_check).FirstOrDefault());
			}
			return false;
		}
		public static IEnumerable<int> GetAllNearFlowers(GameLocation loc, Vector2 tile, int range = -1, Func<Crop, bool> extraCheck = null)
		{
			if (ModEntry.config.UseGiantCrops)
				foreach(var clump in loc.resourceClumps)
					if (clump is GiantCrop giant && IndexIsFlower(giant.parentSheetIndex.Value))
						for(int x = 0; x < giant.width.Value; x++)
							for(int y = 0; y < giant.height.Value; y++)
								if(Math.Abs(giant.tile.X + x - tile.X) + Math.Abs(giant.tile.Y + y - tile.Y) <= range)
									yield return giant.parentSheetIndex.Value;

			Queue<Vector2> openList = new();
			HashSet<Vector2> closedList = new();
			foreach(var vec in Utility.getAdjacentTileLocations(tile))
				openList.Enqueue(vec);
			closedList.Add(tile);
			for (int attempts = 0; range >= 0 || (range < 0 && attempts <= 150); attempts++)
			{
				if (openList.Count <= 0)
					yield break;
				Vector2 currentTile = openList.Dequeue();
				closedList.Add(currentTile);
				if (loc.terrainFeatures.TryGetValue(currentTile, out var tf))
				{
					if (tf is HoeDirt dirt && IsGrown(dirt.crop, extraCheck) && IndexIsFlower(dirt.crop.indexOfHarvest.Value))
						yield return dirt.crop.indexOfHarvest.Value;
					else if (tf is FruitTree tree)
						if (ModEntry.config.UseFruitTrees && tree.fruitsOnTree.Value > 0 &&
							(ModEntry.config.UseAnyFruitTrees || IndexIsFlower(tree.indexOfFruit.Value)))
							yield return tree.indexOfFruit.Value;
				}
				else if (loc.objects.TryGetValue(currentTile, out StardewValley.Object obj))
				{
					if (obj is IndoorPot pot) //pot crop
					{
						if (Utils.GetProduceHere(loc, ModEntry.config.UsePottedFlowers))
						{
							if (ModEntry.config.UseForageFlowers && pot.heldObject.Value != null) //forage in pot
							{
								var ho = pot.heldObject.Value;
								if (ho.CanBeGrabbed && ObjectIsFlower(ho))
									yield return ho.ParentSheetIndex;
							}
							Crop crop = pot.hoeDirt.Value?.crop;
							if (IsGrown(crop, extraCheck) && IndexIsFlower(crop.indexOfHarvest.Value)
								&& (extraCheck is null || extraCheck(crop)))
								yield return crop.indexOfHarvest.Value; //flower in pot
						}
					}
					else
					{
						if (ModEntry.config.UseForageFlowers && obj.CanBeGrabbed && ObjectIsFlower(obj))
							yield return obj.ParentSheetIndex;
						//non-pot forage
					}
				}
				foreach (Vector2 v in Utility.getAdjacentTileLocations(currentTile))
					if (!closedList.Contains(v) && !openList.Contains(v) && (range < 0 || Math.Abs(v.X - tile.X) + Math.Abs(v.Y - tile.Y) <= range))
						openList.Enqueue(v);
			}
		}
		private static bool IsGrown(Crop crop, Func<Crop, bool> extraCheck = null)
		{
			if (crop is not null && !crop.dead.Value &&
			crop.currentPhase.Value >= crop.phaseDays.Count - 1)
				if (extraCheck is null)
					return true;
				else
					return extraCheck(crop);
			return false;
		}
		private static bool IndexIsFlower(int index)
		{
			if (index == 1720) // DGA reserved fake ID
				return false;
			var res = new StardewValley.Object(index, 1);
			return res.Category == -80 || res.HasContextTag("honey_source");
		}
		private static Crop CropFromIndex(int index)
		{
			if (index == 0)
				return null;
			Crop ret = new();
			ret.indexOfHarvest.Value = index;
			return ret;
		}
		private static bool ObjectIsFlower(StardewValley.Object obj)
			=> obj is not null && (obj.Category == -80 || obj.HasContextTag("honey_source"));
	}
}
