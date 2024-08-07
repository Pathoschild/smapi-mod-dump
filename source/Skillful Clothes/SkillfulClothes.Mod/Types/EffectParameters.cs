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
    /**
     * Interface for parameters of effects
     **/
    public interface IEffectParameters
    {
        
    }    

    public class NoEffectParameters : IEffectParameters
    {
        public static NoEffectParameters Default = new NoEffectParameters();

        // --
    }
}