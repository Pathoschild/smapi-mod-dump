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
    internal class Link : ILink 
    {
        private readonly object Original;

        private readonly object Target;

        private readonly IPlatoHelper Helper;

        public IPrivateFields PrivateFields => Original?.GetPrivateFields();
        public IPrivateFields PrivateProperties => Original?.GetPrivateProperties();
        public IPrivateMethods PrivateMethods => Original?.GetPrivateMethods();

        public T GetAs<T>() where T : class
        {
            if (Original is T value)
                return value;

            return null;
        }

        public TReturn CallUnlinked<TLink,TReturn>(Func<TLink,TReturn> call) where TLink : class
        {
            Unlink();
            TReturn result = call(GetAs<TLink>());
            Relink();
            return result;
        }

        public void CallUnlinked<TLink>(Action<TLink> call) where TLink : class
        {
            Unlink();
            call(GetAs<TLink>());
            Relink();
        }

        public void Unlink()
        {
            Helper.Harmony.UnlinkObjects(Original, Target);
        }

        public void Relink()
        {
            Helper.Harmony.LinkObjects(Original, Target);
        }

        public Link(object original, object target, IPlatoHelper helper)
        {
            Original = original;
            Target = target;
            Helper = helper;
        }
    }
}
