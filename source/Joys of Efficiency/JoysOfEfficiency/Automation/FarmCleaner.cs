using System;
using System.Collections.Generic;
using JoysOfEfficiency.Core;
using JoysOfEfficiency.Utils;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Tools;
using Object = StardewValley.Object;

namespace JoysOfEfficiency.Automation
{
    internal class FarmCleaner
    {
        private static Multiplayer Multiplayer => InstanceHolder.Multiplayer;
        private static Config Config => InstanceHolder.Config;
        private static IReflectionHelper Reflection => InstanceHolder.Reflection;

        private static readonly Logger Logger = new Logger("FarmCleaner");

        public static void OnEighthUpdate()
        {
            if (!(Game1.currentLocation is Farm farm))
            {
                return;
            }

            Tool tool = Game1.player.CurrentTool;
            if (tool == null)
            {
                return;
            }

            List<Object> objects = Util.GetObjectsWithin<Object>(Config.RadiusFarmCleanup);
            foreach (Object obj in objects)
            {
                Vector2 loc = Util.GetLocationOf(farm, obj);
                if (Config.CutWeeds && IsWeed(obj.Name) && tool is MeleeWeapon melee && melee.isScythe())
                {
                    CutWeeds(farm, obj, loc);
                    Logger.Log($"Cut weed @{loc}");
                }

                if (Config.BreakRocks && IsRock(obj.Name) && tool is Pickaxe pickaxe && BreakRock(farm, pickaxe, obj, loc))
                {
                    Logger.Log($"Broke rock @{loc}");
                }

                if (Config.ChopTwigs && IsTwig(obj.Name) && tool is Axe && ChopTwig(farm, obj, loc))
                {
                    Logger.Log($"Chopped twig @{loc}");
                }
            }
        }

        private static void CutWeeds(Farm farm, Object obj, Vector2 loc)
        {
            Reflection.GetMethod(obj, "cutWeed").Invoke(Game1.player, farm);
            farm.removeObject(loc, false);
        }

        private static bool ChopTwig(GameLocation farm, Object obj, Vector2 loc)
        {
            Farmer player = Game1.player;
            float stamina = 2 - player.ForagingLevel * 0.1f;
            if (player.Stamina < stamina)
            {
                return false;
            }

            player.Stamina -= stamina;

            obj.fragility.Value = 2;
            farm.playSound("axchop");
            farm.debris.Add(new Debris(new Object(388, 1), loc * 64f + new Vector2(32f, 32f)));
            Game1.createRadialDebris(farm, 12, (int)loc.X, (int)loc.Y, Game1.random.Next(4, 10), false);
            Multiplayer.broadcastSprites(farm, new TemporaryAnimatedSprite(12, new Vector2(loc.X * 64f, loc.Y * 64f), Color.White, 8, Game1.random.NextDouble() < 0.5, 50f));

            farm.removeObject(loc, false);
            return true;
        }

        private static bool BreakRock(GameLocation location, Tool pickaxe, Object @object, Vector2 loc)
        {
            Farmer player = Game1.player;
            float stamina = 2 - player.MiningLevel * 0.1f;
            if (player.Stamina < stamina)
            {
                return false;
            }

            player.Stamina -= stamina;

            int num1 = (int)loc.X;
            int num2 = (int) loc.Y;
            int x = num1 * 64;
            int y = num2 * 64;
            location.playSound("hammer");
            if (@object.minutesUntilReady > 0)
            {
                int num3 = Math.Max(1, pickaxe.upgradeLevel + 1);
                @object.minutesUntilReady.Value -= num3;
                @object.shakeTimer = 200;
                if (@object.minutesUntilReady > 0)
                {
                    Game1.createRadialDebris(Game1.currentLocation, 14, num1, num2, Game1.random.Next(2, 5), false);
                    return false;
                }
            }

            if (@object.ParentSheetIndex < 200 && !Game1.objectInformation.ContainsKey(@object.ParentSheetIndex + 1))
            {
                Multiplayer.broadcastSprites(location,
                    new TemporaryAnimatedSprite(@object.ParentSheetIndex + 1, 300f,
                        1, 2, new Vector2(x - x % 64, y - y % 64), true, @object.flipped)
                    {
                        alphaFade = 0.01f
                    });
            }
            else
            {
                Multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(47, new Vector2(num1 * 64, num2 * 64), Color.Gray, 10, false, 80f));
            }

            Game1.createRadialDebris(location, 14, num1, num2, Game1.random.Next(2, 5), false);
            Multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(46, new Vector2(num1 * 64, num2 * 64), Color.White, 10, false, 80f)
            {
                motion = new Vector2(0.0f, -0.6f),
                acceleration = new Vector2(0.0f, 1f / 500f),
                alphaFade = 0.015f
            });
            location.OnStoneDestroyed(@object.parentSheetIndex, num1, num2, Game1.player);
            if (@object.minutesUntilReady > 0)
                return false;
            location.Objects.Remove(new Vector2(num1, num2));
            location.playSound("stoneCrack");
            ++Game1.stats.RocksCrushed;
            return true;
        }

        private static bool IsWeed(string name)
        {
            return name == "Weeds";
        }

        private static bool IsRock(string name)
        {
            return name == "Stone";
        }

        private static bool IsTwig(string name)
        {
            return name == "Twig";
        }
    }
}
