/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ilyaki/ServerBookmarker
**
*************************************************/

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
