/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using System;

namespace TehPers.CoreMod.Api.Drawing {
    public interface ITextureEvents {
        /// <summary>Raised when a texture is being drawn to the screen.</summary>
        event EventHandler<IDrawingInfo> Drawing;

        /// <summary>Raised after a texture is drawn to the screen.</summary>
        event EventHandler<IReadonlyDrawingInfo> Drawn;
    }
}