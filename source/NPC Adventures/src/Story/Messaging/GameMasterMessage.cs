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
    abstract class GameMasterMessage : IGameMasterMessage
    {
        protected GameMasterMessage(string name)
        {
            this.Name = name;
        }

        public string Name { get; private set; }
    }
}
