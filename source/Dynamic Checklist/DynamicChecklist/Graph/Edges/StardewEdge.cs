/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/gunnargolf/DynamicChecklist
**
*************************************************/

namespace DynamicChecklist.Graph
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Xna.Framework;
    using QuickGraph;

    public class StardewEdge : Edge<StardewVertex>
    {
        public StardewEdge(StardewVertex source, StardewVertex target, string label)
             : base(source, target)
        {
            this.Label = label;
        }

        public string Label { get; private set; }

        public double Cost
        {
            get
            {
                if (this.Source.Location == this.Target.Location)
                {
                    return Vector2.Distance(this.Source.Position, this.Target.Position);
                }
                else
                {
                    return 0;
                }
            }
        }
    }
}
