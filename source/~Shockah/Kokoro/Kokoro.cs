/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley.Menus;
using StardewValley;
using System.Collections.Generic;
using StardewModdingAPI.Events;
using Shockah.Kokoro.Stardew;
using HarmonyLib;
using Nanoray.Shrike;
using Nanoray.Shrike.Harmony;

namespace Shockah.Kokoro
{
	public class Kokoro : BaseMod
	{
		public static Kokoro Instance { get; private set; } = null!;

		private PerScreen<LinkedList<string>> QueuedObjectDialogue { get; init; } = new(() => new());

		public override void Entry(IModHelper helper)
		{
			Instance = this;

			// force-referencing Shrike assemblies, otherwise none dependent mods will load
			_ = typeof(ISequenceMatcher<CodeInstruction>).Name;
			_ = typeof(ILMatches).Name;

			helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
			MachineTracker.Setup(Monitor, helper, new Harmony(ModManifest.UniqueID));
		}

		private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
		{
			// dequeue object dialogue
			var message = QueuedObjectDialogue.Value.First;
			if (message is not null && Game1.activeClickableMenu is not DialogueBox)
			{
				QueuedObjectDialogue.Value.RemoveFirst();
				Game1.drawObjectDialogue(message.Value);
			}
		}

		public void QueueObjectDialogue(string message)
		{
			if (Game1.activeClickableMenu is DialogueBox)
				QueuedObjectDialogue.Value.AddLast(message);
			else
				Game1.drawObjectDialogue(message);
		}
	}
}