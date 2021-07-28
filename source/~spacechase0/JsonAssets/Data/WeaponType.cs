/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using StardewValley.Tools;

namespace JsonAssets.Data
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum WeaponType
    {
        Dagger = MeleeWeapon.dagger,
        Club = MeleeWeapon.club,
        Sword = MeleeWeapon.defenseSword
    }
}
