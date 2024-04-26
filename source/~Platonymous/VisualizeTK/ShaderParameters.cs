/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualizeTK
{
    public class ShaderParameters
    {
        public float SatR { get; set; } = -1.0f;
        public float SatG { get; set; } = -1.0f;
        public float SatB { get; set; } = -1.0f;

        public string Preset { get; set; } = "Custom";

        public Color[] Colors { get; set; } = new Color[0];

        public float SwitchSpeed { get; set; } = -1f;
    }
}
