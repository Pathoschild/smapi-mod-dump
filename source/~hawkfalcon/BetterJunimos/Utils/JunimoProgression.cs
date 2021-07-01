/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/hawkfalcon/Stardew-Mods
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Objects;
using System;
using BetterJunimos.Abilities;

namespace BetterJunimos.Utils {
    public class ProgressionData {
        public bool CanWorkInEveningsPrompted { get; set; }
        public bool CanWorkInEveningsUnlocked { get; set; }
        public bool CanWorkInRainPrompted { get; set; }
        public bool CanWorkInRainUnlocked { get; set; }
        public bool CanWorkInWinterPrompted { get; set; }
        public bool CanWorkInWinterUnlocked { get; set; }
        public bool ClearDeadCropsPrompted { get; set; }
        public bool ClearDeadCropsUnlocked { get; set; }
        public bool FertilizePrompted { get; set; }
        public bool FertilizeUnlocked { get; set; }
        public bool HarvestBushesPrompted { get; set; }
        public bool HarvestBushesUnlocked { get; set; }
        public bool HarvestForageCropsPrompted { get; set; }
        public bool HarvestForageCropsUnlocked { get; set; }
        public bool MoreJunimosPrompted { get; set; }
        public bool MoreJunimosUnlocked { get; set; }
        public bool PlantCropsPrompted { get; set; }
        public bool PlantCropsUnlocked { get; set; }
        public bool ReducedCostToConstructPrompted { get; set; }
        public bool ReducedCostToConstructUnlocked { get; set; }
        public bool UnlimitedJunimosPrompted { get; set; }
        public bool UnlimitedJunimosUnlocked { get; set; }
        public bool WaterPrompted { get; set; }
        public bool WaterUnlocked { get; set; }
        public bool WorkFasterPrompted { get; set; }
        public bool WorkFasterUnlocked { get; set; }
    }

    public class JunimoProgression {
        private const int INITIAL_JUNIMOS_LIMIT = 3;
        private const int MORE_JUNIMOS_LIMIT = 6;

        internal ModConfig Config;
        internal ProgressionData ProgData;
        private readonly IMonitor Monitor;
        private readonly IModHelper Helper;

        public Dictionary<int, List<int>> JunimoPaymentsToday = new Dictionary<int, List<int>>();

        internal JunimoProgression(ModConfig Config, IMonitor Monitor, IModHelper Helper) {
            this.Config = Config;
            this.Monitor = Monitor;
            this.Helper = Helper;
        }

        public void Initialize(ProgressionData ProgData) {
            this.ProgData = ProgData;
        }

        public int MaxJunimosUnlocked {
            get {
                if (!Config.Progression.Enabled) return Config.JunimoHuts.MaxJunimos;
                if (ProgData.UnlimitedJunimosUnlocked) return Config.JunimoHuts.MaxJunimos;
                if (ProgData.MoreJunimosUnlocked) return Math.Min(MORE_JUNIMOS_LIMIT, Config.JunimoHuts.MaxJunimos); ;
                return Math.Min(INITIAL_JUNIMOS_LIMIT, Config.JunimoHuts.MaxJunimos);
            }
        }

        public bool ReducedCostToConstruct { 
            get {
                if (!Util.Config.JunimoHuts.ReducedCostToConstruct) return false;
                if (!Config.Progression.Enabled) return true;
                if (ProgData is null) return Util.Config.JunimoHuts.ReducedCostToConstruct;
                return ProgData.ReducedCostToConstructUnlocked;
            }
        }

        public bool CanWorkInEvenings {
            get {
                if (!Util.Config.JunimoImprovements.CanWorkInEvenings) return false;
                if (!Config.Progression.Enabled) return true;
                return ProgData.CanWorkInEveningsUnlocked;
            }
        }

        public bool CanWorkInRain {
            get {
                if (!Util.Config.JunimoImprovements.CanWorkInRain) return false;
                if (!Config.Progression.Enabled) return true;
                return ProgData.CanWorkInRainUnlocked;
            }
        }

        public bool CanWorkInWinter {
            get {
                if (!Util.Config.JunimoImprovements.CanWorkInWinter) return false;
                if (!Config.Progression.Enabled) return true;
                return ProgData.CanWorkInWinterUnlocked;
            }
        }

        public bool WorkFaster {
            get {
                if (!Util.Config.JunimoImprovements.WorkFaster) return false;
                if (!Config.Progression.Enabled) return true;
                return ProgData.WorkFasterUnlocked;
            }
        }

        public bool CanUseAbility(IJunimoAbility ability) {
            if (!Config.Progression.Enabled) return true;
            PromptForAbility(ability);
            return Unlocked(ability);
        }

