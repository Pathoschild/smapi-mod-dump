/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/SpousesIsland
**
*************************************************/

using StardewModdingAPI;
using System.Collections.Generic;

namespace SpousesIsland.Framework
{
    internal class Commands
    {
        public static void EditDialogue(ContentPackData cpd, IAssetData asset, IMonitor monitor)
        {
            IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
            data["marriage_islandhouse"] = cpd.ArrivalDialogue;
            data["marriage_loc1"] = cpd.Location1.Dialogue;
            data["marriage_loc2"] = cpd.Location2.Dialogue;
            if (cpd.Location3?.Dialogue is not null)
            {
                data["marriage_loc3"] = cpd.Location3.Dialogue;
            };
        }
        public static void EditSchedule(ContentPackData cpd, IAssetData asset)
        {
            IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;

            data["marriage_Mon"] = $"620 FishShop 4 7 0/900 IslandSouth 1 11/940 IslandWest 77 43 0/1020 IslandFarmHouse {cpd.ArrivalPosition} \"Characters\\Dialogue\\{cpd.Spousename}:marriage_islandhouse\"/{cpd.Location1.Time} {cpd.Location1.Name} {cpd.Location1.Position} \"Characters\\Dialogue\\{cpd.Spousename}:marriage_loc1\"/{cpd.Location2.Time} {cpd.Location2.Name} {cpd.Location2.Position} \"Characters\\Dialogue\\{cpd.Spousename}:marriage_loc2\" {cpd.Location3.Time} {cpd.Location3.Name} {cpd.Location3.Position} \"Characters\\Dialogue\\{cpd.Spousename}:marriage_loc3\"/a2150 IslandFarmHouse {cpd.ArrivalPosition}";
            data["marriage_Tue"] = "GOTO marriage_Mon";
            data["marriage_Wed"] = "GOTO marriage_Mon";
            data["marriage_Thu"] = "GOTO marriage_Mon";
            data["marriage_Fri"] = "GOTO marriage_Mon";
            data["marriage_Sat"] = "GOTO marriage_Mon";
            data["marriage_Sun"] = "GOTO marriage_Mon";
        }

        /// <summary>
        /// Checks a translation's key. If it's one of the game's language codes, returns its filename/extension. If not, returns the language code as-is.
        /// </summary>
        internal static string ParseLangCode(string key)
        {
            switch (key.ToLower())
            {
                case "de":
                    return ".de-DE";
                case "en":
                    return "";
                case "es":
                    return ".es-ES";
                case "fr":
                    return ".fr-FR";
                case "hu":
                    return ".hu-HU";
                case "it":
                    return ".it-IT";
                case "ja":
                    return ".ja-JP";
                case "ko":
                    return ".ko-KR";
                case "pt":
                    return ".pt-BR";
                case "ru":
                    return ".ru-RU";
                case "tr":
                    return ".tr-TR";
                case "zh":
                    return ".zh-CN";
                case null:
                    return "";
                default:
                    return $".{key}";
            }
        }

