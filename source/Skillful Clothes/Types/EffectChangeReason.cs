/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LunaticShade/StardewValley.SkillfulClothes
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillfulClothes.Types
{
    /// <summary>
    /// States why an effect should be applied or removed
    /// </summary>
    public enum EffectChangeReason
    {
        ItemPutOn,
        ItemRemoved,
        DayEnd,
        DayStart,

        Reset
    }
}
