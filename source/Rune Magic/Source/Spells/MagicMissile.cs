/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/facufierro/RuneMagic
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using RuneMagic.Source.Effects;
using SpaceCore;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RuneMagic.Source.Spells
{
    public class MagicMissile : Spell
    {
        public MagicMissile() : base(School.Evocation)

        {
            Name = "Magic Missile";
            Description += "Shoots a magic missile per two magic skill levels.";
            Level = 1;
        }

        public override bool Cast()
        {
            if (!Player.MagicStats.ActiveEffects.OfType<CastingMagicMissile>().Any())
            {
                Effect = new CastingMagicMissile(this);
                return base.Cast();
            }
            else
                return false;
        }
    }
}