/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

namespace Omegasis.HappyBirthday.Framework
{
    /// <summary>The data for the current player.</summary>
    public class PlayerData
    {
        public string PlayersName;

        public long PlayerUniqueMultiplayerId;

        /// <summary>The player's current birthday day.</summary>
        public int BirthdayDay;

        /// <summary>The player's current birthday season.</summary>
        public string BirthdaySeason;

        public string favoriteBirthdayGift;
    }
}
