/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TheMightyAmondee/CustomDeathPenaltyPlus
**
*************************************************/

using StardewModdingAPI;
using System;

namespace CustomDeathPenaltyPlus
{
    /// <summary>
    /// Edits game assets
    /// </summary>
    internal class AssetEditor
    {
        private static ModConfig config;

        public static void SetConfig(ModConfig config)
        {
            AssetEditor.config = config;
        }
        /// <summary>
        /// Edits strings in the UI
        /// </summary>
        public class UIFixes : IAssetEditor
        {
            private IModHelper modHelper;

            public UIFixes(IModHelper helper)
            {
                modHelper = helper;
            }
            //Allow asset to be editted if name matches
            public bool CanEdit<T>(IAssetInfo asset)
            {
                return asset.AssetNameEquals("Strings\\UI");
            }

            //Edit asset
            public void Edit<T>(IAssetData asset)
            {
                var UIeditor = asset.AsDictionary<string, string>().Data;

                UIeditor["ItemList_ItemsLost"] = "Items recovered:";
            }
        }

        /// <summary>
        /// Edits strings in StringsFromCSFiles
        /// </summary>
        public class StringsFromCSFilesFixes : IAssetEditor
        {
            private IModHelper modHelper;

            public StringsFromCSFilesFixes(IModHelper helper)
            {
                modHelper = helper;
            }

            //Allow asset to be editted if name matches and any object references exist
            public bool CanEdit<T>(IAssetInfo asset)
            {
                return asset.AssetNameEquals("Strings\\StringsFromCSFiles") && PlayerStateSaver.state != null;
            }

            //Edit asset
            public void Edit<T>(IAssetData asset)
            {
                var editor = asset.AsDictionary<string, string>().Data;
                //Special case when no money is lost
                if (config.MoneyLossCap == 0 || config.MoneytoRestorePercentage == 1)
                {
                    editor["Event.cs.1068"] = "Dr. Harvey didn't charge me for the hospital visit, how nice. ";
                    editor["Event.cs.1058"] = "Fortunately, I still have all my money";
                }
                //Edit events to reflect amount lost
                else
                {
                    editor["Event.cs.1068"] = $"Dr. Harvey charged me {(int)Math.Round(PlayerStateSaver.state.moneylost)}g for the hospital visit. ";
                    editor["Event.cs.1058"] = $"I seem to have lost {(int)Math.Round(PlayerStateSaver.state.moneylost)}g";
                }

                if (config.RestoreItems == true)
                {
                    //Remove unnecessary strings
                    editor["Event.cs.1060"] = "";
                    editor["Event.cs.1061"] = "";
                    editor["Event.cs.1062"] = "";
                    editor["Event.cs.1063"] = "";
                    editor["Event.cs.1071"] = "";
                }
            }
        }
    }
}
