/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

namespace Werewolf.Game
{
    public class WerwolfClientAction
    {
        public string Name {get; set;}

        public string Description { get; set; }

        public bool Active { get; set; } = true;

        public string ID { get; set; }

        public WerwolfClientPlayer Target { get; set; }

        public WerwolfClientAction()
        {

        }

        public WerwolfClientAction(string name, string description, bool active, string iD)
        {
            Name = name;
            Description = description;
            Active = active;
            ID = iD;
        }

        public void SetTarget(WerwolfClientPlayer player)
        {
            Target = player;
        }
    }
}
