/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace RealtimeMinimap.Framework
{
    internal class State
    {
        public bool ShowMinimap { get; set; }
        public RenderTarget2D MinimapTarget { get; set; }
        public RenderTarget2D MinimapLightmap { get; set; }

        public bool DoRenderThisTick { get; set; } = false;
    }
}
