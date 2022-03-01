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
    class NullEffect : IEffect
    {            
        public List<EffectDescriptionLine> EffectDescription => new List<EffectDescriptionLine>() { new EffectDescriptionLine(EffectIcon.None, "Does nothing") };

        public void Apply(Item sourceItem, EffectChangeReason reason)
        {
            // --
        }        

        public void Remove(Item sourceItem, EffectChangeReason reason)
        {
            // --
        }
    }
}
