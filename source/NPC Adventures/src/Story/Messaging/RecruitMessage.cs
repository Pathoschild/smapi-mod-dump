/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/PurrplingMod
**
*************************************************/

namespace NpcAdventure.Story.Messaging
{
    class RecruitMessage : GameMasterMessage
    {
        public RecruitMessage() : base("recruit")
        {

        }

        public RecruitMessage(string companionName) : this()
        {
            this.CompanionName = companionName;
        }

        public string CompanionName { get; set; }
    }
}
