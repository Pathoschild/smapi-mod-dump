/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using SObject = StardewValley.Object;

namespace ConfigureMachineOutputs.Framework.Patches
{
    internal class PerformObjectDropInActionPatch
    {
        private static CmoConfig _config;
        private static IMonitor _monitor;

        //Variables that will be used by Machines that are enabled.
        private static Random _rnd;
        private static int _input;
        private static int _minOut;
        private static int _maxOut;
        private static int _output;

        public PerformObjectDropInActionPatch(IMonitor monitor, CmoConfig config)
        {
            _config = config;
            _monitor = monitor;
        }
        //Gets the crystalarium time needed. Ripped from SDV source
        private static int GetMinutesForCrystalarium(int whichGem)
        {
            switch (whichGem)
            {
                case 60:
                    return 3000;
                case 62:
                    return 2240;
                case 64:
                    return 3000;
                case 66:
                    return 1360;
                case 68:
                    return 1120;
                case 70:
                    return 2400;
                case 72:
                    return 7200;
                case 80:
                    return 420;
                case 82:
                    return 1300;
                case 84:
                    return 1120;
                case 86:
                    return 800;
                default:
                    return 5000;
            }
        }

        //Check item input for the Deconstructor
        public static SObject GetDeconstructorOutput(Item item)
        {
            if (!CraftingRecipe.craftingRecipes.ContainsKey(item.Name))
            {
                return null;
            }
            if (CraftingRecipe.craftingRecipes[item.Name].Split('/')[2].Split(' ').Count() > 1)
            {
                return null;
            }
            if (Utility.IsNormalObjectAtParentSheetIndex(item, 710))
            {
                return new SObject(334, 2);
            }
            string[] ingredients = CraftingRecipe.craftingRecipes[item.Name].Split('/')[0].Split(' ');
            List<SObject> ingredient_objects = new List<SObject>();
            for (int i = 0; i < ingredients.Count(); i += 2)
            {
                ingredient_objects.Add(new SObject(Convert.ToInt32(ingredients[i]), Convert.ToInt32(ingredients[i + 1])));
            }
            if (ingredient_objects.Count == 0)
            {
                return null;
            }
            ingredient_objects.Sort((SObject a, SObject b) => a.sellToStorePrice(-1L) * a.Stack - b.sellToStorePrice(-1L) * b.Stack);
            return ingredient_objects.Last();
        }

        //Method to make it easier checking if the player has enough resources.
        private static bool HasEnough(int item, int amt)
        {
            return Game1.player.getTallyOfObject(item, false) >= amt;
        }

