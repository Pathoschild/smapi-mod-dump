/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/SolidFoundations
**
*************************************************/

using Microsoft.Xna.Framework;

namespace SolidFoundations.Framework.Models.ContentPack.Actions
{
    public class WarpAction
    {
        public string Map { get; set; }
        public Point DestinationTile { get; set; }
        public int FacingDirection { get; set; } = 1;
        public bool IsMagic { get; set; }
    }
}
