/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;
using SObject = StardewValley.Object;

namespace ResetTerrainFeaturesRedux.Framework
{
    public class Generator
    {

        public static Dictionary<string, bool> GeneratorOptions = new Dictionary<string, bool>();

        public static Dictionary<string, Type> typeKeys = new Dictionary<string, Type>
        {
            {
                "Bush",
                typeof(Bush)
            },
            {
                "Tree",
                typeof(Tree)
            },
            {
                "Grass",
                typeof(Grass)
            },
            {
                "ResourceClump",
                typeof(ResourceClump)
            },
            {
                "Path",
                typeof(Flooring)
            },
            {
                "Fence",
                typeof(Fence)
            },
            {
                "Soil",
                typeof(HoeDirt)
            },
            {
                "Object",
                typeof(SObject)
            },
            {
                "TFeature",
                typeof(TerrainFeature)
            }
        };

        public static Dictionary<string, List<int>> indexKeys = new Dictionary<string, List<int>>
        {
            {
                "Weeds",
                new List<int>
                {
                    343,
                    450,
                    674,
                    675,
                    676,
                    677,
                    678,
                    679,
                    784,
                    785,
                    786,
                    792,
                    793,
                    794
                }
            },
            {
                "Twig",
                new List<int>
                {
                    294,
                    295
                }
            },
            {
                "Rock",
                new List<int>
                {
                    450,
                    343
                }
            },
            {
                "Stump",
                new List<int>
                {
                    600
                }
            },
            {
                "Log",
                new List<int>
                {
                    602
                }
            },
            {
                "Boulder",
                new List<int>
                {
                    672
                }
            }
        };

        public static List<string> canGenerate = new List<string>
        {
            "Bush",
            "Tree",
            "Grass",
            "ResourceClump",
            "Weeds",
            "Twig",
            "Rock",
            "Forage",
            "Stump",
            "Log",
            "Boulder",
            "Object",
            "TFeature"
        };
    



	public static void reload(GameLocation location, Type[] types = null, int[] indices = null)
		{
			Generator.clear(location, types, indices);
			Generator.loadMapFeatures(location, types, indices);
		}

		public static int[] getIndicesFromOptions()
		{
			List<int> list = new List<int>();
			List<string> list2 = Generator.GeneratorOptions.Keys.ToList<string>();
			foreach (string key in list2)
			{
				bool flag = !Generator.GeneratorOptions[key];
				if (!flag)
				{
					bool flag2 = Generator.indexKeys.ContainsKey(key);
					if (flag2)
					{
						foreach (int item in Generator.indexKeys[key])
						{
							list.Add(item);
						}
					}
				}
			}
			return list.ToArray();
		}

		public static Type[] getTypesFromOptions()
		{
			List<Type> list = new List<Type>();
			List<string> list2 = Generator.GeneratorOptions.Keys.ToList<string>();
			for (int i = 0; i < list2.Count; i++)
			{
				string key = list2[i];
				bool flag = !Generator.GeneratorOptions[key];
				if (!flag)
				{
					bool flag2 = Generator.typeKeys.ContainsKey(key);
					if (flag2)
					{
						list.Add(Generator.typeKeys[key]);
					}
				}
			}
			return list.ToArray();
		}

