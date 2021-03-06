/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using TehPers.CoreMod.Api.Structs;

namespace TehPers.CoreMod.ContentPacks.Data {
    internal abstract class ItemData {
        /// <summary>The path to the texture containing the sprite relative to the current content pack.</summary>
        public string Texture { get; set; } = null;

        /// <summary>The area in the texture where the sprite is located.</summary>
        public SRectangle? FromArea { get; set; } = null;

        /// <summary>The color to tint the sprite.</summary>
        public SColor Tint { get; set; } = Color.White;
    }
}