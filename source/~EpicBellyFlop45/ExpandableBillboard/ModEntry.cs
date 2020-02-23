using ExpandableBillboard.Models;
using ExpandableBillboard.Patches;
using ExpandableBillboard.Ui;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Quests;
using System;
using System.Collections.Generic;
using System.IO;
using static ExpandableBillboard.Enums;

namespace ExpandableBillboard
{
    public class ModEntry : Mod
    {
        /// <summary>Provides methods to interact with the mod directory.</summary>
        public static IModHelper ModHelper { get; private set; }

        /// <summary>The quests that all the user installed mods contain.</summary>
        public static List<BillBoardQuest> AddedQuests { get; set; } = new List<BillBoardQuest>();
        public static List<Quest> CurrentBillBoardQuests = new List<Quest>();

        /// <summary>Mod entry point.</summary>
        public override void Entry(IModHelper helper)
        {
            ModHelper = this.Helper;

            VerifyAssets();
            LoadContentPacks();
            this.Monitor.Log($"{AddedQuests.Count.ToString()} quests have been added", LogLevel.Trace);
            MethodPatches.ApplyHarmonyPatches(this.ModManifest.UniqueID);

            this.Helper.Events.Display.MenuChanged += OnMenuChanged;
            this.Helper.Events.GameLoop.DayStarted += OnDayStarted;
            this.Helper.Events.Input.ButtonPressed += OnButtonPressed;
        }

        /// <summary>Ensure all assets exist in assets folder.</summary>
        private void VerifyAssets()
        {
            string assetsPath = Path.Combine(this.Helper.DirectoryPath, "Assets");

            string closeButtonPath = Path.Combine(assetsPath, "CloseButton.png");
            if (File.Exists(closeButtonPath))
            {
                this.Monitor.Log("CloseButton.png found.", LogLevel.Trace);
            }
            else
            {
                this.Monitor.Log($"CloseButton.png couldn't be found. Path searched: {closeButtonPath}", LogLevel.Error);
            }

            string acceptButtonPath = Path.Combine(assetsPath, "AcceptButton.png");
            if (File.Exists(acceptButtonPath))
            {
                this.Monitor.Log("AcceptButton.png found.", LogLevel.Trace);
            }
            else
            {
                this.Monitor.Log($"AcceptButton.png couldn't be found. Path searched: {acceptButtonPath}", LogLevel.Error);
            }

            string billboardBackgroundPath = Path.Combine(assetsPath, "BillboardBackground.png");
            if (File.Exists(billboardBackgroundPath))
            {
                this.Monitor.Log("BillboardBackground.png found.", LogLevel.Trace);
            }
            else
            {
                this.Monitor.Log($"BillboardBackground.png couldn't be found. Path seached: {billboardBackgroundPath}", LogLevel.Error);
            }

            string notePath = Path.Combine(assetsPath, "Note.png");
            if (File.Exists(notePath))
            {
                this.Monitor.Log("Note.png found.", LogLevel.Trace);
            }
            else
            {
                this.Monitor.Log($"Note.png coundn't be found. Path searched: {notePath}", LogLevel.Error);
            }

            string NoteBackgroundPath = Path.Combine(assetsPath, "NoteBackground.png");
            if (File.Exists(NoteBackgroundPath))
            {
                this.Monitor.Log("NoteBackground.png found.", LogLevel.Trace);
            }
            else
            {
                this.Monitor.Log($"NoteBackground.png coundn't be found. Path searched: {notePath}", LogLevel.Error);
            }
        }

        /// <summary>Get all the content packs the user has installed that belong to this mod.</summary>
        private void LoadContentPacks()
        {
            foreach (IContentPack contentPack in this.Helper.ContentPacks.GetOwned())
            {
                if (!contentPack.HasFile("content.json"))
                {
                    this.Monitor.Log($"Content pack: {contentPack.Manifest.Name} is missing the content.json file", LogLevel.Error);
                    continue;
                }

                AddedQuests = contentPack.ReadJsonFile<List<BillBoardQuest>>("content.json");
            }
        }

