/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pet-Slime/StardewValley
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using static BirbCore.Attributes.SMod;
using xTile.Dimensions;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace BuffProfessions.Core
{
    [HarmonyPatch(typeof(StardewValley.Tools.Axe), nameof(StardewValley.Tools.Axe.beginUsing))]
    public class AxeBeginUsing_patch
    {
        [HarmonyLib.HarmonyPrefix]
        private static bool Prefix(Axe __instance, GameLocation location, int x, int y, Farmer who)
        {
            // Copied from Stardewvalley.Tool

            if (who.professions.Contains(14) && __instance.UpgradeLevel > 0)
            {
                who.Halt();
                __instance.Update(who.FacingDirection, 0, who);
                switch (who.FacingDirection)
                {
                    case Game1.up:
                        who.FarmerSprite.setCurrentFrame(176);
                        __instance.Update(0, 0, who);
                        break;

                    case Game1.right:
                        who.FarmerSprite.setCurrentFrame(168);
                        __instance.Update(1, 0, who);
                        break;

                    case Game1.down:
                        who.FarmerSprite.setCurrentFrame(160);
                        __instance.Update(2, 0, who);
                        break;

                    case Game1.left:
                        who.FarmerSprite.setCurrentFrame(184);
                        __instance.Update(3, 0, who);
                        break;
                }

                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(StardewValley.Tools.Axe), nameof(StardewValley.Tools.Axe.DoFunction))]
    public class AxeFunction_patch
    {
        [HarmonyLib.HarmonyPrefix]
        private static bool Prefix(Axe __instance, GameLocation location, int x, int y, int power, Farmer who)
        {
            if (who.professions.Contains(14) && __instance.UpgradeLevel > 0)
            {
                LumberjackBuff(__instance, location, x, y, power, who);
                return false; // don't run original logic
            }
            return true;
        }



        public static void LumberjackBuff(Axe tool, GameLocation location, int x, int y, int power, Farmer who)
        {
            tool.lastUser = who;
            Game1.recentMultiplayerRandom = Utility.CreateRandom((short)Game1.random.Next(-32768, 32768));
            if (!tool.IsEfficient)
            {
                who.Stamina -= (float)(2 * power) - (float)who.ForagingLevel * 0.1f;
            }

            power = who.toolPower.Value;
            who.stopJittering();
            Vector2 vector = new Vector2(x / 64, y / 64);
            List<Vector2> list = TilesAffected(vector, power, who);

            foreach (Vector2 item in list)
            {
                int num = (int)item.X;
                int num2 = (int)item.Y;
                Rectangle value = new Rectangle(num * 64, num2 * 64, 64, 64);
                if (location.Map.RequireLayer("Buildings").Tiles[num, num2] != null && location.Map.RequireLayer("Buildings").Tiles[num, num2].TileIndexProperties.ContainsKey("TreeStump"))
                {
                    Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Axe.cs.14023"));
                    continue;
                }

                tool.UpgradeLevel += tool.additionalPower.Value;
                location.performToolAction(tool, num, num2);
                if (location.terrainFeatures.TryGetValue(item, out var value2) && value2.performToolAction(tool, 0, item))
                {
                    location.terrainFeatures.Remove(item);
                }

                if (location.largeTerrainFeatures != null)
                {
                    for (int num3 = location.largeTerrainFeatures.Count - 1; num3 >= 0; num3--)
                    {
                        LargeTerrainFeature largeTerrainFeature = location.largeTerrainFeatures[num3];
                        if (largeTerrainFeature.getBoundingBox().Intersects(value) && largeTerrainFeature.performToolAction(tool, 0, item))
                        {
                            location.largeTerrainFeatures.RemoveAt(num3);
                        }
                    }
                }

                Vector2 key = new Vector2(num, num2);
                if (location.Objects.TryGetValue(key, out var value3) && value3.Type != null && value3.performToolAction(tool))
                {
                    if (value3.Type == "Crafting" && value3.Fragility != 2)
                    {
                        location.debris.Add(new Debris(value3.QualifiedItemId, who.GetToolLocation(), Utility.PointToVector2(who.StandingPixel)));
                    }

                    value3.performRemoveAction();
                    location.Objects.Remove(key);
                }

                tool.UpgradeLevel -= tool.additionalPower.Value;
            }
        }

        public static List<Vector2> TilesAffected(Vector2 tileLocation, int power, Farmer who)
        {
            power++;
            List<Vector2> list = new List<Vector2>();
            list.Add(tileLocation);
            Vector2 vector = Vector2.Zero;
            switch (who.FacingDirection)
            {
                case 0:
                    if (power >= 6)
                    {
                        vector = new Vector2(tileLocation.X, tileLocation.Y - 2f);
                        break;
                    }

                    if (power >= 2)
                    {
                        list.Add(tileLocation + new Vector2(0f, -1f));
                        list.Add(tileLocation + new Vector2(0f, -2f));
                    }

                    if (power >= 3)
                    {
                        list.Add(tileLocation + new Vector2(0f, -3f));
                        list.Add(tileLocation + new Vector2(0f, -4f));
                    }

                    if (power >= 4)
                    {
                        list.RemoveAt(list.Count - 1);
                        list.RemoveAt(list.Count - 1);
                        list.Add(tileLocation + new Vector2(1f, -2f));
                        list.Add(tileLocation + new Vector2(1f, -1f));
                        list.Add(tileLocation + new Vector2(1f, 0f));
                        list.Add(tileLocation + new Vector2(-1f, -2f));
                        list.Add(tileLocation + new Vector2(-1f, -1f));
                        list.Add(tileLocation + new Vector2(-1f, 0f));
                    }

                    if (power >= 5)
                    {
                        for (int num3 = list.Count - 1; num3 >= 0; num3--)
                        {
                            list.Add(list[num3] + new Vector2(0f, -3f));
                        }
                    }

                    break;
                case 1:
                    if (power >= 6)
                    {
                        vector = new Vector2(tileLocation.X + 2f, tileLocation.Y);
                        break;
                    }

                    if (power >= 2)
                    {
                        list.Add(tileLocation + new Vector2(1f, 0f));
                        list.Add(tileLocation + new Vector2(2f, 0f));
                    }

                    if (power >= 3)
                    {
                        list.Add(tileLocation + new Vector2(3f, 0f));
                        list.Add(tileLocation + new Vector2(4f, 0f));
                    }

                    if (power >= 4)
                    {
                        list.RemoveAt(list.Count - 1);
                        list.RemoveAt(list.Count - 1);
                        list.Add(tileLocation + new Vector2(0f, -1f));
                        list.Add(tileLocation + new Vector2(1f, -1f));
                        list.Add(tileLocation + new Vector2(2f, -1f));
                        list.Add(tileLocation + new Vector2(0f, 1f));
                        list.Add(tileLocation + new Vector2(1f, 1f));
                        list.Add(tileLocation + new Vector2(2f, 1f));
                    }

                    if (power >= 5)
                    {
                        for (int num2 = list.Count - 1; num2 >= 0; num2--)
                        {
                            list.Add(list[num2] + new Vector2(3f, 0f));
                        }
                    }

                    break;
                case 2:
                    if (power >= 6)
                    {
                        vector = new Vector2(tileLocation.X, tileLocation.Y + 2f);
                        break;
                    }

                    if (power >= 2)
                    {
                        list.Add(tileLocation + new Vector2(0f, 1f));
                        list.Add(tileLocation + new Vector2(0f, 2f));
                    }

                    if (power >= 3)
                    {
                        list.Add(tileLocation + new Vector2(0f, 3f));
                        list.Add(tileLocation + new Vector2(0f, 4f));
                    }

                    if (power >= 4)
                    {
                        list.RemoveAt(list.Count - 1);
                        list.RemoveAt(list.Count - 1);
                        list.Add(tileLocation + new Vector2(1f, 2f));
                        list.Add(tileLocation + new Vector2(1f, 1f));
                        list.Add(tileLocation + new Vector2(1f, 0f));
                        list.Add(tileLocation + new Vector2(-1f, 2f));
                        list.Add(tileLocation + new Vector2(-1f, 1f));
                        list.Add(tileLocation + new Vector2(-1f, 0f));
                    }

                    if (power >= 5)
                    {
                        for (int num4 = list.Count - 1; num4 >= 0; num4--)
                        {
                            list.Add(list[num4] + new Vector2(0f, 3f));
                        }
                    }

                    break;
                case 3:
                    if (power >= 6)
                    {
                        vector = new Vector2(tileLocation.X - 2f, tileLocation.Y);
                        break;
                    }

                    if (power >= 2)
                    {
                        list.Add(tileLocation + new Vector2(-1f, 0f));
                        list.Add(tileLocation + new Vector2(-2f, 0f));
                    }

                    if (power >= 3)
                    {
                        list.Add(tileLocation + new Vector2(-3f, 0f));
                        list.Add(tileLocation + new Vector2(-4f, 0f));
                    }

                    if (power >= 4)
                    {
                        list.RemoveAt(list.Count - 1);
                        list.RemoveAt(list.Count - 1);
                        list.Add(tileLocation + new Vector2(0f, -1f));
                        list.Add(tileLocation + new Vector2(-1f, -1f));
                        list.Add(tileLocation + new Vector2(-2f, -1f));
                        list.Add(tileLocation + new Vector2(0f, 1f));
                        list.Add(tileLocation + new Vector2(-1f, 1f));
                        list.Add(tileLocation + new Vector2(-2f, 1f));
                    }

                    if (power >= 5)
                    {
                        for (int num = list.Count - 1; num >= 0; num--)
                        {
                            list.Add(list[num] + new Vector2(-3f, 0f));
                        }
                    }

                    break;
            }

            if (power >= 6)
            {
                list.Clear();
                for (int i = (int)vector.X - 2; (float)i <= vector.X + 2f; i++)
                {
                    for (int j = (int)vector.Y - 2; (float)j <= vector.Y + 2f; j++)
                    {
                        list.Add(new Vector2(i, j));
                    }
                }
            }

            return list;
        }
    }

}
