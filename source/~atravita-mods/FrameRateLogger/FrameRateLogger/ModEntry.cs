/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using System.Linq.Expressions;
using System.Reflection;

using AtraShared.Integrations.Interfaces;

using FrameRateLogger.Framework;

using StardewModdingAPI.Events;

namespace FrameRateLogger;

/// <inheritdoc />
internal sealed class ModEntry : Mod
{
    private bool enabled = true;
    private ModConfig config = null!;

    private Func<FrameRateCounter, int>? FramerateGetter { get; set; } = null!;

    private FrameRateCounter? FrameRateCounter { get; set; } = null!;

    /// <inheritdoc />
    public override void Entry(IModHelper helper)
    {
        this.Monitor.Log($"Starting up: {this.ModManifest.UniqueID} - {typeof(ModEntry).Assembly.FullName}");

        this.FrameRateCounter = new(GameRunner.instance);
        helper.Reflection.GetMethod(this.FrameRateCounter, "LoadContent").Invoke();
        FieldInfo? field = helper.Reflection.GetField<int>(this.FrameRateCounter, "frameRate").FieldInfo;

        ParameterExpression? objparam = Expression.Parameter(typeof(FrameRateCounter), "obj");
        MemberExpression? fieldgetter = Expression.Field(objparam, field);
        this.FramerateGetter = Expression.Lambda<Func<FrameRateCounter, int>>(fieldgetter, objparam).Compile();

        this.config = helper.ReadConfig<ModConfig>();
        if (this.config.Enabled)
        {
            this.enabled = true;
        }

        helper.Events.GameLoop.SaveLoaded += (_, _) =>
        {
            this.UnHook();
            if (this.enabled)
            {
                this.Hook();
            }
        };

        helper.Events.GameLoop.ReturnedToTitle += (_, _) => this.UnHook();

        helper.Events.Player.Warped += this.OnWarped;

        helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        if (this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu") is IGenericModConfigMenuApi api)
        {
            api.Register(
                mod: this.ModManifest,
                reset: () =>
                {
                    this.config = new();
                    if (!this.enabled)
                    {
                        this.UnHook();
                        this.Hook();
                    }
                    this.enabled = true;
                },
                save: () =>
                {
                    if (this.config.Enabled != this.enabled)
                    {
                        this.Monitor.Log("Switching " + (this.config.Enabled ? "on" : "off"));
                        this.enabled = this.config.Enabled;

                        this.UnHook();
                        if (this.enabled)
                        {
                            this.Hook();
                        }
                    }

                    this.Helper.WriteConfig(this.config);
                });
            api.AddBoolOption(
                mod: this.ModManifest,
                getValue: () => this.config.Enabled,
                setValue: value => this.config.Enabled = value,
                I18n.Enabled);
        }
    }

    private void OnWarped(object? sender, WarpedEventArgs e)
        => this.Monitor.Log($"Current memory usage {GC.GetTotalMemory(false):N0}", LogLevel.Info);

    private void UnHook()
    {
        this.Helper.Events.GameLoop.OneSecondUpdateTicked -= this.OnUpdateTicked;
        this.Helper.Events.Display.RenderedHud -= this.OnRenderedHud;
    }

    private void Hook()
    {
        //Game1.player.mailReceived.OnElementChanged += this.MailReceived_OnElementChanged;
        this.Helper.Events.GameLoop.OneSecondUpdateTicked += this.OnUpdateTicked;
        this.Helper.Events.Display.RenderedHud += this.OnRenderedHud;
    }

    private void MailReceived_OnElementChanged(Netcode.NetList<string, Netcode.NetString> list, int index, string oldValue, string newValue)
    {
        this.Monitor.Log($"Mail flags changed: index {index} old {oldValue} new {newValue}.", LogLevel.Alert);
    }

    /// <inheritdoc cref="IGameLoopEvents.OneSecondUpdateTicked"/>
    private void OnUpdateTicked(object? sender, OneSecondUpdateTickedEventArgs e)
    {
        if (this.FrameRateCounter is not null && this.FramerateGetter?.Invoke(this.FrameRateCounter) is int value && Game1.game1.IsActive)
        {
            this.Monitor.Log($"Current framerate on {Game1.ticks} is {value}", value < 30 ? LogLevel.Alert : LogLevel.Trace);
        }
    }

    /// <inheritdoc cref="IDisplayEvents.RenderedHud"/>
    private void OnRenderedHud(object? sender, RenderedHudEventArgs e)
    {
        this.FrameRateCounter?.Update(Game1.currentGameTime);
        this.FrameRateCounter?.Draw(Game1.currentGameTime);
    }
}
