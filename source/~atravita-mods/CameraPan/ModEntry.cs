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
using AtraShared.Utils.Extensions;

using CameraPan.Framework;

using HarmonyLib;

using Microsoft.Xna.Framework;

using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;

using AtraUtils = AtraShared.Utils.Utils;

namespace CameraPan;

// TODO: just re-write the viewport center code at this point.
// TODO: draw a big arrow pointing towards the player if the player is off screen?

/// <inheritdoc />
internal sealed class ModEntry : Mod
{
    /// <summary>
    /// The integer ID of the camera item.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:Field names should not contain underscore", Justification = "Reviewed.")]
    internal const int CAMERA_ID = 106;

    /// <summary>
    /// Gets the config instance for this mod.
    /// </summary>
    internal static ModConfig Config { get; private set; } = null!;

    /// <summary>
    /// Gets the logging instance for this mod.
    /// </summary>
    internal static IMonitor ModMonitor { get; private set; } = null!;

    private static readonly PerScreen<Vector2> offset = new (() => Vector2.Zero);

    /// <summary>
    /// Gets or sets the amount of pixels to offset from the player position.
    /// </summary>
    internal static Vector2 Offset
    {
        get => offset.Value;
        set => offset.Value = value;
    }

    /// <inheritdoc />
    public override void Entry(IModHelper helper)
    {
        I18n.Init(helper.Translation);
        ModMonitor = this.Monitor;
        Config = AtraUtils.GetConfigOrDefault<ModConfig>(helper, this.Monitor);

        helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;

        helper.Events.GameLoop.UpdateTicked += this.OnTicked;
        helper.Events.Input.ButtonsChanged += this.OnButtonsChanged;

        helper.Events.Player.Warped += this.OnWarped;
        helper.Events.Display.MenuChanged += this.OnMenuChanged;

        helper.Events.Display.WindowResized += static (_, _) => Config?.RecalculateBounds();
    }

    private static void Reset()
    {
        offset.Value = Vector2.Zero;
    }

    private void OnButtonsChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (Config.ResetButton.JustPressed())
        {
            Reset();
        }
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        this.ApplyPatches(new Harmony(this.ModManifest.UniqueID));
        Config.RecalculateBounds();

        GMCMHelper gmcmHelper = new(this.Monitor, this.Helper.Translation, this.Helper.ModRegistry, this.ModManifest);
        if (gmcmHelper.TryGetAPI())
        {
            gmcmHelper.Register(
                reset: static () => Config = new(),
                save: () =>
                {
                    this.Helper.AsyncWriteConfig(this.Monitor, Config);
                    Config.RecalculateBounds();
                })
            .AddParagraph(I18n.Mod_Description)
            .GenerateDefaultGMCM(static () => Config);
        }
    }

    private void OnMenuChanged(object? sender, MenuChangedEventArgs e)
    {
        if (e.OldMenu is not null && e.NewMenu is null)
        {
            offset.Value = Vector2.Zero;
            Game1.viewportTarget = new Vector2(-2.14748365E+09f, -2.14748365E+09f);
        }
        else if (e.OldMenu is null && e.NewMenu is not null)
        {
            Game1.moveViewportTo(Game1.player.Position, Config.Speed);
        }
    }

    private void OnWarped(object? sender, WarpedEventArgs e)
    {
        if (e.IsLocalPlayer)
        {
            offset.Value = Vector2.Zero;
            Game1.viewportTarget = new Vector2(-2.14748365E+09f, -2.14748365E+09f);
        }
    }

    private void OnTicked(object? sender, UpdateTickedEventArgs e)
    {
        if (!Context.IsPlayerFree)
        {
            return;
        }
        Vector2 pos = this.Helper.Input.GetCursorPosition().ScreenPixels;
        Vector2 adjustment = Vector2.Zero;
        int width = Game1.viewport.Width / 8;
        if (pos.X < width)
        {
            adjustment.X = -Config.Speed;
        }
        else if (pos.X > Game1.viewport.Width - width)
        {
            adjustment.X = Config.Speed;
        }

        int height = Game1.viewport.Height / 8;
        if (pos.Y < height)
        {
            adjustment.Y = -Config.Speed;
        }
        else if (pos.Y > Game1.viewport.Height - height)
        {
            adjustment.Y = Config.Speed;
        }

        Vector2 temp = offset.Value + adjustment;

        temp.X = Math.Clamp(temp.X, -Config.XRangeInternal, Config.XRangeInternal);
        temp.Y = Math.Clamp(temp.Y, -Config.YRangeInternal, Config.YRangeInternal);

        offset.Value = temp;
        Game1.moveViewportTo(Game1.player.Position + offset.Value, Config.Speed);
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
}