        public static bool Prefix(SObject __instance, Item dropInItem, bool probe, Farmer who)
        {
            if (!(dropInItem is SObject))
                return false;
            SObject inputItem = (SObject) dropInItem;
            SObject machine = __instance;
            Multiplayer mp = new Multiplayer();
            //Check the machine for held items
            if (machine.heldObject.Value != null && !machine.Name.Equals("Recycling Machine") &&
                !machine.Name.Equals("Crystalarium") || inputItem != null && inputItem.bigCraftable.Value)
                return false;
            if (machine.bigCraftable.Value && !probe && machine.heldObject.Value == null)
                machine.scale.X = 5f;
            //_monitor.Log("Starting to run custom PerformObjectDropInAction", LogLevel.Trace);
            if (machine.Name.Equals("Incubator"))
            {
                if (machine.heldObject.Value == null && (inputItem.Category == -5 || inputItem.ParentSheetIndex == 107))
                {
                    machine.heldObject.Value = new SObject(inputItem.ParentSheetIndex, 1, false, -1, 0);
                    if (!probe)
                    {
                        
                        who.currentLocation.playSound("coin");
                        machine.MinutesUntilReady = 9000 * inputItem.ParentSheetIndex == 107 ? 2 : 1;
                        if (who.professions.Contains(2))
                            machine.MinutesUntilReady /= 2;
                        if (inputItem.ParentSheetIndex == 108 || inputItem.ParentSheetIndex == 182 ||
                            inputItem.ParentSheetIndex == 305)
                            machine.ParentSheetIndex += 2;
                        else
                            machine.ParentSheetIndex++;
                    }
                    return true;
                }
            }
            else if (machine.Name.Equals("Ostrich Incubator"))
            {
                if (machine.heldObject.Value == null && (int)inputItem.ParentSheetIndex == 289)
                {
                    machine.heldObject.Value = new SObject(inputItem.ParentSheetIndex, 1);
                    if (!probe)
                    {
                        who.currentLocation.playSound("coin");
                        machine.MinutesUntilReady = 15000;
                        if (who.professions.Contains(2))
                        {
                            machine.MinutesUntilReady /= 2;
                        }
                        machine.ParentSheetIndex++;
                        if (who?.currentLocation is AnimalHouse)
                        {
                            ((AnimalHouse) who.currentLocation).hasShownIncubatorBuildingFullMessage = false;
                        }
                    }
                    return true;
                }
            }
            else if (machine.Name.Equals("Slime Incubator"))
            {
                if (machine.heldObject.Value == null && inputItem.Name.Contains("Slime Egg"))
                {
                    machine.heldObject.Value = new SObject(inputItem.ParentSheetIndex, 1);
                    if (!probe)
                    {
                        who.currentLocation.playSound("coin");
                        machine.MinutesUntilReady = 4000;
                        if (who.professions.Contains(2))
                            machine.MinutesUntilReady /= 2;
                        machine.ParentSheetIndex++;
                    }
                    return true;
                }
            }
            else if (machine.Name.Equals("Deconstructor"))
            {
                //Get Calculations
                _input = 1;//_config.Machines.Deconstructor.CustomDeconstructorEnabled ? _config.Machines.Charcoal.CharcoalInputMultiplier : 1;
                _minOut = _config.Machines.Deconstructor.CustomDeconstructorEnabled ? _config.Machines.Deconstructor.DeconstructorMinOutput : 1;
                _maxOut = _config.Machines.Deconstructor.CustomDeconstructorEnabled ? _config.Machines.Deconstructor.DeconstructorMaxOutput : 1;
                _rnd = new Random();
                int totalInput = _config.Machines.Deconstructor.CustomDeconstructorEnabled ? _input * 1 : 1;
                _output = _rnd.Next(_minOut, _maxOut) * _input;


                SObject decon = GetDeconstructorOutput(inputItem);
                if (decon != null)
                {
                    machine.heldObject.Value = new SObject(inputItem.ParentSheetIndex, 1);
                    if (!probe)
                    {
                        machine.heldObject.Value = decon;
                        machine.MinutesUntilReady = 60;
                        machine.heldObject.Value.Stack = _output;
                        Game1.playSound("furnace");
                        Game1.player.removeItemsFromInventory(inputItem.ParentSheetIndex, 1);
                    }

                    return true;
                }

                return false;
                /*
                else if (this.name.Equals("Deconstructor"))
                {
                    Object deconstructor_output = this.GetDeconstructorOutput(dropIn);
                    if (deconstructor_output != null)
                    {
                        this.heldObject.Value = new Object(dropIn.parentSheetIndex, 1);
                        if (!probe)
                        {
                            this.heldObject.Value = deconstructor_output;
                            this.MinutesUntilReady = 60;
                            Game1.playSound("furnace");
                            return true;
                        }
                        return true;
                    }
                    if (!probe)
                    {
                        if (Object.autoLoadChest == null)
                        {
                            Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Deconstructor_fail"));
                        }
                        return false;
                    }
                }*/
            }
            else if (machine.Name.Equals("Geode Crusher"))
            {
                _input = 1;//_config.Machines.GeodeCrusher.CustomGeodeCrusherEnabled ? _config.Machines.GeodeCrusher.GeodeCrusherInputMultiplier : 1;
                _minOut = _config.Machines.GeodeCrusher.CustomGeodeCrusherEnabled ? _config.Machines.GeodeCrusher.GeodeCrusherMinOutput : 1;
                _maxOut = _config.Machines.GeodeCrusher.CustomGeodeCrusherEnabled ? _config.Machines.GeodeCrusher.GeodeCrusherMaxOutput : 1;
                _rnd = new Random();
                int totalInput = _config.Machines.GeodeCrusher.CustomGeodeCrusherEnabled ? _input * 1 : 1;
                _output = _rnd.Next(_minOut, _maxOut) * _input;

                if (who.IsLocalPlayer && who.getTallyOfObject(382, false) <= 0)
                {
                    if (!probe && who.IsLocalPlayer && SObject.autoLoadChest == null)
                    {
                        Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12772"));
                    }
                    return false;
                }
                //All passed now we process
                SObject geode_item = (SObject)Utility.getTreasureFromGeode(inputItem);
                if (machine.heldObject.Value == null)
                {
                    if (!Utility.IsGeode(inputItem, disallow_special_geodes: true) || geode_item == null)
                    {
                        return false;
                    }

                    machine.heldObject.Value = geode_item;
                    if (!probe)
                    {
                        Game1.stats.GeodesCracked++;
                        machine.MinutesUntilReady = 60;
                        machine.heldObject.Value.Stack = _output;
                    }
                    if (probe)
                    {
                        return true;
                    }
                    machine.showNextIndex.Value = true;
                    Utility.addSmokePuff(who.currentLocation, machine.TileLocation * 64f + new Vector2(4f, -48f), 200);
                    Utility.addSmokePuff(who.currentLocation, machine.TileLocation * 64f + new Vector2(-16f, -56f), 300);
                    Utility.addSmokePuff(who.currentLocation, machine.TileLocation * 64f + new Vector2(16f, -52f), 400);
                    Utility.addSmokePuff(who.currentLocation, machine.TileLocation * 64f + new Vector2(32f, -56f), 200);
                    Utility.addSmokePuff(who.currentLocation, machine.TileLocation * 64f + new Vector2(40f, -44f), 500);
                    Game1.playSound("drumkit4");
                    Game1.playSound("stoneCrack");
                    DelayedAction.playSoundAfterDelay("steam", 200);
                    machine.ConsumeInventoryItem(who, 382, 1);
                    inputItem.Stack--;
                    if (inputItem.Stack <= 0)
                    {
                        return true;
                    }
                }
            }
            else if (machine.Name.Equals("Bone Mill"))
            {
                int numItemsToTake = 0;
                int[] itemsToTakeNum = new[] {579, 580, 581, 582, 583, 584, 585, 586, 587, 588, 589, 820, 821, 822, 823, 824, 825, 826, 827, 828};

                //Grab Config settings
                _input = 1;//_config.Machines.BoneMill.CustomBoneMillEnabled ? _config.Machines.BoneMill.BoneMillInputMultiplier : 1;
                _minOut = _config.Machines.BoneMill.CustomBoneMillEnabled ? _config.Machines.BoneMill.BoneMillMinOutput : 1;
                _maxOut = _config.Machines.BoneMill.CustomBoneMillEnabled ? _config.Machines.BoneMill.BoneMillMaxOutput : 1;
                _rnd = new Random();
                int totalInput = _config.Machines.BoneMill.CustomBoneMillEnabled ? _input * 1 : 1;
                _output = _rnd.Next(_minOut, _maxOut) * _input;

                //Start to process machine
                if (itemsToTakeNum.Contains(inputItem.ParentSheetIndex))
                    numItemsToTake = 1;
                else if (inputItem.ParentSheetIndex == 881)
                    numItemsToTake = 5;
                if (numItemsToTake == 0)
                    return false;
                else if (inputItem.Stack < numItemsToTake)
                {
                    if (SObject.autoLoadChest == null)
                    {
                        Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:bonemill_5"));
                    }
                    return false;
                }
                int which = -1;
                    int howMany = 1;
                    switch (Game1.random.Next(4))
                    {
                        case 0:
                            which = 466;
                            howMany = _config.Machines.BoneMill.CustomBoneMillEnabled ? _output : 3;
                            break;
                        case 1:
                            which = 465;
                            howMany = _config.Machines.BoneMill.CustomBoneMillEnabled ? _output : 5;
                            break;
                        case 2:
                            which = 369;
                            howMany = _config.Machines.BoneMill.CustomBoneMillEnabled ? _output : 10;
                            break;
                        case 3:
                            which = 805;
                            howMany = _config.Machines.BoneMill.CustomBoneMillEnabled ? _output : 5;
                            break;
                    }
                    if (Game1.random.NextDouble() < 0.1)
                    {
                        howMany *= 2;
                    }
                    machine.heldObject.Value = new SObject(which, howMany);
                    if (!probe)
                    {
                        machine.ConsumeInventoryItem(who, inputItem, numItemsToTake);
                        machine.MinutesUntilReady = 240;
                        who.currentLocation.playSound("skeletonStep");
                        DelayedAction.playSoundAfterDelay("skeletonHit", 150);
                    }
                
               
            }
            else if (machine.Name.Equals("Keg"))
            {
                //Need to check to see if Custom Mayo enabled.
                _input = _config.Machines.Keg.CustomKegEnabled ? _config.Machines.Keg.KegInputMultiplier : 1;
                _minOut = _config.Machines.Keg.CustomKegEnabled ? _config.Machines.Keg.KegMinOutput : 1;
                _maxOut = _config.Machines.Keg.CustomKegEnabled ? _config.Machines.Keg.KegMaxOutput : 1;
                _rnd = new Random();
                int totalInput = _config.Machines.Keg.CustomKegEnabled ? _input * 1 : 1;
                _output = _rnd.Next(_minOut, _maxOut) * _input;

                //Make sure the player has enough items or fail
                if (Game1.player.getTallyOfObject(inputItem.ParentSheetIndex, false) < totalInput && !probe)
                {
                    Game1.showRedMessage($"You need {totalInput} {inputItem.Name}");
                    return false;
                }
                //Start processing the Kegs
                switch (inputItem.ParentSheetIndex)
                {
                    case 262: //Wheat
                        machine.heldObject.Value = new SObject(Vector2.Zero, 346, "Beer", false, true, false, false);
                        if (!probe)
                        {
                            machine.heldObject.Value.Stack = _output;
                            machine.heldObject.Value.Name = "Beer";
                            who.currentLocation.playSound("Ship");
                            who.currentLocation.playSound("bubbles");
                            machine.MinutesUntilReady = 1750;
                            mp.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(256, 1856, 64, 128), 80f, 6, 999999, machine.TileLocation * 64f + new Vector2(0.0f, (float)sbyte.MinValue), false, false, (float)(((double)machine.TileLocation.Y + 1.0) * 64.0 / 10000.0 + 9.99999974737875E-05), 0.0f, Color.Yellow * 0.75f, 1f, 0.0f, 0.0f, 0.0f, false)
                            {
                                alphaFade = 0.005f
                            });
                            Game1.player.removeItemsFromInventory(inputItem.ParentSheetIndex, totalInput);
                        }
                        return false;
                    case 304: //Hops
                        machine.heldObject.Value = new SObject(Vector2.Zero, 303, "Pale Ale", false, true, false, false);
                        if (!probe)
                        {
                            machine.heldObject.Value.Stack = _output;
                            machine.heldObject.Value.Name = "Pale Ale";
                            who.currentLocation.playSound("Ship");
                            who.currentLocation.playSound("bubbles");
                            machine.MinutesUntilReady = 2250;
                            mp.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(256, 1856, 64, 128), 80f, 6, 999999, machine.TileLocation * 64f + new Vector2(0.0f, (float)sbyte.MinValue), false, false, (float)(((double)machine.TileLocation.Y + 1.0) * 64.0 / 10000.0 + 9.99999974737875E-05), 0.0f, Color.Yellow * 0.75f, 1f, 0.0f, 0.0f, 0.0f, false)
                            {
                                alphaFade = 0.005f
                            });
                            Game1.player.removeItemsFromInventory(inputItem.ParentSheetIndex, totalInput);
                        }
                        return false;
                    case 815:
                        machine.heldObject.Value = new SObject(Vector2.Zero, 614, "Green Tea", canBeSetDown: false, canBeGrabbed: true, isHoedirt: false, isSpawnedObject: false);
                        if (!probe)
                        {
                            machine.heldObject.Value.name = "Green Tea";
                            who.currentLocation.playSound("Ship");
                            who.currentLocation.playSound("bubbles");
                            machine.MinutesUntilReady = 180;
                            mp.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(256, 1856, 64, 128), 80f, 6, 999999, machine.TileLocation * 64f + new Vector2(0f, -128f), flicker: false, flipped: false, (machine.TileLocation.Y + 1f) * 64f / 10000f + 0.0001f, 0f, Color.Lime * 0.75f, 1f, 0f, 0f, 0f)
                            {
                                alphaFade = 0.005f
                            });
                        }
                        return false;
                    case 340: //Honey
                        machine.heldObject.Value = new SObject(Vector2.Zero, 459, "Mead", false, true, false, false);
                        if (!probe)
                        {
                            machine.heldObject.Value.Stack = _output;
                            string meadName = inputItem.Name.Replace("Honey", "");
                            machine.heldObject.Value.Name = meadName +" Mead";
                            who.currentLocation.playSound("Ship");
                            who.currentLocation.playSound("bubbles");
                            machine.MinutesUntilReady = 600;
                            mp.broadcastSprites(who.currentLocation,
                                new TemporaryAnimatedSprite("TileSheets\\animations",
                                    new Rectangle(256, 1856, 64, 128), 80f, 6, 999999,
                                    machine.TileLocation * 64f + new Vector2(0.0f, (float) sbyte.MinValue), false,
                                    false,
                                    (float) (((double) machine.TileLocation.Y + 1.0) * 64.0 / 10000.0 +
                                             9.99999974737875E-05), 0.0f, Color.Yellow * 0.75f, 1f, 0.0f, 0.0f, 0.0f,
                                    false)
                                {
                                    alphaFade = 0.005f
                                });
                            Game1.player.removeItemsFromInventory(inputItem.ParentSheetIndex, totalInput);
                        }
                        return false;
                    case 433: //Coffee Bean
                        machine.heldObject.Value = new SObject(Vector2.Zero, 395, "Coffee", false, true, false, false);
                        if (!probe)
                        {
                            machine.heldObject.Value.Stack = _output;
                            machine.heldObject.Value.Name = "Coffee";
                            who.currentLocation.playSound("Ship");
                            who.currentLocation.playSound("bubbles");
                            machine.MinutesUntilReady = 120;
                            mp.broadcastSprites(who.currentLocation,
                                new TemporaryAnimatedSprite("TileSheets\\animations",
                                    new Rectangle(256, 1856, 64, 128), 80f, 6, 999999,
                                    machine.TileLocation * 64f + new Vector2(0.0f, (float)sbyte.MinValue), false,
                                    false,
                                    (float)(((double)machine.TileLocation.Y + 1.0) * 64.0 / 10000.0 +
                                            9.99999974737875E-05), 0.0f, Color.Yellow * 0.75f, 1f, 0.0f, 0.0f, 0.0f,
                                    false)
                                {
                                    alphaFade = 0.005f
                                });
                            Game1.player.removeItemsFromInventory(inputItem.ParentSheetIndex, totalInput);
                        }
                        return false;
                    default: // All Others
                        switch (inputItem.Category)
                        {
                            case -79://Fruit
                                machine.heldObject.Value = new StardewValley.Object(Vector2.Zero, 348, "BitchAss Wine", canBeSetDown: false, canBeGrabbed: true, isHoedirt: false, isSpawnedObject: false);
                                machine.heldObject.Value.Price = inputItem.Price * 3;
                                if (!probe)
                                {
                                    machine.heldObject.Value.Stack = PerformObjectDropInActionPatch._output;
                                    machine.heldObject.Value.name = inputItem.Name + " Wine";
                                    machine.heldObject.Value.preserve.Value = StardewValley.Object.PreserveType.Wine;
                                    machine.heldObject.Value.preservedParentSheetIndex.Value = inputItem.ParentSheetIndex;
                                    who.currentLocation.playSound("Ship");
                                    who.currentLocation.playSound("bubbles");
                                    machine.MinutesUntilReady = 10000;
                                    mp.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(256, 1856, 64, 128), 80f, 6, 999999, __instance.TileLocation * 64f + new Vector2(0f, -128f), flicker: false, flipped: false, (float)(((double)__instance.TileLocation.Y + 1.0) * 64.0 / 10000.0 + 9.99999974737875E-05), 0f, Color.Lavender * 0.75f, 1f, 0f, 0f, 0f)
                                    {
                                        alphaFade = 0.005f
                                    });
                                    Game1.player.removeItemsFromInventory(inputItem.ParentSheetIndex, totalInput);
                                }
                                return false;
                            case -75://Vegetable
                                machine.heldObject.Value = new StardewValley.Object(Vector2.Zero, 350, "BitchAss Juice", canBeSetDown: false, canBeGrabbed: true, isHoedirt: false, isSpawnedObject: false);
                                machine.heldObject.Value.Price = (int)((double)(inputItem.Price * 2.5));
                                if (!probe)
                                {
                                    machine.heldObject.Value.Stack = PerformObjectDropInActionPatch._output;
                                    machine.heldObject.Value.name = inputItem.Name + " Juice";
                                    machine.heldObject.Value.preserve.Value = StardewValley.Object.PreserveType.Juice;
                                    machine.heldObject.Value.preservedParentSheetIndex.Value = inputItem.ParentSheetIndex;
                                    who.currentLocation.playSound("Ship");
                                    who.currentLocation.playSound("bubbles");
                                    machine.MinutesUntilReady = 10000;
                                    mp.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(256, 1856, 64, 128), 80f, 6, 999999, __instance.TileLocation * 64f + new Vector2(0f, -128f), flicker: false, flipped: false, (float)(((double)__instance.TileLocation.Y + 1.0) * 64.0 / 10000.0 + 9.99999974737875E-05), 0f, Color.Lavender * 0.75f, 1f, 0f, 0f, 0f)
                                    {
                                        alphaFade = 0.005f
                                    });
                                    Game1.player.removeItemsFromInventory(inputItem.ParentSheetIndex, totalInput);
                                }
                                return false;
                                /*
                                machine.heldObject.Value = new SObject(Vector2.Zero, 350, "Bitch Juice",
                                    false, true, false, false);
                                machine.heldObject.Value.Price = (int)((double)(inputItem.Price * 2.5));
                                if (!probe)
                                {
                                    machine.heldObject.Value.Stack = _output;
                                    machine.heldObject.Value.name = inputItem.Name + " Juice";
                                   machine.heldObject.Value.preserve.Value = StardewValley.Object.PreserveType.Juice;//new SObject.PreserveType?(SObject.PreserveType.Juice);
                                    who.currentLocation.playSound("Ship");
                                    who.currentLocation.playSound("bubbles");
                                    machine.MinutesUntilReady = 4000;
                                    mp.broadcastSprites(who.currentLocation,
                                        new TemporaryAnimatedSprite("TileSheets\\animations",
                                            new Rectangle(256, 1856, 64, 128), 80f, 6, 999999,
                                            machine.TileLocation * 64f + new Vector2(0.0f, (float)sbyte.MinValue), false,
                                            false,
                                            (float)(((double)machine.TileLocation.Y + 1.0) * 64.0 / 10000.0 +
                                                    9.99999974737875E-05), 0.0f, Color.Yellow * 0.75f, 1f, 0.0f, 0.0f, 0.0f,
                                            false)
                                        {
                                            alphaFade = 0.005f
                                        });
                                    Game1.player.removeItemsFromInventory(inputItem.ParentSheetIndex, totalInput);
                                }
                                return false;*/
                        }
                        break;
                }
            }
            else if (machine.Name.Equals("Preserves Jar"))
            {
                //Need to check to see if Custom Mayo enabled.
                _input = _config.Machines.PreservesJar.CustomPreservesJarEnabled ? _config.Machines.PreservesJar.PreserveInputMultiplier : 1;
                _minOut = _config.Machines.PreservesJar.CustomPreservesJarEnabled ? _config.Machines.PreservesJar.PreserveMinOutput : 1;
                _maxOut = _config.Machines.PreservesJar.CustomPreservesJarEnabled ? _config.Machines.PreservesJar.PreserveMaxOutput : 1;
                _rnd = new Random();
                int totalInput = _config.Machines.PreservesJar.CustomPreservesJarEnabled ? _input * 1 : 1;
                _output = _rnd.Next(_minOut, _maxOut) * _input;

                //Make sure the player has enough items or fail
                if (Game1.player.getTallyOfObject(inputItem.ParentSheetIndex, false) < totalInput && !probe)
                {
                    Game1.showRedMessage($"You need {totalInput} {inputItem.Name}");
                    return false;
                }
                switch (inputItem.Category)
                {
                    case -79: //Fruit
                        machine.heldObject.Value = new SObject(Vector2.Zero, 344, inputItem.Name +" Jelly", false, true, false, false);
                        machine.heldObject.Value.Price = 50 + inputItem.Price * 2;
                        if (!probe)
                        {
                            machine.heldObject.Value.Stack = _output;
                            machine.heldObject.Value.Name = inputItem.Name + " Jelly";
                            machine.heldObject.Value.preserve.Value = SObject.PreserveType.Jelly;
                            machine.heldObject.Value.preservedParentSheetIndex.Value = inputItem.ParentSheetIndex;
                            machine.heldObject.Value.MinutesUntilReady = 4000;
                            who.currentLocation.playSound("Ship");
                            mp.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(256, 1856, 64, 128), 80f, 6, 999999, machine.TileLocation * 64f + new Vector2(0.0f, (float)sbyte.MinValue), false, false, (float)(((double)machine.TileLocation.Y + 1.0) * 64.0 / 10000.0 + 9.99999974737875E-05), 0.0f, Color.Yellow * 0.75f, 1f, 0.0f, 0.0f, 0.0f, false)
                            {
                                alphaFade = 0.005f
                            });
                            Game1.player.removeItemsFromInventory(inputItem.ParentSheetIndex, totalInput);
                        }
                        return false;
                    case -75: //Vegetable
                        machine.heldObject.Value = new SObject(Vector2.Zero, 342, "Pickled " + inputItem.Name, false, true, false, false);
                        machine.heldObject.Value.Price = 50 + inputItem.Price * 2;
                        if (!probe)
                        {
                            machine.heldObject.Value.Stack = _output;
                            machine.heldObject.Value.Name = "Pickled " + inputItem.Name;
                            machine.heldObject.Value.preserve.Value = SObject.PreserveType.Pickle;
                            machine.heldObject.Value.preservedParentSheetIndex.Value = inputItem.ParentSheetIndex;
                            machine.heldObject.Value.MinutesUntilReady = 4000;
                            who.currentLocation.playSound("Ship");
                            mp.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(256, 1856, 64, 128), 80f, 6, 999999, machine.TileLocation * 64f + new Vector2(0.0f, (float)sbyte.MinValue), false, false, (float)(((double)machine.TileLocation.Y + 1.0) * 64.0 / 10000.0 + 9.99999974737875E-05), 0.0f, Color.Yellow * 0.75f, 1f, 0.0f, 0.0f, 0.0f, false)
                            {
                                alphaFade = 0.005f
                            });
                            Game1.player.removeItemsFromInventory(inputItem.ParentSheetIndex, totalInput);
                        }
                        return false;
                }
                switch ((int)inputItem.ParentSheetIndex)
                {
                    case 829:
                        machine.heldObject.Value = new SObject(Vector2.Zero, 342, "Pickled " + inputItem.Name, canBeSetDown: false, canBeGrabbed: true, isHoedirt: false, isSpawnedObject: false);
                        machine.heldObject.Value.Price = 50 + inputItem.Price * 2;
                        if (!probe)
                        {
                            machine.heldObject.Value.name = "Pickled " + inputItem.Name;
                            machine.heldObject.Value.Stack = _output;
                            machine.heldObject.Value.preserve.Value = SObject.PreserveType.Pickle;
                            machine.heldObject.Value.preservedParentSheetIndex.Value = inputItem.ParentSheetIndex;
                            who.currentLocation.playSound("Ship");
                            machine.MinutesUntilReady = 4000;
                            mp.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(256, 1856, 64, 128), 80f, 6, 999999, machine.TileLocation * 64f + new Vector2(0f, -128f), flicker: false, flipped: false, (machine.TileLocation.Y + 1f) * 64f / 10000f + 0.0001f, 0f, Color.White * 0.75f, 1f, 0f, 0f, 0f)
                            {
                                alphaFade = 0.005f
                            });
                        }
                        return false;
                    case 812:
                        {
                            if ((int)inputItem.preservedParentSheetIndex.Value == 698)
                            {
                                machine.heldObject.Value = new SObject(445, 1);
                                if (!probe)
                                {
                                    machine.MinutesUntilReady = 6000;
                                    who.currentLocation.playSound("Ship");
                                    mp.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(256, 1856, 64, 128), 80f, 6, 999999, machine.TileLocation * 64f + new Vector2(0f, -128f), flicker: false, flipped: false, (machine.TileLocation.Y + 1f) * 64f / 10000f + 0.0001f, 0f, Color.LightBlue * 0.75f, 1f, 0f, 0f, 0f)
                                    {
                                        alphaFade = 0.005f
                                    });
                                }
                                return false;
                            }
                            SObject aged_roe = null;
                            ColoredObject colored_object;
                            aged_roe = (((colored_object = (inputItem as ColoredObject)) == null) ? new SObject(447, 1) : new ColoredObject(447, 1, colored_object.color));
                            machine.heldObject.Value = aged_roe;
                            machine.heldObject.Value.Price = inputItem.Price * 2;
                            if (!probe)
                            {
                                machine.MinutesUntilReady = 4000;
                                machine.heldObject.Value.Stack = _output;
                                machine.heldObject.Value.name = "Aged " + inputItem.Name;
                                machine.heldObject.Value.preserve.Value = SObject.PreserveType.AgedRoe;
                                machine.heldObject.Value.Category = -26;
                                machine.heldObject.Value.preservedParentSheetIndex.Value = inputItem.preservedParentSheetIndex.Value;
                                who.currentLocation.playSound("Ship");
                                mp.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(256, 1856, 64, 128), 80f, 6, 999999, machine.TileLocation * 64f + new Vector2(0f, -128f), flicker: false, flipped: false, (machine.TileLocation.Y + 1f) * 64f / 10000f + 0.0001f, 0f, Color.LightBlue * 0.75f, 1f, 0f, 0f, 0f)
                                {
                                    alphaFade = 0.005f
                                });
                            }
                            return false;
                        }
                }
            }
            else if (machine.Name.Equals("Cheese Press"))
            {
                //Need to check to see if Custom Mayo enabled.
                _input = _config.Machines.CheesePress.CustomCheesePressEnabled ? _config.Machines.CheesePress.CheesePressInputMultiplier : 1;
                _minOut = _config.Machines.CheesePress.CustomCheesePressEnabled ? _config.Machines.CheesePress.CheesePressMinOutput : 1;
                _maxOut = _config.Machines.CheesePress.CustomCheesePressEnabled ? _config.Machines.CheesePress.CheesePressMaxOutput : 1;
                _rnd = new Random();
                int totalInput = _config.Machines.CheesePress.CustomCheesePressEnabled ? _input * 1 : 1;
                _output = _rnd.Next(_minOut, _maxOut) * _input;

                //Make sure the player has enough items or fail
                if (Game1.player.getTallyOfObject(inputItem.ParentSheetIndex, false) < totalInput && !probe)
                {
                    Game1.showRedMessage($"You need {totalInput} {inputItem.Name}");
                    return false;
                }
                switch (inputItem.ParentSheetIndex)
                {
                    case 184: //Milk
                        machine.heldObject.Value = new SObject(Vector2.Zero, 424, null, false, true, false, false);
                        if (!probe)
                        {
                            machine.heldObject.Value.Stack = _output;
                            machine.heldObject.Value.MinutesUntilReady = 200;
                            Game1.player.currentLocation.playSound("Ship");
                            Game1.player.removeItemsFromInventory(inputItem.ParentSheetIndex, totalInput);
                        }
                        return false;
                    case 186: //Large Milk
                        machine.heldObject.Value = new SObject(Vector2.Zero, 424, null, false, true, false, false)
                        {
                            Quality = 2
                        };
                        if (!probe)
                        {
                            machine.heldObject.Value.Stack = _output;
                            machine.heldObject.Value.MinutesUntilReady = 200;
                            Game1.player.currentLocation.playSound("Ship");
                            Game1.player.removeItemsFromInventory(inputItem.ParentSheetIndex, totalInput);
                        }
                        return false;
                    case 436: //Goat Milk
                        machine.heldObject.Value = new SObject(Vector2.Zero, 426, null, false, true, false, false);
                        if (!probe)
                        {
                            machine.heldObject.Value.Stack = _output;
                            machine.heldObject.Value.MinutesUntilReady = 200;
                            Game1.player.currentLocation.playSound("Ship");
                            Game1.player.removeItemsFromInventory(inputItem.ParentSheetIndex, totalInput);
                        }
                        return false;
                    case 438: //Large Goat Milk
                        machine.heldObject.Value = new SObject(Vector2.Zero, 426, null, false, true, false, false)
                        {
                            Quality = 2
                        };
                        if (!probe)
                        {
                            machine.heldObject.Value.Stack = _output;
                            machine.heldObject.Value.MinutesUntilReady = 200;
                            Game1.player.currentLocation.playSound("Ship");
                            Game1.player.removeItemsFromInventory(inputItem.ParentSheetIndex, totalInput);
                        }
                        return false;
                }
            }
            else if (machine.Name.Equals("Mayonnaise Machine"))
            {
                //Need to check to see if Custom Mayo enabled.
                _input = _config.Machines.Mayonnaise.CustomMayoEnabled ? _config.Machines.Mayonnaise.MayoInputMultiplier : 1;
                _minOut = _config.Machines.Mayonnaise.CustomMayoEnabled ? _config.Machines.Mayonnaise.MayoMinOutput : 1;
                _maxOut = _config.Machines.Mayonnaise.CustomMayoEnabled ? _config.Machines.Mayonnaise.MayoMaxOutput : 1;
                _rnd = new Random();
                int totalInput = _config.Machines.Mayonnaise.CustomMayoEnabled ? _input * 1 : 1;
                _output = _rnd.Next(_minOut, _maxOut) * _input;

                //Make sure the player has enough input items
                //Make sure the player has enough items or fail
                if (Game1.player.getTallyOfObject(inputItem.ParentSheetIndex, false) < totalInput && !probe)
                {
                    Game1.showRedMessage($"You need {totalInput} {inputItem.Name}");
                    return false;
                }

                //Need to do Switch for beer,wine,etc
                switch (inputItem.ParentSheetIndex)
                {
                    case 107: //Dino Egg
                    case 174: //Large Egg
                    case 182: //Large Egg
                        machine.heldObject.Value = new SObject(Vector2.Zero, 306, null, false, true, false, false)
                        {
                            Quality = 2
                        };
                        if (!probe)
                        {
                            machine.heldObject.Value.Stack = _output;
                            machine.MinutesUntilReady = 180;
                            who.currentLocation.playSound("Ship");
                            Game1.player.removeItemsFromInventory(inputItem.ParentSheetIndex, totalInput);
                        }
                        return false;
                    case 176: //Egg
                    case 180: //Egg
                        machine.heldObject.Value = new SObject(Vector2.Zero, 306, null, false, true, false, false);
                        if (!probe)
                        {
                            machine.heldObject.Value.Stack = _output;
                            machine.MinutesUntilReady = 180;
                            who.currentLocation.playSound("Ship");
                            Game1.player.removeItemsFromInventory(inputItem.ParentSheetIndex, totalInput);
                        }
                        return false;
                    case 305: //Void Egg
                        machine.heldObject.Value = new SObject(Vector2.Zero, 308, null, false, true, false, false);
                        if (!probe)
                        {
                            machine.heldObject.Value.Stack = _output;
                            machine.MinutesUntilReady = 180;
                            who.currentLocation.playSound("Ship");
                            Game1.player.removeItemsFromInventory(inputItem.ParentSheetIndex, totalInput);
                        }
                        return false;
                    case 442: //Duck Egg
                        machine.heldObject.Value = new SObject(Vector2.Zero, 307, null, false, true, false, false);
                        if (!probe)
                        {
                            machine.heldObject.Value.Stack = _output;
                            machine.MinutesUntilReady = 180;
                            who.currentLocation.playSound("Ship");
                            Game1.player.removeItemsFromInventory(inputItem.ParentSheetIndex, totalInput);
                        }
                        return false;
                }
            }
            else if (machine.Name.Equals("Loom"))
            {
                //Need to check to see if Custom Mayo enabled.
                _input = _config.Machines.Loom.CustomLoomEnabled ? _config.Machines.Loom.LoomInputMultiplier : 1;
                _minOut = _config.Machines.Loom.CustomLoomEnabled ? _config.Machines.Loom.LoomMinOutput : 1;
                _maxOut = _config.Machines.Loom.CustomLoomEnabled ? _config.Machines.Loom.LoomMaxOutput : 1;
                _rnd = new Random();
                int totalInput = _config.Machines.Loom.CustomLoomEnabled ? _input * 1 : 1;
                _output = _rnd.Next(_minOut, _maxOut) * _input;

                //Make sure the player has enough items or fail
                if (Game1.player.getTallyOfObject(inputItem.ParentSheetIndex, false) < totalInput && !probe)
                {
                    Game1.showRedMessage($"You need {totalInput} {inputItem.Name}");
                    return false;
                }
                if (inputItem.ParentSheetIndex == 440)//Wool
                {
                    machine.heldObject.Value = new SObject(Vector2.Zero, 428, null, false, true, false, false);
                    if (!probe)
                    {
                        machine.heldObject.Value.Stack = _output;
                        machine.heldObject.Value.MinutesUntilReady = 240;
                        Game1.player.currentLocation.playSound("Ship");
                        Game1.player.removeItemsFromInventory(inputItem.ParentSheetIndex, totalInput);
                    }
                    return false;
                }
            }
            else if (machine.Name.Equals("Oil Maker"))
            {
                //Need to check to see if Custom Mayo enabled.
                _input = _config.Machines.OilMaker.CustomOilMakerEnabled ? _config.Machines.OilMaker.OilMakerInputMultiplier : 1;
                _minOut = _config.Machines.OilMaker.CustomOilMakerEnabled ? _config.Machines.OilMaker.OilMakerMinOutput : 1;
                _maxOut = _config.Machines.OilMaker.CustomOilMakerEnabled ? _config.Machines.OilMaker.OilMakerMaxOutput : 1;
                _rnd = new Random();
                int totalInput = _config.Machines.OilMaker.CustomOilMakerEnabled ? _input * 1 : 1;
                _output = _rnd.Next(_minOut, _maxOut) * _input;

                //Make sure the player has enough items or fail
                if (Game1.player.getTallyOfObject(inputItem.ParentSheetIndex, false) < totalInput && !probe)
                {
                    Game1.showRedMessage($"You need {totalInput} {inputItem.Name}");
                    return false;
                }
                switch (inputItem.ParentSheetIndex)
                {
                    case 270: //Corn
                        machine.heldObject.Value = new SObject(Vector2.Zero, 247, null, false, true, false, false);
                        if (!probe)
                        {
                            machine.heldObject.Value.Stack = _output;
                            machine.heldObject.Value.MinutesUntilReady = 1000;
                            Game1.player.currentLocation.playSound("bubbles");
                            Game1.player.currentLocation.playSound("sipTea");
                            mp.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(256, 1856, 64, 128), 80f, 6, 999999, machine.TileLocation * 64f + new Vector2(0.0f, (float)sbyte.MinValue), false, false, (float)(((double)machine.TileLocation.Y + 1.0) * 64.0 / 10000.0 + 9.99999974737875E-05), 0.0f, Color.Yellow * 0.75f, 1f, 0.0f, 0.0f, 0.0f, false)
                            {
                                alphaFade = 0.005f
                            });
                            Game1.player.removeItemsFromInventory(inputItem.ParentSheetIndex, totalInput);
                        }
                        return false;
                    case 421: //Sunflower
                        machine.heldObject.Value = new SObject(Vector2.Zero, 247, null, false, true, false, false);
                        if (!probe)
                        {
                            machine.heldObject.Value.Stack = _output;
                            machine.heldObject.Value.MinutesUntilReady = 60;
                            Game1.player.currentLocation.playSound("bubbles");
                            Game1.player.currentLocation.playSound("sipTea");
                            mp.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(256, 1856, 64, 128), 80f, 6, 999999, machine.TileLocation * 64f + new Vector2(0.0f, (float)sbyte.MinValue), false, false, (float)(((double)machine.TileLocation.Y + 1.0) * 64.0 / 10000.0 + 9.99999974737875E-05), 0.0f, Color.Yellow * 0.75f, 1f, 0.0f, 0.0f, 0.0f, false)
                            {
                                alphaFade = 0.005f
                            });
                            Game1.player.removeItemsFromInventory(inputItem.ParentSheetIndex, totalInput);
                        }
                        return false;
                    case 430: //Truffle
                        machine.heldObject.Value = new SObject(Vector2.Zero, 432, null, false, true, false, false);
                        if (!probe)
                        {
                            machine.heldObject.Value.Stack = _output;
                            machine.heldObject.Value.MinutesUntilReady = 360;
                            Game1.player.currentLocation.playSound("bubbles");
                            Game1.player.currentLocation.playSound("sipTea");
                            mp.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(256, 1856, 64, 128), 80f, 6, 999999, machine.TileLocation * 64f + new Vector2(0.0f, (float)sbyte.MinValue), false, false, (float)(((double)machine.TileLocation.Y + 1.0) * 64.0 / 10000.0 + 9.99999974737875E-05), 0.0f, Color.Yellow * 0.75f, 1f, 0.0f, 0.0f, 0.0f, false)
                            {
                                alphaFade = 0.005f
                            });
                            Game1.player.removeItemsFromInventory(inputItem.ParentSheetIndex, totalInput);
                        }
                        return false;
                    case 431: //Sunflower Seeds
                        machine.heldObject.Value = new SObject(Vector2.Zero, 247, null, false, true, false, false);
                        if (!probe)
                        {
                            machine.heldObject.Value.Stack = _output;
                            machine.heldObject.Value.MinutesUntilReady = 3200;
                            Game1.player.currentLocation.playSound("bubbles");
                            Game1.player.currentLocation.playSound("sipTea");
                            mp.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(256, 1856, 64, 128), 80f, 6, 999999, machine.TileLocation * 64f + new Vector2(0.0f, (float)sbyte.MinValue), false, false, (float)(((double)machine.TileLocation.Y + 1.0) * 64.0 / 10000.0 + 9.99999974737875E-05), 0.0f, Color.Yellow * 0.75f, 1f, 0.0f, 0.0f, 0.0f, false)
                            {
                                alphaFade = 0.005f
                            });
                            Game1.player.removeItemsFromInventory(inputItem.ParentSheetIndex, totalInput);
                        }
                        return false;
                }
            }
            else if (machine.Name.Equals("Seed Maker"))
            {
                //Need to check to see if Custom seedmaker enabled.
                _input = _config.Machines.SeedMaker.CustomSeedMakerEnabled ? _config.Machines.SeedMaker.SeedMakerInputMultiplier : 1;
                _minOut = _config.Machines.SeedMaker.CustomSeedMakerEnabled ? _config.Machines.SeedMaker.SeedMakerMinOutput : 1;
                _maxOut = _config.Machines.SeedMaker.CustomSeedMakerEnabled ? _config.Machines.SeedMaker.SeedMakerMaxOutput : 1;
                _rnd = _config.Machines.SeedMaker.CustomSeedMakerEnabled ? new Random() : new Random((int)Game1.stats.DaysPlayed + (int)Game1.uniqueIDForThisGame / 2 + (int)machine.TileLocation.X + (int)machine.TileLocation.Y * 77 + Game1.timeOfDay);
                int totalInput = _config.Machines.SeedMaker.CustomSeedMakerEnabled ? _input * 1 : 1;
                _output = _config.Machines.SeedMaker.CustomSeedMakerEnabled ? _rnd.Next(_minOut, _maxOut) * _input : _rnd.Next(1, 4) * _input;
                int outputAdded = _config.Machines.SeedMaker.MoreSeedsForQuality ? inputItem.Quality : 0;

                if (inputItem.ParentSheetIndex == 433)
                    return false;
                //Make sure the player has enough items or fail
                if (!probe && !HasEnough(inputItem.ParentSheetIndex, totalInput))
                {
                    Game1.showRedMessage($"You need {totalInput} {inputItem.Name}");
                    return false;
                }

                //Lets grab the crop data
                int pts = -1;
                bool found = false;
                Dictionary<int, string> crops = Game1.temporaryContent.Load<Dictionary<int, string>>("Data\\Crops");
                foreach (var crop in crops)
                {
                    if (Convert.ToInt32(crop.Value.Split('/')[3]) == inputItem.ParentSheetIndex)
                    {
                        found = true;
                        pts = crop.Key;
                        break;
                    }
                }

                //Crop data found. Now we start doing the magic.
                if (found)
                {
                    if (!probe)
                    {
                        machine.heldObject.Value = new SObject(pts, _output + outputAdded, false, -1, 0);
                        if (_rnd.NextDouble() < 0.005)
                            machine.heldObject.Value = new SObject(499, _output + outputAdded, false, -1, 0);
                        else if (_rnd.NextDouble() < 0.02)
                            machine.heldObject.Value = new SObject(770, _output + outputAdded, false, -1, 0);
                        who.currentLocation.playSound("Ship");
                        DelayedAction.playSoundAfterDelay("dirtyHit", 250, (GameLocation)null);
                        //inputItem.Stack -= totalInput;
                        who.ActiveObject.Stack -= totalInput;
                        if(who.ActiveObject.Stack <= 0)
                            who.removeItemFromInventory((Item)who.ActiveObject);
                    }
                    return true;
                }

            }
            else if (machine.Name.Equals("Crystalarium"))
            {
                //Need to check to see if Custom Mayo enabled.
                _input = _config.Machines.Crystalarium.CustomCrystalariumEnabled ? _config.Machines.Crystalarium.CrystalInputMultiplier : 1;
                _minOut = _config.Machines.Crystalarium.CustomCrystalariumEnabled ? _config.Machines.Crystalarium.CrystalMinOutput : 1;
                _maxOut = _config.Machines.Crystalarium.CustomCrystalariumEnabled ? _config.Machines.Crystalarium.CystalMaxOutput : 1;
                _rnd = new Random();
                int totalInput = _config.Machines.Crystalarium.CustomCrystalariumEnabled ? _input * 1 : 1;
                _output = _rnd.Next(_minOut, _maxOut) * _input;

                //Make sure the player has enough items or fail
                if (Game1.player.getTallyOfObject(inputItem.ParentSheetIndex, false) < totalInput && !probe)
                {
                    Game1.showRedMessage($"You need {totalInput} {inputItem.Name}");
                    return false;
                }
                if ((inputItem.Category == -2 || inputItem.Category == -12) &&
                    inputItem.ParentSheetIndex != 74 &&
                    (machine.heldObject.Value == null ||
                     machine.heldObject.Value.ParentSheetIndex != inputItem.ParentSheetIndex) &&
                    (machine.heldObject.Value == null || machine.heldObject.Value.MinutesUntilReady > 0))
                {
                    machine.heldObject.Value = (SObject) inputItem.getOne();
                    if (!probe)
                    {
                        machine.heldObject.Value.Stack = _output;
                        machine.heldObject.Value.MinutesUntilReady =
                            GetMinutesForCrystalarium(inputItem.ParentSheetIndex);
                        Game1.player.removeItemsFromInventory(inputItem.ParentSheetIndex, totalInput);
                    }
                }
            }
            else if (machine.Name.Equals("Recycling Machine"))
            {
                //Need to check to see if Custom Recycling Machine is enabled.
                _input = _config.Machines.Recycling.CustomRecyclingEnabled ? _config.Machines.Recycling.CustomRecyclingInputMultiplier : 1;
                _rnd = _rnd = _config.Machines.Recycling.CustomRecyclingEnabled ? new Random() : new Random((int)Game1.uniqueIDForThisGame / 2 + (int)Game1.stats.DaysPlayed + Game1.timeOfDay + (int)machine.TileLocation.X * 200 + (int)machine.TileLocation.Y);
                int totalInput = _config.Machines.Recycling.CustomRecyclingEnabled ? _input * 1 : 1;
                int pts = 0;
                bool replaceStoneWithOreEnabled = _config.Machines.Recycling.ReplaceStoneWithOreEnabled;
                int stoneMinOutput = _config.Machines.Recycling.CustomRecyclingEnabled
                    ? _config.Machines.Recycling.StoneMinOutput
                    : 1;
                int stoneMaxOutput = _config.Machines.Recycling.CustomRecyclingEnabled ? _config.Machines.Recycling.StoneMaxOutput : 4;
                double stoneToCopperChance = _config.Machines.Recycling.StoneToCopperChance;
                double stoneToIronChance = _config.Machines.Recycling.StoneToIronChance;
                double stoneToGoldChance = _config.Machines.Recycling.StoneToGoldChance;
                double stoneToIridiumChance = _config.Machines.Recycling.StoneToIridiumChance;
                bool replaceOreOutput = _config.Machines.Recycling.ReplaceOreOutput;
                int copperMinOutput = _config.Machines.Recycling.CustomRecyclingEnabled ? _config.Machines.Recycling.CopperMinOutput : 1;
                int copperMaxOutput = _config.Machines.Recycling.CustomRecyclingEnabled ? _config.Machines.Recycling.CopperMaxOutput : 1;
                int ironMinOutput = _config.Machines.Recycling.CustomRecyclingEnabled ? _config.Machines.Recycling.IronMinOutput : 1;
                int ironMaxOutput = _config.Machines.Recycling.CustomRecyclingEnabled ? _config.Machines.Recycling.IronMaxOutput : 4;
                int goldMinOutput = _config.Machines.Recycling.CustomRecyclingEnabled ? _config.Machines.Recycling.GoldMinOutput : 1;
                int goldMaxOutput = _config.Machines.Recycling.CustomRecyclingEnabled ? _config.Machines.Recycling.GoldMaxOutput : 1;
                int iridiumMinOutput = _config.Machines.Recycling.CustomRecyclingEnabled ? _config.Machines.Recycling.IridiumMinOutput : 1;
                int iridiumMaxOutput = _config.Machines.Recycling.CustomRecyclingEnabled ? _config.Machines.Recycling.IridiumMaxOutput : 1;
                int woodMinOutput = _config.Machines.Recycling.CustomRecyclingEnabled ? _config.Machines.Recycling.WoodMinOutput : 1;
                int woodMaxOutput = _config.Machines.Recycling.CustomRecyclingEnabled ? _config.Machines.Recycling.WoodMaxOutput : 4;
                int refinedQuartzMinOutput = _config.Machines.Recycling.CustomRecyclingEnabled ? _config.Machines.Recycling.RefinedQuartzMinOutput : 1;
                int refinedQuartzMaxOutput = _config.Machines.Recycling.CustomRecyclingEnabled ? _config.Machines.Recycling.RefinedQuartzMaxOutput : 1;
                int coalMinOutput = _config.Machines.Recycling.CustomRecyclingEnabled ? _config.Machines.Recycling.CoalMinOutput : 1;
                int coalMaxOutput = _config.Machines.Recycling.CustomRecyclingEnabled ? _config.Machines.Recycling.CoalMaxOutput : 1;
                int clothMinOutput = _config.Machines.Recycling.CustomRecyclingEnabled ? _config.Machines.Recycling.ClothMinOutput : 1;
                int clothMaxOutput = _config.Machines.Recycling.CustomRecyclingEnabled ? _config.Machines.Recycling.ClothMaxOutput : 1;
                int torchMinOutput = _config.Machines.Recycling.CustomRecyclingEnabled ? _config.Machines.Recycling.TorchMinOutput : 3;
                int torchMaxOutput = _config.Machines.Recycling.CustomRecyclingEnabled ? _config.Machines.Recycling.TorchMaxOutput : 3;
                //Make sure the player has enough items or fail
                if (Game1.player.getTallyOfObject(inputItem.ParentSheetIndex, false) < totalInput && !probe)
                {
                    Game1.showRedMessage($"You need {totalInput} {inputItem.Name}");
                    return false;
                }
                
                if (inputItem.ParentSheetIndex >= 168 && inputItem.ParentSheetIndex <= 172 &&
                    machine.heldObject.Value == null)
                {
                    switch (inputItem.ParentSheetIndex)
                    {
                        case 168: //Trash to 382 = Coal, 380 = Iron Ore, 390 = Stone. Amt Random 1 - 4
                            if (_rnd.NextDouble() < 0.3)
                                pts = 382;
                            else if (_rnd.NextDouble() < 0.3)
                                pts = 380;
                            else
                                pts = 390;
                            break;
                        case 169: //Driftwood to 382 = Coal, 388 = Wood. Amt 1 - 4
                            pts = _rnd.NextDouble() < 0.3 ? 382 : 388;
                            break;
                        case 170: //Broken Glasses to 338 = Refined Quartz. Amt: 1
                        case 171: //Broken CD to 338 = Refined Quartz. Amt: 1
                            pts = 338;
                            break;
                        case 172: //Soggy Newspaper to 428 = Cloth, 93 = Torch. Amt 428: 1, 93: 3
                            pts = _rnd.NextDouble() < 0.1 ? 428 : 93;
                            break;
                    }
                    if (!probe)
                    {
                        int recOut = 0;
                        int newPts = 0;
                        switch (pts)
                        {
                            case 93://Torch Amt: 3
                                recOut = _rnd.Next(torchMinOutput, torchMaxOutput) * _input;
                                newPts = 93;
                                break;
                            case 338://Refined Quartz Amt: 1
                                recOut = _rnd.Next(refinedQuartzMinOutput, refinedQuartzMaxOutput) * _input;
                                newPts = 338;
                                break;
                            case 380://IronOre Amt 1 - 4
                                if (replaceOreOutput && _rnd.NextDouble() < stoneToCopperChance)
                                {
                                    recOut = _rnd.Next(copperMinOutput, copperMaxOutput) * _input;
                                    newPts = 378;
                                }
                                else if (replaceOreOutput && _rnd.NextDouble() < stoneToGoldChance)
                                {
                                    recOut = _rnd.Next(goldMinOutput, goldMaxOutput) * _input;
                                    newPts = 384;
                                }
                                else if (replaceOreOutput && _rnd.NextDouble() < stoneToIridiumChance)
                                {
                                    recOut = _rnd.Next(iridiumMinOutput, iridiumMaxOutput) * _input;
                                    newPts = 386;
                                }
                                else
                                {
                                    recOut = _rnd.Next(ironMinOutput, ironMaxOutput) * _input;
                                    newPts = 380;
                                }
                                break;
                            case 382://Coal Amt: 1 - 4
                                recOut = _rnd.Next(coalMinOutput, coalMaxOutput) * _input;
                                newPts = 382;
                                break;
                            case 388://Wood Amt: 1 - 4
                                recOut = _rnd.Next(woodMinOutput, woodMaxOutput) * _input;
                                newPts = 388;
                                break;
                            case 390://Stone Amt: 1 - 4
                                if (replaceStoneWithOreEnabled && _rnd.NextDouble() < stoneToCopperChance)
                                {
                                    recOut = _rnd.Next(copperMinOutput, copperMaxOutput) * _input;
                                    newPts = 378;
                                }
                                else if (replaceStoneWithOreEnabled && _rnd.NextDouble() < stoneToIronChance)
                                {
                                    recOut = _rnd.Next(ironMinOutput, ironMaxOutput) * _input;
                                    newPts = 380;
                                }
                                else if (replaceStoneWithOreEnabled && _rnd.NextDouble() < stoneToGoldChance)
                                {
                                    recOut = _rnd.Next(goldMinOutput, goldMaxOutput) * _input;
                                    newPts = 384;
                                }
                                else if (replaceStoneWithOreEnabled && _rnd.NextDouble() < stoneToIridiumChance)
                                {
                                    recOut = _rnd.Next(iridiumMinOutput, iridiumMaxOutput) * _input;
                                    newPts = 386;
                                }
                                else
                                {
                                    recOut = _rnd.Next(stoneMinOutput, stoneMaxOutput * _input);
                                    newPts = 390;
                                }
                                
                                break;
                            case 428://Cloth Amt: 1
                                recOut = _rnd.Next(clothMinOutput, clothMaxOutput) * _input;
                                newPts = 428;
                                break;
                        }
                        machine.heldObject.Value = newPts == 93 ? new Torch(Vector2.Zero, recOut) : new SObject(Vector2.Zero, newPts, recOut);
                        machine.heldObject.Value.MinutesUntilReady = 60;
                        who.removeItemsFromInventory(inputItem.ParentSheetIndex, 1 * _input);
                        Game1.stats.PiecesOfTrashRecycled += (uint)recOut;
                        return false;
                    }
                }
            }
            else if (machine.Name.Equals("Furnace"))
            {
                
                //Check to make sure we should be customizing the Furnace.
                if (_config.Machines.Furnace.CustomFurnaceEnabled)
                {
                    _input = _config.Machines.Furnace.FurnaceInputMultiplier;
                    _minOut = _config.Machines.Furnace.FurnaceMinOutput;
                    _maxOut = _config.Machines.Furnace.FurnaceMaxOutput;
                    _rnd = new Random();
                    _output = _rnd.Next(_minOut, _maxOut) * _input;
                }
                int totalCoalInput = _config.Machines.Furnace.CustomFurnaceEnabled ? _input : 1;
                int totalOreInput = _config.Machines.Furnace.CustomFurnaceEnabled ? 5 * _input : 5;
                int totalOutput = _config.Machines.Furnace.CustomFurnaceEnabled ? _output : 1;

                if (who.IsLocalPlayer && who.getTallyOfObject(382, false) <= totalCoalInput - 1)
                {
                    if(!probe && who.IsLocalPlayer)
                        Game1.showRedMessage($"Requires {totalCoalInput} Coal.");
                    return false;
                }
                //Found the coal
                if (machine.heldObject.Value == null && !probe)
                {
                    if (inputItem.Stack <= totalOreInput && inputItem.ParentSheetIndex != 80 &&
                        inputItem.ParentSheetIndex != 82 && inputItem.ParentSheetIndex != 330)
                    {
                        Game1.showRedMessage($"You need {totalOreInput} ores.");
                        return false;
                    }
                    //Switch ParentSheetIndex
                    switch (inputItem.ParentSheetIndex)
                    {
                        case 80:
                            machine.heldObject.Value =
                                new SObject(Vector2.Zero, 338, "Refined Quartz", false, true, false, false);
                            machine.MinutesUntilReady = 90;
                            break;
                        case 82:
                            machine.heldObject.Value = new SObject(338, totalOutput);
                            machine.MinutesUntilReady = 90;
                            break;
                        case 378:
                            machine.heldObject.Value = new SObject(Vector2.Zero, 334, totalOutput);
                            machine.MinutesUntilReady = 30;
                            break;
                        case 380:
                            machine.heldObject.Value = new SObject(Vector2.Zero, 335, totalOutput);
                            machine.MinutesUntilReady = 120;
                            break;
                        case 384:
                            machine.heldObject.Value = new SObject(Vector2.Zero, 336, totalOutput);
                            machine.MinutesUntilReady = 300;
                            break;
                        case 386:
                            machine.heldObject.Value = new SObject(Vector2.Zero, 337, totalOutput);
                            machine.MinutesUntilReady = 480;
                            break;
                        default:
                            return false;
                    }
                    Game1.currentLocation.playSound("furnace");
                    machine.initializeLightSource(machine.TileLocation, false);
                    machine.showNextIndex.Value = true;
                    mp.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite(30, machine.TileLocation * 64f + new Vector2(0.0f, -16f), Color.White, 4, false, 50f, 10, 64, (float)(((double)machine.TileLocation.Y + 1.0) * 64.0 / 10000.0 + 9.99999974737875E-05), -1, 0)
                    {
                        alphaFade = 0.005f
                    });

                    Game1.player.removeItemsFromInventory(382, totalCoalInput);
                    inputItem.Stack -= totalOreInput;
                    return inputItem.Stack <= 0;
                }
                if (machine.heldObject.Value == null && probe)
                {
                    switch (inputItem.ParentSheetIndex)
                    {
                        case 80:
                        case 82:
                        case 378:
                        case 380:
                        case 384:
                        case 386:
                            machine.heldObject.Value = new SObject();
                            return true;
                    }
                }
            }
            else if (machine.Name.Equals("Charcoal Kiln"))
            {
                //Need to check to see if Custom Mayo enabled.
                _input = _config.Machines.Charcoal.CustomCharcoalEnabled ? _config.Machines.Charcoal.CharcoalInputMultiplier : 1;
                _minOut = _config.Machines.Charcoal.CustomCharcoalEnabled ? _config.Machines.Charcoal.CharcoalMinOutput : 1;
                _maxOut = _config.Machines.Charcoal.CustomCharcoalEnabled ? _config.Machines.Charcoal.CharcoalMaxOutput : 1;
                _rnd = new Random();
                int totalInput = _config.Machines.Charcoal.CustomCharcoalEnabled ? _input * 1 : 1;
                _output = _rnd.Next(_minOut, _maxOut) * _input;

                //Make sure the player has enough items or fail
                if (!probe && (inputItem.ParentSheetIndex != 388 || Game1.player.getTallyOfObject(388, false) < totalInput))
                {
                    Game1.showRedMessage($"You need {totalInput} {inputItem.Name}");
                    return false;
                }
                if (!probe && machine.heldObject.Value == null && (inputItem.ParentSheetIndex == 388 || Game1.player.getTallyOfObject(388, false) > totalInput))
                {
                    machine.heldObject.Value = new SObject(382, _output) {Stack = _output};
                    Game1.player.currentLocation.playSound("openBox");
                    DelayedAction.playSoundAfterDelay("fireball", 50, who.currentLocation);
                    mp.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(256, 1856, 64, 128), 80f, 6, 999999, machine.TileLocation * 64f + new Vector2(0.0f, (float)sbyte.MinValue), false, false, (float)(((double)machine.TileLocation.Y + 1.0) * 64.0 / 10000.0 + 9.99999974737875E-05), 0.0f, Color.Yellow * 0.75f, 1f, 0.0f, 0.0f, 0.0f, false)
                    {
                        alphaFade = 0.005f
                    });
                    Game1.player.removeItemsFromInventory(inputItem.ParentSheetIndex, totalInput);
                }
                else if (machine.heldObject.Value == null && probe && inputItem.ParentSheetIndex == 388 &&
                         who.getTallyOfObject(388, false) > 10)
                {
                    machine.heldObject.Value = new SObject();
                    return true;
                }

            }
            else if (machine.Name.Equals("Slime Egg-Press"))
            {
                //Need to check to see if Custom slime enabled.
                _input = _config.Machines.SlimeEggPress.CustomSlimeEggPressEnabled ? _config.Machines.SlimeEggPress.SlimeInputMultiplier : 100;
                _minOut = _config.Machines.SlimeEggPress.CustomSlimeEggPressEnabled ? _config.Machines.SlimeEggPress.SlimeMinOutput : 1;
                _maxOut = _config.Machines.SlimeEggPress.CustomSlimeEggPressEnabled ? _config.Machines.SlimeEggPress.SlimeMaxOutput : 1;
                _rnd = new Random();
                int totalInput = _config.Machines.SlimeEggPress.CustomSlimeEggPressEnabled ? _input * 100 : 1;
                _output = _rnd.Next(_minOut, _maxOut) * _input;

                //Make sure the player has enough items or fail
                if (!probe && (inputItem.ParentSheetIndex != 766 || !HasEnough(766, totalInput)))
                {
                    Game1.showRedMessage($"You need {totalInput} {inputItem.Name}");
                    return false;
                }
                if (machine.heldObject.Value == null && !probe &&
                    (inputItem.ParentSheetIndex == 766 && HasEnough(766, totalInput)))
                {
                    int pts = 680;
                    //Get random chance of the egg Output
                    if (Game1.random.NextDouble() < 0.05)
                        pts = 439;
                    else if (Game1.random.NextDouble() < 0.1)
                        pts = 437;
                    else if (Game1.random.NextDouble() < 0.25)
                        pts = 413;
                    machine.heldObject.Value = new SObject(pts, _output, false, -1, 0);
                    machine.heldObject.Value.MinutesUntilReady = 1200;
                    who.currentLocation.playSound("slimeHit");
                    DelayedAction.playSoundAfterDelay("bubbles", 50, (GameLocation)null);
                    mp.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(256, 1856, 64, 128), 80f, 6, 999999, machine.TileLocation * 64f + new Vector2(0.0f, (float)sbyte.MinValue), false, false, (float)(((double)machine.TileLocation.Y + 1.0) * 64.0 / 10000.0 + 9.99999974737875E-05), 0.0f, Color.Yellow * 0.75f, 1f, 0.0f, 0.0f, 0.0f, false)
                    {
                        alphaFade = 0.005f
                    });
                    Game1.player.removeItemsFromInventory(inputItem.ParentSheetIndex, totalInput);
                }
                else if (machine.heldObject.Value == null && probe &&
                         (inputItem.ParentSheetIndex == 766 && HasEnough(766, totalInput)))
                {
                    machine.heldObject.Value = new SObject();
                    return true;
                }
            }
            else if (machine.Name.Contains("Hopper") && inputItem.ParentSheetIndex == 178)
            {
                if (!probe)
                {
                    if (Utility.numSilos() <= 0)
                    {
                        Game1.showRedMessage(Game1.content.LoadString("Strings\\Buildings:NeedSilo"));
                        return false;
                    }
                    who.currentLocation.playSound("Ship");
                    DelayedAction.playSoundAfterDelay("grassyStep", 100, (GameLocation)null);
                    if (inputItem.Stack == 0)
                        inputItem.Stack = 1;
                    var farm = Game1.getLocationFromName("Farm") as Farm;
                    if (farm != null)
                    {
                        int piecesOfHay1 = (int)farm.piecesOfHay.Value;
                        int addHay = farm.tryToAddHay(inputItem.Stack);
                        int piecesOfHay2 = (int)farm.piecesOfHay.Value;
                        if (piecesOfHay1 <= 0 && piecesOfHay2 > 0)
                            machine.showNextIndex.Value = true;
                        else if (piecesOfHay2 <= 0)
                            machine.showNextIndex.Value = false;
                        inputItem.Stack = addHay;
                        if (addHay <= 0)
                            return true;
                    }
                }
                else
                {
                    machine.heldObject.Value = new SObject();
                    return true;
                }
            }
            if(machine.Name.Contains("Table") && machine.heldObject.Value == null && !inputItem.bigCraftable.Value && !inputItem.Name.Contains("Table"))
            {
                machine.heldObject.Value = (SObject)inputItem.getOne();
                if (!probe)
                    who.currentLocation.playSound("woodyStep");
                return true;
            }
            //End
            SObject object2 = machine.heldObject.Value;
            return false;
        }
    }
}
