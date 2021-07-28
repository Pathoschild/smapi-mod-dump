/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/PlatoTK
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatoTK.Content
{
    internal abstract class ObjectInjection : AssetInjection<object>
    {
        public ObjectInjection(
            IPlatoHelper helper,
            string assetName,
            object value,
            InjectionMethod method,
            string conditions = "")
            : base(helper, assetName, value, method, conditions)
        {
        }
    }

    internal class ObjectInjection<T> : ObjectInjection
    {
        public ObjectInjection(
            IPlatoHelper helper,
            string assetName,
            T value,
            InjectionMethod method,
            string conditions = "")
            : base(helper, assetName, value, method, conditions)
        {
        }
    }
}
