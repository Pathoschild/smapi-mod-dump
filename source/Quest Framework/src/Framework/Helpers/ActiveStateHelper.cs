/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/QuestFramework
**
*************************************************/

using QuestFramework.Quests.State;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace QuestFramework.Framework.Helpers
{
    internal static class ActiveStateHelper
    {
        public static IEnumerable<ActiveStateField> GatherActiveStateProperties(object fromObj)
        {
            return from prop in fromObj.GetType().GetProperties()
                   where GetActiveStateAttr(prop) != null
                   select TransformProperty(prop);

            ActiveStateAttribute GetActiveStateAttr(PropertyInfo propInfo)
            {
                return propInfo.GetCustomAttribute<ActiveStateAttribute>();
            }

            ActiveStateField TransformProperty(PropertyInfo propInfo)
            {
                if (propInfo.GetValue(fromObj) == null)
                {
                    throw new ActiveStateFieldException($"Active state property `{propInfo.Name}` can't return null!");
                }

                if (!(propInfo.GetValue(fromObj) is ActiveStateField stateProp))
                {
                    throw new ActiveStateFieldException(
                        $"Active state property `{propInfo.Name}` type `{propInfo.PropertyType.Name}` " +
                        $"is incompatible with type `ActiveStateField`");
                }

                if (stateProp.Name == null)
                {
                    stateProp.Name = GetActiveStateAttr(propInfo).Name ?? propInfo.Name;
                }

                return stateProp;
            }
        }

        public static IEnumerable<ActiveStateField> GatherActiveStateFields(object fromObj)
        {
            return from field in fromObj.GetType().GetFields()
                   where GetActiveStateAttr(field) != null
                   select TransformField(field);

            ActiveStateAttribute GetActiveStateAttr(FieldInfo fieldInfo)
            {
                return fieldInfo.GetCustomAttribute<ActiveStateAttribute>();
            }

            ActiveStateField TransformField(FieldInfo fieldInfo)
            {
                if (fieldInfo.GetValue(fromObj) == null)
                {
                    throw new ActiveStateFieldException($"Active state field `{fieldInfo.Name}` can't be null!");
                }

                if (!(fieldInfo.GetValue(fromObj) is ActiveStateField stateField))
                {
                    throw new ActiveStateFieldException(
                        $"Active state field `{fieldInfo.Name}` type `{fieldInfo.FieldType.Name}` " +
                        $"is incompatible with type `ActiveStateField`");
                }

                if (stateField.Name == null)
                {
                    stateField.Name = GetActiveStateAttr(fieldInfo).Name ?? fieldInfo.Name;
                }

                return stateField;
            }
        }
    }
}
