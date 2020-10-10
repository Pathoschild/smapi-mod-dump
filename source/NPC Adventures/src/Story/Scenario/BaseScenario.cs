/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/PurrplingMod
**
*************************************************/

namespace NpcAdventure.Story.Scenario
{
    public abstract class BaseScenario : IScenario
    {
        public IGameMaster GameMaster { get; set; }
        public StoryHelper StoryHelper { get => (this.GameMaster as GameMaster)?.StoryHelper; }

        public abstract void Dispose();
        public abstract void Initialize();

        internal GameMaster GetGameMaster()
        {
            return this.GameMaster as GameMaster;
        }
    }
}
