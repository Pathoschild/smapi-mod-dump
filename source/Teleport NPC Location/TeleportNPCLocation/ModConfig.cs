/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/chencrstu/TeleportNPCLocation
**
*************************************************/

using System;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace TeleportNPCLocation
{
    public class ModConfig
    {
        /// <summary>The keys which toggle npc menu.</summary>
        public KeybindList ToggleNPCMenu { get; set; } = new(SButton.OemTilde);

        /// <summary>show more npc info.</summary>
        public bool showMoreInfo { get; set; } = true;

        /// <summary>ban npc list.</summary>
        public string banNPCListString = "";
    }
}
