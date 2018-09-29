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
