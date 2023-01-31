/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraBase.Toolkit.Reflection;

using AtraCore.Framework.ReflectionManager;

using AtraShared.ConstantsAndEnums;
using AtraShared.Integrations;
using AtraShared.Utils;
using AtraShared.Utils.Extensions;

using ForgeMenuChoice.HarmonyPatches;

using HarmonyLib;

using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI.Events;

using AtraUtils = AtraShared.Utils.Utils;

namespace ForgeMenuChoice;

/// <inheritdoc/>
internal sealed class ModEntry : Mod
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    /// <summary>
    /// Gets the logger for this file.
    /// </summary>
    internal static IMonitor ModMonitor { get; private set; }

    /// <summary>
    /// Gets the game content helper for this mod.
    /// </summary>
    internal static IGameContentHelper GameContentHelper { get; private set; }

    /// <summary>
    /// Gets the translation helper for this mod.
    /// </summary>
    internal static ITranslationHelper TranslationHelper { get; private set; }

    /// <summary>
    /// Gets the configuration class for this mod.
    /// </summary>
    internal static ModConfig Config { get; private set; }

    /// <summary>
    /// Gets the input helper for this mod.
    /// </summary>
    internal static IInputHelper InputHelper { get; private set; }

    /// <summary>
    /// Gets the string utilities for this mod.
    /// </summary>
    internal static StringUtils StringUtils { get; private set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    /// <summary>
    /// Gets a delegate that checks to see if the forge instance is Casey's NewForgeMenu or not.
    /// </summary>
    internal static Func<object, bool>? IsSpaceForge { get; private set; } = null;

    /// <inheritdoc/>
    public override void Entry(IModHelper helper)
    {
        ModMonitor = this.Monitor;

        StringUtils = new(this.Monitor);
        GameContentHelper = helper.GameContent;
        TranslationHelper = helper.Translation;
        InputHelper = helper.Input;

        I18n.Init(helper.Translation);
        AssetLoader.Initialize(helper.GameContent);
        Config = AtraUtils.GetConfigOrDefault<ModConfig>(helper, this.Monitor);

        this.Monitor.Log($"Starting up: {this.ModManifest.UniqueID} - {typeof(ModEntry).Assembly.FullName}");

        helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;

        helper.Events.Player.Warped += this.Player_Warped;
        helper.Events.Content.AssetRequested += this.OnAssetRequested;
        helper.Events.Content.LocaleChanged += this.OnLocaleChanged;
        helper.Events.Content.AssetsInvalidated += this.OnAssetInvalidated;

        helper.Events.Input.ButtonsChanged += this.OnButtonsChanged;
    }

    private void OnButtonsChanged(object? sender, ButtonsChangedEventArgs e)
        => ForgeMenuPatches.ApplyButtonPresses(e);

    private void OnLocaleChanged(object? sender, LocaleChangedEventArgs e)
    {
        this.Helper.GameContent.InvalidateCacheAndLocalized(AssetLoader.ENCHANTMENT_NAMES_LOCATION);

        // This is the games cache of enchantment names. I null it here to clear it.
        this.Helper.Reflection.GetField<List<BaseEnchantment>?>(typeof(BaseEnchantment), "_enchantments").SetValue(null);
        AssetLoader.Refresh();
    }

    /// <inheritdoc cref="IGameLoopEvents.GameLaunched"/>
    /// <remarks>We must wait until GameLaunched to patch in order to patch Spacecore.</remarks>
    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        this.ApplyPatches(new Harmony(this.ModManifest.UniqueID));

        GMCMHelper helper = new(this.Monitor, this.Helper.Translation, this.Helper.ModRegistry, this.ModManifest);
        if (helper.TryGetAPI())
        {
            helper.Register(
                reset: static () => Config = new ModConfig(),
                save: () => this.Helper.AsyncWriteConfig(this.Monitor, Config))
            .AddParagraph(I18n.ModDescription)
            .GenerateDefaultGMCM(static () => Config);
        }
    }

    private void ApplyPatches(Harmony harmony)
    {
        try
        {
            harmony.PatchAll(typeof(ModEntry).Assembly);

            if (this.Helper.ModRegistry.Get("Goldenrevolver.EnchantableScythes") is IModInfo sycthes)
            {
                this.Monitor.Log("Applying compat patches for Enchantable Scythes.", LogLevel.Debug);
                GetEnchantmentPatch.ApplyPatch(harmony);
            }

            if (this.Helper.ModRegistry.Get("spacechase0.SpaceCore") is not IModInfo spacecore)
            {
                this.Monitor.Log($"Spacecore not installed, compat patches unnecessary.", LogLevel.Trace);
            }
            else
            {
                if (AccessTools.TypeByName("SpaceCore.Interface.NewForgeMenu") is Type spaceforge)
                {
                    this.Monitor.Log($"Got spacecore's forge for compat patching.", LogLevel.Debug);
                    harmony.Patch(
                        original: spaceforge.GetCachedMethod("cleanupBeforeExit", ReflectionCache.FlagTypes.InstanceFlags),
                        prefix: new HarmonyMethod(typeof(ForgeMenuPatches), nameof(ForgeMenuPatches.PrefixBeforeExit)));
                    harmony.Patch(
                        original: spaceforge.GetCachedMethod("IsValidCraft", ReflectionCache.FlagTypes.InstanceFlags),
                        prefix: new HarmonyMethod(typeof(ForgeMenuPatches), nameof(ForgeMenuPatches.PrefixIsValidCraft)));
                    harmony.Patch(
                        original: spaceforge.GetCachedMethod("draw", ReflectionCache.FlagTypes.InstanceFlags, new Type[] { typeof(SpriteBatch) }),
                        postfix: new HarmonyMethod(typeof(ForgeMenuPatches), nameof(ForgeMenuPatches.PostfixDraw)));
                    harmony.Patch(
                        original: spaceforge.GetCachedMethod("receiveLeftClick", ReflectionCache.FlagTypes.InstanceFlags),
                        postfix: new HarmonyMethod(typeof(ForgeMenuPatches), nameof(ForgeMenuPatches.PostFixLeftClick)));
                    harmony.Patch(
                        original: spaceforge.GetCachedMethod("receiveRightClick", ReflectionCache.FlagTypes.InstanceFlags),
                        postfix: new HarmonyMethod(typeof(ForgeMenuPatches), nameof(ForgeMenuPatches.PostfixRightClick)));
                    harmony.Patch(
                        original: spaceforge.GetCachedMethod("gameWindowSizeChanged", ReflectionCache.FlagTypes.InstanceFlags),
                        postfix: new HarmonyMethod(typeof(ForgeMenuPatches), nameof(ForgeMenuPatches.PostfixGameWindowSizeChanged)));
                    harmony.Patch(
                        original: spaceforge.GetCachedMethod("performHoverAction", ReflectionCache.FlagTypes.InstanceFlags),
                        postfix: new HarmonyMethod(typeof(ForgeMenuPatches), nameof(ForgeMenuPatches.PostfixPerformHoverAction)));

                    IsSpaceForge = spaceforge.GetTypeIs();
                }
                else
                {
                    this.Monitor.Log($"Failed to grab Spacecore for compat patching, this mod may not work.", LogLevel.Warn);
                }
            }
        }
        catch (Exception ex)
        {
            ModMonitor.Log(string.Format(ErrorMessageConsts.HARMONYCRASH, ex), LogLevel.Error);
        }
        harmony.Snitch(this.Monitor, harmony.Id, transpilersOnly: true);
    }

    /*****************
     * REGION ASSET MANAGEMENT
     * ************/

    private void Player_Warped(object? sender, WarpedEventArgs e)
    {
        if (e.IsLocalPlayer)
        {
            AssetLoader.Refresh();
        }
    }

    private void OnAssetInvalidated(object? sender, AssetsInvalidatedEventArgs e)
        => AssetLoader.Refresh(e.NamesWithoutLocale);

    private void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
        => AssetLoader.OnLoadAsset(e);
}
