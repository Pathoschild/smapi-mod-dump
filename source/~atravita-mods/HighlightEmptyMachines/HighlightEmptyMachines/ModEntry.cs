/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraShared.ConstantsAndEnums;
using AtraShared.Integrations;
using AtraShared.MigrationManager;
using AtraShared.Niceties;
using AtraShared.Utils.Extensions;
using HarmonyLib;
using HighlightEmptyMachines.Framework;
using StardewModdingAPI.Events;

using AtraUtils = AtraShared.Utils.Utils;

namespace HighlightEmptyMachines;

/// <inheritdoc />
internal sealed class ModEntry : Mod
{
    private MigrationManager? migrator;

    private GMCMHelper? gmcmHelper = null;

    /// <summary>
    /// Gets the logger for this mod.
    /// </summary>
    internal static IMonitor ModMonitor { get; private set; } = null!;

    /// <summary>
    /// Gets the config instance for this mod.
    /// </summary>
    internal static ModConfig Config { get; private set; } = null!;

    /// <summary>
    /// Gets the translation helper for this mod.
    /// </summary>
    internal static ITranslationHelper TranslationHelper { get; private set; } = null!;

    /// <inheritdoc />
    public override void Entry(IModHelper helper)
    {
        I18n.Init(helper.Translation);
        ModMonitor = this.Monitor;
        TranslationHelper = helper.Translation;

        Config = AtraUtils.GetConfigOrDefault<ModConfig>(helper, this.Monitor);

        helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
        helper.Events.GameLoop.ReturnedToTitle += this.OnReturnedToTitle;
    }

    /// <summary>
    /// Applies the patches for this mod.
    /// </summary>
    /// <param name="harmony">This mod's harmony instance.</param>
    /// <remarks>Delay until GameLaunched in order to patch other mods....</remarks>
    private void ApplyPatches(Harmony harmony)
    {
        try
        {
            harmony.PatchAll();
        }
        catch (Exception ex)
        {
            ModMonitor.Log(string.Format(ErrorMessageConsts.HARMONYCRASH, ex), LogLevel.Error);
        }
        harmony.Snitch(this.Monitor, harmony.Id, transpilersOnly: true);
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        this.ApplyPatches(new Harmony(this.ModManifest.UniqueID));

        PFMMachineHandler.TryGetAPI(this.Helper.ModRegistry);
        this.gmcmHelper = new(this.Monitor, this.Helper.Translation, this.Helper.ModRegistry, this.ModManifest);
        if (this.gmcmHelper.TryGetAPI())
        {
            this.gmcmHelper.TryGetOptionsAPI();
            this.SetUpBasicConfig();
        }
        if (this.Helper.ModRegistry.IsLoaded("Digus.ProducerFrameworkMod"))
        {
            this.Helper.Events.Player.Warped += this.OnPlayerWarp;
            this.Helper.Events.GameLoop.DayStarted += this.OnDayStarted;

            this.Helper.ConsoleCommands.Add(
                name: "av.hem.list_pfm_machines",
                documentation: "Prints info about PFM machines...",
                callback: PFMMachineHandler.PrintPFMRecipes);
        }
    }

    /// <summary>
    /// Resets the GMCM when the player has returned to the title.
    /// </summary>
    /// <param name="sender">SMAPI.</param>
    /// <param name="e">event args.</param>
    private void OnReturnedToTitle(object? sender, ReturnedToTitleEventArgs e)
    {
        if (this.gmcmHelper?.HasGottenAPI == true)
        {
            this.gmcmHelper.Unregister();
            this.SetUpBasicConfig();
        }
    }

    private void OnDayStarted(object? sender, DayStartedEventArgs e)
        => PFMMachineHandler.RefreshValidityList(Game1.currentLocation);

    private void OnPlayerWarp(object? sender, WarpedEventArgs e)
        => PFMMachineHandler.RefreshValidityList(e.NewLocation);

