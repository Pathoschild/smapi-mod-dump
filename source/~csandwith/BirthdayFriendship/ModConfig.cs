/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/csandwith/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace BirthdayFriendship
{
    public class ModConfig
    {
        public bool ModEnabled { get; set; } = true;
        public int Hearts { get; set; } = 4;
    }
}
