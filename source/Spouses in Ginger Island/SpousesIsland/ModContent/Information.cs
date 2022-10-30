/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/SpousesIsland
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;

namespace SpousesIsland.Framework
{
    internal class Information
    {
        /// <summary>
        /// Checks a translation's key. If it's one of the game's language codes, returns its filename/extension. If not, returns the language code as-is.
        /// </summary>
        internal static string ParseLangCode(string key)
        {
            switch (key.ToLower())
            {
                case "de":
                    return ".de-DE";
                case "en":
                    return "";
                case "es":
                    return ".es-ES";
                case "fr":
                    return ".fr-FR";
                case "hu":
                    return ".hu-HU";
                case "it":
                    return ".it-IT";
                case "ja":
                    return ".ja-JP";
                case "ko":
                    return ".ko-KR";
                case "pt":
                    return ".pt-BR";
                case "ru":
                    return ".ru-RU";
                case "tr":
                    return ".tr-TR";
                case "zh":
                    return ".zh-CN";
                case null:
                    return "";
                default:
                    return $".{key}";
            }
        }

        internal static bool ShouldReloadDevan(bool Leah, bool Elliott, bool CCC)
        {
            if (Leah is true || Elliott is true || CCC is true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        internal static bool HasMod(string ModID)
        {
            if (ModEntry.Help.ModRegistry.Get(ModID) is not null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
