//Copyright (c) 2019 Jahangmar

//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU Lesser General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//(at your option) any later version.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//GNU Lesser General Public License for more details.

//You should have received a copy of the GNU Lesser General Public License
//along with this program. If not, see <https://www.gnu.org/licenses/>.

using System.Collections.Generic;

using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Locations;
using StardewValley.Objects;

using Microsoft.Xna.Framework;
using StardewValley.Tools;
using StardewValley.TerrainFeatures;
using StardewValley.Buildings;

namespace InteractionTweaks
{
    public class EatingBlockingFeature : ModFeature
    {
        private static bool isEating = false;

        public static new void Enable()
        {
            Helper.Events.Input.ButtonPressed += Input_ButtonPressed;
        }

        public static new void Disable()
        {
            Helper.Events.Input.ButtonPressed -= Input_ButtonPressed;
        }

        static void Input_ButtonPressed(object sender, StardewModdingAPI.Events.ButtonPressedEventArgs e)
        {
            GameLocation location = Game1.currentLocation;
            Farmer player = Game1.player;

            if (location != null && e.Button.IsActionButton())
            {
                Vector2 grabTileVec = e.Cursor.GrabTile;
                Vector2 cursorScreenPos = e.Cursor.ScreenPixels;
                Vector2 cursorMapPos = e.Cursor.AbsolutePixels;
                Vector2 cursorMapTile = new Vector2((int)(cursorMapPos.X / Game1.tileSize), (int) (cursorMapPos.Y / Game1.tileSize));
                Object objAtGrabTile = location.getObjectAtTile((int)grabTileVec.X, (int)grabTileVec.Y);

                //TODO use Utility.canGrabSomethingFromHere instead?
            if (Game1.activeClickableMenu != null
                    || objAtGrabTile?.heldObject?.Value != null && objAtGrabTile.heldObject.Value.readyForHarvest //interactable containers (e.g. preserver jars) has finished product
                    || location.isActionableTile((int)grabTileVec.X, (int)grabTileVec.Y, player) //isActionableTile checks stuff like doors, chests, ...
                    || location.isActionableTile((int)grabTileVec.X, (int)grabTileVec.Y + 1, player)
                    || location.objects.ContainsKey(grabTileVec) && ((bool)location.objects[grabTileVec].isSpawnedObject) //forage and animal products
                    || location.terrainFeatures.ContainsKey(grabTileVec) && location.terrainFeatures[grabTileVec] is HoeDirt hoeDirt && hoeDirt.crop != null && hoeDirt.crop.currentPhase.Value == hoeDirt.crop.phaseDays.Count-1 && hoeDirt.crop.dayOfCurrentPhase == 0//crops ready for harvest
                    || (location is Farm && grabTileVec.X >= 71 && grabTileVec.X <= 72 && grabTileVec.Y >= 13 && grabTileVec.Y <= 14) //shippingBin on Farm map
                    || Game1.getFarm().getAllFarmAnimals().Exists((animal) => animal.currentLocation == location && AnimalCollision(animal, cursorMapPos)) //animals
                    || CanGift(cursorMapTile, grabTileVec) //talking and gifting
                    || location is Farm farm && farm.getBuildingAt(grabTileVec) is FishPond
                    || location.getCharacterFromName("TrashBear") is Character trashBear && AnimalCollision(trashBear, cursorMapPos)

                /*|| player.isRidingHorse()*/ || !player.canMove)
                {
                    return;
                }

                if (player.ActiveObject != null && objAtGrabTile != null && objAtGrabTile.performObjectDropInAction(player.ActiveObject, true, player)) //container (e.g. preserver jar) accepts input
                {
                    //Monitor.Log($"true == player.ActiveObject != null && obj != null && obj.performObjectDropInAction(player.ActiveObject, true, player)", LogLevel.Trace);
                    objAtGrabTile.heldObject.Value = null; //performObjectDropInAction sets heldObject so we reset it
                    return;
                }

                if (false/*Config.WeaponBlockingFeature*/ && player.CurrentTool is MeleeWeapon && NotInFightingLocation())
                {
                    Helper.Input.Suppress(e.Button);
                }
                else if (Config.EatingFeature && player.CurrentItem is Object food && food.Edibility > -300 &&
                    !isEating && player.ActiveObject != null && !Game1.dialogueUp && !Game1.eventUp && !player.canOnlyWalk && !player.FarmerSprite.PauseForSingleAnimation && !Game1.fadeToBlack)
                {
                    //Monitor.Log("Eating " + food.Name + "; Edibility is " + food.Edibility, LogLevel.Trace);

                    player.faceDirection(2);
                    isEating = true;
                    player.itemToEat = player.ActiveObject;
                    player.FarmerSprite.setCurrentSingleAnimation(304);

                    int untilFull = UntilFull(player, food);

                    //Monitor.Log($"Until full is {untilFull}, staminaInc: {StaminaInc(food)}*{untilFull}, new: {player.Stamina + StaminaInc(food) * untilFull}/{player.MaxStamina}, healthInc: {HealthInc(food)}*{untilFull}, new: {player.health + HealthInc(food) * untilFull}/{player.maxHealth}", LogLevel.Trace);

                    Response[] responses = {
                            new Response ("One", GetTrans("dia.eatanswerone")),
                            new Response ("Multi", GetTrans("dia.eatanswermulti", new { amount = untilFull})),
                            new Response ("No", GetTrans("dia.eatanswerno"))
                        };
                    Response[] noMultiResponses =
                    {
                        new Response ("One", GetTrans("dia.eatanswerone")),
                        new Response ("No", GetTrans("dia.eatanswerno"))
                    };
                    location.createQuestionDialogue(GetTrans("dia.eatquestion", new { item = food.DisplayName }), (food.Edibility > 0 && untilFull > 1) ? responses : noMultiResponses, delegate (Farmer _, string answer)
                    {
                        switch (answer)
                        {
                            case "One":
                                player.eatHeldObject();
                                break;
                            case "Multi":
                                //Monitor.Log("Eating stack", LogLevel.Trace);
                                float oldStamina = player.Stamina;
                                int oldHealth = player.health;
                                EatFood(player, food, untilFull - 1);
                                //Monitor.Log("Eating last object", LogLevel.Trace);
                                float midStamina = player.Stamina;
                                int midHealth = player.health;
                                player.eatHeldObject();
                                HUDMessages(oldStamina, oldHealth, midStamina, midHealth);
                                break;
                        }
                        isEating = false;
                    });
                    Helper.Input.Suppress(e.Button);
                }
            }
        }

