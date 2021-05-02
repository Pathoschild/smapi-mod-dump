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
using StardewValley;

namespace StardustCore.Events.Preconditions.PlayerSpecific
{
    public class HasItem:EventPrecondition
    {

        public int id;
        public int amount;

        public HasItem()
        {

        }

        public HasItem(int ID,int Amount=1)
        {
            this.id = ID;
            this.amount = Amount;
        }


        public override string ToString()
        {
            return this.precondition_playerHasItem();
        }


        /// <summary>
        /// Player has the item with the given id. Parent sheet index?
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public string precondition_playerHasItem()
        {
            StringBuilder b = new StringBuilder();
            b.Append("i ");
            b.Append(this.id.ToString());
            return b.ToString();
        }

        public override bool meetsCondition()
        {
            return Game1.player.hasItemInInventory(this.id,this.amount);
        }
    }
}
