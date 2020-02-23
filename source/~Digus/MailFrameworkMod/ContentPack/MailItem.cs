using System.Collections.Generic;

namespace MailFrameworkMod.ContentPack
{
    public class MailItem
    {
        public string Id;
        public string GroupId;
        public string Title;
        public string Text;
        public string Recipe;
        public List<Attachment> Attachments;
        public string LetterBG;
        public int WhichBG;
        public int? TextColor;
        public string UpperRightCloseButton;
        public bool Repeatable;
        public string Date;
        public List<int> Days;
        public List<string> Seasons;
        public string Weather;
        public List<FriendshipCondition> FriendshipConditions;
        public List<SkillCondition> SkillConditions;
        public List<StatsCondition> StatsConditions;
        public List<CollectionCondition> CollectionConditions;
        public double? RandomChance;
        public List<string> Buildings;
        public bool RequireAllBuildings;
        public List<string> MailReceived;
        public bool RequireAllMailReceived;
        public List<string> MailNotReceived;
        public List<int> EventsSeen;
        public bool RequireAllEventsSeen;
        public List<int> EventsNotSeen;
        public List<string> RecipeKnown;
        public bool RequireAllRecipeKnown;
        public List<string> RecipeNotKnown;
    }
}
