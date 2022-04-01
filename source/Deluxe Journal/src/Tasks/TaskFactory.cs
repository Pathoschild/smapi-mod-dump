/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

using StardewModdingAPI;
using StardewValley;

namespace DeluxeJournal.Tasks
{
    /// <summary>Task factory used to create ITask instances.</summary>
    public abstract class TaskFactory
    {
        protected IReadOnlyList<TaskParameter>? _cachedParameters;

        /// <summary>Initialize the state of the factory with the values of an existing ITask instance.</summary>
        /// <param name="task">ITask instance.</param>
        /// <param name="translation">Translation helper.</param>
        public abstract void Initialize(ITask task, ITranslationHelper translation);

        /// <summary>Create a new ITask instance.</summary>
        /// <param name="name">The name of the new task.</param>
        public abstract ITask? Create(string name);

        /// <summary>Can this factory create a valid ITask in its current state?</summary>
        public virtual bool IsReady()
        {
            return GetParameters().All(parameter => parameter.IsValid());
        }

        /// <summary>The item to display in the AddTaskMenu as a "smart icon."</summary>
        public virtual Item? SmartIconItem()
        {
            return null;
        }

        /// <summary>The NPC to display in the AddTaskMenu as a "smart icon."</summary>
        public virtual NPC? SmartIconNPC()
        {
            return null;
        }

        /// <summary>The name to display in the AddTaskMenu as a "smart icon."</summary>
        public virtual string? SmartIconName()
        {
            return null;
        }

        /// <summary>Get the parameters of this factory.</summary>
        public IReadOnlyList<TaskParameter> GetParameters()
        {
            if (_cachedParameters == null)
            {
                _cachedParameters = GetType().GetProperties()
                    .Where(prop => Attribute.IsDefined(prop, typeof(TaskParameterAttribute)))
                    .Select(prop =>
                    {
                        var attribute = (TaskParameterAttribute)prop.GetCustomAttributes(typeof(TaskParameterAttribute), true).First();
                        return new TaskParameter(this, prop, attribute);
                    }).ToList();
            }

            return _cachedParameters;
        }
    }
}
