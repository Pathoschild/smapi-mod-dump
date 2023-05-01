/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/rikai/AnimalSelector
**
*************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;
using StardewValley.Locations;

namespace AnimalSelector
{
    public class ModEntry : Mod {

        private ModConfig Config;
        private const int CHICKEN = 0;
        private const int DUCK = 6;
        private const int RABBIT = 5;
        private const int DINOSAUR = 7;
        private const int COW = 1;
        private const int GOAT = 3;
        private const int SHEEP = 4;
        private const int PIG = 2;
        private const int OSTRICH = 8;

        private bool choosingAnimal = false;
        private bool drawAnimal = false;
        private int currentAnimalType = -1;
        private int heartLevel = 0;
        private int chickenIndex = 0;
        private int cowIndex = 0;
        private int dinosaurIndex = 0;

        private List<Texture2D> chickenTextures;
        private List<Texture2D> duckTextures;
        private List<Texture2D> rabbitTextures;
        private List<Texture2D> dinosaurTextures;

        private List<Texture2D> cowTextures;
        private List<Texture2D> goatTextures;
        private List<Texture2D> sheepTextures;
        private List<Texture2D> pigTextures;
        private List<Texture2D> ostrichTextures;

        private Dictionary<string, string> animalData;
        private Texture2D heartFullTexture;
        private Texture2D heartEmptyTexture;

        // Coop Animals
        private readonly List<string> chickens = new List<string>() {
            "White Chicken",
            "Brown Chicken",
            "Void Chicken",
            "Blue Chicken",
            "Golden Chicken",
        };
        private readonly List<string> ducks = new List<string>() { "Duck" };
        private readonly List<string> rabbits = new List<string>() { "Rabbit" };
        private readonly List<string> dinosaurs = new List<string>() { "Dinosaur" };

        // Barn Animals
        private readonly List<string> cows = new List<string>() {
            "White Cow",
            "Brown Cow",
        };
        private readonly List<string> goats = new List<string>() { "Goat" };
        private readonly List<string> sheep = new List<string>() { "Sheep" };
        private readonly List<string> pigs = new List<string>() { "Pig" };
        private readonly List<string> ostrich = new List<string>() { "Ostrich" };

        public override void Entry(IModHelper helper) {

            Config = Helper.ReadConfig<ModConfig>();

            chickenTextures = new List<Texture2D>() {
                helper.ModContent.Load<Texture2D>("Assets/White Chicken.png"),
                helper.ModContent.Load<Texture2D>("Assets/Brown Chicken.png"),
                helper.ModContent.Load<Texture2D>("Assets/Void Chicken.png"),
                helper.ModContent.Load<Texture2D>("Assets/Blue Chicken.png"),
                helper.ModContent.Load<Texture2D>("Assets/Golden Chicken.png"),
            };
            duckTextures = new List<Texture2D>() { helper.ModContent.Load<Texture2D>("Assets/Duck.png"), };
            rabbitTextures = new List<Texture2D>() { helper.ModContent.Load<Texture2D>("Assets/Rabbit.png"), };
            dinosaurTextures = new List<Texture2D>() { helper.ModContent.Load<Texture2D>("Assets/Dinosaur.png"), };

            cowTextures = new List<Texture2D>() {
                helper.ModContent.Load<Texture2D>("Assets/White Cow.png"),
                helper.ModContent.Load<Texture2D>("Assets/Brown Cow.png"),
            };
            goatTextures = new List<Texture2D>() { helper.ModContent.Load<Texture2D>("Assets/Goat.png"), };
            sheepTextures = new List<Texture2D>() { helper.ModContent.Load<Texture2D>("Assets/Sheep.png"), };
            pigTextures = new List<Texture2D>() { helper.ModContent.Load<Texture2D>("Assets/Pig.png"), };
            ostrichTextures = new List<Texture2D>() { helper.ModContent.Load<Texture2D>("Assets/Ostrich.png"), };

            heartFullTexture = helper.ModContent.Load<Texture2D>("Assets/heartFull.png");
            heartEmptyTexture = helper.ModContent.Load<Texture2D>("Assets/heartEmpty.png");

            animalData = Game1.content.Load<Dictionary<string, string>>("Data\\FarmAnimals");

            helper.Events.Input.ButtonPressed += OnButtonPressed;
            /// helper.Events.Input.CursorMoved += OnCursorMoved;
            helper.Events.Display.MenuChanged += OnMenuChanged;
            helper.Events.Display.Rendered += OnRendered;
            helper.Events.GameLoop.OneSecondUpdateTicked += OnOneSecondUpdateTicked;
            helper.Events.Input.MouseWheelScrolled += OnMouseWheelScrolled;

        }

