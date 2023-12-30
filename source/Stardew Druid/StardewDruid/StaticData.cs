/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using StardewValley;
using System.Collections.Generic;

namespace StardewDruid
{
    class StaticData
    {

        public int staticVersion;

        public long staticId;

        public string activeBlessing;

        public Dictionary<string, bool> questList;

        public Dictionary<int, string> weaponAttunement;

        public Dictionary<string, int> taskList;

        public Dictionary<string, string> characterList;

        public int setProgress;

        public int activeProgress;

        public StaticData()
        {

            staticId = Game1.player.UniqueMultiplayerID;

            activeBlessing = "none";

            questList = new();

            weaponAttunement = new();

            taskList = new();

            characterList = new();

            setProgress = -1;

            activeProgress = 0;

        }

    }

}
