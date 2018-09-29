using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiftTasteHelper.Framework
{
    #region BaseGiftDataProvider
    internal abstract class BaseGiftDataProvider : IGiftDataProvider
    {
        public event DataSourceChangedDelegate DataSourceChanged;

        protected IGiftDatabase Database;

        public BaseGiftDataProvider(IGiftDatabase database)
        {
            this.Database = database;
            this.Database.DatabaseChanged += () => DataSourceChanged?.Invoke();
        }

        public virtual IEnumerable<int> GetGifts(string npcName, GiftTaste taste, bool includeUniversal)
        {
            IEnumerable<int> gifts = Database.GetGiftsForTaste(npcName, taste);
            if (includeUniversal)
            {
                // Individual NPC tastes may conflict with the universal ones.
                return GetUniversalGifts(npcName, taste).Concat(gifts);
            }
            return gifts;
        }

        public virtual IEnumerable<int> GetUniversalGifts(string npcName, GiftTaste taste)
        {
            return Database.GetGiftsForTaste(Utils.UniversalTasteNames[taste], taste)
                .Where(itemId => Utils.GetTasteForGift(npcName, itemId) == taste);
        }
    }
    #endregion BaseGiftDataProvider

    #region ProgressionGiftDataProvider
    internal class ProgressionGiftDataProvider : BaseGiftDataProvider
    {
        public ProgressionGiftDataProvider(IGiftDatabase database)
            : base(database)
        {
        }

        public override IEnumerable<int> GetGifts(string npcName, GiftTaste taste, bool includeUniversal)
        {
            // Universal gifts are stored with the regular ones in the DB so we need to remove them if they shouldn't be included.
            var gifts = base.GetGifts(npcName, taste, includeUniversal);
            if (!includeUniversal)
            {
                // Filter out any that are also in the universal table.
                // Note that this probably won't work correctly for categories, but we're not bothering with those for now.
                var universal = Utils.GetItemsForTaste(Utils.UniversalTasteNames[taste], taste);
                return gifts.Except(universal);
            }
            return gifts;
        }

        public override IEnumerable<int> GetUniversalGifts(string npcName, GiftTaste taste)
        {
            var universal = Utils.GetItemsForTaste(Utils.UniversalTasteNames[taste], taste);
            return Database.GetGiftsForTaste(npcName, taste).Intersect(universal);
        }
    }
    #endregion ProgressionGiftDataProvider

    #region AllGiftDataProvider 
    internal class AllGiftDataProvider : BaseGiftDataProvider
    {
        public AllGiftDataProvider(IGiftDatabase database)
            : base(database)
        {
            var tasteTypes = Enum.GetValues(typeof(GiftTaste)).Cast<GiftTaste>().Where(val => val != GiftTaste.MAX);
            foreach (var giftTaste in Game1.NPCGiftTastes)
            {
                foreach (var taste in tasteTypes)
                {
                    Database.AddGifts(giftTaste.Key, taste, Utils.GetItemsForTaste(giftTaste.Key, taste));
                }
            }
        }
    }
    #endregion AllGiftDataProvider
}
