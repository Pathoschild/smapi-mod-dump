/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/modular-overhaul
**
*************************************************/

namespace DaLion.Overhaul.Modules.Core.StatusEffects;

#region using directives

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DaLion.Overhaul.Modules.Core.Events;
using DaLion.Overhaul.Modules.Core.Extensions;
using DaLion.Shared.Extensions;
using Microsoft.Xna.Framework;
using StardewValley.Monsters;

#endregion using directives

/// <summary>The animation that plays above a burning <see cref="Monster"/>.</summary>
public class BurnAnimation : TemporaryAnimatedSprite
{
    private readonly int _segmentIndex = -1;

    /// <summary>Initializes a new instance of the <see cref="BurnAnimation"/> class.</summary>
    /// <param name="monster">The stunned <see cref="Monster"/>.</param>
    /// <param name="duration">The duration in milliseconds.</param>
    public BurnAnimation(Monster monster, int duration)
        : base(
            30,
            monster.Position,
            Color.White,
            4,
            Game1.random.NextBool(),
            50f,
            duration / 200)
    {
        this.Position = monster.GetOverheadOffset() - new Vector2(0, 8f);
        this.positionFollowsAttachedCharacter = true;
        this.attachedCharacter = monster;
        this.layerDepth = 999999f;
        EventManager.Enable<BurnAnimationUpdateTickedEvent>();
        EventManager.Enable<BurnAnimationRenderedWorldEvent>();
    }

    /// <summary>Initializes a new instance of the <see cref="BurnAnimation"/> class for a royal <see cref="Serpent"/>.</summary>
    /// <param name="royal">The stunned <see cref="Serpent"/>.</param>
    /// <param name="duration">The duration in milliseconds.</param>
    /// <param name="segmentIndex">The index of the <paramref name="royal"/> <see cref="Serpent"/> segment to which this instance should be attached.</param>
    public BurnAnimation(Serpent royal, int duration, int segmentIndex)
        : base(
            30,
            royal.position,
            Color.White,
            4,
            Game1.random.NextBool(),
            50f,
            duration / 200)
    {
        this.positionFollowsAttachedCharacter = true;
        this.attachedCharacter = royal;
        this.layerDepth = 999999f;
        this._segmentIndex = segmentIndex;
        EventManager.Enable<BurnAnimationUpdateTickedEvent>();
        EventManager.Enable<BurnAnimationRenderedWorldEvent>();
    }

    internal static ConditionalWeakTable<Monster, List<BurnAnimation>> BurnAnimationsByMonster { get; } = new();

    /// <inheritdoc />
    public override bool update(GameTime time)
    {
        var result = base.update(time);
        var monster = (Monster)this.attachedCharacter;
        if (result || monster.Health <= 0 || !monster.IsBurning())
        {
            BurnAnimationsByMonster.Remove(monster);
            return result;
        }

        if (this.attachedCharacter is Serpent serpent && this._segmentIndex >= 0)
        {
            var segment = serpent.segments[this._segmentIndex];
            this.Position = new Vector2(segment.X + 32f, segment.Y) - serpent.Position;
            return result;
        }

        this.Position = monster.GetOverheadOffset() - new Vector2(0, 8f);
        return result;
    }
}