        /// <summary>
        /// Checks the values of the contentpack provided. If any conflict, returns a detailed error and the value "false" (to indicate the pack isn't valid).
        /// </summary>
        /// <param name="cpd">The content pack being parsed.</param>
        /// <param name="monitor">Monitor, used to inform of any errors</param>
        /// <returns></returns>
        internal static bool ParseContentPack(ContentPackData cpd, IMonitor monitor)
        {
            bool tempbool = false;
            if (cpd.ArrivalPosition is null)
            {
                monitor.Log($"There's no arrival position in {cpd.Spousename}'s schedule!", LogLevel.Error);
                tempbool = true;
            }
            if (cpd.ArrivalDialogue is null)
            {
                monitor.Log($"There's no arrival dialogue in {cpd.Spousename}'s schedule!", LogLevel.Error);
                tempbool = true;
            }
            if (cpd.Location1?.Time >= cpd.Location2?.Time)
            {
                monitor.Log($"Location1's Time ({cpd.Location1.Time}) conflicts with Location2's Time ({cpd.Location2.Time})! This will cause errors. (Make sure the values aren't the same, and that Time1 happens before Time2.)", LogLevel.Warn);
                tempbool = true;
            }
            if (cpd.Location1?.Time >= cpd.Location3?.Time)
            {
                monitor.Log($"Location1's Time ({cpd.Location1.Time}) conflicts with Location3's Time ({cpd.Location3.Time})! This will cause errors. (Make sure the values aren't the same, and that Time1 happens before Time3.)", LogLevel.Warn);
                tempbool = true;
            }
            if (cpd.Location2?.Time >= cpd.Location3?.Time)
            {
                monitor.Log($"Location2's Time ({cpd.Location2.Time}) conflicts with Location3's Time ({cpd.Location3.Time})! This will cause errors. (Make sure the values aren't the same, and that Time2 happens before Time3.)", LogLevel.Warn);
                tempbool = true;
            }
            if (cpd.Location1.Name is null)
            {
                monitor.Log($"There's no name for the Location1 map. Add the field and try again.", LogLevel.Error);
                tempbool = true;
            }
            if (cpd.Location1.Time <= 1020 || cpd.Location1.Time > 2150)
            {
                monitor.Log("Make sure the arrival time of Location1 is between 1030 and 2150.", LogLevel.Error);
                tempbool = true;
            }
            if (cpd.Location1.Position is null)
            {
                monitor.Log($"There's no position for {cpd.Spousename} in Location1.", LogLevel.Error);
                tempbool = true;
            }
            if (cpd.Location1.Dialogue is null)
            {
                monitor.Log($"There's no dialogue for {cpd.Spousename} in Location1.", LogLevel.Error);
                tempbool = true;
            }
            if (cpd.Location2.Name is null)
            {
                monitor.Log($"There's no name for the Location2 map. Add the field and try again.", LogLevel.Error);
                tempbool = true;
            }
            if (cpd.Location2.Time <= 1020 || cpd.Location2.Time > 2150)
            {
                monitor.Log("Make sure the arrival time of Location2 is between 1030 and 2150.", LogLevel.Error);
                tempbool = true;
            }
            if (cpd.Location2.Position is null)
            {
                monitor.Log($"There's no position for {cpd.Spousename} in Location2.", LogLevel.Error);
                tempbool = true;
            }
            if (cpd.Location2.Dialogue is null)
            {
                monitor.Log($"There's no dialogue for {cpd.Spousename} in Location2.", LogLevel.Error);
                tempbool = true;
            }
            if (cpd.Location3 is not null)
            {
                if (cpd.Location3.Name is null)
                {
                    monitor.Log("There's no name for the Location3 map. Add the field and try again.", LogLevel.Error);
                    tempbool = true;
                }
                if (cpd.Location3.Time <= 1020 || cpd.Location3.Time > 2150)
                {
                    monitor.Log("Make sure the arrival time of Location3 is between 1030 and 2150.", LogLevel.Error);
                    tempbool = true;
                }
                if (cpd.Location3.Position is null)
                {
                    monitor.Log($"There's no position for {cpd.Spousename} in Location3.", LogLevel.Error);
                    tempbool = true;
                }
                if (cpd.Location3.Dialogue is null)
                {
                    monitor.Log($"There's no dialogue for {cpd.Spousename} in Location3.", LogLevel.Error);
                    tempbool = true;
                }
            }

            int TranslationN = 0;
            foreach (DialogueTranslation kpv in cpd.Translations)
            {
                TranslationN++;
                if (!IsListValid(kpv))
                {
                    if (kpv.Key is null)
                    {
                        monitor.LogOnce($"Translation key missing in {cpd.Spousename}'s content. The translation won't be added. (Array {TranslationN} in list)", LogLevel.Warn);
                        tempbool = true;
                    }
                    if (kpv.Location1 is null)
                    {
                        monitor.LogOnce($"Location1 missing in {cpd.Spousename}'s content. The translation won't be added. (Array {TranslationN} in list)", LogLevel.Warn);
                        tempbool = true;
                    }
                    if (kpv.Location2 is null)
                    {
                        monitor.LogOnce($"Location2 missing in {cpd.Spousename}'s content. The translation won't be added. (Array {TranslationN} in list)", LogLevel.Warn);
                        tempbool = true;
                    }
                };
            }
            if (tempbool is true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Compares the integers provided. If conditions apply, returns true (to reload assets).
        /// </summary>
        /// <param name="Previous"> The Random number chosen the previous in-game day.</param>
        /// <param name="CustomChance"> The custom number set by the user.</param>
        /// <param name="Current"> Today(in-game)'s randomly chosen number. Values range from 0-100.</param>
        /// <returns></returns>
        internal static bool ParseReloadCondition(int Previous, int CustomChance, int Current)
        {
            if ((CustomChance < Current && CustomChance >= Previous) || CustomChance >= Current)
            {
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Checks the validity of a specified DialogueTranslation. If the values aren't null or whitespace, returns true.
        /// </summary>
        internal static bool IsListValid(DialogueTranslation kpv)
        {
            if (!string.IsNullOrWhiteSpace(kpv.Key) && !string.IsNullOrWhiteSpace(kpv.Location1) && !string.IsNullOrWhiteSpace(kpv.Location2))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        internal static bool IsLoc3Valid(ContentPackData cpd)
        {
            if (cpd.Location3?.Time is not 0 && !string.IsNullOrWhiteSpace(cpd.Location3?.Name) && !string.IsNullOrWhiteSpace(cpd.Location3?.Position) && !string.IsNullOrWhiteSpace(cpd.Location3?.Dialogue))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        internal static bool ShouldReloadDevan(bool Leah, bool Elliott, bool CCC)
        {
            if (Leah is true || Elliott is true || CCC is true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        internal static bool HasMod(string ModID, IModHelper Helper)
        {
            if (Helper.ModRegistry.Get(ModID) is not null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
