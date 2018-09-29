using Igorious.StardewValley.DynamicAPI.Constants;
using Igorious.StardewValley.DynamicAPI.Extensions;
using Newtonsoft.Json;

namespace Igorious.StardewValley.DynamicAPI.Data.Supporting
{
    public class CharacterGiftPreferences : GiftPreferences
    {
        #region	Properties

        [JsonProperty]
        public string LovedText { get; set; }

        [JsonProperty]
        public string LikedText { get; set; }

        [JsonProperty]
        public string NeutralText { get; set; }

        [JsonProperty]
        public string DislikedText { get; set; }

        [JsonProperty]
        public string HatedText { get; set; }

        #endregion

        #region Serialization

        public static CharacterGiftPreferences Parse(string characterGiftPreferences, string name = null)
        {
            var parts = characterGiftPreferences.Split('/');
            return new CharacterGiftPreferences
            {
                Name = !string.IsNullOrWhiteSpace(name)? name.ToEnum<CharacterName>() : CharacterName.Universal,
                LovedText = parts[0].TrimStart(),
                Loved = PartToIDs(parts[1]),
                LikedText = parts[2],
                Liked = PartToIDs(parts[3]),
                DislikedText = parts[4],
                Disliked = PartToIDs(parts[5]),
                HatedText = parts[6],
                Hated = PartToIDs(parts[7]),
                NeutralText = parts[8],
                Neutral = PartToIDs(parts[9]),
            };
        }

        public override string ToString()
        {
            return $" {LovedText}/{Loved.Serialize()}/{LikedText}/{Liked.Serialize()}/{DislikedText}/{Disliked.Serialize()}"
                   + $"/{HatedText}/{Hated.Serialize()}/{NeutralText}/{Neutral.Serialize()}/ ";
        }

        #endregion
    }
}