using System.Collections.Generic;

namespace MailFrameworkMod.ContentPack
{
    public class MailItem
    {
        public string Id;
        public string Text;
        public string Recipe;
        public List<Attachment> Attachments;
        public int WhichBG;
        public int? TextColor;
        public bool Repeatable;
        public string Date;
        public List<int> Days;
        public List<string> Seasons;
        public string Weather;
        public List<FriendshipCondition> FriendshipConditions;
        public List<SkillCondition> SkillConditions;
    }
}
