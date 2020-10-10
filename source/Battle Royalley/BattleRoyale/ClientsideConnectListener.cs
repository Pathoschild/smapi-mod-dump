/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ilyaki/BattleRoyalley
**
*************************************************/

using StardewValley.Network;

namespace BattleRoyale
{
    class ClientsideConnectListener : Patch
	{
		protected override PatchDescriptor GetPatchDescriptor() => new PatchDescriptor(typeof(Client), "receiveServerIntroduction");

		public static void Postfix(Client __instance)
		{
			new AutoKicker().SendMyVersionToTheServer(__instance);
		}
	}
}
