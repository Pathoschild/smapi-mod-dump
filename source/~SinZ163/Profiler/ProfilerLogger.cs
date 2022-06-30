/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/SinZ163/StardewMods
**
*************************************************/

using StardewModdingAPI;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Profiler
{
    internal record ProfileLoggerRow(DateTimeOffset OccuredAt, object Metadata);

    public class ProfilerLogger : IDisposable
    {

        private readonly StreamWriter File;


        public ConcurrentStack<EventMetadata> EventMetadata { get; private set; }

        internal ProfilerLogger(string path)
        {
            EventMetadata = new();
            File = new StreamWriter(path, append: false);
        }

        internal void AddRow(ProfileLoggerRow row)
        {
            File.WriteLine(JsonSerializer.Serialize(row));
            File.Flush();
        }

        public void Dispose()
        {
            File.Dispose();
        }
    }
}
