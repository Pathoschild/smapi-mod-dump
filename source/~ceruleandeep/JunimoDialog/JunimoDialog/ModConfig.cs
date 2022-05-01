/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ceruleandeep/CeruleanStardewMods
**
*************************************************/

using System.Collections.Generic;
using StardewModdingAPI;

namespace JunimoDialog
{
    internal class ModConfig
    {
        public bool Happy { get; set; } = true;
        public bool Grumpy { get; set; } = true;
        public float DialogChance { get; set; } = 0.05f;
        public float JunimoTextChance { get; set; } = 0.50f;
        public bool ExtraDebugOutput { get; set; }
    }
}
