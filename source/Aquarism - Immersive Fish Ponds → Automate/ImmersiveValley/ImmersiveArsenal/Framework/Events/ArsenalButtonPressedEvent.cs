/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Arsenal.Framework.Events;

#region using directives

using Common.Enums;
using Common.Events;
using Common.Extensions.Stardew;
using StardewModdingAPI.Events;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class ArsenalButtonPressedEvent : ButtonPressedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ArsenalButtonPressedEvent(EventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    protected override void OnButtonPressedImpl(object? sender, ButtonPressedEventArgs e)
    {
        var player = Game1.player;
        if ((!e.Button.IsActionButton() && !e.Button.IsUseToolButton()) || Game1.options.gamepadControls ||
            player.CurrentTool is not (MeleeWeapon or Slingshot) || player.UsingTool || player.isRidingHorse()) return;

        var direction = player.FacingDirection;
        player.FaceTowardsTile(Game1.currentCursorTile);
        if (!player.isMoving() || !player.running) return;

        var driftTrajectory = Common.Utility.VectorFromFacingDirection((FacingDirection)direction) *
                             (1f + Game1.player.addedSpeed * 0.1f) * 2f;
        player.setTrajectory(driftTrajectory);
    }
}