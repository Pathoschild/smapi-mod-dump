/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JacquePott/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassProduction
{
    /// <summary>
    /// Whether or not the machine upgrade supports machines with inputs or machines with no inputs.
    /// </summary>
    public enum InputRequirement
    {
        InputRequired, NoInputsOnly, NoRequirements
    }
}
