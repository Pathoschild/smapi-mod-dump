/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Hunter-Chambers/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Inventories;
using StardewValley.Menus;

namespace DeluxeAutoPetter.helpers
{
    internal static class QuestDetails
    {
        /** **********************
         * Class Variables
         ********************** **/
        private static string? QUEST_ID;
        private static string? QUEST_MAIL_ID;
        private static string? QUEST_REWARD_MAIL_ID;
        private static string? DELUXE_AUTO_PETTER_ID;

        private static readonly string AUTO_PETTER_ID = "(BC)272";
        private static readonly string HARDWOOD_ID = "(O)709";
        private static readonly string IRIDIUM_BAR_ID = "(O)337";

        private static readonly string DROPBOX_GAME_LOCATION = "Mountain";
        private static readonly Vector2 DROPBOX_LOCATION = new Vector2(18.5f, 25.5f) * Game1.tileSize;
        private static readonly Vector2 DROPBOX_INDICATOR_LOCATION = new(DROPBOX_LOCATION.X - 3, DROPBOX_LOCATION.Y - Game1.tileSize); // the indicator is 6px wide, so -3px to center it
        private static readonly Rectangle DROPBOX_BOUNDING_BOX = new((int)DROPBOX_LOCATION.X - (int)(Game1.tileSize * 1.5), (int)DROPBOX_LOCATION.Y - (int)(Game1.tileSize * 2.5), Game1.tileSize * 3, Game1.tileSize * 3);

        private static readonly Dictionary<string, int> DONATION_REQUIREMENTS = new()
        {
            { AUTO_PETTER_ID, 1 },
            { HARDWOOD_ID, 300 },
            { IRIDIUM_BAR_ID, 25 }
        };

        private static MultiplayerHandler.QuestData? QUEST_DATA;
        private static Inventory? DONATED_ITEMS;

        /** **********************
         * Variable Getters
         ********************** **/
        // ID Getters
        public static string GetAutoPetterID()
        {
            return AUTO_PETTER_ID;
        }

        public static string GetHardwoodID()
        {
            return HARDWOOD_ID;
        }

        public static string GetIridiumBarID()
        {
            return IRIDIUM_BAR_ID;
        }

        public static string GetQuestID()
        {
            if (QUEST_ID is null) throw new ArgumentNullException($"{nameof(QUEST_ID)} has not been initialized! The {nameof(Initialize)} method must be called first!");

            return QUEST_ID;
        }

        public static string GetQuestMailID()
        {
            if (QUEST_MAIL_ID is null) throw new ArgumentNullException($"{nameof(QUEST_MAIL_ID)} has not been initialized! The {nameof(Initialize)} method must be called first!");

            return QUEST_MAIL_ID;
        }

        public static string GetDeluxeAutoPetterID()
        {
            if (DELUXE_AUTO_PETTER_ID is null) throw new ArgumentNullException($"{nameof(DELUXE_AUTO_PETTER_ID)} has not been initialized! The {nameof(Initialize)} method must be called first!");

            return DELUXE_AUTO_PETTER_ID;
        }

        // Data Getters
        public static string GetDropBoxGameLocationString()
        {
            return DROPBOX_GAME_LOCATION;
        }

        public static bool GetIsTriggered()
        {
            if (QUEST_DATA is null) throw new ArgumentNullException($"{nameof(QUEST_DATA)} has not been initialized! The {nameof(LoadQuestData)} method must be called first!");

            return QUEST_DATA.IsTriggered;
        }

        /** **********************
         * Data Setters
         ********************** **/
        public static void SetIsTriggered(bool isTriggered)
        {
            if (QUEST_DATA is null) throw new ArgumentNullException($"{nameof(QUEST_DATA)} has not been initialized! The {nameof(LoadQuestData)} method must be called first!");

            QUEST_DATA.IsTriggered = isTriggered;
        }

        /** **********************
         * Public Methods
         ********************** **/
        // Utility methods
        public static void Initialize(string modID)
        {
            QUEST_ID = $"{modID}.Quest";
            DELUXE_AUTO_PETTER_ID = $"{modID}.DeluxeAutoPetter";
            QUEST_MAIL_ID = $"{modID}.Mail0";
            QUEST_REWARD_MAIL_ID = $"{modID}.Mail1";
        }

        public static void LoadQuestData(long playerID)
        {
            QUEST_DATA = MultiplayerHandler.GetPlayerQuestData(playerID);
            CreateDonatedInventory(QUEST_DATA.DonationCounts);
        }

        public static bool IsQuestDataNull()
        {
            return QUEST_DATA is null;
        }

