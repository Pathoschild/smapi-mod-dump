/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/xirsoi/MayonnaisePlusPlus
**
*************************************************/

using System.Collections.Generic;
using StardewModdingAPI;

namespace MayonnaisePlusPlus
{
	internal class Loader {
		public static IModHelper HELPER;
		public static ITranslationHelper I18N;
		public static ModConfig CONFIG;
		public static Dictionary<string,int> DATA;

		public Loader(IModHelper helper) {
			HELPER = helper;
			I18N = helper.Translation;
			DATA = new Dictionary<string, int>();
			CONFIG = helper.ReadConfig<ModConfig>();
		}
	}
}
