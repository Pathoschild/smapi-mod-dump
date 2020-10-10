/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

namespace Omegasis.SaveAnywhere.Framework.Models
{
    /// <summary>The data for the current player.</summary>
    public class PlayerData
    {
        /// <summary>The current time.</summary>
        public int Time { get; set; }

        /// <summary>The saved character data.</summary>
        public CharacterData[] Characters { get; set; }

        /// <summary>Checks if the character was swimming at save time.</summary>
        public bool IsCharacterSwimming { get; set; }
    }
}
