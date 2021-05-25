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
    abstract class SingleEffect : IEffect
    {
        List<EffectDescriptionLine> effectDescription;
        public List<EffectDescriptionLine> EffectDescription
        {
            get
            {
                if (effectDescription == null)
                {
                    effectDescription = new List<EffectDescriptionLine>() { GenerateEffectDescription() };
                }

                return effectDescription;
            }
        }        

        public abstract void Apply(Item sourceItem, EffectChangeReason reason);

        public abstract void Remove(Item sourceItem, EffectChangeReason reason);

        protected abstract EffectDescriptionLine GenerateEffectDescription();        
    }
}
