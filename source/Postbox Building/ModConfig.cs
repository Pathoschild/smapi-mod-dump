/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/i-saac-b/PostBoxMod
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace PostBoxMod
{
    public sealed class ModConfig
    {
        public float PostedGiftRelationshipModifier { get; set; } = 0.75f;
        public int PostboxCost { get; set; } = 2500;
        public int PostboxBuildTime { get; set; } = 2;
        public string PostboxMaterialCost { get; set; } = "Normal"; // "Free", "Expensive", "Endgame", "Custom"
        public string CustomPostboxMaterialCost { get; set; } = "";
        public bool VerboseGifting { get; set; } = false;
    }
}
