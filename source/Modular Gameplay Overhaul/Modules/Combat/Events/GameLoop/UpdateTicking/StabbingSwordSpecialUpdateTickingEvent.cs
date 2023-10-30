/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/modular-overhaul
**
*************************************************/

namespace DaLion.Overhaul.Modules.Combat.Events.GameLoop.UpdateTicking;

#region using directives

using DaLion.Overhaul.Modules.Combat.Enchantments;
using DaLion.Overhaul.Modules.Combat.Events.GameLoop.UpdateTicked;
using DaLion.Overhaul.Modules.Combat.Events.Input.ButonPressed;
using DaLion.Overhaul.Modules.Combat.Extensions;
using DaLion.Shared.Constants;
using DaLion.Shared.Enums;
using DaLion.Shared.Events;
using DaLion.Shared.Exceptions;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class StabbingSwordSpecialUpdateTickingEvent : UpdateTickingEvent
{
    private static int _currentFrame = -1;
    private static int _animationFrames;

    /// <summary>Initializes a new instance of the <see cref="StabbingSwordSpecialUpdateTickingEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal StabbingSwordSpecialUpdateTickingEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnEnabled()
    {
        var user = Game1.player;
        var sword = (MeleeWeapon)user.CurrentTool;
        Reflector
            .GetUnboundMethodDelegate<Action<MeleeWeapon, Farmer>>(sword, "beginSpecialMove")
            .Invoke(sword, user);

        var facingDirection = (FacingDirection)user.FacingDirection;
        var facingVector = facingDirection.ToVector();
        if (facingDirection.IsVertical())
        {
            facingVector *= -1f; // for some reason up and down are inverted
        }

        var trajectory = facingVector * (20f + (Game1.player.addedSpeed * 2f)) *
                         (sword.hasEnchantmentOfType<InfinityEnchantment>()
                             ? 1.5f
                             : 1.2f);
        user.setTrajectory(trajectory);

        _animationFrames = sword.hasEnchantmentOfType<InfinityEnchantment>()
                ? 24
                : 16; // translates exactly to (6 tiles : 4 tiles) with 0 added speed
        var frame = (FacingDirection)user.FacingDirection switch
        {
            FacingDirection.Up => 276,
            FacingDirection.Right => 274,
            FacingDirection.Down => 272,
            FacingDirection.Left => 278,
            _ => ThrowHelperExtensions.ThrowUnexpectedEnumValueException<FacingDirection, int>(
                (FacingDirection)user.FacingDirection),
        };

        user.FarmerSprite.setCurrentFrame(frame, 0, 15, 2, user.FacingDirection == 3, true);
        user.currentLocation.playSound(sword.CurrentParentTileIndex == WeaponIds.LavaKatana ? "fireball" : "daggerswipe");
        this.Manager.Enable<StabbingSwordSpecialInterruptedButtonPressedEvent>();
        if (CombatModule.Config.FaceMouseCursor)
        {
            this.Manager.Enable<StabbingSwordSpecialHomingUpdateTickedEvent>();
        }
    }

    /// <inheritdoc />
    protected override void OnDisabled()
    {
        var user = Game1.player;
        user.completelyStopAnimatingOrDoingAction();
        user.setTrajectory(Vector2.Zero);
        user.forceCanMove();
        user.FarmerSprite.currentAnimationIndex = 0;
        _currentFrame = 0;
        this.Manager.Disable<StabbingSwordSpecialHomingUpdateTickedEvent>();
        this.Manager.Disable<StabbingSwordSpecialInterruptedButtonPressedEvent>();
    }

    /// <inheritdoc />
    protected override void OnUpdateTickingImpl(object? sender, UpdateTickingEventArgs e)
    {
        var user = Game1.player;
        var sword = (MeleeWeapon)user.CurrentTool;

        // check for warps to prevent out-of-bounds
        var nextPosition = user.nextPosition(user.FacingDirection);
        if (user.currentLocation.isCollidingWithWarp(nextPosition, user) is { } warp)
        {
            user.DoStabbingSpecialCooldown(sword);
            this.Disable();
            user.warpFarmer(warp);
            return;
        }

        if (++_currentFrame > _animationFrames)
        {
            user.DoStabbingSpecialCooldown(sword);
            this.Disable();
        }
        else
        {
            var sprite = user.FarmerSprite;
            if (_currentFrame == 1)
            {
                sprite.currentAnimationIndex++;
            }
            else if (_currentFrame == _animationFrames - 1)
            {
                sprite.currentAnimationIndex--;
            }

            sprite.CurrentFrame = sprite.CurrentAnimation[sprite.currentAnimationIndex % sprite.CurrentAnimation.Count]
                .frame;

            var (x, y) = user.getUniformPositionAwayFromBox(user.FacingDirection, 48);
            sword.DoDamage(user.currentLocation, (int)x, (int)y, user.FacingDirection, 1, user);
            sword.isOnSpecial = true;
        }
    }
}
