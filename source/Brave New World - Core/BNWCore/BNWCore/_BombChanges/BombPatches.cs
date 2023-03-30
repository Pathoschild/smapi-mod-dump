/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DiogoAlbano/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;

namespace BNWCore.Patches
{
    public class BombPatches
    {
        private static readonly List<int> MineralIgnoreList = new List<int>()
        {
		//Weeds
		0, 313, 314, 315, 316, 317, 318, 319, 320, 321, 452, 674, 675, 676, 677, 678, 679, 750, 784, 785, 786, 792, 793, 794, 882, 883, 884,
        //Barrels
        118, 120, 122, 124, 174,
        //Crates
        119, 121, 123, 125, 175,
        //Stone
        2, 4, 75, 76, 77, 95, 290, 343, 450, 668, 670, 751, 760, 762, 764, 765, 816, 817, 818, 819, 843, 844, 845, 846, 847, 849, 850
        };
        public static bool Explode_Prefix(GameLocation __instance, Vector2 tileLocation, ref int radius, Farmer who, ref bool damageFarmers, ref int damage_amount)
        {
            if(Game1.MasterPlayer.mailReceived.Contains("fire_mining_blessing"))
            {
                try
                {
                    damageFarmers = InternalConfig.DamageFarmers;
                    radius = Convert.ToInt32(radius * InternalConfig.Radius);
                    if (damage_amount > 0)
                    {
                        damage_amount = Convert.ToInt32(damage_amount * InternalConfig.Damage);
                    }
                    else
                    {
                        damage_amount = Convert.ToInt32(radius * Game1.random.Next(radius * 6, (radius * 8) + 1) * InternalConfig.Damage);
                    }
                    ChangedExplode(__instance, tileLocation, radius, who);
                }
                catch (Exception ex)
                {
                    ModEntry.IMonitor.Log($"Failed in {nameof(Explode_Prefix)}:\n{ex}", LogLevel.Error);
                }               
            }
            return true;
        }
        public static void ChangedExplode(GameLocation instance, Vector2 tileLocation, int radius, Farmer who)
        {
            if (Game1.MasterPlayer.mailReceived.Contains("fire_mining_blessing"))
            {
                var area = new Rectangle(Convert.ToInt32(tileLocation.X - radius - 1f) * 64, Convert.ToInt32(tileLocation.Y - radius - 1f) * 64, (radius * 2 + 1) * 64, (radius * 2 + 1) * 64);
                if (InternalConfig.BreakClumps)
                {
                    var removed = new List<ResourceClump>(instance.resourceClumps.Count);
                    foreach (var clump in instance.resourceClumps)
                    {
                        var loc = clump.tile.Value;
                        if (clump.getBoundingBox(loc).Intersects(area))
                        {
                            var numChunks = ((clump.parentSheetIndex == 672) ? 15 : 10);
                            var debris = 390;
                            switch (clump.parentSheetIndex)
                            {
                                case ResourceClump.stumpIndex:
                                case ResourceClump.hollowLogIndex:
                                    debris = 709;
                                    instance.playSound("stumpCrack");
                                    break;
                                case ResourceClump.boulderIndex:
                                case ResourceClump.mineRock1Index:
                                case ResourceClump.mineRock2Index:
                                case ResourceClump.mineRock3Index:
                                case ResourceClump.mineRock4Index:
                                case ResourceClump.meteoriteIndex:
                                    instance.playSound("boulderBreak");
                                    break;
                            }
                            if (Game1.IsMultiplayer)
                            {
                                Game1.createMultipleObjectDebris(debris, (int)tileLocation.X, (int)tileLocation.Y, numChunks, who.UniqueMultiplayerID);
                            }
                            else
                            {
                                Game1.createRadialDebris(Game1.currentLocation, debris, (int)tileLocation.X, (int)tileLocation.Y, numChunks, resource: false, -1, item: true);
                            }
                            Game1.createRadialDebris(Game1.currentLocation, 32, (int)tileLocation.X, (int)tileLocation.Y, Game1.random.Next(6, 12), resource: false);
                            removed.Add(clump);
                        }
                    }
                    removed.ForEach(x => instance.resourceClumps.Remove(x));
                }
                if (InternalConfig.CollectMinerals)
                {
                    var removed = new List<Vector2>();
                    foreach (var obj in instance.Objects.Pairs)
                    {
                        if (!obj.Value.CanBeGrabbed)
                        {
                            continue;
                        }
                        else if (!obj.Value.IsSpawnedObject)
                        {
                            continue;
                        }
                        else if (MineralIgnoreList.Contains(obj.Value.ParentSheetIndex))
                        {
                            continue;
                        }
                        if (obj.Value.getBoundingBox(obj.Key).Intersects(area))
                        {
                            try
                            {
                                Game1.createObjectDebris(obj.Value.ParentSheetIndex, Convert.ToInt32(obj.Key.X), Convert.ToInt32(obj.Key.Y), who.uniqueMultiplayerID, instance);
                                removed.Add(obj.Key);
                            }
                            catch (KeyNotFoundException ex)
                            {
                            }
                        }
                    }
                    removed.ForEach(x => instance.destroyObject(x, who));
                }
            }
        }
        public static string ObjectParentSheetIndexToName(int psi)
        {
            switch (psi)
            {
                case 0:
                case 313:
                case 314:
                case 315:
                case 316:
                case 317:
                case 318:
                case 319:
                case 320:
                case 321:
                case 452:
                case 674:
                case 675:
                case 676:
                case 677:
                case 678:
                case 679:
                case 750:
                case 784:
                case 785:
                case 786:
                case 792:
                case 793:
                case 794:
                case 882:
                case 883:
                case 884:
                    return $"Weeds ({psi})";
                case 118:
                case 120:
                case 122:
                case 124:
                case 174:
                    return $"Barrel ({psi})";
                case 119:
                case 121:
                case 123:
                case 125:
                case 175:
                case 922:
                case 923:
                case 924:
                    return $"Crate ({psi})";
                case 2:
                case 4:
                case 75:
                case 76:
                case 77:
                case 95:
                case 290:
                case 343:
                case 450:
                case 668:
                case 670:
                case 751:
                case 760:
                case 762:
                case 764:
                case 765:
                case 816:
                case 817:
                case 818:
                case 819:
                case 843:
                case 844:
                case 845:
                case 846:
                case 847:
                case 849:
                case 850:
                    return $"Stone ({psi})";

                default:
                    return $"Unknown ({psi})";
            }
        }
    }
}