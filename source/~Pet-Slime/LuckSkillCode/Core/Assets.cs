/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pet-Slime/StardewValley
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BirbCore.Attributes;
using Microsoft.Xna.Framework.Graphics;

namespace LuckSkill
{
    [SAsset(Priority = 0)]
    public class Assets
    {
        [SAsset.Asset("assets/LuckiconA.png")]
        public Texture2D IconA { get; set; }

        [SAsset.Asset("assets/LuckiconB.png")]
        public Texture2D IconB { get; set; }

        [SAsset.Asset("assets/Luck5a.png")]
        public Texture2D Luck5a { get; set; }
        [SAsset.Asset("assets/Luck5b.png")]
        public Texture2D Luck5b { get; set; }
        [SAsset.Asset("assets/Luck10a1.png")]
        public Texture2D Luck10a1 { get; set; }
        [SAsset.Asset("assets/Luck10a2.png")]
        public Texture2D Luck10a2 { get; set; }
        [SAsset.Asset("assets/Luck10b1.png")]
        public Texture2D Luck10b1 { get; set; }
        [SAsset.Asset("assets/Luck10b2.png")]
        public Texture2D Luck10b2 { get; set; }
    }
}
