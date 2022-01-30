/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/

using StardewModdingAPI.Utilities;
using System.Collections.Generic;

namespace PlayerAnimationFramework
{
    public class PlayerAnimation
    {
        public string chatTrigger;
        public string keyTrigger;
        public List<PlayerAnimationFrame> animations = new List<PlayerAnimationFrame>();
    }

    public class PlayerAnimationFrame
    {
        public int frame = -1;
        public int facing = -1;
        public int length;
        public bool secondaryArm;
        public bool flip;
        public float jitter;
        public float jump;
        public string sound;
        public string music;
        public int emote = -1;
    }
}