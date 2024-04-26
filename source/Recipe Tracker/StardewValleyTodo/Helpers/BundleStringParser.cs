/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NoxChimaera/StardewValleyTODO
**
*************************************************/

namespace StardewValleyTodo.Helpers {
    public struct BundleParsedKey {
        public string RoomName { get; set; }
        public int SpriteIndex { get; set; }

        public BundleParsedKey(string roomName, int spriteIndex) {
            RoomName = roomName;
            SpriteIndex = spriteIndex;
        }
    }

    public struct BundleParsedValue {
        public string BundleName { get; set; }
        public string Reward { get; set; }
        public string Ingredients { get; set; }
        public int NumberOfItems { get; set; }
        public string DisplayName { get; set; }

        public BundleParsedValue(string bundleName, string reward, string ingredients, int numberOfItems, string displayName) {
            BundleName = bundleName;
            Reward = reward;
            Ingredients = ingredients;
            NumberOfItems = numberOfItems;
            DisplayName = displayName;
        }
    }

    public static class BundleStringParser {
        public static BundleParsedKey parseKey(string raw) {
            var parts = raw.Split('/');

            return new BundleParsedKey(
                parts[0],
                int.Parse(parts[1])
            );
        }

        public static BundleParsedValue parseValue(string raw) {
            // Animal/BO 16 1/186 1 0 182 1 0 174 1 0 438 1 0 440 1 0 442 1 0/4/5//Животный
            var parts = raw.Split('/');

            return new BundleParsedValue(
                // Animal
                parts[0],
                // BO 16 1
                parts[1],
                // 186 1 0 182 1 0 174 1 0 438 1 0 440 1 0 442 1 0
                parts[2],
                parts[4] == "" ? 0 : int.Parse(parts[4]),
                parts[6]
            );
        }
    }
}