    /// <summary>
    /// Sets up the basic GMCM (does not include PFM machines).
    /// </summary>
    private void SetUpBasicConfig()
    {
        if (this.gmcmHelper?.HasGottenAPI == true)
        {
            if (this.Helper.ModRegistry.IsLoaded("Digus.ProducerFrameworkMod"))
            {
                this.gmcmHelper.Register(
                    reset: static () =>
                    {
                        Config = new();
                        PFMMachineHandler.RefreshValidityList(Game1.currentLocation);
                    },
                    save: () =>
                    {
                        this.Helper.AsyncWriteConfig(this.Monitor, Config);
                        PFMMachineHandler.RefreshValidityList(Game1.currentLocation);
                    });
            }
            else
            {
                this.gmcmHelper.Register(
                reset: static () => Config = new(),
                save: () => this.Helper.AsyncWriteConfig(this.Monitor, Config));
            }
            this.gmcmHelper.AddParagraph(I18n.ModDescription)
            .GenerateDefaultGMCM(static () => Config)
            .AddPageHere(
                pageId: "individual-machines",
                linkText: I18n.IndividualMachines_Title,
                tooltip: I18n.IndividualMachines_Description,
                pageTitle: I18n.IndividualMachines_Title);

            foreach (VanillaMachinesEnum machine in Config.VanillaMachines.Keys)
            {
                this.gmcmHelper.AddBoolOption(
                    name: () => machine.GetBestTranslatedString(),
                    getValue: () => Config.VanillaMachines[machine],
                    setValue: (bool val) => Config.VanillaMachines[machine] = val);
            }
        }
    }

    /// <summary>
    /// OnSaveLoaded event handler.
    /// </summary>
    /// <param name="sender">SMAPI.</param>
    /// <param name="e">Event args.</param>
    /// <remarks>EventPriority.Low to slot after pfm.</remarks>
    [EventPriority(EventPriority.Low)]
    private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
    {
        if (Context.IsSplitScreen && Context.ScreenId != 0)
        {
            return;
        }

        this.migrator = new(this.ModManifest, this.Helper, this.Monitor);
        if (!this.migrator.CheckVersionInfo())
        {
            this.Helper.Events.GameLoop.Saved += this.WriteMigrationData;
        }
        else
        {
            this.migrator = null;
        }

        if (this.Helper.ModRegistry.IsLoaded("Digus.ProducerFrameworkMod"))
        {
            PFMMachineHandler.ProcessPFMRecipes();

            foreach (int machineID in PFMMachineHandler.PFMMachines)
            {
                // Prepopulate the machine list.
                _ = Config.ProducerFrameworkModMachines.TryAdd(machineID.GetBigCraftableName(), true);
            }

            if (this.gmcmHelper?.HasGottenAPI == true)
            {
                this.gmcmHelper.AddSectionTitle(I18n.PFM_Section)
                    .AddParagraph(I18n.PFM_Description);

                foreach (int machineID in PFMMachineHandler.PFMMachines)
                {
                    // Add an option for it.
                    this.gmcmHelper.AddBoolOption(
                        name: () => machineID.GetBigCraftableTranslatedName(),
                        getValue: () => !Config.ProducerFrameworkModMachines.TryGetValue(machineID.GetBigCraftableName(), out bool val) || val,
                        setValue: (val) => Config.ProducerFrameworkModMachines[machineID.GetBigCraftableName()] = val);
                }
            }

            this.Monitor.Log("PFM compat set up!", LogLevel.Trace);
            this.Helper.AsyncWriteConfig(this.Monitor, Config);
        }
        else
        {
            this.Monitor.Log("PFM not installed, integration unnecessary", LogLevel.Trace);
        }
    }

    /// <summary>
    /// Writes migration data then detaches the migrator.
    /// </summary>
    /// <param name="sender">Smapi thing.</param>
    /// <param name="e">Arguments for just-before-saving.</param>
    private void WriteMigrationData(object? sender, SavedEventArgs e)
    {
        if (this.migrator is not null)
        {
            this.migrator.SaveVersionInfo();
            this.migrator = null;
        }
        this.Helper.Events.GameLoop.Saved -= this.WriteMigrationData;
    }
}