using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;

namespace Revitalize.Framework.Crafting
{
    public class StatCost
    {
        int health;
        int stamina;
        int magic;
        int gold;

        public StatCost(int Stamina = 0, int Health = 0, int Gold = 0, int Magic = 0){
            this.stamina = Stamina;
            this.health = Health;
            this.gold = Gold;
            this.magic = Magic;
        }

        /// <summary>
        /// Checks if the player can afford the cost but allows for player to collapse.
        /// </summary>
        /// <returns></returns>
        public bool canAffordCost() {

            if (Game1.player.stamina >= this.stamina && Game1.player.health >= this.health && Game1.player.Money >= this.gold && Revitalize.ModCore.playerInfo.magicManager.currentMagic >= this.magic) return true;
            return false;

        }

        /// <summary>
        /// Same as affording the cost but prevents the player from collapsing.
        /// </summary>
        /// <returns></returns>
        public bool canSafelyAffordCost()
        {
            if (Game1.player.stamina > this.stamina && Game1.player.health > this.health && Game1.player.Money >= this.gold && Revitalize.ModCore.playerInfo.magicManager.currentMagic >= this.magic) return true;
            return false;
        }

        /// <summary>
        /// Consume all necessary components for this cost.
        /// </summary>
        public void payCost()
        {
            if (canSafelyAffordCost())
            {
                Game1.player.stamina -= this.stamina;
                Game1.player.health -= this.health;
                Game1.player.Money = Game1.player.Money - this.gold;
                Revitalize.ModCore.playerInfo.magicManager.currentMagic -= this.magic;
            }
        }
    }
}
