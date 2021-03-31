/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Harmony;

namespace ImJustMatt.Common.Patches
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    internal class AssemblyPatch
    {
        private readonly Assembly _assembly;

        public AssemblyPatch(string name) : this(a => a.FullName.StartsWith($"{name},"))
        {
        }

        public AssemblyPatch(Func<Assembly, bool> matcher)
        {
            _assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(matcher);
        }

        public AssemblyPatchType Type(string name)
        {
            return _assembly != null ? new AssemblyPatchType(_assembly.GetType(name)) : null;
        }

        public MethodInfo Method(string type, string method)
        {
            return Type(type).Method(method);
        }

        internal class AssemblyPatchType
        {
            private readonly Type _type;

            internal AssemblyPatchType(Type type)
            {
                _type = type;
            }

            public MethodInfo Method(string name)
            {
                return AccessTools.Method(_type, name);
            }

            public MethodInfo Method(Func<MethodInfo, bool> matcher)
            {
                return AccessTools.GetDeclaredMethods(_type).FirstOrDefault(matcher);
            }
        }
    }
}