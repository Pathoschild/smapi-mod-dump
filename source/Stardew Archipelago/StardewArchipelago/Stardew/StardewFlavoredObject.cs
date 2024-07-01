/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

//using StardewArchipelago.Archipelago;
//using StardewArchipelago.Constants.Vanilla;
//using StardewArchipelago.Items.Mail;
//using StardewValley;

//namespace StardewArchipelago.Stardew
//{
//    public class StardewFlavoredObject : StardewObject
//    {
//        public StardewObject FlavorItem { get; private set; }
//        public override string Name { get; protected set; }

//        public StardewFlavoredObject(StardewObject flavorItem, string id, string name, int sellPrice, int edibility, string type, int category, string displayName, string description)
//        : base(id, name, sellPrice, edibility, type, category, displayName, description)
//        {
//            FlavorItem = flavorItem;
//        }

//        public override Item PrepareForGivingToFarmer(int amount = 1)
//        {
//            return new Object(Id, amount);
//        }

//        public override void GiveToFarmer(Farmer farmer, int amount = 1)
//        {
//            var item = PrepareForGivingToFarmer(amount);
//            farmer.addItemByMenuIfNecessary(item);
//        }

//        public override LetterAttachment GetAsLetter(ReceivedItem receivedItem, int amount = 1)
//        {
//            return new LetterItemAttachment(receivedItem, this, amount);
//        }

//        public override string GetQualifiedId()
//        {
//            return $"{QualifiedItemIds.OBJECT_QUALIFIER}{Id}";
//        }
//    }
//}



