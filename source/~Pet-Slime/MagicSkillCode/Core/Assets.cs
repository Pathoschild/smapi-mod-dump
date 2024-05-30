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

namespace MagicSkillCode.Core
{
    [SAsset(Priority = 0)]
    public class Assets
    {
        [SAsset.Asset("assets/interface/MagiciconA.png")]
        public Texture2D IconA { get; set; }

        [SAsset.Asset("assets/interface/MagiciconB.png")]
        public Texture2D IconB { get; set; }

        [SAsset.Asset("assets/interface/Magic5a.png")]
        public Texture2D Magic5a { get; set; }

        [SAsset.Asset("assets/interface/Magic5b.png")]
        public Texture2D Magic5b { get; set; }

        [SAsset.Asset("assets/interface/Magic10a1.png")]
        public Texture2D Magic10a1 { get; set; }

        [SAsset.Asset("assets/interface/Magic10a2.png")]
        public Texture2D Magic10a2 { get; set; }

        [SAsset.Asset("assets/interface/Magic10b1.png")]
        public Texture2D Magic10b1 { get; set; }

        [SAsset.Asset("assets/interface/Magic10b2.png")]
        public Texture2D Magic10b2 { get; set; }



        [SAsset.Asset("assets/entities/cloud.png")]
        public Texture2D CloudMount { get; set; }

        [SAsset.Asset("assets/magic/arcane/school-icon.png")]
        public Texture2D ArcaneSchoolIcon { get; set; }

        [SAsset.Asset("assets/magic/arcane/analyze/1.png")]
        public Texture2D ArcaneSchoolAnalyze1 { get; set; }


    }
}
