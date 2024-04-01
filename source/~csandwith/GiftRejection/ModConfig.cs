/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/csandwith/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using System.Collections.Generic;

namespace GiftRejection
{
    public class ModConfig
    {
        public bool ModEnabled { get; set; } = true;
        public bool RejectDisliked { get; set; } = true;
        public bool RejectHated { get; set; } = true;
        public float DislikedThrowDistance { get; set; } = 2;
        public float HatedThrowDistance { get; set; } = 3;
    }
}
