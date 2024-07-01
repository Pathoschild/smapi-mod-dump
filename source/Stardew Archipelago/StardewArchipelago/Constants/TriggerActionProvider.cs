/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System.Reflection;

namespace StardewArchipelago.Constants
{
    public static class TriggerActionProvider
    {
        public static readonly string TRAVELING_MERCHANT_PURCHASE = CreateId("TravelingMerchantPurchase");


        private static string CreateId(string name)
        {
            return $"{ModEntry.Instance.ModManifest.UniqueID}.TriggerAction.{name}";
        }

        private static string CreateId(MemberInfo t)
        {
            return CreateId(t.Name);
        }

        private static string CreateId<T>()
        {
            return CreateId(typeof(T));
        }
    }
}
