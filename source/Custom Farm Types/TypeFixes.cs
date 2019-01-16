using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using SpaceCore.Utilities;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using xTile;

namespace CustomFarmTypes
{
    internal static class TypeFixes
    {
        internal static void fix()
        {
            Hijack.hijack(typeof(   Game1).GetMethod(nameof(Game1.performTenMinuteClockUpdate), BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance),
                          typeof(NewGame1).GetMethod(nameof(NewGame1.performTenMinuteClockUpdate), BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance));
            Hijack.hijack(typeof(   FarmAnimal).GetMethod(nameof(FarmAnimal.updateWhenCurrentLocation), BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance),
                          typeof(NewFarmAnimal).GetMethod(nameof(NewFarmAnimal.updateWhenCurrentLocation), BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance));
            
            Hijack.hijack(typeof(   BlueprintsMenu).GetMethod(nameof(BlueprintsMenu.receiveLeftClick), BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance),
                          typeof(NewBlueprintsMenu).GetMethod(nameof(NewBlueprintsMenu.receiveLeftClick), BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance));
            // This is causing problems, and doesn't appear to be used?
            /*Hijack.hijack(typeof(   CataloguePage).GetMethod("receiveLeftClick", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance),
                          typeof(NewCataloguePage).GetMethod("receiveLeftClick", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance));
            */
        }

        private class NewGame1 : Game1
        {
            public static void performTenMinuteClockUpdate()
            {
                if (Game1.IsServer)
                    MultiplayerUtility.broadcastGameClock();
                int trulyDarkTime = Game1.getTrulyDarkTime();
                Game1.gameTimeInterval = 0;
                Game1.timeOfDay += 10;
                if (Game1.timeOfDay % 100 >= 60)
                    Game1.timeOfDay = Game1.timeOfDay - Game1.timeOfDay % 100 + 100;
                if (Game1.isLightning && Game1.timeOfDay < 2400)
                    Utility.performLightningUpdate();
                if (Game1.timeOfDay == trulyDarkTime)
                    Game1.currentLocation.switchOutNightTiles();
                else if (Game1.timeOfDay == Game1.getModeratelyDarkTime())
                {
                    if (Game1.currentLocation.IsOutdoors && !Game1.isRaining)
                        Game1.ambientLight = Color.White;
                    if (!Game1.isRaining && !(Game1.currentLocation is MineShaft) && (Game1.currentSong != null && !Game1.currentSong.Name.Contains("ambient")) && Game1.currentLocation is Town)
                        Game1.changeMusicTrack("none");
                }
                if (Game1.currentLocation.isOutdoors && !Game1.isRaining && (!Game1.eventUp && Game1.currentSong != null) && (Game1.currentSong.Name.Contains("day") && Game1.isDarkOut()))
                    Game1.changeMusicTrack("none");
                if (Game1.weatherIcon == 1)
                {
                    int int32 = Convert.ToInt32(Game1.temporaryContent.Load<Dictionary<string, string>>("Data\\Festivals\\" + Game1.currentSeason + (object)Game1.dayOfMonth)["conditions"].Split('/')[1].Split(' ')[0]);
                    if (Game1.whereIsTodaysFest == null)
                        Game1.whereIsTodaysFest = Game1.temporaryContent.Load<Dictionary<string, string>>("Data\\Festivals\\" + Game1.currentSeason + (object)Game1.dayOfMonth)["conditions"].Split('/')[0];
                    if (Game1.timeOfDay == int32)
                    {
                        string str = Game1.temporaryContent.Load<Dictionary<string, string>>("Data\\Festivals\\" + Game1.currentSeason + (object)Game1.dayOfMonth)["conditions"].Split('/')[0];
                        if (!(str == "Forest"))
                        {
                            if (!(str == "Town"))
                            {
                                if (str == "Beach")
                                    str = Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2639");
                            }
                            else
                                str = Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2637");
                        }
                        else
                            str = Game1.currentSeason.Equals("winter") ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2634") : Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2635");
                        Game1.showGlobalMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2640", (object)Game1.temporaryContent.Load<Dictionary<string, string>>("Data\\Festivals\\" + Game1.currentSeason + (object)Game1.dayOfMonth)["name"]) + str);
                    }
                }
                Game1.player.performTenMinuteUpdate();
                switch (Game1.timeOfDay)
                {
                    case 2500:
                        Game1.dayTimeMoneyBox.timeShakeTimer = 2000;
                        Game1.player.doEmote(24);
                        break;
                    case 2600:
                        Game1.dayTimeMoneyBox.timeShakeTimer = 2000;
                        Game1.farmerShouldPassOut = true;
                        if (Game1.player.getMount() != null)
                        {
                            Game1.player.getMount().dismount();
                            break;
                        }
                        break;
                    case 2800:
                        Game1.exitActiveMenu();
                        Game1.player.faceDirection(2);
                        Game1.player.completelyStopAnimatingOrDoingAction();
                        Game1.player.animateOnce(293);
                        if (Game1.player.getMount() != null)
                        {
                            Game1.player.getMount().dismount();
                            break;
                        }
                        break;
                    case 1200:
                        if (Game1.currentLocation.isOutdoors && !Game1.isRaining && (Game1.currentSong == null || Game1.currentSong.IsStopped || Game1.currentSong.Name.ToLower().Contains("ambient")))
                        {
                            Game1.playMorningSong();
                            break;
                        }
                        break;
                    case 2000:
                        if (!Game1.isRaining && Game1.currentLocation is Town)
                        {
                            Game1.changeMusicTrack("none");
                            break;
                        }
                        break;
                    case 2400:
                        Game1.dayTimeMoneyBox.timeShakeTimer = 2000;
                        Game1.player.doEmote(24);
                        Game1.showGlobalMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2652"));
                        break;
                }
                if (Game1.timeOfDay >= 2600)
                    Game1.farmerShouldPassOut = true;
                foreach (GameLocation location in Game1.locations)
                {
                    location.performTenMinuteUpdate(Game1.timeOfDay);
                    if (location is Farm)
                        ((BuildableGameLocation)location).timeUpdate(10);
                }
                if (Game1.mine == null)
                    return;
                Game1.mine.performTenMinuteUpdate(Game1.timeOfDay);
            }

        }
        private class NewFarmAnimal : FarmAnimal
        {
            private bool isEating;

