/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/BussinBungus/BungusSDVMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData.FarmAnimals;

namespace DefaultSkinReplace
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {
        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;

            helper.ConsoleCommands.Add("dasr_reroll", "Rerolls skins for default animals. To make animals default, run 'dasr_reset all' first!\n\nUsage: dasr_reroll <name>\n- name: name of animal to reroll, or 'all' to reroll all.", this.Reroll);
            helper.ConsoleCommands.Add("dasr_pick", "Picks a specific farm animal skin.\n\nUsage: dasr_pick <name> <skin>\n- name: name of animal to pick skin for.\n- skin: the skin to apply.", this.Pick);
            helper.ConsoleCommands.Add("dasr_reset", "Resets to default skin.\n\nUsage: dasr_reset <name>\n- name: name of animal to reset, or 'all' to reset all.", this.Reset);
        }

        /// <summary>Get all farm animals from the world and friendship data.</summary>
        public IEnumerable<FarmAnimal> GetFarmAnimals()
        {
            List<FarmAnimal> animals = new List<FarmAnimal>();
            Utility.ForEachLocation(delegate (GameLocation location)
            {
                animals.AddRange(location.animals.Values.ToList());
                return true;
            });
            return animals;
        }

        /*********
        ** Private methods
        *********/
        /// <summary>Raised on day start.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            foreach (FarmAnimal animal in this.GetFarmAnimals())
            {
                RollSkin(animal);
            }
        }
        private void RollSkin(FarmAnimal animal)
        {
            FarmAnimalData data = animal.GetAnimalData();

            if (data != null)
            {
                if (animal.skinID.Value == null && data.Skins != null)
                {
                    Monitor.Log($"Detected a skinless {animal.type}.");
                    Random random = Utility.CreateRandom(animal.myID.Value);
                    Monitor.Log($"Generated random from animal ID {animal.myID.Value}.");
                    float totalWeight = 1f;
                    foreach (FarmAnimalSkin skin2 in data.Skins)
                    {
                        Monitor.Log($"Adding {skin2.Id} weight of {skin2.Weight}");
                        totalWeight += skin2.Weight;
                        Monitor.Log($"Total weight now {totalWeight}");
                    }
                    totalWeight = Utility.RandomFloat(0f, totalWeight, random);
                    Monitor.Log($"Randomized, total weight now {totalWeight}");
                    foreach (FarmAnimalSkin skin in data.Skins)
                    {
                        totalWeight -= skin.Weight;
                        Monitor.Log($"{skin.Id} weight subtracted, total weight now {totalWeight}");
                        if (totalWeight <= 0f)
                        {
                            animal.skinID.Value = skin.Id;
                            Monitor.Log($"Total weight <0, skin set to {skin.Id}");
                            break;
                        }
                    }
                }
            }
        }

        private void Reroll(string command, string[] args)
        {
            foreach (FarmAnimal animal in this.GetFarmAnimals())
            {
                if (args[0] == "all" || args[0] == animal.Name)
                {
                    Monitor.Log($"Rerolling skin for {animal.type} {animal.Name}.", LogLevel.Info);
                    RollSkin(animal);
                }
            }
        }
        private void Pick(string command, string[] args)
        {
            foreach (FarmAnimal animal in this.GetFarmAnimals())
            {
                string skinID = null;
                
                if (args[0] == animal.Name)
                {
                    FarmAnimalData data = animal.GetAnimalData();
                    if (data != null && data.Skins != null)
                    {
                        foreach (FarmAnimalSkin skin in data.Skins)
                        {
                            if (skin.Id.Contains(args[1]))
                            {
                                skinID = skin.Id;
                                break;
                            }
                        }

                    if (skinID != null)
                    {
                        animal.skinID.Value = skinID;
                        Monitor.Log($"{animal.type} {animal.Name} given skin {skinID}.", LogLevel.Info);
                        break;
                    }
                    Monitor.Log($"Skin name may be incorrect.", LogLevel.Info);
                    break;
                    }
                }  
            }
        }
        private void Reset(string command, string[] args)
        {
            foreach (FarmAnimal animal in this.GetFarmAnimals())
            {
                if (args[0] == "all" || args[0] == animal.Name)
                {
                    FarmAnimalData data = animal.GetAnimalData();

                    if (data != null)
                    {
                        if (animal.skinID.Value != null)
                        {
                            Monitor.Log($"Resetting {animal.type} {animal.Name} to default skin.", LogLevel.Info);
                            animal.skinID.Value = null;
                        }
                    }
                }
            }
        }
    }
}
