/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sophiesalacia/StardewMods
**
*************************************************/

using ContentPatcher;
using SolidFoundations.Framework.Interfaces.Internal;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace CarWarp;

internal class Globals
{
    public static IManifest Manifest { get; set; }
    public static ModConfig Config { get; set; }
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
    public static string WarpLocationsContentPath => "sophie.CarWarp.Locations";

    public static IApi? SolidFoundationsApi;
    public static IContentPatcherAPI? ContentPatcherApi;
    public static IGenericModConfigMenuApi? GMCMApi;

    internal static void InitializeConfig()
    {
        Config = Helper.ReadConfig<ModConfig>();
    }

    internal static bool InitializeSFApi()
    {
        SolidFoundationsApi = Helper.ModRegistry.GetApi<IApi>("PeacefulEnd.SolidFoundations");

        return SolidFoundationsApi is not null;
    }

    internal static bool InitializeCPApi()
    {
        ContentPatcherApi = Helper.ModRegistry.GetApi<IContentPatcherAPI>("Pathoschild.ContentPatcher");

        return ContentPatcherApi is not null;
    }

    internal static bool InitializeGMCMApi()
    {
        GMCMApi = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");

        return GMCMApi is not null;
    }

    internal static void InitializeGlobals(ModEntry modEntry)
    {
        Manifest = modEntry.ModManifest;
        Helper = modEntry.Helper;
        Log.Monitor = modEntry.Monitor;
    }
}
