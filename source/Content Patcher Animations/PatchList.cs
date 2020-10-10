/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/ContentPatcherAnimations
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentPatcherAnimations
{
    public class Patch
    {
        public string LogName; // To identify, to check if it is active
        public string Action; // To make sure this is an EditImage

        // Target and FromFile are taken from CP since it handles tokens
        // Same for FromARea and ToArea

        // MINE
        public int AnimationFrameTime = -1;
        public int AnimationFrameCount = -1;
    }
    public class PatchList
    {
        public ISemanticVersion Format;
        public List<Patch> Changes;
    }
}
