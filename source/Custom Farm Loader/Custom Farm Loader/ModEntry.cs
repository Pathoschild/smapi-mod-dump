/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley-custom-farm-loader
**
*************************************************/

using System;
using StardewValley;
using StardewValley.Menus;
using StardewModdingAPI;
using HarmonyLib;
using Custom_Farm_Loader.Menus;
using System.Collections.Generic;

namespace Custom_Farm_Loader
{

    public class ModEntry : Mod
    {

        public static Mod Mod;
        public static ModConfig Config;
        public static IMonitor _Monitor;
        public static IModHelper _Helper;

        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<ModConfig>();

            Mod = this;
            _Monitor = Monitor;
            _Helper = Helper;

            Lib.Main.Initialize(this);
            GameLoopInjections.Main.Initialize(this);
            Menus.Main.Initialize(this);


            helper.ConsoleCommands.Add("cfl_debug", "Debug Breakpoint", this.debug);
            helper.ConsoleCommands.Add("cfl_furniture", "Debug Breakpoint", this.printFurniture);
        }

        private void debug(string command, string[] args)
        {
            if (System.Diagnostics.Debugger.IsAttached)
                System.Diagnostics.Debugger.Break();
        }

        private void printFurniture(string command, string[] args)
        {
            var player = Game1.player;
            var loc = player.currentLocation;
            string res = "\"StartFurniture\": [" + System.Environment.NewLine;
            if (loc.Name == "FarmHouse") {
                StardewValley.Locations.FarmHouse fh = (loc as StardewValley.Locations.FarmHouse);
                if (fh.wallpaperIDs.Contains("Bedroom"))
                    res += $@"{{ ""Type"": ""Wallpaper"", ""ID"":""{fh.appliedWallpaper["Bedroom"]}""," + Environment.NewLine;
                if (fh.floorIDs.Contains("Bedroom"))
                    res += $@"{{ ""Type"": ""Floor"", ""ID"":""{fh.appliedFloor["Bedroom"]}""," + Environment.NewLine;
            }

            foreach (var furniture in loc.furniture) {
                if (furniture.parentSheetIndex != 0)
                    res += $"{{ /*{furniture.Name}*/ \"ID\": {furniture.ParentSheetIndex}, " +
                           $"\"Position\": \"{furniture.TileLocation.X}, {furniture.TileLocation.Y}\", " +
                           (furniture.rotations > 1 ? $"\"Rotations\": {furniture.rotations - 1} " : "") +
                             "}," + Environment.NewLine;
                else
                    loc = loc;
            }
            res += "]";

            Monitor.Log(res, LogLevel.Info);
        }
    }

}
