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

using static DeluxeJournal.Task.TaskParameterAttribute;

namespace DeluxeJournal.Task
{
    /// <summary>Task factory used to create ITask instances.</summary>
    public abstract class TaskFactory
    {
        protected const Constraint ItemIdsConstraint = Constraint.ItemId | Constraint.ItemCategory | Constraint.NotEmpty;
        protected const Constraint ObjectIdsConstraint = Constraint.SObject | Constraint.ItemCategory | Constraint.NotEmpty;
        protected const Constraint OptionalItemIdsConstraint = Constraint.ItemId | Constraint.ItemCategory;
        protected const Constraint OptionalObjectIdsConstraint = Constraint.SObject | Constraint.ItemCategory;

        private IReadOnlyList<TaskParameter>? _cachedParameters;

        /// <summary>Flags indicating which smart icons are enabled.</summary>
        public virtual SmartIconFlags EnabledSmartIcons => SmartIconFlags.None;

        /// <summary>Allow drawing the <see cref="TaskParser.Count"/> value.</summary>
        public virtual bool EnableSmartIconCount => false;

        /// <summary>Initialize the state of the factory with the values of an existing ITask instance.</summary>
        /// <param name="task">ITask instance.</param>
        /// <param name="translation">Translation helper.</param>
        public abstract void Initialize(ITask task, ITranslationHelper translation);

        /// <summary>Create a new <see cref="ITask"/> instance.</summary>
        /// <param name="name">The name of the new task.</param>
        /// <returns>A new task inheriting from <see cref="ITask"/> or <c>null</c> if the parameter values are insufficient.</returns>
        public abstract ITask? Create(string name);

        /// <summary>Can this factory create a valid ITask in its current state?</summary>
        public virtual bool IsReady()
        {
            return GetParameters().All(parameter => parameter.IsValid());
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
