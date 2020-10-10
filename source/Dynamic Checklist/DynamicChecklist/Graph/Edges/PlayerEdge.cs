/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/gunnargolf/DynamicChecklist
**
*************************************************/

namespace DynamicChecklist.Graph.Edges
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using DynamicChecklist.Graph.Vertices;
    using Microsoft.Xna.Framework;

    public class PlayerEdge : StardewEdge
    {
        public PlayerEdge(MovableVertex source, StardewVertex target)
            : base(source, target, "Player")
        {
        }
    }
}
