/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Arsenal.Commands;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Commands;
using Microsoft.Xna.Framework;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
[Debug]
internal sealed class SwordBlessCommand : ConsoleCommand
{
    /// <summary>Initializes a new instance of the <see cref="SwordBlessCommand"/> class.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal SwordBlessCommand(CommandHandler handler)
        : base(handler)
    {
    }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "bless" };

    /// <inheritdoc />
    public override string Documentation => "Transform a currently held Dark Sword into a Holy Blade.";

    /// <inheritdoc />
    public override void Callback(string[] args)
    {
        var player = Game1.player;
        if (player.CurrentTool is not MeleeWeapon { InitialParentTileIndex: Constants.DarkSwordIndex })
        {
            player.CurrentTool = new MeleeWeapon(Constants.DarkSwordIndex);
        }

        player.Halt();
        player.faceDirection(2);
        player.showCarrying();
        player.jitterStrength = 1f;
        Game1.pauseThenDoFunction(3000, Utils.GetHolyBlade);
        Game1.changeMusicTrack("none", false, Game1.MusicContext.Event);
        Game1.currentLocation.playSound("crit");
        Game1.screenGlowOnce(Color.Transparent, true, 0.01f, 0.999f);
        DelayedAction.playSoundAfterDelay("stardrop", 1500);
        Game1.screenOverlayTempSprites.AddRange(
            Utility.sparkleWithinArea(
                new Rectangle(0, 0, Game1.viewport.Width, Game1.viewport.Height),
                500,
                Color.Gold,
                10,
                2000));
        Game1.afterDialogues = (Game1.afterFadeFunction)Delegate.Combine(
            Game1.afterDialogues, (Game1.afterFadeFunction)(() => Game1.stopMusicTrack(Game1.MusicContext.Event)));
    }
}