		public static void clear(GameLocation location, Type[] types = null, int[] parentSheetIndices = null)
		{
			List<Vector2> list = new List<Vector2>();
			List<Vector2> list2 = new List<Vector2>();
			List<LargeTerrainFeature> list3 = new List<LargeTerrainFeature>();
			foreach (Vector2 vector in location.terrainFeatures.Keys)
			{
				TerrainFeature terrainFeature = location.terrainFeatures[vector];
				bool flag = Generator.GeneratorOptions.ContainsKey("Crop") && Generator.GeneratorOptions["Crop"] && terrainFeature is HoeDirt;
				if (flag)
				{
					(terrainFeature as HoeDirt).crop = null;
				}
				bool flag2 = types == null || types.Contains(terrainFeature.GetType()) || types.Contains(typeof(TerrainFeature));
				if (flag2)
				{
					list.Add(vector);
				}
			}
			foreach (Vector2 vector2 in location.Objects.Keys)
			{
				SObject @object = location.objects[vector2];
				bool flag3 = types == null || types.Contains(@object.GetType()) || (parentSheetIndices != null && parentSheetIndices.Contains(@object.ParentSheetIndex)) || types.Contains(typeof(SObject)) || (Generator.GeneratorOptions.ContainsKey("Forage") && Generator.GeneratorOptions["Forage"] && @object.isForage(location));
				if (flag3)
				{
					list2.Add(vector2);
				}
			}
			foreach (LargeTerrainFeature largeTerrainFeature in location.largeTerrainFeatures)
			{
				bool flag4 = types == null || types.Contains(largeTerrainFeature.GetType());
				if (flag4)
				{
					list3.Add(largeTerrainFeature);
				}
			}
			bool flag5 = location is Farm;
			if (flag5)
			{
				Farm farm = location as Farm;
				List<ResourceClump> list4 = new List<ResourceClump>();
				foreach (ResourceClump resourceClump in farm.resourceClumps)
				{
					bool flag6 = types == null || types.Contains(resourceClump.GetType()) || (parentSheetIndices != null && parentSheetIndices.Contains(resourceClump.parentSheetIndex.Value));
					if (flag6)
					{
						list4.Add(resourceClump);
					}
				}
				foreach (ResourceClump resourceClump2 in list4)
				{
					bool flag7 = farm.resourceClumps.Contains(resourceClump2);
					if (flag7)
					{
						farm.resourceClumps.Remove(resourceClump2);
					}
				}
			}
			bool flag8 = location is Woods;
			if (flag8)
			{
				Woods woods = location as Woods;
				List<ResourceClump> list5 = new List<ResourceClump>();
				foreach (ResourceClump resourceClump3 in woods.stumps)
				{
					bool flag9 = types == null || types.Contains(resourceClump3.GetType()) || (parentSheetIndices != null && parentSheetIndices.Contains(resourceClump3.parentSheetIndex.Value));
					if (flag9)
					{
						list5.Add(resourceClump3);
					}
				}
				foreach (ResourceClump resourceClump4 in list5)
				{
					bool flag10 = woods.stumps.Contains(resourceClump4);
					if (flag10)
					{
						woods.stumps.Remove(resourceClump4);
					}
				}
			}
			bool flag11 = location is Forest;
			if (flag11)
			{
				Forest forest = location as Forest;
				bool flag12 = forest.log != null && parentSheetIndices != null && parentSheetIndices.Contains(forest.log.parentSheetIndex.Value);
				if (flag12)
				{
					forest.log = null;
				}
			}
			foreach (Vector2 vector3 in list)
			{
				bool flag13 = location.terrainFeatures.ContainsKey(vector3);
				if (flag13)
				{
					location.terrainFeatures.Remove(vector3);
				}
			}
			foreach (Vector2 vector4 in list2)
			{
				bool flag14 = location.objects.ContainsKey(vector4);
				if (flag14)
				{
					location.objects.Remove(vector4);
				}
			}
			foreach (LargeTerrainFeature largeTerrainFeature2 in list3)
			{
				bool flag15 = location.largeTerrainFeatures.Contains(largeTerrainFeature2);
				if (flag15)
				{
					location.largeTerrainFeatures.Remove(largeTerrainFeature2);
				}
			}
		}

