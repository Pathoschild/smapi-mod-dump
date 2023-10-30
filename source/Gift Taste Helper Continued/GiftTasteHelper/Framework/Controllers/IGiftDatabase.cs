/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Just-Chaldea/GiftTasteHelper
**
*************************************************/

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
