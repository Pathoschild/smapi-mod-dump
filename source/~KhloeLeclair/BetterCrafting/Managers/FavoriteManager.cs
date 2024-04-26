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
using System.Diagnostics.CodeAnalysis;

using Leclair.Stardew.BetterCrafting.Models;
using Leclair.Stardew.Common.Events;
using Leclair.Stardew.Common.Types;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using StardewValley;

namespace Leclair.Stardew.BetterCrafting.Managers;

public class FavoriteManager : BaseManager {

	private Favorites? UserFavorites;
	private bool Modified;

	#region Lifecycle

	public FavoriteManager(ModEntry mod) : base(mod) { }

	#endregion Lifecycle

	#region Events

	[Subscriber]
	[EventPriority(EventPriority.Low)]
	public void OnSaveLoaded(object? sender, SaveLoadedEventArgs e) {
		LoadFavorites();
	}

	#endregion

	#region Save and Loading

	// TODO: Store favorites in the save data, which would require
	// multiplayer packets to sync with remote clients.

	public bool DoesHaveSaveFavorites() {
		if (string.IsNullOrEmpty(Constants.SaveFolderName))
			return false;

		Favorites? data;
		string path = $"savedata/favorites/{Constants.SaveFolderName}.json";

		try {
			data = Mod.Helper.Data.ReadJsonFile<Favorites>(path);
		} catch (Exception ex) {
			Log($"The {path} file is invalid or corrupt.", LogLevel.Error, ex);
			return false;
		}

		return data?.Cooking is not null && data?.Crafting is not null;
	}

	public void LoadFavorites() {
		if (string.IsNullOrEmpty(Constants.SaveFolderName))
			return;

		Favorites? data;
		string path = Mod.UseGlobalSave
			? $"savedata/favorites.json"
			: $"savedata/favorites/{Constants.SaveFolderName}.json";

		try {
			data = Mod.Helper.Data.ReadJsonFile<Favorites>(path);
		} catch (Exception ex) {
			Log($"The {path} file is invalid or corrupt.", LogLevel.Error, ex);
			data = new();
		}

		data ??= new Favorites();

		data.Cooking ??= new();
		data.Crafting ??= new();

		UserFavorites = data;
		Modified = false;
	}

	public void SaveFavorites() {
		if (string.IsNullOrEmpty(Constants.SaveFolderName) || UserFavorites == null || !Modified)
			return;

		string path = Mod.UseGlobalSave
			? $"savedata/favorites.json"
			: $"savedata/favorites/{Constants.SaveFolderName}.json";

		try {
			Mod.Helper.Data.WriteJsonFile(path, UserFavorites);
		} catch (Exception ex) {
			Log($"There was an error saving favorites to {path}", LogLevel.Error, ex);
		}

		Modified = false;
	}

	#endregion

	#region Favorites

	[MemberNotNull(nameof(UserFavorites))]
	private void AssertLoaded() {
		if (UserFavorites == null)
			throw new InvalidOperationException("Favorites have not been loaded yet.");
	}

	[MemberNotNullWhen(true, nameof(UserFavorites))]
	public bool IsLoaded() {
		return UserFavorites != null;
	}

	private CaseInsensitiveHashSet GetFavoriteRecipes(Farmer who, bool cooking) {
		long id = who.UniqueMultiplayerID;
		CaseInsensitiveHashSet? result;
		if (cooking)
			UserFavorites!.Cooking.TryGetValue(id, out result);
		else
			UserFavorites!.Crafting.TryGetValue(id, out result);

		if (result == null) {
			result = new();
			if (cooking)
				UserFavorites!.Cooking.Add(id, result);
			else
				UserFavorites!.Crafting.Add(id, result);
		}

		return result;
	}

	public bool IsFavoriteRecipe(string name, bool cooking, Farmer? who = null) {
		AssertLoaded();
		who ??= Game1.player;

		lock (UserFavorites) {
			CaseInsensitiveHashSet favorites = GetFavoriteRecipes(who, cooking);
			return favorites.Contains(name);
		}
	}

	public void SetFavoriteRecipe(string name, bool cooking, bool favorited, Farmer? who = null) {
		AssertLoaded();
		who ??= Game1.player;

		lock (UserFavorites) {
			CaseInsensitiveHashSet favorites = GetFavoriteRecipes(who, cooking);
			if (favorited)
				favorites.Add(name);
			else
				favorites.Remove(name);

			Modified = true;
		}
	}

	public void ToggleFavoriteRecipe(string name, bool cooking, Farmer? who = null) {
		AssertLoaded();
		who ??= Game1.player;

		lock (UserFavorites) {
			CaseInsensitiveHashSet favorites = GetFavoriteRecipes(who, cooking);
			if (favorites.Contains(name))
				favorites.Remove(name);
			else
				favorites.Add(name);

			Modified = true;
		}
	}

	#endregion

}
