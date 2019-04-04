using System;
using System.Reflection;
using Harmony;
using MultiplayerEmotes.Extensions;
using StardewModdingAPI;
using StardewValley;

namespace MultiplayerEmotes.Framework.Patches {

	internal static class CharacterPatch {

		internal class DoEmotePatch : ClassPatch {

			public override MethodInfo Original => AccessTools.Method(typeof(Character), nameof(Character.doEmote), new Type[] { typeof(int), typeof(bool), typeof(bool) });
			public override MethodInfo Postfix => AccessTools.Method(this.GetType(), nameof(DoEmotePatch.DoEmote_Postfix));

			private static IReflectionHelper Reflection;

			public DoEmotePatch(IReflectionHelper reflection) {
				Reflection = reflection;
			}

			private static void DoEmote_Postfix(Character __instance, int whichEmote) {
#if DEBUG
				ModEntry.ModMonitor.Log($"Character emote ({__instance.GetType()})", LogLevel.Trace);
#endif
				if(Context.IsMultiplayer && !Game1.eventUp && !(__instance is Farmer)) {
#if DEBUG
					ModEntry.ModMonitor.Log($"Character broadcasting emote.", LogLevel.Trace);
#endif
					// Traverse.Create(typeof(Game1)).Field("multiplayer").GetValue<Multiplayer>().BroadcastEmote(whichEmote);
					Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue().BroadcastEmote(whichEmote, __instance);
				}
			}

		}

	}
}
