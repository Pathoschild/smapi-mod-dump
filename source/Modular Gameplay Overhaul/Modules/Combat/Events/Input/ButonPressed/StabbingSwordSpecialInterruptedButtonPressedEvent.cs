/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/modular-overhaul
**
*************************************************/

namespace DaLion.Overhaul.Modules.Combat.Events.Input.ButonPressed;

#region using directives

using DaLion.Overhaul.Modules.Combat.Events.GameLoop.UpdateTicked;
using DaLion.Shared.Enums;
using DaLion.Shared.Events;
using DaLion.Shared.Exceptions;
using DaLion.Shared.Extensions.Xna;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class StabbingSwordSpecialInterruptedButtonPressedEvent : ButtonPressedEvent
{
    /// <summary>Initializes a new instance of the <see cref="StabbingSwordSpecialInterruptedButtonPressedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal StabbingSwordSpecialInterruptedButtonPressedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnButtonPressedImpl(object? sender, ButtonPressedEventArgs e)
    {
        var user = Game1.player;
        if (user.CurrentTool is not MeleeWeapon { isOnSpecial: true })
        {
            this.Disable();
            return;
        }

        var facingDirection = (FacingDirection)user.FacingDirection;
        var angle = 0d;
        Vector2 trajectory;
        switch (e.Button)
        {
            case SButton.W or SButton.LeftThumbstickUp or SButton.DPadUp:
                if (facingDirection.IsHorizontal())
                {
                    trajectory = new Vector2(user.xVelocity, user.yVelocity);
                    angle = facingDirection switch
                    {
                        FacingDirection.Left => -90d,
                        FacingDirection.Right => 90d,
                        _ => 0d,
                    };

                    user.setTrajectory(trajectory.Rotate(angle));
                    user.FacingDirection = (int)FacingDirection.Up;
                }

                break;

            case SButton.D or SButton.LeftThumbstickRight or SButton.DPadRight:
                if (facingDirection.IsVertical())
                {
                    trajectory = new Vector2(user.xVelocity, user.yVelocity);
                    angle = facingDirection switch
                    {
                        FacingDirection.Up => -90d,
                        FacingDirection.Down => 90d,
                        _ => 0d,
                    };

                    user.setTrajectory(trajectory.Rotate(angle));
                    user.FacingDirection = (int)FacingDirection.Right;
                }

                break;

            case SButton.S or SButton.LeftThumbstickDown or SButton.DPadDown:
                if (facingDirection.IsHorizontal())
                {
                    trajectory = new Vector2(user.xVelocity, user.yVelocity);
                    angle = facingDirection switch
                    {
                        FacingDirection.Left => 90d,
                        FacingDirection.Right => -90d,
                        _ => 0d,
                    };

                    user.setTrajectory(trajectory.Rotate(angle));
                    user.FacingDirection = (int)FacingDirection.Down;
                }

                break;

            case SButton.A or SButton.LeftThumbstickLeft or SButton.DPadLeft:
                if (facingDirection.IsVertical())
                {
                    trajectory = new Vector2(user.xVelocity, user.yVelocity);
                    angle = facingDirection switch
                    {
                        FacingDirection.Up => 90d,
                        FacingDirection.Down => -90d,
                        _ => 0d,
                    };

                    user.setTrajectory(trajectory.Rotate(angle));
                    user.FacingDirection = (int)FacingDirection.Left;
                }

                break;
        }

        if (angle == 0)
        {
            return;
        }

        var frame = (FacingDirection)user.FacingDirection switch
        {
            FacingDirection.Up => 276,
            FacingDirection.Right => 274,
            FacingDirection.Down => 272,
            FacingDirection.Left => 278,
            _ => ThrowHelperExtensions.ThrowUnexpectedEnumValueException<FacingDirection, int>(
                (FacingDirection)user.FacingDirection),
        };

        var sprite = user.FarmerSprite;
        sprite.setCurrentFrame(frame, 0, 15, 2, user.FacingDirection == 3, true);
        sprite.currentAnimationIndex++;
        sprite.CurrentFrame =
            sprite.CurrentAnimation[sprite.currentAnimationIndex % sprite.CurrentAnimation.Count].frame;
        this.Manager.Disable<StabbingSwordSpecialHomingUpdateTickedEvent>();
        this.Disable();
    }
}
