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
