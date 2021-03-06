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
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using TehPers.CoreMod.Api.Drawing;

namespace TehPers.CoreMod.Drawing {
    internal interface ITrackedTextureInternal : ITrackedTexture {
        new Texture2D CurrentTexture { get; set; }
        IEnumerable<EventHandler<IDrawingInfo>> GetDrawingHandlers();
        IEnumerable<EventHandler<IReadonlyDrawingInfo>> GetDrawnHandlers();
    }
}