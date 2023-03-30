/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/PlatoTK
**
*************************************************/

using System.Collections.Generic;
using System.Reflection;

namespace PlatoUI.Reflection
{
    public interface IPrivateMethods
    {
        IEnumerable<MethodInfo> this[string name] { get; }

        T CallMethod<T>(string name, params object[] args);
        void CallMethod(string name, params object[] args);

    }
}
