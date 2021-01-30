/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/GenDeathrow/SDV_BlessingsAndCurses
**
*************************************************/

using BNC.Twitch;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Monsters;
using System.Xml.Serialization;

namespace BNC
{
    class TwitchBigSlime : BigSlime, ITwitchMonster
    {

        public TwitchBigSlime() { }

        public TwitchBigSlime(Vector2 position) : base(position, 0) {
            this.Health = health / 2;
            this.MaxHealth = maxHealth / 2;
        }


        //Farmer will still take damage even with a ring.. but wont get slimed
        public override void collisionWithFarmerBehavior()
        {
            if (Game1.random.NextDouble() < 0.3 && !this.Player.temporarilyInvincible && (!this.Player.isWearingRing(520) && Game1.buffsDisplay.addOtherBuff(new Buff(13))))
                this.currentLocation.playSound("slime");

            this.farmerPassesThrough = false;
        }

        [XmlIgnore]
        public string TwitchName { get; set; } = "null";

        public string GetTwitchName()
        {
            return TwitchName;
        }

        public void setTwitchName(string username)
        {
            TwitchName = username;
            this.Name = username;
            this.displayName = username;
        }
    }
}
