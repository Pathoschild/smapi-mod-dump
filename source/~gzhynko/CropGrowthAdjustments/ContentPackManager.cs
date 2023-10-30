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
        public List<Adjustments> ContentPacks = new List<Adjustments>();

        private const string ContentJsonName = "adjustments.json";

        public void InitializeContentPacks(IModHelper helper, IMonitor monitor)
        {
            monitor.Log("Loading content packs...", LogLevel.Info);
            
            foreach (var contentPack in helper.ContentPacks.GetOwned())
            {
                if (!contentPack.HasFile(ContentJsonName))
                {
                    monitor.Log(
                        $"  [{contentPack.Manifest.Name}] - Incorrect content pack folder structure. Expected {ContentJsonName} to be present in the folder.", LogLevel.Error);
                    continue;
                }

                var adjustments = new Adjustments();
                
                try
                {
                    adjustments.CropAdjustments = contentPack.ReadJsonFile<List<CropAdjustment>>(ContentJsonName);
                }
                catch (Exception e)
                {
                    monitor.Log($"  [{contentPack.Manifest.Name}] - Error while parsing adjustments.json: {e}", LogLevel.Error);
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
                    adjustment.CropProduceItemId = Utility.GetItemIdByName(adjustment.CropProduceName, helper);

                    if (adjustment.CropProduceItemId == -1)
                    {
                        if (jsonAssetsApi == null) continue;
                        
                        adjustment.CropProduceItemId = jsonAssetsApi.GetObjectId(adjustment.CropProduceName);
                    }
                }
            }
        }

        public void AssignCropOriginalRowsInSpritesheet(IModHelper helper)
        {
            foreach (var contentPack in ContentPacks)
            {
                foreach (var adjustment in contentPack.CropAdjustments)
                {
                    if(adjustment.SpecialSpritesForSeasons.Count == 0) continue;
                    
                    var cropData =  Utility.GetCropDataForProduceItemId(adjustment.CropProduceItemId, helper);
                    if(cropData == null)
                    {
                        ModEntry.ModMonitor.Log($"[{contentPack.ContentPack.Manifest.Name}] - Unable to get the original row in spritesheet for {adjustment.CropProduceName}. Special sprites won't work.", LogLevel.Error);
                        continue;
                    }
                    
                    adjustment.OriginalRowInSpriteSheet = int.Parse(cropData[2]);
                }
            }

            // ModEntry.ModMonitor.Log("assignCropOriginalRowsInSpritesheet", LogLevel.Info);
        }
    }
}