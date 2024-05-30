/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/HeyImAmethyst/Ars-Venefici
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArsVenefici.Framework.Interfaces.Spells
{
    /// <summary>
    /// Base interface for a spell part. A spell part can be a component of type ISpellComponent, a modifier of type ISpellModifier or a shape of type ISpellShape.
    /// </summary>
    public interface ISpellPart
    {

        /// <returns>The type of this spell part.</returns>
        SpellPartType GetType();

        string GetId();

        int ManaCost();

        /// <summary>
        /// The types of the spell parts.
        /// </summary>

    }

    public enum SpellPartType
    {
        SHAPE, COMPONENT, MODIFIER
    }
}
