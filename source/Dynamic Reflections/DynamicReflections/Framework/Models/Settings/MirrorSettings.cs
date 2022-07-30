/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/DynamicReflections
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicReflections.Framework.Models.Settings
{
    internal class MirrorSettings
    {
        public Rectangle Dimensions { get; set; }
        public float ReflectionScale { get; set; } // TODO: Implement this property
        public Color ReflectionOverlay { get; set; }
        public Vector2 ReflectionOffset { get; set; }
    }
}
