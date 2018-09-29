using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewSymphonyRemastered
{
    public static class StaticExtentions
    {
        /// <summary>
        /// Checks if a given object is null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool isNull<T>(this T obj)
        {
            if (obj == null) return true;
            else return false;
        }
    }
}
