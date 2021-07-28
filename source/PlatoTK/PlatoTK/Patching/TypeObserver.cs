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

namespace PlatoTK.Patching
{
    internal class TypeObserver
    {
        public readonly string Id;

        public readonly Type Type;

        public readonly Delegate Observer;

        public TypeObserver(string id, Type type, Delegate observer)
        {
            Id = id;
            Type = type;
            Observer = observer;
        }
    }
}