		public static void loadMapFeatures(GameLocation location, Type[] types = null, int[] parentSheetIndices = null)
		{
			GameLocation gameLocation = Generator.makeClone(location);
			List<ResourceClump> clumps = Generator.getClumps(location);
			foreach (KeyValuePair<Vector2, TerrainFeature> keyValuePair in gameLocation.terrainFeatures.Pairs)
			{
				bool flag = types == null || types.Contains(keyValuePair.Value.GetType()) || types.Contains(typeof(TerrainFeature));
				if (flag)
				{
					bool flag2 = !location.terrainFeatures.ContainsKey(keyValuePair.Key) && !location.objects.ContainsKey(keyValuePair.Key);
					if (flag2)
					{
						location.terrainFeatures[keyValuePair.Key] = keyValuePair.Value;
					}
				}
			}
			foreach (KeyValuePair<Vector2, SObject> keyValuePair2 in gameLocation.objects.Pairs)
			{
				bool flag3 = types == null || types.Contains(keyValuePair2.Value.GetType()) || (parentSheetIndices != null && parentSheetIndices.Contains(keyValuePair2.Value.ParentSheetIndex)) || types.Contains(typeof(SObject)) || (Generator.GeneratorOptions.ContainsKey("Forage") && Generator.GeneratorOptions["Forage"] && keyValuePair2.Value.isForage(location));
				if (flag3)
				{
					bool flag4 = !location.terrainFeatures.ContainsKey(keyValuePair2.Key) && !location.objects.ContainsKey(keyValuePair2.Key);
					if (flag4)
					{
						location.objects[keyValuePair2.Key] = keyValuePair2.Value;
					}
				}
			}
			foreach (LargeTerrainFeature largeTerrainFeature in gameLocation.largeTerrainFeatures)
			{
				bool flag5 = types == null || types.Contains(largeTerrainFeature.GetType());
				if (flag5)
				{
					bool flag6 = !location.largeTerrainFeatures.Contains(largeTerrainFeature);
					if (flag6)
					{
						location.largeTerrainFeatures.Add(largeTerrainFeature);
					}
				}
			}
			bool flag7 = location is Farm;
			if (flag7)
			{
				foreach (ResourceClump resourceClump in clumps)
				{
					bool flag8 = types == null || types.Contains(resourceClump.GetType()) || (parentSheetIndices != null && parentSheetIndices.Contains(resourceClump.parentSheetIndex.Value));
					if (flag8)
					{
						bool flag9 = !(location as Farm).resourceClumps.Contains(resourceClump);
						if (flag9)
						{
							(location as Farm).addResourceClumpAndRemoveUnderlyingTerrain(resourceClump.parentSheetIndex.Value, resourceClump.width.Value, resourceClump.height.Value, resourceClump.tile.Value);
						}
					}
				}
			}
		}

		public static List<ResourceClump> getClumps(GameLocation location)
		{
			List<ResourceClump> list = new List<ResourceClump>();
			foreach (FieldInfo fieldInfo in location.GetType().GetFields())
			{
				bool flag = fieldInfo.FieldType == typeof(ResourceClump);
				if (flag)
				{
					list.Add(fieldInfo.GetValue(location) as ResourceClump);
				}
				else
				{
					bool flag2 = fieldInfo.FieldType is ICollection<ResourceClump>;
					if (flag2)
					{
						foreach (ResourceClump item in (fieldInfo as ICollection<ResourceClump>))
						{
							list.Add(item);
						}
					}
				}
			}
			return list;
		}

		public static GameLocation makeClone(GameLocation source)
		{
			Type type = source.GetType();
			DoLog.Log("Location was of type " + type.ToString(), 0);
			ConstructorInfo constructor = type.GetConstructor(new Type[]
			{
				typeof(string),
				typeof(string)
			});
			bool flag = constructor != null;
			if (flag)
			{
                DoLog.Log("Found a (string, string) constructor.  Invoking...", 0);
				try
				{
					GameLocation gameLocation = constructor.Invoke(new object[]
					{
						source.mapPath.Value,
						source.Name
					}) as GameLocation;
                    DoLog.Log("Constructed location was of type " + gameLocation.GetType().ToString(), 0);
					gameLocation.DayUpdate(Game1.dayOfMonth);
					return gameLocation;
				}
				catch (Exception)
				{
				}
			}
			ConstructorInfo constructor2 = type.GetConstructor(new Type[0]);
			bool flag2 = constructor2 != null;
			GameLocation result;
			if (flag2)
			{
                DoLog.Log("Found an empty constructor.  Invoking...", 0);
				GameLocation gameLocation2 = constructor2.Invoke(new object[0]) as GameLocation;
                DoLog.Log("Constructed location was of type " + gameLocation2.GetType().ToString(), 0);
				gameLocation2.loadObjects();
				gameLocation2.DayUpdate(Game1.dayOfMonth);
				result = gameLocation2;
			}
			else
			{
                DoLog.Log("Could not construct the location.  Defaulting to GameLocation class...", 0);
				result = new GameLocation(source.mapPath.Value, source.Name);
			}
			return result;
		}

