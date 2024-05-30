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
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;

namespace MagicSkillCode.Core
{
    [SConfig]
    [SToken]
    public class Config
    {

        [SConfig.Option("SeedShop")]
        public string AltarLocation { get; set; } = "SeedShop";
        [SConfig.Option()]
        public int AltarX { get; set; } = -1;

        [SConfig.Option()]
        public int AltarY { get; set; } = -1;

        [SConfig.Option("WizardHouse")]
        public string RadioLocation { get; set; } = "WizardHouse";

        [SConfig.Option()]
        public int RadioX { get; set; } = -1;

        [SConfig.Option()]
        public int RadioY { get; set; } = -1;



        [SConfig.Option()]
        public SButton Key_Cast { get; set; } = SButton.Q;

        [SConfig.Option()]
        public SButton Key_SwapSpells { get; set; } = SButton.Tab;

        [SConfig.Option()]
        public SButton Key_Spell1 { get; set; } = SButton.D1;

        [SConfig.Option()]
        public SButton Key_Spell2 { get; set; } = SButton.D2;

        [SConfig.Option()]
        public SButton Key_Spell3 { get; set; } = SButton.D3;

        [SConfig.Option()]
        public SButton Key_Spell4 { get; set; } = SButton.D4;

        [SConfig.Option()]
        public SButton Key_Spell5 { get; set; } = SButton.D5;
    }
}
