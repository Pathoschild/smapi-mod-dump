using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterCrafting
{
    static class ReflectionExtension
    {
        public static TValue GetFieldValue<TValue>(this IReflectionHelper reflection, object obj, string name, bool required = true)
        {
            return reflection.GetPrivateField<TValue>(obj, name, required).GetValue();
        }
    }
}
