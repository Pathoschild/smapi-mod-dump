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

using System.Runtime.CompilerServices;
using DaLion.Overhaul.Modules.Core.Events;
using DaLion.Overhaul.Modules.Core.Extensions;
using DaLion.Shared.Extensions;
using Microsoft.Xna.Framework;
using StardewValley.Monsters;

#endregion using directives

/// <summary>The animation that plays above poisoned <see cref="Monster"/>.</summary>
public class PoisonAnimation : TemporaryAnimatedSprite
{
    /// <summary>Initializes a new instance of the <see cref="PoisonAnimation"/> class.</summary>
    /// <param name="monster">The stunned <see cref="Monster"/>.</param>
    /// <param name="duration">The duration in milliseconds.</param>
    public PoisonAnimation(Monster monster, int duration)
        : base(
            $"{Manifest.UniqueID}/PoisonAnimation",
            new Rectangle(0, 0, 64, 128),
            50f,
            6,
            duration / 300,
            monster.Position,
            false,
            Game1.random.NextBool())
    {
        this.Position = new Vector2(0f, -64f);
        this.positionFollowsAttachedCharacter = true;
        this.attachedCharacter = monster;
        this.layerDepth = 999999f;
        EventManager.Enable<PoisonAnimationUpdateTickedEvent>();
        EventManager.Enable<PoisonAnimationRenderedWorldEvent>();
    }

    internal static ConditionalWeakTable<Monster, PoisonAnimation> PoisonAnimationByMonster { get; } = new();

    /// <inheritdoc />
    public override bool update(GameTime time)
    {
        var result = base.update(time);
        var monster = (Monster)this.attachedCharacter;
        if (result || monster.Health <= 0 || !monster.IsPoisoned())
        {
            PoisonAnimationByMonster.Remove(monster);
            return result;
        }

        this.Position = new Vector2(0f, -64f);
        return result;
    }
}
