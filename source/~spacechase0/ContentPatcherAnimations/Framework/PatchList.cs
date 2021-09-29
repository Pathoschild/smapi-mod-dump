/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;
using StardewModdingAPI;

namespace ContentPatcherAnimations.Framework
{
    internal class PatchList
    {
        public ISemanticVersion Format;
        public List<Patch> Changes;
    }
}
