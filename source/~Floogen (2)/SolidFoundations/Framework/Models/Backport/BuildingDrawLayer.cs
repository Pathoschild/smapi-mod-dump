/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/SolidFoundations
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;

namespace SolidFoundations.Framework.Models.Backport
{
    // TODO: When updated to SDV v1.6, this class should be deleted in favor of using StardewValley.GameData.BuildingDrawLayer
    public class BuildingDrawLayer
    {
        [ContentSerializer(Optional = true)]
        public string Texture;

        public string SourceRect;

        public Vector2 DrawPosition;

        [ContentSerializer(Optional = true)]
        public bool DrawInBackground;

        [ContentSerializer(Optional = true)]
        public float SortTileOffset;

        [ContentSerializer(Optional = true)]
        public string OnlyDrawIfChestHasContents;

        [ContentSerializer(Optional = true)]
        public int FrameDuration = 90;

        [ContentSerializer(Optional = true)]
        public int FrameCount = 1;

        [ContentSerializer(Optional = true)]
        public int FramesPerRow = -1;

        [ContentSerializer(Optional = true)]
        public Point AnimalDoorOffset = Point.Zero;

        protected Rectangle? _sourceRect;

        public Rectangle GetSourceRect()
        {
            if (!this._sourceRect.HasValue)
            {
                try
                {
                    string[] array = this.SourceRect.Split(' ');
                    this._sourceRect = new Rectangle(int.Parse(array[0]), int.Parse(array[1]), int.Parse(array[2]), int.Parse(array[3]));
                }
                catch (Exception)
                {
                    this._sourceRect = Rectangle.Empty;
                }
            }
            return this._sourceRect.Value;
        }

        public Rectangle GetSourceRect(int time)
        {
            Rectangle sourceRect = this.GetSourceRect();
            time /= this.FrameDuration;
            time %= this.FrameCount;
            if (this.FramesPerRow < 0)
            {
                sourceRect.X += sourceRect.Width * time;
            }
            else
            {
                sourceRect.X += sourceRect.Width * (time % this.FramesPerRow);
                sourceRect.Y += sourceRect.Height * (time / this.FramesPerRow);
            }
            return sourceRect;
        }
    }
}