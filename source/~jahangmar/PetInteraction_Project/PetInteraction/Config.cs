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
        public bool show_message_on_warp = true;
        public bool unconditional_love = false;
    }
}
