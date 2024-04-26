/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using Force.DeepCloner;
using StardewDruid.Cast;
using StardewDruid.Character;
using StardewDruid.Journal;
using StardewValley;
using System;
using System.Collections.Generic;

namespace StardewDruid.Data
{
    class StaticData
    {

        public static int version;

        public long id;

        public Rite.rites rite;

        public Dictionary<string, QuestProgress> progress;

        public Dictionary<string, Character.Character.mode> characters;

        public Dictionary<int,Rite.rites> attunement;

        public QuestHandle.milestones milestone;

        public StaticData()
        {

            version = 220;
            id = Game1.player.UniqueMultiplayerID;
            rite = Rite.rites.none;
            progress = new();
            characters = new();
            attunement = new();
            milestone = QuestHandle.milestones.none;

        }

    }

}
