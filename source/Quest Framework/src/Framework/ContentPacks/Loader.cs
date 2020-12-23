/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/QuestFramework
**
*************************************************/

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using PurrplingCore.Lexing;
using PurrplingCore.Lexing.LexTokens;
using QuestFramework.Framework.ContentPacks.Model;
using QuestFramework.Framework.Helpers;
using QuestFramework.Offers;
using QuestFramework.Quests;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace QuestFramework.Framework.ContentPacks
{
    class Loader
    {
        private readonly Regex allowedChars = new Regex("^[a-zA-Z0-9_-]*$");

        public Loader(IMonitor monitor, QuestManager manager, QuestOfferManager scheduleManager)
        {
            this.Contents = new List<Content>();
            this.ValidContents = new List<Content>();
            this.Monitor = monitor;
            this.Manager = manager;
            this.ScheduleManager = scheduleManager;
        }

        public List<Content> Contents { get; }
        public List<Content> ValidContents { get; }
        public IMonitor Monitor { get; }
        public QuestManager Manager { get; }
        public QuestOfferManager ScheduleManager { get; }

        public void LoadPacks(IEnumerable<IContentPack> contentPacks)
        {
            foreach (var contentPack in contentPacks)
            {
                this.Monitor.Log($"Loading content pack {contentPack.Manifest.UniqueID} ...");

                var content = this.LoadContentPack(contentPack);

                if (content != null && this.ValidateContent(content))
                {
                    this.ValidContents.Add(content);
                }
            }

            if (this.ValidContents.Count > 0)
            {
                this.PrintContentPackListSummary();
            }
        }

        private void PrintContentPackListSummary()
        {
            this.Monitor.Log($"Loaded {this.ValidContents.Count} content packs:", LogLevel.Info);
            this.ValidContents.Select(c => c.Owner.Manifest)
                .ToList()
                .ForEach(
                    (m) => this.Monitor.Log($"   {m.Name} {m.Version} by {m.Author} ({m.UniqueID})", LogLevel.Info));
        }

        private void Prepare(Content content)
        {
            foreach (var schedule in content.Offers)
            {
                if (!schedule.QuestName.Contains('@'))
                {
                    schedule.QuestName = $"{schedule.QuestName}@{content.Owner.Manifest.UniqueID}";
                }
            }
        }

        public bool ValidateContent(Content content)
        {
            bool isValid = true;

            if (content.Format == null || content.Format.IsOlderThan("1.0"))
            {
                this.Monitor.Log($"Content pack `{content.Owner.Manifest.Name}` has unsupported format version {content.Format}.", LogLevel.Error);
                isValid = false;
            }

            if (content.Quests == null || !content.Quests.Any())
            {
                this.Monitor.Log($"Content pack `{content.Owner.Manifest.Name}` contains no quests.", LogLevel.Error);
                isValid = false;
            }

            return isValid;
        }

        private void Apply(Content content)
        {
            // Register quests
            foreach (var questData in content.Quests)
            {
                try
                {
                    CustomQuest managedQuest = this.MapQuest(content, questData);

                    if (questData.Hooks != null)
                    {
                        managedQuest.Hooks.AddRange(questData.Hooks);
                    }

                    if (!this.allowedChars.IsMatch(managedQuest.Name))
                    {
                        this.Monitor.Log($"Quest name `{managedQuest.Name}` contains unallowed characters in pack `{content.Owner.Manifest.UniqueID}`", LogLevel.Error);
                        return;
                    }

                    this.ApplyHandlers(managedQuest, questData);
                    this.Manager.RegisterQuest(managedQuest);
                } 
                catch (InvalidQuestException ex)
                {
                    this.Monitor.Log($"Error while creating quest `{questData.Name}` from pack `{content.Owner.Manifest.UniqueID}`: {ex.Message}", LogLevel.Error);
                }
            }

            // Add quest schedules
            foreach (var offer in content.Offers)
            {
                this.ScheduleManager.AddOffer(offer);
            }
        }

        private void ApplyHandlers(CustomQuest managedQuest, QuestData questData)
        {
            managedQuest.Accepted += (_sender, _info) =>
            {
                if (_info.VanillaQuest.completed.Value)
                    return;

                // Add/Remove conversation topic(s) when quest was ACCEPTED
                if (!string.IsNullOrEmpty(questData.ConversationTopic?.AddWhenQuestAccepted))
                    ConversationTopicHelper.AddConversationTopic(questData.ConversationTopic.AddWhenQuestAccepted);
                if (!string.IsNullOrEmpty(questData.ConversationTopic?.RemoveWhenQuestAccepted))
                    ConversationTopicHelper.RemoveConversationTopic(questData.ConversationTopic.RemoveWhenQuestAccepted);
            };
            managedQuest.Completed += (_sender, _info) =>
            {
                if (!_info.VanillaQuest.completed.Value)
                    return;

                // Add/Remove conversation topic(s) when quest was COMPLETED
                if (!string.IsNullOrEmpty(questData.ConversationTopic?.AddWhenQuestCompleted))
                    ConversationTopicHelper.AddConversationTopic(questData.ConversationTopic.AddWhenQuestCompleted);
                if (!string.IsNullOrEmpty(questData.ConversationTopic?.RemoveWhenQuestCompleted))
                    ConversationTopicHelper.RemoveConversationTopic(questData.ConversationTopic.RemoveWhenQuestCompleted);
            };
            managedQuest.Removed += (_sender, _info) =>
            {
                if (_info.VanillaQuest.completed.Value)
                    return;

                // Add/Remove conversation topic(s) when quest was REMOVED
                if (!string.IsNullOrEmpty(questData.ConversationTopic?.AddWhenQuestRemoved))
                    ConversationTopicHelper.AddConversationTopic(questData.ConversationTopic.AddWhenQuestRemoved);
                if (!string.IsNullOrEmpty(questData.ConversationTopic?.RemoveWhenQuestRemoved))
                    ConversationTopicHelper.RemoveConversationTopic(questData.ConversationTopic.RemoveWhenQuestRemoved);
            };
        }

        public void RegisterQuestsFromPacks()
        {
            this.ValidContents.ForEach(content => this.Apply(content.Translate(content.Owner.Translation)));

        }

        private int GetJsonAssetId(string name, string type)
        {
            if (QuestFrameworkMod.Instance.Bridge.JsonAssets == null)
            {
                this.Monitor.Log("JsonAssets mod is not installed! To use JsonAssets items install it from https://www.nexusmods.com/stardewvalley/mods/1720", LogLevel.Error);
                return -1;
            }

            switch (type.ToLower())
            {
                case "object":
                    return QuestFrameworkMod.Instance.Bridge.JsonAssets.GetObjectId(name);
                case "bigcraftable":
                    return QuestFrameworkMod.Instance.Bridge.JsonAssets.GetBigCraftableId(name);
                default:
                    this.Monitor.Log($"Unsupported JsonAssets type by Quest Framework: {type}", LogLevel.Error);
                    return -1;
            }
        }

        private string TranspileToken(string name, string value)
        {
            switch (name)
            {
                case "ja":
                    string[] s = value.Split('|');
                    string type = s.Length < 2 ? "object" : s[1].Trim();
                    int id = this.GetJsonAssetId(s[0].Trim(), type);

                    if (id == -1)
                        this.Monitor.Log($"JsonAssets: Unknown item name `{s[0]}` of type `{type}`", LogLevel.Error);

                    return id.ToString();

            }

            return "{{" + name + ":" + value + "}}";
        }

        private string ApplyTokens(string rawStr)
        {
            var lexer = new Lexer();
            List<string> parts = new List<string>();

            if (rawStr == null)
                return null;

            foreach (var bit in lexer.ParseBits(rawStr, false))
            {
                if (bit.Type == LexTokenType.Literal)
                {
                    parts.Add(bit.ToString());
                    continue;
                }

                if (bit.Type == LexTokenType.Token && bit is LexTokenToken token)
                {
                    parts.Add(this.TranspileToken(token.Name, token.InputArgs.ToString()));
                    continue;
                }

                parts.Add(bit.ToString());
            }

            return string.Join("", parts);
        }

        private QuestType ParseBaseQuestType(QuestData questData, Content content)
        {
            if (questData.Type.Contains('/'))
                return QuestType.Custom;

            if (Enum.TryParse(questData.Type, out QuestType parsedType))
                return parsedType;

            this.Monitor.Log($"Invalid quest type `{questData.Type}` for `{questData.Name}` in pack `{content.Owner.Manifest.UniqueID}`", LogLevel.Error);

            return QuestType.Basic;
        }

        private CustomQuest CreateQuest(string type)
        {
            return type.Contains('/')
                ? this.Manager.CreateQuestOfType(type)
                : new CustomQuest();
        }

        private CustomQuest MapQuest(Content content, QuestData questData)
        {
            string trigger = questData.Trigger?.ToString();
            var managedQuest = this.CreateQuest(questData.Type);

            managedQuest.Name = questData.Name;
            managedQuest.BaseType = this.ParseBaseQuestType(questData, content);
            managedQuest.Title = questData.Title;
            managedQuest.Description = questData.Description;
            managedQuest.Objective = questData.Objective;
            managedQuest.DaysLeft = questData.DaysLeft;
            managedQuest.Reward = this.ParseReward(questData.Reward, questData.RewardType);
            managedQuest.RewardType = questData.RewardType;
            managedQuest.RewardAmount = questData.RewardAmount;
            managedQuest.RewardDescription = questData.RewardDescription;
            managedQuest.ReactionText = questData.ReactionText;
            managedQuest.Cancelable = questData.Cancelable;
            managedQuest.Trigger = this.ApplyTokens(trigger);
            managedQuest.NextQuests = questData.NextQuests;
            managedQuest.Colors = questData.Colors;
            managedQuest.OwnedByModUid = content.Owner.Manifest.UniqueID;

            if (questData.CustomTypeId != -1)
            {
                managedQuest.CustomTypeId = questData.CustomTypeId;
            }

            if (questData.FriendshipGain != null)
            {
                foreach (var fship in questData.FriendshipGain)
                {
                    managedQuest.FriendshipGain[fship.Key] = fship.Value;
                }
            }

            if (questData.Tags != null)
            {
                foreach (var tag in questData.Tags)
                {
                    managedQuest.Tags[tag.Key] = tag.Value;
                }
            }

            if (!string.IsNullOrEmpty(questData.Texture))
            {
                try
                {
                    managedQuest.Texture = content.Owner.LoadAsset<Texture2D>(questData.Texture);
                } 
                catch (ContentLoadException ex)
                {
                    this.Monitor.Log($"Couldn't load quest background texture file `{questData.Texture}`: {ex.Message}");
                }
            }

            questData.PopulateExtendedData(managedQuest);

            return managedQuest;
        }

        private int ParseReward(JToken reward, RewardType rewardType)
        {
            if (reward != null && reward.Type != JTokenType.Null)
            {
                if (reward.Type == JTokenType.Integer)
                {
                    return reward.ToObject<int>();
                }

                int id;
                string rewardName = reward.ToObject<string>();
                switch (rewardType)
                {
                    case RewardType.Money:
                        return reward.ToObject<int>();
                    case RewardType.Object:
                        id = ItemHelper.GetObjectId(rewardName);

                        if (id == -1)
                        {
                            this.Monitor.Log($"Unknown object `{rewardName}` for quest reward.", LogLevel.Error);
                        }

                        return id;
                    case RewardType.Weapon:
                        id = ItemHelper.GetWeaponId(rewardName);

                        if (id == -1)
                        {
                            this.Monitor.Log($"Unknown weapon `{rewardName}` for quest reward.", LogLevel.Error);
                        }

                        return id;
                }
            }

            return 0;
        }

        public Content LoadContentPack(IContentPack contentPack)
        {
            if (!contentPack.HasFile("quests.json"))
            {
                this.Monitor.Log($"Content pack `{contentPack.Manifest.Name}` has no entry file `quests.json`", LogLevel.Error);
                return null;
            }

            var content = contentPack.ReadJsonFile<Content>("quests.json");

            content.Owner = contentPack;
            this.Prepare(content);

            return content;
        }
    }
}
