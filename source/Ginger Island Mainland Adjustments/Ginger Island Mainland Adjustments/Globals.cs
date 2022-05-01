/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/Ginger-Island-Mainland-Adjustments
**
*************************************************/

using System.Reflection;
using AtraShared.Schedules;
using GingerIslandMainlandAdjustments.Configuration;
using GingerIslandMainlandAdjustments.CustomConsoleCommands;

namespace GingerIslandMainlandAdjustments;

/// <summary>
/// Class to handle global variables.
/// </summary>
internal static class Globals
{
    private const string SaveDataKey = "GIMA_SAVE_KEY";

    // Values are set in the Mod.Entry method, so should never be null.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    /// <summary>
    /// Gets this mod's manifest.
    /// </summary>
    internal static IManifest Manifest { get; private set; }

    /// <summary>
    /// Gets SMAPI's logging service.
    /// </summary>
    internal static IMonitor ModMonitor { get; private set; }

    /// <summary>
    /// Gets or sets mod configuration class.
    /// </summary>
    internal static ModConfig Config { get; set; }

    /// <summary>
    /// Gets SMAPI's reflection helper.
    /// </summary>
    internal static IReflectionHelper ReflectionHelper { get; private set; }

    /// <summary>
    /// Gets SMAPI's Content helper.
    /// </summary>
    internal static IContentHelper ContentHelper { get; private set; }

    /// <summary>
    /// Gets SMAPI's mod registry helper.
    /// </summary>
    internal static IModRegistry ModRegistry { get; private set; }

    /// <summary>
    /// Gets SMAPI's helper class.
    /// </summary>
    internal static IModHelper Helper { get; private set; }

    /// <summary>
    /// Gets the input helper.
    /// </summary>
    internal static IInputHelper InputHelper { get; private set; }

    /// <summary>
    /// Gets the instance of the schedule utility functions.
    /// </summary>
    internal static ScheduleUtilityFunctions UtilitySchedulingFunctions { get; private set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    /// <summary>
    /// Gets the instance of the class used for custom save data for this mod.
    /// </summary>
    /// <remarks>Only accessible by the MasterPlayer.</remarks>
    internal static SaveDataModel? SaveDataModel { get; private set; }

    /// <summary>
    /// Gets the github location for this mod.
    /// </summary>
    /// <remarks>Used to direct bug reports.</remarks>
    internal static string GithubLocation { get; } = "https://github.com/atravita-mods/Ginger-Island-Mainland-Adjustments/issues";

    /// <summary>
    /// Gets a reference to  of Child2NPC's ModEntry.IsChildNPC.
    /// </summary>
    /// <remarks>Null if C2NPC is not installed or method not found.</remarks>
    internal static Func<NPC, bool>? IsChildToNPC { get; private set; }

    /// <summary>
    /// Initialize globals, including reading config file.
    /// </summary>
    /// <param name="helper">SMAPI's IModHelper.</param>
    /// <param name="monitor">SMAPI's logging service.</param>
    internal static void Initialize(IModHelper helper, IMonitor monitor, IManifest manifest)
    {
        Globals.ModMonitor = monitor;
        Globals.ReflectionHelper = helper.Reflection;
        Globals.InputHelper = helper.Input;
        Globals.ContentHelper = helper.Content;
        Globals.ModRegistry = helper.ModRegistry;
        Globals.Helper = helper;
        Globals.Manifest = manifest;

        try
        {
            Globals.Config = helper.ReadConfig<ModConfig>();
        }
        catch
        {
            Globals.ModMonitor.Log(I18n.IllFormatedConfig(), LogLevel.Warn);
            Globals.Config = new();
        }

        UtilitySchedulingFunctions = new(
            monitor: Globals.ModMonitor,
            translation: Globals.Helper.Translation);
    }

    /// <summary>
    /// Tries to get a handle on Child2NPC's IsChildNPC.
    /// </summary>
    /// <returns>True if successful, false otherwise.</returns>
    internal static bool GetIsChildToNPC()
    {
        if (ModRegistry.Get("Loe2run.ChildToNPC") is null)
        {
            ModMonitor.Log($"Child2NPC not installed - no need to adjust for that.", LogLevel.Trace);
            return false;
        }
        if (Type.GetType("ChildToNPC.ModEntry, ChildToNPC")?.GetMethod("IsChildNPC", new Type[] { typeof(Character) }) is MethodInfo childToNPCMethod)
        {
            IsChildToNPC = childToNPCMethod.CreateDelegate<Func<NPC, bool>>();
            return true;
        }
        ModMonitor.Log("IsChildNPC method not found - integration with Child2NPC failed.", LogLevel.Warn);
        return false;
    }

    /// <summary>
    /// Loads data out of save.
    /// </summary>
    internal static void LoadDataFromSave()
    {
        if (Context.IsWorldReady && Context.IsMainPlayer)
        {
            SaveDataModel = Helper.Data.ReadGlobalData<SaveDataModel>(SaveDataKey) ?? new();
        }
    }

    /// <summary>
    /// Saves custom data.
    /// </summary>
    internal static void SaveCustomData()
    {
        if (Context.IsWorldReady && Context.IsMainPlayer)
        {
            Helper.Data.WriteSaveData(SaveDataKey, SaveDataModel);
        }
    }
}
