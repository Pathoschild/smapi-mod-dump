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

using StardewModdingAPI.Events;

namespace FrameRateLogger;

/// <inheritdoc />
internal sealed class ModEntry : Mod
{
    private Func<FrameRateCounter, int>? FramerateGetter { get; set; } = null!;

    private FrameRateCounter? FrameRateCounter { get; set; } = null!;

    /// <inheritdoc />
    public override void Entry(IModHelper helper)
    {
        this.FrameRateCounter = new(GameRunner.instance);
        helper.Reflection.GetMethod(this.FrameRateCounter, "LoadContent").Invoke();
        FieldInfo? field = helper.Reflection.GetField<int>(this.FrameRateCounter, "frameRate").FieldInfo;

        ParameterExpression? objparam = Expression.Parameter(typeof(FrameRateCounter), "obj");
        MemberExpression? fieldgetter = Expression.Field(objparam, field);
        this.FramerateGetter = Expression.Lambda<Func<FrameRateCounter, int>>(fieldgetter, objparam).Compile();

        helper.Events.Display.RenderedHud += this.OnRenderedHud;
        helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
        helper.Events.GameLoop.ReturnedToTitle += this.ReturnedToTitle;

        helper.Events.Player.Warped += this.OnWarped;
    }

    private void OnWarped(object? sender, WarpedEventArgs e)
        => this.Monitor.Log($"Current memory usage {GC.GetTotalMemory(false):N0}", LogLevel.Info);

    private void ReturnedToTitle(object? sender, ReturnedToTitleEventArgs e)
        => this.Helper.Events.GameLoop.OneSecondUpdateTicked -= this.OnUpdateTicked;

    private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
        => this.Helper.Events.GameLoop.OneSecondUpdateTicked += this.OnUpdateTicked;

    private void OnUpdateTicked(object? sender, OneSecondUpdateTickedEventArgs e)
    {
        if (this.FrameRateCounter is not null && this.FramerateGetter?.Invoke(this.FrameRateCounter) is int value)
        {
            this.Monitor.Log($"Current framerate on {Game1.ticks} is {value}", value < 30 ? LogLevel.Alert : LogLevel.Trace);
        }
    }

    private void OnRenderedHud(object? sender, RenderedHudEventArgs e)
    {
        this.FrameRateCounter?.Update(Game1.currentGameTime);
        this.FrameRateCounter?.Draw(Game1.currentGameTime);
    }
}
