using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTN2.MapData
{
    public struct Placement
    {
        public float X { get; set; }
        public float Y { get; set; }

        public Placement(float X, float Y) {
            this.X = X;
            this.Y = Y;
        }
    }
}
