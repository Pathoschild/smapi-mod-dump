/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Weapons.Commands;

#region using directives

using System.Diagnostics.CodeAnalysis;
using DaLion.Overhaul.Modules.Weapons.Events;
using DaLion.Overhaul.Modules.Weapons.Extensions;
using DaLion.Shared.Commands;
using Microsoft.Xna.Framework;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
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
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:Element should begin with upper-case letter", Justification = "Preference for inner functions.")]
    public override void Callback(string trigger, string[] args)
    {
        var player = Game1.player;
        if (player.CurrentTool is not MeleeWeapon { InitialParentTileIndex: ItemIDs.DarkSword })
        {
            Log.W("You must be holding the Dark Sword to use this command.");
            return;
        }

        player.Halt();
        player.faceDirection(2);
        player.showCarrying();
        player.jitterStrength = 1f;
        Game1.pauseThenDoFunction(3000, getHolyBlade);
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

        void getHolyBlade()
        {
            var player = Game1.player;
            if (player.CurrentTool is not MeleeWeapon { InitialParentTileIndex: ItemIDs.DarkSword } darkSword)
            {
                return;
            }

            Game1.flashAlpha = 1f;
            player.holdUpItemThenMessage(new MeleeWeapon(ItemIDs.HolyBlade));
            darkSword.transform(ItemIDs.HolyBlade);
            darkSword.RefreshStats();
            player.jitterStrength = 0f;
            Game1.screenGlowHold = false;
            EventManager.Disable<CurseUpdateTickedEvent>();
        }
    }
}
