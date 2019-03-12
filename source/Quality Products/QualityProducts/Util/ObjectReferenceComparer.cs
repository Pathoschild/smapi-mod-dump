using System.Runtime.CompilerServices;
using System.Collections.Generic;

/*** 
 * From Pathoschild.Stardew.Common.Utilities
 * see https://github.com/Pathoschild/StardewMods/blob/595c21818eea6ace20280180b17004153e9dacee/Common/Utilities/ObjectReferenceComparer.cs
 ***/
namespace QualityProducts.Util
{
    internal class ObjectReferenceComparer<T> : IEqualityComparer<T>
    {
        public bool Equals(T x, T y)
        {
            return RuntimeHelpers.Equals(x,y);
        }

        public int GetHashCode(T obj)
        {
            return RuntimeHelpers.GetHashCode(obj);
        }
    }
}
