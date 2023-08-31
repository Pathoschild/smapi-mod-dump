/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using StardewValley;

namespace StardewArchipelago.Extensions
{
    public static class StringExtensions
    {
        public static string ToCapitalized(this string word)
        {
            if (word.Length < 2)
            {
                return word.ToUpper();
            }

            return word[..1].ToUpper() + word[1..].ToLower();
        }
    }
}
