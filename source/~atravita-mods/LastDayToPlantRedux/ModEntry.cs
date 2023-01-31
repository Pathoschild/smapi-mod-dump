/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraShared.Integrations;
using AtraShared.MigrationManager;
using AtraShared.Utils.Extensions;
using AtraShared.Utils.Shims;

using LastDayToPlantRedux.Framework;

using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;

using StardewValley.Menus;

using AtraUtils = AtraShared.Utils.Utils;

namespace LastDayToPlantRedux;

/// <inheritdoc />
internal sealed class ModEntry : Mod
{
    private MigrationManager? migrator;
    private readonly PerScreen<bool> hasSeeds = new(() => false);

    /// <summary>
    /// Gets the logger for this mod.
    /// </summary>
    internal static IMonitor ModMonitor { get; private set; } = null!;

    /// <summary>
    /// Gets the config instance for this mod.
    /// </summary>
    internal static ModConfig Config { get; private set; } = null!;

    /// <inheritdoc />
    public override void Entry(IModHelper helper)
    {
        // bind helpers.
        ModMonitor = this.Monitor;
        I18n.Init(helper.Translation);
        AssetManager.Initialize(helper.GameContent);

        this.Monitor.Log($"Starting up: {this.ModManifest.UniqueID} - {typeof(ModEntry).Assembly.FullName}");

        Config = AtraUtils.GetConfigOrDefault<ModConfig>(helper, this.Monitor);
        helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;

        helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
        helper.Events.GameLoop.DayStarted += this.OnDayStart;
        helper.Events.GameLoop.DayStarted += this.UpdateMailboxen;
        helper.Events.GameLoop.ReturnedToTitle += this.OnReturnedToTitle;
        helper.Events.Player.Warped += this.OnPlayerWarped;

        helper.Events.Player.InventoryChanged += (_, e) => InventoryWatcher.Watch(e, helper.Data);
        helper.Events.GameLoop.Saving += (_, _) => InventoryWatcher.SaveModel(helper.Data);

        helper.Events.Multiplayer.PeerConnected += static (_, e) => MultiplayerManager.OnPlayerConnected(e);
        helper.Events.Multiplayer.PeerDisconnected += static (_, e) => MultiplayerManager.OnPlayerDisconnected(e);

        helper.Events.Content.AssetRequested += static (_, e) => AssetManager.Apply(e);
        helper.Events.Content.AssetsInvalidated += static (_, e) => AssetManager.InvalidateCache(e);
    }

    /// <inheritdoc />
    public override object? GetApi() => new LastDayToPlantAPI();

    /// <inheritdoc cref="IGameLoopEvents.ReturnedToTitle"/>
    [EventPriority(EventPriority.High + 10)]
    private void OnReturnedToTitle(object? sender, ReturnedToTitleEventArgs e)
    {
        CropAndFertilizerManager.RequestInvalidateCrops();
        CropAndFertilizerManager.RequestInvalidateFertilizers();
        InventoryWatcher.ClearModel();
        MultiplayerManager.Reset();
    }

    /// <inheritdoc cref="IGameLoopEvents.GameLaunched"/>
    [EventPriority(EventPriority.Low)]
    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        MultiplayerManager.SetShouldCheckPrestiged(this.Helper.ModRegistry);

        // Ask for AtraCore's JAShims to be initialized.
        JsonAssetsShims.Initialize(this.Monitor, this.Helper.Translation, this.Helper.ModRegistry);

        GMCMHelper helper = new(this.Monitor, this.Helper.Translation, this.Helper.ModRegistry, this.ModManifest);
        if (helper.TryGetAPI())
        {
            helper.Register(
                reset: static () => Config = new(),
                save: () => this.Helper.AsyncWriteConfig(this.Monitor, Config))
            .AddParagraph(I18n.ModDescription)
            .GenerateDefaultGMCM(static () => Config);
        }
    }

    /// <inheritdoc cref="IGameLoopEvents.SaveLoaded"/>
    [EventPriority(EventPriority.Low)]
    private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
    {
        // move the mail to the end so it's easier to find.
        Game1.player.mailReceived.Remove(AssetManager.MailFlag);
        Game1.player.mailReceived.Add(AssetManager.MailFlag);

        FarmerWatcher? watcher = new();
        Game1.player.professions.OnArrayReplaced += watcher.Professions_OnArrayReplaced;
        Game1.player.professions.OnElementChanged += watcher.Professions_OnElementChanged;

        if (Context.ScreenId == 0)
        {
            InventoryWatcher.LoadModel(this.Helper.Data);

            this.migrator = new(this.ModManifest, this.Helper, this.Monitor);

            if (!this.migrator.CheckVersionInfo())
            {
                this.Helper.Events.GameLoop.Saved += this.WriteMigrationData;
            }
            else
            {
                this.migrator = null;
            }
        }
    }

    /// <inheritdoc cref="IGameLoopEvents.DayStarted"/>
    [EventPriority(EventPriority.Low)]
    private void OnDayStart(object? sender, DayStartedEventArgs e)
    {
        if (Context.ScreenId != 0)
        {
            return;
        }

        bool hasSeeds = AssetManager.UpdateOnDayStart();
        this.Helper.GameContent.InvalidateCacheAndLocalized("Data/mail");
        this.hasSeeds.Value = hasSeeds;

        if (Context.IsSplitScreen)
        {
            foreach (int? screen in this.Helper.Multiplayer.GetConnectedPlayers().Where(player => player.IsSplitScreen).Select(player => player.ScreenID))
            {
                if (screen is not null)
                {
                    this.hasSeeds.SetValueForScreen(screen.Value, hasSeeds);
                }
            }
        }
    }

    /// <inheritdoc cref="IGameLoopEvents.DayStarted"/>
    /// <remarks>Runs after <see cref="OnDayStart(object?, DayStartedEventArgs)"/>, used to populate the mailboxes in splitscreen.
    /// Event priorities make sure that one runs first.</remarks>
    [EventPriority(EventPriority.Low - 1000)]
    private void UpdateMailboxen(object? sender, DayStartedEventArgs e)
    {
        Game1.mailbox.Remove(AssetManager.MailFlag);
        if (Config.DisplayOption == DisplayOptions.InMailbox && this.hasSeeds.Value)
        {
            Game1.mailbox.Add(AssetManager.MailFlag);
        }
    }

    /// <inheritdoc cref="IPlayerEvents.Warped"/>
    [EventPriority(EventPriority.Low)]
    private void OnPlayerWarped(object? sender, WarpedEventArgs e)
    {
        if (this.hasSeeds.Value && e.IsLocalPlayer && Context.IsPlayerFree && Config.DisplayOption == DisplayOptions.OnFirstWarp)
        {
            this.hasSeeds.Value = false;
            Dictionary<string, string>? maildata = Game1.content.Load<Dictionary<string, string>>(AssetManager.DataMail.BaseName);

            if (maildata.TryGetValue(AssetManager.MailFlag, out string? mail))
            {
                Game1.activeClickableMenu = new LetterViewerMenu(mail, AssetManager.MailFlag);
            }
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
