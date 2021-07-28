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
    public interface ILinked
    {
        ILink Link {set;}
        bool CanLinkWith(object linkedObject);
        void OnLink(IPlatoHelper helper, object linkedObject);
        void OnUnLink(IPlatoHelper helper, object linkedObject);
    }

}
