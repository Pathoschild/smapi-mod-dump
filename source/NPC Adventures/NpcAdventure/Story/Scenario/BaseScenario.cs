namespace NpcAdventure.Story.Scenario
{
    public abstract class BaseScenario : IScenario
    {
        public IGameMaster GameMaster { get; set; }
        public StoryHelper StoryHelper { get => (this.GameMaster as GameMaster)?.StoryHelper; }

        public abstract void Dispose();
        public abstract void Initialize();
    }
}
