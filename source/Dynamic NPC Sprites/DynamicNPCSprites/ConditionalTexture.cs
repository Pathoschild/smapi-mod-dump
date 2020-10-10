/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/miketweaver/BashNinja_SDV_Mods
**
*************************************************/

using System;
using System.Collections.Generic;

namespace DynamicNPCSprites
{
    public class ConditionalTexture
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string[] Weather { get; set; }
        public string[] Season { get; set; }
        public string Sprite { get; set; }
        public string Portrait { get; set; }
    }
}
