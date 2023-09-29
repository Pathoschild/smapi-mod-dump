/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sophiesalacia/StardewMods
**
*************************************************/

using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace Umbrellas;

internal class Globals
{
    public static ModConfig Config { get; set; }
    public static IManifest Manifest { get; set; }
    public static IModHelper Helper { get; set; }
    public static ICommandHelper CCHelper => Helper.ConsoleCommands;
    public static IGameContentHelper GameContent => Helper.GameContent;
    public static IModContentHelper ModContent => Helper.ModContent;
    public static IContentPackHelper ContentPackHelper => Helper.ContentPacks;
    public static IDataHelper DataHelper => Helper.Data;
    public static IInputHelper InputHelper => Helper.Input;
    public static IModEvents EventHelper => Helper.Events;
    public static IMultiplayerHelper MultiplayerHelper => Helper.Multiplayer;
    public static IReflectionHelper ReflectionHelper => Helper.Reflection;
    public static ITranslationHelper TranslationHelper => Helper.Translation;
    public static string UUID => Manifest.UniqueID;

    public static IGenericModConfigMenuApi GmcmApi;

    public static bool UmbrellaNeeded;

    public static string DataPath = "sophie.Umbrellas/NPCData";
    public static string InclusionsPath = "sophie.Umbrellas/Inclusions";

    internal static void InitializeConfig()
    {
        Config = Helper.ReadConfig<ModConfig>();
    }

    internal static void InitializeGlobals(ModEntry modEntry)
    {
        Log.Monitor = modEntry.Monitor;
        Manifest = modEntry.ModManifest;
        Helper = modEntry.Helper;
    }

    internal static bool InitializeGmcmApi()
    {
        GmcmApi = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");

        return GmcmApi is not null;
    }
}