            public virtual bool updateWhenCurrentLocation(GameTime time, GameLocation location)
            {
                if (!Game1.shouldTimePass())
                    return false;
                if (this.health <= 0)
                    return true;
                if (this.hitGlowTimer > 0)
                    this.hitGlowTimer = this.hitGlowTimer - time.ElapsedGameTime.Milliseconds;
                if (this.sprite.currentAnimation != null)
                {
                    if (this.sprite.animateOnce(time))
                        this.sprite.currentAnimation = (List<FarmerSprite.AnimationFrame>)null;
                    return false;
                }
                this.update(time, location, this.myID, false);
                if (this.behaviors(time, location) || this.sprite.currentAnimation != null)
                    return false;
                if (this.controller != null && this.controller.timerSinceLastCheckPoint > 10000)
                {
                    this.controller = (PathFindController)null;
                    this.Halt();
                }
                if (location is Farm && this.noWarpTimer <= 0 && this.home != null && this.home.getRectForAnimalDoor().Contains(this.GetBoundingBox().Center.X, this.GetBoundingBox().Top))
                {
                    ((AnimalHouse)this.home.indoors).animals.Add(this.myID, this);
                    this.setRandomPosition(this.home.indoors);
                    this.faceDirection(Game1.random.Next(4));
                    this.controller = (PathFindController)null;
                    if (Utility.isOnScreen(this.getTileLocationPoint(), Game1.tileSize * 3, location))
                        Game1.playSound("dwoop");
                    return true;
                }
                int val1 = 0;
                int noWarpTimer = this.noWarpTimer;
                TimeSpan elapsedGameTime = time.ElapsedGameTime;
                int milliseconds1 = elapsedGameTime.Milliseconds;
                int val2 = noWarpTimer - milliseconds1;
                this.noWarpTimer = Math.Max(val1, val2);
                if (this.pauseTimer > 0)
                {
                    int pauseTimer = this.pauseTimer;
                    elapsedGameTime = time.ElapsedGameTime;
                    int milliseconds2 = elapsedGameTime.Milliseconds;
                    this.pauseTimer = pauseTimer - milliseconds2;
                }
                if (Game1.timeOfDay >= 2000)
                {
                    this.sprite.currentFrame = this.buildingTypeILiveIn.Contains("Coop") ? 16 : 12;
                    this.sprite.UpdateSourceRect();
                    if (!this.isEmoting && Game1.random.NextDouble() < 0.002)
                        this.doEmote(24, true);
                }
                else if (this.pauseTimer <= 0)
                {
                    if (Game1.random.NextDouble() < 0.001 && this.age >= (int)this.ageWhenMature && ((int)Game1.gameMode == 3 && this.sound != null) && (Utility.isOnScreen(this.position, Game1.tileSize * 3) && Game1.soundBank != null))
                    {
                        Cue cue = Game1.soundBank.GetCue(this.sound);
                        string name = "Pitch";
                        double num = (double)(1200 + Game1.random.Next(-200, 201));
                        cue.SetVariable(name, (float)num);
                        cue.Play();
                    }
                    if (!Game1.IsClient && Game1.random.NextDouble() < 0.007 && this.uniqueFrameAccumulator == -1)
                    {
                        int direction = Game1.random.Next(5);
                        if (direction != (this.facingDirection + 2) % 4)
                        {
                            if (direction < 4)
                            {
                                int facingDirection = this.facingDirection;
                                this.faceDirection(direction);
                                if (!location.isOutdoors && location.isCollidingPosition(this.nextPosition(direction), Game1.viewport, (Character)this))
                                {
                                    this.faceDirection(facingDirection);
                                    return false;
                                }
                            }
                            switch (direction)
                            {
                                case 0:
                                    this.SetMovingUp(true);
                                    break;
                                case 1:
                                    this.SetMovingRight(true);
                                    break;
                                case 2:
                                    this.SetMovingDown(true);
                                    break;
                                case 3:
                                    this.SetMovingLeft(true);
                                    break;
                                default:
                                    this.Halt();
                                    this.sprite.StopAnimation();
                                    break;
                            }
                        }
                        else if (this.noWarpTimer <= 0)
                        {
                            this.Halt();
                            this.sprite.StopAnimation();
                        }
                    }
                    if (!Game1.IsClient && this.isMoving() && (Game1.random.NextDouble() < 0.014 && this.uniqueFrameAccumulator == -1) || Game1.IsClient && Game1.random.NextDouble() < 0.014 && (double)this.distanceFromLastServerPosition() <= 4.0)
                    {
                        this.Halt();
                        this.sprite.StopAnimation();
                        if (Game1.random.NextDouble() < 0.75)
                        {
                            this.uniqueFrameAccumulator = 0;
                            if (this.buildingTypeILiveIn.Contains("Coop"))
                            {
                                switch (this.facingDirection)
                                {
                                    case 0:
                                        this.sprite.currentFrame = 20;
                                        break;
                                    case 1:
                                        this.sprite.currentFrame = 18;
                                        break;
                                    case 2:
                                        this.sprite.currentFrame = 16;
                                        break;
                                    case 3:
                                        this.sprite.currentFrame = 22;
                                        break;
                                }
                            }
                            else if (this.buildingTypeILiveIn.Contains("Barn"))
                            {
                                switch (this.facingDirection)
                                {
                                    case 0:
                                        this.sprite.currentFrame = 15;
                                        break;
                                    case 1:
                                        this.sprite.currentFrame = 14;
                                        break;
                                    case 2:
                                        this.sprite.currentFrame = 13;
                                        break;
                                    case 3:
                                        this.sprite.currentFrame = 14;
                                        break;
                                }
                            }
                        }
                        this.sprite.UpdateSourceRect();
                    }
                    if (this.uniqueFrameAccumulator != -1)
                    {
                        int frameAccumulator = this.uniqueFrameAccumulator;
                        elapsedGameTime = time.ElapsedGameTime;
                        int milliseconds2 = elapsedGameTime.Milliseconds;
                        this.uniqueFrameAccumulator = frameAccumulator + milliseconds2;
                        if (this.uniqueFrameAccumulator > 500)
                        {
                            if (this.buildingTypeILiveIn.Contains("Coop"))
                                this.sprite.CurrentFrame = this.sprite.CurrentFrame + 1 - this.sprite.CurrentFrame % 2 * 2;
                            else if (this.sprite.CurrentFrame > 12)
                            {
                                this.sprite.CurrentFrame = (this.sprite.CurrentFrame - 13) * 4;
                            }
                            else
                            {
                                switch (this.facingDirection)
                                {
                                    case 0:
                                        this.sprite.CurrentFrame = 15;
                                        break;
                                    case 1:
                                        this.sprite.CurrentFrame = 14;
                                        break;
                                    case 2:
                                        this.sprite.CurrentFrame = 13;
                                        break;
                                    case 3:
                                        this.sprite.CurrentFrame = 14;
                                        break;
                                }
                            }
                            this.uniqueFrameAccumulator = 0;
                            if (Game1.random.NextDouble() < 0.4)
                                this.uniqueFrameAccumulator = -1;
                        }
                    }
                    else if (!Game1.IsClient)
                        this.MovePosition(time, Game1.viewport, location);
                }
                if (!this.isMoving() && location is Farm && this.controller == null)
                {
                    this.Halt();
                    Microsoft.Xna.Framework.Rectangle boundingBox1 = this.GetBoundingBox();
                    foreach (KeyValuePair<long, FarmAnimal> animal in (Dictionary<long, FarmAnimal>)(location as Farm).animals)
                    {
                        if (!animal.Value.Equals((object)this))
                        {
                            Microsoft.Xna.Framework.Rectangle boundingBox2 = animal.Value.GetBoundingBox();
                            if (boundingBox2.Intersects(boundingBox1))
                            {
                                int x1 = boundingBox1.Center.X;
                                boundingBox2 = animal.Value.GetBoundingBox();
                                int x2 = boundingBox2.Center.X;
                                int num1 = x1 - x2;
                                int y1 = boundingBox1.Center.Y;
                                boundingBox2 = animal.Value.GetBoundingBox();
                                int y2 = boundingBox2.Center.Y;
                                int num2 = y1 - y2;
                                if (num1 > 0 && Math.Abs(num1) > Math.Abs(num2))
                                    this.SetMovingUp(true);
                                else if (num1 < 0 && Math.Abs(num1) > Math.Abs(num2))
                                    this.SetMovingDown(true);
                                else if (num2 > 0)
                                    this.SetMovingLeft(true);
                                else
                                    this.SetMovingRight(true);
                            }
                        }
                    }
                }
                return false;
            }

