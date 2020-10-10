/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Felix-Dev/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FelixDev.StardewMods.FeTK.Framework.Services
{
    /// <summary>
    /// Defines constants that specify the type of recipe attached to a <see cref="RecipeMail"/>.
    /// </summary>
    public enum RecipeType
    {
        /// <summary>A cooking recipe.</summary>
        Cooking = 0,
        /// <summary>A crafting recipe.</summary>
        Crafting
    }
}
