using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTN2.MapData {
    public struct Area {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public Area(int X, int Y, int Width, int Height) {
            this.X = X;
            this.Y = Y;
            this.Width = Width;
            this.Height = Height;
        }
    }
}
