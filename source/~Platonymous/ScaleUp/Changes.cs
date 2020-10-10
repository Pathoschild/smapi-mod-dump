/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaleUp
{
    public class Changes
    {
        public string Action { get; set; } = "";
        public string Target { get; set; } = "";

        public Dictionary<string, int> ToArea = new Dictionary<string, int>();
        public bool ScaleUp { get; set; } = false;
        public int OriginalWidth { get; set; } = -1;

        public string FromFileScaled { get; set; } = "";

        public string FromFile { get; set; } = "";


        public int AnimationFrameTime { get; set; } = -1;

        public int AnimationFrameCount { get; set; } = -1;

        internal Texture2D sTex = null;
    }
}
