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
using StardewValley.GameData.Tools;
using StardewValley.ItemTypeDefinitions;
using DeluxeJournal.Util;

using static DeluxeJournal.Task.TaskParameterAttribute;

namespace DeluxeJournal.Task
{
    /// <summary>TaskFactory parameter.</summary>
    public class TaskParameter
    {
        private bool _valid;
        private bool _revalidate;

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
                _revalidate = true;
                Property.SetValue(Factory, value == null && Type.IsValueType ? Activator.CreateInstance(Type) : value);
            }
        }

        public TaskParameter(TaskFactory factory, PropertyInfo property, TaskParameterAttribute attribute)
        {
            _valid = false;
            _revalidate = true;
            Factory = factory;
            Property = property;
            Attribute = attribute;
        }

        /// <summary>Set the property value only if it abides by the <see cref="TaskParameterAttribute.Constraints"/>.</summary>
        /// <remarks>NOTE: This performs sanitization, see <see cref="Sanitize"/>.</remarks>
        /// <param name="value">The parameter value to be set.</param>
        /// <returns>Whether the value was set.</returns>
        public bool TrySetValue(object? value)
        {
            bool valid = Sanitize(value, out var sanitized);

            if (valid)
            {
                Value = sanitized;
                _valid = true;
                _revalidate = false;
            }

            return valid;
        }

        /// <summary>Check if this parameter has a value that abides by the <see cref="TaskParameterAttribute.Constraints"/>.</summary>
        public bool IsValid()
        {
            if (_revalidate)
            {
                _valid = Sanitize(Value, out _);
                _revalidate = false;
            }

            return _valid;
        }

        /// <summary>
        /// Take in a raw value and attempt to output a value that abides by this parameter's
        /// <see cref="TaskParameterAttribute.Constraints"/>. Sanitization will not add new information,
        /// but will only clamp <see cref="int"/> values or remove invalid entries in lists.
        /// </summary>
        /// 
        /// <param name="value">Raw input value.</param>
        /// <param name="sanitized">Sanitized output value, or <c>null</c> if unsuccessful.</param>
        /// 
        /// <returns>
        /// <c>true</c> if <paramref name="sanitized"/> follows the parameter contraints;
        /// <c>false</c> if the value could not be sanitized.
        /// </returns>
        public bool Sanitize(object? value, out object? sanitized)
        {
            Constraint constraints = Attribute.Constraints;
            sanitized = value;

            if (!Attribute.Required || constraints == Constraint.None)
            {
                return true;
            }

            if (value == null)
            {
                return !constraints.HasFlag(Constraint.NotNull);
            }
            else if (value is int num)
            {
                if (constraints.HasFlag(Constraint.GE1) && num < 1)
                {
                    sanitized = 1;
                }
                else if (constraints.HasFlag(Constraint.GE0) && num < 0)
                {
                    sanitized = 0;
                }
            }
            else if (value is string str && str.Length == 0)
            {
                return !constraints.HasFlag(Constraint.NotEmptyString);
            }
            else if (constraints.HasFlag(Constraint.NotEmpty) && value is IList<string> list && list.Count == 0)
            {
                return false;
            }
            else if (constraints.HasFlag(Constraint.ItemId) || constraints.HasFlag(Constraint.ItemCategory))
            {
                if (value is not IList<string> itemIds)
                {
                    if (value is not string itemId)
                    {
                        return false;
                    }

                    itemIds = new[] { itemId };
                }

                IList<string> sanitizedItemIds = new List<string>();

                foreach (string itemId in itemIds)
                {
                    if (constraints.HasFlag(Constraint.ItemCategory) && itemId.StartsWith('-'))
                    {
                        sanitizedItemIds.Add(itemId);
                        continue;
                    }
                    else if (!constraints.HasFlag(Constraint.ItemId))
                    {
                        continue;
                    }

                    string baseItemId = FlavoredItemHelper.GetPreserveId(itemId);

                    if (!ItemRegistry.Exists(baseItemId))
                    {
                        continue;
                    }
                    else if (constraints.HasFlag(Constraint.Upgradable))
                    {
                        if (ItemRegistry.GetData(baseItemId) is not ParsedItemData data
                            || data.RawData is not ToolData toolData
                            || !ToolHelper.IsToolUpgradable(toolData))
                        {
                            continue;
                        }
                    }
                    else if (Attribute.HasAnyConstraint(Constraint.SObject ^ Constraint.Craftable))
                    {
                        if (ItemRegistry.GetData(baseItemId) is ParsedItemData data
                            && (data.GetItemTypeId() == ItemRegistry.type_object
                                || data.GetItemTypeId() == ItemRegistry.type_bigCraftable))
                        {
                            if (constraints.HasFlag(Constraint.Craftable)
                                && data.Category != SObject.CraftingCategory
                                && data.Category != SObject.BigCraftableCategory
                                && !CraftingRecipe.craftingRecipes.ContainsKey(data.InternalName))
                            {
                                continue;
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }

                    sanitizedItemIds.Add(itemId);
                }

                if (value is IList<string>)
                {
                    sanitized = sanitizedItemIds;
                }
                else if (sanitizedItemIds.Count > 0)
                {
                    sanitized = sanitizedItemIds.First();
                }
                else
                {
                    sanitized = null;
                    return false;
                }

                return !(constraints.HasFlag(Constraint.NotEmpty) && sanitizedItemIds.Count == 0);
            }

            return true;
        }
    }
}
