/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/FishingTrawler
**
*************************************************/



using FishingTrawler.Framework.Utilities;
using StardewValley;

namespace FishingTrawler.Framework.Objects.Items.Resources
{
    public class CoalClump
    {
        private const int MAX_SIZE = 3;
        private const int JUNK_OBJECT_BASE_ID = 001;

        public static Object CreateInstance(int size = 1)
        {
            var coal = new Object($"{JUNK_OBJECT_BASE_ID}", 1);
            coal.modData[ModDataKeys.COAL_CLUMP_KEY] = size.ToString();

            return coal;
        }

        public static bool IsValid(Item item)
        {
            if (item is not null && item.modData.ContainsKey(ModDataKeys.COAL_CLUMP_KEY))
            {
                return true;
            }

            return false;
        }

        public static int GetSize(Item item)
        {
            if (IsValid(item) is false || int.TryParse(item.modData[ModDataKeys.COAL_CLUMP_KEY], out int size) is false)
            {
                return -1;
            }

            return size;
        }

        public static bool IncrementSize(Item item, int increment)
        {
            if (IsValid(item) is false)
            {
                return false;
            }

            int currentSize = GetSize(item);
            if (currentSize + increment > MAX_SIZE)
            {
                Game1.addHUDMessage(new HUDMessage(FishingTrawler.i18n.Get("game_message.coal_clump.max_stack"), 3) { timeLeft = 1000f });
                return false;
            }

            item.modData[ModDataKeys.COAL_CLUMP_KEY] = (currentSize + increment).ToString();

            return true;
        }
    }
}
