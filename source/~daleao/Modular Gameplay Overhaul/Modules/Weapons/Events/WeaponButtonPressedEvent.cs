/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Weapons.Events;

#region using directives

using DaLion.Shared.Enums;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Stardew;
using StardewModdingAPI.Events;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class WeaponButtonPressedEvent : ButtonPressedEvent
{
    /// <summary>Initializes a new instance of the <see cref="WeaponButtonPressedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal WeaponButtonPressedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    public override bool IsEnabled => WeaponsModule.Config.FaceMouseCursor || WeaponsModule.Config.SlickMoves;

    /// <inheritdoc />
    protected override void OnButtonPressedImpl(object? sender, ButtonPressedEventArgs e)
    {
        if (!Context.IsWorldReady || Game1.activeClickableMenu is not null)
        {
            return;
        }

        var player = Game1.player;
        if (player.UsingTool || player.isRidingHorse() || !player.CanMove)
        {
            return;
        }

        var isActionButton = e.Button.IsActionButton();
        var isUseToolButton = e.Button.IsUseToolButton();
        if (!isActionButton && !isUseToolButton)
        {
            return;
        }

        if (WeaponsModule.Config.EnableAutoSelection && Globals.AreEnemiesAround &&
            player.CurrentTool != WeaponsModule.State.AutoSelectableWeapon &&
            WeaponSelector.TryFor(player, out var index))
        {
            Game1.player.CurrentToolIndex = index;
        }

        if (player.CurrentTool is not MeleeWeapon weapon)
        {
            return;
        }

        var originalDirection = (FacingDirection)player.FacingDirection;
        if (WeaponsModule.Config.FaceMouseCursor && !Game1.options.gamepadControls &&
            !weapon.isScythe())
        {
            player.FaceTowardsTile(Game1.currentCursorTile);
        }

        if (isActionButton)
        {
            if (weapon.isScythe())
            {
                return;
            }

            switch (weapon.type.Value)
            {
                case MeleeWeapon.stabbingSword when MeleeWeapon.attackSwordCooldown > 0:
                case MeleeWeapon.defenseSword when MeleeWeapon.defenseCooldown > 0:
                case MeleeWeapon.dagger when MeleeWeapon.daggerCooldown > 0:
                case MeleeWeapon.club when MeleeWeapon.clubCooldown > 0:
                    return;
            }
        }

        if (!player.isMoving() || !player.running || !WeaponsModule.Config.SlickMoves)
        {
            return;
        }

        var directionVector = originalDirection.ToVector();
        if (originalDirection.IsVertical())
        {
            directionVector *= -1f;
        }

        var driftVelocity = directionVector * (1f + (Game1.player.addedSpeed * 0.1f)) * 3f;
        WeaponsModule.State.DriftVelocity = driftVelocity;
        this.Manager.Enable<SlickMovesUpdateTickingEvent>();
    }
}
