using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace CustomHarvestFix
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        private IModHelper mHelper;
        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            mHelper = helper;
            mHelper.Events.GameLoop.SaveLoaded += OnSaveLoad;
            

        }

        /*********
        ** Private methods
        *********/

        /// <summary>
        /// Raised when the save is loaded.
        /// </summary>
        /// <param name="sender">object that sent the call.</param>
        /// <param name="e">the event data.</param>
        private void OnSaveLoad(object sender, SaveLoadedEventArgs e)
        {
            //load the current data from the information in memory.
            //the int[] stores 0 = standard produce, 1 = deluxe produce
            Dictionary<string, int[]> farmAnimalDataProduceTable = new Dictionary<string, int[]>();
            PopulateAnimalProduceDictionary(ref farmAnimalDataProduceTable);
            if (Game1.IsMasterGame)
            {
                //do nothing on non-main machines for now.
                Farm farm = Game1.getFarm();
                foreach(StardewValley.Buildings.Building thisBuilding in farm.buildings)
                {
                    this.Monitor.Log("Checking inside " + thisBuilding.nameOfIndoors);
                    if (thisBuilding.indoors.Value != null && (thisBuilding.buildingType.Contains("Barn") || thisBuilding.buildingType.Contains("Coop")))
                    {
                        //loop through the animals inside this building.
                        AnimalHouse thisInside = (AnimalHouse)thisBuilding.indoors.Value;
                        foreach (FarmAnimal thisAnimal in thisInside.animals.Values)
                        {
                            
                            if (thisAnimal.defaultProduceIndex.Value != farmAnimalDataProduceTable[thisAnimal.type.Value][0] || thisAnimal.deluxeProduceIndex.Value != farmAnimalDataProduceTable[thisAnimal.type.Value][1])
                            {    
                                //fix the standard item.
                                if (thisAnimal.defaultProduceIndex.Value != farmAnimalDataProduceTable[thisAnimal.type.Value][0]) {
                                    this.Monitor.Log(thisAnimal.Name + " the " + thisAnimal.type.Value + " producing: basic item " + thisAnimal.defaultProduceIndex.Value + " but should be producing basic item " + farmAnimalDataProduceTable[thisAnimal.type.Value][0].ToString() + ". Fixed.");
                                    if (thisAnimal.currentProduce == thisAnimal.defaultProduceIndex)
                                    {
                                        //update current product to the current defaultProduceIndex
                                        //Helper.Reflection.GetField<Netcode.NetInt>(thisAnimal, "currentProduce").SetValue(new Netcode.NetInt(farmAnimalDataProduceTable[thisAnimal.type.Value][0]));
                                        Helper.Reflection.GetField<Netcode.NetInt>(thisAnimal, "currentProduce").GetValue().Value = farmAnimalDataProduceTable[thisAnimal.type.Value][0];
                                    }
                                    //Helper.Reflection.GetField<Netcode.NetInt>(thisAnimal, "defaultProduceIndex").SetValue(new Netcode.NetInt(farmAnimalDataProduceTable[thisAnimal.type.Value][0]));
                                    Helper.Reflection.GetField<Netcode.NetInt>(thisAnimal, "defaultProduceIndex").GetValue().Value = farmAnimalDataProduceTable[thisAnimal.type.Value][0];
                                }

                                //fix the delux item.
                                if (thisAnimal.deluxeProduceIndex.Value != farmAnimalDataProduceTable[thisAnimal.type.Value][1])
                                {
                                    this.Monitor.Log(thisAnimal.Name + " the " + thisAnimal.type.Value + " producing deluxe item " + thisAnimal.deluxeProduceIndex.Value + " but should be producing deluxe item " + farmAnimalDataProduceTable[thisAnimal.type.Value][1].ToString() + ". Fixed.");
                                    if (thisAnimal.currentProduce == thisAnimal.deluxeProduceIndex)
                                    {
                                        //update current product to the current deluxeProduceIndex
                                        //Helper.Reflection.GetField<Netcode.NetInt>(thisAnimal, "currentProduce").SetValue(new Netcode.NetInt(farmAnimalDataProduceTable[thisAnimal.type.Value][1]));
                                        Helper.Reflection.GetField<Netcode.NetInt>(thisAnimal, "currentProduce").GetValue().Value = farmAnimalDataProduceTable[thisAnimal.type.Value][1];
                                    }
                                    //Helper.Reflection.GetField<Netcode.NetInt>(thisAnimal, "deluxeProduceIndex").SetValue(new Netcode.NetInt(farmAnimalDataProduceTable[thisAnimal.type.Value][1]));
                                    Helper.Reflection.GetField<Netcode.NetInt>(thisAnimal, "deluxeProduceIndex").GetValue().Value = farmAnimalDataProduceTable[thisAnimal.type.Value][1];
                                }
                            } else
                            {
                                this.Monitor.Log("Checking an animal: " + thisAnimal.Name + " the " + thisAnimal.type.Value+" produces the correct items");
                            }
                        }
                    }
                }
                this.Monitor.Log("Done fixing animals that needed fixing");
            }
        }

        /// <summary>
        /// Populates the farm animal produce table from the data in memory.
        /// </summary>
        /// <param name="table">The table to populate.</param>
        private void PopulateAnimalProduceDictionary(ref Dictionary<string, int[]> table)
        {
            //clear the table out first so we don't end up with duplicate data.
            table.Clear();

            Dictionary<string, string> animalData = Game1.content.Load<Dictionary<string, string>>("Data\\FarmAnimals");

            foreach(KeyValuePair<string,string> data in animalData)
            {
                //this.Monitor.Log("Data for " + data.Key + ": " + data.Value);
                string[] animalValues = data.Value.Split('/');
                int basicItem;
                int.TryParse(animalValues[2], out basicItem);
                int deluxeItem;
                int.TryParse(animalValues[3], out deluxeItem);

                table.Add(data.Key, new int[2] { basicItem, deluxeItem });
            }

            this.Monitor.Log("Animal Produce Table Created");

            //we might have the updated animal data now.  Not entirely sure honestly Lets find out!
        }
    }
}
