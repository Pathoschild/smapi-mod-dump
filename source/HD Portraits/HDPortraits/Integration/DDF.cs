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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace HDPortraits.Integration
{
	[ModInit(WhenHasMod = "aedenthorn.DialogueDisplayFramework")]
	internal class DDF
	{
		internal static void Init()
		{
			var target = Reflection.TypeNamed("DialogueDisplayFramework.ModEntry")?
				.GetNestedType("DialogueBox_drawPortrait_Patch")
				.MethodNamed("Prefix");

			if (target is null)
				return;

			ModEntry.monitor.Log("Patching Dialogue Display Framework...");
			ModEntry.harmony.Patch(target, transpiler: new(typeof(DDF).MethodNamed(nameof(Patch))));
		}

		private static float ReplacePortrait(bool tile, bool empty, Dialogue dlg, ref Rectangle region, ref Texture2D tex)
		{
			var meta = PortraitDrawPatch.currentMeta.Value;
			if (tile && empty && meta is not null)
			{
				if (meta.TryGetTexture(out var rtex))
					tex = rtex;
				region = meta.GetRegion(dlg.currentDialogueIndex, Game1.currentGameTime.ElapsedGameTime.Milliseconds);
				return 64f / region.Width;
			}
			return 1f;
		}
		private static IEnumerable<CodeInstruction> Patch(IEnumerable<CodeInstruction> source, ILGenerator gen)
			=> patcher.Run(source, gen);

		private static ILHelper patcher = new ILHelper(ModEntry.monitor, "DDF")
			.SkipTo(new CodeInstruction[]
			{
				new(OpCodes.Ldsfld, typeof(Game1).FieldNamed(nameof(Game1.random))),
				new(OpCodes.Ldc_I4_M1),
				new(OpCodes.Ldc_I4_2),
				new(OpCodes.Callvirt, typeof(Random).MethodNamed(nameof(Random.Next), new[]{typeof(int), typeof(int)})),
				new(OpCodes.Stloc_S, 26)
			})
			.Skip(5)
			.Add(new CodeInstruction[]
			{
				new(OpCodes.Ldloc_S, 4),
				new(OpCodes.Ldfld, Reflection.TypeNamed("DialogueDisplayFramework.PortraitData").FieldNamed("tileSheet")),
				new(OpCodes.Ldloc_S, 4),
				new(OpCodes.Ldfld, Reflection.TypeNamed("DialogueDisplayFramework.PortraitData").FieldNamed("texturePath")),
				new(OpCodes.Call, typeof(String).MethodNamed(nameof(String.IsNullOrEmpty))),
				new(OpCodes.Ldarg_0),
				new(OpCodes.Ldfld, typeof(DialogueBox).FieldNamed(nameof(DialogueBox.characterDialogue))),
				new(OpCodes.Ldloca_S, 25),
				new(OpCodes.Ldloca_S, 24),
				new(OpCodes.Call, typeof(DDF).MethodNamed(nameof(ReplacePortrait)))
			})
			.StoreLocal("scale", typeof(float))
			.SkipTo(new CodeInstruction[]
			{
				new(OpCodes.Ldloc_S, 4),
				new(OpCodes.Ldfld, Reflection.TypeNamed("DialogueDisplayFramework.BaseData").FieldNamed("scale"))
			})
			.Skip(2)
			.LoadLocal("scale")
			.Add(new CodeInstruction[]
			{
				new(OpCodes.Mul)
			})
			.Finish();
	}
}
