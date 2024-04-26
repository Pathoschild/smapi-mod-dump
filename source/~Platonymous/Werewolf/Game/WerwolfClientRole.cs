/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

using System.Collections.Generic;

namespace LandGrants.Game
{
    public class WerwolfClientRole
    {
        public string Name { get; set; }

        public string ID { get; set; }

        public List<WerwolfClientAction> Actions { get; set; } = new List<WerwolfClientAction>();

        public string Description { get; set; }

        public WerwolfClientRole()
        {

        }

        public WerwolfClientRole(string name, string iD, List<WerwolfClientAction> actions, string description)
        {
            Name = name;
            ID = iD;
            Actions = actions;
            Description = description;
        }
    }
}
