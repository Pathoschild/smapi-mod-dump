using System.Collections.Generic;

namespace ServerBrowser
{
	class ModConfig
	{
		public List<string> RequiredMods { get; set; } = new List<string>() { };//Use UniqueIDs, e.g. Ilyaki.ServerBrowser
		public string ServerMessage { get; set; } = "This is my server message.\nHello world!";
		public string Password { get; set; } = "";
	}
}