        /// <summary>The method invoked when the player opens/closes a displayed menu. Used to replace the old billboard menu with the new one.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (e.NewMenu is Billboard)
            {
                Game1.activeClickableMenu = null;
                Game1.activeClickableMenu = new BetterBillboardMenu();
            }
        }

        /// <summary>The method invoken when the player startes a new day. Used for adding new billboard quests.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            // ensure there is space for a new quest
            if (CurrentBillBoardQuests.Count == 8)
            {
                return;
            }

            // use rng to decide if user should get another quest
            double chance = Game1.random.NextDouble();
            if (chance < .4)
            {
                // decide which type of quest to get
                string[] questTypes = Enum.GetNames(typeof(QuestType));
                string questType = questTypes[Game1.random.Next(questTypes.Length)];

                switch (questType)
                {
                    case "SlayMonsters":
                        {
                            //Quest quest = new SlayMonsterQuest();
                            //CurrentBillBoardQuests.Add(quest);
                            this.Monitor.Log("New SlayMonsterQuest has been added to the billboard", LogLevel.Trace);

                            break;
                        }
                    case "Fishing":
                        {
                            //Quest quest = new FishingQuest();
                            //CurrentBillBoardQuests.Add(quest);
                            this.Monitor.Log("New FishingQuest has been added to the billboard", LogLevel.Trace);

                            break;
                        }
                    case "ItemDelivery":
                        {
                            Quest quest = new ItemDeliveryQuest();
                            CurrentBillBoardQuests.Add(quest);
                            this.Monitor.Log("New ItemDeliveryQuest has been added to the billboard", LogLevel.Trace);

                            break;
                        }
                }
            }
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button == SButton.G)
            {
                if (Game1.activeClickableMenu == null)
                {
                    Game1.activeClickableMenu = new BetterBillboardMenu();
                }
                else if (Game1.activeClickableMenu is BetterBillboardMenu)
                {
                    Game1.activeClickableMenu = null;
                }
            }
            else if (e.Button == SButton.H)
            {
                if (Game1.activeClickableMenu == null)
                {
                    Quest quest = new ItemDeliveryQuest();
                    CurrentBillBoardQuests.Add(quest);
                    Game1.activeClickableMenu = new NoteMenu(AddedQuests[0], 0);
                }
                else if (Game1.activeClickableMenu is NoteMenu)
                {
                    Game1.activeClickableMenu = null;
                }
            }
        }

        /// <summary>Turn text tags ({FARMER} & {REQUESTER}) into their constant values.</summary>
        /// <param name="quest">The quest object that contains the description, objective, and title.</param>
        /// <returns>An updated quest model.</returns>
        public static BillBoardQuest ResolveQuestTextTags(BillBoardQuest quest)
        {
            quest.Title = quest.Title.Replace("{FARMER}", Game1.player.Name);
            quest.Title = quest.Title.Replace("{REQUESTER}", quest.Requester);
            quest.Description = quest.Description.Replace("{FARMER}", Game1.player.Name);
            quest.Description = quest.Description.Replace("{REQUESTER}", quest.Requester);
            quest.Objective = quest.Objective.Replace("{FARMER}", Game1.player.Name);
            quest.Objective = quest.Objective.Replace("{REQUESTER}", quest.Requester);

            return quest;
        }

        /// <summary>Adds the requester sign off and rewards to the quest description</summary>
        /// <param name="quest">The quest object that contains the rewards, description, and requester</param>
        /// <returns>A constructed description string</returns>
        public static string ConstructDescriptionString(BillBoardQuest quest)
        {
            string questDescription = $"{quest.Description}\n\n";
            
            if (quest.FriendshipReward > 0)
            {
                questDescription += $"- {quest.Requester} will be thankful\n";
            }

            if (quest.MoneyReward > 0)
            {
                questDescription += $"- {quest.MoneyReward}g on delivery";
            }

            return questDescription;
        }
    }
}
