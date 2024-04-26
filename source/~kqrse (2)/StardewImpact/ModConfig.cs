/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/kqrse/StardewValleyMods-aedenthorn
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using System.Collections.Generic;

namespace StardewImpact
{
    public class ModConfig
    {
        public bool ModEnabled { get; set; } = true;
        public int MinPoints { get; set; } = 1500;
        public float PortraitScale { get; set; } = 2f;
        public int PortraitSpacing { get; set; } = 32;
        public float SkillsScale { get; set; } = 2f;
        public int SkillsSpacing { get; set; } = 32;
        public SButton Button1 { get; set; } = SButton.D1;
        public SButton Button2 { get; set; } = SButton.D2;
        public SButton Button3 { get; set; } = SButton.D3;
        public SButton Button4 { get; set; } = SButton.D4;
        public SButton Button5 { get; set; } = SButton.D5;
        public SButton SkillButton { get; set; } = SButton.E;
        public SButton BurstButton { get; set; } = SButton.Q;
        public int CurrentSkillOffsetX { get; set; } = 0;
        public int CurrentSkillOffsetY { get; set; } = 0;
    }
}
