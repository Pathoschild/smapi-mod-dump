/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/barteke22/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Linq;

namespace DiagonalAim
{
    public class ModEntry : Mod
    {
        private ModConfig config;
        private string carTexture = "Minigames\\Intro";
        private Farmer who;
        private float avgSpeed = 10f;
        private float carBottomID = 9999.420f;//can be nexusID.spritetype (just a random idea), this is in case you have other temp-sprites - so they can be identified and not use same logic as these
        private float carTopID2 = 9999.421f;//separated top and bottom so they're not double iterated
        private float busID = 9999.422f;//use this to differenciate between x/y/width/height
        Random r = new Random();

        public override void Entry(IModHelper helper)
        {
            config = Helper.ReadConfig<ModConfig>();

            helper.Events.Input.ButtonsChanged += Input_ButtonsChanged;
            helper.Events.GameLoop.UpdateTicked += GameLoop_UpdateTicked;
            //helper.Events.Display.RenderingWorld += Display_RenderingWorld;//can use this one instead to make the logic more in sync with sprite (as it happens right before its draw)
        }


        private void Input_ButtonsChanged(object sender, ButtonsChangedEventArgs e)
        {
            who = Game1.player;
            float layer = (who.Position.Y + 128f) / 10000f; //my car logic will be simple, cause this is mainly for collision - this uses who.Position bacause it's placed relative to who,
                                                            //otherwise use placement position, also dependent on scale, so could calc from that (you basically want to add +- its height)

            Vector2 speed = new Vector2(-7f - (float)r.NextDouble() * 12, 0f);
            float scaleCar = 3f;
            if (e.Pressed.Contains(SButton.X))//place kar with X
            {
                TemporaryAnimatedSprite car = new TemporaryAnimatedSprite(carTexture, new Rectangle(160, 124, 80, 52), who.Position, false, 0f, Color.White)
                {
                    layerDepth = layer,
                    scale = scaleCar,
                    motion = speed,
                    id = carBottomID
                };
                who.currentLocation.TemporarySprites.Add(car);
                who.currentLocation.TemporarySprites.Add(new TemporaryAnimatedSprite(carTexture, new Rectangle(160, 188, 80, 52), who.Position, false, 0f, new Color(r.Next(256), r.Next(256), r.Next(256)))
                {
                    layerDepth = layer + 0.0000001f,
                    scale = scaleCar,
                    motion = speed,
                    id = carTopID2
                });
            }
            if (e.Pressed.Contains(SButton.V))//place bus with V
            {
                layer = (who.Position.Y + 300f) / 10000f;
                who.currentLocation.TemporarySprites.Add(new TemporaryAnimatedSprite(carTexture, new Rectangle(0, 0, 128, 64), who.Position, false, 0f, new Color(r.Next(256), r.Next(256), r.Next(256)))
                {
                    layerDepth = layer,
                    scale = 5f,
                    motion = speed,
                    flipped = true,
                    id = busID
                });
            }
        }

        //private void Display_RenderingWorld(object sender, RenderingWorldEventArgs e)
        //{

        //}
        private void GameLoop_UpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (who?.currentLocation != null) //replace with stuff like right location check etc
            {
                foreach (var vehicle in who.currentLocation.temporarySprites.Where(x => x.id == carBottomID || x.id == busID))
                {
                    float y = vehicle.Position.Y;
                    float width = vehicle.scale;
                    float height = vehicle.scale;

                    if (vehicle.id == carBottomID)//adjusted a bit to make the hitbox more 'fair' - lower so you don't get hit by the 'roof', bit less tyres
                    {
                        y += (22f * vehicle.scale);
                        width *= 80f;
                        height *= 26f;
                    }
                    else
                    {
                        y += (40f * vehicle.scale);
                        width *= 128f;
                        height *= 18f;
                    }


                    Rectangle vehicleBounds = new Rectangle((int)vehicle.Position.X, (int)y, (int)width, (int)height);
                    if (who.GetBoundingBox().Intersects(vehicleBounds))
                    {
                        float hitPower = Math.Abs(vehicle.motion.X) * (vehicle.id == carBottomID ? 1f : 2f);
                        who.setTrajectory(-Utility.getAwayFromPlayerTrajectory(vehicleBounds, who) * hitPower * 0.02f);//multiply the trajectory/damage based on speed/mass (less speed = less knockback)
                        who.takeDamage(r.Next(0, 3) + (int)hitPower, true, null);
                    }


                    //crash avoidance
                    bool noObstruction = true;

                    foreach (var v2 in who.currentLocation.temporarySprites.Where(x => x != vehicle
                        && ((x.id == carBottomID && ((vehicle.id == carBottomID && x.flipped == vehicle.flipped) || (vehicle.id != carBottomID && x.flipped != vehicle.flipped))) 
                          || x.id == busID       && ((vehicle.id == busID       && x.flipped == vehicle.flipped) || (vehicle.id != busID       && x.flipped != vehicle.flipped)))))//uhhhhhh, all other vehicles going in the same direction?
                    {
                        if (vehicle.motion.X < 0)//going left?
                        {
                            if (v2.Position.X < vehicleBounds.X && v2.Position.X + (v2.scale * (v2.id == carBottomID ? 80f : 128f)) > vehicleBounds.X - 150)//checks if car 150px in front
                            {
                                if (vehicle.motion.X < v2.motion.X)
                                {
                                    if (vehicle.id == carBottomID) who.currentLocation.temporarySprites.Where(x => x.id == carTopID2 && x.Position == vehicle.Position).ToArray()[0].motion += new Vector2(0.2f, 0f);
                                    vehicle.motion += new Vector2(0.2f, 0f);
                                    noObstruction = false;
                                }
                            }
                        }
                        else//right
                        {
                            if (v2.Position.X > vehicleBounds.X && v2.Position.X < vehicleBounds.X + vehicleBounds.Width + 150)//checks if car 150px in front
                            {
                                if (vehicle.motion.X > v2.motion.X)
                                {
                                    if (vehicle.id == carBottomID) who.currentLocation.temporarySprites.Where(x => x.id == carTopID2 && x.Position == vehicle.Position).ToArray()[0].motion -= new Vector2(0.2f, 0f);
                                    vehicle.motion -= new Vector2(0.2f, 0f);
                                    noObstruction = false;
                                }
                            }
                        }
                    }
                    if (noObstruction && Math.Abs(vehicle.motion.X) < avgSpeed)//speed limit for slower cars
                    {
                        Vector2 acc = new Vector2(vehicle.motion.X < 0 ? -0.1f : 0.1f, 0f);
                        if (vehicle.id == carBottomID) who.currentLocation.temporarySprites.Where(x => x.id == carTopID2 && x.Position == vehicle.Position).ToArray()[0].motion += acc;
                        vehicle.motion += acc;
                    }
                }
            }
        }
    }
}
