/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Tlitookilakin/AeroCore
**
*************************************************/

using AeroCore.API;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;

namespace AeroCore.Models
{
    public class LightingEventArgs : ILightingEventArgs
    {
        public float intensity { get; }
        public Color ambient { get; }
        public Vector2 offset { get; }
        public Vector2 worldOffset { get; }
        public float scale { get; }
        public SpriteBatch batch => Game1.spriteBatch;

        internal LightingEventArgs(float intensity, Color ambient, Vector2 offset, Vector2 wOffset)
        {
            this.offset = offset;
            this.worldOffset = wOffset;
            this.intensity = intensity;
            this.ambient = ambient;
            this.scale = 2f / Game1.options.lightingQuality;
        }
        public Vector2 GlobalToLocal(Vector2 position)
        {
            var port = Game1.viewport.Location;
            return new((position.X - port.X) * scale + offset.X, (position.Y - port.Y) * scale + offset.Y);
        }
        public Vector2 ScreenToLocal(Vector2 position) => position * scale + offset;
        public Point GlobalToLocal(Point position)
        {
            var port = Game1.viewport.Location;
            return new((int)((position.X - port.X) * scale + offset.X), (int)((position.Y - port.Y) * scale + offset.Y));
        }
        public Point ScreenToLocal(Point position) => new((int)(position.X * scale), (int)(position.Y * scale));
    }
}
