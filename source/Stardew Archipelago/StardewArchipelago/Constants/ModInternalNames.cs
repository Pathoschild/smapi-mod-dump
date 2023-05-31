/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewArchipelago.Constants
{
    public static class ModInternalNames
    {
        public static readonly Dictionary<string, string> InternalNames = new Dictionary<string, string>()
        {
            { ModNames.ARCHAEOLOGY, "ExcavationSkill" },
            { ModNames.JUNA, "NPC Juna" },
            { ModNames.JASPER, "NPC Jasper" },
            { ModNames.ALEC, "NPC Alec Revisited" },
            { ModNames.YOBA, "[CP] Yoba NPC" },
            { ModNames.EUGENE, "Eugene NPC Eng Translation" },
            { ModNames.WELLWICK, "[CP] Wellwick" },
            { ModNames.MISTER_GINGER, "NPC Mr Ginger" },
            { ModNames.SHIKO, "Papaya's Custom NPC Mod" },
            { ModNames.DELORES, "Delores" },
            { ModNames.AYEISHA, "Ayeisha" },
        };
    }
}
