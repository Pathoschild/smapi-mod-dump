/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley/custom-farm-loader
**
*************************************************/

using System;
using StardewValley;
using StardewValley.Menus;
using StardewModdingAPI;
using HarmonyLib;
using Custom_Farm_Loader.Menus;
using System.Collections.Generic;
using Custom_Farm_Loader.API;
using Custom_Farm_Loader.Lib;
using Custom_Farm_Loader.Lib.Enums;
using Custom_Farm_Loader.GameLoopInjections;
using StardewValley.Locations;
using Microsoft.Xna.Framework;

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
            GenericModConfigMenuHandler.Initialize(this);

            helper.ConsoleCommands.Add("cfl", "CFL Commands", commands);
            helper.ConsoleCommands.Add("cfl_debug", "Debug Breakpoint", this.debug);
        }

        private void commands(string command, string[] args)
        {
            if (args.Length == 0) {
                printValidCommands();
                return;
            }

            switch (args[0].ToLower()) {
                case "help":
                    printHelp(); break;

                case "dayupdate" or "du":
                    dayupdateCommand(args.Length > 1 ? int.Parse(args[1]) : 1); break;

                case "furniture":
                    printFurniture(); break;

                case "reload":
                    reloadCFL(); break;

                case "type" or "farmtype":
                    Monitor.Log(Game1.GetFarmTypeID(), LogLevel.Info); break;

                case "fixstarterquest":
                    Game1.player.addQuest((Game1.whichModFarm?.Id == "MeadowlandsFarm") ? "132" : "6");
                    Game1.dayTimeMoneyBox.PingQuestLog();
                    break;

                default:
                    Monitor.Log("Unknown Command: " + args[0], LogLevel.Error); break;
            }
        }

        private void printHelp() =>
            Monitor.Log(
                "Valid Commands:\n" +
                "DAYUPDATE NUM      Performs all valid daily updates of the players location NUM times\n" +
                "FURNITURE          Prints out all furniture of the current location as json so it can be directly copied into StartFurniture\n" +
                "RELOAD             Reloads all cached cfl_map.json data\n" +
                "TYPE               Prints the current farm type"
                    , LogLevel.Info);


        private void printValidCommands() =>
            Monitor.Log("Valid Commands: help, dayupdate, furniture, reload, type", LogLevel.Info);

        private void reloadCFL()
        {
            CustomFarm.clearCache();
            Monitor.Log("Done!", LogLevel.Info);
        }

        private void dayupdateCommand(int times)
        {
            if (!CustomFarm.IsCFLMapSelected()) {
                Monitor.Log("No CFL map selected", LogLevel.Error);
                return;
            }

            var customFarm = CustomFarm.getCurrentCustomFarm();
            while (times-- > 0)
                DailyUpdateEvents.update(customFarm.DailyUpdates.FindAll(el => el.Area.LocationName == Game1.currentLocation.Name));
        }

        private void debug(string command, string[] args)
        {
            if (System.Diagnostics.Debugger.IsAttached)
                System.Diagnostics.Debugger.Break();
        }

        private void printFurniture()
        {
            if (Game1.currentLocation is null) {
                Monitor.Log("No GameLocation loaded", LogLevel.Error);
                return;
            }

            var player = Game1.player;
            var loc = player.currentLocation;
            string res = Environment.NewLine + "\"StartFurniture\": [" + Environment.NewLine;
            string map = $", \"Map\": \"{loc.Name}\"";
            if (loc is FarmHouse fh) {
                if (fh.wallpaperIDs.Contains("Bedroom"))
                    res += "\t" + $@"{{ ""Type"": ""Wallpaper"", ""ID"":""{fh.appliedWallpaper["Bedroom"]}"" }}," + Environment.NewLine;
                if (fh.floorIDs.Contains("Bedroom"))
                    res += "\t" + $@"{{ ""Type"": ""Floor"", ""ID"":""{fh.appliedFloor["Bedroom"]}"" }}," + Environment.NewLine;

                map = "";
            }

            foreach (var furniture in loc.furniture) {
                if (furniture.ParentSheetIndex == 2048 && furniture.TileLocation == new Vector2(9, 8)) //Start Location of bed that gets placed anyway
                    continue;

                string heldObject = "";
                if (furniture.heldObject.Value is StardewValley.Object h) {
                    heldObject = ", \"HeldObject\": {" +
                        $" /*{h.Name}*/ \"ID\": \"{h.ItemId}\"" +
                        (h is not StardewValley.Objects.Furniture ? ", \"Type\": \"Item\"" : "") +
                        (h is StardewValley.Objects.Furniture f && f.currentRotation.Value > 0 ? $", \"Rotations\": {totalRotations(f)}" : "") +
                        (h.Quality > 0 ? ", \"Quality\": \"" + h.Quality switch { 1 => "Silver", 2 => "Gold", 4 => "Iridium", _ => "" } + "\"" : "") +
                        " }";
                }

                res += "\t" + $"{{ /*{furniture.Name}*/ \"ID\": \"{furniture.ItemId}\"" +
                       $", \"Position\": \"{furniture.TileLocation.X}, {furniture.TileLocation.Y}\"" +
                       map +
                       (furniture.currentRotation.Value > 0 ? $", \"Rotations\": {totalRotations(furniture)}" : "") +
                       heldObject +
                         " }," + Environment.NewLine;
            }
            res += "],";

            Monitor.Log(res, LogLevel.Info);
        }

        private int totalRotations(StardewValley.Objects.Furniture f)
        {
            //Furniture can have 0, 2 or 4 rotations
            //When it can only be rotated twice the currentRotation jumps between 0 and 2
            //CFL doesn't care about the technical "currentRotation" value, CFL rotates once per rotation
            if (f.currentRotation.Value == 0 || f.rotations.Value == 4)
                return f.currentRotation.Value;

            return 1;
        }
    }

}
