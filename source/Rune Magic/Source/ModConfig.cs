/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/facufierro/RuneMagic
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuneMagic.Source
{
    public class ModConfig
    {
        public bool DevMode { get; set; } = false;
        public SButton CastKey { get; set; } = SButton.R;
        public SButton ActionBarKey { get; set; } = SButton.Q;
        public SButton SpellBookKey { get; set; } = SButton.K;
        public int CastbarScale { get; set; } = 2;
    }
}