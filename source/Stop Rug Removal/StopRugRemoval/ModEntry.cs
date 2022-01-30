/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StopRugRemoval
**
*************************************************/

using System.Reflection;
using System.Text;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley.Objects;

namespace StopRugRemoval;

/// <summary>
/// Entry class to the mod.
/// </summary>
public class ModEntry : Mod
{
    // the following two fields are set in the entry method, which is approximately as close as I can get to the constructor anyways.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    /// <summary>
    /// Instance that holds the logger for this file.
    /// </summary>
    [SuppressMessage("ReSharper", "CA2211", Justification = "This is needed to give harmony patches access to the logger. Also, multithreading is not used anyways.")]
    [SuppressMessage("StyleCop", "SA1401", Justification = "The logger, so unlikely to change in the future")]
    public static IMonitor ModMonitor;

    /// <summary>
    /// Instance that holds the configuration for this mod.
    /// </summary>
    [SuppressMessage("ReSharper", "CA2211", Justification = "This is needed to give harmony patches access to the configuration. Also, multithreading is not used anyways.")]
    [SuppressMessage("StyleCop", "SA1401", Justification = "The config file, unlikely to change in the future")]
    public static ModConfig Config;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    /// <inheritdoc/>
    public override void Entry(IModHelper helper)
    {
        try
        {
            Config = this.Helper.ReadConfig<ModConfig>();
        }
        catch
        {
            this.Monitor.Log(I18n.IllFormatedConfig(), LogLevel.Warn);
            Config = new();
        }

        ModMonitor = this.Monitor;
        I18n.Init(helper.Translation);

        this.ApplyPatches(new Harmony(this.ModManifest.UniqueID));

        helper.Events.GameLoop.GameLaunched += this.SetUpConfig;
        //helper.Events.GameLoop.Saving += this.BeforeSave;
        //saved as well?
    }

    /// <summary>
    /// Applies and logs this mod's harmony patches.
    /// </summary>
    /// <param name="harmony">My harmony instance.</param>
    private void ApplyPatches(Harmony harmony)
    {
        // handle patches from annotations.
        harmony.PatchAll();
        foreach (MethodBase? method in harmony.GetPatchedMethods())
        {
            if (method is null)
            {
                continue;
            }
            Patches patches = Harmony.GetPatchInfo(method);

            StringBuilder sb = new();
            sb.Append("Patched method ").Append(method.GetFullName());
            foreach (Patch patch in patches.Prefixes.Where((Patch p) => p.owner.Equals(this.ModManifest.UniqueID)))
            {
                sb.AppendLine().Append("\tPrefixed with method: ").Append(patch.PatchMethod.GetFullName());
            }
            foreach (Patch patch in patches.Postfixes.Where((Patch p) => p.owner.Equals(this.ModManifest.UniqueID)))
            {
                sb.AppendLine().Append("\tPostfixed with method: ").Append(patch.PatchMethod.GetFullName());
            }
            foreach (Patch patch in patches.Transpilers.Where((Patch p) => p.owner.Equals(this.ModManifest.UniqueID)))
            {
                sb.AppendLine().Append("\tTranspiled with method: ").Append(patch.PatchMethod.GetFullName());
            }
            foreach (Patch patch in patches.Finalizers.Where((Patch p) => p.owner.Equals(this.ModManifest.UniqueID)))
            {
                sb.AppendLine().Append("\tFinalized with method: ").Append(patch.PatchMethod.GetFullName());
            }
            ModMonitor.Log(sb.ToString(), LogLevel.Trace);
        }
    }

    /// <summary>
    /// Clear all NoSpawn tiles before saving.
    /// </summary>
    /// <param name="sender">From SMAPI.</param>
    /// <param name="e">Saving Event arguments...</param>
    /// <exception cref="NotImplementedException">Haven't finished writing this yet.</exception>
    private void BeforeSave(object? sender, SavingEventArgs e) => throw new NotImplementedException();

    private void SetUpConfig(object? sender, GameLaunchedEventArgs e)
    {
        IModInfo gmcm = this.Helper.ModRegistry.Get("spacechase0.GenericModConfigMenu");
        if (gmcm is null)
        {
            this.Monitor.Log(I18n.GmcmNotFound(), LogLevel.Debug);
            return;
        }
        if (gmcm.Manifest.Version.IsOlderThan("1.6.0"))
        {
            this.Monitor.Log(I18n.GmcmVersionMessage(version: "1.6.0", currentversion: gmcm.Manifest.Version), LogLevel.Info);
            return;
        }

        var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
        if (configMenu is null)
        {
            return;
        }

        configMenu.Register(
            mod: this.ModManifest,
            reset: () => Config = new ModConfig(),
            save: () => this.Helper.WriteConfig(Config)
            );

        configMenu.AddParagraph(
            mod: this.ModManifest,
            text: I18n.Mod_Description
            );

        configMenu.AddBoolOption(
            mod: this.ModManifest,
            getValue: () => Config.Enabled,
            setValue: value => Config.Enabled = value,
            name: I18n.Enabled_Title
            );
    }
}
