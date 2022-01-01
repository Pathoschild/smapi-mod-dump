/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/

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
        public List<string> AdditionalMailReceived;
        public string LetterBG;
        public int WhichBG;
        public int? TextColor;
        public string UpperRightCloseButton;
        public bool Repeatable;
        public string Date;
        public List<int> Days;
        public List<string> Seasons;
        public string Weather;
        public int? HouseUpgradeLevel;
        public List<FriendshipCondition> FriendshipConditions;
        public List<SkillCondition> SkillConditions;
        public List<StatsCondition> StatsConditions;
        public List<CollectionCondition> CollectionConditions;
        public string ExpandedPrecondition;
        public string[] ExpandedPreconditions;
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
        public bool AutoOpen;
    }
}
