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

namespace SpookySkill
{
    [SAsset(Priority = 0)]
    public class Assets
    {
        [SAsset.Asset("assets/scare/SpookyiconA.png")]
        public Texture2D IconA_Scary { get; set; }

        [SAsset.Asset("assets/scare/SpookyiconB.png")]
        public Texture2D IconB_Scary { get; set; }

        [SAsset.Asset("assets/scare/Spooky5a.png")]
        public Texture2D Spooky5a_Scary { get; set; }
        [SAsset.Asset("assets/scare/Spooky5b.png")]
        public Texture2D Spooky5b_Scary { get; set; }
        [SAsset.Asset("assets/scare/Spooky10a1.png")]
        public Texture2D Spooky10a1_Scary { get; set; }
        [SAsset.Asset("assets/scare/Spooky10a2.png")]
        public Texture2D Spooky10a2_Scary { get; set; }
        [SAsset.Asset("assets/scare/Spooky10b1.png")]
        public Texture2D Spooky10b1_Scary { get; set; }
        [SAsset.Asset("assets/scare/Spooky10b2.png")]
        public Texture2D Spooky10b2_Scary { get; set; }


        [SAsset.Asset("assets/thief/SpookyiconA.png")]
        public Texture2D IconA_Thief { get; set; }

        [SAsset.Asset("assets/thief/SpookyiconB1.png")]
        public Texture2D IconB_Thief_1 { get; set; }

        [SAsset.Asset("assets/thief/SpookyiconB2.png")]
        public Texture2D IconB_Thief_2 { get; set; }

        [SAsset.Asset("assets/thief/SpookyiconB3.png")]
        public Texture2D IconB_Thief_3 { get; set; }

        [SAsset.Asset("assets/thief/Spooky5a.png")]
        public Texture2D Spooky5a_Thief { get; set; }
        [SAsset.Asset("assets/thief/Spooky5b.png")]
        public Texture2D Spooky5b_Thief { get; set; }
        [SAsset.Asset("assets/thief/Spooky10a1.png")]
        public Texture2D Spooky10a1_Thief { get; set; }
        [SAsset.Asset("assets/thief/Spooky10a2.png")]
        public Texture2D Spooky10a2_Thief { get; set; }
        [SAsset.Asset("assets/thief/Spooky10b1.png")]
        public Texture2D Spooky10b1_Thief { get; set; }
        [SAsset.Asset("assets/thief/Spooky10b2.png")]
        public Texture2D Spooky10b2_Thief { get; set; }
    }
}
