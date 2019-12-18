using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTN2.MapData
{
    public struct Interaction
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Interaction(int X, int Y) {
            this.X = X;
            this.Y = Y;
        }

        public Point ToPoint() {
            return new Point(X, Y);
        }
    }
}
