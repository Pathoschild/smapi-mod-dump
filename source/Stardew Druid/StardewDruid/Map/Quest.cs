/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace StardewDruid.Map
{
    public class Quest
    {

        public string name;

        public string triggerType;

        public string markerType;

        public List<string> triggerLocation;

        public int startTime;

        public string triggerBlessing;

        public int triggerTile;

        public string triggerAction;

        public bool triggerAnywhere;

        public string triggerMilestone;

        public bool triggerSpecial;

        public Dictionary<string, Vector2> vectorList = new();

        public List<Type> triggerLocale;

        public int triggerRadius;

        public int questId;

        public int questValue;

        public string questTitle;

        public string questDescription;

        public string questObjective;

        public int questReward;

        public int questLevel;

        public int taskCounter;

        public string taskFinish;

        public bool useTarget;

    }
}
