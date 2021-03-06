/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;

namespace TehPers.CoreMod.ContentPacks.Data {
    internal class ContentPackData {
        /// <summary>Paths to additional content files which should be loaded as part of this content pack.</summary>
        public string[] Include { get; set; } = new string[0];

        // TODO: Support both string values (comma separated values like CP) and array values
        /// <summary>Details about the config file. Format is { propertyName, possibleValues }.</summary>
        public Dictionary<string, PossibleConfigValuesData> Config { get; set; } = new Dictionary<string, PossibleConfigValuesData>();

        /// <summary>Custom base objects, which override the texture "Maps/springobjects".</summary>
        public Dictionary<string, SObjectData> Objects { get; set; } = new Dictionary<string, SObjectData>();

        /// <summary>Custom weapons, which override the texture "Maps/springobjects".</summary>
        public Dictionary<string, WeaponData> Weapons { get; set; } = new Dictionary<string, WeaponData>();

        /// <summary>Custom recipes.</summary>
        public List<RecipeData> Recipes { get; set; } = new List<RecipeData>();
    }
}
