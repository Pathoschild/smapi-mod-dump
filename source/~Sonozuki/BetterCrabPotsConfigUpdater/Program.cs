/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sonozuki/StardewMods
**
*************************************************/

using BetterCrabPotsConfigUpdater.ModConfigs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace BetterCrabPotsConfigUpdater
{
    /// <summary>The program entry point.</summary>
    public class Program
    {
        /*********
        ** Public Methods 
        *********/
        /// <summary>The program entry point.</summary>
        /// <param name="args">The passed arguments. These will be the 'OldConfig' and 'NewConfig' folder paths.</param>
        public static void Main(string[] args)
        {
            try
            {
                if (!ValidateArgs(args))
                {
                    return;
                }
                
                // get the old config file
                string oldConfigPath = Path.GetFullPath(args[0]);
                
                // deserialize old config directly from the file
                OldModConfig oldModConfig;
                using (StreamReader file = File.OpenText(oldConfigPath))
                {
                    oldModConfig = (OldModConfig)new JsonSerializer().Deserialize(file, typeof(OldModConfig));
                }

                NewModConfig newModConfig = ConvertToNewConfigLayout(oldModConfig);
                using (StreamWriter sWriter = new StreamWriter(args[1]))
                using (JsonWriter jWriter = new JsonTextWriter(sWriter))
                {
                    jWriter.Formatting = Formatting.Indented;

                    new JsonSerializer().Serialize(jWriter, newModConfig);
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\nconfig.json successfully converted");
                Console.ForegroundColor = ConsoleColor.White;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nconfig.json failed to convert: {ex.Message}\n{ex.StackTrace}");
                Console.ForegroundColor = ConsoleColor.White;
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }


        /*********
        ** Private Methods 
        *********/
        /// <summary>This method ensures there are a valid amount of arguments.</summary>
        /// <param name="args">The arguments passed to the app.</param>
        /// <returns>Returns a bool depending if the arguments are valid.</returns>
        private static bool ValidateArgs(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Invalid args supplied");
                Console.ReadLine();
                return false;
            }

            return true;
        }

        /// <summary>This method will convert the old config format to the new config format.</summary>
        /// <param name="oldModConfig">The old config object to get converted.</param>
        /// <returns>Returns the config in the new format.</returns>
        private static NewModConfig ConvertToNewConfigLayout(OldModConfig oldModConfig)
        {
            NewModConfig newModConfig = new NewModConfig();

            newModConfig.EnableTrash = oldModConfig.EnableTrash;
            newModConfig.RequiresBait = oldModConfig.RequiresBait;
            newModConfig.PercentChanceForTrash = oldModConfig.PercentChanceForTrash;
            newModConfig.EnableBetterQuality = oldModConfig.EnableBetterQuality;
            newModConfig.EnablePassiveTrash = oldModConfig.EnablePassiveTrash;
            newModConfig.PercentChanceForPassiveTrash = oldModConfig.PercentChanceForPassiveTrash;
            newModConfig.WhatCanBeFoundAsPassiveTrash = oldModConfig.WhatCanBeFoundAsPassiveTrash;

            // farmLand
            foreach (KeyValuePair<int, int> farmLandItem in oldModConfig.WhatCanBeFoundInFarmLand)
            {
                Item test3 = new Item(farmLandItem.Key, farmLandItem.Value, 1);
                newModConfig.FarmLand.WhatCanBeFound.Add(test3);
            }
            foreach (KeyValuePair<int, int> farmLandTrashItem in oldModConfig.WhatCanBeFoundInDesert_AsTrash)
            {
                newModConfig.FarmLand.WhatTrashCanBeFound.Add(new Item(farmLandTrashItem.Key, farmLandTrashItem.Value, 1));
            }

            // cindersapForest
            foreach (KeyValuePair<int, int> cindersapForestItem in oldModConfig.WhatCanBeFoundInCindersapForest)
            {
                newModConfig.CindersapForest.WhatCanBeFound.Add(new Item(cindersapForestItem.Key, cindersapForestItem.Value, 1));
            }
            foreach (KeyValuePair<int, int> cindersapForestTrashItem in oldModConfig.WhatCanBeFoundInCindersapForest_AsTrash)
            {
                newModConfig.CindersapForest.WhatTrashCanBeFound.Add(new Item(cindersapForestTrashItem.Key, cindersapForestTrashItem.Value, 1));
            }

            // mountainsLake
            foreach (KeyValuePair<int, int> mountainsLakeItem in oldModConfig.WhatCanBeFoundInMountainsLake)
            {
                newModConfig.MountainsLake.WhatCanBeFound.Add(new Item(mountainsLakeItem.Key, mountainsLakeItem.Value, 1));
            }
            foreach (KeyValuePair<int, int> mountainsLakeTrashItem in oldModConfig.WhatCanBeFoundInMountainsLake_AsTrash)
            {
                newModConfig.MountainsLake.WhatTrashCanBeFound.Add(new Item(mountainsLakeTrashItem.Key, mountainsLakeTrashItem.Value, 1));
            }
                
            // town
            foreach (KeyValuePair<int, int> townItem in oldModConfig.WhatCanBeFoundInTown)
            {
                newModConfig.Town.WhatCanBeFound.Add(new Item(townItem.Key, townItem.Value, 1));
            }
            foreach (KeyValuePair<int, int> townTrashItem in oldModConfig.WhatCanBeFoundInTown_AsTrash)
            {
                newModConfig.Town.WhatTrashCanBeFound.Add(new Item(townTrashItem.Key, townTrashItem.Value, 1));
            }

            // minesLayer20
            foreach (KeyValuePair<int, int> minesLayer20Item in oldModConfig.WhatCanBeFoundInMines_Layer20)
            {
                newModConfig.Mines_Layer20.WhatCanBeFound.Add(new Item(minesLayer20Item.Key, minesLayer20Item.Value, 1));
            }
            foreach (KeyValuePair<int, int> minesLayer20TrashItem in oldModConfig.WhatCanBeFoundInMines_Layer20_AsTrash)
            {
                newModConfig.Mines_Layer20.WhatTrashCanBeFound.Add(new Item(minesLayer20TrashItem.Key, minesLayer20TrashItem.Value, 1));
            }

            // minesLayer60
            foreach (KeyValuePair<int, int> minesLayer60Item in oldModConfig.WhatCanBeFoundInMines_Layer60)
            {
                newModConfig.Mines_Layer60.WhatCanBeFound.Add(new Item(minesLayer60Item.Key, minesLayer60Item.Value, 1));
            }
            foreach (KeyValuePair<int, int> minesLayer60TrashItem in oldModConfig.WhatCanBeFoundInMines_Layer60_AsTrash)
            {
                newModConfig.Mines_Layer60.WhatTrashCanBeFound.Add(new Item(minesLayer60TrashItem.Key, minesLayer60TrashItem.Value, 1));
            }

            // minesLayer100
            foreach (KeyValuePair<int, int> minesLayer100Item in oldModConfig.WhatCanBeFoundInMines_Layer100)
            {
                newModConfig.Mines_Layer100.WhatCanBeFound.Add(new Item(minesLayer100Item.Key, minesLayer100Item.Value, 1));
            }
            foreach (KeyValuePair<int, int> minesLayer100TrashItem in oldModConfig.WhatCanBeFoundInMines_Layer100_AsTrash)
            {
                newModConfig.Mines_Layer100.WhatTrashCanBeFound.Add(new Item(minesLayer100TrashItem.Key, minesLayer100TrashItem.Value, 1));
            }

            // mutantBugLair
            foreach (KeyValuePair<int, int> mutantBugLairItem in oldModConfig.WhatCanBeFoundInMutantBugLair)
            {
                newModConfig.MutantBugLair.WhatCanBeFound.Add(new Item(mutantBugLairItem.Key, mutantBugLairItem.Value, 1));
            }
            foreach (KeyValuePair<int, int> mutantBugLairTrashItem in oldModConfig.WhatCanBeFoundInMutantBugLair_AsTrash)
            {
                newModConfig.MutantBugLair.WhatTrashCanBeFound.Add(new Item(mutantBugLairTrashItem.Key, mutantBugLairTrashItem.Value, 1));
            }

            // witchsSwamp
            foreach (KeyValuePair<int, int> witchsSwampItem in oldModConfig.WhatCanBeFoundInWitchsSwamp)
            {
                newModConfig.WitchsSwamp.WhatCanBeFound.Add(new Item(witchsSwampItem.Key, witchsSwampItem.Value, 1));
            }
            foreach (KeyValuePair<int, int> witchsSwampTrashItem in oldModConfig.WhatCanBeFoundInWitchsSwamp_AsTrash)
            {
                newModConfig.WitchsSwamp.WhatTrashCanBeFound.Add(new Item(witchsSwampTrashItem.Key, witchsSwampTrashItem.Value, 1));
            }

            // secretWoods
            foreach (KeyValuePair<int, int> secretWoodsItem in oldModConfig.WhatCanBeFoundInSecretWoods)
            {
                newModConfig.SecretWoods.WhatCanBeFound.Add(new Item(secretWoodsItem.Key, secretWoodsItem.Value, 1));
            }
            foreach (KeyValuePair<int, int> secretWoodsTrashItem in oldModConfig.WhatCanBeFoundInSecretWoods_AsTrash)
            {
                newModConfig.SecretWoods.WhatTrashCanBeFound.Add(new Item(secretWoodsTrashItem.Key, secretWoodsTrashItem.Value, 1));
            }

            // desert
            foreach (KeyValuePair<int, int> desertItem in oldModConfig.WhatCanBeFoundInDesert)
            {
                newModConfig.Desert.WhatCanBeFound.Add(new Item(desertItem.Key, desertItem.Value, 1));
            }
            foreach (KeyValuePair<int, int> desertTrashItem in oldModConfig.WhatCanBeFoundInDesert_AsTrash)
            {
                newModConfig.Desert.WhatTrashCanBeFound.Add(new Item(desertTrashItem.Key, desertTrashItem.Value, 1));
            }

            // sewers
            foreach (KeyValuePair<int, int> sewerItem in oldModConfig.WhatCanBeFoundInSewers)
            {
                newModConfig.Sewers.WhatCanBeFound.Add(new Item(sewerItem.Key, sewerItem.Value, 1));
            }
            foreach (KeyValuePair<int, int> sewerTrashItem in oldModConfig.WhatCanBeFoundInSewers_AsTrash)
            {
                newModConfig.Sewers.WhatTrashCanBeFound.Add(new Item(sewerTrashItem.Key, sewerTrashItem.Value, 1));
            }

            // beach
            foreach (KeyValuePair<int, int> beachItem in oldModConfig.WhatCanBeFoundInOcean)
            {
                newModConfig.Beach.WhatCanBeFound.Add(new Item(beachItem.Key, beachItem.Value, 1));
            }
            foreach (KeyValuePair<int, int> beachTrashItem in oldModConfig.WhatCanBeFoundInOcean_AsTrash)
            {
                newModConfig.Beach.WhatTrashCanBeFound.Add(new Item(beachTrashItem.Key, beachTrashItem.Value, 1));
            }

            return newModConfig;
        }
    }
}
