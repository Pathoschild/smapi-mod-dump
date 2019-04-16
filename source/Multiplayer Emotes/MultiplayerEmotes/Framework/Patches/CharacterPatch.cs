using System;
using System.Reflection;
using Harmony;
using MultiplayerEmotes.Extensions;
using StardewModdingAPI;
using StardewValley;

namespace MultiplayerEmotes.Framework.Patches {

	internal static class CharacterPatch {

		internal sealed class DoEmotePatch : ClassPatch {

			public override MethodInfo Original { get; } = AccessTools.Method(typeof(Character), nameof(Character.doEmote), new[] { typeof(int), typeof(bool), typeof(bool) });
			public override MethodInfo Postfix { get; } = AccessTools.Method(typeof(DoEmotePatch), nameof(DoEmotePatch.DoEmote_Postfix));

			private static IReflectionHelper Reflection { get; set; }

			public static DoEmotePatch Instance { get; } = new DoEmotePatch();

			// Explicit static constructor to avoid the compiler to mark type as beforefieldinit
			static DoEmotePatch() { }

			private DoEmotePatch() { }

			public static DoEmotePatch CreatePatch(IReflectionHelper reflection) {
				Reflection = reflection;
				return Instance;
			}

			private static void DoEmote_Postfix(Character __instance, int whichEmote) {

#if DEBUG
				ModEntry.ModMonitor.Log($"DoEmote_Postfix (enabled: {Instance.PostfixEnabled})", LogLevel.Trace);
#endif
				if(!Instance.PostfixEnabled) {
					return;
				}

#if DEBUG
				ModEntry.ModMonitor.Log($"Character emote ({__instance.GetType()})", LogLevel.Trace);
#endif

				if(!Context.IsMultiplayer || Game1.eventUp || __instance is Farmer) {
					return;
				}

#if DEBUG
				ModEntry.ModMonitor.Log("Character broadcasting emote.", LogLevel.Trace);
#endif
				// Traverse.Create(typeof(Game1)).Field("multiplayer").GetValue<Multiplayer>().BroadcastEmote(whichEmote);
				Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue().BroadcastEmote(whichEmote, __instance);

			}

		}

	}
}
