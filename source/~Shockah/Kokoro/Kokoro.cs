/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using Nanoray.Shrike;
using Nanoray.Shrike.Harmony;
using Shockah.Kokoro.Stardew;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;
using System.Collections.Generic;
using System.Linq;

namespace Shockah.Kokoro;

public class Kokoro : BaseMod
{
	public static Kokoro Instance { get; private set; } = null!;

	private readonly PerScreen<LinkedList<string>> QueuedObjectDialogue = new(() => new());
	private Dictionary<long, SaveFileDescriptor> SaveFileDescriptors { get; init; } = new();

	public override void Entry(IModHelper helper)
	{
		Instance = this;

		// force-referencing Shrike assemblies, otherwise none dependent mods will load
		_ = typeof(ISequenceMatcher<CodeInstruction>).Name;
		_ = typeof(ILMatches).Name;

		helper.Events.GameLoop.GameLaunched += OnGameLaunched;
		helper.Events.GameLoop.SaveCreated += OnSaveCreated;
		helper.Events.GameLoop.Saving += OnSaving;
		helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
		helper.Events.Display.RenderedActiveMenu += OnRenderedActiveMenu;
	}

	private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
	{
		var descriptorsModel = Helper.Data.ReadGlobalData<SaveFilesModel>($"{ModManifest.UniqueID}.SaveFileDescriptors");
		if (descriptorsModel is not null)
			foreach (var entry in descriptorsModel.Entries)
				SaveFileDescriptors[entry.PlayerID] = entry.Descriptor;

		var harmony = new Harmony(ModManifest.UniqueID);
		LoadGameMenuPatches.Apply(harmony);
		FarmTypeManagerPatches.Apply(harmony);
        MachineTracker.Setup(Monitor, Helper, harmony);
	}

	private void OnSaveCreated(object? sender, SaveCreatedEventArgs e)
		=> UpdateSaveFileDescriptor(Game1.player);

	private void OnSaving(object? sender, SavingEventArgs e)
	{
		if (Context.IsMainPlayer)
			UpdateSaveFileDescriptor(Game1.player);
	}

	private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
	{
		if (Game1.gameMode == Game1.titleScreenGameMode)
		{
			GameTime gameTime = new(new((long)(e.Ticks / 60.0 * 10000000.0)), new((long)(1 / 60.0 * 10000000.0)));
			for (int i = Game1.hudMessages.Count - 1; i >= 0; i--)
				if (Game1.hudMessages.ElementAt(i).update(gameTime))
					Game1.hudMessages.RemoveAt(i);
		}

		if (Context.IsWorldReady)
		{
			// dequeue object dialogue
			var message = QueuedObjectDialogue.Value.First;
			if (message is not null && Game1.activeClickableMenu is not DialogueBox)
			{
				QueuedObjectDialogue.Value.RemoveFirst();
				Game1.drawObjectDialogue(message.Value);
			}
		}
    }

	private void OnRenderedActiveMenu(object? sender, RenderedActiveMenuEventArgs e)
	{
		if (Game1.gameMode != Game1.titleScreenGameMode)
			return;
		for (int i = Game1.hudMessages.Count - 1; i >= 0; i--)
			Game1.hudMessages[i].draw(Game1.spriteBatch, i);
	}

	public void QueueObjectDialogue(string message)
	{
		if (Game1.activeClickableMenu is DialogueBox)
			QueuedObjectDialogue.Value.AddLast(message);
		else
			Game1.drawObjectDialogue(message);
	}

	internal SaveFileDescriptor? GetSaveFileDescriptor(Farmer player)
		=> SaveFileDescriptors.TryGetValue(player.UniqueMultiplayerID, out var descriptor) ? descriptor : null;

	internal void UpdateSaveFileDescriptor(Farmer player)
	{
		SaveFileDescriptors[player.UniqueMultiplayerID] = SaveFileDescriptor.CreateFromCurrentState();
		FlushSaveFileDescriptor();
	}

	private void FlushSaveFileDescriptor()
	{
		SaveFilesModel model = new();
		foreach (var (key, descriptor) in SaveFileDescriptors)
			model.Entries.Add(new(key, descriptor));
		Helper.Data.WriteGlobalData($"{ModManifest.UniqueID}.SaveFileDescriptors", model);
	}
}