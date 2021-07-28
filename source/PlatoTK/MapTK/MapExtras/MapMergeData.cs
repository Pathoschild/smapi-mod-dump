/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/PlatoTK
**
*************************************************/

using Microsoft.Xna.Framework;

namespace MapTK.MapExtras
{
    internal class MapMergeData
    {
        public string Source { get; set; }

        public string Target { get; set; }

        public Rectangle FromArea { get; set; }

        public Rectangle ToArea { get; set; }

        public bool RemoveEmpty { get; set; } = false;

        public bool PatchMapProperties { get; set; } = false;
    }
}
