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

namespace PlatoTK.Reflection
{
    internal class PrivateProperties : IPrivateFields
    {
        private object Target;

        public object this[string name]
        {
            get
            {
                return Target.GetPropertyValue(name, Target is Type);
            }
            set
            {
                Target.SetPropertyValue(value, name, Target is Type);
            }
        }

        public PrivateProperties(object target)
        {
            Target = target;
        }

        public T Get<T>(string name)
        {
            if (this[name] is T value)
                return value;

            return default;
        }

        public void Set<T>(string name, T value)
        {
            this[name] = value;
        }
    }
}
