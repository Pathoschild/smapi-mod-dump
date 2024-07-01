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
    /// <summary>Attribute to mark a TaskFactory property as a TaskParameter.</summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class TaskParameterAttribute : Attribute
    {
        /// <summary>
        /// A collection of <see cref="Name"/> values. Also used in localization when
        /// displaying the parameter field.
        /// </summary>
        public static class TaskParameterNames
        {
            /// <summary>Specifies a color type parameter.</summary>
            public const string Color = "color";

            /// <summary>Specifies an item type parameter.</summary>
            public const string Item = "item";

            /// <summary>Specifies a tool type parameter.</summary>
            public const string Tool = "tool";

            /// <summary>Specifies an NPC type parameter.</summary>
            public const string NPC = "npc";

            /// <summary>Specifies a building type parameter.</summary>
            public const string Building = "building";

            /// <summary>Specifies a farm animal type parameter.</summary>
            public const string FarmAnimal = "animal";

            /// <summary>Specifies a counting type parameter.</summary>
            public const string Count = "count";

            /// <summary>Specifies a quality type parameter.</summary>
            public const string Quality = "quality";
        }

        /// <summary>Parser tags for populating parameter values.</summary>
        public enum TaskParameterTag
        {
            /// <summary>Task color schema index of type <see cref="int"/>.</summary>
            ColorIndex,

            /// <summary>
            /// A qualified item ID list of type <see cref="IList{T}"/> with generic
            /// type <see cref="string"/>.
            /// </summary>
            ItemList,

            /// <summary>
            /// A farm animal name list of type <see cref="IList{T}"/> with generic type
            /// <see cref="string"/>.
            /// </summary>
            FarmAnimalList,

            /// <summary>An NPC name of type <see cref="string"/>.</summary>
            NpcName,

            /// <summary>A building name of type <see cref="string"/>.</summary>
            Building,

            /// <summary>An item count of type <see cref="int"/>.</summary>
            Count,

            /// <summary>An item quality of type <see cref="int"/>.</summary>
            Quality
        }

        /// <summary>Input type when modifying the parameter value in the options menu.</summary>
        public enum TaskParameterInputType
        {
            /// <summary>Parse the value from a string.</summary>
            TextBox,

            /// <summary>Select the value from a set of options in a drop-down list.</summary>
            DropDown,

            /// <summary>Select the value from a set of buttons displaying a color.</summary>
            ColorButtons
        }

        /// <summary>Constraints on the parameter in the IsValid check.</summary>
        [Flags]
        public enum Constraint
        {
            /// <summary>No constraints.</summary>
            None = 0,

            /// <summary><see cref="object"/> is not null.</summary>
            NotNull = 1 << 0,

            /// <summary><see cref="string"/> is not empty.</summary>
            NotEmptyString = 1 << 1,

            /// <summary>Parameter is not null or empty.</summary>
            NotEmpty = NotNull | NotEmptyString,

            /// <summary><see cref="int"/> greater than or equal to zero.</summary>
            GE0 = 1 << 2 | NotNull,

            /// <summary><see cref="int"/> greater than or equal to one.</summary>
            GE1 = 1 << 3 | GE0,

            /// <summary>
            /// <see cref="IList{T}"/> of <see cref="string"/>s that contains existing item IDs.
            /// </summary>
            ItemId = 1 << 4,

            /// <summary>
            /// <see cref="IList{T}"/> of <see cref="string"/>s that contains item categories.
            /// </summary>
            ItemCategory = 1 << 5,

            /// <summary>
            /// <see cref="IList{T}"/> of item ID <see cref="string"/>s inheriting type
            /// <see cref="StardewValley.Object"/>.
            /// </summary>
            SObject = 1 << 6 | ItemId,

            /// <summary>
            /// <see cref="IList{T}"/> of item ID <see cref="string"/>s that can be crafted.
            /// </summary>
            Craftable = 1 << 7 | ItemId,

            /// <summary>
            /// <see cref="IList{T}"/> of item ID <see cref="string"/>s of type <see cref="StardewValley.Tool"/>
            /// that can be upgraded.
            /// </summary>
            Upgradable = 1 << 8 | ItemId
        }

        /// <summary>Parameter name. Also used in localization when displaying the parameter field.</summary>
        public string Name { get; set; }

        /// <summary>Name of the parent parameter that this parameter modifies.</summary>
        public string? Parent { get; set; } = null;

        /// <summary>Parser tag for populating parameter values.</summary>
        public TaskParameterTag Tag { get; set; }

        /// <summary>Input type when modifying the parameter value in the options menu.</summary>
        public TaskParameterInputType InputType { get; set; } = TaskParameterInputType.TextBox;

        /// <summary>Is this task required?</summary>
        /// <remarks>If set to <c>false</c>, IsValid is always true.</remarks>
        public bool Required { get; set; } = true;

        /// <summary>If set to <c>true</c>, this parameter is not exposed to the user in the task options menu.</summary>
        public bool Hidden { get; set; } = false;

        /// <summary>Constraints on the parameter in the IsValid check.</summary>
        /// <remarks>
        /// IMPORTANT: <see cref="Constraint"/> flags DO NOT ensure that the parameter
        /// value is set with the correct restrictions. The constraints are only used to
        /// filter the input parsed by the <see cref="TaskParser"/>. Parameter values may
        /// still need to be verified within the task/factory logic.
        /// </remarks>
        public Constraint Constraints { get; set; } = Constraint.NotEmpty;

        public TaskParameterAttribute(string name, TaskParameterTag tag)
        {
            Name = name;
            Tag = tag;
        }

        /// <summary>
        /// Determines whether the <see cref="Constraints"/> has one or more <see cref="Constraint"/>
        /// flag enabled.
        /// </summary>
        /// <param name="constraints">Bitwise OR'd <see cref="Constraint"/> flags to test.</param>
        public bool HasAnyConstraint(Constraint constraints)
        {
            return (Constraints & constraints) != Constraint.None;
        }
    }
}
