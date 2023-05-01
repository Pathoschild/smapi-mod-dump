/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DiogoAlbano/StardewValleyMods
**
*************************************************/

using StardewValley.TerrainFeatures;
using StardewValley;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SObject = StardewValley.Object;

namespace BNWCore
{
    internal static class Remove_Crows_Event_Patches
    {
        public static bool Farm_AddCrows(ref Farm __instance)
        {
            if (Game1.MasterPlayer.mailReceived.Contains("nature_foraging_blessing"))
            {
                return false;
            }
            else
            {
                int num1 = 0;
                foreach (KeyValuePair<Vector2, TerrainFeature> pair in __instance.terrainFeatures.Pairs)
                {
                    if (pair.Value is HoeDirt dirt && dirt.crop != null)
                        ++num1;
                }
                List<Vector2> vector2List = new List<Vector2>();
                foreach (KeyValuePair<Vector2, SObject> pair in __instance.objects.Pairs)
                {
                    if (pair.Value.Name.Contains("arecrow"))
                    {
                        vector2List.Add(pair.Key);
                    }
                }
                int num2 = System.Math.Min(4, num1 / 16);
                for (int index1 = 0; index1 < num2; ++index1)
                {
                    if (Game1.random.NextDouble() < 1.0)
                    {
                        for (int index2 = 0; index2 < 10; ++index2)
                        {
                            Vector2 key = __instance.terrainFeatures.Pairs.ElementAt(Game1.random.Next(__instance.terrainFeatures.Count())).Key;
                            if (__instance.terrainFeatures[key] is HoeDirt dirt && dirt.crop?.currentPhase.Value > 1)
                            {
                                bool flag = false;
                                foreach (Vector2 index3 in vector2List)
                                {
                                    if (Vector2.Distance(index3, key) < 9.0)
                                    {
                                        flag = true;
                                        ++__instance.objects[index3].SpecialVariable;
                                        break;
                                    }
                                }
                                if (!flag)
                                    dirt.crop = null;
                                break;
                            }
                        }
                    }
                }
            }
            return false;
        }
    }
}