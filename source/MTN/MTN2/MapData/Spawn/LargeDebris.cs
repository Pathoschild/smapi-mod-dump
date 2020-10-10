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
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace MTN2.MapData {
    public class LargeDebris : Resource {
        private int Width = 2;
        private int Height = 2;

        public LargeDebris() : base("Farm") { }

        public LargeDebris(string MapName) : base(MapName) { }

        public override void AddAtPoint(Vector2 Point, Spawn SItem) {
            Farm farm = (Farm)Map;
            farm.resourceClumps.Add(new ResourceClump(SItem.ItemId, Width, Height, Point));
        }

        public override void SpawnAll(int Attempts) {
            int Amount;
            for (int i = 0; i < ResourceList.Count; i++) {
                Amount = ResourceList[i].GenerateAmount();
                for (int j = 0; j < Amount; j++) {
                    if (!ResourceList[i].Roll()) continue;
                    if (ResourceList[i].Boundary == SpawnType.areaBound) {
                        AreaBoundLogic(ResourceList[i], Attempts, Width, Height);
                    } else if (ResourceList[i].Boundary == SpawnType.pathTileBound) {
                        TileBoundLogic(ResourceList[i], Attempts, Width, Height);
                    }
                }
            }
        }

        public override void SpawnItem(int Attempts, int index) {
            if (index >= ResourceList.Count) return;

            int Amount = ResourceList[index].GenerateAmount();
            for (int i = 0; i < Amount; i++) {
                if (!ResourceList[index].Roll()) continue;
                if (ResourceList[index].Boundary == SpawnType.areaBound) {
                    AreaBoundLogic(ResourceList[index], Attempts, Width, Height);
                } else if (ResourceList[index].Boundary == SpawnType.pathTileBound) {
                    TileBoundLogic(ResourceList[index], Attempts, Width, Height);
                }
            }
        }
    }
}
