/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/PlatoTK
**
*************************************************/

namespace PlatoTK.Patching
{
    internal class TracedObject
    {
        internal readonly object Original;

        internal readonly object Target;

        internal readonly IPlatoHelper Helper;

        public TracedObject(object original, object target, IPlatoHelper helper)
        {
            Original = original;
            Target = target;
            Helper = helper;
        }
    }
}
