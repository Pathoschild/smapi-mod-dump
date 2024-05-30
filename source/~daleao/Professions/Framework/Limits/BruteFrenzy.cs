/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Professions.Framework.Limits;

#region using directives

using DaLion.Professions.Framework.Buffs;
using Microsoft.Xna.Framework;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="BruteFrenzy"/> class.</summary>
public sealed class BruteFrenzy()
    : LimitBreak(Profession.Brute, "Frenzy", Color.OrangeRed, Color.OrangeRed)
{
    /// <summary>Gets or sets the number of enemies defeated while active.</summary>
    internal int KillCount { get; set; }

    /// <inheritdoc />
    internal override void Activate()
    {
        base.Activate();
        this.KillCount = 0;
        Game1.player.applyBuff(new BruteFrenzyBuff());
    }

    /// <inheritdoc />
    internal override void Deactivate()
    {
        base.Deactivate();
        Game1.player.buffs.AppliedBuffs.Remove(PoacherAmbushBuff.ID);

        var who = Game1.player;
        var healed = (int)(who.maxHealth * this.KillCount * 0.05f);
        who.health = Math.Min(who.health + healed, who.maxHealth);
        who.currentLocation.debris.Add(new Debris(
            healed,
            new Vector2(who.StandingPixel.X + 8, who.StandingPixel.Y),
            Color.Lime,
            1f,
            who));
        Game1.playSound("healSound");
    }

    /// <inheritdoc />
    internal override void Countdown()
    {
        // base duration 15 s * 60 fps = 900 frames
        this.ChargeValue -= BASE_MAX_CHARGE / 900d;
    }
}
