/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/StardewMods
**
*************************************************/

using Newtonsoft.Json;
using QuestFramework.Quests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestEssentials.Messages
{
    public class StoryMessage : ICompletionMessage, IStoryMessage
    {
        public StoryMessage(string name, string trigger)
        {
            this.Trigger = trigger;
            this.Name = name;
        }

        public StoryMessage(string trigger)
        {
            this.Trigger = trigger;
            this.Name = trigger;
        }

        public string Trigger { get; }

        public string Name { get; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