        private bool Unlocked(IJunimoAbility ability) {
            if (ability.AbilityName() == "ClearDeadCrops") return ProgData.ClearDeadCropsUnlocked;
            if (ability.AbilityName() == "Fertilize") return ProgData.FertilizeUnlocked;
            if (ability.AbilityName() == "HarvestBushes") return ProgData.HarvestBushesUnlocked;
            if (ability.AbilityName() == "HarvestForageCrops") return ProgData.HarvestForageCropsUnlocked;
            if (ability.AbilityName() == "PlantCrops") return ProgData.PlantCropsUnlocked;
            if (ability.AbilityName() == "Water") return ProgData.WaterUnlocked;
            if (ability.AbilityName() == "HarvestCrops") return true;
            Monitor.Log($"Unlocked: progression checked for {ability.AbilityName()} but not implemented", LogLevel.Debug);
            return true;
        }

        private bool Prompted(IJunimoAbility ability) {
            if (ability.AbilityName() == "ClearDeadCrops") return ProgData.ClearDeadCropsPrompted;
            if (ability.AbilityName() == "Fertilize") return ProgData.FertilizePrompted;
            if (ability.AbilityName() == "HarvestBushes") return ProgData.HarvestBushesPrompted;
            if (ability.AbilityName() == "HarvestForageCrops") return ProgData.HarvestForageCropsPrompted;
            if (ability.AbilityName() == "PlantCrops") return ProgData.PlantCropsPrompted;
            if (ability.AbilityName() == "Water") return ProgData.WaterPrompted;
            if (ability.AbilityName() == "HarvestCrops") return true;
            Monitor.Log($"Prompted: progression checked for {ability.AbilityName()} but not implemented", LogLevel.Debug);
            return true;
        }

        public void PromptForCanWorkInEvenings() {
            if (!Config.Progression.Enabled || ProgData.CanWorkInEveningsUnlocked || ProgData.CanWorkInEveningsPrompted) return;
            if (!Config.JunimoImprovements.CanWorkInEvenings) return;
            Util.SendMessage(GetPromptText("WorkInEvenings", Config.Progression.CanWorkInEvenings.Item, Config.Progression.CanWorkInEvenings.Stack));
            ProgData.CanWorkInEveningsPrompted = true;
        }

        public void DayStartedProgressionPrompt(bool isWinter, bool isRaining) {
            string prompt = null;

            if (!Config.Progression.Enabled) return;

            if (isWinter && !ProgData.CanWorkInWinterUnlocked && !ProgData.CanWorkInWinterPrompted && Config.JunimoImprovements.CanWorkInWinter) {
                prompt = Util.Progression.GetPromptText("CanWorkInWinter", Config.Progression.CanWorkInWinter.Item, Config.Progression.CanWorkInWinter.Stack);
                ProgData.CanWorkInWinterPrompted = true;
            }
            else if (isRaining && !ProgData.CanWorkInRainUnlocked && !ProgData.CanWorkInRainPrompted && Config.JunimoImprovements.CanWorkInRain) {
                prompt = Util.Progression.GetPromptText("CanWorkInRain", Config.Progression.CanWorkInRain.Item, Config.Progression.CanWorkInRain.Stack);
                ProgData.CanWorkInRainPrompted = true;
            }
            else if (Config.JunimoHuts.MaxJunimos >= MaxJunimosUnlocked && !ProgData.MoreJunimosUnlocked && !ProgData.MoreJunimosPrompted) {
                prompt = Util.Progression.GetPromptText("MoreJunimos", Config.Progression.MoreJunimos.Item, Config.Progression.MoreJunimos.Stack);
                ProgData.MoreJunimosPrompted = true;
            }
            else if (Config.JunimoHuts.MaxJunimos >= MaxJunimosUnlocked && ProgData.MoreJunimosUnlocked && !ProgData.UnlimitedJunimosUnlocked && !ProgData.UnlimitedJunimosPrompted) {
                prompt = Util.Progression.GetPromptText("UnlimitedJunimos", Config.Progression.UnlimitedJunimos.Item, Config.Progression.UnlimitedJunimos.Stack);
                ProgData.UnlimitedJunimosPrompted = true;
            }
            else if (!ProgData.WorkFasterUnlocked && !ProgData.WorkFasterPrompted && Config.JunimoImprovements.WorkFaster) {
                prompt = Util.Progression.GetPromptText("WorkFaster", Config.Progression.WorkFaster.Item, Config.Progression.WorkFaster.Stack);
                ProgData.WorkFasterPrompted = true;
            }
            else if (!ProgData.ReducedCostToConstructUnlocked && !ProgData.ReducedCostToConstructPrompted && Config.JunimoHuts.ReducedCostToConstruct) {
                prompt = Util.Progression.GetPromptText("ReducedCostToConstruct", Config.Progression.ReducedCostToConstruct.Item, Config.Progression.ReducedCostToConstruct.Stack);
                ProgData.ReducedCostToConstructPrompted = true;
            }

            if (prompt is not null) {
                Util.SendMessage(prompt);
            }
        }

