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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace HDPortraits.Integration
{
	// TODO: fix help wanted patch
	//[ModInit(WhenHasMod = "aedenthorn.HelpWanted")]
	internal class HelpWanted
	{
		internal static void Init()
		{
			var target = Reflection.TypeNamed("HelpWanted.OrdersBillboard").MethodNamed("draw", new[] {typeof(SpriteBatch)});

			if (target is null)
				return;

			ModEntry.monitor.Log("Patching Help Wanted...");
			ModEntry.harmony.Patch(target, transpiler: new(typeof(HelpWanted).MethodNamed(nameof(Patch))));
		}

		internal static IEnumerable<CodeInstruction> Patch(IEnumerable<CodeInstruction> source, ILGenerator gen)
		/*{
			var sb = new StringBuilder();
			foreach(var code in patcher.Run(source, gen))
			{
				yield return code;
				sb.AppendLine(code.ToString());
			}
			ModEntry.monitor.Log(sb.ToString());
		}*/
			=> patcher.Run(source, gen);

		private static Type dictType = typeof(Dictionary<,>).MakeGenericType(typeof(int), Reflection.TypeNamed("HelpWanted.IQuestData"));

		private static ILHelper patcher = new ILHelper(ModEntry.monitor, "Help Wanted")
			.SkipTo(new CodeInstruction[]
			{
				new(OpCodes.Ldsfld, Reflection.TypeNamed("HelpWanted.OrdersBillboard").FieldNamed("questDict")),
				new(OpCodes.Ldloc_S, 4),
				new(OpCodes.Ldfld, typeof(ClickableComponent).FieldNamed(nameof(ClickableComponent.myID))),
				new(OpCodes.Callvirt, dictType.MethodNamed("get_Item")),
				new(OpCodes.Callvirt, Reflection.TypeNamed("HelpWanted.IQuestData").PropertyGetter("icon"))
			})
			.Add(new CodeInstruction[] //load default source
			{
				new(OpCodes.Ldsfld, Reflection.TypeNamed("HelpWanted.OrdersBillboard").FieldNamed("questDict")),
				new(OpCodes.Ldloc_S, 4),
				new(OpCodes.Ldfld, typeof(ClickableComponent).FieldNamed(nameof(ClickableComponent.myID))),
				new(OpCodes.Callvirt, dictType.MethodNamed("get_Item")),
				new(OpCodes.Callvirt, Reflection.TypeNamed("HelpWanted.IQuestData").PropertyGetter("iconSource"))
			})
			.StoreLocal("source", typeof(Rectangle))
			.Skip(5)
			.Add(new CodeInstruction[]
			{
				new(OpCodes.Call, typeof(HelpWanted).MethodNamed(nameof(check)))
			})
			.StoreLocal("tex", typeof(Texture2D))
			.LoadLocal("tex", true)
			.LoadLocal("source", true)
			.Add(new CodeInstruction[]
			{
				new(OpCodes.Call, typeof(HelpWanted).MethodNamed(nameof(SwapMetadata)))
			})
			.StoreLocal("scale", typeof(float))
			.LoadLocal("tex")
			.SkipTo(new CodeInstruction[] // get source
			{
				new(OpCodes.Ldsfld, Reflection.TypeNamed("HelpWanted.OrdersBillboard").FieldNamed("questDict")),
				new(OpCodes.Ldloc_S, 4),
				new(OpCodes.Ldfld, typeof(ClickableComponent).FieldNamed(nameof(ClickableComponent.myID))),
				new(OpCodes.Callvirt, dictType.MethodNamed("get_Item")),
				new(OpCodes.Callvirt, Reflection.TypeNamed("HelpWanted.IQuestData").PropertyGetter("iconSource"))
			})
			.Remove(5)
			.LoadLocal("source")
			.SkipTo(new CodeInstruction[] // get scale
			{
				new(OpCodes.Ldsfld, Reflection.TypeNamed("HelpWanted.OrdersBillboard").FieldNamed("questDict")),
				new(OpCodes.Ldloc_S, 4),
				new(OpCodes.Ldfld, typeof(ClickableComponent).FieldNamed(nameof(ClickableComponent.myID))),
				new(OpCodes.Callvirt, dictType.MethodNamed("get_Item")),
				new(OpCodes.Callvirt, Reflection.TypeNamed("HelpWanted.IQuestData").PropertyGetter("iconScale"))
			})
			.Skip(5)
			.LoadLocal("scale")
			.Add(new CodeInstruction[]
			{
				new(OpCodes.Mul)
			})
			.Finish();

		private static float SwapMetadata(ref Texture2D what, ref Rectangle source)
		{
			var scale = 1f;
			if (what.Name.StartsWith("Portraits" + PathUtilities.PreferredAssetSeparator) && ModEntry.TryGetMetadata(what.Name[10..], null, out var meta))
			{
				source = meta.GetRegion(0, Game1.currentGameTime.ElapsedGameTime.Milliseconds);
				scale = 64f / source.Width;

				if (!meta.TryGetTexture(out var tex))
					what = tex;
			}
			return scale;
		}
		private static Texture2D check(Texture2D source)
		{
			ModEntry.monitor.LogOnce(source.Name, StardewModdingAPI.LogLevel.Debug);
			return source;
		}
	}
}