            private void findTruffle(StardewValley.Farmer who)
            {
                Utility.spawnObjectAround(Utility.getTranslatedVector2(this.getTileLocation(), this.FacingDirection, 1f), new StardewValley.Object(this.getTileLocation(), 430, 1), (GameLocation)Game1.getFarm());
                if (new Random((int)this.myID / 2 + (int)Game1.stats.DaysPlayed + Game1.timeOfDay).NextDouble() <= (double)this.friendshipTowardFarmer / 1500.0)
                    return;
                this.currentProduce = -1;
            }

            private bool behaviors(GameTime time, GameLocation location)
            {
                if (this.home == null)
                    return false;
                if (this.isEating)
                {
                    if (this.home != null && this.home.getRectForAnimalDoor().Intersects(this.GetBoundingBox()))
                    {
                        FarmAnimal.behaviorAfterFindingGrassPatch((Character)this, location);
                        this.isEating = false;
                        this.Halt();
                        return false;
                    }
                    if (this.buildingTypeILiveIn.Contains("Barn"))
                    {
                        this.sprite.Animate(time, 16, 4, 100f);
                        if (this.sprite.CurrentFrame >= 20)
                        {
                            this.isEating = false;
                            this.sprite.loop = true;
                            this.sprite.CurrentFrame = 0;
                            this.faceDirection(2);
                        }
                    }
                    else
                    {
                        this.sprite.Animate(time, 24, 4, 100f);
                        if (this.sprite.CurrentFrame >= 28)
                        {
                            this.isEating = false;
                            this.sprite.loop = true;
                            this.sprite.CurrentFrame = 0;
                            this.faceDirection(2);
                        }
                    }
                    return true;
                }
                if (!Game1.IsClient)
                {
                    if (this.controller != null)
                        return true;
                    if (location.IsOutdoors && (int)this.fullness < 195 && Game1.random.NextDouble() < 0.002)
                        this.controller = new PathFindController((Character)this, location, new PathFindController.isAtEnd(FarmAnimal.grassEndPointFunction), -1, false, new PathFindController.endBehavior(FarmAnimal.behaviorAfterFindingGrassPatch), 200, Point.Zero);
                    if (Game1.timeOfDay >= 1700 && location.IsOutdoors && (this.controller == null && Game1.random.NextDouble() < 0.002))
                    {
                        this.controller = new PathFindController((Character)this, location, new PathFindController.isAtEnd(PathFindController.isAtEndPoint), 0, false, (PathFindController.endBehavior)null, 200, new Point(this.home.tileX + this.home.animalDoor.X, this.home.tileY + this.home.animalDoor.Y));
                        if (location.getFarmersCount() == 0)
                        {
                            ((AnimalHouse)this.home.indoors).animals.Add(this.myID, this);
                            this.setRandomPosition(this.home.indoors);
                            this.faceDirection(Game1.random.Next(4));
                            this.controller = (PathFindController)null;
                            (location as Farm).animals.Remove(this.myID);
                            return true;
                        }
                    }
                    if (location.IsOutdoors && !Game1.isRaining && (!Game1.currentSeason.Equals("winter") && this.currentProduce != -1) && (this.age >= (int)this.ageWhenMature && this.type.Contains("Pig") && Game1.random.NextDouble() < 0.0002))
                    {
                        Microsoft.Xna.Framework.Rectangle boundingBox = this.GetBoundingBox();
                        for (int corner = 0; corner < 4; ++corner)
                        {
                            Vector2 cornersOfThisRectangle = Utility.getCornersOfThisRectangle(ref boundingBox, corner);
                            Vector2 key = new Vector2((float)(int)((double)cornersOfThisRectangle.X / (double)Game1.tileSize), (float)(int)((double)cornersOfThisRectangle.Y / (double)Game1.tileSize));
                            if (location.terrainFeatures.ContainsKey(key) || location.objects.ContainsKey(key))
                                return false;
                        }
                        if (Game1.player.currentLocation.Equals((object)location))
                        {
                            DelayedAction.playSoundAfterDelay("dirtyHit", 450);
                            DelayedAction.playSoundAfterDelay("dirtyHit", 900);
                            DelayedAction.playSoundAfterDelay("dirtyHit", 1350);
                        }
                        if (location.Equals((object)Game1.currentLocation))
                        {
                            switch (this.FacingDirection)
                            {
                                case 0:
                                    this.sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
                {
                  new FarmerSprite.AnimationFrame(9, 250),
                  new FarmerSprite.AnimationFrame(11, 250),
                  new FarmerSprite.AnimationFrame(9, 250),
                  new FarmerSprite.AnimationFrame(11, 250),
                  new FarmerSprite.AnimationFrame(9, 250),
                  new FarmerSprite.AnimationFrame(11, 250, false, false, new AnimatedSprite.endOfAnimationBehavior(this.findTruffle), false)
                });
                                    break;
                                case 1:
                                    this.sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
                {
                  new FarmerSprite.AnimationFrame(5, 250),
                  new FarmerSprite.AnimationFrame(7, 250),
                  new FarmerSprite.AnimationFrame(5, 250),
                  new FarmerSprite.AnimationFrame(7, 250),
                  new FarmerSprite.AnimationFrame(5, 250),
                  new FarmerSprite.AnimationFrame(7, 250, false, false, new AnimatedSprite.endOfAnimationBehavior(this.findTruffle), false)
                });
                                    break;
                                case 2:
                                    this.sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
                {
                  new FarmerSprite.AnimationFrame(1, 250),
                  new FarmerSprite.AnimationFrame(3, 250),
                  new FarmerSprite.AnimationFrame(1, 250),
                  new FarmerSprite.AnimationFrame(3, 250),
                  new FarmerSprite.AnimationFrame(1, 250),
                  new FarmerSprite.AnimationFrame(3, 250, false, false, new AnimatedSprite.endOfAnimationBehavior(this.findTruffle), false)
                });
                                    break;
                                case 3:
                                    this.sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
                {
                  new FarmerSprite.AnimationFrame(5, 250, false, true, (AnimatedSprite.endOfAnimationBehavior) null, false),
                  new FarmerSprite.AnimationFrame(7, 250, false, true, (AnimatedSprite.endOfAnimationBehavior) null, false),
                  new FarmerSprite.AnimationFrame(5, 250, false, true, (AnimatedSprite.endOfAnimationBehavior) null, false),
                  new FarmerSprite.AnimationFrame(7, 250, false, true, (AnimatedSprite.endOfAnimationBehavior) null, false),
                  new FarmerSprite.AnimationFrame(5, 250, false, true, (AnimatedSprite.endOfAnimationBehavior) null, false),
                  new FarmerSprite.AnimationFrame(7, 250, false, true, new AnimatedSprite.endOfAnimationBehavior(this.findTruffle), false)
                });
                                    break;
                            }
                            this.sprite.loop = false;
                        }
                        else
                            this.findTruffle(Game1.player);
                    }
                }
                return false;
            }

        }
        private class NewBlueprintsMenu : BlueprintsMenu
        {
            private bool placingStructure;
            private bool demolishing;
            private bool upgrading;
            private bool queryingAnimals;
            private int currentTab;
            private Vector2 positionOfAnimalWhenClicked;
            private BluePrint hoveredItem;
            private BluePrint structureForPlacement;
            private FarmAnimal currentAnimal;
            private Texture2D buildingPlacementTiles;
            private string hoverText = "";
            private List<Dictionary<ClickableComponent, BluePrint>> blueprintButtons = new List<Dictionary<ClickableComponent, BluePrint>>();
            private List<ClickableComponent> tabs = new List<ClickableComponent>();

            public NewBlueprintsMenu(int x, int y) : base(x, y) { }
            public override void receiveLeftClick(int x, int y, bool playSound = true)
            {
                if (this.currentAnimal != null)
                {
                    this.currentAnimal = (FarmAnimal)null;
                    this.placingStructure = true;
                    this.queryingAnimals = true;
                }
                if (!this.placingStructure)
                {
                    Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height);
                    foreach (ClickableComponent key in this.blueprintButtons[this.currentTab].Keys)
                    {
                        if (key.containsPoint(x, y))
                        {
                            if (key.name.Equals("Info Tool"))
                            {
                                this.placingStructure = true;
                                this.queryingAnimals = true;
                                Game1.playSound("smallSelect");
                                return;
                            }
                            if (this.blueprintButtons[this.currentTab][key].doesFarmerHaveEnoughResourcesToBuild())
                            {
                                this.structureForPlacement = this.blueprintButtons[this.currentTab][key];
                                this.placingStructure = true;
                                if (this.currentTab == 1)
                                    this.upgrading = true;
                                Game1.playSound("smallSelect");
                                return;
                            }
                            Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:BlueprintsMenu.cs.10002"), Color.Red, 3500f));
                            return;
                        }
                    }
                    foreach (ClickableComponent tab in this.tabs)
                    {
                        if (tab.containsPoint(x, y))
                        {
                            this.currentTab = this.getTabNumberFromName(tab.name);
                            Game1.playSound("smallSelect");
                            if (this.currentTab != 3)
                                return;
                            this.placingStructure = true;
                            this.demolishing = true;
                            return;
                        }
                    }
                    if (rectangle.Contains(x, y))
                        return;
                    Game1.exitActiveMenu();
                }
                else if (this.demolishing)
                {
                    Building buildingAt = ((BuildableGameLocation)Game1.getLocationFromName("Farm")).getBuildingAt(new Vector2((float)((Game1.viewport.X + Game1.getOldMouseX()) / Game1.tileSize), (float)((Game1.viewport.Y + Game1.getOldMouseY()) / Game1.tileSize)));
                    if (buildingAt != null && ((BuildableGameLocation)Game1.getLocationFromName("Farm")).destroyStructure(buildingAt))
                    {
                        int num = buildingAt.tileY + buildingAt.tilesHigh;
                        for (int index = 0; index < buildingAt.texture.Bounds.Height / Game1.tileSize; ++index)
                        {
                            GameLocation currentLocation = Game1.currentLocation;
                            Texture2D texture = buildingAt.texture;
                            Microsoft.Xna.Framework.Rectangle bounds = buildingAt.texture.Bounds;
                            int x1 = bounds.Center.X;
                            bounds = buildingAt.texture.Bounds;
                            int y1 = bounds.Center.Y;
                            int width = Game1.tileSize / 16;
                            int height = Game1.tileSize / 16;
                            Microsoft.Xna.Framework.Rectangle sourcerectangle = new Microsoft.Xna.Framework.Rectangle(x1, y1, width, height);
                            int xTile = buildingAt.tileX + Game1.random.Next(buildingAt.tilesWide);
                            int yTile = buildingAt.tileY + buildingAt.tilesHigh - index;
                            int numberOfChunks = Game1.random.Next(20, 45);
                            int groundLevelTile = num;
                            Game1.createRadialDebris(currentLocation, texture, sourcerectangle, xTile, yTile, numberOfChunks, groundLevelTile);
                        }
                        Game1.playSound("explosion");
                        Utility.spreadAnimalsAround(buildingAt, (Farm)Game1.getLocationFromName("Farm"));
                    }
                    else
                        Game1.exitActiveMenu();
                }
                else if (this.upgrading && Game1.currentLocation is Farm)
                {
                    Building buildingAt = ((BuildableGameLocation)Game1.getLocationFromName("Farm")).getBuildingAt(new Vector2((float)((Game1.viewport.X + Game1.getOldMouseX()) / Game1.tileSize), (float)((Game1.viewport.Y + Game1.getOldMouseY()) / Game1.tileSize)));
                    if (buildingAt != null && this.structureForPlacement.name != null && buildingAt.buildingType.Equals(this.structureForPlacement.nameOfBuildingToUpgrade))
                    {
                        buildingAt.indoors.map = Game1.game1.xTileContent.Load<Map>("Maps\\" + this.structureForPlacement.mapToWarpTo);
                        buildingAt.indoors.name = this.structureForPlacement.mapToWarpTo;
                        buildingAt.buildingType = this.structureForPlacement.name;
                        buildingAt.texture = this.structureForPlacement.texture;
                        if (buildingAt.indoors is AnimalHouse)
                            ((AnimalHouse)buildingAt.indoors).resetPositionsOfAllAnimals();
                        Game1.playSound("axe");
                        this.structureForPlacement.consumeResources();
                        buildingAt.color = Color.White;
                        Game1.exitActiveMenu();
                    }
                    else if (buildingAt != null)
                        Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:BlueprintsMenu.cs.10011"), Color.Red, 3500f));
                    else
                        Game1.exitActiveMenu();
                }
                else if (this.queryingAnimals)
                {
                    if (!(Game1.currentLocation is Farm) && !(Game1.currentLocation is AnimalHouse))
                        return;
                    foreach (FarmAnimal farmAnimal in Game1.currentLocation is Farm ? ((Farm)Game1.currentLocation).animals.Values.ToList<FarmAnimal>() : ((AnimalHouse)Game1.currentLocation).animals.Values.ToList<FarmAnimal>())
                    {
                        if (new Microsoft.Xna.Framework.Rectangle((int)farmAnimal.position.X, (int)farmAnimal.position.Y, farmAnimal.sprite.SourceRect.Width, farmAnimal.sprite.SourceRect.Height).Contains(Game1.viewport.X + Game1.getOldMouseX(), Game1.viewport.Y + Game1.getOldMouseY()))
                        {
                            this.positionOfAnimalWhenClicked = Game1.GlobalToLocal(Game1.viewport, farmAnimal.position);
                            this.currentAnimal = farmAnimal;
                            this.queryingAnimals = false;
                            this.placingStructure = false;
                            if (farmAnimal.sound == null || farmAnimal.sound.Equals(""))
                                break;
                            Game1.playSound(farmAnimal.sound);
                            break;
                        }
                    }
                }
                else if (!(Game1.currentLocation is Farm))
                    Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:BlueprintsMenu.cs.10012"), Color.Red, 3500f));
                else if (!this.structureForPlacement.doesFarmerHaveEnoughResourcesToBuild())
                    Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:BlueprintsMenu.cs.10002"), Color.Red, 3500f));
                else if (this.tryToBuild())
                {
                    this.structureForPlacement.consumeResources();
                    if (this.structureForPlacement.blueprintType.Equals("Animals"))
                        return;
                    Game1.playSound("axe");
                }
                else
                    Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:BlueprintsMenu.cs.10016"), Color.Red, 3500f));
            }
        }
        private class NewCataloguePage : CataloguePage
        {
            private string descriptionText = "";
            private string hoverText = "";
            private List<ClickableTextureComponent> sideTabs = new List<ClickableTextureComponent>();
            private List<Dictionary<ClickableComponent, BluePrint>> blueprintButtons = new List<Dictionary<ClickableComponent, BluePrint>>();
            private InventoryMenu inventory;
            private Item heldItem;
            private int currentTab;
            private BluePrint hoveredItem;
            private bool demolishing;
            private bool upgrading;
            private bool placingStructure;
            private BluePrint structureForPlacement;
            private GameMenu parent;
            private Texture2D buildingPlacementTiles;

            public NewCataloguePage(int x, int y, int width, int height, GameMenu parent) : base(x, y, width, height, parent)
            {
            }
            public override void receiveLeftClick(int x, int y, bool playSound = true)
            {
                if (!this.placingStructure)
                {
                    this.heldItem = this.inventory.leftClick(x, y, this.heldItem, true);
                    for (int index = 0; index < this.sideTabs.Count; ++index)
                    {
                        if (this.sideTabs[index].containsPoint(x, y) && this.currentTab != index)
                        {
                            Game1.playSound("smallSelect");
                            if (index == 3)
                            {
                                this.placingStructure = true;
                                this.demolishing = true;
                                this.parent.invisible = true;
                            }
                            else
                            {
                                this.sideTabs[this.currentTab].bounds.X -= CataloguePage.widthToMoveActiveTab;
                                this.currentTab = index;
                                this.sideTabs[index].bounds.X += CataloguePage.widthToMoveActiveTab;
                            }
                        }
                    }
                    foreach (ClickableComponent key in this.blueprintButtons[this.currentTab].Keys)
                    {
                        if (key.containsPoint(x, y))
                        {
                            if (this.blueprintButtons[this.currentTab][key].doesFarmerHaveEnoughResourcesToBuild())
                            {
                                this.structureForPlacement = this.blueprintButtons[this.currentTab][key];
                                this.placingStructure = true;
                                this.parent.invisible = true;
                                if (this.currentTab == 1)
                                    this.upgrading = true;
                                Game1.playSound("smallSelect");
                                break;
                            }
                            Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:BlueprintsMenu.cs.10002"), Color.Red, 3500f));
                            break;
                        }
                    }
                }
                else if (this.demolishing)
                {
                    if (!(Game1.currentLocation is Farm))
                        return;
                    if (Game1.IsClient)
                    {
                        Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:CataloguePage.cs.10148"), Color.Red, 3500f));
                    }
                    else
                    {
                        Vector2 vector2 = new Vector2((float)((Game1.viewport.X + Game1.getOldMouseX()) / Game1.tileSize), (float)((Game1.viewport.Y + Game1.getOldMouseY()) / Game1.tileSize));
                        Building buildingAt = ((BuildableGameLocation)Game1.currentLocation).getBuildingAt(vector2);
                        if (Game1.IsMultiplayer && buildingAt != null && buildingAt.indoors.farmers.Count > 0)
                            Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:CataloguePage.cs.10149"), Color.Red, 3500f));
                        else if (buildingAt != null && ((BuildableGameLocation)Game1.currentLocation).destroyStructure(buildingAt))
                        {
                            int num = buildingAt.tileY + buildingAt.tilesHigh;
                            for (int index = 0; index < buildingAt.texture.Bounds.Height / Game1.tileSize; ++index)
                            {
                                GameLocation currentLocation = Game1.currentLocation;
                                Texture2D texture = buildingAt.texture;
                                Microsoft.Xna.Framework.Rectangle bounds = buildingAt.texture.Bounds;
                                int x1 = bounds.Center.X;
                                bounds = buildingAt.texture.Bounds;
                                int y1 = bounds.Center.Y;
                                int width = Game1.tileSize / 16;
                                int height = Game1.tileSize / 16;
                                Microsoft.Xna.Framework.Rectangle sourcerectangle = new Microsoft.Xna.Framework.Rectangle(x1, y1, width, height);
                                int xTile = buildingAt.tileX + Game1.random.Next(buildingAt.tilesWide);
                                int yTile = buildingAt.tileY + buildingAt.tilesHigh - index;
                                int numberOfChunks = Game1.random.Next(20, 45);
                                int groundLevelTile = num;
                                Game1.createRadialDebris(currentLocation, texture, sourcerectangle, xTile, yTile, numberOfChunks, groundLevelTile);
                            }
                            Game1.playSound("explosion");
                            Utility.spreadAnimalsAround(buildingAt, (Farm)Game1.currentLocation);
                            if (!Game1.IsServer)
                                return;
                            MultiplayerUtility.broadcastBuildingChange((byte)1, vector2, "", Game1.currentLocation.name, Game1.player.uniqueMultiplayerID);
                        }
                        else
                        {
                            this.parent.invisible = false;
                            this.placingStructure = false;
                            this.demolishing = false;
                        }
                    }
                }
                else if (this.upgrading && Game1.currentLocation is Farm)
                    (Game1.currentLocation as Farm).tryToUpgrade(((BuildableGameLocation)Game1.getLocationFromName("Farm")).getBuildingAt(new Vector2((float)((Game1.viewport.X + Game1.getOldMouseX()) / Game1.tileSize), (float)((Game1.viewport.Y + Game1.getOldMouseY()) / Game1.tileSize))), this.structureForPlacement);
                else if (!CataloguePage.canPlaceThisBuildingOnTheCurrentMap(this.structureForPlacement, Game1.currentLocation))
                    Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:CataloguePage.cs.10152"), Color.Red, 3500f));
                else if (!this.structureForPlacement.doesFarmerHaveEnoughResourcesToBuild())
                    Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:BlueprintsMenu.cs.10002"), Color.Red, 3500f));
                else if (this.tryToBuild())
                {
                    this.structureForPlacement.consumeResources();
                    if (this.structureForPlacement.blueprintType.Equals("Animals"))
                        return;
                    Game1.playSound("axe");
                }
                else
                {
                    if (Game1.IsClient)
                        return;
                    Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:BlueprintsMenu.cs.10016"), Color.Red, 3500f));
                }
            }

            private bool tryToBuild()
            {
                if (this.structureForPlacement.blueprintType.Equals("Animals"))
                    return ((Farm)Game1.getLocationFromName("Farm")).placeAnimal(this.structureForPlacement, new Vector2((float)((Game1.viewport.X + Game1.getOldMouseX()) / Game1.tileSize), (float)((Game1.viewport.Y + Game1.getOldMouseY()) / Game1.tileSize)), false, Game1.player.uniqueMultiplayerID);
                return (Game1.currentLocation as BuildableGameLocation).buildStructure(this.structureForPlacement, new Vector2((float)((Game1.viewport.X + Game1.getOldMouseX()) / Game1.tileSize), (float)((Game1.viewport.Y + Game1.getOldMouseY()) / Game1.tileSize)), false, Game1.player, false);
            }
        }

        // Unsure how to do this one, sinc eit would involve overwriting a constructor
        /*
        private class NewFarmInfoPage : FarmInfoPage
        {
            public NewFarmInfoPage(int x, int y, int width, int height) : base(x, y, width, height)
            {
            }
        }
        */
    }
}
