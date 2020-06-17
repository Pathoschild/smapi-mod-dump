using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.TerrainFeatures;
using Harmony;

namespace DestroyableBushes
{
    public static class HarmonyPatch_BushesDropWood
    {
        /// <summary>Applies this Harmony patch to the game through the provided instance.</summary>
        /// <param name="harmony">This mod's Harmony instance.</param>
        public static void ApplyPatch(HarmonyInstance harmony)
        {
            ModEntry.Instance.Monitor.Log($"Applying Harmony patch \"{nameof(HarmonyPatch_BushesDropWood)}\": postfixing SDV method \"Bush.performToolAction(Tool, int, Vector2, GameLocation)\".", LogLevel.Trace);
            harmony.Patch(
                original: AccessTools.Method(typeof(Bush), nameof(Bush.performToolAction), new[] { typeof(Tool), typeof(int), typeof(Vector2), typeof(GameLocation) }),
                postfix: new HarmonyMethod(typeof(HarmonyPatch_BushesDropWood), nameof(performToolAction_Postfix))
            );
        }

        /// <summary>If this bush was destroyed, this drops an amount of wood designated by this mod's config.json file settings.</summary>
        /// <param name="t">The <see cref="Tool"/> used on this bush.</param>
        /// <param name="__instance">The <see cref="Bush"/> on which a tool is being used.</param>
        /// <param name="__result">True if this bush was destroyed."/></param>
        public static void performToolAction_Postfix(Tool t, Vector2 tileLocation, GameLocation location, Bush __instance, bool __result)
        {
            try
            {
                if (__result && location != null && ModEntry.Config.AmountOfWoodDropped != null) //if this bush was destroyed AND relevant settings aren't null
                {
                    int amountOfWood = 0; //the amount of wood this bush should drop

                    switch (__instance.size.Value) //based on the bush's size, set the amount of wood
                    {
                        case Bush.smallBush:
                            amountOfWood = ModEntry.Config.AmountOfWoodDropped.SmallBushes;
                            break;
                        case Bush.mediumBush:
                            amountOfWood = ModEntry.Config.AmountOfWoodDropped.MediumBushes;
                            break;
                        case Bush.largeBush:
                            amountOfWood = ModEntry.Config.AmountOfWoodDropped.LargeBushes;
                            break;
                        case Bush.greenTeaBush:
                            amountOfWood = ModEntry.Config.AmountOfWoodDropped.GreenTeaBushes;
                            break;
                        default:
                            break;
                    }

                    if (amountOfWood > 0) //if this bush should drop any wood
                    {
                        if (t?.getLastFarmerToUse() is Farmer farmer && farmer.professions.Contains(12)) //if the player destroying this bush has the "Forester" profession
                        {
                            double multipliedWood = 1.25 * amountOfWood; //increase wood by 25%
                            amountOfWood = (int)Math.Floor(multipliedWood); //update the amount of wood (round down)
                            if (multipliedWood > amountOfWood) //if the multiplied wood had a decimal
                            {
                                multipliedWood -= amountOfWood; //get the decimal amount of wood
                                if (Game1.random.NextDouble() < multipliedWood) //use the decimal as a random chance
                                {
                                    amountOfWood++; //add 1 wood
                                }
                            }
                        }

                        //drop the amount of wood at this bush's location
                        Game1.createRadialDebris(location, 12, (int)tileLocation.X, (int)tileLocation.Y, amountOfWood, true, -1, false, -1);
                    }
                }
            }
            catch (Exception ex)
            {
                ModEntry.Instance.Monitor.LogOnce($"Harmony patch \"{nameof(performToolAction_Postfix)}\" has encountered an error:\n{ex.ToString()}", LogLevel.Error);
            }
        }
    }
}
