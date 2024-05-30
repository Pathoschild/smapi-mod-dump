/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pet-Slime/StardewValley
**
*************************************************/

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using MoonShared.Asset;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using BirbCore.Attributes;
using HarmonyLib;

namespace ArchaeologySkill
{
    [SAsset(Priority = 0)]
    public class Assets
    {

        [SAsset.Asset("assets/ArchaeologyiconA.png")]
        public Texture2D IconA { get; set; }

        [SAsset.Asset("assets/ArchaeologyiconB.png")]
        public Texture2D IconB { get; set; }

        [SAsset.Asset("assets/ArchaeologyiconBalt.png")]
        public Texture2D IconBalt { get; set; }

        [SAsset.Asset("assets/Archaeology5a.png")]
        public Texture2D Archaeology5a { get; set; }
        [SAsset.Asset("assets/Archaeology5b.png")]
        public Texture2D Archaeology5b { get; set; }
        [SAsset.Asset("assets/Archaeology10a1.png")]
        public Texture2D Archaeology10a1 { get; set; }
        [SAsset.Asset("assets/Archaeology10a2.png")]
        public Texture2D Archaeology10a2 { get; set; }
        [SAsset.Asset("assets/Archaeology10b1.png")]
        public Texture2D Archaeology10b1 { get; set; }
        [SAsset.Asset("assets/Archaeology10b2.png")]
        public Texture2D Archaeology10b2 { get; set; }

        // Prestige Icons
        [SAsset.Asset("assets/Archaeology5aP.png")]
        public Texture2D Archaeology5aP { get; set; }
        [SAsset.Asset("assets/Archaeology5bP.png")]
        public Texture2D Archaeology5bP { get; set; }
        [SAsset.Asset("assets/Archaeology10a1P.png")]
        public Texture2D Archaeology10a1P { get; set; }
        [SAsset.Asset("assets/Archaeology10a2P.png")]
        public Texture2D Archaeology10a2P { get; set; }
        [SAsset.Asset("assets/Archaeology10b1P.png")]
        public Texture2D Archaeology10b1P { get; set; }
        [SAsset.Asset("assets/Archaeology10b2P.png")]
        public Texture2D Archaeology10b2P { get; set; }


        [SAsset.Asset("assets/gold_rush.png")]
        public Texture2D Gold_Rush_Buff { get; set; }



        [SAsset.Asset("assets/water_shifter.png")]
        public Texture2D Water_shifter { get; set; }




        [SAsset.Asset("assets/totem_volcano_warp.png", AssetLoadPriority.Medium)]
        public Texture2D Totem_volcano_warp { get; set; }
        public string Totem_volcano_warpPath { get; set; }



        [SAsset.Asset("assets/itemDefinitions.json")]
        public Dictionary<string, List<string>> ItemDefinitions { get; set; }




    }
}
