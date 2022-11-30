/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/BleakCodex/SimpleSounds
**
*************************************************/

using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSounds
{
    public class Content
    {
        public List<Sound>? Sounds { get; set; }
    }

    public class Sound
    {
        public string? Name { get; set; }
        public string? FromFile { get; set; }
        public bool Loop { get; set; } = false;
        public int InstanceLimit { get; set; } = 1;
        public CueDefinition.LimitBehavior LimitBehavior { get; set; } = CueDefinition.LimitBehavior.FailToPlay;
    }
}
