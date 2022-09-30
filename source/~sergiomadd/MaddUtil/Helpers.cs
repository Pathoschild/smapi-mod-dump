/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sergiomadd/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;

namespace MaddUtil
{
    public static class Helpers
    {
        private static IModContentHelper modContentHelper;
        private static IContentHelper contentHelper;
        private static ITranslationHelper translationHelper;
        private static IModHelper modHelper;

        public static void SetModHelper(IModHelper helper)
        {
            modHelper = helper;
            SetContentHelper(helper.Content);
            SetModContentHelper(helper.ModContent);
            SetModContentHelper(helper.ModContent);
            SetTranslationHelper(helper.Translation);
        }

        public static IModHelper GetModHelper()
        {
            return modHelper;
        }
        public static void SetModContentHelper(IModContentHelper helper)
        {
            modContentHelper = helper;
        }

        public static IModContentHelper GetModContentHelper()
        {
            return modContentHelper;
        }
        public static void SetContentHelper(IContentHelper helper)
        {
            contentHelper = helper;
        }

        public static IContentHelper GetContentHelper()
        {
            return contentHelper;
        }
        public static void SetTranslationHelper(ITranslationHelper helper)
        {
            translationHelper = helper;
        }

        public static ITranslationHelper GetTranslationHelper()
        {
            return translationHelper;
        }
    }
}
