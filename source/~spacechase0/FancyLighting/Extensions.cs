/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;

namespace FancyLighting
{
    public static class Extensions
    {
        public static string GetLogSummary(this Exception exception)
        {
            return (string)AccessTools.Method("StardewModdingAPI.Internal.ExceptionHelper:GetLogSummary").Invoke(null, new object[] { exception });
        }
        public static string GetMenuChainLabel(this IClickableMenu menu)
        {
            return (string)AccessTools.Method("StardewModdingAPI.Framework.InternalExtensions:GetMenuChainLabel").Invoke(null, new object[] { menu });
        }
    }
}