		public static void loadTerrainFeatures(GameLocation location, Type[] types = null)
		{
			Dictionary<Vector2, TerrainFeature> dictionary = new Dictionary<Vector2, TerrainFeature>();
			foreach (KeyValuePair<Vector2, TerrainFeature> keyValuePair in location.terrainFeatures.Pairs)
			{
				dictionary.Add(keyValuePair.Key, keyValuePair.Value);
			}
			location.terrainFeatures.Clear();
			Dictionary<Vector2, SObject> dictionary2 = new Dictionary<Vector2, SObject>();
			foreach (KeyValuePair<Vector2, SObject> keyValuePair2 in location.objects.Pairs)
			{
				dictionary2.Add(keyValuePair2.Key, keyValuePair2.Value);
			}
			location.objects.Clear();
			List<LargeTerrainFeature> list = new List<LargeTerrainFeature>();
			foreach (LargeTerrainFeature item in location.largeTerrainFeatures)
			{
				list.Add(item);
			}
			location.largeTerrainFeatures.Clear();
			location.loadObjects();
			foreach (KeyValuePair<Vector2, TerrainFeature> keyValuePair3 in location.terrainFeatures.Pairs)
			{
				bool flag = types == null || types.Contains(keyValuePair3.Value.GetType());
				if (flag)
				{
					dictionary[keyValuePair3.Key] = keyValuePair3.Value;
				}
			}
			foreach (KeyValuePair<Vector2, SObject> keyValuePair4 in location.objects.Pairs)
			{
				bool flag2 = types == null || types.Contains(keyValuePair4.Value.GetType());
				if (flag2)
				{
					dictionary2[keyValuePair4.Key] = keyValuePair4.Value;
				}
			}
			foreach (LargeTerrainFeature largeTerrainFeature in location.largeTerrainFeatures)
			{
				bool flag3 = types == null || types.Contains(largeTerrainFeature.GetType());
				if (flag3)
				{
					bool flag4 = !list.Contains(largeTerrainFeature);
					if (flag4)
					{
						list.Add(largeTerrainFeature);
					}
				}
			}
			location.terrainFeatures.Clear();
			location.objects.Clear();
			location.largeTerrainFeatures.Clear();
			foreach (Vector2 vector in dictionary.Keys)
			{
				location.terrainFeatures[vector] = dictionary[vector];
			}
			foreach (Vector2 vector2 in dictionary2.Keys)
			{
				location.objects[vector2] = dictionary2[vector2];
			}
			foreach (LargeTerrainFeature largeTerrainFeature2 in list)
			{
				bool flag5 = !location.largeTerrainFeatures.Contains(largeTerrainFeature2);
				if (flag5)
				{
					location.largeTerrainFeatures.Add(largeTerrainFeature2);
				}
			}
		}

		public static GameLocation duplicateAndGenerate(GameLocation location)
		{
			bool flag = location.map == null;
			GameLocation result;
			if (flag)
			{
				bool flag2 = location is MineShaft;
				if (flag2)
				{
					result = new MineShaft((location as MineShaft).mineLevel);
				}
				else
				{
					result = null;
				}
			}
			else
			{
				bool flag3 = location.Name == null;
				if (flag3)
				{
					result = null;
				}
				else
				{
					bool flag4 = location is MineShaft;
					if (flag4)
					{
						result = new MineShaft((location as MineShaft).mineLevel);
					}
					else
					{
						bool flag5 = location is Town;
						if (flag5)
						{
							result = new Town(location.mapPath.Value, location.Name);
						}
						else
						{
							bool flag6 = location is Farm;
							if (flag6)
							{
								result = new Farm(location.mapPath.Value, location.Name);
							}
							else
							{
								bool flag7 = location is Woods;
								if (flag7)
								{
									Woods woods = new Woods(location.mapPath.Value, location.Name);
									woods.DayUpdate(Game1.dayOfMonth);
									result = woods;
								}
								else
								{
									bool flag8 = location is Forest;
									if (flag8)
									{
										result = new Forest(location.mapPath.Value, location.Name);
									}
									else
									{
										result = new GameLocation(location.mapPath.Value, location.Name);
									}
								}
							}
						}
					}
				}
			}
			return result;
		}
	}
}
