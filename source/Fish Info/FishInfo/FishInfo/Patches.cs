/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Speshkitty/FishInfo
**
*************************************************/

using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Reflection;
using System.Text;

namespace FishInfo
{
    public class Patches
    {
        public static void DoPatches()
        {
            HarmonyInstance harmony = HarmonyInstance.Create("speshkitty.fishinfo.harmony");

            harmony.Patch(typeof(CollectionsPage).GetMethod("performHoverAction", BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly),
                postfix: new HarmonyMethod(typeof(CollectionsPage_PerformHoverAction).GetMethod("Postfix")));

            harmony.Patch(typeof(CollectionsPage).GetMethod("createDescription", BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly),
                postfix: new HarmonyMethod(typeof(CollectionsPage_CreateDescription).GetMethod("Postfix")));
        }
        
        public class CollectionsPage_PerformHoverAction
        {
            public static void Postfix(CollectionsPage __instance, ref string ___hoverText, int ___currentTab, int ___currentPage, int x, int y)
            {
                foreach (ClickableTextureComponent c in __instance.collections[___currentTab][___currentPage])
                {
                    if (!c.containsPoint(x, y))
                    {
                        c.scale = Math.Max(c.scale - 0.02f, c.baseScale);
                    }
                    else
                    {
                        c.scale = Math.Min(c.scale + 0.02f, c.baseScale + 0.1f);
                        if (___currentTab == 1) // We only care about the fish tab
                        {
                            int FishID = Convert.ToInt32(c.name.Split(new char[] { ' ' })[0]);
                            if (!ModEntry.FishInfo.TryGetValue(FishID, out FishData fishData))
                            {
                                ___hoverText = "???";
                                continue;
                            }

                            if (!Convert.ToBoolean(c.name.Split(new char[] { ' ' })[1])) //Isn't unlocked
                            {
                                if (!ModEntry.Config.RequireFishCaughtForFullInfo) //if we require fish to be caught to show the info
                                {
                                    ___hoverText = fishData.ToString();
                                    continue;
                                }
                                else
                                {
                                    ___hoverText = "";
                                    StringBuilder sb = new StringBuilder();
                                    if (ModEntry.Config.UncaughtFishAlwaysShowName)
                                    {
                                        sb.Append(fishData.FishName);
                                    }
                                    if (ModEntry.Config.UncaughtFishAlwaysShowLocation)
                                    {
                                        sb.AppendLine(fishData.InfoLocation);
                                    }
                                    if (ModEntry.Config.UncaughtFishAlwaysShowSeason)
                                    {
                                        sb.AppendLine(fishData.InfoSeason);
                                    }
                                    if (ModEntry.Config.UncaughtFishAlwaysShowTime)
                                    {
                                        sb.AppendLine(fishData.InfoTime);
                                    }
                                    if (ModEntry.Config.UncaughtFishAlwaysShowWeather)
                                    {
                                        sb.AppendLine(fishData.InfoWeather);
                                    }

                                    ___hoverText = sb.ToString().Trim();
                                }

                            }
                        }
                    }
                }
            }
        }
        
        public class CollectionsPage_CreateDescription
        {
            public static void Postfix(CollectionsPage __instance, ref string __result, int index)
            {
                if (!Game1.objectInformation.TryGetValue(index, out string itemData)) // Break if it's not an item - dodges weird edge cases
                {
                    return;
                }

                string[] split = itemData.Split(new char[] { '/' });

                if (!split[3].Contains("Fish")) { return; } //break if it isn't a fish
                
                if (ModEntry.FishInfo.TryGetValue(index, out FishData loadedData))
                {
                    __result += loadedData.InfoLocation;

                    if (!loadedData.IsCrabPot)
                    {
                        __result += loadedData.InfoSeason;
                        __result += loadedData.InfoTime;
                        __result += loadedData.InfoWeather;
                    }
                }
                
            }
        }
        
    }
}
