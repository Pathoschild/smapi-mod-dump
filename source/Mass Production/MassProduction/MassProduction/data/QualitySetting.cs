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
    /// What quality of output does the machine produce? Overrides definitions in base machine's PFM settings.
    /// </summary>
    public enum QualitySetting
    {
        NoStars = 0, Silver = 1, Gold = 2, Iridium = 3, KeepInput = -1
    }
}
