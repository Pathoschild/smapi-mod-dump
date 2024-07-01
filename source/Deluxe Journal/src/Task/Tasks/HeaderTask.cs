/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

namespace DeluxeJournal.Task.Tasks
{
    /// <summary>Non-completable dummy task that serves as a header in the task list.</summary>
    internal class HeaderTask : TaskBase
    {
        public override bool Active
        {
            get => true;
            set { }
        }

        public override bool Complete
        {
            get => false;
            set { }
        }

        public override bool IsHeader => true;

        /// <summary>Whether this header has its task group collapsed.</summary>
        /// <remarks>Only the tasks shown in the <see cref="Menus.TasksOverlay"/> may be collapsed.</remarks>
        public bool IsCollapsed { get; set; }

        /// <summary>Serialization constructor.</summary>
        public HeaderTask() : base(TaskTypes.Header)
        {
        }

        public HeaderTask(string name) : base(TaskTypes.Header, name)
        {
        }
    }
}
