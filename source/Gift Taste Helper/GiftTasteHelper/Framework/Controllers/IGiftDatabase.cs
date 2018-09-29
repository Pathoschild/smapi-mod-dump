namespace GiftTasteHelper.Framework
{
    delegate void DataSourceChangedDelegate();

    internal interface IGiftDatabase
    {
        event DataSourceChangedDelegate DatabaseChanged;

        bool AddGift(string npcName, int itemId, GiftTaste taste);
        bool AddGifts(string npcName, GiftTaste taste, int[] itemIds);

        int[] GetGiftsForTaste(string npcName, GiftTaste taste);
    }
}
