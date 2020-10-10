/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/cheesysteak/stardew-steak
**
*************************************************/

namespace MoreMultiplayerInfo.Helpers
{
    public class GameTimeHelper
    {
        public static int GameTimeToMinutes(int input)
        {
            var hours = input / 100;
            var minutes = input % 100;

            return hours * 60 + minutes;
        }

    }
}
