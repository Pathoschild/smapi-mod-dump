/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Arsenal.Commands;

#region using directives

using Common.Commands;
using Framework;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Tools;
using System;

#endregion using directives

[UsedImplicitly]
internal sealed class PurificationCommand : ConsoleCommand
{
    /// <summary>Construct an instance.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal PurificationCommand(CommandHandler handler)
        : base(handler) { }

    /// <inheritdoc />
    public override string Trigger => "purification";

    /// <inheritdoc />
    public override string Documentation => "Transform a currently held Dark Sword into a Holy Blade.";

    /// <inheritdoc />
    public override void Callback(string[] args)
    {
        Game1.player.CurrentTool = new MeleeWeapon(Constants.DARK_SWORD_INDEX_I);
        Game1.player.Halt();
        Game1.player.faceDirection(2);
        Game1.player.showCarrying();
        Game1.player.jitterStrength = 1f;
        Game1.pauseThenDoFunction(3000, Utils.GetHolyBlade);
        Game1.changeMusicTrack("none", false, Game1.MusicContext.Event);
        Game1.currentLocation.playSound("crit");
        Game1.screenGlowOnce(Color.Transparent, true, 0.01f, 0.999f);
        DelayedAction.playSoundAfterDelay("stardrop", 1500);
        Game1.screenOverlayTempSprites.AddRange(
            Utility.sparkleWithinArea(new(0, 0, Game1.viewport.Width, Game1.viewport.Height), 500, Color.Gold, 10,
                2000));
        Game1.afterDialogues = (Game1.afterFadeFunction)Delegate.Combine(Game1.afterDialogues,
            (Game1.afterFadeFunction)delegate { Game1.stopMusicTrack(Game1.MusicContext.Event); });
    }
}