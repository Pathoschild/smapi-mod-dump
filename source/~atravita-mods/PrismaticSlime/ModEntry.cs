/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraCore.Utilities;

using AtraShared.ConstantsAndEnums;
using AtraShared.Integrations;
using AtraShared.Integrations.Interfaces;
using AtraShared.MigrationManager;
using AtraShared.Utils.Extensions;
using HarmonyLib;
using PrismaticSlime.Framework;
using StardewModdingAPI.Events;

namespace PrismaticSlime;

/// <inheritdoc/>
internal sealed class ModEntry : Mod
{
    /// <summary>
    /// String key used to index the number of slime balls popped.
    /// </summary>
    internal const string SlimePoppedStat = "atravita.SlimeBallsPopped";

    /// <summary>
    /// Int Id of the prismatic jelly.
    /// </summary>
    internal const int PrismaticJelly = 876;

    private const string SAVEKEY = "item_ids";

    private static IJsonAssetsAPI? jsonAssets;
    private MigrationManager? migrator;

    /// <summary>
    /// Gets the logger for this mod.
    /// </summary>
    internal static IMonitor ModMonitor { get; private set; } = null!;

#pragma warning disable SA1201 // Elements should appear in the correct order - keeping fields near their accessors.
    private static int prismaticSlimeEgg = -1;

    /// <summary>
    /// Gets the integer BuffId of the Prismatic Slime Egg. -1 if not found/not loaded yet.
    /// </summary>
    internal static int PrismaticSlimeEgg
    {
        get
        {
            if (prismaticSlimeEgg == -1)
            {
                prismaticSlimeEgg = jsonAssets?.GetObjectId("atravita.PrismaticSlime Egg") ?? -1;
            }
            return prismaticSlimeEgg;
        }
    }

    private static int prismaticSlimeRing = -1;

    /// <summary>
    /// Gets the integer BuffId of the Prismatic Slime Ring. -1 if not found/not loaded yet.
    /// </summary>
    internal static int PrismaticSlimeRing
    {
        get
        {
            if (prismaticSlimeRing == -1)
            {
                prismaticSlimeRing = jsonAssets?.GetObjectId("atravita.PrismaticSlimeRing") ?? -1;
            }
            return prismaticSlimeRing;
        }
    }

    private static int prismaticJellyToast = -1;

    /// <summary>
    /// Gets the integer BuffId of the Prismatic Slime Ring. -1 if not found/not loaded yet.
    /// </summary>
    internal static int PrismaticJellyToast
    {
        get
        {
            if (prismaticJellyToast == -1)
            {
                prismaticJellyToast = jsonAssets?.GetObjectId("atravita.PrismaticJellyToast") ?? -1;
            }
            return prismaticJellyToast;
        }
    }
#pragma warning restore SA1201 // Elements should appear in the correct order

    /// <inheritdoc/>
    public override void Entry(IModHelper helper)
    {
        ModMonitor = this.Monitor;
        AssetManager.Initialize(helper.GameContent);
        I18n.Init(helper.Translation);

        this.Monitor.Log($"Starting up: {this.ModManifest.UniqueID} - {typeof(ModEntry).Assembly.FullName}");

        helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;

        this.ApplyPatches(new Harmony(this.ModManifest.UniqueID));
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        {
            IntegrationHelper helper = new(this.Monitor, this.Helper.Translation, this.Helper.ModRegistry, LogLevel.Warn);
            if (!helper.TryGetAPI("spacechase0.JsonAssets", "1.10.3", out jsonAssets))
            {
                this.Monitor.Log("Packs could not be loaded! This mod will probably not function.", LogLevel.Error);
                return;
            }
            jsonAssets.LoadAssets(Path.Combine(this.Helper.DirectoryPath, "assets", "json-assets"), this.Helper.Translation);
        }

        this.Helper.Events.Content.AssetRequested += static (_, e) => AssetManager.Apply(e);
        this.Helper.Events.Content.AssetsInvalidated += static (_, e) => AssetManager.Reset(e.NamesWithoutLocale);

        this.Helper.Events.GameLoop.ReturnedToTitle += this.OnReturnedToTitle;
        this.Helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
    }

    private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
    {
        MultiplayerHelpers.AssertMultiplayerVersions(this.Helper.Multiplayer, this.ModManifest, this.Monitor, this.Helper.Translation);

        this.migrator = new(this.ModManifest, this.Helper, this.Monitor);
        if (!this.migrator.CheckVersionInfo())
        {
            this.Helper.Events.GameLoop.Saved += this.WriteMigrationData;
        }
        else
        {
            this.migrator = null;
        }

        if (Context.IsMainPlayer)
        {
            // hook event to save Ids so future migrations are possible.
            this.Helper.Events.GameLoop.Saving -= this.OnSaving;
            this.Helper.Events.GameLoop.Saving += this.OnSaving;
        }
    }

    /// <summary>
    /// Resets the IDs when returning to the title.
    /// </summary>
    /// <param name="sender">SMAPI.</param>
    /// <param name="e">Event args.</param>
    [EventPriority(EventPriority.High)]
    private void OnReturnedToTitle(object? sender, ReturnedToTitleEventArgs e)
    {
        prismaticSlimeRing = -1;
        prismaticSlimeEgg = -1;
        prismaticJellyToast = -1;
    }

    private void ApplyPatches(Harmony harmony)
    {
        try
        {
            // handle patches from annotations.
            harmony.PatchAll(typeof(ModEntry).Assembly);
        }
        catch (Exception ex)
        {
            ModMonitor.Log(string.Format(ErrorMessageConsts.HARMONYCRASH, ex), LogLevel.Error);
        }

        harmony.Snitch(this.Monitor, harmony.Id, transpilersOnly: true);
    }

    #region migration

    /// <inheritdoc cref="IGameLoopEvents.Saving"/>
    private void OnSaving(object? sender, SavingEventArgs e)
    {
        this.Helper.Events.GameLoop.Saving -= this.OnSaving;
        if (Context.IsMainPlayer)
        {
            DataModel data = this.Helper.Data.ReadSaveData<DataModel>(SAVEKEY) ?? new();
            bool changed = false;

            if (data.EggId != PrismaticSlimeEgg)
            {
                data.EggId = PrismaticSlimeEgg;
                changed = true;
            }

            if (data.RingId != PrismaticSlimeRing)
            {
                data.RingId = PrismaticSlimeRing;
                changed = true;
            }

            if (data.ToastId != PrismaticJellyToast)
            {
                data.ToastId = PrismaticJellyToast;
                changed = true;
            }

            if (changed)
            {
                ModEntry.ModMonitor.Log("Writing ids into save");
                this.Helper.Data.WriteSaveData("item_ids", data);
            }
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
    #endregion
}
