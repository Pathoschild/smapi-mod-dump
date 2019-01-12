using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTN2.MapData {
    public class Warp {
        public string TargetMap { get; set; } = "Farm";
        public int FromX { get; set; }
        public int FromY { get; set; }
        public int ToX { get; set; }
        public int ToY { get; set; }

        public Warp() { }

        public Warp(int FromX, int FromY, int ToX, int ToY) {
            this.FromX = FromX;
            this.FromY = FromY;
            this.ToX = ToX;
            this.ToY = ToY;
        }

        public Warp(string TargetMap, int FromX, int FromY, int ToX, int ToY) {
            this.TargetMap = TargetMap;
            this.FromX = FromX;
            this.FromY = FromY;
            this.ToX = ToX;
            this.ToY = ToY;
        }
    }
}
