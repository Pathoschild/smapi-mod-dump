/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Stardew.Arsenal.Framework.Events;

#region using directives

using Common.Enums;
using Common.Events;
using Common.Exceptions;
using Common.Extensions.Reflection;
using Enchantments;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley.Tools;
using System;

#endregion using directives

[UsedImplicitly]
internal sealed class StabbySwordSpecialUpdateTickingEvent : UpdateTickingEvent
{
    private static readonly Lazy<Action<MeleeWeapon, Farmer>> _BeginSpecialMove = new(() =>
        typeof(MeleeWeapon).RequireMethod("beginSpecialMove").CompileUnboundDelegate<Action<MeleeWeapon, Farmer>>());

    private static int _currentFrame = -1, _animationFrames;

    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal StabbySwordSpecialUpdateTickingEvent(EventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    protected override void OnUpdateTickingImpl(object? sender, UpdateTickingEventArgs e)
    {
        var user = Game1.player;
        var sword = (MeleeWeapon)user.CurrentTool;
        ++_currentFrame;
        if (_currentFrame == 0)
        {
            _BeginSpecialMove.Value(sword, user);

            var trajectory = Common.Utility.VectorFromFacingDirection((FacingDirection) user.FacingDirection) *
                             (25f + Game1.player.addedSpeed * 2.5f);
            user.setTrajectory(trajectory);

            _animationFrames = sword.hasEnchantmentOfType<InfinityEnchantment>() ? 24 : 15; // don't ask me why but this translated exactly to (5 tiles : 4 tiles)
            var frame = (FacingDirection)user.FacingDirection switch
            {
                FacingDirection.Up => 276,
                FacingDirection.Right => 274,
                FacingDirection.Down => 272,
                FacingDirection.Left => 278,
                _ => ThrowHelperExtensions.ThrowUnexpectedEnumValueException<FacingDirection, int>(
                    (FacingDirection)user.FacingDirection)
            };

            user.FarmerSprite.setCurrentFrame(frame, 0, 15, 2, user.FacingDirection == 3, true);
            Game1.playSound("daggerswipe");
        }
        else if (_currentFrame > _animationFrames)
        {
            user.completelyStopAnimatingOrDoingAction();
            user.setTrajectory(Vector2.Zero);
            user.forceCanMove();
#if RELEASE
            MeleeWeapon.attackSwordCooldown = MeleeWeapon.attackSwordCooldownTime;
            if (ModEntry.ProfessionsApi is null && user.professions.Contains(Farmer.acrobat)) MeleeWeapon.attackSwordCooldown /= 2;
            if (sword.hasEnchantmentOfType<ArtfulEnchantment>()) MeleeWeapon.attackSwordCooldown /= 2;
            if (sword.hasEnchantmentOfType<GarnetEnchantment>())
                MeleeWeapon.attackSwordCooldown = (int) (MeleeWeapon.attackSwordCooldown *
                                                         (1f - sword.GetEnchantmentLevel<TopazEnchantment>() * 0.1f));
#endif
            _currentFrame = -1;
            Disable();
        }
        else
        {
            var sprite = user.FarmerSprite;
            if (_currentFrame == 1) ++sprite.currentAnimationIndex;
            else if (_currentFrame == _animationFrames - 1) --sprite.currentAnimationIndex;

            sprite.CurrentFrame = sprite.CurrentAnimation[sprite.currentAnimationIndex].frame;

            var (x, y) = user.getUniformPositionAwayFromBox(user.FacingDirection, 48);
            sword.DoDamage(user.currentLocation, (int)x, (int)y, user.FacingDirection, 1, user);
            sword.isOnSpecial = true;
        }
    }
}