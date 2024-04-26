/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Hunter-Chambers/StardewValleyMods
**
*************************************************/

using StardewValley;
using StardewValley.GameData.BigCraftables;

namespace DeluxeAutoPetter.helpers
{
    internal static class ObjectDetails
    {
        /** **********************
         * Class Variables
         ********************** **/
        private static string? DELUXE_AUTO_PETTER_ID;
        private static BigCraftableData? DELUXE_AUTO_PETTER_DETAILS;

        /** **********************
         * Variable Getters
         ********************** **/
        public static string GetDeluxeAutoPetterID()
        {
            if (DELUXE_AUTO_PETTER_ID is null) throw new ArgumentNullException($"{nameof(DELUXE_AUTO_PETTER_ID)} has not been initialized! The {nameof(Initialize)} method must be called first!");

            return DELUXE_AUTO_PETTER_ID;
        }


        /** **********************
         * Public Methods
         ********************** **/
        public static void Initialize(string modID, string modDirectoryPath)
        {
            DELUXE_AUTO_PETTER_ID = $"{modID}.DeluxeAutoPetterObject";
            DELUXE_AUTO_PETTER_DETAILS = new()
            {
                Name = "Deluxe Auto-Petter",
                DisplayName = I18n.DeluxeAutoPetterDisplayName(),
                Description = I18n.DeluxeAutoPetterDescription(),
                Texture = Path.Combine(modDirectoryPath, "assets", "TileSheets", "DeluxeAutoPetter.xnb")
            };
        }

        public static void LoadObject()
        {
            if (DELUXE_AUTO_PETTER_DETAILS is null) throw new ArgumentNullException($"{nameof(DELUXE_AUTO_PETTER_DETAILS)} has not been initialized! The {nameof(Initialize)} method must be called first!");

            DataLoader.BigCraftables(Game1.content)[GetDeluxeAutoPetterID()] = DELUXE_AUTO_PETTER_DETAILS;
        }
    }
}
