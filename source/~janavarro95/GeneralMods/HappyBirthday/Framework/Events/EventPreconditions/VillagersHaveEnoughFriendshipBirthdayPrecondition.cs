/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omegasis.StardustCore.Events.Preconditions;
using StardewValley;

namespace Omegasis.HappyBirthday.Framework.Events.EventPreconditions
{
    public class VillagersHaveEnoughFriendshipBirthdayPrecondition: EventPrecondition
    {

        public const string EventPreconditionId = "Omegasis.HappyBirthday.Framework.EventPreconditions.VillagersHaveEnoughFriendshipBirthdayPrecondition";

        public VillagersHaveEnoughFriendshipBirthdayPrecondition()
        {

        }

        public override string ToString()
        {
            return EventPreconditionId;
        }

        public override bool meetsCondition()
        {

            List<NPC> npcs = new List<NPC>()
            {
                Game1.getCharacterFromName("Lewis"),

                Game1.getCharacterFromName("Gus"),
                Game1.getCharacterFromName("Emily"),
                Game1.getCharacterFromName("Sandy"),

                Game1.getCharacterFromName("Alex"),
                Game1.getCharacterFromName("George"),
                Game1.getCharacterFromName("Evelyn"),

                Game1.getCharacterFromName("Harvey"),

                Game1.getCharacterFromName("Marnie"),
                Game1.getCharacterFromName("Shane"),
                Game1.getCharacterFromName("Jas"),

                Game1.getCharacterFromName("Pierre"),
                Game1.getCharacterFromName("Caroline"),

                Game1.getCharacterFromName("Penny"),
                Game1.getCharacterFromName("Pam"),

                Game1.getCharacterFromName("Abigail"),
                Game1.getCharacterFromName("Sebastian"),

                Game1.getCharacterFromName("Sam"),

                Game1.getCharacterFromName("Haley"),

                Game1.getCharacterFromName("Elliott"),
                Game1.getCharacterFromName("Leah"),

                Game1.getCharacterFromName("Robin"),
                Game1.getCharacterFromName("Demetrius"),
                Game1.getCharacterFromName("Maru"),

                Game1.getCharacterFromName("Linus"),

                Game1.getCharacterFromName("Clint"),

                Game1.getCharacterFromName("Vincent"),
                Game1.getCharacterFromName("Jodi"),

                Game1.getCharacterFromName("Willy"),

                Game1.getCharacterFromName("Wizard")
            };

            if (Game1.year >= 2)
            {
                npcs.Add(Game1.getCharacterFromName("Kent"));
            }

            foreach (NPC npc in npcs)
            {
               int heartLevel= Game1.player.getFriendshipHeartLevelForNPC(npc.Name);
                if (heartLevel < HappyBirthdayModCore.Configs.modConfig.minimumFriendshipLevelForCommunityBirthdayParty) return false;
            }

            return true;
        }

    }
}
