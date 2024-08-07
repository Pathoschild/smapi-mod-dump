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
    public interface IEffect
    {
        string EffectId { get; }

        List<EffectDescriptionLine> EffectDescription { get; }
        void Apply(Item sourceItem, EffectChangeReason reason);
        void Remove(Item sourceItem, EffectChangeReason reason);        
    }
}
