/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/ForgeMenuChoice
**
*************************************************/

using AtraBase.Toolkit.Reflection;
using AtraShared.Integrations;
using AtraShared.Utils.Extensions;
using ForgeMenuChoice.HarmonyPatches;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;

namespace ForgeMenuChoice;

/// <inheritdoc/>
internal class ModEntry : Mod
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    /// <summary>
    /// Gets the logger for this file.
    /// </summary>
    internal static IMonitor ModMonitor { get; private set; }

    /// <summary>
    /// Gets the content helper for this mod.
    /// </summary>
    internal static IContentHelper ContentHelper { get; private set; }

    /// <summary>
    /// Gets the translation helper for this mod.
    /// </summary>
    internal static ITranslationHelper TranslationHelper { get; private set; }

    /// <summary>
    /// Gets the configuration class for this mod.
    /// </summary>
    internal static ModConfig Config { get; private set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    /// <inheritdoc/>
    public override void Entry(IModHelper helper)
    {
        ModMonitor = this.Monitor;
        ContentHelper = helper.Content;
        TranslationHelper = helper.Translation;
        I18n.Init(helper.Translation);

        try
        {
            Config = this.Helper.ReadConfig<ModConfig>();
        }
        catch
        {
            this.Monitor.Log(I18n.IllFormatedConfig(), LogLevel.Warn);
            Config = new();
        }

        helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
        helper.Events.Player.Warped += this.OnWarp;
        helper.Content.AssetLoaders.Add(AssetLoader.Instance);
    }

    private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
    {
        ContentHelper.InvalidateCache(AssetLoader.ENCHANTMENT_NAMES_LOCATION);
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type. This is a valid call, SMAPI just doesn't use nullable.
        // This is the games cache of enchantment names. I null it here to clear it, in case the user changes languages.
        this.Helper.Reflection.GetField<List<BaseEnchantment>>(typeof(BaseEnchantment), "_enchantments").SetValue(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        AssetLoader.Refresh();
    }

    /// <summary>
    /// Things to run after all mods are initialized.
    /// And the game is launched.
    /// </summary>
    /// <param name="sender">SMAPI.</param>
    /// <param name="e">Event arguments.</param>
    /// <remarks>We must wait until GameLaunched to patch in order to patch Spacecore.</remarks>
    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        this.ApplyPatches(new Harmony(this.ModManifest.UniqueID));

        GMCMHelper helper = new(this.Monitor, this.Helper.Translation, this.Helper.ModRegistry, this.ModManifest);
        if (!helper.TryGetAPI())
        {
            return;
        }
        helper.Register(
            reset: () => Config = new ModConfig(),
            save: () => this.Helper.WriteConfig(Config))
        .AddParagraph(I18n.ModDescription)
        .AddEnumOption(
            name: I18n.TooltipBehavior_Title,
            getValue: () => Config.TooltipBehavior,
            setValue: (value) => Config.TooltipBehavior = value,
            tooltip: I18n.TooltipBehavior_Description)
        .AddBoolOption(
            name: I18n.EnableTooltipAutogeneration_Title,
            getValue: () => Config.EnableTooltipAutogeneration,
            setValue: (value) => Config.EnableTooltipAutogeneration = value,
            tooltip: I18n.EnableTooltipAutogeneration_Description);
    }

    private void ApplyPatches(Harmony harmony)
    {
        try
        {
            harmony.PatchAll();

            if (this.Helper.ModRegistry.Get("spacechase0.SpaceCore") is not IModInfo spacecore)
            {
                this.Monitor.Log($"Spacecore not installed, compat patches unnecessary.", LogLevel.Debug);
            }
            else
            {
                if (AccessTools.TypeByName("SpaceCore.Interface.NewForgeMenu") is Type spaceforge)
                {
                    this.Monitor.Log($"Got spacecore's forge for compat patching.", LogLevel.Debug);
                    harmony.Patch(
                        original: spaceforge.InstanceMethodNamed("cleanupBeforeExit"),
                        prefix: new HarmonyMethod(typeof(ForgeMenuPatches), nameof(ForgeMenuPatches.PrefixBeforeExit)));
                    harmony.Patch(
                        original: spaceforge.InstanceMethodNamed("IsValidCraft"),
                        prefix: new HarmonyMethod(typeof(ForgeMenuPatches), nameof(ForgeMenuPatches.PrefixIsValidCraft)));
                    harmony.Patch(
                        original: spaceforge.InstanceMethodNamed("draw", new Type[] { typeof(SpriteBatch) }),
                        postfix: new HarmonyMethod(typeof(ForgeMenuPatches), nameof(ForgeMenuPatches.PostfixDraw)));
                    harmony.Patch(
                        original: spaceforge.InstanceMethodNamed("receiveLeftClick"),
                        postfix: new HarmonyMethod(typeof(ForgeMenuPatches), nameof(ForgeMenuPatches.PostFixLeftClick)));
                    harmony.Patch(
                        original: spaceforge.InstanceMethodNamed("receiveRightClick"),
                        postfix: new HarmonyMethod(typeof(ForgeMenuPatches), nameof(ForgeMenuPatches.PostfixRightClick)));
                    harmony.Patch(
                        original: spaceforge.InstanceMethodNamed("gameWindowSizeChanged"),
                        postfix: new HarmonyMethod(typeof(ForgeMenuPatches), nameof(ForgeMenuPatches.PostfixGameWindowSizeChanged)));
                    harmony.Patch(
                        original: spaceforge.InstanceMethodNamed("performHoverAction"),
                        postfix: new HarmonyMethod(typeof(ForgeMenuPatches), nameof(ForgeMenuPatches.PostfixPerformHoverAction)));
                }
                else
                {
                    this.Monitor.Log($"Failed to grab Spacecore for compat patching, this mod may not work.", LogLevel.Warn);
                }
            }
        }
        catch (Exception ex)
        {
            ModMonitor.Log($"Mod failed while applying patches:\n{ex}", LogLevel.Error);
        }
        harmony.Snitch(this.Monitor, harmony.Id, transpilersOnly: true);
    }

    private void OnWarp(object? sender, WarpedEventArgs e)
    {
        if (e.IsLocalPlayer)
        {
            AssetLoader.Refresh();
        }
    }
}