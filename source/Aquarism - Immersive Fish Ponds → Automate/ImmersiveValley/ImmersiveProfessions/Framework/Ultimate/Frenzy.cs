/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

// ReSharper disable PossibleLossOfFraction
namespace DaLion.Stardew.Professions.Framework.Ultimate;

#region using directives

using System;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Monsters;

using Sounds;

#endregion using directives

/// <summary>Handles Brute ultimate activation.</summary>
internal sealed class Frenzy : Ultimate
{
    //private double _elapsedSinceDoT;
    public const float PCT_INCREMENT_PER_RAGE_F = 0.01f;

    /// <summary>Construct an instance.</summary>
    internal Frenzy()
    : base(Color.OrangeRed, Color.OrangeRed)
    {
    }

    #region public properties

    /// <summary>The ID of the buff that displays while Frenzy is active.</summary>
    public static int BuffId { get; } = ModEntry.Manifest.UniqueID.GetHashCode() + (int) UltimateIndex.Frenzy + 4;

    /// <inheritdoc />
    public override UltimateIndex Index => UltimateIndex.Frenzy;

    #endregion public properties

    #region internal properties

    /// <inheritdoc />
    internal override SFX ActivationSfx => SFX.BruteRage;

    /// <inheritdoc />
    internal override Color GlowColor => Color.OrangeRed;

    #endregion internal properties

    #region internal methods

    /// <inheritdoc />
    internal override void Activate()
    {
        base.Activate();

        // fear
        foreach (var monster in Game1.currentLocation.characters.OfType<Monster>()
                     .Where(m => m.Player.IsLocalPlayer))
            monster.stunTime = 1000;

        ModEntry.PlayerState.BruteKillCounter = 0;

        Game1.buffsDisplay.removeOtherBuff(BuffId);
        Game1.buffsDisplay.addOtherBuff(
            new(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                1,
                GetType().Name,
                ModEntry.ModHelper.Translation.Get("brute.ulti"))
            {
                which = BuffId,
                sheetIndex = 48,
                glow = GlowColor,
                millisecondsDuration = (int) (15000 * ((double) MaxValue / BASE_MAX_VALUE_I) / ModEntry.Config.UltimateDrainFactor),
                description = ModEntry.ModHelper.Translation.Get("brute.ultidesc")
            }
        );
    }

    /// <inheritdoc />
    internal override void Deactivate()
    {
        base.Deactivate();

        Game1.buffsDisplay.removeOtherBuff(BuffId);

        var who = Game1.player;
        var healed = (int) (who.maxHealth * ModEntry.PlayerState.BruteKillCounter * 0.05f);
        who.health = Math.Min(who.health + healed, who.maxHealth);
        who.currentLocation.debris.Add(new(healed,
            new(who.getStandingX() + 8, who.getStandingY()), Color.Lime, 1f, who));
    }

    /// <inheritdoc />
    internal override void Countdown(double elapsed)
    {
        ChargeValue -= elapsed * 0.12 / 18.0;
    }

    #endregion internal methods
}