/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Enaium-StardewValleyMods/Labeling
**
*************************************************/

using System.Collections.Generic;
using StardewModdingAPI;

namespace Labeling.Framework
{
    public class Config
    {
        public SButton OpenLabelScreen { get; set; } = SButton.J;
        public List<Labeling> Labelings { get; set; } = new List<Labeling>();
    }
}