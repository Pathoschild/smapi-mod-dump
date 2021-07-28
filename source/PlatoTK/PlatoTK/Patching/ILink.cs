/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/PlatoTK
**
*************************************************/

using PlatoTK.Reflection;
using System;

namespace PlatoTK.Patching
{
    public interface ILink
    {
        IPrivateFields PrivateFields { get; }
        IPrivateFields PrivateProperties { get; }
        IPrivateMethods PrivateMethods { get; }
        T GetAs<T>() where T : class;
        void Unlink();
        void Relink();

        TReturn CallUnlinked<TLink, TReturn>(Func<TLink, TReturn> call) where TLink : class;

        void CallUnlinked<TLink>(Action<TLink> call) where TLink : class;
    }
}
