using Bookcase.Events;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FishingLogbook.Tracker;
using FishingLogbook.UI;
using StardewValley.Menus;
using System.IO;
using StardewValley.Monsters;

namespace FishingLogbook
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            SaveEvents.AfterLoad += LoadFishingLog;
            SaveEvents.AfterSave += SaveFishingLog;
            BookcaseEvents.OnItemTooltip.Add((e) => TooltipPatch.OnTooltipDisplay(e, FishingLog), Priority.Low);
            BookcaseEvents.FishCaughtInfo.Add((e) =>
            {
                if (e.FishSize == -1)
                    return;
                Monitor.Log($"Fish caught {e.FishID} :: {e.FishSize}");
                FishingLog.RecordCatch(e.FishID, e.FishSize, e.FishQuality);
                SaveFishingLog(null, null);
            });
            BookcaseEvents.PostBundleSpecificPageSetup.Add((e) =>
            {
                foreach (var ingredient in e.ingredientList)
                {
                    if (ingredient != null && ingredient.item.Category == StardewValley.Object.FishCategory)
                        ingredient.hoverText += $"\r\n{FishingLog.GetCatchConditionsAsString(ingredient.item)}";
                }
            });
            BookcaseEvents.CollectionsPageDrawEvent.Add((e) =>
            {
                if (e.currentTab == CollectionsPage.fishTab && e.hoverText != "")
                {
                    Item i = null;
                    foreach (ClickableTextureComponent c in e.collections[e.currentTab][e.currentPage])
                    {
                        if (int.TryParse(c.name.Split(' ')[0], out int index))
                        {
                            string name = e.hoverText.Split('\r')[0];
                            if ((Game1.objectInformation[index].Split('/')[0].Equals(name, StringComparison.CurrentCultureIgnoreCase)))
                            {
                                i = new StardewValley.Object(index, 1, false, 0, 0);
                                break;
                            }
                        }
                    }
                    if (i != null)
                    {
                        string result = FishingLog.GetCatchConditionsAsString(i);
                        if (!e.hoverText.Contains(result))
                            e.hoverText += "\r\n\r\n" + result;
                    }
                }
            });
        }

        public FishingLog FishingLog
        {
            get;
            private set;
        }

        private void SaveFishingLog(object sender, EventArgs e)
        {
            Monitor.Log("Saving to " + FishingLogSavePath);
            Helper.WriteJsonFile(FishingLogSavePath, FishingLog);
            Monitor.Log("Complete");
        }

        private void LoadFishingLog(object sender, EventArgs e)
        {
            Monitor.Log("Attempting to load " + FishingLogSavePath);
            FishingLog = Helper.ReadJsonFile<FishingLog>(FishingLogSavePath);
            if (FishingLog == null)
            {
                Monitor.Log("FishingLog not found.");
                FishingLog = new FishingLog();
                return;
            }
            Monitor.Log("Load successful.");
        }

        private string FishingLogSavePath
        {
            get
            {
                return SaveInfoPath("Logbook");
            }
        }
        private string SaveInfoPath(string name)
        {
            return $"{Constants.CurrentSavePath}{Path.DirectorySeparatorChar}{Constants.SaveFolderName}_{ModManifest.UniqueID}_{name}.json";
        }

    }
}
