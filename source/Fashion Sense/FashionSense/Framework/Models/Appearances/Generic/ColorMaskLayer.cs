/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/FashionSense
**
*************************************************/

using System.Collections.Generic;

namespace FashionSense.Framework.Models.Appearances.Generic
{
    public class ColorMaskLayer
    {
        public string Name { get; set; }
        public bool IgnoreUserColorChoice { get; set; }
        public int[] DefaultColor { get; set; }
        public List<int[]> Values { get; set; }
    }
}
