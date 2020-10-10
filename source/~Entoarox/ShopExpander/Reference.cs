/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Entoarox/StardewMods
**
*************************************************/

namespace Entoarox.ShopExpander
{
    internal class Reference
    {
        /*********
        ** Accessors
        *********/
        public string Owner;
        public int Item;
        public int Amount;
        public string Conditions = null;


        /*********
        ** Public methods
        *********/
        public Reference(string owner, int item, int amount, string conditions = null)
        {
            this.Owner = owner;
            this.Item = item;
            this.Amount = amount;
            this.Conditions = conditions;
        }
    }
}
