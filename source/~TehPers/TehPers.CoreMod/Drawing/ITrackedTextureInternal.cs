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