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
        public static Mod ModInstance;
        public static ModConfig Config;

        public static new IMonitor Monitor;
        public static new IModHelper Helper;
        public static new IManifest ModManifest;

        public override void Entry(IModHelper helper)
        {
            ModInstance = this;
            Config = helper.ReadConfig<ModConfig>();

            Monitor = ModInstance.Monitor;
            Helper = ModInstance.Helper;
            ModManifest = ModInstance.ModManifest;

            API.Main.Initialize();
            Lib.Main.Initialize();

            Helper.ConsoleCommands.Add("pif", "", commands);
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
