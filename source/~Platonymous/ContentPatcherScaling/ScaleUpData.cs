/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

using System.Collections.Generic;

namespace PyTKLite
{
    public class ScaleUpData
    {
        public float Scale { get; set; } = 1f;
        public Animation Animation {get; set;} = null;
        public int[] SourceArea { get; set; } = null;
    }

    public class Animation
    {
        public int FrameWidth { get; set; } = -1;
        public int FrameHeight { get; set; } = -1;
        public int FPS { get; set; } = 30;

        public bool Loop { get; set; } = true;
    }
}
