/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LunaticShade/StardewValley.SkillfulClothes
**
*************************************************/

using SkillfulClothes.Types;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillfulClothes.Effects
{
    /*
     * A set of effects
     */
    class EffectSet : IEffect
    {

        public IEffect[] Effects { get; }        

        List<EffectDescriptionLine> IEffect.EffectDescription => Effects.SelectMany(x => x.EffectDescription).ToList();        

        private EffectSet(params IEffect[] effects)
        {
            Effects = effects;
        }

        public static EffectSet Of(params IEffect[] effects)
        {
            return new EffectSet(effects);
        }

        public void Apply(Item sourceItem, EffectChangeReason reason)
        {
            foreach(var effect in Effects)
            {
                effect.Apply(sourceItem, reason);
            }
        }

        public void Remove(Item sourceItem, EffectChangeReason reason)
        {
            foreach (var effect in Effects)
            {
                effect.Remove(sourceItem, reason);
            }
        }
    }
}
