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

        public static bool TilesAffectedPatch(ref Tool __instance, ref List<Vector2> __result, ref Vector2 tileLocation, ref int power, ref Farmer who)
        {
            try
            {
                // this should always be the case, but just to make sure
                if (__instance is WateringCan || __instance is Hoe)
                {
                    // set the list of affected tiles ourselves
                    __result = NewTilesAffected(ref __instance, ref tileLocation, ref power, ref who);

                    // don't call original method
                    return false;
                }
                else
                {
                    // call original method
                    return true;
                }
            }
            catch (Exception e)
            {
                mod.ErrorLog("There was an exception in a patch", e);
                return true;
            }
        }

        public static List<Vector2> NewTilesAffected(ref Tool tool, ref Vector2 tileLocation, ref int power, ref Farmer who)
        {
            power++;
            var tileLocations = new List<Vector2>
            {
                Vector2.Zero
            };

            if (tool is WateringCan)
            {
                if (power >= 2)
                {
                    tileLocations.Add(new Vector2(1f, 0f));
                    tileLocations.Add(new Vector2(-1f, 0));
                }

                if (power >= 3)
                {
                    tileLocations.Add(new Vector2(0f, -1f));
                    tileLocations.Add(new Vector2(1f, -1f));
                    tileLocations.Add(new Vector2(-1f, -1f));
                }
            }
            else if (tool is Hoe)
            {
                if (power >= 2)
                {
                    tileLocations.Add(new Vector2(0f, -1f));
                    tileLocations.Add(new Vector2(0f, -2f));
                }

                if (power >= 3)
                {
                    tileLocations.Add(new Vector2(0f, -3f));
                    tileLocations.Add(new Vector2(0f, -4f));
                    tileLocations.Add(new Vector2(0f, -5f));
                }
            }

            if (power >= 4)
            {
                tileLocations.Clear();

                for (int i = 0; i < 3; i++)
                {
                    tileLocations.Add(new Vector2(0f, -i));
                    tileLocations.Add(new Vector2(1f, -i));
                    tileLocations.Add(new Vector2(-1f, -i));
                }
            }

            if (power >= 5)
            {
                // we have to iterate forwards so the animation is the right way around
                // we have to make a copy of the count so it doesn't loop forever
                int count = tileLocations.Count;

                for (int i = 0; i < count; i++)
                {
                    tileLocations.Add(tileLocations[i] + new Vector2(0f, -3f));
                }
            }

            if (power >= 6)
            {
                // again clear to have the right order of the elements, less expensive than sorting
                tileLocations.Clear();

                for (int i = 0; i < 6; i++)
                {
                    tileLocations.Add(new Vector2(0f, -i));
                    tileLocations.Add(new Vector2(1f, -i));
                    tileLocations.Add(new Vector2(-1f, -i));
                    tileLocations.Add(new Vector2(2f, -i));
                    tileLocations.Add(new Vector2(-2f, -i));
                }
            }

            // turn the offsets made for facing direction 0 to work for the current facing direction
            AdjustForFacingDirection(ref tileLocations, who.FacingDirection);

            // add the tile location after rotating so the math works out
            for (int i = 0; i < tileLocations.Count; i++)
            {
                tileLocations[i] += tileLocation;
            }

            return tileLocations;
        }

        public override void Entry(IModHelper helper)
        {
            mod = this;
            var harmony = new Harmony(ModManifest.UniqueID);

            try
            {
                harmony.Patch(
                   original: AccessTools.Method(typeof(Tool), "tilesAffected"),
                   prefix: new HarmonyMethod(typeof(ChangedWateringCanAndHoeArea), nameof(TilesAffectedPatch)));
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

        private static void AdjustForFacingDirection(ref List<Vector2> tileLocations, int facingDirection)
        {
            switch (facingDirection)
            {
                case 1:
                    for (int i = 0; i < tileLocations.Count; i++)
                    {
                        tileLocations[i] = new Vector2(-tileLocations[i].Y, -tileLocations[i].X);
                    }

                    break;

                case 2:
                    for (int i = 0; i < tileLocations.Count; i++)
                    {
                        tileLocations[i] = -tileLocations[i];
                    }

                    break;

                case 3:
                    for (int i = 0; i < tileLocations.Count; i++)
                    {
                        tileLocations[i] = new Vector2(tileLocations[i].Y, tileLocations[i].X);
                    }

                    break;
            }
        }
    }
}