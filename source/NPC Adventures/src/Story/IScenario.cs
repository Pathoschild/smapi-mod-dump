/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/PurrplingMod
**
*************************************************/

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
