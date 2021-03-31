/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/explosivetortellini/StardewValleyDRP
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace Discord
{
    public partial class StorageManager
    {
        public IEnumerable<FileStat> Files()
        {
            var fileCount = Count();
            var files = new List<FileStat>();
            for (var i = 0; i < fileCount; i++)
            {
                files.Add(StatAt(i));
            }
            return files;
        }
    }
}
