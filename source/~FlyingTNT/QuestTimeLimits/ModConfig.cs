/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/FlyingTNT/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using System.Collections.Generic;

namespace QuestTimeLimits
{
    public class ModConfig
    {
        public bool ModEnabled { get; set; } = true;
        public float DailyQuestMult { get; set; } = 2;
        public float SpecialOrderMult { get; set; } = 2;
    }
}
