/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tlitookilakin/HDPortraits
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;
using System.Collections.Generic;
using System.Reflection.Emit;
using AeroCore;
using AeroCore.Utils;

namespace HDPortraits.Patches
{
	[HarmonyPatch(typeof(ShopMenu))]
	internal class ShopPatch
	{
		[HarmonyPatch("setUpShopOwner")]
		[HarmonyPostfix]
		[HarmonyPriority(Priority.Last)]
		internal static void Init(ShopMenu __instance, string who)
		{
			var npc = __instance.portraitPerson;
			var name = DialoguePatch.GetTextureNameSync(npc, out var has_suffix);

			if (who is null && name is null)
				return;

			name ??= NPC.getTextureNameForCharacter(who);

			if (name.Length is 0)
				ModEntry.monitor.Log("Could not retrieve portrait name for nameless NPC!", StardewModdingAPI.LogLevel.Warn);

			string suffix = npc is not null && !has_suffix ? PortraitDrawPatch.GetSuffix(npc) : null;

			if (ModEntry.TryGetMetadata(name, suffix, out var meta))
			{
				PortraitDrawPatch.lastLoaded.Value.Add(meta);
				PortraitDrawPatch.currentMeta.Value = meta;
				meta.Animation?.Reset();
			}
		}

		[HarmonyPatch("draw")]
		[HarmonyTranspiler]
		internal static IEnumerable<CodeInstruction> drawPatch(IEnumerable<CodeInstruction> instructions, ILGenerator gen)
			=> drawPatcher.Run(instructions, gen);

		internal static Rectangle GetData()
		{
			var current = PortraitDrawPatch.currentMeta.Value;
			if (current is null)
				return new(0, 0, 64, 64);
			return current.GetRegion(0, Game1.currentGameTime.ElapsedGameTime.Milliseconds);
		}

		internal static ILHelper drawPatcher = new ILHelper(ModEntry.monitor, "Shop draw")
			.SkipTo(new CodeInstruction[]
			{
				new(OpCodes.Ldarg_1),
				new(OpCodes.Ldarg_0),
				new(OpCodes.Ldfld, typeof(ShopMenu).FieldNamed("portraitPerson")),
				new(OpCodes.Callvirt, typeof(NPC).MethodNamed("get_Portrait"))
			})
			.Skip(4)
			.Add(new CodeInstruction(OpCodes.Call,typeof(PortraitDrawPatch).MethodNamed("SwapTexture")))
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
