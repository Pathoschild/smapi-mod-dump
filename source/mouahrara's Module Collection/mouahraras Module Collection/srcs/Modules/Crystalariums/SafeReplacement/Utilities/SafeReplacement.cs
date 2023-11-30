/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mouahrara/mouahrarasModuleCollection
**
*************************************************/

using StardewModdingAPI.Utilities;
using StardewValley;

namespace mouahrarasModuleCollection.Crystalariums.SafeReplacement.Utilities
{
	internal class SafeReplacementUtility
	{
		private static readonly PerScreen<Item> objectToRecover = new(() => null);

		internal static void Reset()
		{
			SetObjectToRecover(null);
		}

		internal static void SetObjectToRecover(Item value)
		{
			objectToRecover.Value = value;
		}

		internal static Item GetObjectToRecover()
		{
			return objectToRecover.Value;
		}
	}
}
