namespace NpcAdventure.Story.Messaging
{
    abstract class GameMasterMessage : IGameMasterMessage
    {
        protected GameMasterMessage(string name)
        {
            this.Name = name;
        }

        public string Name { get; private set; }
    }
}
