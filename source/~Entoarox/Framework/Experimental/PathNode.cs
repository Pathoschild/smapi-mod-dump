/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Entoarox/StardewMods
**
*************************************************/

namespace Entoarox.Framework.Experimental
{
    public class PathNode
    {
        /*********
        ** Accessors
        *********/
        public PathNode Parent;
        public int X;
        public int Y;


        /*********
        ** Public methods
        *********/
        public PathNode(int x, int y, PathNode parent = null)
        {
            this.Parent = parent;
            this.X = x;
            this.Y = y;
        }

        public override bool Equals(object obj)
        {
            return obj is PathNode node && node.X == this.X && node.Y == this.Y;
        }

        public override int GetHashCode()
        {
            return (this.X << 8) | this.Y;
        }
    }
}
