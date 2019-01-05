using StardewValley.Menus;
using System;
using System.Collections.Generic;

namespace BattleRoyale
{
	class SocialTabRemoveNPCs : Patch
	{
		protected override PatchDescriptor GetPatchDescriptor() => new PatchDescriptor(typeof(SocialPage), "", new Type[] { typeof(int), typeof(int) , typeof(int) , typeof(int) });

		public static void Postfix(SocialPage __instance)
		{
			var r = ModEntry.BRGame.ModHelper.Reflection;
			var names = r.GetField<List<object>>(__instance, "names").GetValue();
			var sprites = r.GetField<List<ClickableTextureComponent>>(__instance, "sprites").GetValue();

			for (int i = names.Count - 1; i >= 0; i--)
			{
				if (names[i] is string)//NPCs are string, players are long
				{
					names.RemoveAt(i);
					sprites.RemoveAt(i);
				}
			}
		}
	}
}