        private void OnMouseWheelScrolled(object sender, MouseWheelScrolledEventArgs e) {

            if (!drawAnimal) {
                return;
            }

            if (e.NewValue < e.OldValue && heartLevel > 0) {
                heartLevel -= 1;
            } else if (e.NewValue > e.OldValue && heartLevel < 5) {
                heartLevel += 1;
            } else {
                return;
            }

            Game1.playSound("smallSelect");
        }

        private void OnOneSecondUpdateTicked(object sender, EventArgs e) {

            if (!choosingAnimal) {
                return;
            }

            if (Game1.activeClickableMenu is PurchaseAnimalsMenu menu) {

                FarmAnimal animal = Helper.Reflection.GetField<FarmAnimal>(Game1.activeClickableMenu, "animalBeingPurchased").GetValue();

                if (animal != null) {
                    if (currentAnimalType == CHICKEN) {
                        animal.type.Value = chickens[chickenIndex];
                    } else if (currentAnimalType == COW) {
                        animal.type.Value = cows[cowIndex];
                    } else if (currentAnimalType == DINOSAUR)
                    {
                        animal.type.Value = dinosaurs[dinosaurIndex];
                    }
                }
            }

            drawAnimal = true;
        }

        private void OnRendered(object sender, EventArgs e) {

            if (!choosingAnimal) {
                return;
            }

            if (Game1.globalFade) {
                return;
            }

            int dy = Convert.ToInt32(16 * Math.Sin(0.05 * Game1.ticks));
            int dx;
            int w = 64;
            int h = 64;
            switch (currentAnimalType) {
                case CHICKEN:
                case DUCK:
                case RABBIT:
                    dx = 24;
                    dy += 24;
                    break;
                case COW:
                case GOAT:
                case PIG:
                case SHEEP:
                    dx = 64;
                    dy += 88;
                    w = 128;
                    h = 128;
                    break;
                case DINOSAUR:
                case OSTRICH:
                    dx = 64;
                    dy += 88;
                    w = 128;
                    h = 128;
                    break;
                default:
                    return;
            }
            dy += heartLevel > 0 ? 24 : 0;
            int mx = Game1.getMouseX();
            int my = Game1.getMouseY();
            Texture2D texture = chickenTextures[0];
            switch (currentAnimalType) {
                case CHICKEN: texture = chickenTextures[chickenIndex]; break;
                case COW: texture = cowTextures[cowIndex]; break;
                case DUCK: texture = duckTextures[0]; break;
                case RABBIT: texture = rabbitTextures[0]; break;
                case GOAT: texture = goatTextures[0]; break;
                case PIG: texture = pigTextures[0]; break;
                case SHEEP: texture = sheepTextures[0]; break;
                case DINOSAUR: texture = dinosaurTextures[0]; break;
                case OSTRICH: texture = ostrichTextures[0]; break;
            }

            Game1.spriteBatch.Draw(texture, new Rectangle(mx - dx, my - 64 - dy, w, h), Color.White);

            if (heartLevel > 0) {
                for (int i=0; i<5; i++) {
                    Game1.spriteBatch.Draw(i < heartLevel ? heartFullTexture : heartEmptyTexture, new Rectangle(mx - (19 - i * 8) * 4, my - 28, 28, 24), Color.White);
                }
            }
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e) {

            if (e.Button == SButton.Escape || e.Button == SButton.E) {
                choosingAnimal = false;
                drawAnimal = false;
            }

            if (Game1.activeClickableMenu is PurchaseAnimalsMenu menu) {

                FarmAnimal animal = Helper.Reflection.GetField<FarmAnimal>(Game1.activeClickableMenu, "animalBeingPurchased").GetValue();
                if (e.Button == SButton.MouseLeft) {

                    if (menu.doneNamingButton.containsPoint((int)e.Cursor.ScreenPixels.X, (int)e.Cursor.ScreenPixels.Y) ||
                        menu.okButton.containsPoint((int)e.Cursor.ScreenPixels.X, (int)e.Cursor.ScreenPixels.Y)) {
                        choosingAnimal = false;
                        drawAnimal = false;
                    }

                    Building buildingAt = Game1.getFarm().getBuildingAt(new Vector2(e.Cursor.AbsolutePixels.X, e.Cursor.AbsolutePixels.Y) / 64);
                    if (buildingAt != null && buildingAt.buildingType.Value.Contains(animal.buildingTypeILiveIn.Value) &&
                        !(buildingAt.indoors.Value as AnimalHouse).isFull()) {
                        choosingAnimal = false;
                        drawAnimal = false;
                    }

                    foreach (ClickableTextureComponent component in menu.animalsToPurchase) {
                        if (component.containsPoint((int)e.Cursor.ScreenPixels.X, (int)e.Cursor.ScreenPixels.Y)) {
                            if (Game1.player.Money >= component.item.salePrice()) {
                                string type = component.item.Name;
                                if (type != null) {
                                    choosingAnimal = true;
                                    if (type.Contains("Chicken")) {
                                        currentAnimalType = CHICKEN;
                                    } else if (type.Contains("Cow")) {
                                        currentAnimalType = COW;
                                    } else if (type.Contains("Pig")) {
                                        currentAnimalType = PIG;
                                    } else if (type.Contains("Goat")) {
                                        currentAnimalType = GOAT;
                                    } else if (type.Contains("Sheep")) {
                                        currentAnimalType = SHEEP;
                                    } else if (type.Contains("Rabbit")) {
                                        currentAnimalType = RABBIT;
                                    } else if (type.Contains("Duck")) {
                                        currentAnimalType = DUCK;
                                    } else if (type.Contains("Dinosaur")) {
                                        currentAnimalType = DINOSAUR;
                                    } else if (type.Contains("Ostrich")) {
                                        currentAnimalType = OSTRICH;
                                    } else {
                                        currentAnimalType = -1;
                                        choosingAnimal = false;
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }

                if (currentAnimalType < 0) {
                    return;
                }

                bool leftOrRight = false;
                int delta = 0;
                if (e.Button == SButton.Left) {
                    delta = -1;
                    leftOrRight = true;
                } else if (e.Button == SButton.Right) {
                    delta = 1;
                    leftOrRight = true;
                }

                if (leftOrRight) {
                    switch (currentAnimalType) {
                        case CHICKEN:
                            chickenIndex = (delta + chickenIndex + chickens.Count) % chickens.Count;
                            while (!IsChickenTypeUnlocked(chickenIndex)) {
                                chickenIndex = (delta + chickenIndex + chickens.Count) % chickens.Count;
                            }
                            break;
                        case DINOSAUR:
                            dinosaurIndex = (delta + dinosaurIndex + dinosaurs.Count) % dinosaurs.Count;
                            while (!IsDinosaurUnlocked(1))
                            {
                                dinosaurIndex = (delta + dinosaurIndex + dinosaurs.Count) % dinosaurs.Count;
                            }
                            break;
                        case COW:
                            cowIndex = (delta + cowIndex + cows.Count) % cows.Count;
                            break;
                    }

                    if (currentAnimalType == CHICKEN || currentAnimalType == COW) {
                        animal.displayType = (currentAnimalType == CHICKEN) ? chickens[chickenIndex] : cows[cowIndex];
                    }
                }
            }
        }

        private bool IsChickenTypeUnlocked(int type) {
            switch (type) {
                case 0: return true;
                case 1: return true;
                case 2:
                    Game1.player.basicShipped.TryGetValue(305, out int eggsShipped);
                    Game1.player.basicShipped.TryGetValue(308, out int mayoShipped);
                    return Game1.player.eventsSeen.Contains(942069) || Game1.player.hasRustyKey || Config.EnableVoidChickens || eggsShipped > 0 || mayoShipped > 0;
                case 3: return Game1.player.eventsSeen.Contains(3900074) || Config.EnableBlueChickens;
                case 4: return false;
                default: return false;
            }
        }

        private bool IsDinosaurUnlocked(int type) {
            switch (type) {
                case 0: return (Game1.getLocationFromName("ArchaeologyHouse") as LibraryMuseum).museumAlreadyHasArtifact(107);
                default: return false;
            }
        }

        private void OnMenuChanged(object sender, MenuChangedEventArgs e) {

            if (e.OldMenu is PurchaseAnimalsMenu menu2) {
                FarmAnimal animal = Helper.Reflection.GetField<FarmAnimal>(menu2, "animalBeingPurchased").GetValue();
                if (animal != null) {

                    string data = null;
                    string key = "";

                    if (animal.type.Value.Contains("Chicken")) {
                        key = chickens[chickenIndex];
                    } else if (animal.type.Value.Contains("Cow")) {
                        key = cows[cowIndex];
                    } else if (animal.type.Value.Contains("Duck")) {
                        key = "Duck";
                    } else if (animal.type.Value.Contains("Goat")) {
                        key = "Goat";
                    } else if (animal.type.Value.Contains("Pig")) {
                        key = "Pig";
                    } else if (animal.type.Value.Contains("Rabbit")) {
                        key = "Rabbit";
                    } else if (animal.type.Value.Contains("Sheep")) {
                        key = "Sheep";
                    } else if (animal.type.Value.Contains("Dinosaur")) {
                        key = "Dinosaur";
                    } else if (animal.type.Value.Contains("Ostrich")) {
                        key = "Ostrich";
                    } else {
                        Monitor.Log($"Invalid animal type: {animal.type.Value}", LogLevel.Warn);
                        return;
                    }

                    animalData.TryGetValue(key, out data);

                    if (data != null) {
                        string[] strArray = data.Split('/');
                        animal.type.Value = key;
                        animal.sound.Value = strArray[4].Equals("none") ? null : strArray[4];
                        animal.defaultProduceIndex.Value = Convert.ToInt32(strArray[2]);
                        animal.deluxeProduceIndex.Value = Convert.ToInt32(strArray[3]);
                        if (Config.AnimalStartsAsAdult) {
                            animal.age.Value = animal.ageWhenMature.Value;
                        }
                        animal.Sprite = new AnimatedSprite("Animals\\" + (animal.age.Value < animal.ageWhenMature.Value ? "Baby" : "") + ((key.Equals("Duck") && animal.age.Value < animal.ageWhenMature.Value) ? "White Chicken" : key), 0, Convert.ToInt32(strArray[16]), Convert.ToInt32(strArray[17]));
                        animal.price.Value = Convert.ToInt32(strArray[24]);
                        animal.friendshipTowardFarmer.Value = heartLevel * 200;
                        animal.fullnessDrain.Value = Convert.ToByte(strArray[20]);
                        animal.happinessDrain.Value = Convert.ToByte(strArray[21]);
                        animal.meatIndex.Value = Convert.ToInt32(strArray[23]);
                    } else {
                        Monitor.Log($"data is null - key: {key}", LogLevel.Info);
                    }
                }
            }
            choosingAnimal = false;
        }
    }
}