        public void PromptForAbility(IJunimoAbility ability) {
            if (Unlocked(ability) || Prompted(ability)) return;
            string an = ability.AbilityName();
            switch (an) {
                case "ClearDeadCrops":
                    Util.SendMessage(GetPromptText(an, Config.Progression.ClearDeadCrops.Item, Config.Progression.ClearDeadCrops.Stack));
                    ProgData.ClearDeadCropsPrompted = true;
                    break;

                case "Fertilize":
                    Util.SendMessage(GetPromptText(an, Config.Progression.Fertilize.Item, Config.Progression.Fertilize.Stack));
                    ProgData.FertilizePrompted = true;
                    break;

                case "HarvestBushes":
                    Util.SendMessage(GetPromptText(an, Config.Progression.HarvestBushes.Item, Config.Progression.HarvestBushes.Stack));
                    ProgData.HarvestBushesPrompted = true;
                    break;

                case "HarvestForageCrops":
                    Util.SendMessage(GetPromptText(an, Config.Progression.HarvestForageCrops.Item, Config.Progression.HarvestForageCrops.Stack));
                    ProgData.HarvestForageCropsPrompted = true;
                    break;

                case "PlantCrops":
                    Util.SendMessage(GetPromptText(an, Config.Progression.PlantCrops.Item, Config.Progression.PlantCrops.Stack));
                    ProgData.PlantCropsPrompted = true;
                    break;

                case "Water":
                    Util.SendMessage(GetPromptText(an, Config.Progression.Water.Item, Config.Progression.Water.Stack));
                    ProgData.WaterPrompted = true;
                    break;

                default:
                    Monitor.Log($"PromptForAbility: progression prompt check needed for {an} but not implemented", LogLevel.Debug);
                    break;
            }
        }

        public string GetPromptText(string ability, int item, int stack) {
            string text_key = ability + "PromptText";
            StardewValley.Object i = new StardewValley.Object(item, stack);
            string prompt = Helper.Translation.Get(text_key).ToString();
            if (prompt.Contains("no translation")) {
                prompt = "For {{qty}} {{item}}, the Junimos can " + ability.ToLower();
                Monitor.Log($"GetPromptText: no translation for {text_key}", LogLevel.Debug);
            }
            return prompt.Replace("{{qty}}", stack.ToString()).Replace("{{item}}", i.DisplayName);
        }

        public string GetSuccessText(string ability) {
            string text_key = ability + "SuccessText";
            string prompt = Helper.Translation.Get(text_key).ToString();
            if (prompt.Contains("no translation")) {
                prompt = "Now the Junimos can " + ability.ToLower();
                Monitor.Log($"GetSuccessText: no translation for {text_key}", LogLevel.Debug);
            }
            return prompt;
        }

