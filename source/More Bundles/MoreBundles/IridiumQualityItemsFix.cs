/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TwinBuilderOne/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;

namespace MoreBundles
{

    /*
     * This class fixes a bug in the vanilla game where
     * bundles requiring items to be iridium quality are
     * misread by the bundle generator.
     * 
     * Essentially, if the letters "IQ" are detected in front of
     * the item name, the resultant bundle would expect items with
     * a quality of 3 (invalid) instead of quality 4, which is iridium
     * quality.
     * 
     * This class changes the quality of all items with quality 3 to
     * quality 4.
     */
    public static class IridiumQualityItemsFix
    {
        public static Dictionary<string, string> FixIridiumQualityItems(Dictionary<string, string> data)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            foreach (var item in data)
            {
                string[] bundleData = item.Value.Split('/');
                string[] itemsList = bundleData[2].Split(' ');

                for (int i = 2; i < itemsList.Length; i += 3)
                {
                    if (itemsList[i] == "3")
                    {
                        itemsList[i] = "4";
                    }
                }

                bundleData[2] = string.Join(" ", itemsList);
                result[item.Key] = string.Join("/", bundleData);
            }

            return result;
        }
    }
}
