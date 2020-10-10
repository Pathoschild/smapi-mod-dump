/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Dawilly/MTN2
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTN2.MapData
{
    public class Structure
    {
        public Placement? Coordinates { get; set; }
        public Interaction PointOfInteraction { get; set; }

        public Structure() {
            Coordinates = new Placement(0, 0);
            PointOfInteraction = new Interaction(0, 0);
        }

        public Structure(Placement Coordinates, Interaction PointOfInteraction) {
            this.Coordinates = Coordinates;
            this.PointOfInteraction = PointOfInteraction;
        }
    }
}
