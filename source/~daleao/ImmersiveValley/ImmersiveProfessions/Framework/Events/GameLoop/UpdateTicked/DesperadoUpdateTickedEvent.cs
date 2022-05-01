/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using System;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Tools;

using AssetLoaders;
using Extensions;
using Ultimate;

#endregion using directives

internal class DesperadoUpdateTickedEvent : UpdateTickedEvent
{
    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object sender, UpdateTickedEventArgs e)
    {
        if (Game1.player.CurrentTool is not Slingshot slingshot || slingshot.attachments[0] is null ||
            !Game1.player.usingSlingshot ||
            ModEntry.PlayerState.RegisteredUltimate is DeathBlossom {IsActive: true}) return;

        var overcharge = slingshot.GetDesperadoOvercharge(Game1.player);
        if (overcharge <= 0f) return;

        Game1.player.jitterStrength = Math.Max(0f, overcharge - 0.5f);

        if (Game1.soundBank is null) return;

        SoundBank.DesperadoChargeSound ??= Game1.soundBank.GetCue("SinWave");
        if (SoundBank.DesperadoChargeSound is null) return;

        if (!SoundBank.DesperadoChargeSound.IsPlaying) SoundBank.DesperadoChargeSound.Play();

        SoundBank.DesperadoChargeSound.SetVariable("Pitch", 2400f * overcharge);
    }
}