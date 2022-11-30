/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using EverlastingBaitsAndUnbreakableTacklesMod.integrations;
using MailFrameworkMod;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Quests;

namespace EverlastingBaitsAndUnbreakableTacklesMod
{
    public class DataLoader
    {
        private const string CraftingrecipesAssetName = "Data/CraftingRecipes";
        public static IModHelper Helper;
        public static ITranslationHelper I18N;
        public static ModConfig ModConfig;
        public static CraftingData CraftingData;

        public DataLoader(IModHelper helper, IManifest manifest)
        {
            Helper = helper;
            I18N = helper.Translation;
            ModConfig = helper.ReadConfig<ModConfig>();

            CraftingData = DataLoader.Helper.Data.ReadJsonFile<CraftingData>("data\\CraftingRecipes.json") ?? new CraftingData();
            DataLoader.Helper.Data.WriteJsonFile("data\\CraftingRecipes.json", CraftingData);

            Helper.Events.Content.AssetRequested += this.Edit;


            MailDao.SaveLetter
            (
                new Letter
                (
                    "IridiumQualityFishWithWildBait"
                    ,I18N.Get("IridiumQualityFishWithWildBait.Letter")
                    ,(l)=> !ModConfig.DisableIridiumQualityFish && !Game1.player.mailReceived.Contains(l.Id) && Game1.player.craftingRecipes.ContainsKey("Wild Bait") && Game1.player.FishingLevel >= 4
                    , (l)=> Game1.player.mailReceived.Add(l.Id)
                )
            );

            AddLetter(BaitTackle.EverlastingBait, (l)=> !ModConfig.DisableBaits && Game1.player.FishingLevel >= 10 && CheckNpcFriendship("Willy", 10) && !Game1.player.craftingRecipes.ContainsKey(BaitTackle.EverlastingBait.GetDescription()));
            AddLetter(BaitTackle.EverlastingWildBait, (l)=> !ModConfig.DisableBaits && Game1.player.craftingRecipes.ContainsKey("Wild Bait") && Game1.player.craftingRecipes.ContainsKey(BaitTackle.EverlastingBait.GetDescription()) && Game1.player.craftingRecipes[BaitTackle.EverlastingBait.GetDescription()] > 0 && CheckNpcFriendship("Linus", 10) && !Game1.player.craftingRecipes.ContainsKey(BaitTackle.EverlastingWildBait.GetDescription()));
            AddLetter(BaitTackle.EverlastingMagnet, (l)=> !ModConfig.DisableBaits && Game1.player.FishingLevel >= 10  && CheckNpcFriendship("Wizard", 10) && !Game1.player.craftingRecipes.ContainsKey(BaitTackle.EverlastingMagnet.GetDescription()), null,2);
            MailDao.SaveLetter
            (
                new Letter
                (
                    "UnbreakableTackleIntroduction"
                    , I18N.Get("UnbreakableTackleIntroduction.Letter")
                    , (l)=> !ModConfig.DisableTackles && !Game1.player.mailReceived.Contains(l.Id) && Game1.player.achievements.Contains(21) && Game1.player.FishingLevel >= 8 && CheckNpcFriendship("Willy", 6) && CheckNpcFriendship("Clint", 6)
                    , (l)=> Game1.player.mailReceived.Add(l.Id)
                )
            );
            AddLetter
            (
                BaitTackle.UnbreakableSpinner
                , (l) => !ModConfig.DisableTackles && Game1.player.achievements.Contains(21) && Game1.player.FishingLevel >= 8 && CheckNpcFriendship("Willy", 6) && CheckNpcFriendship("Clint", 6) && !Game1.player.craftingRecipes.ContainsKey(BaitTackle.UnbreakableSpinner.GetDescription())
                , (l) => LoadTackleQuest(BaitTackle.UnbreakableSpinner)
            );
            AddLetter
            (
                BaitTackle.UnbreakableLeadBobber
                , (l) => !ModConfig.DisableTackles && Game1.player.mailReceived.Contains(BaitTackle.UnbreakableSpinner.GetQuestName()) && !Game1.player.craftingRecipes.ContainsKey(BaitTackle.UnbreakableLeadBobber.GetDescription())
                , (l) => LoadTackleQuest(BaitTackle.UnbreakableLeadBobber)
            );
            AddLetter
            (
                BaitTackle.UnbreakableTrapBobber
                , (l) => !ModConfig.DisableTackles && Game1.player.mailReceived.Contains(BaitTackle.UnbreakableLeadBobber.GetQuestName()) && !Game1.player.craftingRecipes.ContainsKey(BaitTackle.UnbreakableTrapBobber.GetDescription())
                , (l) => LoadTackleQuest(BaitTackle.UnbreakableTrapBobber)
            );
            AddLetter
            (
                BaitTackle.UnbreakableCorkBobber
                , (l) => !ModConfig.DisableTackles && Game1.player.mailReceived.Contains(BaitTackle.UnbreakableTrapBobber.GetQuestName()) && !Game1.player.craftingRecipes.ContainsKey(BaitTackle.UnbreakableCorkBobber.GetDescription())
                , (l) => LoadTackleQuest(BaitTackle.UnbreakableCorkBobber)
            );
            AddLetter
            (
                BaitTackle.UnbreakableTreasureHunter
                , (l) => !ModConfig.DisableTackles && Game1.player.mailReceived.Contains(BaitTackle.UnbreakableCorkBobber.GetQuestName()) && !Game1.player.craftingRecipes.ContainsKey(BaitTackle.UnbreakableTreasureHunter.GetDescription())
                , (l) => LoadTackleQuest(BaitTackle.UnbreakableTreasureHunter)
            );
            AddLetter
            (
                BaitTackle.UnbreakableBarbedHook
                , (l) => !ModConfig.DisableTackles && Game1.player.mailReceived.Contains(BaitTackle.UnbreakableTreasureHunter.GetQuestName()) && !Game1.player.craftingRecipes.ContainsKey(BaitTackle.UnbreakableBarbedHook.GetDescription())
                , (l) => LoadTackleQuest(BaitTackle.UnbreakableBarbedHook)
            );
            AddLetter
            (
                BaitTackle.UnbreakableDressedSpinner
                , (l) => !ModConfig.DisableTackles && Game1.player.mailReceived.Contains(BaitTackle.UnbreakableBarbedHook.GetQuestName()) && !Game1.player.craftingRecipes.ContainsKey(BaitTackle.UnbreakableDressedSpinner.GetDescription())
                , (l) => LoadTackleQuest(BaitTackle.UnbreakableDressedSpinner)
            );
            MailDao.SaveLetter
            (
                new Letter
                (
                    "UnbreakableTackleReward"
                    , I18N.Get("UnbreakableTackleReward.Letter")
                    , new List<Item> { new StardewValley.Object(74,1) }
                    , (l) => !ModConfig.DisableTackles && Game1.player.mailReceived.Contains(BaitTackle.UnbreakableDressedSpinner.GetQuestName()) && !Game1.player.mailReceived.Contains(l.Id)
                    , (l) =>
                    {
                        Game1.player.mailReceived.Add(l.Id);
                    }
                )
            );

            CreateConfigMenu(manifest);
        }

