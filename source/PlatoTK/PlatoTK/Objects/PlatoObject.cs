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
using System;

namespace PlatoTK.Objects
{
    public abstract class PlatoObject<TLink> : IPlatoObject
        where TLink : class
    {
        protected TLink Base => Link.GetAs<TLink>();

        public ILink Link { protected get; set; }

        protected IPlatoHelper Helper { get; private set; }

        const string IdentifierKey = "PlatoObject";

        public static void SetIdentifier(StardewValley.Item item, Type type)
        {
#if ANDROID
            item.netName.Value = item.GetType().Name;
#else
            if (item.modDataForSerialization.ContainsKey(IdentifierKey))
                item.modDataForSerialization[IdentifierKey] = type.FullName;
            else
                item.modDataForSerialization.Add(IdentifierKey, type.FullName);
#endif
        }

        public static bool CanLinkWith(StardewValley.Item linkedObject, object platoObject)
        {
#if ANDROID
            if (linkedObject is TLink && linkedObject.netName.Value == platoObject.GetType().Name)
                return true;
#else
            if (linkedObject is TLink && linkedObject.modDataForSerialization.ContainsKey(IdentifierKey) && linkedObject.modDataForSerialization[IdentifierKey] == platoObject.GetType().FullName)
                return true;
#endif

            return false;
        }

        public virtual bool CanLinkWith(object linkedObject)
        {
            if(linkedObject is StardewValley.Item item)
                return CanLinkWith(item, this);

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
