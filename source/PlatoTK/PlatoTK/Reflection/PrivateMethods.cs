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
using System.Reflection;

namespace PlatoTK.Reflection
{
    internal class PrivateMethods : IPrivateMethods
    {
        private object Target;

        public IEnumerable<MethodInfo> this[string name]
        {
            get
            {
                return (Target is Type t ? t : Target?.GetType())?.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | (Target is Type ? BindingFlags.Static : BindingFlags.Instance))
                    .Where(m => m.Name == name);
            }
        }

        public PrivateMethods(object target)
        {
            Target = target;
        }

        public void CallMethod(string name, params object[] args)
        {
            Target.CallAction(name, args);
        }

        public T CallMethod<T>(string name, params object[] args)
        {
            return Target.CallFunction<T>(name, args);
        }
    }
}
