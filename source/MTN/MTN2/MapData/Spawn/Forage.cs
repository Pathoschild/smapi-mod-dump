using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using SObject = StardewValley.Object;

namespace MTN2.MapData {
    public class Forage : Resource {
        private int Width = 1;
        private int Height = 1;

        public Forage() : base("Farm") { }

        public Forage(string MapName) : base(MapName) { }

        public override void AddAtPoint(Vector2 Point, Spawn SItem) {
            Map.dropObject(new SObject(Point, SItem.ItemId, null, false, true, false, true));
        }

        public override void SpawnAll(int Attempts) {
            for (int i = 0; i < ResourceList.Count; i++) {
                if (!ResourceList[i].Roll()) continue;
                if (ResourceList[i].Boundary == SpawnType.areaBound) {
                    AreaBoundLogic(ResourceList[i], Attempts, Width, Height);
                } else if (ResourceList[i].Boundary == SpawnType.pathTileBound) {
                    TileBoundLogic(ResourceList[i], Attempts, Width, Height);
                }
            }
        }

        public override void SpawnItem(int Attempts, int index) {
            if (index >= ResourceList.Count) return;

            if (!ResourceList[index].Roll()) return;
            if (ResourceList[index].Boundary == SpawnType.areaBound) {
                AreaBoundLogic(ResourceList[index], Attempts, Width, Height);
            } else if (ResourceList[index].Boundary == SpawnType.pathTileBound) {
                TileBoundLogic(ResourceList[index], Attempts, Width, Height);
            }
        }
    }
}
