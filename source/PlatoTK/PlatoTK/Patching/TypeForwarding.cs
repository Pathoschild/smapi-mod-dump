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

namespace PlatoTK.Patching
{
    internal class TypeForwarding
    {
        internal readonly Type FromType;
        internal readonly Type ToType;
        internal readonly IPlatoHelper Helper;
        internal readonly object TargetForAllInstances;

        public TypeForwarding(Type fromType, Type toType, IPlatoHelper helper, object targetForAllInstances = null)
        {
            FromType = fromType;
            ToType = toType;
            Helper = helper;
            TargetForAllInstances = targetForAllInstances;
        }
    }
}
