/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/SolidFoundations
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolidFoundations.Framework.Models.Backport;
using SolidFoundations.Framework.Models.ContentPack;
using SolidFoundations.Framework.Utilities.Backport;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using Object = StardewValley.Object;

namespace SolidFoundations.Framework.Patches.Buildings
{
    // TODO: When updated to SDV v1.6, delete this patch
    internal class PurchaseAnimalsMenuPatch : PatchTemplate
    {
        private readonly Type _object = typeof(PurchaseAnimalsMenu);

        internal PurchaseAnimalsMenuPatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(typeof(PurchaseAnimalsMenu), nameof(PurchaseAnimalsMenu.receiveLeftClick), new[] { typeof(int), typeof(int), typeof(bool) }), prefix: new HarmonyMethod(GetType(), nameof(ReceiveLeftClickPrefix)));
            harmony.Patch(AccessTools.Method(typeof(PurchaseAnimalsMenu), nameof(PurchaseAnimalsMenu.performHoverAction), new[] { typeof(int), typeof(int) }), prefix: new HarmonyMethod(GetType(), nameof(PerformHoverActionPrefix)));
        }

        private static bool ReceiveLeftClickPrefix(PurchaseAnimalsMenu __instance, int ___priceOfAnimal, bool ___freeze, bool ___onFarm, ref bool ___namingAnimal, ref Building ___newAnimalHome, FarmAnimal ___animalBeingPurchased, TextBoxEvent ___e, TextBox ___textBox, int x, int y, bool playSound = true)
        {
            if (Game1.IsFading() || ___freeze)
            {
                return false;
            }
            if (__instance.okButton != null && __instance.okButton.containsPoint(x, y) && __instance.readyToClose())
            {
                if (___onFarm)
                {
                    __instance.setUpForReturnToShopMenu();
                    Game1.playSound("smallSelect");
                }
                else
                {
                    Game1.exitActiveMenu();
                    Game1.playSound("bigDeSelect");
                }
            }

            if (___onFarm)
            {
                Vector2 tile = new Vector2((int)((Utility.ModifyCoordinateFromUIScale(x) + (float)Game1.viewport.X) / 64f), (int)((Utility.ModifyCoordinateFromUIScale(y) + (float)Game1.viewport.Y) / 64f));
                GenericBuilding buildingAt = (Game1.getLocationFromName("Farm") as Farm).getBuildingAt(tile) as GenericBuilding;
                if (buildingAt is null)
                {
                    return true;
                }
                if (buildingAt != null && !___namingAnimal)
                {
                    if (buildingAt.Model != null && buildingAt.Model.ValidOccupantTypes.Contains(___animalBeingPurchased.buildingTypeILiveIn.Value))
                    {
                        if ((buildingAt.indoors.Value as AnimalHouse).isFull())
                        {
                            Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:PurchaseAnimalsMenu.cs.11321"));
                        }
                        else if (___animalBeingPurchased.harvestType.Value != 2)
                        {
                            ___namingAnimal = true;
                            __instance.doneNamingButton.visible = true;
                            __instance.randomButton.visible = true;
                            __instance.textBoxCC.visible = true;
                            ___newAnimalHome = buildingAt;
                            if (___animalBeingPurchased.sound.Value != null && Game1.soundBank != null)
                            {
                                ICue cue = Game1.soundBank.GetCue(___animalBeingPurchased.sound.Value);
                                cue.SetVariable("Pitch", 1200 + Game1.random.Next(-200, 201));
                                cue.Play();
                            }
                            ___textBox.OnEnterPressed += ___e;
                            ___textBox.Text = ___animalBeingPurchased.displayName;
                            Game1.keyboardDispatcher.Subscriber = ___textBox;
                            if (Game1.options.SnappyMenus)
                            {
                                __instance.currentlySnappedComponent = __instance.getComponentWithID(104);
                                __instance.snapCursorToCurrentSnappedComponent();
                            }
                        }
                        else if (Game1.player.Money >= ___priceOfAnimal)
                        {
                            ___newAnimalHome = buildingAt;
                            ___animalBeingPurchased.home = ___newAnimalHome;
                            ___animalBeingPurchased.homeLocation.Value = new Vector2(___newAnimalHome.tileX.Value, ___newAnimalHome.tileY.Value);
                            ___animalBeingPurchased.setRandomPosition(___animalBeingPurchased.home.indoors);
                            (___newAnimalHome.indoors.Value as AnimalHouse).animals.Add(___animalBeingPurchased.myID, ___animalBeingPurchased);
                            (___newAnimalHome.indoors.Value as AnimalHouse).animalsThatLiveHere.Add(___animalBeingPurchased.myID);
                            ___newAnimalHome = null;
                            ___namingAnimal = false;
                            if (___animalBeingPurchased.sound.Value != null && Game1.soundBank != null)
                            {
                                ICue cue2 = Game1.soundBank.GetCue(___animalBeingPurchased.sound.Value);
                                cue2.SetVariable("Pitch", 1200 + Game1.random.Next(-200, 201));
                                cue2.Play();
                            }
                            Game1.player.Money -= ___priceOfAnimal;
                            Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:PurchaseAnimalsMenu.cs.11324", ___animalBeingPurchased.displayType), Color.LimeGreen, 3500f));
                        }
                        else if (Game1.player.Money < ___priceOfAnimal)
                        {
                            Game1.dayTimeMoneyBox.moneyShakeTimer = 1000;
                        }
                    }
                    else
                    {
                        Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:PurchaseAnimalsMenu.cs.11326", ___animalBeingPurchased.displayType));
                    }
                }
                if (___namingAnimal)
                {
                    if (__instance.doneNamingButton.containsPoint(x, y))
                    {
                        __instance.textBoxEnter(___textBox);
                        Game1.playSound("smallSelect");
                    }
                    else if (___namingAnimal && __instance.randomButton.containsPoint(x, y))
                    {
                        ___animalBeingPurchased.Name = Dialogue.randomName();
                        ___animalBeingPurchased.displayName = ___animalBeingPurchased.Name;
                        ___textBox.Text = ___animalBeingPurchased.displayName;
                        __instance.randomButton.scale = __instance.randomButton.baseScale;
                        Game1.playSound("drumkit6");
                    }
                    ___textBox.Update();
                }

                return false;
            }

            return true;
        }

        private static bool PerformHoverActionPrefix(PurchaseAnimalsMenu __instance, bool ___freeze, bool ___onFarm, bool ___namingAnimal, FarmAnimal ___animalBeingPurchased, int x, int y)
        {
            __instance.hovered = null;
            if (Game1.IsFading() || ___freeze)
            {
                return false;
            }

            if (__instance.okButton != null)
            {
                if (__instance.okButton.containsPoint(x, y))
                {
                    __instance.okButton.scale = Math.Min(1.1f, __instance.okButton.scale + 0.05f);
                }
                else
                {
                    __instance.okButton.scale = Math.Max(1f, __instance.okButton.scale - 0.05f);
                }
            }
            if (___onFarm)
            {
                if (!___namingAnimal)
                {
                    Vector2 clickTile = new Vector2((int)((Utility.ModifyCoordinateFromUIScale(x) + (float)Game1.viewport.X) / 64f), (int)((Utility.ModifyCoordinateFromUIScale(y) + (float)Game1.viewport.Y) / 64f));
                    Farm f = Game1.getLocationFromName("Farm") as Farm;
                    foreach (Building building in f.buildings)
                    {
                        building.color.Value = Color.White;
                    }
                    GenericBuilding selection = f.getBuildingAt(clickTile) as GenericBuilding;
                    if (selection != null)
                    {
                        if (selection.Model.ValidOccupantTypes.Contains(___animalBeingPurchased.buildingTypeILiveIn.Value) && !(selection.indoors.Value as AnimalHouse).isFull())
                        {
                            selection.color.Value = Color.LightGreen * 0.8f;
                        }
                        else
                        {
                            selection.color.Value = Color.Red * 0.8f;
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
                if (__instance.doneNamingButton != null)
                {
                    if (__instance.doneNamingButton.containsPoint(x, y))
                    {
                        __instance.doneNamingButton.scale = Math.Min(1.1f, __instance.doneNamingButton.scale + 0.05f);
                    }
                    else
                    {
                        __instance.doneNamingButton.scale = Math.Max(1f, __instance.doneNamingButton.scale - 0.05f);
                    }
                }
                __instance.randomButton.tryHover(x, y, 0.5f);

                return false;
            }

            return true;
        }
    }
}