        public void ReloadQuestWhenClient()
        {
            if (!Context.IsMainPlayer)
            {
                foreach (Quest quest in Game1.player.questLog)
                {
                    if (Enum.IsDefined(typeof(BaitTackle), quest.id.Value))
                    {
                        LoadQuestText(quest);
                    }
                }
            }
        }

        public static void LoadTackleQuest(BaitTackle baitTackle)
        {
            Quest quest = new Quest();
            quest.id.Value = (int)baitTackle;
            quest.questType.Value = 1;
            LoadQuestText(quest);
            quest.showNew.Value = true;
            quest.moneyReward.Value = 0;
            quest.rewardDescription.Value = (string) null;
            quest.canBeCancelled.Value = false;
            Game1.player.questLog.Add(quest);
            Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Farmer.cs.2011"), 2));
        }

        private static void LoadQuestText(Quest quest)
        {
            BaitTackle baitTackle = (BaitTackle)quest.id.Value;
            string baitTackleName = I18N.Get($"{baitTackle.ToString()}.Name");
            quest.questTitle = baitTackleName;
            quest.questDescription = I18N.Get("Quest.Description", new {Item = baitTackleName});
            string questObjective;
            if (Game1.player.craftingRecipes.ContainsKey(baitTackle.GetDescription()) &&
                Game1.player.craftingRecipes[baitTackle.GetDescription()] > 0)
            {
                questObjective = "Quest.LastObjective";
            }
            else
            {
                questObjective = "Quest.FirstObjective";
            }

            quest.currentObjective = I18N.Get(questObjective, new { Item = baitTackleName });
        }

        private void AddLetter(BaitTackle baitTackle, Func<Letter, bool> condition, Action<Letter> callback = null,int whichBG = 0)
        {
            MailDao.SaveLetter(new Letter(baitTackle.ToString() + "Recipe", I18N.Get(baitTackle.ToString() + ".Letter"), baitTackle.GetDescription(), condition, callback, whichBG));
        }
        
