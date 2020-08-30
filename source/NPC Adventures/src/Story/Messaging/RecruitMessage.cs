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
