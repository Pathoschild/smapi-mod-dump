/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Goldenrevolver/Stardew-Valley-Mods
**
*************************************************/

namespace ChangedWateringCanAndHoeArea
{
    using HarmonyLib;
    using Microsoft.Xna.Framework;
    using StardewModdingAPI;
    using StardewValley;
    using StardewValley.Tools;
    using System;
    using System.Collections.Generic;

    public class ChangedWateringCanAndHoeArea : Mod
    {
        private static ChangedWateringCanAndHoeArea mod;
        private static bool shouldNotPatchReaching = false;

        public static void TilesAffectedPatch(Tool __instance, Vector2 tileLocation, int power, Farmer who, ref List<Vector2> __result)
        {
            try
            {
                // this should always be the case, but just to make sure
                if (__instance is WateringCan || __instance is Hoe)
                {
                    if (power is 2 or 3 or 6)
                    {
                        if (power is 6 && shouldNotPatchReaching)
                        {
                            return;
                        }

                        // set the list of affected tiles ourselves
                        __result = NewTilesAffected(__instance, tileLocation, power, who);
                    }
                }
            }
            catch (Exception e)
            {
                mod.ErrorLog("There was an exception in a patch", e);
            }
        }

        private static List<Vector2> NewTilesAffected(Tool tool, Vector2 tileLocation, int power, Farmer who)
        {
            // this is done by the base implementation now (and power is no longer 'ref' anyway)
            ////power++;
            var newTileLocations = new List<Vector2>
            {
                Vector2.Zero
            };

            if (power >= 6)
            {
                // again clear to have the right order of the elements, less expensive than sorting
                newTileLocations.Clear();

                for (int i = 0; i < 6; i++)
                {
                    newTileLocations.Add(new Vector2(0f, -i));
                    newTileLocations.Add(new Vector2(1f, -i));
                    newTileLocations.Add(new Vector2(-1f, -i));
                    newTileLocations.Add(new Vector2(2f, -i));
                    newTileLocations.Add(new Vector2(-2f, -i));
                }
            }
            else
            {
                if (tool is WateringCan)
                {
                    if (power >= 2)
                    {
                        newTileLocations.Add(new Vector2(1f, 0f));
                        newTileLocations.Add(new Vector2(-1f, 0));
                    }

                    if (power >= 3)
                    {
                        newTileLocations.Add(new Vector2(0f, -1f));
                        newTileLocations.Add(new Vector2(1f, -1f));
                        newTileLocations.Add(new Vector2(-1f, -1f));
                    }
                }
                else if (tool is Hoe)
                {
                    if (power >= 2)
                    {
                        newTileLocations.Add(new Vector2(0f, -1f));
                        newTileLocations.Add(new Vector2(0f, -2f));
                    }

                    if (power >= 3)
                    {
                        newTileLocations.Add(new Vector2(0f, -3f));
                        newTileLocations.Add(new Vector2(0f, -4f));
                        newTileLocations.Add(new Vector2(0f, -5f));
                    }
                }

                // power >= 4 cannot happen anymore, but I will leave this here as legacy code
                if (power >= 4)
                {
                    newTileLocations.Clear();

                    for (int i = 0; i < 3; i++)
                    {
                        newTileLocations.Add(new Vector2(0f, -i));
                        newTileLocations.Add(new Vector2(1f, -i));
                        newTileLocations.Add(new Vector2(-1f, -i));
                    }
                }

                // power >= 5 cannot happen anymore, but I will leave this here as legacy code
                if (power >= 5)
                {
                    // we have to iterate forwards so the animation is the right way around
                    // we have to make a copy of the count so it doesn't loop forever
                    int count = newTileLocations.Count;

                    for (int i = 0; i < count; i++)
                    {
                        newTileLocations.Add(newTileLocations[i] + new Vector2(0f, -3f));
                    }
                }
            }

            if (who.FacingDirection != 0)
            {
                // turn the offsets made for facing direction 0 to work for the current facing direction
                AdjustForFacingDirection(newTileLocations, who.FacingDirection);
            }

            // add the tile location after rotating so the math works out
            for (int i = 0; i < newTileLocations.Count; i++)
            {
                newTileLocations[i] += tileLocation;
            }

            return newTileLocations;
        }

        private static void AdjustForFacingDirection(List<Vector2> tileLocations, int facingDirection)
        {
            for (int i = 0; i < tileLocations.Count; i++)
            {
                tileLocations[i] = facingDirection switch
                {
                    1 => new Vector2(-tileLocations[i].Y, -tileLocations[i].X),
                    2 => -tileLocations[i],
                    3 => new Vector2(tileLocations[i].Y, tileLocations[i].X),
                    _ => tileLocations[i],
                };
            }
        }

        public override void Entry(IModHelper helper)
        {
            mod = this;

            shouldNotPatchReaching = mod.Helper.ModRegistry.IsLoaded("kakashigr.RadioactiveTools");

            if (shouldNotPatchReaching)
            {
                ErrorLog("Disabled Reach Buff in favor of Radioactive Tools");
            }

            var harmony = new Harmony(ModManifest.UniqueID);

            try
            {
                harmony.Patch(
                   original: AccessTools.Method(typeof(Tool), "tilesAffected"),
                   postfix: new HarmonyMethod(typeof(ChangedWateringCanAndHoeArea), nameof(TilesAffectedPatch)));
            }
            catch (Exception e)
            {
                ErrorLog("Error while trying to setup required patches:", e);
            }
        }

        public void DebugLog(object o)
        {
            Monitor.Log(o == null ? "null" : o.ToString(), LogLevel.Debug);
        }

        public void ErrorLog(object o, Exception e = null)
        {
            string baseMessage = o == null ? "null" : o.ToString();

            string errorMessage = e == null ? string.Empty : $"\n{e.Message}\n{e.StackTrace}";

            Monitor.Log(baseMessage + errorMessage, LogLevel.Error);
        }
    }
}