/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/GStefanowich/SDV-Forecaster
**
*************************************************/

using System;
using System.Globalization;
using System.Linq;

namespace ForecasterText.Objects {
    public static class Config {
        public static string Normalize<T>(T e) where T : struct, Enum
            => CultureInfo.InvariantCulture.TextInfo.ToTitleCase(e.ToString().ToLower().Replace('_', ' '));
        
        public static T FromInput<T>(string value) where T : struct, Enum
            => Enum.TryParse(value?.Replace(' ', '_'), true, out T display) ? display : default;
        
        public static string[] Values<T>() where T : struct, Enum
            => Enum.GetValues<T>().Select(Config.Normalize).ToArray();
    }
}