        public void Edit(object sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo(CraftingrecipesAssetName))
            {
                e.Edit(asset =>
                {
                    var data = asset.AsDictionary<string,string>().Data;
                    if (!ModConfig.DisableBaits)
                    {
                        AddRecipeData(data, BaitTackle.EverlastingBait, CraftingData.EverlastingBait);
                        AddRecipeData(data, BaitTackle.EverlastingWildBait, CraftingData.EverlastingWildBait);
                        AddRecipeData(data, BaitTackle.EverlastingMagnet, CraftingData.EverlastingMagnet);
                    }
                    if (!ModConfig.DisableTackles)
                    {
                        AddRecipeData(data, BaitTackle.UnbreakableSpinner, CraftingData.UnbreakableSpinner);
                        AddRecipeData(data, BaitTackle.UnbreakableLeadBobber, CraftingData.UnbreakableLeadBobber);
                        AddRecipeData(data, BaitTackle.UnbreakableTrapBobber, CraftingData.UnbreakableTrapBobber);
                        AddRecipeData(data, BaitTackle.UnbreakableCorkBobber, CraftingData.UnbreakableCorkBobber);
                        AddRecipeData(data, BaitTackle.UnbreakableTreasureHunter, CraftingData.UnbreakableTreasureHunter);
                        AddRecipeData(data, BaitTackle.UnbreakableBarbedHook, CraftingData.UnbreakableBarbedHook);
                        AddRecipeData(data, BaitTackle.UnbreakableDressedSpinner, CraftingData.UnbreakableDressedSpinner);
                    }
                });
            }
        }

        private string AddRecipeData(IDictionary<string, string> data, BaitTackle baitTackle, string recipe)
        {
            return data[baitTackle.GetDescription()] =  GetRecipeString(recipe, baitTackle);
        }

        private string GetRecipeString(string recipe, BaitTackle baitTackle)
        {
            var recipeString = $"{recipe}/Home/{(int)baitTackle} 1/false/null";
            if (LocalizedContentManager.CurrentLanguageCode != LocalizedContentManager.LanguageCode.en)
            {
                recipeString += "/" + I18N.Get($"{baitTackle.ToString()}.Name");
            }
            return recipeString;
        }

        private bool CheckNpcFriendship(string name, int friendshipHeartLevel)
        {
            if (friendshipHeartLevel > 8)
            {
                NPC npc = Game1.getCharacterFromName(name);
                if (npc.datable.Value)
                {
                    friendshipHeartLevel = 8;
                }
            }
            var npcFriendshipHeartLevel = Game1.player.getFriendshipHeartLevelForNPC(name);
            return npcFriendshipHeartLevel >= friendshipHeartLevel;
        }

        private void CreateConfigMenu(IManifest manifest)
        {
            GenericModConfigMenuApi api = Helper.ModRegistry.GetApi<GenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (api != null)
            {
                api.RegisterModConfig(manifest, () => DataLoader.ModConfig = new ModConfig(), () => Helper.WriteConfig(DataLoader.ModConfig));

                api.RegisterSimpleOption(manifest, "Disable Baits", "Disable all features related to everlasting baits. You won't receive letters about it, the crafting recipes won't show, and existing everlasting baits will be consumed as normal.", () => DataLoader.ModConfig.DisableBaits, (bool val) =>
                {
                    if(DataLoader.ModConfig.DisableBaits != val) DataLoader.Helper.GameContent.InvalidateCache(CraftingrecipesAssetName);
                    DataLoader.ModConfig.DisableBaits = val;
                });

                api.RegisterSimpleOption(manifest, "Disable Tackles", "Disable features related to everlasting baits. You won't receive letters starting quests, the crafting recipes won't show, and existing unbreakable tackles will wear out. Active quests will still show and be able to be ended if existing tackles.", () => DataLoader.ModConfig.DisableTackles, (bool val) =>
                {
                    if (DataLoader.ModConfig.DisableTackles != val) DataLoader.Helper.GameContent.InvalidateCache(CraftingrecipesAssetName);
                    DataLoader.ModConfig.DisableTackles = val;
                });

                api.RegisterLabel(manifest, "Iridium Quality Fish:", "Properties for iridium quality fish functionality added by this mod.");

                api.RegisterSimpleOption(manifest, "Disable", "You won't get any extra chance to catch iridium quality fish, nor you will receive Linus letter about it.", () => DataLoader.ModConfig.DisableIridiumQualityFish, (bool val) => DataLoader.ModConfig.DisableIridiumQualityFish = val);

                api.RegisterSimpleOption(manifest, "Only With Everlasting Baits", "You will only have an extra chance for iridium quality fish if using iridium quality baits.", () => DataLoader.ModConfig.IridiumQualityFishOnlyWithIridiumQualityBait, (bool val) => DataLoader.ModConfig.IridiumQualityFishOnlyWithIridiumQualityBait = val);

                api.RegisterSimpleOption(manifest, "Only With Wild Bait", "You will only have an extra chance to get iridium quality fish using wild bait.", () => DataLoader.ModConfig.IridiumQualityFishOnlyWithWildBait, (bool val) => DataLoader.ModConfig.IridiumQualityFishOnlyWithWildBait = val);

                api.RegisterSimpleOption(manifest, "Minimum Size", "Set a minimum size for the extra chance to get be iridium quality fish. It does not change the chance for it to be iridium quality(fishSize/2), it just enable the fish to be iridium quality after its bigger than the size chosen.", () => DataLoader.ModConfig.IridiumQualityFishMinimumSize, (float val) => DataLoader.ModConfig.IridiumQualityFishMinimumSize = val);
            }
        }
    }
}
