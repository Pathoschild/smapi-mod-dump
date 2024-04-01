/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/csandwith/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace FurnitureRecolor
{
    public class ColorDictData
    {
        public Dictionary<string, List<RecolorData>> colorDict = new Dictionary<string, List<RecolorData>>();
    }

    public class RecolorData
    {
        public int X;
        public int Y;
        public Color color;
        public string name;
    }
}