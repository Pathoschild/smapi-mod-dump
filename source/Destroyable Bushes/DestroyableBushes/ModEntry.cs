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
    public partial class ModEntry : Mod
    {
        public static Mod Instance { get; set; } = null;
        public static ModConfig Config { get; set; } = null;
        public static ModData Data { get; set; } = null;

        public override void Entry(IModHelper helper)
        {
            Instance = this; //provide a global reference to this mod's utilities

            Config = Helper.ReadConfig<ModConfig>(); //attempt to load (or create) config.json

            //enable SMAPI events
            Helper.Events.GameLoop.GameLaunched += EnableGMCM; //try to enable Generic Mod Config Menu when the game has launched
            Helper.Events.GameLoop.SaveLoaded += LoadModData;
            Helper.Events.GameLoop.Saving += SaveModData;
            Helper.Events.GameLoop.DayStarted += RegrowBushes;
            Helper.Events.Multiplayer.ModMessageReceived += ReceiveDestroyedBushMessage;

            //apply Harmony patches to SDV code
            ApplyHarmonyPatches();

            //enable SMAPI console commands
            Helper.ConsoleCommands.Add
            (
                name: "add_bush",
                documentation: "Creates a bush of the specified size.\n" +
                               "Bushes added by this command will regrow if that setting is enabled.\n" +
                               "\n" +
                               "Usage: add_bush <size> [x y] [location]\n" +
                               "- size: The bush's size, as a name or number. 0 = \"small\", 1 = \"medium\", 2 = \"large\", 3 = \"tea\".\n" +
                               "- x y (optional): The bush's tile coordinates. If not provided, the bush will appear in front of the player.\n" +
                               "- location (optional): The name of the bush's map, e.g. \"Farm\". If not provided, the player's current map will be used.\n" +
                               "\n" +
                               "Examples:\n" +
                               "  add_bush 2\n" +
                               "  add_bush large\n" +
                               "  add_bush 2 64 19\n" +
                               "  add_bush 2 64 19 farm",
                callback: Commands.AddBush
            );

            Helper.ConsoleCommands.Add
            (
                name: "remove_bush",
                documentation: "Removes a bush from the specified location.\n" +
                               "Bushes removed by this command will NOT regrow.\n" +
                               "\n" +
                               "Usage: remove_bush [x y] [location]\n" +
                               "- x y (optional): The bush's tile coordinates. If not provided, a bush will be removed on, or in front of, the player.\n" +
                               "- location (optional): The name of the bush's map, e.g. \"Farm\". If not provided, the player's current map will be used.\n" +
                               "\n" +
                               "Examples:\n" +
                               "  remove_bush\n" +
                               "  remove_bush 64 19\n" +
                               "  remove_bush 64 19 farm",
                callback: Commands.RemoveBush
            );
        }

        /// <summary>Applies any Harmony patches used by this mod.</summary>
        private void ApplyHarmonyPatches()
        {
            HarmonyInstance harmony = HarmonyInstance.Create(ModManifest.UniqueID); //create this mod's Harmony instance

            //apply all patches
            HarmonyPatch_BushesAreDestroyable.ApplyPatch(harmony);
            HarmonyPatch_DestroyedBushBehavior.ApplyPatch(harmony);
        }
    }
}
