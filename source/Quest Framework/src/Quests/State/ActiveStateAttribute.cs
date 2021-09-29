/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/QuestFramework
**
*************************************************/

using System;

namespace QuestFramework.Quests.State
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class ActiveStateAttribute : Attribute
    {
        public string Name { get; }

        public ActiveStateAttribute()
        {
        }

        public ActiveStateAttribute(string name)
        {
            this.Name = name;
        }
    }
}