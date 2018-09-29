using System;
using System.IO;
using System.Collections.Generic;
using StardewValley;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using Mopsy_Ranch_Livin;

namespace MopsyRanchLife
{
    public class ModEntry : Mod, IAssetEditor
    {
        private static readonly string DefaultSuffix = "Ranch";
        private string Suffix { get; set; } = DefaultSuffix;
        private int MaxSuffixLength { get; set; } = 10;
        private ModConfig _config;

        public override void Entry(IModHelper helper)
        {
            helper.ConsoleCommands.Add("farm_setsuffix", "Sets the player's farm suffix.\n\nUsage: farm_setsuffix <value>\n- value: farm suffix, e.g. Ranch.", ChangeFarmSuffix);
            SaveEvents.AfterLoad += this.SaveEvents_AfterLoad;
            SaveEvents.AfterReturnToTitle += this.SaveEvents_AfterReturnToTitle;
        }

        public void SaveEvents_AfterLoad(object sender, EventArgs e)
        {
            _config = Helper.ReadJsonFile<ModConfig>(Path.Combine(Constants.CurrentSavePath, "Mopsy-Ranch-Livin.json")) ?? new ModConfig();
            if (!String.IsNullOrEmpty(_config.Suffix) && _config.Suffix != Suffix)
            {
                ChangeFarmSuffix("config", new[] { _config.Suffix });
            }
        }

        public void SaveEvents_AfterReturnToTitle(object sender, EventArgs e)
        {
            ChangeFarmSuffix("config", new[] { DefaultSuffix });
        }

        public void ChangeFarmSuffix(string name, string[] args)
        {
            if (!Context.IsWorldReady)
            {
                Monitor.Log("You must load a save to use this command. Things will show as \"Ranch\" until then.", LogLevel.Info);
                return;
            }
            Suffix = args[0];
            if (String.IsNullOrEmpty(Suffix))
            {
                Monitor.Log($"You must specify a name to change your farm suffix to!", LogLevel.Info);
                return;
            }
            if (Suffix.Length > MaxSuffixLength)
            {
                Monitor.Log($"You may not use a farm name suffix longer than {MaxSuffixLength} characters long.", LogLevel.Info);
                return;
            }
            if (name == "farm_setsuffix")
            {
                _config.Suffix = Suffix;
                Helper.WriteJsonFile(Path.Combine(Constants.CurrentSavePath, "Mopsy-Ranch-Livin.json"), _config);
            }

            Helper.Content.InvalidateCache("Characters/Dialogue/Lewis");
            Helper.Content.InvalidateCache("Characters/Dialogue/MarriageDialogue");
            Helper.Content.InvalidateCache("Characters/Dialogue/MarriageDialogueEmily");
            Helper.Content.InvalidateCache("Characters/Dialogue/MarriageDialogueHarvey");
            Helper.Content.InvalidateCache("Characters/Dialogue/MarriageDialogueLeah");
            Helper.Content.InvalidateCache("Data/Events/BusStop");
            Helper.Content.InvalidateCache("Data/Events/FarmHouse");
            Helper.Content.InvalidateCache("Strings/Locations");
            Helper.Content.InvalidateCache("Strings/Notes");
            Helper.Content.InvalidateCache("Strings/SpeechBubbles");
            Helper.Content.InvalidateCache("Strings/StringsFromCSFiles");
            Helper.Content.InvalidateCache("Strings/StringsFromMaps");
            Helper.Content.InvalidateCache("Strings/UI");
        }

        /// <summary>Get whether this instance can edit the given asset.</summary>
        /// <param name="asset">Basic metadata about the asset being loaded.</param>
        public bool CanEdit<T>(IAssetInfo asset)
        {
            return
                asset.AssetNameEquals("Characters/Dialogue/Lewis")
                || asset.AssetNameEquals("Characters/Dialogue/MarriageDialogue")
                || asset.AssetNameEquals("Characters/Dialogue/MarriageDialogueEmily")
                || asset.AssetNameEquals("Characters/Dialogue/MarriageDialogueHarvey")
                || asset.AssetNameEquals("Characters/Dialogue/MarriageDialogueLeah")
                || asset.AssetNameEquals("Data/Events/BusStop")
                || asset.AssetNameEquals("Data/Events/FarmHouse")
                || asset.AssetNameEquals("Strings/Locations")
                || asset.AssetNameEquals("Strings/Notes")
                || asset.AssetNameEquals("Strings/SpeechBubbles")
                || asset.AssetNameEquals("Strings/StringsFromCSFiles")
                || asset.AssetNameEquals("Strings/StringsFromMaps")
                || asset.AssetNameEquals("Strings/UI");
        }

