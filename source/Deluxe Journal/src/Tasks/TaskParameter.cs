/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

using System.Reflection;
using StardewValley;

namespace DeluxeJournal.Tasks
{
    /// <summary>TaskFactory parameter.</summary>
    public class TaskParameter
    {
        /// <summary>Constraints on the parameter in the IsValid check.</summary>
        /// <remarks>
        /// IMPORTANT: This does NOT prevent the parameter value from being set if the value does not meet
        /// the Constraint requirements. Additional checks must be made by the programmer to enforce the
        /// constraints (if necessary).
        /// </remarks>
        [Flags]
        public enum Constraint
        {
            /// <summary>No constraints.</summary>
            None = 0,

            /// <summary>Object is not null.</summary>
            NotNull = 0x01,

            /// <summary>String is not empty.</summary>
            NotStringEmpty = 0x02,

            /// <summary>Integer greater than zero.</summary>
            GT0 = 0x04,

            /// <summary>Integer greater than or equal to zero.</summary>
            GE0 = 0x08 | GT0,

            /// <summary>Integer greater than or equal to one.</summary>
            GE1 = 0x10 | GE0,

            /// <summary>Object is of type StardewValley.Object.</summary>
            SObject = 0x20,

            /// <summary>SObject is giftable to an NPC.</summary>
            Giftable = 0x40 | SObject,

            /// <summary>SObject can be crafted.</summary>
            Craftable = 0x80 | SObject,

            /// <summary>Parameter is not null or empty (default).</summary>
            NotEmpty = NotNull | NotStringEmpty
        }

        /// <summary>The TaskFactory this parameter belongs to.</summary>
        public TaskFactory Factory { get; set; }

        /// <summary>The underlying property.</summary>
        public PropertyInfo Property { get; set; }

        /// <summary>The attribute associated with this parameter.</summary>
        public TaskParameterAttribute Attribute { get; set; }

        /// <summary>The property type.</summary>
        public Type Type => Property.PropertyType;

        /// <summary>The property value.</summary>
        public object? Value
        {
            get
            {
                return Property.GetValue(Factory);
            }

            set
            {
                Property.SetValue(Factory, (value == null && Type.IsValueType) ? Activator.CreateInstance(Type) : value);
            }
        }

        public TaskParameter(TaskFactory factory, PropertyInfo property, TaskParameterAttribute attribute)
        {
            Factory = factory;
            Property = property;
            Attribute = attribute;
        }

        /// <summary>Does this parameter have a valid value?</summary>
        public bool IsValid()
        {
            Constraint constraints = Attribute.Constraints;

            if (!Attribute.Required || constraints == Constraint.None)
            {
                return true;
            }

            if (Property.GetValue(Factory) is object value)
            {
                if (value is int num)
                {
                    if (constraints.HasFlag(Constraint.GE1))
                    {
                        return num >= 1;
                    }
                    else if (constraints.HasFlag(Constraint.GE0))
                    {
                        return num >= 0;
                    }
                    else if (constraints.HasFlag(Constraint.GT0))
                    {
                        return num > 0;
                    }
                }
                else if (value is string str)
                {
                    return !constraints.HasFlag(Constraint.NotStringEmpty) || str.Length > 0;
                }
                else if (constraints.HasFlag(Constraint.SObject))
                {
                    if (value is not SObject item)
                    {
                        return false;
                    }
                    else if (constraints.HasFlag(Constraint.Giftable) && !item.canBeGivenAsGift())
                    {
                        return false;
                    }
                    else if (constraints.HasFlag(Constraint.Craftable) &&
                        item.Category != SObject.CraftingCategory &&
                        item.Category != SObject.BigCraftableCategory &&
                        !CraftingRecipe.craftingRecipes.ContainsKey(item.Name))
                    {
                        return false;
                    }
                }
            }
            else
            {
                return !constraints.HasFlag(Constraint.NotNull);
            }

            return true;
        }
    }
}
