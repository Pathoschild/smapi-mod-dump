namespace ServerBrowser
{
	class SearchOptions
	{
		public bool ShowFullServers { get; set; } = true;
		public bool ShowFullCabinServers { get; set; } = true;
		public bool ShowPasswordProtectedSerers { get; set; } = false;
		public string SearchQuery { get; set; } = "";
	}
}
