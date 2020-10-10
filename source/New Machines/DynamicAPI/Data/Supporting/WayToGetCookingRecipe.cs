/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Igorious/StardevValleyNewMachinesMod
**
*************************************************/

using System.ComponentModel;
using Igorious.StardewValley.DynamicAPI.Constants;
using Igorious.StardewValley.DynamicAPI.Extensions;
using Newtonsoft.Json;

namespace Igorious.StardewValley.DynamicAPI.Data.Supporting
{
    public sealed class WayToGetCookingRecipe
    {
        #region Properties

        [JsonProperty]
        public string FriendshipWith { get; set; }

        [JsonProperty]
        public decimal? Hearts { get; set; }

        [JsonProperty]
        public Skill Skill { get; set; }

        [JsonProperty]
        public int? SkillLevel { get; set; }

        [JsonProperty]
        public bool FromTV { get; set; }

        [JsonIgnore, DefaultValue(100)]
        private int FromTVIndex { get; set; } = 100;

        #endregion

        #region Serialization

        public static WayToGetCookingRecipe Parse(string recipeWayToGet)
        {
            var parts = recipeWayToGet.Split(' ');
            var way = new WayToGetCookingRecipe();
            switch (parts[0])
            {
                case "f":
                    way.FriendshipWith = parts[1];
                    way.Hearts = int.Parse(parts[2]);
                    break;

                case "s":
                    way.Skill = parts[1].ToEnum<Skill>();
                    way.SkillLevel = int.Parse(parts[2]);
                    break;

                case "l":
                    way.FromTV = true;
                    way.FromTVIndex = int.Parse(parts[1]);
                    break;
            }
            return way;
        }

        public override string ToString()
        {
            if (FriendshipWith != null) return $"f {FriendshipWith} {Hearts}";
            if (Skill != Skill.Undefined) return $"s {Skill} {SkillLevel}";
            if (FromTV) return $"l {FromTVIndex}";
            return "default";
        }

        #endregion
    }
}