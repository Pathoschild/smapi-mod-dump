/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using System;
using System.Collections.Generic;

namespace StardewDruid.Map
{

    public class Quest
    {

        public string name;

        public string type;

        public List<string> triggerLocation;

        public int startTime;

        public string triggerBlessing;

        public Microsoft.Xna.Framework.Vector2 triggerVector;

        public List<Type> triggerLocale;

        public string triggerMarker;

        public bool triggerAnywhere;

        public string triggerCompanion;

        public Microsoft.Xna.Framework.Color triggerColour = Microsoft.Xna.Framework.Color.White;

        public int questId;

        //public string questCharacter;

        public int questValue;

        public string questTitle;

        public string questDescription;

        public string questObjective;

        public int questReward;

        public int taskCounter;

        public string taskFinish;

        public int questProgress;

        public string journalSection;

        public string journalTitle;

        public string journalQuestion;

        public string journalAnswer;

        public string questDiscuss;


    }
}
