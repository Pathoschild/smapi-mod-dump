using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;

namespace AnimalChooser
{
    public class ModEntry : Mod {

        private ModConfig Config;
        private const int CHICKEN = 0;
        private const int COW = 1;

        private bool choosingAnimal = false;
        private bool drawAnimal = false;
        private int currentAnimalType = -1;
        private int heartLevel = 0;
        private int chickenIndex = 0;
        private int cowIndex = 0;
        
        private List<Texture2D> chickenTextures;
        private List<Texture2D> cowTextures;
        private Dictionary<string, string> animalData;
        private Texture2D heartFullTexture;
        private Texture2D heartEmptyTexture;

        private readonly List<string> chickens = new List<string>() {
            "White Chicken",
            "Brown Chicken",
            "Void Chicken",
            "Blue Chicken",
        };

        private readonly List<string> cows = new List<string>() {
            "White Cow",
            "Brown Cow",
        };
        
        public override void Entry(IModHelper helper) {

            Config = Helper.ReadConfig<ModConfig>();

            chickenTextures = new List<Texture2D>() {
                helper.Content.Load<Texture2D>("Assets/White Chicken.png"),
                helper.Content.Load<Texture2D>("Assets/Brown Chicken.png"),
                helper.Content.Load<Texture2D>("Assets/Void Chicken.png"),
                helper.Content.Load<Texture2D>("Assets/Blue Chicken.png"),
            };

            cowTextures = new List<Texture2D>() {
                helper.Content.Load<Texture2D>("Assets/White Cow.png"),
                helper.Content.Load<Texture2D>("Assets/Brown Cow.png"),
            };

            heartFullTexture = helper.Content.Load<Texture2D>("Assets/heartFull.png");
            heartEmptyTexture = helper.Content.Load<Texture2D>("Assets/heartEmpty.png");

            animalData = Game1.content.Load<Dictionary<string, string>>("Data\\FarmAnimals");

            InputEvents.ButtonPressed += InputEvents_ButtonPressed;
            ControlEvents.MouseChanged += ControlEvents_MouseChanged;
            MenuEvents.MenuChanged += MenuEvents_MenuChanged;
            GraphicsEvents.OnPostRenderEvent += GraphicsEvents_OnPostRenderEvent;
            GameEvents.OneSecondTick += GameEvents_OneSecondTick;
        }

        private void ControlEvents_MouseChanged(object sender, EventArgsMouseStateChanged e) {
            
            if (!drawAnimal) {
                return;
            }

            if (e.NewState.ScrollWheelValue < e.PriorState.ScrollWheelValue && heartLevel > 0) {
                heartLevel -= 1;
            } else if (e.NewState.ScrollWheelValue > e.PriorState.ScrollWheelValue && heartLevel < 5) {
                heartLevel += 1;
            } else {
                return;
            }

            Game1.playSound("smallSelect");
        }

        private void GameEvents_OneSecondTick(object sender, EventArgs e) {

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
                    }                    
                }
            }

            drawAnimal = true;
        }

        private void GraphicsEvents_OnPostRenderEvent(object sender, EventArgs e) {

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
                    dx = 24;
                    dy += 24;
                    break;
                case COW:
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
            Texture2D texture = currentAnimalType == CHICKEN ? chickenTextures[chickenIndex] : cowTextures[cowIndex];
            Game1.spriteBatch.Draw(texture, new Rectangle(mx - dx, my - 64 - dy, w, h), Color.White);
            
            if (heartLevel > 0) {
                for (int i=0; i<5; i++) {
                    Game1.spriteBatch.Draw(i < heartLevel ? heartFullTexture : heartEmptyTexture, new Rectangle(mx - (19 - i * 8) * 4, my - 28, 28, 24), Color.White);
                }
            }
        }

        private void InputEvents_ButtonPressed(object sender, EventArgsInput e) {

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
                            if (Game1.player.money >= component.item.salePrice()) {
                                string type = component.item.Name;
                                if (type != null) {
                                    if (type.Contains("Chicken")) {
                                        currentAnimalType = CHICKEN;
                                        choosingAnimal = true;
                                    } else if (type.Contains("Cow")) {
                                        currentAnimalType = COW;
                                        choosingAnimal = true;
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

                if (currentAnimalType != CHICKEN && currentAnimalType != COW) {
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
                        case COW:
                            cowIndex = (delta + cowIndex + cows.Count) % cows.Count;
                            break;
                    }

                    animal.displayType = (currentAnimalType == CHICKEN) ? chickens[chickenIndex] : cows[cowIndex];
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
                default: return false;
            }
        }

        private void MenuEvents_MenuChanged(object sender, EventArgsClickableMenuChanged e) {

            if (e.PriorMenu is PurchaseAnimalsMenu menu2) {
                FarmAnimal animal = Helper.Reflection.GetField<FarmAnimal>(menu2, "animalBeingPurchased").GetValue();
                if (animal != null) {

                    string data = null;
                    string key = "";

                    if (animal.type.Value.Contains("Chicken")) {
                        key = chickens[chickenIndex];
                        animalData.TryGetValue(key, out data);
                    } else if (animal.type.Value.Contains("Cow")) {
                        key = cows[cowIndex];
                        animalData.TryGetValue(key, out data);
                    } else {
                        Monitor.Log($"Invalid animal type: {animal.type.Value}", LogLevel.Warn);
                        return;
                    }

                    if (data != null) {
                        string[] strArray = data.Split('/');
                        animal.type.Value = currentAnimalType == CHICKEN ? chickens[chickenIndex] : cows[cowIndex];
                        animal.sound.Value = strArray[4].Equals("none") ? null : strArray[4];
                        animal.defaultProduceIndex.Value = Convert.ToInt32(strArray[2]);
                        animal.deluxeProduceIndex.Value = Convert.ToInt32(strArray[3]);
                        animal.Sprite = new AnimatedSprite("Animals\\" + (animal.age.Value < animal.ageWhenMature.Value ? "Baby" : "") + (currentAnimalType == CHICKEN ? chickens[chickenIndex] : cows[cowIndex]), 0, Convert.ToInt32(strArray[16]), Convert.ToInt32(strArray[17]));
                        animal.price.Value = Convert.ToInt32(strArray[24]);
                        animal.friendshipTowardFarmer.Value = heartLevel * 200;
                        animal.fullnessDrain.Value = Convert.ToByte(strArray[20]);
                        animal.happinessDrain.Value = Convert.ToByte(strArray[21]);
                        animal.meatIndex.Value = Convert.ToInt32(strArray[23]);
                    } else {
                        Monitor.Log($"data is null - key: {key}", LogLevel.Info);
                    }

                    if (Config.AnimalStartsAsAdult) {
                        animal.age.Value = animal.ageWhenMature.Value;
                    }

                }
            }
            choosingAnimal = false;
        }
    }
}
