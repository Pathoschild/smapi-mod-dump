/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LunaticShade/StardewValley.SkillfulClothes
**
*************************************************/

using SkillfulClothes.Effects;
using SkillfulClothes.Types;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillfulClothes
{
#if DEBUG
    /// <summary>
    /// A Helper effect with arbitrary effects used for debugging
    /// </summary>
    class DebugEffect : ParameterlessSingleEffect
    {
        List<EffectDescriptionLine> effectDescription = new List<EffectDescriptionLine>() { new EffectDescriptionLine(EffectIcon.Money, "Arbitrary effects used for debugging") };

        public override List<EffectDescriptionLine> EffectDescription => effectDescription;

        public override void Apply(Item sourceItem, EffectChangeReason reason)
        {
            
        }

        public override void Remove(Item sourceItem, EffectChangeReason reason)
        {
            
        }
    }
#endif
}
