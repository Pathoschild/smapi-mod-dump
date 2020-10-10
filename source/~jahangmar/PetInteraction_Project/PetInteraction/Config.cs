/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jahangmar/StardewValleyMods
**
*************************************************/

using System;
namespace PetInteraction
{
    public class Config
    {
        //public int catch_up_distance { set; get; } = 3;
        public int pet_speed { set; get; } = 6;
        public int pet_fast_speed { set; get; } = 10;
        public int pet_friendship_decrease_onhit = 20;
        public int pet_fetch_friendship_chance = 40;
        public int pet_fetch_friendship_increase = 10;
        public int pet_petting_friendship_increase = 12;
        public float stick_range = 12;
        public bool show_message_on_warp = true;
        public bool unconditional_love = false;
        public bool love_everytime_at_max_friendship = false;
    }
}
