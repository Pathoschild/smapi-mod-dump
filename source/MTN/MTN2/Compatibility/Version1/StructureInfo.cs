using MTN2.MapData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTN2.Compatibility {
    public class StructureInfo {
        public Placement coordinates { get; set; }
        public Interaction pointOfInteraction { get; set; }

        public StructureInfo() {
            coordinates = new Placement(0, 0);
            pointOfInteraction = new Interaction(0, 0);
        }

        public StructureInfo(Placement Coordinates, Interaction PointOfInteraction) {
            this.coordinates = Coordinates;
            this.pointOfInteraction = PointOfInteraction;
        }

        public static void Convert(Structure farmStructure, StructureInfo oldFarmStructure) {
            if (oldFarmStructure == null) return;
            farmStructure.Coordinates = oldFarmStructure.coordinates;
            farmStructure.PointOfInteraction = oldFarmStructure.pointOfInteraction;
        }
    }
}
