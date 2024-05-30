/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Professions.Framework.Events.GameLoop.UpdateTicked;

#region using directives

using DaLion.Professions.Framework.Events.Display.RenderedWorld;
using DaLion.Shared.Events;
using Microsoft.Xna.Framework.Audio;
using StardewModdingAPI.Events;
using StardewValley.Tools;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="DesperadoOverchargeUpdateTickedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class DesperadoOverchargeUpdateTickedEvent(EventManager? manager = null)
    : UpdateTickedEvent(manager ?? ProfessionsMod.EventManager)
{
    /// <inheritdoc />
    protected override void OnEnabled()
    {
        if (Game1.player.CurrentTool is not Slingshot)
        {
            this.Disable();
            return;
        }

        this.Manager.Enable<DesperadoRenderedWorldEvent>();
    }

    /// <inheritdoc />
    protected override void OnDisabled()
    {
        Game1.player.stopJittering();
        SoundBox.SinWave.Stop(AudioStopOptions.Immediate);
        this.Manager.Disable<DesperadoRenderedWorldEvent>();
    }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        var firer = Game1.player;
        if (firer.CurrentTool is not Slingshot slingshot || !firer.usingSlingshot || slingshot.CanAutoFire())
        {
            this.Disable();
            return;
        }

        var overchargePct = slingshot.GetOvercharge() - 1f;
        if (overchargePct <= 0f)
        {
            return;
        }

        firer.jitterStrength = Math.Max(0f, overchargePct);

        if (Game1.soundBank is null)
        {
            return;
        }

        if (!SoundBox.SinWave.IsPlaying)
        {
            SoundBox.SinWave.Play();
        }

        SoundBox.SinWave.SetVariable("Pitch", 2400f * overchargePct);
    }
}
