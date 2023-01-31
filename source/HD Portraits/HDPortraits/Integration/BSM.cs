/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tlitookilakin/HDPortraits
**
*************************************************/

using AeroCore;
using AeroCore.Utils;
using HarmonyLib;
using HDPortraits.Patches;
using StardewValley.Menus;
using StardewValley;
using System.Collections.Generic;
using System.Reflection.Emit;
using Microsoft.Xna.Framework;

namespace HDPortraits.Integration
{
	[ModInit(WhenHasMod = "spacechase0.BetterShopMenu")]
	internal class BSM
	{
		internal static void Init()
		{
			var target = Reflection.TypeNamed("BetterShopMenu.Mod").MethodNamed("DrawGridLayout");

			if (target is null)
				return;

			ModEntry.monitor.Log("Patching Better Shop Menu...");
			ModEntry.harmony.Patch(target, transpiler: new(typeof(BSM).MethodNamed(nameof(Patch))));
		}

		private static IEnumerable<CodeInstruction> Patch(IEnumerable<CodeInstruction> source, ILGenerator gen)
			=> patcher.Run(source, gen);

		private static ILHelper patcher = new ILHelper(ModEntry.monitor, "BSM")
			.SkipTo(new CodeInstruction[]
			{
				new(OpCodes.Ldarg_1),
				new(OpCodes.Ldloc_0),
				new(OpCodes.Ldfld, typeof(ShopMenu).FieldNamed("portraitPerson")),
				new(OpCodes.Callvirt, typeof(NPC).MethodNamed("get_Portrait"))
			})
			.Skip(4)
			.Add(new CodeInstruction(OpCodes.Call, typeof(PortraitDrawPatch).MethodNamed("SwapTexture")))
			.SkipTo(new CodeInstruction[] {
				new(OpCodes.Ldc_I4_0),
				new(OpCodes.Ldc_I4_0),
				new(OpCodes.Ldc_I4_S, 64),
				new(OpCodes.Ldc_I4_S, 64),
			})
			.Remove(5)
			.Add(new CodeInstruction[] {
				new(OpCodes.Call, typeof(ShopPatch).MethodNamed("GetData")),
				new(OpCodes.Dup)
			})
			.StoreLocal("region", typeof(Rectangle))
			.SkipTo(new CodeInstruction(OpCodes.Ldc_R4, 4f))
			.Remove(1)
			.Add(new CodeInstruction[]
			{
				new(OpCodes.Ldc_R4, 256f)
			})
			.LoadLocal("region")
			.Add(new CodeInstruction[]{ // (64 / n) * 4; 64 is default size. s = 256 / n
				new(OpCodes.Ldfld, typeof(Rectangle).FieldNamed(nameof(Rectangle.Width))),
				new(OpCodes.Conv_R4),
				new(OpCodes.Div)
			})
			.Finish();
	}
}
