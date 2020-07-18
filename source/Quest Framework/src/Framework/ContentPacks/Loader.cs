using PurrplingCore.Lexing;
using PurrplingCore.Lexing.LexTokens;
using QuestFramework.Framework.ContentPacks.Model;
using QuestFramework.Quests;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestFramework.Framework.ContentPacks
{
    class Loader
    {
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
            int count = 0;
            foreach (var contentPack in contentPacks)
            {
                var content = this.LoadContentPack(contentPack);

                if (content != null && this.Validate(content))
                {
                    this.ValidContents.Add(content);
                    ++count;
                }
            }

            this.Monitor.Log($"Loaded {count} content packs.", LogLevel.Info); 
        }

        private void Prepare(Content content)
        {
            foreach (var schedule in content.Offers)
            {
                if (!schedule.QuestName.Contains('@'))
                {
                    schedule.QuestName = $"{schedule.QuestName}@{content.owner.Manifest.UniqueID}";
                }
            }
        }

        private bool Validate(Content content)
        {
            bool isValid = true;

            if (content.Format == null || content.Format.IsOlderThan("1.0"))
            {
                this.Monitor.Log($"Content pack `{content.owner.Manifest.Name}` has unsupported format version {content.Format}.", LogLevel.Error);
                isValid = false;
            }

            if (content.Quests == null || !content.Quests.Any())
            {
                this.Monitor.Log($"Content pack `{content.owner.Manifest.Name}` contains no quests.", LogLevel.Error);
                isValid = false;
            }

            return isValid;
        }

        private void Apply(Content content)
        {
            // Register quests
            foreach (var quest in content.Quests)
            {
                CustomQuest managedQuest = this.MapQuest(content, quest);

                if (quest.Hooks != null)
                {
                    managedQuest.Hooks.AddRange(quest.Hooks);
                }

                this.Manager.RegisterQuest(managedQuest);
            }

            // Add quest schedules
            foreach (var offer in content.Offers)
            {
                this.ScheduleManager.AddOffer(offer);
            }
        }

        public void RegisterQuestsFromPacks()
        {
            this.ValidContents.ForEach(content => this.Apply(content));

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

        private CustomQuest MapQuest(Content content, Quest quest)
        {
            string trigger = quest.Trigger?.ToString();

            var managedQuest = new CustomQuest(quest.Name)
            {
                Title = quest.Title,
                Description = quest.Description,
                BaseType = quest.Type,
                Objective = quest.Objective,
                DaysLeft = quest.DaysLeft,
                Reward = quest.Reward,
                RewardDescription = quest.RewardDescription,
                ReactionText = quest.ReactionText,
                Cancelable = quest.Cancelable,
                Trigger = this.ApplyTokens(trigger),
                NextQuests = quest.NextQuests,
                OwnedByModUid = content.owner.Manifest.UniqueID,
            };

            if (quest.CustomTypeId != -1)
            {
                managedQuest.CustomTypeId = quest.CustomTypeId;
            }

            return managedQuest;
        }

        private Content LoadContentPack(IContentPack contentPack)
        {
            if (!contentPack.HasFile("quests.json"))
            {
                this.Monitor.Log($"Content pack `{contentPack.Manifest.Name}` has no entry file `quests.json`", LogLevel.Error);
                return null;
            }

            var content = contentPack.ReadJsonFile<Content>("quests.json");

            content.owner = contentPack;
            this.Prepare(content);

            return content;
        }
    }
}
