using System.Collections.Generic;

namespace ServerBookmarker
{
    public class BookmarksDataModel
    {
        public Dictionary<string, string> Bookmarks { get; set; } = new Dictionary<string, string>() 
        {
            {"Localhost", "127.0.0.1" }
        };
    }
}
