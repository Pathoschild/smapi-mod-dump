/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

#nullable enable

using System;
using System.Collections.Generic;

using Leclair.Stardew.BetterCrafting.Menus;
using Leclair.Stardew.Common.Crafting;

using StardewModdingAPI;

using StardewValley;
using StardewValley.Menus;

namespace Leclair.Stardew.BetterCrafting.Models;

public class PerformCraftEvent : IGlobalPerformCraftEventV2 {

	public IRecipe Recipe { get; }
	public Farmer Player { get; }
	public Item? Item { get; set; }
	public IReadOnlyDictionary<IIngredient, List<Item>> MatchingItems { get; }
	public IClickableMenu Menu { get; }

	public bool IsDone { get; private set; }
	public bool Success { get; private set; }

	public Action? OnDone { get; internal set; }

	public PerformCraftEvent(IRecipe recipe, Farmer who, Item? item, IReadOnlyDictionary<IIngredient, List<Item>> matchingItems, IClickableMenu menu) {
		Recipe = recipe;
		Player = who;
		Item = item;
		MatchingItems = matchingItems;
		Menu = menu;
	}

	public void Cancel() {
		if (!IsDone) {
			IsDone = true;
			Success = false;
			OnDone?.Invoke();
		}
	}

	public void Complete() {
		if (!IsDone) {
			IsDone = true;
			Success = true;
			OnDone?.Invoke();
		}
	}
}


public class ChainedPerformCraftHandler {

	public readonly (IManifest?, Action<IGlobalPerformCraftEvent>)[] Handlers;

	public readonly IRecipe Recipe;
	public readonly Farmer Player;
	public readonly BetterCraftingPage Menu;

	public Item? Item;
	public IReadOnlyDictionary<IIngredient, List<Item>> MatchingItems;

	public readonly Action<ChainedPerformCraftHandler> OnDone;

	private bool finished = false;
	private bool success = true;
	private int current = 0;
	private PerformCraftEvent? currentEvent;

	public ChainedPerformCraftHandler(ModEntry mod, IRecipe recipe, Farmer who, Item? item, IReadOnlyDictionary<IIngredient, List<Item>> matchingItems, BetterCraftingPage menu, Action<ChainedPerformCraftHandler> onDone) {

		List<(IManifest?, Action<IGlobalPerformCraftEvent>)> handlers = new();

		foreach (var api in mod.APIInstances.Values)
			foreach (var hook in api.GetPerformCraftHooks())
				handlers.Add((api.Other, hook));

		handlers.Add((null, recipe.PerformCraft));
		Handlers = handlers.ToArray();

		Recipe = recipe;
		Player = who;
		Item = item;
		MatchingItems = matchingItems;
		Menu = menu;
		OnDone = onDone;

		Process();
	}

	// We're done when we've had a non-success, or we've run out of handlers.
	public bool IsDone => !success || current >= Handlers.Length;
	public bool Success => success;

	public Exception? Exception { get; private set; }
	public IManifest? ExceptionSource { get; private set; }

	private void Finish() {
		current++;
		if (currentEvent is not null) {
			Item = currentEvent.Item;
			success = currentEvent.Success;
		}

		Process();
	}

	private void Process() {

		if (IsDone) {
			if (!finished)
				OnDone(this);
			finished = true;
			return;
		}

		currentEvent = new PerformCraftEvent(Recipe, Player, Item, MatchingItems, Menu);

		try {
			Handlers[current].Item2.Invoke(currentEvent);
		} catch (Exception ex) {
			// If there's an exception, stash it for later, mark that
			// we're done, and leave.
			Exception = ex;
			ExceptionSource = Handlers[current].Item1;
			success = false;
			currentEvent = null;
			Finish();
			return;
		}

		if (currentEvent.IsDone)
			Finish();

		// We don't set this immediately to avoid weird double events.
		currentEvent.OnDone = Finish;
	}


}
