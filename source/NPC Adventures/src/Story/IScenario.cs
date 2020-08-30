namespace NpcAdventure.Story
{
    public interface IScenario
    {
        IGameMaster GameMaster { get; set; }
        void Initialize();
        void Dispose();
    }

    public interface IEventScenario : IScenario
    {
        bool CheckForEvent();
    }
}
