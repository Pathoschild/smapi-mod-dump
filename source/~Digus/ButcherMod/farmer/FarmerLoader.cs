/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnimalHusbandryMod.animals;
using AnimalHusbandryMod.common;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace AnimalHusbandryMod.farmer
{
    public class FarmerLoader
    {
        public static FarmerData FarmerData = null;

        public static void LoadData()
        {
            if (Context.IsMainPlayer)
            {
                FarmerData = DataLoader.Helper.Data.ReadJsonFile<FarmerData>($"data/farmers/{Constants.SaveFolderName}.json") ?? new FarmerData();
            }
        }

        public static void SaveData()
        {
            if (Context.IsMainPlayer)
            {
                DataLoader.Helper.Data.WriteJsonFile<FarmerData>($"data/farmers/{Constants.SaveFolderName}.json", FarmerData);
            }
        }

        public static void MoveOldPregnancyData()
        {
            if (Context.IsMainPlayer)
            {
                if (FarmerData?.PregnancyData == null || FarmerData.PregnancyData.Count == 0) return;
                AnimalHusbandryModEntry.monitor.Log($"Migrating pregnancy data from old format to the new one.", LogLevel.Info);
                FarmerData?.PregnancyData?.RemoveAll(p =>
                {
                    try
                    {
                        FarmAnimal farmAnimal = Utility.getAnimal(p.Id);
                        if (farmAnimal != null)
                        {
                            farmAnimal.allowReproduction.Value = p.AllowReproductionBeforeInsemination;
                            PregnancyController.AddPregnancy(farmAnimal, p.DaysUntilBirth);
                            farmAnimal.allowReproduction.Value = false;
                        }
                        else
                        {
                            AnimalHusbandryModEntry.monitor.Log($"The animal id '{p.Id}' was not found in the game and its pregnancy data is being discarded.", LogLevel.Warn);
                        }
                        return true;
                    }
                    catch (Exception e)
                    {
                        AnimalHusbandryModEntry.monitor.Log($"Unexpected error while trying to migrate pregnancy data of animal id '{p.Id}'. The pregnancy data will be kept on the old format but will not be used.", LogLevel.Warn);
                        AnimalHusbandryModEntry.monitor.Log($"Message from pregnancy error above: {e.Message}");
                        return false;
                    }
                });
            }
        }

        public static void MoveOldAnimalStatusData()
        {
            if (Context.IsMainPlayer)
            {
                if (FarmerData?.AnimalData == null || FarmerData.AnimalData.Count == 0) return;
                AnimalHusbandryModEntry.monitor.Log($"Migrating animal status data from old format to the new one.", LogLevel.Info);
                FarmerData?.AnimalData?.RemoveAll(s =>
                {
                    try
                    {
                        FarmAnimal farmAnimal = Utility.getAnimal(s.Id);
                        if (farmAnimal != null)
                        {
                            if (s.DayParticipatedContest != null) farmAnimal.SetDayParticipatedContest(s.DayParticipatedContest);
                            if (s.HasWon != null) farmAnimal.SetHasWon(s.HasWon.Value);
                            if (s.LastDayFeedTreat != null) farmAnimal.SetLastDayFeedTreat(s.LastDayFeedTreat);
                            s.FeedTreatsQuantity.ToList().ForEach(t => farmAnimal.SetFeedTreatsQuantity(t.Key, t.Value));
                        }
                        else
                        {
                            AnimalHusbandryModEntry.monitor.Log($"The animal id '{s.Id}' was not found in the game and its animal status data is being discarded.", LogLevel.Warn);
                        }
                        return true;
                    }
                    catch (Exception e)
                    {
                        AnimalHusbandryModEntry.monitor.Log($"Unexpected error while trying to migrate animal status data of animal id '{s.Id}'. The animal status data will be kept on the old format but will not be used.", LogLevel.Warn);
                        AnimalHusbandryModEntry.monitor.Log($"Message from pregnancy error above: {e.Message}");
                        return false;
                    }
                });
            }
        }
    }
}
