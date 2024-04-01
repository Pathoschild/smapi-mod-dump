/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley/personal-indoor-farm
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;

namespace PersonalIndoorFarm
{
    public class ModEntry : Mod
    {
        public static Mod Mod;
        //public static ModConfig Config;
        public static IMonitor _Monitor;
        public static IModHelper _Helper;

        public override void Entry(IModHelper helper)
        {
            Mod = this;
            _Monitor = Monitor;
            _Helper = Helper;

            Lib.Main.Initialize();

            Helper.ConsoleCommands.Add("pif", "", commands);
            //Helper.Events.GameLoop.
        }

        private void commands(string command, string[] args)
        {
            var location = Game1.currentLocation;

            if (System.Diagnostics.Debugger.IsAttached)
                System.Diagnostics.Debugger.Break();

            location.terrainFeatures.TryGetValue(new Microsoft.Xna.Framework.Vector2(6, 6), out var dirt);
            var hoedirt = dirt as HoeDirt;
        }
    }
}
