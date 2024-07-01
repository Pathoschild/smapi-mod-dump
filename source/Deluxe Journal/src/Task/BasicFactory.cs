/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

namespace DeluxeJournal.Task
{
    /// <summary>Generic factory for an <see cref="ITask"/> without state.</summary>
    public class BasicFactory<T> : TaskFactory where T : ITask, new()
    {
        protected override void InitializeInternal(ITask task)
        {
        }

        protected override ITask? CreateInternal(string name)
        {
            return new T() { Name = name };
        }
    }
}
