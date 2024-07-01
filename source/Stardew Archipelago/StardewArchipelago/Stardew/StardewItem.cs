/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using StardewArchipelago.Archipelago;
using StardewArchipelago.Items.Mail;
using StardewValley;

namespace StardewArchipelago.Stardew
{
    public abstract class StardewItem
    {
        public string Id { get; private set; }
        public string Name { get; protected set; }
        public int SellPrice { get; private set; }
        public string DisplayName { get; private set; }
        public string Description { get; private set; }

        public StardewItem(string id, string name, int sellPrice, string displayName, string description)
        {
            // Debug.Assert(int.TryParse(id, out _));
            Id = id;
            Name = name;
            SellPrice = sellPrice;
            DisplayName = displayName;
            Description = description;
        }

        public abstract Item PrepareForGivingToFarmer(int amount = 1);

        public abstract void GiveToFarmer(Farmer farmer, int amount = 1);

        public abstract LetterAttachment GetAsLetter(ReceivedItem receivedItem, int amount = 1);

        public override string ToString()
        {
            return $"{Name} [{Id}]";
        }

        public abstract string GetQualifiedId();
    }
}
