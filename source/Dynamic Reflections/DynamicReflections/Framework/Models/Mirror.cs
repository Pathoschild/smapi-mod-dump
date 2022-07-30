/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/DynamicReflections
**
*************************************************/

using DynamicReflections.Framework.Models.Settings;
using Microsoft.Xna.Framework;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicReflections.Framework.Models
{
    internal class Mirror
    {
        public Point TilePosition { get { return _tilePosition; } set { _tilePosition = value; WorldPosition = new Vector2(value.X, value.Y) * 64f; } }
        internal Point _tilePosition;
        public Vector2 WorldPosition { get; set; }
        public MirrorSettings Settings { get; set; } = new MirrorSettings();

        public bool IsEnabled { get; set; }
        public int ActiveIndex { get; set; }
        public Vector2 PlayerReflectionPosition { get; set; }
        public Furniture? FurnitureLink { get; set; }
    }
}
