/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/miketweaver/DailyNews
**
*************************************************/

using System.Collections.Generic;

namespace DailyNews
{
    public class ModConfig
    {
        public bool showMessages { get; set; } = true;
		public string defaultNewscaster { get; set; } = "assets/news.png";
        public string extension { get; set; } = "*.json";
        public string contentFolder { get; set; } = "news";
	}
}
