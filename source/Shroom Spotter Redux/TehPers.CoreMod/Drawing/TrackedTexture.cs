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
    internal class TrackedTexture : ITrackedTextureInternal {
        private readonly HashSet<EventHandler<IDrawingInfo>> _drawingHandlers = new HashSet<EventHandler<IDrawingInfo>>();
        private readonly HashSet<EventHandler<IReadonlyDrawingInfo>> _drawnHandlers = new HashSet<EventHandler<IReadonlyDrawingInfo>>();

        public Texture2D CurrentTexture { get; set; }

        public TrackedTexture(Texture2D initialTexture) {
            this.CurrentTexture = initialTexture;
        }

        public IEnumerable<EventHandler<IDrawingInfo>> GetDrawingHandlers() {
            return this._drawingHandlers;
        }

        public IEnumerable<EventHandler<IReadonlyDrawingInfo>> GetDrawnHandlers() {
            return this._drawnHandlers;
        }

        public event EventHandler<IDrawingInfo> Drawing {
            add => this._drawingHandlers.Add(value);
            remove => this._drawingHandlers.Remove(value);
        }

        public event EventHandler<IReadonlyDrawingInfo> Drawn {
            add => this._drawnHandlers.Add(value);
            remove => this._drawnHandlers.Remove(value);
        }
    }
}