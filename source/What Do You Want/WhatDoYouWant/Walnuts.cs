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
    internal class Walnuts
    {
        // Methods of checking for walnuts not yet found, adapted from base game functions
        private const string WalnutType_MissingTheseNuts = "MissingTheseNuts";
        private const string WalnutType_MissingLimitedNutDrops = "MissingLimitedNutDrops";
        private const string WalnutType_GoldenCoconutCracked = "GoldenCoconutCracked";
        private const string WalnutType_GotBirdieReward = "GotBirdieReward";

        // List of walnuts
        private static readonly List<List<string>> WalnutList = new()
        {
            // base game function name, token passed to it, hint text (Strings\\Locations:NutHint_*) or "none", [number of walnuts - 1 if not specified]
            new List<string>() { WalnutType_MissingTheseNuts, "Bush_IslandNorth_13_33", "VolcanoLava" },
            new List<string>() { WalnutType_MissingTheseNuts, "Bush_IslandNorth_5_30", "VolcanoLava" },
            new List<string>() { WalnutType_MissingTheseNuts, "Buried_IslandNorth_19_39", "BuriedArch" },
            new List<string>() { WalnutType_MissingTheseNuts, "Bush_IslandNorth_4_42", "NorthHidden" },
            new List<string>() { WalnutType_MissingTheseNuts, "Bush_IslandNorth_45_38", "NorthHidden" },
            new List<string>() { WalnutType_MissingTheseNuts, "Bush_IslandNorth_47_40", "NorthHidden" },
            new List<string>() { WalnutType_MissingTheseNuts, "IslandLeftPlantRestored", "Arch" },
            new List<string>() { WalnutType_MissingTheseNuts, "IslandRightPlantRestored", "Arch" },
            new List<string>() { WalnutType_MissingTheseNuts, "IslandBatRestored", "Arch" },
            new List<string>() { WalnutType_MissingTheseNuts, "IslandFrogRestored", "Arch" },
            new List<string>() { WalnutType_MissingTheseNuts, "IslandCenterSkeletonRestored", "Arch", "6" },
            new List<string>() { WalnutType_MissingTheseNuts, "IslandSnakeRestored", "Arch", "3" },
            new List<string>() { WalnutType_MissingTheseNuts, "Buried_IslandNorth_19_13", "NorthBuried" },
            new List<string>() { WalnutType_MissingTheseNuts, "Buried_IslandNorth_57_79", "NorthBuried" },
            new List<string>() { WalnutType_MissingTheseNuts, "Buried_IslandNorth_54_21", "NorthBuried" },
            new List<string>() { WalnutType_MissingTheseNuts, "Buried_IslandNorth_42_77", "NorthBuried" },
            new List<string>() { WalnutType_MissingTheseNuts, "Buried_IslandNorth_62_54", "NorthBuried" },
            new List<string>() { WalnutType_MissingTheseNuts, "Buried_IslandNorth_26_81", "NorthBuried" },
            new List<string>() { WalnutType_MissingTheseNuts, "Bush_IslandNorth_20_26", "NorthHidden" },
            new List<string>() { WalnutType_MissingTheseNuts, "Bush_IslandNorth_9_84", "NorthHidden" },
            new List<string>() { WalnutType_MissingTheseNuts, "Bush_IslandNorth_56_27", "NorthHidden" },
            new List<string>() { WalnutType_MissingTheseNuts, "Bush_IslandSouth_31_5", "NorthHidden" },
            new List<string>() { WalnutType_MissingTheseNuts, "TreeNut", "HutTree" },
            new List<string>() { WalnutType_MissingTheseNuts, "IslandWestCavePuzzle", "WestHidden", "3" },
            new List<string>() { WalnutType_MissingTheseNuts, "SandDuggy", "WestHidden" },
            new List<string>() { WalnutType_MissingLimitedNutDrops, "TigerSlimeNut", "TigerSlime" },
            new List<string>() { WalnutType_MissingTheseNuts, "Buried_IslandWest_21_81", "WestBuried" },
            new List<string>() { WalnutType_MissingTheseNuts, "Buried_IslandWest_62_76", "WestBuried" },
            new List<string>() { WalnutType_MissingTheseNuts, "Buried_IslandWest_39_24", "WestBuried" },
            new List<string>() { WalnutType_MissingTheseNuts, "Buried_IslandWest_88_14", "WestBuried" },
            new List<string>() { WalnutType_MissingTheseNuts, "Buried_IslandWest_43_74", "WestBuried" },
            new List<string>() { WalnutType_MissingTheseNuts, "Buried_IslandWest_30_75", "WestBuried" },
            new List<string>() { WalnutType_MissingLimitedNutDrops, "MusselStone", "MusselStone", "5" },
            new List<string>() { WalnutType_MissingLimitedNutDrops, "IslandFarming", "IslandFarming", "5" },
            new List<string>() { WalnutType_MissingTheseNuts, "Bush_IslandWest_104_3", "WestHidden" },
            new List<string>() { WalnutType_MissingTheseNuts, "Bush_IslandWest_31_24", "WestHidden" },
            new List<string>() { WalnutType_MissingTheseNuts, "Bush_IslandWest_38_56", "WestHidden" },
            new List<string>() { WalnutType_MissingTheseNuts, "Bush_IslandWest_75_29", "WestHidden" },
            new List<string>() { WalnutType_MissingTheseNuts, "Bush_IslandWest_64_30", "WestHidden" },
            new List<string>() { WalnutType_MissingTheseNuts, "Bush_IslandWest_54_18", "WestHidden" },
            new List<string>() { WalnutType_MissingTheseNuts, "Bush_IslandWest_25_30", "WestHidden" },
            new List<string>() { WalnutType_MissingTheseNuts, "Bush_IslandWest_15_3", "WestHidden" },
            new List<string>() { WalnutType_MissingLimitedNutDrops, "IslandFishing", "IslandFishing", "5" },
            new List<string>() { WalnutType_MissingLimitedNutDrops, "VolcanoNormalChest", "VolcanoTreasure" },
            new List<string>() { WalnutType_MissingLimitedNutDrops, "VolcanoRareChest", "VolcanoTreasure" },
            new List<string>() { WalnutType_MissingLimitedNutDrops, "VolcanoBarrel", "VolcanoBarrel", "5" },
            new List<string>() { WalnutType_MissingLimitedNutDrops, "VolcanoMining", "VolcanoMining", "5" },
            new List<string>() { WalnutType_MissingLimitedNutDrops, "VolcanoMonsterDrop", "VolcanoMonsters", "5" },
            new List<string>() { WalnutType_MissingLimitedNutDrops, "Island_N_BuriedTreasureNut", "Journal" },
            new List<string>() { WalnutType_MissingLimitedNutDrops, "Island_W_BuriedTreasureNut", "Journal" },
            new List<string>() { WalnutType_MissingLimitedNutDrops, "Island_W_BuriedTreasureNut2", "Journal" },
            new List<string>() { WalnutType_MissingTheseNuts, "Mermaid", "Journal", "5" },
            new List<string>() { WalnutType_MissingTheseNuts, "TreeNutShot", "Journal" },
            new List<string>() { WalnutType_MissingTheseNuts, "Buried_IslandSouthEastCave_36_26", "SouthEastBuried" },
            new List<string>() { WalnutType_MissingTheseNuts, "Buried_IslandSouthEast_25_17", "SouthEastBuried" },
            new List<string>() { WalnutType_MissingTheseNuts, "StardropPool", "StardropPool" },
            new List<string>() { WalnutType_MissingTheseNuts, "Bush_Caldera_28_36", "Caldera" },
            new List<string>() { WalnutType_MissingTheseNuts, "Bush_Caldera_9_34", "Caldera" },
            new List<string>() { WalnutType_MissingTheseNuts, "Bush_CaptainRoom_2_4", "WestHidden" }, // shipwreck
            new List<string>() { WalnutType_MissingTheseNuts, "BananaShrine", "none", "3" },
            new List<string>() { WalnutType_MissingTheseNuts, "Bush_IslandEast_17_37", "none" }, // out in the open near Leo's hut
            new List<string>() { WalnutType_MissingLimitedNutDrops, "Darts", "none", "3" },
            new List<string>() { WalnutType_MissingTheseNuts, "IslandGourmand1", "Gourmand", "5" },
            new List<string>() { WalnutType_MissingTheseNuts, "IslandGourmand2", "Gourmand", "5" },
            new List<string>() { WalnutType_MissingTheseNuts, "IslandGourmand3", "Gourmand", "5" },
            new List<string>() { WalnutType_MissingTheseNuts, "IslandShrinePuzzle", "IslandShrine", "5" },
            new List<string>() { WalnutType_MissingTheseNuts, "Bush_IslandShrine_23_34", "none" },
            new List<string>() { WalnutType_GoldenCoconutCracked, "", "GoldenCoconut" },
            new List<string>() { WalnutType_GotBirdieReward, "", "none", "5" } // Pirate's Wife
        };

        private static string GetHintText(string hintKey)
        {
            var hintText = Game1.content.LoadString($"Strings\\Locations:NutHint_{hintKey}");
            if (hintText.StartsWith("{0} "))
            {
                hintText = hintText.Substring(4, 1).ToUpper() + hintText.Substring(5);
            }
            return hintText;
        }

        public static void ShowWalnutsList(ModEntry modInstance)
        {
            var linesToDisplay = new List<string>();

            // adapted from base game logic for hints in Leo's hut
            //   TODO sort options: area, hint
            //   TODO option to provide more detail (see Stardew Checker source)
            //   TODO option to omit areas not yet unlocked, otherwise identify them as such
            var hintDictionary = new Dictionary<string, int>();
            foreach (var walnut in WalnutList)
            {
                var walnutFunction = walnut[0];
                var walnutToken = walnut[1];
                var walnutHint = walnut[2];
                var walnutNumberTotal = (walnut.Count >= 4) ? Convert.ToInt32(walnut[3]) : 1;

                var walnutNumberMissing = 0;
                switch (walnutFunction)
                {
                    case WalnutType_MissingTheseNuts:
                        if (!Game1.player.team.collectedNutTracker.Contains(walnutToken))
                        {
                            walnutNumberMissing = walnutNumberTotal;
                        }
                        break;
                    case WalnutType_MissingLimitedNutDrops:
                        walnutNumberMissing = walnutNumberTotal - Math.Max(Game1.player.team.GetDroppedLimitedNutCount(walnutToken), 0);
                        break;
                    case WalnutType_GoldenCoconutCracked:
                        if (!Game1.netWorldState.Value.GoldenCoconutCracked)
                        {
                            walnutNumberMissing = walnutNumberTotal;
                        }
                        break;
                    case WalnutType_GotBirdieReward:
                        if (!Game1.MasterPlayer.hasOrWillReceiveMail("gotBirdieReward"))
                        {
                            walnutNumberMissing = walnutNumberTotal;
                        }
                        break;
                }

                if (walnutNumberMissing > 0)
                {
                    if (!hintDictionary.ContainsKey(walnutHint))
                    {
                        hintDictionary[walnutHint] = 0;
                    }
                    hintDictionary[walnutHint] += walnutNumberMissing;
                }
            }

            foreach (var hint in hintDictionary
                .OrderBy(entry => entry.Key == "none" ? 2 : 1)
                .ThenBy(entry => GetHintText(entry.Key))
            )
            {
                var hintText = (hint.Key == "none") ? "???" : GetHintText(hint.Key);
                if (hint.Value > 1)
                {
                    hintText += $" ({hint.Value})";
                }
                linesToDisplay.Add($"* {hintText}{ModEntry.LineBreak}");
            }

            if (linesToDisplay.Count == 0)
            {
                var completeDescription = modInstance.Helper.Translation.Get("Walnuts_Complete", new { title = modInstance.GetTitle_Walnuts() });
                Game1.drawDialogueNoTyping(completeDescription);
                return;
            }

            modInstance.ShowLines(linesToDisplay, title: modInstance.GetTitle_Walnuts());
        }

    }
}