        /// <summary>Edit a matched asset.</summary>
        /// <param name="asset">A helper which encapsulates metadata about an asset and enables changes to it.</param>
        public void Edit<T>(IAssetData asset)
        {
            string lowerSuffix = Suffix.ToLower();
            IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;

            if (asset.AssetNameEquals("Characters/Dialogue/MarriageDialogue"))
            {
                data["Rainy_Day_Sebastian"] = data["Rainy_Day_Sebastian"].Replace("farm", lowerSuffix);
                data["Indoor_Day_4"] = data["Indoor_Day_4"].Replace("farm", lowerSuffix);
                data["Indoor_Day_Abigail"] = data["Indoor_Day_Abigail"].Replace("farm", lowerSuffix);
                data["Indoor_Night_Sam"] = data["Indoor_Night_Sam"].Replace("farm", lowerSuffix);
                data["Outdoor_1"] = data["Outdoor_1"].Replace("farm", lowerSuffix);
                //data["Outdoor_2"] = data["//Outdoor_2"].Replace("farm", lowerSuffix);
                data["Outdoor_Leah"] = data["Outdoor_Leah"].Replace("farm", lowerSuffix);
                data["Outdoor_Penny"] = data["Outdoor_Penny"].Replace("farm", lowerSuffix);
                data["Outdoor_Harvey"] = data["Outdoor_Harvey"].Replace("farm", lowerSuffix);
                data["Outdoor_Elliott"] = data["Outdoor_Elliott"].Replace("farm", lowerSuffix);
                data["Outdoor_Alex"] = data["Outdoor_Alex"].Replace("farm", lowerSuffix);
                data["OneKid_2"] = data["OneKid_2"].Replace("farm", lowerSuffix);
                data["Neutral_8"] = data["Neutral_8"].Replace("farm", lowerSuffix);
                //data["spring_Sebastian"] = data["//spring_Sebastian"].Replace("farm", lowerSuffix);
                data["spring_Elliott"] = data["spring_Elliott"].Replace(@"%farm farm", @"%farm " + lowerSuffix);
                data["fall_Penny"] = data["fall_Penny"].Replace("farm", lowerSuffix);
            }

            if (asset.AssetNameEquals("Characters/Dialogue/MarriageDialogueEmily"))
            {
                data["Indoor_Day_0"] = data["Indoor_Day_0"].Replace(@"%farm farm", @"%farm " + lowerSuffix);
                data["TwoKids_3"] = data["TwoKids_3"].Replace("farm", lowerSuffix);
            }

            if (asset.AssetNameEquals("Characters/Dialogue/MarriageDialogueHarvey"))
            {
                data["Indoor_Night_3"] = data["Indoor_Night_3"].Replace(@"%farm farm", @"%farm " + lowerSuffix);
            }

            if (asset.AssetNameEquals("Characters/Dialogue/MarriageDialogueLeah"))
            {
                data["TwoKids_3"] = data["TwoKids_3"].Replace("farm", lowerSuffix);
                data["Neutral_8"] = data["Neutral_8"].Replace("farm", lowerSuffix);
                data["winter_28"] = data["winter_28"].Replace(@"%farm farm", @"%farm " + lowerSuffix);
            }

            if (asset.AssetNameEquals("Characters/Dialogue/Lewis"))
            {
                data["Town_57_40"] = data["Town_57_40"].Replace("farm", lowerSuffix);
                data["Fri"] = data["Fri"].Replace(@"%farm farm", @"%farm " + lowerSuffix);
                data["summer_Sun"] = data["summer_Sun"].Replace("farm", lowerSuffix);
                data["fall_Town_57_40"] = data["fall_Town_57_40"].Replace("farm", lowerSuffix);
                data["fall_Fri"] = data["fall_Fri"].Replace("farm", lowerSuffix);
                data["fall_12"] = data["fall_12"].Replace("farm", lowerSuffix);
                data["fall_Sun"] = data["fall_Sun"].Replace("farm", lowerSuffix);
                data["winter_Fri"] = data["winter_Fri"].Replace("farm", lowerSuffix);
            }

            if (asset.AssetNameEquals("Data/Events/BusStop"))
            {
                data["60367/u 0"] = data["60367/u 0"].Replace("farm's", lowerSuffix + "'s");
                data["60367/u 0"] = data["60367/u 0"].Replace("%farm farm", "%farm " + lowerSuffix);
            }

            if (asset.AssetNameEquals("Data/Events/FarmHouse"))
            {
                data["558291/y 3/H"] = data["558291/y 3/H"].Replace("%farm Farm", "%farm " + Suffix);
            }

            if (asset.AssetNameEquals("Strings/Locations"))
            {
                data["ScienceHouse_CarpenterMenu_Construct"] = data["ScienceHouse_CarpenterMenu_Construct"].Replace("Farm", Suffix);
                data["Farm_WeedsDestruction"] = data["Farm_WeedsDestruction"].Replace("farm", lowerSuffix);
                data["WitchHut_EvilShrineRightActivate"] = data["WitchHut_EvilShrineRightActivate"].Replace("farm", lowerSuffix);
                data["WitchHut_EvilShrineRightDeActivate"] = data["WitchHut_EvilShrineRightDeActivate"].Replace("farm", lowerSuffix);
            }

            if (asset.AssetNameEquals("Strings/Notes"))
            {
                data["2"] = data["2"].Replace("farm", lowerSuffix);
                data["6"] = data["6"].Replace("farm", lowerSuffix);
            }

            if (asset.AssetNameEquals("Strings/SpeechBubbles"))
            {
                data["AnimalShop_Marnie_Greeting3"] = data["AnimalShop_Marnie_Greeting3"].Replace("Farm", Suffix);
            }

            if (asset.AssetNameEquals("Strings/StringsFromCSFiles"))
            {
                data["Event.cs.1306"] = data["Event.cs.1306"].Replace("farm", lowerSuffix);
                data["Event.cs.1308"] = data["Event.cs.1308"].Replace("Farm", Suffix);
                data["Event.cs.1315"] = data["Event.cs.1315"].Replace("farm", lowerSuffix);
                data["Event.cs.1317"] = data["Event.cs.1317"].Replace("Farm", Suffix);
                data["Event.cs.1843"] = data["Event.cs.1843"].Replace("farm", lowerSuffix);
                data["Farmer.cs.1972"] = data["Farmer.cs.1972"].Replace("farm", lowerSuffix);
                data["Game1.cs.2782"] = data["Game1.cs.2782"].Replace("farm", lowerSuffix);
                data["NPC.cs.4474"] = data["NPC.cs.4474"].Replace("farm", lowerSuffix);
                data["NPC.cs.4485"] = data["NPC.cs.4485"].Replace("Farm", Suffix);
                data["Utility.cs.5229"] = data["Utility.cs.5229"].Replace("farm", lowerSuffix);
                data["Utility.cs.5230"] = data["Utility.cs.5230"].Replace("lowerFind", lowerSuffix);
                data["BlueprintsMenu.cs.10012"] = data["BlueprintsMenu.cs.10012"].Replace("Farm", Suffix);
                data["CataloguePage.cs.10148"] = data["CataloguePage.cs.10148"].Replace("farm", lowerSuffix);
                data["LoadGameMenu.cs.11019"] = data["LoadGameMenu.cs.11019"].Replace("Farm", Suffix);
                data["MapPage.cs.11064"] = data["MapPage.cs.11064"].Replace("Farm", Suffix);
                data["GrandpaStory.cs.12051"] = data["GrandpaStory.cs.12051"].Replace("Farm", Suffix);
                data["GrandpaStory.cs.12055"] = data["GrandpaStory.cs.12055"].Replace("Farm", Suffix);
                data["HoeDirt.cs.13919"] = data["HoeDirt.cs.13919"].Replace("farm", lowerSuffix);
                data["Axe.cs.14023"] = data["Axe.cs.14023"].Replace("farm", lowerSuffix);
            }

            if (asset.AssetNameEquals("Strings/StringsFromMaps"))
            {
                data["Forest.1"] = data["Forest.1"].Replace("Farm", Suffix);
            }

            if (asset.AssetNameEquals("Strings/UI"))
            {
                data["Character_Farm"] = data["Character_Farm"].Replace("Farm", Suffix);
                data["Character_FarmNameSuffix"] = data["Character_FarmNameSuffix"].Replace("Farm", Suffix);
                data["Inventory_FarmName"] = data["Inventory_FarmName"].Replace("Farm", Suffix);
                data["CoopMenu_HostNewFarm"] = data["CoopMenu_HostNewFarm"].Replace("Farm", Suffix);
                data["CoopMenu_HostFile"] = data["CoopMenu_HostFile"].Replace("Farm", Suffix);
                data["CoopMenu_RevisitFriendFarm"] = data["CoopMenu_RevisitFriendFarm"].Replace("Farm", Suffix);
                data["CoopMenu_JoinFriendFarm"] = data["CoopMenu_JoinFriendFarm"].Replace("Farm", Suffix);
                data["Chat_MuseumComplete"] = data["Chat_MuseumComplete"].Replace("Farm", Suffix);
                data["Chat_Museum40"] = data["Chat_Museum40"].Replace("Farm", Suffix);
                data["Chat_Earned15k"] = data["Chat_Earned15k"].Replace("Farm", Suffix);
                data["Chat_Earned50k"] = data["Chat_Earned50k"].Replace("Farm", Suffix);
                data["Chat_Earned250k"] = data["Chat_Earned250k"].Replace("Farm", Suffix);
                data["Chat_Earned1m"] = data["Chat_Earned1m"].Replace("Farm", Suffix);
                data["Chat_Earned10m"] = data["Chat_Earned10m"].Replace("Farm", Suffix);
                data["Chat_Earned100m"] = data["Chat_Earned100m"].Replace("Farm", Suffix);
            }
        }
    }
}
