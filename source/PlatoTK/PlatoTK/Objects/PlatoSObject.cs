/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/PlatoTK
**
*************************************************/

using PlatoTK.Patching;
using StardewValley;

namespace PlatoTK.Objects
{
    public abstract class PlatoSObject<TLink> : StardewValley.Object, IPlatoObject
        where TLink : Item
    {
        protected TLink Base => Link.GetAs<TLink>();

        public ILink Link { protected get; set; }

        protected IPlatoHelper Helper { get; private set; }

        public virtual bool CanLinkWith(object linkedObject)
        {
            if (linkedObject is StardewValley.Item item)
                return PlatoObject<TLink>.CanLinkWith(item, this);

            return false;
        }

        public virtual void OnConstruction(IPlatoHelper helper, object linkedObject)
        {
            Helper = helper;
        }

        public virtual void OnLink(IPlatoHelper helper, object linkedObject)
        {
        }

        public virtual void OnUnLink(IPlatoHelper helper, object linkedObject)
        {
        }

        public virtual void Dispose()
        {
            Link.Unlink();
        }
    }
}
