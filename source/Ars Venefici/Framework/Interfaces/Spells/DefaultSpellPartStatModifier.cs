/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/HeyImAmethyst/Ars-Venefici
**
*************************************************/

using ArsVenefici.Framework.Spells;
using ArsVenefici.Framework.Spells.Shape;
using ArsVenefici.Framework.Util;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StardewValley.Minigames.TargetGame;

namespace ArsVenefici.Framework.Interfaces.Spells
{
    public class DefaultSpellPartStatModifier : ISpellPartStatModifier
    {
        
        public static ISpellPartStatModifier NOOP = new ISpellPartStatModifier((baseValue, modified, spell, caster, target, componentIndex) => modified);
        public static ISpellPartStatModifier COUNTING = new ISpellPartStatModifier((baseValue, modified, spell, caster, target, componentIndex) => modified + 1);

        public DefaultSpellPartStatModifier(SpellPartStatModifier p) : base(p)
        {

        }

        public static ISpellPartStatModifier Add(float value)
        {
            return new ISpellPartStatModifier((baseValue, modified, spell, caster, target, componentIndex) => modified + value);
        }

        public static ISpellPartStatModifier Multiply(float value)
        {
            return new ISpellPartStatModifier((baseValue, modified, spell, caster, target, componentIndex) => modified * value);
        }

        public static ISpellPartStatModifier AddMultipliedBase(float value)
        {
            return new ISpellPartStatModifier((baseValue, modified, spell, caster, target, componentIndex) => modified + baseValue * value);
        }

        public static ISpellPartStatModifier SubtractMultipliedBase(float value)
        {
            return new ISpellPartStatModifier((baseValue, modified, spell, caster, target, componentIndex) => modified - baseValue * value);
        }

        public static ISpellPartStatModifier AddMultiplied(float value)
        {
            return new ISpellPartStatModifier((baseValue, modified, spell, caster, target, componentIndex) => modified + modified * value);
        }

        public static ISpellPartStatModifier SubtractMultiplied(float value)
        {
            return new ISpellPartStatModifier((baseValue, modified, spell, caster, target, componentIndex) => modified - modified * value);
        }
    }
}
