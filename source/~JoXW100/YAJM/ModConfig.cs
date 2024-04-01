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
using Netcode;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YAJM
{
    public class ModConfig
    {
        public bool EnableMod { get; set; } = true;
        public SButton JumpButton { get; set; } = SButton.Space;
        public bool EnableMultiJump { get; set; } = false;
        public int MaxJumpDistance { get; set; } = 10;
        public bool PlayJumpSound { get; set; } = true;
        public bool CustomHorseTexture { get; set; } = false;
        public float OrdinaryJumpHeight { get; set; } = 8f;
        public string JumpSound { get; set; } = "dwop";
        public Vector2 HorseShadowOffset { get; set; } = Vector2.Zero;
    }
}
