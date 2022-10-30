/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using System.Collections.Generic;
using BirbShared.Asset;
using Microsoft.Xna.Framework.Graphics;

namespace SocializingSkill
{
    [AssetClass]
    internal class Assets
    {
        [AssetProperty("assets/iconA.png")]
        public Texture2D IconA { get; set; }
        [AssetProperty("assets/iconB.png")]
        public Texture2D IconB { get; set; }


        [AssetProperty("assets/friendly.png")]
        public Texture2D Friendly { get; set; }
        [AssetProperty("assets/smoothtalker.png")]
        public Texture2D SmoothTalker { get; set; }
        [AssetProperty("assets/gifter.png")]
        public Texture2D Gifter { get; set; }
        [AssetProperty("assets/helpful.png")]
        public Texture2D Helpful { get; set; }
        [AssetProperty("assets/haggler.png")]
        public Texture2D Haggler { get; set; }
        [AssetProperty("assets/beloved.png")]
        public Texture2D Beloved { get; set; }


        [AssetProperty("assets/belovedtable.json")]
        public Dictionary<string, List<string>> BelovedTable { get; set; }
    }
}
