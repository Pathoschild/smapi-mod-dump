using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NpcAdventure.Loader
{
    internal static class AssetPatchHelper
    {
        internal static MethodInfo MakeKeyValuePatcher<T>(MethodInfo patchMethod)
        {
            // get dictionary's key/value types
            Type[] genericArgs = typeof(T).GetGenericArguments();
            if (genericArgs.Length != 2)
                throw new InvalidOperationException("Can't parse the asset's dictionary key/value types.");
            Type keyType = typeof(T).GetGenericArguments().FirstOrDefault();
            Type valueType = typeof(T).GetGenericArguments().LastOrDefault();
            if (keyType == null)
                throw new InvalidOperationException("Can't parse the asset's dictionary key type.");
            if (valueType == null)
                throw new InvalidOperationException("Can't parse the asset's dictionary value type.");

            if (!patchMethod.IsGenericMethodDefinition || patchMethod.GetGenericArguments().Length != 2)
            {
                throw new InvalidOperationException($"Patch method {patchMethod.Name} is not generic method definition or don't match generic pattern <TKey, TValue>");
            }

            return patchMethod.MakeGenericMethod(keyType, valueType);
        }
    }
}