        private static bool CanGift(Vector2 cursorMapTile, Vector2 grabTile)
        {
            if (Utility.checkForCharacterInteractionAtTile(cursorMapTile, Game1.player) || Utility.checkForCharacterInteractionAtTile(new Vector2(cursorMapTile.X, cursorMapTile.Y + 1), Game1.player)) //mouse is over character
            {
                if ((Game1.mouseCursor == 3 || Game1.mouseCursor == 4) && System.Math.Abs(Game1.mouseCursorTransparency - 1f) < 0.01) //3 is gift cursor and 4 is speech bubble cursor, transparency == 1 means that player is close enough to character
                    return true;
            }
            else if (Utility.checkForCharacterInteractionAtTile(grabTile, Game1.player)) //mouse if far away
            {
                return true;
            }
            return false;
        }

        private static bool NotInFightingLocation()
        {
            GameLocation loc = Game1.currentLocation;
            bool nomonster = true;
            foreach (NPC npc in loc.getCharacters())
            {
                if (npc.IsMonster)
                {
                    nomonster = false;
                    break;
                }
            }
            return nomonster;
            /*
            loc is Farm && Game1.whichFarm == 4 //wilderness farm
                || loc is MineShaft
                || loc is StardewValley.Locations.Woods
                */               
        }

        /// <summary>
        /// Checks if cursor position collides with animal bounding box
        /// </summary>
        /// <param name="animal">Animal.</param>
        /// <param name="mapVec">The position of the cursor relative to the top-left corner of the map.</param>
        private static bool AnimalCollision(Character animal, Vector2 mapVec)
        {
            return animal.GetBoundingBox().Intersects(new Rectangle((int)mapVec.X, (int)mapVec.Y, Game1.tileSize, Game1.tileSize));
        }

        /// <summary>
        /// Increase of stamina as calculated by the game.
        /// </summary>
        /// <param name="food">Food.</param>
        private static int StaminaInc(Object food)
        {
            return (int)System.Math.Ceiling((double)@food.Edibility * 2.5) + (int)@food.quality * @food.Edibility;
        }

        /// <summary>
        /// Increase of health as calculated by the game.
        /// </summary>
        /// <param name="food">Food.</param>
        private static int HealthInc(Object food)
        {
            return ((@food.Edibility >= 0) ? ((int)((float)StaminaInc(food) * 0.45f)) : 0);
        }

        /// <summary>
        /// Returns the amount of items of the given item stack that have to be eaten until the players health or stamina is full
        /// </summary>
        /// <param name="player">Player.</param>
        /// <param name="food">Food.</param>
        private static int UntilFull(Farmer player, Object food)
        {
            int sinc = StaminaInc(food);
            int hinc = HealthInc(food);

            int round(double d) => Config.EatingWithoutWaste ? (int)System.Math.Floor(d) : (int)System.Math.Ceiling(d);

            int neededForTopHealth = round((player.maxHealth - (float)player.health) / hinc);
            int neededForTopStamina = round((player.MaxStamina - player.Stamina) / sinc);
            int needed = 1;
            if (Config.EatingTopHealth && Config.EatingTopStamina)
                needed = System.Math.Max(neededForTopStamina, neededForTopHealth);
            else if (!Config.EatingTopHealth && !Config.EatingTopStamina)
                needed = 1;
            else if (Config.EatingTopHealth && !Config.EatingTopStamina)
                needed = neededForTopHealth;
            else if (!Config.EatingTopHealth && Config.EatingTopStamina)
                needed = neededForTopStamina;

            return System.Math.Min(needed, food.Stack);
        }

        private static void EatFood(Farmer player, Object food, int redAmount)
        {
            //Monitor.Log($"Setting Stamina to System.Math.Min({(float)player.MaxStamina}, {player.Stamina + (float)StaminaInc(food) * redAmount})", LogLevel.Trace);

            player.Stamina = System.Math.Min((float)player.MaxStamina, player.Stamina + (float)StaminaInc(food) * redAmount);

            //Monitor.Log($"Setting health to System.Math.Min({player.maxHealth}, {player.health + HealthInc(food) * redAmount})", LogLevel.Trace);

            player.health = System.Math.Min(player.maxHealth, player.health + HealthInc(food) * redAmount);
            player.removeItemsFromInventory(food.ParentSheetIndex, redAmount);
        }
        /// <summary>
        /// Shows health and stamina messages.
        /// </summary>
        private static void HUDMessages(float oldStamina, int oldHealth, float midStamina, int midHealth)
        {
            if (midStamina > oldStamina)
            {
                string staminaText = Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3116", (int)(midStamina - oldStamina));
                Game1.addHUDMessage(new HUDMessage(staminaText, HUDMessage.stamina_type));
            }
            if (midHealth > oldHealth)
            {
                string healthText = Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3118", midHealth - oldHealth);
                Game1.addHUDMessage(new HUDMessage(healthText, HUDMessage.health_type));
            }
        }

    }
}
