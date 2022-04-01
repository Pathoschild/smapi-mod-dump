/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

using Newtonsoft.Json;
using DeluxeJournal.Framework.Serialization;
using DeluxeJournal.Tasks;

namespace DeluxeJournal.Framework.Data
{
    [JsonConverter(typeof(TaskDataConverter))]
    internal class TaskData
    {
        public IDictionary<string, List<ITask>> Tasks { get; set; } = new Dictionary<string, List<ITask>>();
    }
}
