/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/cl4r3/Halloween-Mod-Jam-2023
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using StardewModdingAPI;

namespace TricksAndTreats
{
    internal static class Globals
    {
        internal static IMonitor Monitor { get; set; }
        internal static IModHelper Helper { get; set; }
        internal static IManifest Manifest { get; set; }
        internal static IJsonAssetsApi JA;
        internal static IGenericModConfigMenuApi GMCM;
        internal static IContentPatcherApi CP;
        internal static ModConfig Config;
        internal static ConfigMenu ConfigMenu;

        internal static readonly string AssetPath = "Mods/TricksAndTreats";
        internal static readonly string JAPath = Path.Combine("assets", "JsonAssets");
        internal static readonly string NPCsExt = ".NPCs";
        internal static readonly string CostumesExt = ".Costumes";
        internal static readonly string TreatsExt = ".Treats";

        internal const string HouseFlag = "TaT.LargeScaleTrick";
        internal const string HouseCT = "house_pranked";
        internal const string CostumeCT = "costume_react-";
        internal const string TreatCT = "give_candy";

        internal const string ModPrefix = "TaT.";
        internal const string PaintKey = ModPrefix + "previous-skin";
        internal const string StolenKey = ModPrefix + "stolen-items";
        internal const string ScoreKey = ModPrefix + "treat-score";
        internal const string CostumeKey = ModPrefix + "costume-set";
        internal const string ChestKey = ModPrefix + "reached-chest";
        internal const string MysteryKey = ModPrefix + "original-treat";
        //internal const string CobwebKey = ModPrefix + "cobwebbed";

        public static string[] ValidRoles = { "candygiver", "candytaker", "trickster", "observer", };
        //public static string[] ValidFlavors = { "sweet", "sour", "salty", "hot", "gourmet", "candy", "healthy", "joja", "fatty", };

        internal static Dictionary<string, int> ClothingInfo;
        internal static Dictionary<string, int> FoodInfo;
        internal static Dictionary<string, int> HatInfo;

        internal static Dictionary<string, Celebrant> NPCData;
        internal static Dictionary<string, Costume> CostumeData;
        internal static Dictionary<string, Treat> TreatData;

        internal static void Initialize(IMod mod, IModHelper helper)
        {
            Monitor = mod.Monitor;
            Helper = helper;
            Manifest = mod.ModManifest;

            ConfigMenu = new ConfigMenu(mod);
            ConsoleCommands.Register(mod);
        }
    }
}