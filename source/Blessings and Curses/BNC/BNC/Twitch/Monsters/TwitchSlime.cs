using BNC.Twitch;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Monsters;

namespace BNC
{
    class TwitchSlime : GreenSlime, ITwitchMonster
    {

        public TwitchSlime(Vector2 position, int currentMineLevel) : base(position, currentMineLevel) { }

        public string TwitchName { get; set; } = "null";

        public string GetTwitchName()
        {
            return TwitchName;
        }

        //Farmer will still take damage even with a ring.. but wont get slimed
        public override void collisionWithFarmerBehavior()
        {
            if (Game1.random.NextDouble() < 0.3 && !this.Player.temporarilyInvincible && (!this.Player.isWearingRing(520) && Game1.buffsDisplay.addOtherBuff(new Buff(13))))
                this.currentLocation.playSound("slime");

            this.farmerPassesThrough = false;
        }

        public void setTwitchName(string username)
        {
            TwitchName = username;
        }
    }
}
