/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

using DeluxeJournal.Framework.Tasks;

namespace DeluxeJournal.Tasks
{
    /// <summary>Basic task with no auto-completion features.</summary>
    public class BasicTask : TaskBase
    {
        /// <summary>Serialization constructor.</summary>
        public BasicTask() : base(TaskTypes.Basic)
        {
        }

        public BasicTask(string name) : base(TaskTypes.Basic, name)
        {
        }
    }
}
