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
    public class ShippedItems:EventPrecondition
    {

        public List<KeyValuePair<int, int>> shippedItems;

        public ShippedItems()
        {
            this.shippedItems = new List<KeyValuePair<int, int>>();
        }

        public ShippedItems(int id, int amount)
        {
            this.shippedItems.Add(new KeyValuePair<int, int>(id, amount));
        }

        public ShippedItems(List<KeyValuePair<int,int>> ShippedItems)
        {
            this.shippedItems = ShippedItems.ToList();
        }

        public override string ToString()
        {
            return this.precondition_playerHasShippedTheseItems();
        }

        /// <summary>
        /// Current player has shipped at least <Amount> of the specified item. Can specify multiple item and number pairs, in which case all of them must be met.
        /// </summary>
        /// <param name="Pairs"></param>
        /// <returns></returns>
        public string precondition_playerHasShippedTheseItems()
        {
            StringBuilder b = new StringBuilder();
            b.Append("s ");
            for (int i = 0; i < this.shippedItems.Count; i++)
            {

                int ID = this.shippedItems[i].Key;
                int Amount = this.shippedItems[i].Value;
                b.Append(ID);
                b.Append(" ");
                b.Append(Amount.ToString());

                if (i != this.shippedItems.Count - 1)
                {
                    b.Append(" ");
                }
            }


            return b.ToString();
        }

        public override bool meetsCondition()
        {
            foreach (KeyValuePair<int, int> pair in this.shippedItems) {
                if (Game1.player.basicShipped.ContainsKey(pair.Key)){
                    if (Game1.player.basicShipped[pair.Key] <= pair.Value) return false;
                }
                else return false;
            }
            return true;
        }

    }
}
