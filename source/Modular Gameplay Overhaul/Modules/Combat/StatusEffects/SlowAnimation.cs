/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/modular-overhaul
**
*************************************************/

namespace DaLion.Overhaul.Modules.Combat.StatusEffects;

#region using directives

using System.Runtime.CompilerServices;
using DaLion.Overhaul.Modules.Combat.Events.Display.RenderedWorld;
using DaLion.Overhaul.Modules.Combat.Events.GameLoop.UpdateTicked;
using DaLion.Overhaul.Modules.Combat.Extensions;
using Microsoft.Xna.Framework;
using StardewValley.Monsters;

#endregion using directives

/// <summary>The animation that plays above a slowed <see cref="Monster"/>.</summary>
public class SlowAnimation : TemporaryAnimatedSprite
{
    /// <summary>Initializes a new instance of the <see cref="SlowAnimation"/> class.</summary>
    /// <param name="monster">The Slowned <see cref="Monster"/>.</param>
    /// <param name="duration">The duration in milliseconds.</param>
    public SlowAnimation(Monster monster, int duration)
        : base(
            $"{Manifest.UniqueID}/SlowAnimation",
            new Rectangle(0, 0, 16, 16),
            500,
            4,
            duration / 2000,
            Vector2.Zero,
            false,
            false,
            999999f,
            0f,
            Color.White,
            4f,
            0f,
            0f,
            0f)
    {
        this.positionFollowsAttachedCharacter = true;
        this.attachedCharacter = monster;
        EventManager.Enable<SlowAnimationRenderedWorldEvent>();
        EventManager.Enable<SlowAnimationUpdateTickedEvent>();
    }

    internal static ConditionalWeakTable<Monster, SlowAnimation> SlowAnimationByMonster { get; } = new();

    /// <inheritdoc />
    public override bool update(GameTime time)
    {
        var result = base.update(time);
        var monster = (Monster)this.attachedCharacter;
        if (result || monster.Health <= 0)
        {
            SlowAnimationByMonster.Remove(monster);
            return result;
        }

        var offset = monster.GetOverheadOffset(time);
        offset.Y -= 32;
        this.Position = offset;
        return result;
    }
}
