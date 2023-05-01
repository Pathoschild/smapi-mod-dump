/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

#if DEBUG
using System.Diagnostics;
#endif

using AtraShared.ConstantsAndEnums;
using AtraShared.Integrations;
using AtraShared.MigrationManager;
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

        helper.Events.GameLoop.DayStarted += static (_, _) => BeehouseHandler.UpdateStatus(Game1.currentLocation);
        helper.Events.Player.Warped += static (_, e) => BeehouseHandler.UpdateStatus(e.NewLocation);

        this.Monitor.Log($"Starting up: {this.ModManifest.UniqueID} - {typeof(ModEntry).Assembly.FullName}");
    }

    [EventPriority(EventPriority.Low)]
    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        this.ApplyPatches(new Harmony(this.ModManifest.UniqueID));

        PFMMachineHandler.TryGetAPI(this.Helper.ModRegistry);
        BeehouseHandler.TryGetAPI(this.Helper.ModRegistry);
        this.gmcmHelper = new(this.Monitor, this.Helper.Translation, this.Helper.ModRegistry, this.ModManifest);
        if (this.gmcmHelper.TryGetAPI())
        {
            this.gmcmHelper.TryGetOptionsAPI();
            this.SetUpBasicConfig();
        }
        if (this.Helper.ModRegistry.IsLoaded("Digus.ProducerFrameworkMod"))
        {
            this.Helper.Events.Player.Warped += static (_, e) => PFMMachineHandler.RefreshValidityList(e.NewLocation);
            this.Helper.Events.GameLoop.DayStarted += static (_, _) => PFMMachineHandler.RefreshValidityList(Game1.currentLocation);

            this.Helper.ConsoleCommands.Add(
                name: "av.hem.list_pfm_machines",
                documentation: "Prints info about PFM machines...",
                callback: PFMMachineHandler.PrintPFMRecipes);
        }
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
            harmony.PatchAll(typeof(ModEntry).Assembly);
        }
        catch (Exception ex)
        {
            ModMonitor.Log(string.Format(ErrorMessageConsts.HARMONYCRASH, ex), LogLevel.Error);
        }
        harmony.Snitch(this.Monitor, harmony.Id, transpilersOnly: true);
    }

    /// <inheritdoc cref="IGameLoopEvents.ReturnedToTitle"/>
    /// <remarks>
    /// Resets the GMCM when the player has returned to the title.
    /// </remarks>
    private void OnReturnedToTitle(object? sender, ReturnedToTitleEventArgs e)
    {
        if (this.gmcmHelper?.HasGottenAPI == true)
        {
            this.gmcmHelper.Unregister();
            this.SetUpBasicConfig();
        }
    }

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
                        BeehouseHandler.UpdateStatus(Game1.currentLocation);
                    },
                    save: () =>
                    {
                        this.Helper.AsyncWriteConfig(this.Monitor, Config);
                        PFMMachineHandler.RefreshValidityList(Game1.currentLocation);
                        BeehouseHandler.UpdateStatus(Game1.currentLocation);
                    });
            }
            else
            {
                this.gmcmHelper.Register(
                reset: static () =>
                {
                    Config = new();
                    BeehouseHandler.UpdateStatus(Game1.currentLocation);
                },
                save: () =>
                {
                    this.Helper.AsyncWriteConfig(this.Monitor, Config);
                    BeehouseHandler.UpdateStatus(Game1.currentLocation);
                });
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

    /// <inheritdoc cref="IGameLoopEvents.SaveLoaded"/>
    /// <remarks>EventPriority.Low to slot after pfm.</remarks>
    [EventPriority(EventPriority.Low - 2000)]
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
#if DEBUG
            Stopwatch sw = Stopwatch.StartNew();
#endif

            PFMMachineHandler.ProcessPFMRecipes();
            bool changed = false;

            // Pre-populate the machine list.
            foreach (int machineID in PFMMachineHandler.ConditionalPFMMachines.Concat(PFMMachineHandler.UnconditionalPFMMachines))
            {
                changed |= Config.ProducerFrameworkModMachines.TryAdd(machineID.GetBigCraftableName(), true);
            }

            if (this.gmcmHelper?.HasGottenAPI == true)
            {
                this.gmcmHelper.AddSectionTitle(I18n.PFM_Section)
                    .AddParagraph(I18n.PFM_Description);

                foreach (int machineID in PFMMachineHandler.ConditionalPFMMachines.Concat(PFMMachineHandler.UnconditionalPFMMachines))
                {
                    this.gmcmHelper.AddBoolOption(
                        name: () => machineID.GetBigCraftableTranslatedName(),
                        getValue: () => !Config.ProducerFrameworkModMachines.TryGetValue(machineID.GetBigCraftableName(), out bool val) || val,
                        setValue: (val) => Config.ProducerFrameworkModMachines[machineID.GetBigCraftableName()] = val);
                }
            }

            this.Monitor.Log("PFM compat set up!", LogLevel.Trace);
            if (changed)
            {
                this.Helper.AsyncWriteConfig(this.Monitor, Config);
            }
#if DEBUG
            sw.Stop();
            this.Monitor.Log($"PFM compat took {sw.ElapsedMilliseconds} ms.");
#endif
        }
        else
        {
            this.Monitor.Log("PFM not installed, integration unnecessary", LogLevel.Trace);
        }
    }

    /// <inheritdoc cref="IGameLoopEvents.Saved"/>
    /// <remarks>
    /// Writes migration data then detaches the migrator.
    /// </remarks>
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