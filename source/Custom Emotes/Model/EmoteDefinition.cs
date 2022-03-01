/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/CustomEmotes
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace CustomEmotes.Model
{
    internal class EmoteDefinition
    {
        public string Image { get; set; }
        public Dictionary<int, string> Map { get; set; }
        public string EnableWithMod { get; set; }
        public string DisableWithMod { get; set; }
    }
}
