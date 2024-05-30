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
    [HarmonyPatch(typeof(StardewValley.Tools.Pickaxe), nameof(StardewValley.Tools.Pickaxe.beginUsing))]
    public class PickAxeBeginUsing_patch
    {
        [HarmonyLib.HarmonyPrefix]
        private static bool Prefix(Pickaxe __instance, GameLocation location, int x, int y, Farmer who)
        {
            // Copied from Stardewvalley.Tool

            if (who.professions.Contains(21) && __instance.UpgradeLevel > 0)
            {
                BirbCore.Attributes.Log.Warn($"The power of the {__instance.DisplayName} is {who.toolPower.Value}");
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

    [HarmonyPatch(typeof(StardewValley.Farmer), nameof(StardewValley.Farmer.toolPowerIncrease))]
    public class FarmerToolPower_patch
    {

        private static int ToolPitchAccumulator;

        [HarmonyLib.HarmonyPrefix]
        private static bool Prefix(Farmer __instance)
        {
            // Copied from Stardewvalley.Tool
            var who = __instance;
            if (who.professions.Contains(21) && who.CurrentTool is Pickaxe)
            {

                if (who.toolPower.Value == 0)
                {
                    ToolPitchAccumulator = 0;
                }

                who.toolPower.Value++;

                Color color = Color.White;
                int num = ((who.FacingDirection == 0) ? 4 : ((who.FacingDirection == 2) ? 2 : 0));
                switch (who.toolPower.Value)
                {
                    case 1:
                        color = Color.Orange;

                        who.jitterStrength = 0.25f;
                        break;
                    case 2:
                        color = Color.LightSteelBlue;

                        who.jitterStrength = 0.5f;
                        break;
                    case 3:
                        color = Color.Gold;
                        who.jitterStrength = 1f;
                        break;
                    case 4:
                        color = Color.Violet;
                        who.jitterStrength = 2f;
                        break;
                    case 5:
                        color = Color.BlueViolet;
                        who.jitterStrength = 3f;
                        break;
                }

                int num2 = ((who.FacingDirection == 1) ? 40 : ((who.FacingDirection == 3) ? (-40) : ((who.FacingDirection == 2) ? 32 : 0)));
                int num3 = 192;

                int y = who.StandingPixel.Y;
                Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(21, who.Position - new Vector2(num2, num3), color, 8, flipped: false, 70f, 0, 64, (float)y / 10000f + 0.005f, 128));
                Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(192, 1152, 64, 64), 50f, 4, 0, who.Position - new Vector2((who.FacingDirection != 1) ? (-64) : 0, 128f), flicker: false, who.FacingDirection == 1, (float)y / 10000f, 0.01f, Color.White, 1f, 0f, 0f, 0f));
                int value = Utility.CreateRandom(Game1.dayOfMonth, (double)who.Position.X * 1000.0, who.Position.Y).Next(12, 16) * 100 + who.toolPower.Value * 100;
                Game1.playSound("toolCharge", value);

                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(StardewValley.Tools.Pickaxe), nameof(StardewValley.Tools.Pickaxe.DoFunction))]
    public class PickAxeFunction_patch
    {

        private static int boulderTileX;
        private static int boulderTileY;
        private static int hitsToBoulder;


        [HarmonyLib.HarmonyPrefix]
        private static bool Prefix(Pickaxe __instance, GameLocation location, int x, int y, int power, Farmer who)
        {
            if (who.professions.Contains(21) && __instance.UpgradeLevel > 0)
            {
                ProspectorBuff(__instance, location, x, y, power, who);
                return false; // don't run original logic
            }
            return true;
        }



        public static void ProspectorBuff(Pickaxe tool, GameLocation location, int x, int y, int power, Farmer who)
        {
            tool.lastUser = who;
            Game1.recentMultiplayerRandom = Utility.CreateRandom((short)Game1.random.Next(-32768, 32768));
            if (!tool.IsEfficient)
            {
                who.Stamina -= (float)(2 * power) - (float)who.ForagingLevel * 0.1f;
            }

            Utility.clampToTile(new Vector2(x, y));
            BirbCore.Attributes.Log.Warn($"The power of the {tool.DisplayName} is {who.toolPower.Value}");
            power = who.toolPower.Value;
            who.stopJittering();
            Vector2 originSpot = new Vector2(x / 64, y / 64);
            List<Vector2> list = TilesAffected(originSpot, power, who);

            foreach (Vector2 item in list)
            {
                int num = (int)item.X;
                int num2 = (int)item.Y;
                Vector2 vector = new Vector2(num, num2);
                if (location.performToolAction(tool, num, num2))
                {
                    return;
                }

                location.Objects.TryGetValue(vector, out var value);
                if (value == null)
                {
                    if (who.FacingDirection == 0 || who.FacingDirection == 2)
                    {
                        num = (x - 8) / 64;
                        location.Objects.TryGetValue(new Vector2(num, num2), out value);
                        if (value == null)
                        {
                            num = (x + 8) / 64;
                            location.Objects.TryGetValue(new Vector2(num, num2), out value);
                        }
                    }
                    else
                    {
                        num2 = (y + 8) / 64;
                        location.Objects.TryGetValue(new Vector2(num, num2), out value);
                        if (value == null)
                        {
                            num2 = (y - 8) / 64;
                            location.Objects.TryGetValue(new Vector2(num, num2), out value);
                        }
                    }

                    x = num * 64;
                    y = num2 * 64;
                    if (location.terrainFeatures.TryGetValue(vector, out var value2) && value2.performToolAction(tool, 0, vector))
                    {
                        location.terrainFeatures.Remove(vector);
                    }
                }

                vector = new Vector2(num, num2);
                if (value != null)
                {
                    if (value.IsBreakableStone())
                    {
                        location.playSound("hammer", vector);
                        if (value.MinutesUntilReady > 0)
                        {
                            int num3 = Math.Max(1, tool.UpgradeLevel + 1) + tool.additionalPower.Value;
                            value.minutesUntilReady.Value -= num3;
                            value.shakeTimer = 200;
                            if (value.MinutesUntilReady > 0)
                            {
                                Game1.createRadialDebris(Game1.currentLocation, 14, num, num2, Game1.random.Next(2, 5), resource: false);
                                return;
                            }
                        }

                        TemporaryAnimatedSprite temporaryAnimatedSprite = ((ItemRegistry.GetDataOrErrorItem(value.QualifiedItemId).TextureName == "Maps\\springobjects" && value.ParentSheetIndex < 200 && !Game1.objectData.ContainsKey((value.ParentSheetIndex + 1).ToString()) && value.QualifiedItemId != "(O)25") ? new TemporaryAnimatedSprite(value.ParentSheetIndex + 1, 300f, 1, 2, new Vector2(x - x % 64, y - y % 64), flicker: true, value.flipped)
                        {
                            alphaFade = 0.01f
                        } : new TemporaryAnimatedSprite(47, new Vector2(num * 64, num2 * 64), Color.Gray, 10, flipped: false, 80f));
                        Game1.Multiplayer.broadcastSprites(location, temporaryAnimatedSprite);
                        Game1.createRadialDebris(location, 14, num, num2, Game1.random.Next(2, 5), resource: false);
                        Game1.Multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(46, new Vector2(num * 64, num2 * 64), Color.White, 10, flipped: false, 80f)
                        {
                            motion = new Vector2(0f, -0.6f),
                            acceleration = new Vector2(0f, 0.002f),
                            alphaFade = 0.015f
                        });
                        location.OnStoneDestroyed(value.ItemId, num, num2, who);
                        if (who != null && who.stats.Get("Book_Diamonds") != 0 && Game1.random.NextDouble() < 0.0066)
                        {
                            Game1.createObjectDebris("(O)72", num, num2, who.UniqueMultiplayerID, location);
                            if (who.professions.Contains(19) && Game1.random.NextBool())
                            {
                                Game1.createObjectDebris("(O)72", num, num2, who.UniqueMultiplayerID, location);
                            }
                        }

                        if (value.MinutesUntilReady <= 0)
                        {
                            value.performRemoveAction();
                            location.Objects.Remove(new Vector2(num, num2));
                            location.playSound("stoneCrack", vector);
                            Game1.stats.RocksCrushed++;
                        }
                    }
                    else if (value.Name.Contains("Boulder"))
                    {
                        location.playSound("hammer", vector);
                        if (tool.UpgradeLevel < 2)
                        {
                            Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\StringsFromCSFiles:Pickaxe.cs.14194")));
                            return;
                        }

                        if (num == boulderTileX && num2 == boulderTileY)
                        {
                            hitsToBoulder += power + 1;
                            value.shakeTimer = 190;
                        }
                        else
                        {
                            hitsToBoulder = 0;
                            boulderTileX = num;
                            boulderTileY = num2;
                        }

                        if (hitsToBoulder >= 4)
                        {
                            location.removeObject(vector, showDestroyedObject: false);
                            Game1.Multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(5, new Vector2(64f * vector.X - 32f, 64f * (vector.Y - 1f)), Color.Gray, 8, Game1.random.NextBool(), 50f)
                            {
                                delayBeforeAnimationStart = 0
                            });
                            Game1.Multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(5, new Vector2(64f * vector.X + 32f, 64f * (vector.Y - 1f)), Color.Gray, 8, Game1.random.NextBool(), 50f)
                            {
                                delayBeforeAnimationStart = 200
                            });
                            Game1.Multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(5, new Vector2(64f * vector.X, 64f * (vector.Y - 1f) - 32f), Color.Gray, 8, Game1.random.NextBool(), 50f)
                            {
                                delayBeforeAnimationStart = 400
                            });
                            Game1.Multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(5, new Vector2(64f * vector.X, 64f * vector.Y - 32f), Color.Gray, 8, Game1.random.NextBool(), 50f)
                            {
                                delayBeforeAnimationStart = 600
                            });
                            Game1.Multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(25, new Vector2(64f * vector.X, 64f * vector.Y), Color.White, 8, Game1.random.NextBool(), 50f, 0, -1, -1f, 128));
                            Game1.Multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(25, new Vector2(64f * vector.X + 32f, 64f * vector.Y), Color.White, 8, Game1.random.NextBool(), 50f, 0, -1, -1f, 128)
                            {
                                delayBeforeAnimationStart = 250
                            });
                            Game1.Multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(25, new Vector2(64f * vector.X - 32f, 64f * vector.Y), Color.White, 8, Game1.random.NextBool(), 50f, 0, -1, -1f, 128)
                            {
                                delayBeforeAnimationStart = 500
                            });
                            location.playSound("boulderBreak", vector);
                        }
                    }
                    else if (value.performToolAction(tool))
                    {
                        value.performRemoveAction();
                        if (value.Type == "Crafting" && (int)value.fragility != 2)
                        {
                            Game1.currentLocation.debris.Add(new Debris(value.QualifiedItemId, who.GetToolLocation(), Utility.PointToVector2(who.StandingPixel)));
                        }

                        Game1.currentLocation.Objects.Remove(vector);
                    }
                }
                else
                {
                    location.playSound("woodyHit", vector);
                    if (location.doesTileHaveProperty(num, num2, "Diggable", "Back") != null)
                    {
                        Game1.Multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(12, new Vector2(num * 64, num2 * 64), Color.White, 8, flipped: false, 80f)
                        {
                            alphaFade = 0.015f
                        });
                    }
                }
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
