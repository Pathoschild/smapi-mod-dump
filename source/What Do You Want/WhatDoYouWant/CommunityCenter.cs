/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/emurphy42/WhatDoYouWant
**
*************************************************/

using StardewValley;

namespace WhatDoYouWant
{
    internal class CommunityCenter
    {
        private const string CommunityCenter_Money = "-1";

        private static readonly Dictionary<string, int> BundleAreas = new()
        {
            { "Pantry", 0 },
            { "Crafts Room", 1 },
            { "Fish Tank", 2 },
            { "Boiler Room", 3 },
            { "Vault", 4 },
            { "Bulletin Board", 5 }
        };

        public static void ShowCommunityCenterList(ModEntry modInstance)
        {
            var completeDescription = modInstance.Helper.Translation.Get("CommunityCenter_Complete", new { title = ModEntry.GetTitle_CommunityCenter() });

            if (Game1.player.mailReceived.Contains("ccIsComplete"))
            {
                Game1.drawDialogueNoTyping(completeDescription);
                return;
            }

            if (Game1.player.hasOrWillReceiveMail("JojaMember"))
            {
                var unavailableDescription = modInstance.Helper.Translation.Get("CommunityCenter_Unavailable", new { title = ModEntry.GetTitle_CommunityCenter() });
                Game1.drawDialogueNoTyping(unavailableDescription);
                return;
            }

            var qualitySilver = modInstance.Helper.Translation.Get("CommunityCenter_Silver");
            var qualityGold = modInstance.Helper.Translation.Get("CommunityCenter_Gold");
            var qualityIridium = modInstance.Helper.Translation.Get("CommunityCenter_Iridium");

            var moreOptionsThanSlots = modInstance.Helper.Translation.Get("CommunityCenter_MoreOptionsThanSlots");

            var bundleAreas = new Dictionary<int, string>();
            var bundleNames = new Dictionary<int, string>();
            var bundleOptions = new Dictionary<int, int>(); // total number of bundle options, whether or not already donated
            var bundleSlotsNeeded = new Dictionary<int, int>(); // e.g. Crab Pot Bundle has 10 options, but donating any 5 completes it
            var bundleItems = new Dictionary<int, List<string>>(); // list of options not already donated

            // adapted from base game logic for Community Center to refresh its bundle data
            var communityCenter = Game1.RequireLocation<StardewValley.Locations.CommunityCenter>("CommunityCenter");
            var bundleDictionary = communityCenter.bundlesDict();
            var bundleData = Game1.netWorldState.Value.BundleData;
            foreach (var keyValuePair in bundleData)
            {
                // e.g. key = "Pantry/4", value = "Animal/BO 16 1/186 1 0 182 1 0 174 1 0 438 1 0 440 1 0 442 1 0/4/5//Animal"
                //   bundle name / reward (type ID quantity) / options to donate (ID quantity minimum-quality) / color index / number of options needed / translated bundle name
                var keyArray = keyValuePair.Key.Split('/');

                var areaName = keyArray[0];
                // Missing Bundle is after Community Center
                if (areaName == "Abandoned Joja Mart")
                {
                    continue;
                }
                // TODO option to only include unlocked bundles, otherwise identify them as such
                //if (!communityCenter.shouldNoteAppearInArea(CommunityCenter.getAreaNumberFromName(areaName)))
                //{
                //    continue;
                //}

                var areaId = StardewValley.Locations.CommunityCenter.getAreaNumberFromName(areaName);
                var areaDisplayName = StardewValley.Locations.CommunityCenter.getAreaDisplayNameFromNumber(BundleAreas[areaName]);

                var bundleId = Convert.ToInt32(keyArray[1]);
                if (!bundleDictionary.ContainsKey(bundleId))
                {
                    continue;
                }

                var valueArray = keyValuePair.Value.Split('/');

                bundleAreas[bundleId] = areaDisplayName;
                bundleNames[bundleId] = (valueArray.Length > 6) ? valueArray[6] : valueArray[0];
                bundleSlotsNeeded[bundleId] = 0;
                if (valueArray.Length > 4 && Int32.TryParse(valueArray[4], out int slotsNeeded))
                {
                    bundleSlotsNeeded[bundleId] = slotsNeeded;
                }
                bundleItems[bundleId] = new List<string>();

                var itemsNeededList = ArgUtility.SplitBySpace(valueArray[2]);
                bundleOptions[bundleId] = itemsNeededList.Length / 3;
                for (var index = 0; index < itemsNeededList.Length; index += 3)
                {
                    if (bundleDictionary[bundleId][index / 3])
                    {
                        continue;
                    }

                    string itemId;
                    if (int.TryParse(itemsNeededList[index], out int result) && result < 0)
                    {
                        itemId = result.ToString();
                    }
                    else
                    {
                        var data = ItemRegistry.GetData(itemsNeededList[index]);
                        itemId = (data != null) ? data.QualifiedItemId : "(O)" + itemsNeededList[index];
                    }
                    var itemQuantityNeeded = Convert.ToInt32(itemsNeededList[index + 1]);
                    var minimumQuality = Convert.ToInt32(itemsNeededList[index + 2]);

                    string itemDescription;
                    switch (itemId)
                    {
                        case CommunityCenter_Money:
                            itemDescription = $"{itemQuantityNeeded}g";
                            break;
                        case Cooking.CookingIngredient_AnyMilk:
                            itemDescription = Game1.content.LoadString(ModEntry.StringKey_AnyMilk);
                            break;
                        case Cooking.CookingIngredient_AnyEgg:
                            itemDescription = Game1.content.LoadString(ModEntry.StringKey_AnyEgg);
                            break;
                        case Cooking.CookingIngredient_AnyFish:
                            itemDescription = Game1.content.LoadString(ModEntry.StringKey_AnyFish);
                            break;
                        default:
                            var dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(itemId);
                            itemDescription = modInstance.GetItemDescription(dataOrErrorItem);
                            break;
                    }
                    if (itemId != CommunityCenter_Money)
                    {
                        if (itemQuantityNeeded > 1)
                        {
                            itemDescription += $" x{itemQuantityNeeded}";
                        }
                        switch (minimumQuality)
                        {
                            case 1:
                                itemDescription += $" ({qualitySilver})";
                                break;
                            case 2:
                                itemDescription += $" ({qualityGold})";
                                break;
                            case 3:
                            case 4:
                                itemDescription += $" ({qualityIridium})";
                                break;
                        }
                    }
                    bundleItems[bundleId].Add(itemDescription);
                }
            }

            var linesToDisplay = new List<string>();

            foreach (var keyValuePair in bundleItems.OrderBy(keyValuePair => keyValuePair.Key))
            {
                var bundleId = keyValuePair.Key;
                if (bundleItems[bundleId].Count == 0)
                {
                    continue;
                }
                var areaName = bundleAreas[bundleId];
                var bundleName = Game1.content.LoadString("Strings\\UI:JunimoNote_BundleName", bundleNames[bundleId]);
                var bundleSlotPrefix = "";
                if (bundleSlotsNeeded[bundleId] != 0 && bundleSlotsNeeded[bundleId] != bundleOptions[bundleId])
                {
                    var numberOptionsAlreadyDonated = bundleOptions[bundleId] - bundleItems[bundleId].Count;
                    var bundleSlotsShort = bundleSlotsNeeded[bundleId] - numberOptionsAlreadyDonated;
                    bundleSlotPrefix = $"{bundleSlotsShort} {moreOptionsThanSlots} ";
                }
                var bundleItemList = String.Join(", ", bundleItems[bundleId]);
                linesToDisplay.Add($"* {areaName} - {bundleName} - {bundleSlotPrefix}{bundleItemList}{ModEntry.LineBreak}");
            }

            if (linesToDisplay.Count == 0)
            {
                Game1.drawDialogueNoTyping(completeDescription);
                return;
            }

            modInstance.ShowLines(linesToDisplay, title: ModEntry.GetTitle_CommunityCenter(), longerLinesExpected: true);
        }
    }
}
