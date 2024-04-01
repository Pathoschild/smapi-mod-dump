/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JoXW100/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using System.Collections.Generic;

namespace LikeADuckToWater
{
    public class ModConfig
    {
        public bool ModEnabled { get; set; } = true;
        public bool SwimAfterAutoPet { get; set; } = true;
        public bool EatBeforeSwimming { get; set; } = true;
        public float MaxDistance { get; set; } = 20;
        public float ChancePerTick { get; set; } = 0.03f;
        public int FriendshipGain { get; set; } = 10;
    }
}
