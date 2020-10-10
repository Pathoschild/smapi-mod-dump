/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JohnsonNicholas/SDVMods
**
*************************************************/

using System;

namespace ClimatesOfFerngillRebuild
{
    public class WeatherNotificationArgs : EventArgs
    {
        public bool Present { get; private set; }
        public string Weather { get; private set; }

        public WeatherNotificationArgs(string weather, bool present)
        {
            Weather = weather;
            Present = present;
        }
    }
}
