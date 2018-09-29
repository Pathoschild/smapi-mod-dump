using Igorious.StardewValley.DynamicAPI.Constants;
using Igorious.StardewValley.DynamicAPI.Extensions;
using Newtonsoft.Json;

namespace Igorious.StardewValley.DynamicAPI.Data.Supporting
{
    public sealed class WayToGetCraftingRecipe
    {
        #region Properties

        [JsonProperty]
        public Skill Skill { get; set; }

        [JsonProperty]
        public int? SkillLevel { get; set; }

        [JsonProperty]
        public bool IsDefault { get; set; }

        [JsonIgnore]
        private bool UsedSkillFlag { get; set; }

        #endregion

        #region Serialization

        public static WayToGetCraftingRecipe Parse(string recipeWayToGet)
        {
            var parts = recipeWayToGet.Split(' ');
            var way = new WayToGetCraftingRecipe();
            switch (parts[0])
            {
                case "s":
                    way.UsedSkillFlag = true;
                    way.Skill = parts[1].ToEnum<Skill>();
                    way.SkillLevel = int.Parse(parts[2]);
                    break;

                case "l":
                    way.IsDefault = true;
                    break;

                case "null":
                    break;

                default:
                    way.Skill = parts[0].ToEnum<Skill>();
                    way.SkillLevel = int.Parse(parts[1]);
                    break;
            }
            return way;
        }

        public override string ToString()
        {
            if (IsDefault) return "l 0";
            if (UsedSkillFlag) return $"s {Skill} {SkillLevel}";
            if (SkillLevel != null) return $"{Skill} {SkillLevel}";
            return "null";
        }

        #endregion
    }
}