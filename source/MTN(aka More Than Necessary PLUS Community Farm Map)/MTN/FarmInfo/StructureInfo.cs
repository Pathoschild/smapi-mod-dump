using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTN.FarmInfo
{
    /// <summary>
    /// Simple class that retains information of buildings, objects, and points that have been moved within a custom farm map.
    /// 
    /// Vital. Populated by JsonSerializer
    /// </summary>
    public class StructureInfo
    {
        //Subclass for rendering of building.
        public class Placement
        {
            public Single x;
            public Single y;

            protected Single xInPixels
            {
                get { return x * 64f; }
                set { x = value / 64f; }
            }

            protected Single yInPixels
            {
                get { return y * 64f; }
                set { y = value / 64f; }
            }

            public Placement() { }
            public Placement(Single x, Single y)
            {
                this.x = x;
                this.y = y;
            }
        }

        public class WarpPoint
        {
            public int x { get; set; }
            public int y { get; set; }

            public WarpPoint() { }
            public WarpPoint(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        //For Buildings, this is where to render the drawing of the building. Unused for objects/points.
        public Placement coordinates = null;
        //For Buildings, this is the point of the door. For anything else, the point to interact said object with.
        public WarpPoint pointOfInteraction = null;
        private bool isBuilding = false;

        public StructureInfo() { }

        public StructureInfo(Placement coordinates, WarpPoint pointOfInteraction)
        {
            this.coordinates = coordinates;
            this.pointOfInteraction = pointOfInteraction;
            isBuilding = true;
        }

        public StructureInfo(WarpPoint coordinates)
        {
            this.pointOfInteraction = coordinates;
            isBuilding = false;
        }
    }
}
