/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/greyivy/OrnithologistsGuild
**
*************************************************/

using System;
using System.Linq;
using Microsoft.Xna.Framework;
using OrnithologistsGuild.Content;
using OrnithologistsGuild.Game;
using OrnithologistsGuild.Game.Critters;
using OrnithologistsGuild.Game.Items;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace OrnithologistsGuild
{
    public partial class ModEntry : Mod
    {
        public static BirdieDef debug_AlwaysSpawn = null;
        public static PerchType? debug_PerchType = null;
        private static bool debug_EnableBirdWhisperer = false;
        public static Vector2? debug_BirdWhisperer = null;
        private static bool debug_CanSpawnAtOrRelocateTo = false;

        private void RegisterDebugCommands()
        {
            Helper.ConsoleCommands.Add("ogd", "Adds debug items to inventory", OnDebugCommand);
            Helper.ConsoleCommands.Add("ogs", "Consistently spawns specified birdie ID", OnDebugCommand);
            Helper.ConsoleCommands.Add("ogp", "Forces birdies to perch on specified perch type", OnDebugCommand);
            Helper.ConsoleCommands.Add("ogw", "Bird Whisperer: ask a random bird (nicely) to relocate to wherever you click", OnDebugCommand);
            Helper.ConsoleCommands.Add("ogcsr", "Debugs `BetterBirdie.CanSpawnAtOrRelocateTo()`", OnDebugCommand);
        }

        private void OnDebugCommand(string cmd, string[] args)
        {
            if (cmd.Equals("ogd"))
            {
                if (args.Length > 0)
                {
                    if (args[0].Equals("food", StringComparison.OrdinalIgnoreCase))
                    {
                        Game1.player.addItemByMenuIfNecessary((Item)new StardewValley.Object(270, 32)); // Corn
                        Game1.player.addItemByMenuIfNecessary((Item)new StardewValley.Object(770, 32)); // Mixed Seeds
                        Game1.player.addItemByMenuIfNecessary((Item)new StardewValley.Object(431, 32)); // Sunflower Seeds
                        Game1.player.addItemByMenuIfNecessary((Item)new StardewValley.Object(832, 32)); // Pineapple
                        Game1.player.addItemByMenuIfNecessary((Item)DGAContentPack.Find("SeedHuller").ToItem());
                        return;
                    }
                    else if (args[0].Equals("feeder", StringComparison.OrdinalIgnoreCase))
                    {
                        Game1.player.addItemByMenuIfNecessary((Item)DGAContentPack.Find("WoodenHopper").ToItem());
                        Game1.player.addItemByMenuIfNecessary((Item)DGAContentPack.Find("WoodenPlatform").ToItem());
                        Game1.player.addItemByMenuIfNecessary((Item)DGAContentPack.Find("PlasticTube").ToItem());
                        return;
                    }
                    else if (args[0].Equals("bath", StringComparison.OrdinalIgnoreCase))
                    {
                        Game1.player.addItemByMenuIfNecessary((Item)DGAContentPack.Find("StoneBath").ToItem());
                        Game1.player.addItemByMenuIfNecessary((Item)DGAContentPack.Find("HeatedStoneBath").ToItem());
                        return;
                    }
                    else if (args[0].Equals("binoculars", StringComparison.OrdinalIgnoreCase))
                    {
                        Game1.player.addItemByMenuIfNecessary((Item)new LifeList());
                        Game1.player.addItemByMenuIfNecessary((Item)new JojaBinoculars());
                        Game1.player.addItemByMenuIfNecessary((Item)new AntiqueBinoculars());
                        Game1.player.addItemByMenuIfNecessary((Item)new ProBinoculars());
                        return;
                    }
                }

                Monitor.Log("`ogd`: must specify either `food`, `feeder`, `bath`, or `binoculars`", LogLevel.Warn);
            }
            else if (cmd.Equals("ogs"))
            {
                BirdieDef birdieDef = ContentPackManager.BirdieDefs.Values.FirstOrDefault(birdieDef => birdieDef.ID.Equals(args.Length == 0 ? "HouseSparrow" : args[0], StringComparison.OrdinalIgnoreCase));
                if (birdieDef != null)
                {
                    Monitor.Log($"`ogs` enabled: only spawning {birdieDef.ID}", LogLevel.Info);
                } else
                {
                    Monitor.Log($"`ogs` disabled (birdie \"{args[0]}\" not found)", LogLevel.Warn);
                }

                debug_AlwaysSpawn = birdieDef;
            }
            else if (cmd.Equals("ogp"))
            {
                if (args.Length > 0)
                {
                    debug_PerchType = (PerchType)Enum.Parse(typeof(PerchType), args[0]);
                    Monitor.Log($"`ogp` enabled: only relocating to {debug_PerchType} perches", LogLevel.Info);
                } else
                {
                    debug_PerchType = null;
                    Monitor.Log($"`ogp` disabled", LogLevel.Warn);
                }
            }
            else if (cmd.Equals("ogw"))
            {
                debug_EnableBirdWhisperer = !debug_EnableBirdWhisperer;
                if (debug_EnableBirdWhisperer) Monitor.Log($"`ogw` enabled: click a tile", LogLevel.Info);
                else Monitor.Log($"`ogw` disabled", LogLevel.Warn);
            }
            else if (cmd.Equals("ogcsr"))
            {
                debug_CanSpawnAtOrRelocateTo = !debug_CanSpawnAtOrRelocateTo;
                if (debug_CanSpawnAtOrRelocateTo) Monitor.Log($"`ogcsr` enabled: click a tile", LogLevel.Info);
                else Monitor.Log($"`ogcsr` disabled", LogLevel.Warn);
            }
        }

        private void DebugHandleInput(ButtonPressedEventArgs e)
        {
            if (!e.Button.IsUseToolButton()) return;

            if (debug_EnableBirdWhisperer)
            {
                debug_BirdWhisperer = e.Cursor.Tile * Game1.tileSize;
                Monitor.Log($"Bird whisperer: {debug_BirdWhisperer.ToString()}");

                var birdie = Utilities.Randomize(Game1.player.currentLocation.critters.Where(c => c is BetterBirdie)).FirstOrDefault();
                if (birdie != null)
                {
                    ((BetterBirdie)birdie).Frighten();
                }
            }

            if (debug_CanSpawnAtOrRelocateTo)
            {
                if (debug_AlwaysSpawn == null)
                {
                    Monitor.Log("Must call `ogs` first", LogLevel.Error);
                }
                else
                {
                    Monitor.Log($"{debug_AlwaysSpawn.ID} CanSpawnAtOrRelocateTo {e.Cursor.Tile} = {BetterBirdie.CanSpawnAtOrRelocateTo(Game1.player.currentLocation, e.Cursor.Tile, debug_AlwaysSpawn)}", LogLevel.Info);
                }
            }
        }
    }
}

