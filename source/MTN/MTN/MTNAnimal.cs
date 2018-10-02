using Microsoft.Xna.Framework;
using Netcode;
using StardewValley;
using StardewValley.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XNARectangle = Microsoft.Xna.Framework.Rectangle;

namespace MTN {
    class MTNAnimal : FarmAnimal {

        private Farm mapOfHome;

        public MTNAnimal(string type, long id, long ownerID, Farm mapOfHome) : base(type, id, ownerID) {

        }

        public override void updateWhenNotCurrentLocation(Building currentBuilding, GameTime time, GameLocation environment) {
            NetEvent1Field<int, NetInt> makeThoseMoves = (NetEvent1Field<int, NetInt>)Memory.instance.Helper.Reflection.GetField<NetEvent1Field<int, NetInt>>(this, "doFarmerPushEvent");
            makeThoseMoves.Poll();
            //this.doFarmerPushEvent.Poll();
            if (!Game1.shouldTimePass()) {
                return;
            }
            update(time, environment, myID, false);
            if (!Game1.IsMasterGame) {
                return;
            }
            if (currentBuilding != null && Game1.random.NextDouble() < 0.002 && currentBuilding.animalDoorOpen && Game1.timeOfDay < 1630 && !Game1.isRaining && !Game1.currentSeason.Equals("winter") && environment.getFarmersCount() == 0) {
                Farm farm = (Farm)Game1.getLocationFromName("Farm");
                if (farm.isCollidingPosition(new XNARectangle((currentBuilding.tileX + currentBuilding.animalDoor.X) * 64 + 2, (currentBuilding.tileY + currentBuilding.animalDoor.Y) * 64 + 2, (this.isCoopDweller() ? 64 : 128) - 4, 60), Game1.viewport, false, 0, false, this, false, false, false) || farm.isCollidingPosition(new XNARectangle((currentBuilding.tileX + currentBuilding.animalDoor.X) * 64 + 2, (currentBuilding.tileY + currentBuilding.animalDoor.Y + 1) * 64 + 2, (this.isCoopDweller() ? 64 : 128) - 4, 60), Game1.viewport, false, 0, false, this, false, false, false)) {
                    return;
                }
                if (farm.animals.ContainsKey(this.myID)) {
                    for (int i = farm.animals.Count() - 1; i >= 0; i--) {
                        if (farm.animals.Pairs.ElementAt(i).Key.Equals(this.myID)) {
                            farm.animals.Remove(this.myID);
                            break;
                        }
                    }
                }
                (currentBuilding.indoors.Value as AnimalHouse).animals.Remove(this.myID);
                farm.animals.Add(this.myID, this);
                this.faceDirection(2);
                this.SetMovingDown(true);
                base.Position = new Vector2((float)currentBuilding.getRectForAnimalDoor().X, (float)((currentBuilding.tileY + currentBuilding.animalDoor.Y) * 64 - (this.Sprite.getHeight() * 4 - this.GetBoundingBox().Height) + 32));
                this.controller = new PathFindController(this, farm, new PathFindController.isAtEnd(FarmAnimal.grassEndPointFunction), Game1.random.Next(4), false, new PathFindController.endBehavior(FarmAnimal.behaviorAfterFindingGrassPatch), 200, Point.Zero);
                if (this.controller == null || this.controller.pathToEndPoint == null || this.controller.pathToEndPoint.Count < 3) {
                    this.SetMovingDown(true);
                    this.controller = null;
                } else {
                    this.faceDirection(2);
                    base.Position = new Vector2((float)(this.controller.pathToEndPoint.Peek().X * 64), (float)(this.controller.pathToEndPoint.Peek().Y * 64 - (this.Sprite.getHeight() * 4 - this.GetBoundingBox().Height) + 16));
                    if (!this.isCoopDweller()) {
                        this.position.X -= 32f;
                    }
                }
                this.noWarpTimer = 3000;
                NetInt currentOccupants = currentBuilding.currentOccupants;
                int value = currentOccupants.Value;
                currentOccupants.Value = value - 1;
                if (Utility.isOnScreen(base.getTileLocationPoint(), 192, farm)) {
                    farm.localSound("sandyStep");
                }
                if (environment.isTileOccupiedByFarmer(base.getTileLocation()) != null) {
                    environment.isTileOccupiedByFarmer(base.getTileLocation()).temporaryImpassableTile = this.GetBoundingBox();
                }
            }
            Memory.instance.Helper.Reflection.GetMethod(this, "behaviors").Invoke(time, environment);
            //this.behaviors(time, environment);
            //base.updateWhenNotCurrentLocation(currentBuilding, time, environment);
        }
    }
}
