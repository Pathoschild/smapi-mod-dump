/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using StardewValley;

namespace CustomNPCFramework.Framework.Enums
{
    /// <summary>Gender enum to signify the different genders for NPCs. Do what you want with this. For code simplicity anything that is non-binary is specified under other.</summary>
    public enum Genders
    {
        /// <summary>Used for npcs to signify that they are the male gender.</summary>
        male = NPC.male,

        /// <summary>Used for npcs to signify that they are the female gender.</summary>
        female = NPC.female,

        /// <summary>Used for npcs to signify that they are a non gender binary gender.</summary>
        other = NPC.undefined
    }
}
