/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;

namespace CustomCaskMod
{
    internal class CaskOverrides
    {
        public static bool PerformObjectDropInAction(ref Cask __instance, ref Item dropIn, ref bool probe, ref Farmer who, ref bool __result)
        {
            if (dropIn != null && dropIn is StardewValley.Object &&
                (dropIn as StardewValley.Object).bigCraftable.Value || __instance.heldObject.Value != null)
            {
                __result = false;
                return false;
            }
            if (!probe && (who == null || !(who.currentLocation is Cellar)) && !DataLoader.ModConfig.EnableCasksAnywhere)
            {
                Game1.showRedMessageUsingLoadString("Strings\\Objects:CaskNoCellar");
                __result = false;
                return false;
            }

            if (__instance.Quality >= 4)
            {
                __result = false;
                return false;
            }
            bool flag = false;
            float num = 1f;

            if (DataLoader.CaskDataId.ContainsKey(dropIn.ParentSheetIndex))
            {
                flag = true;
                num = DataLoader.CaskDataId[dropIn.ParentSheetIndex];
            }
            else if (DataLoader.CaskDataId.ContainsKey(dropIn.Category))
            {
                flag = true;
                num = DataLoader.CaskDataId[dropIn.Category];
            }
            else
            {
                switch (dropIn.ParentSheetIndex)
                {
                    case 303:
                        flag = true;
                        num = 1.66f;
                        break;
                    case 346:
                        flag = true;
                        num = 2f;
                        break;
                    case 348:
                        flag = true;
                        num = 1f;
                        break;
                    case 424:
                        flag = true;
                        num = 4f;
                        break;
                    case 426:
                        flag = true;
                        num = 4f;
                        break;
                    case 459:
                        flag = true;
                        num = 2f;
                        break;
                }
            }
            

            if (!flag)
            {
                __result = false;
                return false;
            }
            __instance.heldObject.Value = dropIn.getOne() as StardewValley.Object;
            if (!probe)
            {
                __instance.agingRate.Value = num;
                __instance.daysToMature.Value = 56f;
                __instance.MinutesUntilReady = 999999;
                if (__instance.heldObject.Value.Quality == 1)
                    __instance.daysToMature.Value = 42f;
                else if (__instance.heldObject.Value.Quality == 2)
                    __instance.daysToMature.Value = 28f;
                else if (__instance.heldObject.Value.Quality == 4)
                {
                    __instance.daysToMature.Value = 0.0f;
                    __instance.MinutesUntilReady = 1;
                }
                who.currentLocation.playSound("Ship");
                who.currentLocation.playSound("bubbles");
                Multiplayer multiplayer = DataLoader.Helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();
                multiplayer.broadcastSprites
                (
                    who.currentLocation
                    , new TemporaryAnimatedSprite[1]
                    {
                        new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(256, 1856, 64, 128), 80f, 6, 999999, __instance.TileLocation * 64f + new Vector2(0.0f, (float) sbyte.MinValue), false, false, (float) (((double) __instance.TileLocation.Y + 1.0) * 64.0 / 10000.0 + 9.99999974737875E-05), 0.0f, Color.Yellow * 0.75f, 1f, 0.0f, 0.0f, 0.0f, false)
                        {
                            alphaFade = 0.005f
                        }
                    }
                );
            }
            __result = true;
            return false;
        }

        public static void CaskMachine(object __instance)
        {
            IReflectedField<Dictionary<int, float>> agingRates = CustomCaskModEntry.Helper.Reflection.GetField<Dictionary<int, float>>(__instance, "AgingRates");

            foreach (var keyValuePair in DataLoader.CaskDataId)
            {
                agingRates.GetValue()[keyValuePair.Key] = keyValuePair.Value;
            }
        }
    }
}
