/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/miome/MultitoolMod
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace MultitoolMod
{
    class ModConfig
    {
        public SButton ToolButton { get; set; } = SButton.Q;
        public SButton InfoButton { get; set; } = SButton.N;
        public SButton CleanButton { get; set; } = SButton.C;
    }
}