        // Visual Methods
        public static void ShowDropboxLocator(bool doShow)
        {
            Game1.getLocationFromName(DROPBOX_GAME_LOCATION).showDropboxIndicator = doShow;
            Game1.getLocationFromName(DROPBOX_GAME_LOCATION).dropBoxIndicatorLocation = DROPBOX_INDICATOR_LOCATION;
        }

        public static bool IsMouseOverDropbox(Vector2 mousePosition)
        {
            return DROPBOX_BOUNDING_BOX.Contains(mousePosition * Game1.tileSize);
        }

        public static Vector2 GetInteractionDistanceFromDropboxVector(Vector2 playerInteractionPosition)
        {
            return playerInteractionPosition - DROPBOX_LOCATION;
        }

        public static QuestContainerMenu CreateQuestContainerMenu()
        {
            if (DONATED_ITEMS is null) throw new ArgumentNullException($"{nameof(DONATED_ITEMS)} has not been initialized! The {nameof(LoadQuestData)} method must be called first!");

            return new QuestContainerMenu(DONATED_ITEMS, 1, HighlightAcceptableItems, GetAcceptCount, null, UpdateDonationCounts);
        }

        /** **********************
         * Private Methods
         ********************** **/
        private static bool HighlightAcceptableItems(Item item)
        {
            return DONATION_REQUIREMENTS.ContainsKey(item.QualifiedItemId);
        }

        private static int GetAcceptCount(Item item)
        {
            if (DONATED_ITEMS is null) throw new ArgumentNullException($"{nameof(DONATED_ITEMS)} has not been initialized! The {nameof(LoadQuestData)} method must be called first!");

            if (!HighlightAcceptableItems(item)) return 0; // basically means 'if not valid, then return 0'

            int totalNeeded = DONATION_REQUIREMENTS[item.QualifiedItemId];
            int donatedCount = DONATED_ITEMS.FirstOrDefault(donatedItem => donatedItem is not null && donatedItem.QualifiedItemId.Equals(item.QualifiedItemId), null)?.Stack ?? 0;

            return Math.Min(totalNeeded - donatedCount, item.Stack);
        }

        private static void UpdateDonationCounts()
        {
            if (QUEST_ID is null) throw new ArgumentNullException($"{nameof(QUEST_ID)} has not been initialized! The {nameof(Initialize)} method must be called first!");
            if (QUEST_REWARD_MAIL_ID is null) throw new ArgumentNullException($"{nameof(QUEST_REWARD_MAIL_ID)} has not been initialized! The {nameof(Initialize)} method must be called first!");
            if (DONATED_ITEMS is null) throw new ArgumentNullException($"{nameof(DONATED_ITEMS)} has not been initialized! The {nameof(LoadQuestData)} method must be called first!");
            if (QUEST_DATA is null) throw new ArgumentNullException($"{nameof(QUEST_DATA)} has not been initialized! The {nameof(LoadQuestData)} method must be called first!");

            foreach (Item? item in DONATED_ITEMS)
                if (item is not null) QUEST_DATA.DonationCounts[item.QualifiedItemId] = item.Stack;

            if (AreDonationRequirementsMet())
            {
                Game1.player.completeQuest(QUEST_ID);
                ShowDropboxLocator(false);
                Game1.player.mailForTomorrow.Add(QUEST_REWARD_MAIL_ID);
            }
        }

        private static bool AreDonationRequirementsMet()
        {
            if (DONATED_ITEMS is null) throw new ArgumentNullException($"{nameof(DONATED_ITEMS)} has not been initialized! The {nameof(LoadQuestData)} method must be called first!");

            bool completed = true; // assume quest is completed
            int i = DONATED_ITEMS.Count - 1;
            while (i >= 0 && completed)
            {
                Item? item = DONATED_ITEMS[i];
                if (item is null || item.Stack != DONATION_REQUIREMENTS[item.QualifiedItemId]) completed = false;
                else i--;
            }

            return completed;
        }

        private static void CreateDonatedInventory(Dictionary<string, int> donatedDetails)
        {
            Item? autoPetter = donatedDetails[AUTO_PETTER_ID] <= 0 ? null : ItemRegistry.Create(AUTO_PETTER_ID, donatedDetails[AUTO_PETTER_ID]);
            Item? hardwood = donatedDetails[HARDWOOD_ID] <= 0 ? null : ItemRegistry.Create(HARDWOOD_ID, donatedDetails[HARDWOOD_ID]);
            Item? iridiumBars = donatedDetails[IRIDIUM_BAR_ID] <= 0 ? null : ItemRegistry.Create(IRIDIUM_BAR_ID, donatedDetails[IRIDIUM_BAR_ID]);

            DONATED_ITEMS = new() { autoPetter, hardwood, iridiumBars };
        }
    }
}
