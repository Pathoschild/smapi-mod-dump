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
using SObject = StardewValley.Object;

namespace MTN2.MapData {
    public class Ore : Resource {
        private int Width = 1;
        private int Height = 1;

        public Ore() : base("Farm") { }

        public Ore(string MapName) : base(MapName) { }

        public override void AddAtPoint(Vector2 Point, Spawn SItem) {
            SObject newRock = new SObject(Point, SItem.ItemId, 10) {
                MinutesUntilReady = GetRockHealth(SItem.ItemId)
            };
            Map.objects.Add(Point, newRock);
        }

        public override void SpawnAll(int Attempts) {
            for (int i = 0; i < ResourceList.Count; i++) {
                if (ResourceList[i].SkillLevel > Game1.MasterPlayer.MiningLevel) continue;
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

            if (ResourceList[index].SkillLevel > Game1.MasterPlayer.MiningLevel) return;
            if (!ResourceList[index].Roll()) return;
            if (ResourceList[index].Boundary == SpawnType.areaBound) {
                AreaBoundLogic(ResourceList[index], Attempts, Width, Height);
            } else if (ResourceList[index].Boundary == SpawnType.pathTileBound) {
                TileBoundLogic(ResourceList[index], Attempts, Width, Height);
            }
        }

        private int GetRockHealth(int id) {
            switch (id) {
                case 668:
                    return 2;
                case 77:
                    return 7;
                case 76:
                    return 5;
                case 75:
                case 751:
                    return 3;
                case 290:
                    return 4;
                case 764:
                    return 8;
                case 765:
                    return 16;
                default:
                    return 1;
            }
        }
    }
}
