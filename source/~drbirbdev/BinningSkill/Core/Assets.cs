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

namespace BinningSkill
{
    [AssetClass]
    public class Assets
    {
        [AssetProperty("assets/binningiconA.png")]
        public Texture2D IconA { get; set; }
        [AssetProperty("assets/binningiconB.png")]
        public Texture2D IconB { get; set; }


        [AssetProperty("assets/environmentalist.png")]
        public Texture2D Environmentalist { get; set; }
        [AssetProperty("assets/reclaimer.png")]
        public Texture2D Reclaimer { get; set; }
        [AssetProperty("assets/recycler.png")]
        public Texture2D Recycler { get; set; }
        [AssetProperty("assets/salvager.png")]
        public Texture2D Salvager { get; set; }
        [AssetProperty("assets/sneak.png")]
        public Texture2D Sneak { get; set; }
        [AssetProperty("assets/upseller.png")]
        public Texture2D Upseller { get; set; }

        [AssetProperty("assets/trashtable.json")]
        public Dictionary<string, List<string>> TrashTable { get; set; }

        [AssetProperty("assets/salvagertable.json")]
        public Dictionary<string, List<string>> SalvagerTable { get; set; }
    }
}
