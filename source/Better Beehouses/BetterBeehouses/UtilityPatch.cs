/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tlitookilakin/BetterBeehouses
**
*************************************************/

using BetterBeehouses.integration;
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
		private static Action<Crop, Vector2> tposf;

		internal static void Init()
		{
			tposf = typeof(Crop).FieldNamed("tilePosition").GetInstanceFieldSetter<Crop, Vector2>();
		}

		[HarmonyPatch("findCloseFlower", new Type[] { typeof(GameLocation), typeof(Vector2), typeof(int), typeof(Func<Crop, bool>) })]
		[HarmonyPrefix]
		[HarmonyPriority(Priority.High)]
		internal static bool preCheck(GameLocation location, Vector2 startTileLocation, int range, Func<Crop, bool> additional_check, ref Crop __result)
		{
			if (ModEntry.config.UseRandomFlower)
			{
				var items = GetAllNearFlowers(location, startTileLocation, range, additional_check).ToArray();
				if (items.Length > 0)
					__result = CropFromIndex(items[Game1.random.Next(items.Length)]);
				else
					__result = null;
				return false;
			} else if (ModEntry.config.UseFruitTrees || ModEntry.config.UseGiantCrops || 
				ModEntry.config.UseForageFlowers || Utils.GetProduceHere(location, ModEntry.config.UsePottedFlowers))
			{
				__result = CropFromIndex(GetAllNearFlowers(location, startTileLocation, range, additional_check).FirstOrDefault());
				return false;
			}
			return true;
		}
		public static IEnumerable<KeyValuePair<Vector2, int>> GetAllNearFlowers(GameLocation loc, Vector2 tile, int range = -1, Func<Crop, bool> extraCheck = null)
		{
			var GiantCrops = new Dictionary<Vector2, int>();
			if (ModEntry.config.UseGiantCrops)
				foreach(var clump in loc.resourceClumps)
					if (clump is GiantCrop giant && IndexIsFlower(giant.parentSheetIndex.Value))
						for(int x = 0; x < giant.width.Value; x++)
							for(int y = 0; y < giant.height.Value; y++)
								if(Math.Abs(giant.tile.X + x - tile.X) + Math.Abs(giant.tile.Y + y - tile.Y) <= range)
									GiantCrops.Add(new(giant.tile.X + x, giant.tile.Y + y), giant.parentSheetIndex.Value);

			var wildflowers = WildFlowers.GetData(loc);
			Queue<Vector2> openList = new();
			HashSet<Vector2> closedList = new();
			openList.Enqueue(tile);
			for (int attempts = 0; range >= 0 || (range < 0 && attempts <= 150); attempts++)
			{
				if (openList.Count <= 0)
					yield break;
				Vector2 currentTile = openList.Dequeue();
				if (GiantCrops.TryGetValue(currentTile, out var gc))
				{
					yield return new(currentTile, gc);
				}
				else if (wildflowers is not null && wildflowers.TryGetValue(currentTile, out var wilf))
				{
					yield return new(currentTile, wilf.indexOfHarvest.Value);
				}
				else if (loc.terrainFeatures.TryGetValue(currentTile, out var tf))
				{
					if (tf is HoeDirt dirt && IsGrown(dirt.crop, extraCheck) && IndexIsFlower(dirt.crop.indexOfHarvest.Value))
						yield return new(currentTile, dirt.crop.indexOfHarvest.Value);
					else if (tf is FruitTree tree && ModEntry.config.UseFruitTrees && tree.fruitsOnTree.Value > 0 && 
						(ModEntry.config.UseAnyFruitTrees || IndexIsFlower(tree.indexOfFruit.Value)))
							yield return new(currentTile, tree.indexOfFruit.Value);
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
									yield return new(currentTile, ho.ParentSheetIndex);
							}
							Crop crop = pot.hoeDirt.Value?.crop;
							if (IsGrown(crop, extraCheck) && IndexIsFlower(crop.indexOfHarvest.Value)
								&& (extraCheck is null || extraCheck(crop)))
								yield return new(currentTile, crop.indexOfHarvest.Value); //flower in pot
						}
					}
					else
					{
						if (ModEntry.config.UseForageFlowers && obj.CanBeGrabbed && ObjectIsFlower(obj))
							yield return new(currentTile, obj.ParentSheetIndex);
						//non-pot forage
					}
				}
				foreach (Vector2 v in Utility.getAdjacentTileLocations(currentTile))
					if (!closedList.Contains(v) && !openList.Contains(v) && (range < 0 || Math.Abs(v.X - tile.X) + Math.Abs(v.Y - tile.Y) <= range))
						openList.Enqueue(v);
				closedList.Add(currentTile);
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
			if (ModEntry.config.AnythingHoney)
				return true;

			if (index == 1720) // DGA reserved fake ID
				return false;
			var res = new StardewValley.Object(index, 1);
			return res.Category == -80 || res.HasContextTag("honey_source");
		}
		private static Crop CropFromIndex(KeyValuePair<Vector2, int> what)
		{
			if (what.Value == 0)
				return null;
			Crop ret = new();
			ret.indexOfHarvest.Value = what.Value;
			tposf(ret, what.Key);
			return ret;
		}
		private static bool ObjectIsFlower(StardewValley.Object obj)
			=> obj is not null && (obj.Category == -80 || obj.HasContextTag("honey_source"));
	}
}
