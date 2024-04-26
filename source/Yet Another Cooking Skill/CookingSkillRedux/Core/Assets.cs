/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pet-Slime/StardewValley
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using BirbCore.Attributes;

namespace CookingSkill
{
    [SAsset(Priority = 0)]
    public class Assets
    {
        [SAsset.Asset("assets/cookingiconA.png")]
        public Texture2D IconA { get; set; }

        [SAsset.Asset("assets/cookingiconB.png")]
        public Texture2D IconB { get; set; }

        [SAsset.Asset("assets/cooking5a.png")]
        public Texture2D Cooking5a { get; set; }

        [SAsset.Asset("assets/cooking5b.png")]
        public Texture2D Cooking5b { get; set; }

        [SAsset.Asset("assets/cooking10a1.png")]
        public Texture2D Cooking10a1 { get; set; }

        [SAsset.Asset("assets/cooking10a2.png")]
        public Texture2D Cooking10a2 { get; set; }

        [SAsset.Asset("assets/cooking10b1.png")]
        public Texture2D Cooking10b1 { get; set; }

        [SAsset.Asset("assets/cooking10b2.png")]
        public Texture2D Cooking10b2 { get; set; }


        [SAsset.Asset("assets/random_buff.png")]
        public Texture2D Random_Buff { get; set; }

    }
}
