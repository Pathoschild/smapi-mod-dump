
using System;
using System.Reflection;
using Harmony;
using MultiplayerEmotes.Extensions;
using StardewModdingAPI;
using StardewValley;

namespace MultiplayerEmotes.Framework.Patches {

	internal static class FarmerPatch {

		internal class DoEmotePatch : ClassPatch {

			public override MethodInfo Original => AccessTools.Method(typeof(Farmer), nameof(Farmer.doEmote), new Type[] { typeof(int) });
			public override MethodInfo Postfix => AccessTools.Method(this.GetType(), nameof(DoEmotePatch.DoEmote_Postfix));

			private static IReflectionHelper Reflection;

			public DoEmotePatch(IReflectionHelper reflection) {
				Reflection = reflection;
			}

			private static void DoEmote_Postfix(Farmer __instance, int whichEmote) {
#if DEBUG
				ModEntry.ModMonitor.Log($"Farmer emote ({__instance.GetType()})", LogLevel.Trace);
#endif
				if(Context.IsMultiplayer && __instance.IsLocalPlayer) {
#if DEBUG
					ModEntry.ModMonitor.Log($"Farmer broadcasting emote.", LogLevel.Trace);
#endif
					// Traverse.Create(typeof(Game1)).Field("multiplayer").GetValue<Multiplayer>().BroadcastEmote(whichEmote);
					Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue().BroadcastEmote(whichEmote, __instance);
				}
			}

		}

	}

}
