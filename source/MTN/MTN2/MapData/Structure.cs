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