        public void ReceiveProgressionItems(JunimoHut hut) {
            Chest chest = hut.output.Value;

            if (!ProgData.CanWorkInEveningsUnlocked && ProgData.CanWorkInEveningsPrompted) {
                if (ReceiveItems(chest, Config.Progression.CanWorkInEvenings.Stack, Config.Progression.CanWorkInEvenings.Item)) {
                    ProgData.CanWorkInEveningsUnlocked = true;
                    Util.SendMessage(GetSuccessText("CanWorkInEvenings"));
                }
            }

            if (!ProgData.CanWorkInRainUnlocked && ProgData.CanWorkInRainPrompted) {
                if (ReceiveItems(chest, Config.Progression.CanWorkInRain.Stack, Config.Progression.CanWorkInRain.Item)) {
                    ProgData.CanWorkInRainUnlocked = true;
                    Util.SendMessage(GetSuccessText("CanWorkInRain"));
                }
            }

            if (!ProgData.CanWorkInWinterUnlocked && ProgData.CanWorkInWinterPrompted) {
                if (ReceiveItems(chest, Config.Progression.CanWorkInWinter.Stack, Config.Progression.CanWorkInWinter.Item)) {
                    ProgData.CanWorkInWinterUnlocked = true;
                    Util.SendMessage(GetSuccessText("CanWorkInWinter"));
                }
            }

            if (!ProgData.MoreJunimosUnlocked && ProgData.MoreJunimosPrompted) {
                if (ReceiveItems(chest, Config.Progression.MoreJunimos.Stack, Config.Progression.MoreJunimos.Item)) {
                    ProgData.MoreJunimosUnlocked = true;
                    Util.SendMessage(GetSuccessText("MoreJunimos"));
                }
            }

            if (!ProgData.ReducedCostToConstructUnlocked && ProgData.ReducedCostToConstructPrompted) {
                if (ReceiveItems(chest, Config.Progression.ReducedCostToConstruct.Stack, Config.Progression.ReducedCostToConstruct.Item)) {
                    ProgData.ReducedCostToConstructUnlocked = true;
                    Util.SendMessage(GetSuccessText("ReducedCostToConstruct"));
                }
            }

            if (!ProgData.UnlimitedJunimosUnlocked && ProgData.UnlimitedJunimosPrompted) {
                if (ReceiveItems(chest, Config.Progression.UnlimitedJunimos.Stack, Config.Progression.UnlimitedJunimos.Item)) {
                    ProgData.UnlimitedJunimosUnlocked = true;
                    Util.SendMessage(GetSuccessText("UnlimitedJunimos"));
                }
            }

            if (!ProgData.WorkFasterUnlocked && ProgData.WorkFasterPrompted) {
                if (ReceiveItems(chest, Config.Progression.WorkFaster.Stack, Config.Progression.WorkFaster.Item)) {
                    ProgData.WorkFasterUnlocked = true;
                    Util.SendMessage(GetSuccessText("WorkFaster"));
                }
            }

            if (!ProgData.ClearDeadCropsUnlocked && ProgData.ClearDeadCropsPrompted) {
                if (ReceiveItems(chest, Config.Progression.ClearDeadCrops.Stack, Config.Progression.ClearDeadCrops.Item)) {
                    ProgData.ClearDeadCropsUnlocked = true;
                    Util.SendMessage(GetSuccessText("ClearDeadCrops"));
                }
            }

            if (!ProgData.FertilizeUnlocked && ProgData.FertilizePrompted) {
                if (ReceiveItems(chest, Config.Progression.Fertilize.Stack, Config.Progression.Fertilize.Item)) {
                    ProgData.FertilizeUnlocked = true;
                    Util.SendMessage(GetSuccessText("Fertilize"));
                }
            }

            if (!ProgData.HarvestBushesUnlocked && ProgData.HarvestBushesPrompted) {
                if (ReceiveItems(chest, Config.Progression.HarvestBushes.Stack, Config.Progression.HarvestBushes.Item)) {
                    ProgData.HarvestBushesUnlocked = true;
                    Util.SendMessage(GetSuccessText("HarvestBushes"));
                }
            }

            if (!ProgData.HarvestForageCropsUnlocked && ProgData.HarvestForageCropsPrompted) {
                if (ReceiveItems(chest, Config.Progression.HarvestForageCrops.Stack, Config.Progression.HarvestForageCrops.Item)) {
                    ProgData.HarvestForageCropsUnlocked = true;
                    Util.SendMessage(GetSuccessText("HarvestForageCrops"));
                }
            }

            if (!ProgData.PlantCropsUnlocked && ProgData.PlantCropsPrompted) {
                if (ReceiveItems(chest, Config.Progression.PlantCrops.Stack, Config.Progression.PlantCrops.Item)) {
                    ProgData.PlantCropsUnlocked = true;
                    Util.SendMessage(GetSuccessText("PlantCrops"));
                }
            }

            if (!ProgData.WaterUnlocked && ProgData.WaterPrompted) {
                if (ReceiveItems(chest, Config.Progression.Water.Stack, Config.Progression.Water.Item)) {
                    ProgData.WaterUnlocked = true;
                    Util.SendMessage(GetSuccessText("Water"));
                }
            }
        }

        private bool ReceiveItems(Chest chest, int needed, int index) {
            // Monitor.Log($"ReceiveItems wants {needed} of [{index}]", LogLevel.Debug);

            if (needed == 0) return true;
            List<int> items;
            if (!JunimoPaymentsToday.TryGetValue(index, out items)) {
                items = new List<int>();
                JunimoPaymentsToday[index] = items;
            }
            int paidSoFar = items.Count();
            // Monitor.Log($"ReceiveItems got {paidSoFar} of [{index}] already", LogLevel.Debug);

            if (paidSoFar == needed) return true;

            foreach (int i in Enumerable.Range(paidSoFar, needed)) {
                Item foundItem = chest.items.FirstOrDefault(item => item != null && item.ParentSheetIndex == index);
                if (foundItem != null) {
                    items.Add(foundItem.ParentSheetIndex);
                    Util.RemoveItemFromChest(chest, foundItem);
                }
            }
            // Monitor.Log($"ReceiveItems finished with {items.Count()} of [{index}]", LogLevel.Debug);
            return items.Count() >= needed;
        }
    }
}
