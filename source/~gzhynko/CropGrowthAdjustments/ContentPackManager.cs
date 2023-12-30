/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/gzhynko/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using CropGrowthAdjustments.Types;
using StardewModdingAPI;

namespace CropGrowthAdjustments
{
    public class ContentPackManager
    {
        public List<Adjustments> ContentPacks = new();

        private const string ContentJsonName = "adjustments.json";

        public void InitializeContentPacks(IModHelper helper, IMonitor monitor)
        {
            monitor.Log("Loading content packs...", LogLevel.Info);
            
            foreach (var contentPack in helper.ContentPacks.GetOwned())
            {
                if (!contentPack.HasFile(ContentJsonName))
                {
                    monitor.Log(
                        $"  {contentPack.Manifest.Name} - Incorrect content pack folder structure. Expected {ContentJsonName} to be present in the folder.", LogLevel.Error);
                    continue;
                }

                var adjustments = new Adjustments();
                
                try
                {
                    adjustments.CropAdjustments = contentPack.ReadJsonFile<List<CropAdjustment>>(ContentJsonName);
                }
                catch (Exception e)
                {
                    monitor.Log($"  {contentPack.Manifest.Name} - Error while parsing adjustments.json: {e}", LogLevel.Error);
                    continue;
                }
                
                adjustments.ContentPack = contentPack;
                
                ContentPacks.Add(adjustments);
                // provide info about the loaded content pack
                monitor.Log(
                    $"  Loaded {contentPack.Manifest.Name} {contentPack.Manifest.Version} by {contentPack.Manifest.Author}: {contentPack.Manifest.Description}", LogLevel.Info);
            }

            if (ContentPacks.Count == 0)
            {
                monitor.Log("  No content packs to load.", LogLevel.Info);
            }
        }

        public void AssignCropProduceItemIds(IModHelper helper, IJsonAssetsApi jsonAssetsApi)
        {
            foreach (var contentPack in ContentPacks)
            {
                foreach (var adjustment in contentPack.CropAdjustments)
                {
                    // skip assigning item id from game data if it is specified by the content pack
                    if (adjustment.CropProduceItemId != -1)
                    {
                        continue;
                    }
                    
                    // first try to get directly from game data
                    adjustment.CropProduceItemId = Utility.GetItemIdByName(adjustment.CropProduceName, helper);

                    // then try to get from JsonAssets API
                    if (adjustment.CropProduceItemId == -1 && jsonAssetsApi != null)
                    {
                        ModEntry.ModMonitor.Log($"assigning crop produce id for {adjustment.CropProduceName} from JsonAssets");
                        
                        adjustment.CropProduceItemId = jsonAssetsApi.GetObjectId(adjustment.CropProduceName);
                    }
                    
                    // finally, warn the player if the ID is still not assigned
                    // this means that either the crop produce name is specified incorrectly OR that the crop produce name was edited
                    if (adjustment.CropProduceItemId == -1)
                    {
                        ModEntry.ModMonitor.Log($"{contentPack.ContentPack.Manifest.Name} - Unable to assign ID to {adjustment.CropProduceName}. " + 
                                                $"Make sure the name you specified matches the desired crop name. Otherwise, if this crop had its produce item name" +
                                                $"edited (e.g. via ContentPatcher), make sure to specify the CropProduceItemId in adjustments.json.", LogLevel.Warn);
                        continue; 
                    }
                    
                    ModEntry.ModMonitor.Log($"assigned {adjustment.CropProduceItemId} to {adjustment.CropProduceName}");
                }
            }
        }

        public void AssignCropRowsInSpritesheet(IModHelper helper)
        {
            foreach (var contentPack in ContentPacks)
            {
                foreach (var adjustment in contentPack.CropAdjustments)
                {
                    if(adjustment.SpecialSpritesForSeasons == null || adjustment.SpecialSpritesForSeasons.Count == 0) continue;
                    
                    var cropData =  Utility.GetCropDataForProduceItemId(adjustment.CropProduceItemId, helper);
                    if(cropData == null)
                    {
                        ModEntry.ModMonitor.Log($"{contentPack.ContentPack.Manifest.Name} - Unable to get the crop data for {adjustment.CropProduceName}. Special sprites won't work.", LogLevel.Error);
                        continue;
                    }
                    
                    adjustment.RowInCropSpriteSheet = int.Parse(cropData[2]);
                }
            }
        }
    }
}