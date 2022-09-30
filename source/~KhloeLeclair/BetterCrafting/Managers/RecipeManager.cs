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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;

using Leclair.Stardew.BetterCrafting.DynamicRules;
using Leclair.Stardew.BetterCrafting.Models;
using Leclair.Stardew.Common;
using Leclair.Stardew.Common.Crafting;
using Leclair.Stardew.Common.Events;

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;

using StardewValley;

namespace Leclair.Stardew.BetterCrafting.Managers;

public class RecipeManager : BaseManager {

	// Name -> ID Conversion
	private readonly static Regex NAME_TO_ID = new(@"[^a-z0-9_]+", RegexOptions.Compiled | RegexOptions.IgnoreCase);

	// Providers
	private readonly List<IRecipeProvider> Providers = new();

	// Recipes
	// These are per-screen in case there is vanilla CraftingRecipe behavior
	// that does things separately per player. As an example, if a player has
	// the profession that makes Crab Pots cheaper, they should have the
	// cheaper recipe while the other player should now.

	private readonly PerScreen<int> CraftingCount = new(() => 0);
	private readonly PerScreen<int> CookingCount = new(() =>0);

	private readonly PerScreen<Dictionary<string, IRecipe>> CraftingRecipesByName = new(() => new());
	private readonly PerScreen<Dictionary<string, IRecipe>> CookingRecipesByName = new(() => new());

	private readonly PerScreen<List<IRecipe>> CraftingRecipes = new(() => new());
	private readonly PerScreen<List<IRecipe>> CookingRecipes = new(() => new());

	private readonly PerScreen<Dictionary<IRecipe, (List<NPC>, List<NPC>)?>> RecipeTastes = new(() => new());

	// Categories
	private Category[]? DefaultCraftingCategories;
	private Category[]? DefaultCookingCategories;
	private readonly object DefaultLock = new();

	private readonly Dictionary<long, Category[]> CraftingCategories = new();
	private readonly Dictionary<long, Category[]> CookingCategories = new();

	// Dynamic Rules
	private readonly Dictionary<string, IDynamicRuleHandler> RuleHandlers = new();
	private readonly InvalidRuleHandler invalidRuleHandler = new();

	private readonly Dictionary<long, AppliedDefaults> AppliedDefaults = new();

	private readonly AddedAPICategories API_Cooking = new();
	private readonly AddedAPICategories API_Crafting = new();

	private readonly Dictionary<string, Func<string>> API_DisplayName_Cooking = new();
	private readonly Dictionary<string, Func<string>> API_DisplayName_Crafting = new();

	public bool DefaultsLoaded = false;

	public RecipeManager(ModEntry mod) : base(mod) {
		RegisterDefaultRuleHandlers();

	}

	private static void AssertFarmer([NotNull] Farmer? who) {
		if (who == null)
			throw new ArgumentNullException(nameof(who));
	}

	#region Events

	[Subscriber]
	private void OnAssetInvalidated(object? sender, AssetsInvalidatedEventArgs e) {
		foreach(var name in e.NamesWithoutLocale) {
			if (name.IsEquivalentTo(@"Data\CraftingRecipes") || name.IsEquivalentTo(@"Data\CookingRecipes")) {
				Invalidate();
				break;
			}
		}
	}

	[Subscriber]
	private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e) {
		LoadRecipes();
		LoadCategories();
	}

	[Subscriber]
	private void OnDayStarted(object? sender, DayStartedEventArgs e) {
		Invalidate();
	}

	#endregion

	#region Lock Helpers

	private void WithRecipeProviders(Action action) {
		lock ((Providers as ICollection).SyncRoot) {
			action();
		}
	}

	private void WithRecipes(Action action) {
		lock ((CraftingRecipes.Value as ICollection).SyncRoot) {
			lock ((CookingRecipes.Value as ICollection).SyncRoot) {
				action();
			}
		}
	}

	private void WithRecipesByName(Action action) {
		lock ((CraftingRecipesByName.Value as ICollection).SyncRoot) {
			lock ((CookingRecipesByName.Value as ICollection).SyncRoot) {
				action();
			}
		}
	}

	private void WithCategories(Action action) {
		lock ((CraftingCategories as ICollection).SyncRoot) {
			lock ((CookingCategories as ICollection).SyncRoot) {
				action();
			}
		}
	}

	private void WithDefaultCategories(Action action) {
		lock (DefaultLock) {
			action();
		}
	}

	#endregion

	#region Recipe Handling

	public List<IRecipe> GetRecipes(bool cooking) {
		if (CraftingCount.Value != CraftingRecipe.craftingRecipes.Count || CookingCount.Value != CraftingRecipe.cookingRecipes.Count) {
			if (CraftingCount.Value != 0 || CookingCount.Value != 0)
				Log("Recipe count changed. Re-caching recipes.", LogLevel.Info);
			LoadRecipes();
		}

		List<IRecipe> result;

		if (cooking)
			result = CookingRecipes.Value;
		else
			result = CraftingRecipes.Value;

		bool forked = false;

		WithRecipeProviders(() => {
			foreach (var provider in Providers) {
				if (provider.CacheAdditionalRecipes)
					continue;

				var extra = provider.GetAdditionalRecipes(cooking);
				if (extra != null) {
					if (!forked) {
						result = result.ToList();
						forked = true;
					}

					result.AddRange(extra);
				}
			}
		});

#if DEBUG
		result.Add(new TestRecipe());
#endif

		return result;
	}


	#endregion

	#region Gift Tastes

	public void ClearGiftTastes() {
		RecipeTastes.Value.Clear();
	}

	public (List<NPC>, List<NPC>)? GetGiftTastes(IRecipe recipe) {
		if (RecipeTastes.Value.TryGetValue(recipe, out var cached))
			return cached;

		Item? item = recipe.CreateItemSafe();
		if (item is null || item is not StardewValley.Object sobj || sobj.bigCraftable.Value) {
			RecipeTastes.Value.Add(recipe, null);
			return null;
		}

		DisposableList<NPC> chars;
		try {
			chars = Utility.getAllCharacters();
		} catch(Exception ex) {
			Log("Unable to get character list due to error. Gift tastes will not function.", LogLevel.Warn, ex);
			return null;
		}

		List<NPC> loves = new();
		List<NPC> likes = new();

		foreach (NPC npc in chars) {
			if (!npc.CanSocialize)
				continue;

			if (!Mod.Config.ShowAllTastes && !Game1.player.hasGiftTasteBeenRevealed(npc, item.ParentSheetIndex))
				continue;

			int taste;
			try {
				taste = npc.getGiftTasteForThisItem(item);
			} catch {
				// This will error for items without a gift taste. Just ignore it.
				continue;
			}

			if (taste == NPC.gift_taste_love)
				loves.Add(npc);
			if (taste == NPC.gift_taste_like)
				likes.Add(npc);
		}

		loves.Sort((a, b) => a.displayName.CompareTo(b.displayName));
		likes.Sort((a, b) => a.displayName.CompareTo(b.displayName));

		(List<NPC>, List<NPC>)? result = (loves.Count > 0 || likes.Count > 0) ? (loves, likes) : null;

		RecipeTastes.Value.Add(recipe, result);
		return result;
	}

	#endregion

	#region Category Handling

	public string? GetCategoryDisplayName(Category category, bool cooking) {
		if (category is null)
			return null;

		if (string.IsNullOrEmpty(category.I18nKey))
			return category.Name;

		if (!string.IsNullOrEmpty(category.Id) && category.I18nKey == "api-access") {
			Dictionary<string, Func<string>> names = cooking ? API_DisplayName_Cooking : API_DisplayName_Crafting;

			if (names.ContainsKey(category.Id))
				return names[category.Id]();

			return category.Name;
		}

		return Mod.Helper.Translation.Get(category.I18nKey);
	}

	public void CreateDefaultCategory(bool cooking, string categoryId, Func<string> Name, IEnumerable<string>? recipeNames = null, string? iconRecipe = null, bool useRules = false, IEnumerable<IDynamicRuleData>? rules = null) {

		AddedAPICategories added = cooking ? API_Cooking : API_Crafting;
		Dictionary<string, Func<string>> names = cooking ? API_DisplayName_Cooking : API_DisplayName_Crafting;

		if (!names.ContainsKey(categoryId))
			names.Add(categoryId, Name);

		if (added.AddedCategories.ContainsKey(categoryId))
			return;

		var cat = new Category() {
			Id = categoryId,
			Name = Name(),
			I18nKey = $"api-access",
			Icon = new CategoryIcon() {
				Type = CategoryIcon.IconType.Item,
				RecipeName = iconRecipe,
			},
			UseRules = useRules,
			DynamicRules = rules?.
				Select(x => DynamicRuleData.FromGeneric(x)).ToList()
		};

		if (recipeNames is not null)
			cat.Recipes = new(recipeNames);

		added.AddedCategories.Add(
			categoryId,
			cat
		);

		Invalidate();
	}

	public void CreateDefaultCategory(bool cooking, string categoryId, string Name, IEnumerable<string>? recipeNames = null, string? iconRecipe = null, bool useRules = false, IEnumerable<IDynamicRuleData>? rules = null) {

		AddedAPICategories added = cooking ? API_Cooking : API_Crafting;

		if (added.AddedCategories.ContainsKey(categoryId))
			return;

		var cat = new Category() {
			Id = categoryId,
			Name = Name,
			Icon = new CategoryIcon() {
				Type = CategoryIcon.IconType.Item,
				RecipeName = iconRecipe,
			},
			UseRules = useRules,
			DynamicRules = rules?.
				Select(x => DynamicRuleData.FromGeneric(x)).ToList()
		};

		if (recipeNames is not null)
			cat.Recipes = new(recipeNames);

		added.AddedCategories.Add(
			categoryId,
			cat
		);

		Invalidate();
	}

	public void AddRecipesToDefaultCategory(bool cooking, string categoryId, IEnumerable<string> recipeNames) {
		AddedAPICategories added = cooking ? API_Cooking : API_Crafting;

		if (!added.AddedRecipes.TryGetValue(categoryId, out var recipes)) {
			recipes = new(recipeNames);
			added.AddedRecipes[categoryId] = recipes;
			if (recipes.Count > 0)
				Invalidate();
			return;
		}

		int size = recipes.Count;

		foreach (string name in recipeNames)
			recipes.Add(name);

		if (recipes.Count != size)
			Invalidate();
	}

	public void RemoveRecipesFromDefaultCategory(bool cooking, string categoryId, IEnumerable<string> recipeNames) {
		AddedAPICategories added = cooking ? API_Cooking : API_Crafting;

		if (!added.RemovedRecipes.TryGetValue(categoryId, out var recipes)) {
			recipes = new(recipeNames);
			added.RemovedRecipes[categoryId] = recipes;
			if (recipes.Count > 0)
				Invalidate();
			return;
		}

		int size = recipes.Count;

		foreach (string name in recipeNames)
			recipes.Add(name);

		if (recipes.Count != size)
			Invalidate();
	}


	public Category[] GetCategories(Farmer who, bool cooking) {
		AssertFarmer(who);

		if (!DefaultsLoaded)
			LoadDefaults();

		long id = who.UniqueMultiplayerID;
		Category[]? result = null;
		AppliedStuff? applied = null;

		WithCategories(() => {
			if (cooking) {
				CookingCategories.TryGetValue(id, out result);
				if (AppliedDefaults.TryGetValue(id, out var defs))
					applied = defs.Cooking;
			} else {
				CraftingCategories.TryGetValue(id, out result);
				if (AppliedDefaults.TryGetValue(id, out var defs))
					applied = defs.Crafting;
			}
		});

		result ??= (cooking ? DefaultCookingCategories : DefaultCraftingCategories) ?? Array.Empty<Category>();

		if (ApplyCategoryChanges(ref result, ref applied, cooking ? API_Cooking : API_Crafting)) {
			// If we've made changes, store them.
			SetCategories(who, result, cooking);

			if (applied is not null)
				WithCategories(() => {
					if (AppliedDefaults.TryGetValue(id, out var defs)) {
						if (cooking)
							defs.Cooking = applied;
						else
							defs.Crafting = applied;
					} else if (cooking)
						AppliedDefaults[id] = new() {
							Cooking = applied,
						};
					else
						AppliedDefaults[id] = new() {
							Crafting = applied,
						};
				});

			// We don't save the changes though. Not unless someone makes
			// further changes to their categories.
		}

		return result;
	}

	private static bool ApplyCategoryChanges(ref Category[] existing, ref AppliedStuff? applied, AddedAPICategories added) {

		bool changed = false;

		string?[] ids = existing.Select(cat => cat.Id).ToArray();

		if (applied is null)
			applied = new();

		List<Category> working = new(existing);

		// Add new categories first.
		foreach(var cat in added.AddedCategories.Values) {
			if (cat.Id is null || applied.AddedCategories.Contains(cat.Id))
				continue;

			// Make a clone of the category.
			var ncat = new Category() {
				Id = cat.Id,
				Name = cat.Name,
				I18nKey = cat.I18nKey,
				Icon = cat.Icon == null ? null : new CategoryIcon() {
					Type = cat.Icon.Type,
					RecipeName = cat.Icon.RecipeName,
					Source = cat.Icon.Source,
					Path = cat.Icon.Path,
					Rect = cat.Icon.Rect,
					Scale = cat.Icon.Scale,
				},
				UseRules = cat.UseRules,
				DynamicRules = cat.DynamicRules?.
					Select(x => DynamicRuleData.FromGeneric(x)).ToList()
			};

			if (cat.Recipes is not null)
				ncat.Recipes = new(cat.Recipes);

			if (cat.UnwantedRecipes is not null)
				ncat.UnwantedRecipes = cat.UnwantedRecipes.ToArray();

			// And add it.
			applied.AddedCategories.Add(cat.Id);
			working.Add(ncat);
			changed = true;
		}

		// Add and remove recipes.
		foreach(var cat in working) {
			if (cat.Id is null)
				continue;

			if (added.AddedRecipes.TryGetValue(cat.Id, out var newRecipes)) {
				if (!applied.AddedRecipes.TryGetValue(cat.Id, out var appliedRecipes)) {
					appliedRecipes = new();
					applied.AddedRecipes[cat.Id] = appliedRecipes;
				}

				foreach (string recipe in newRecipes) {
					if (appliedRecipes.Contains(recipe))
						continue;

					appliedRecipes.Add(recipe);
					changed = true;

					if (cat.Recipes is null)
						cat.Recipes = new();
					if (!cat.Recipes.Contains(recipe))
						cat.Recipes.Add(recipe);
				}
			}

			if (added.RemovedRecipes.TryGetValue(cat.Id, out var removedRecipes)) {
				if (!applied.RemovedRecipes.TryGetValue(cat.Id, out var appliedRecipes)) {
					appliedRecipes = new();
					applied.RemovedRecipes[cat.Id] = appliedRecipes;
				}

				foreach (string recipe in removedRecipes) {
					if (appliedRecipes.Contains(recipe))
						continue;

					appliedRecipes.Add(recipe);
					changed = true;

					if (cat.Recipes is not null && cat.Recipes.Contains(recipe))
						cat.Recipes.Remove(recipe);
				}
			}
		}

		if (changed)
			existing = working.ToArray();

		return changed;
	}

	public void SetCategories(Farmer who, IEnumerable<Category>? categories, bool cooking) {
		AssertFarmer(who);

		long id = who.UniqueMultiplayerID;
		Category[] array;
		if (categories is Category[] cats)
			array = cats;
		else
			array = categories?.ToArray() ?? Array.Empty<Category>();

		WithCategories(() => {
			if (cooking) {
				if (array == null)
					CookingCategories.Remove(id);
				else
					CookingCategories[id] = array;
			} else {
				if (array == null)
					CraftingCategories.Remove(id);
				else
					CraftingCategories[id] = array;
			}
		});
	}

	#endregion

	#region Dynamic Rules

	private void RegisterDefaultRuleHandlers() {
		RegisterRuleHandler("Uncrafted", new UncraftedRuleHandler());
		RegisterRuleHandler("Search", new SearchRuleHandler(Mod));
		RegisterRuleHandler("Machine", new MachineRuleHandler(Mod));
		RegisterRuleHandler("Sprinkler", new SprinklerRuleHandler());
		//RegisterTypeHandler("Light", new LightTypeHandler());
		RegisterRuleHandler("BuffFarming", new BuffRuleHandler(BuffRuleHandler.FARMING));
		RegisterRuleHandler("BuffFishing", new BuffRuleHandler(BuffRuleHandler.FISHING));
		RegisterRuleHandler("BuffMining", new BuffRuleHandler(BuffRuleHandler.MINING));
		RegisterRuleHandler("BuffLuck", new BuffRuleHandler(BuffRuleHandler.LUCK));
		RegisterRuleHandler("BuffForaging", new BuffRuleHandler(BuffRuleHandler.FORAGING));
		RegisterRuleHandler("BuffMaxEnergy", new BuffRuleHandler(BuffRuleHandler.MAX_ENERGY));
		RegisterRuleHandler("BuffMagnetism", new BuffRuleHandler(BuffRuleHandler.MAGNETISM));
		RegisterRuleHandler("BuffSpeed", new BuffRuleHandler(BuffRuleHandler.SPEED));
		RegisterRuleHandler("BuffDefense", new BuffRuleHandler(BuffRuleHandler.DEFENSE));
		RegisterRuleHandler("BuffAttack", new BuffRuleHandler(BuffRuleHandler.ATTACK));
		RegisterRuleHandler("BuffGarlic", new SingleItemRuleHandler(772));
		RegisterRuleHandler("BuffLife", new SingleItemRuleHandler(773));
		RegisterRuleHandler("BuffMuscle", new SingleItemRuleHandler(351));
		RegisterRuleHandler("BuffSquidInk", new SingleItemRuleHandler(921));
		RegisterRuleHandler("BuffMonsterMusk", new SingleItemRuleHandler(879));
	}

	public bool RegisterRuleHandler(string key, IDynamicRuleHandler handler) {
		lock(RuleHandlers) {
			if (RuleHandlers.ContainsKey(key))
				return false;

			RuleHandlers.Add(key, handler);
			return true;
		}
	}

	public bool UnregisterRuleHandler(string key) {
		lock(RuleHandlers) {
			if (!RuleHandlers.ContainsKey(key))
				return false;

			RuleHandlers.Remove(key);
			return true;
		}
	}

	public KeyValuePair<string, IDynamicRuleHandler>[] GetRuleHandlers() {
		lock (RuleHandlers) {
			return RuleHandlers.ToArray();
		}
	}

	public bool TryGetRuleHandler(string key, [NotNullWhen(true)] out IDynamicRuleHandler? handler) {
		lock(RuleHandlers) {
			return RuleHandlers.TryGetValue(key, out handler);
		}
	}

	public IDynamicRuleHandler GetInvalidRuleHandler() {
		return invalidRuleHandler;
	}

	public (IDynamicRuleHandler, object?, DynamicRuleData)[]? HydrateDynamicRules(IEnumerable<DynamicRuleData>? ruleData) {
		if (ruleData is null)
			return null;

		List<(IDynamicRuleHandler, object?, DynamicRuleData)> result = new();

		lock (RuleHandlers) {
			foreach (DynamicRuleData rule in ruleData) {
				if (RuleHandlers.TryGetValue(rule.Id, out var handler)) {
					object? state;
					try {
						state = handler.ParseState(rule);
					} catch (Exception ex) {
						Log("An error occurred while executing a dynamic type handler.", LogLevel.Error, ex);

						result.Add((invalidRuleHandler, invalidRuleHandler.ParseState(rule), rule));
						continue;
					}

					result.Add((handler, state, rule));
				} else
					result.Add((invalidRuleHandler, invalidRuleHandler.ParseState(rule), rule));
			}
		}

		return result.Count > 0 ? result.ToArray() : null;
	}

	#endregion

	#region IRecipeProvider

	public IRecipe? GetBaseRecipe(CraftingRecipe recipe) {
		IRecipe result = new BaseRecipe(recipe);

		// We don't care if CreateItem returns null, but it
		// cannot throw an exception.
		try {
			result.CreateItem();
		} catch (Exception ex) {
			Log($"An error occurred creating an item for the recipe \"{result.Name}\". The recipe will be skipped.", LogLevel.Warn, ex);
			return null;
		}

		return result;
	}

	#endregion

	#region Recipe Providers

	public void Invalidate() {
		CraftingCount.ResetAllScreens();
		CookingCount.ResetAllScreens();
		RecipeTastes.ResetAllScreens();
	}

	public void AddProvider(IRecipeProvider provider) {
		if (provider == null)
			throw new ArgumentNullException(nameof(provider));

		WithRecipeProviders(() => {
			if (Providers.Contains(provider))
				return;

			Providers.Add(provider);
			Providers.Sort((a, b) => a.RecipePriority.CompareTo(b.RecipePriority));
		});

		Invalidate();
	}

	public void RemoveProvider(IRecipeProvider provider) {
		if (provider == null)
			return;

		WithRecipeProviders(() => {
			if (Providers.Contains(provider))
				Providers.Remove(provider);
		});

		Invalidate();
	}

	#endregion

	#region Data Loading

	public IRecipe? GetProvidedRecipe(string name, bool cooking) {
		CraftingRecipe raw;
		try {
			raw = new(name, cooking);
		} catch(Exception ex) {
			Log($"An error occurred creating a crafting recipe instance for \"{name}\" (cooking:{cooking}). The recipe will be skipped.", LogLevel.Warn, ex);
			return null;
		}

		lock ((Providers as ICollection).SyncRoot) {
			foreach (IRecipeProvider provider in Providers) {
				IRecipe? recipe;
				try {
					recipe = provider.GetRecipe(raw);
				} catch (Exception ex) {
					Log($"An error occurred in a recipe provider getting a recipe for \"{name}\" (cooking:{cooking}).", LogLevel.Warn, ex);
					continue;
				}

				if (recipe == null)
					continue;

				// We don't care if CreateItem returns null, but it
				// cannot throw an exception.
				try {
					recipe.CreateItem();

				} catch (Exception ex) {
					Log($"An error occurred creating an item for the recipe \"{recipe.Name}\" from {provider.GetType().FullName ?? provider.GetType().Name}. The recipe will be skipped.", LogLevel.Warn, ex);
					return null;
				}

				return recipe;
			}
		}

		try {
			return GetBaseRecipe(raw);
		} catch(Exception ex) {
			Log($"An error occurred creating a recipe instance for \"{name}\" (cooking:{cooking}). The recipe will be skipped.", LogLevel.Warn, ex);
			return null;
		}
	}


	public void LoadRecipes() {
		WithRecipes(() => WithRecipesByName(() => {
			CraftingRecipesByName.Value.Clear();
			CookingRecipesByName.Value.Clear();

			CraftingRecipes.Value.Clear();
			CookingRecipes.Value.Clear();

			CookingCount.Value = CraftingRecipe.cookingRecipes.Count;
			CraftingCount.Value = CraftingRecipe.craftingRecipes.Count;

			// Cooking
			foreach (string key in CraftingRecipe.cookingRecipes.Keys) {
				IRecipe? recipe = GetProvidedRecipe(key, true);
				if (recipe == null)
					continue;

				CookingRecipesByName.Value.Add(key, recipe);
				CookingRecipes.Value.Add(recipe);
			}

			foreach (IRecipeProvider provider in Providers) {
				if (!provider.CacheAdditionalRecipes)
					continue;

				var recipes = provider.GetAdditionalRecipes(true);
				if (recipes != null)
					foreach(IRecipe recipe in recipes) {
						if (recipe == null)
							continue;

						// We don't care if CreateItem returns null, but it
						// cannot throw an exception.
						try {
							recipe.CreateItem();

						} catch (Exception ex) {
							Log($"An error occurred creating an item for the recipe \"{recipe.Name}\" from {provider.GetType().FullName ?? provider.GetType().Name}", LogLevel.Warn, ex);
							continue;
						}

						CookingRecipesByName.Value.Add(recipe.Name, recipe);
						CookingRecipes.Value.Add(recipe);
					}
			}

			CookingRecipes.Value.Sort((a, b) => a.SortValue.CompareTo(b.SortValue));

			// Crafting
			foreach (string key in CraftingRecipe.craftingRecipes.Keys) {
				IRecipe? recipe = GetProvidedRecipe(key, false);
				if (recipe == null)
					continue;

				CraftingRecipesByName.Value.Add(key, recipe);
				CraftingRecipes.Value.Add(recipe);
			}

			foreach (IRecipeProvider provider in Providers) {
				if (!provider.CacheAdditionalRecipes)
					continue;

				var recipes = provider.GetAdditionalRecipes(false);
				if (recipes != null)
					foreach (IRecipe recipe in recipes) {
						if (recipe == null)
							continue;

						// We don't care if CreateItem returns null, but it
						// cannot throw an exception.
						try {
							recipe.CreateItem();

						} catch (Exception ex) {
							Log($"An error occurred creating an item for the recipe \"{recipe.Name}\" from {provider.GetType().FullName ?? provider.GetType().Name}", LogLevel.Warn, ex);
							continue;
						}

						CraftingRecipesByName.Value.Add(recipe.Name, recipe);
						CraftingRecipes.Value.Add(recipe);
					}
			}

			Log($"Loaded {CookingRecipes.Value.Count} cooking recipes and {CraftingRecipes.Value.Count} crafting recipes.", LogLevel.Debug);
		}));
	}

	private Dictionary<string, Category> HydrateCategories(IEnumerable<Category> categories, string source, Dictionary<string, Category>? byID = null) {
		byID ??= new();

		foreach (Category cat in categories) {
			try {
				MergeCategory(cat, byID);
			} catch (Exception ex) {
				Log($"Skipping bad category in {source}.", LogLevel.Warn, ex);
			}
		}

		return byID;
	}

	public void SaveCategories() {
		WithCategories(() => {
			Dictionary<long, Categories> data = new();

			string newName = I18n.Category_New();

			foreach (var entry in CraftingCategories) {
				long id = entry.Key;
				if (entry.Value == null || entry.Value.Length == 0)
					continue;

				Category[] valid = entry.Value.Where(cat => {
					return
						!((cat.Recipes == null || cat.Recipes.Count == 0) &&
						cat.Name.Equals(newName) &&
						(cat.Icon == null ||
							(cat.Icon.Type == CategoryIcon.IconType.Item
							&& string.IsNullOrEmpty(cat.Icon.RecipeName))));
				}).ToArray();

				if (valid.Length == 0)
					continue;

				data[id] = new() {
					Crafting = valid
				};
			}

			foreach (var entry in CookingCategories) {
				long id = entry.Key;
				if (entry.Value == null || entry.Value.Length == 0)
					continue;

				Category[] valid = entry.Value.Where(cat => {
					return
						!((cat.Recipes == null || cat.Recipes.Count == 0) &&
						cat.Name.Equals(newName) &&
						(cat.Icon == null ||
							(cat.Icon.Type == CategoryIcon.IconType.Item
							&& !string.IsNullOrEmpty(cat.Icon.RecipeName))));
				}).ToArray();

				if (valid.Length == 0)
					continue;

				if (data.ContainsKey(id))
					data[id].Cooking = valid;
				else
					data[id] = new() {
						Cooking = valid
					};
			}

			foreach (var entry in AppliedDefaults) {
				long id = entry.Key;
				if (entry.Value == null)
					continue;

				if (data.ContainsKey(id))
					data[id].Applied = entry.Value;
				else
					data[id] = new() {
						Applied = entry.Value
					};
			}

			// We have the data. Save it.
			string path = $"savedata/categories/{Constants.SaveFolderName}.json";

			try {
				Mod.Helper.Data.WriteJsonFile(path, data);
			} catch (Exception ex) {
				Log($"The {path} file could not be saved.", LogLevel.Error, ex);
			}
		});
	}

	public void LoadCategories() {
		WithCategories(() => {
			CookingCategories.Clear();
			CraftingCategories.Clear();
			AppliedDefaults.Clear();

			string path = $"savedata/categories/{Constants.SaveFolderName}.json";
			Dictionary<long, Categories>? data;

			try {
				data = Mod.Helper.Data.ReadJsonFile<Dictionary<long, Categories>>(path);
			} catch (Exception ex) {
				Log($"The {path} file is invalid or corrupt.", LogLevel.Error, ex);
				data = null;
			}

			if (data == null)
				return;

			foreach (KeyValuePair<long, Categories> entry in data) {
				if (entry.Value == null)
					continue;

				// Cooking
				if (entry.Value.Cooking != null)
					CookingCategories.Add(
						entry.Key,
						HydrateCategories(
							entry.Value.Cooking,
							$"{path} for user {entry.Key}"
						).Values.ToArray()
					);

				// Crafting
				if (entry.Value.Crafting != null)
					CraftingCategories.Add(
						entry.Key,
						HydrateCategories(
							entry.Value.Crafting,
							$"{path} for user {entry.Key}"
						).Values.ToArray()
					);

				// Applied Stuff
				if (entry.Value.Applied != null)
					AppliedDefaults.Add(
						entry.Key,
						entry.Value.Applied
					);
			}
		});
	}

	public void LoadDefaults() {
		WithDefaultCategories(() => {
			DefaultsLoaded = true;
			Dictionary<string, Category> CraftingByID = new();
			Dictionary<string, Category> CookingByID = new();

			// Read the primary categories data file.
			const string path = "assets/categories.json";
			Categories? cats = null;

			try {
				cats = Mod.Helper.Data.ReadJsonFile<Categories>(path);
				if (cats == null)
					Log($"The {path} file is missing or invalid.", LogLevel.Error);
			} catch (Exception ex) {
				Log($"The {path} file is invalid.", LogLevel.Error, ex);
			}

			if (cats != null) {
				if (cats.Cooking != null)
					HydrateCategories(cats.Cooking, path, CookingByID);

				if (cats.Crafting != null)
					HydrateCategories(cats.Crafting, path, CraftingByID);
			}

			// Now read categories from content packs.
			foreach (var cp in Mod.Helper.ContentPacks.GetOwned()) {
				if (!cp.HasFile("categories.json"))
					continue;

				cats = null;
				try {
					cats = cp.ReadJsonFile<Categories>("categories.json");
				} catch (Exception ex) {
					Log($"The categories.json file of {cp.Manifest.Name} is invalid.", LogLevel.Error, ex);
				}

				if (cats != null) {
					if (cats.Cooking != null)
						HydrateCategories(cats.Cooking, cp.Manifest.Name, CookingByID);
					if (cats.Crafting != null)
						HydrateCategories(cats.Crafting, cp.Manifest.Name, CraftingByID);
				}
			}

			DefaultCraftingCategories = CraftingByID.Values.ToArray();
			DefaultCookingCategories = CookingByID.Values.ToArray();

			Log($"Loaded {DefaultCookingCategories.Length} cooking categories and {DefaultCraftingCategories.Length} crafting categories.", LogLevel.Debug);
		});
	}

	private void MergeCategory(Category cat, Dictionary<string, Category> categories) {
		if (string.IsNullOrEmpty(cat.Id)) {
			// Generate the ID from the name.
			if (string.IsNullOrEmpty(cat.Name))
				throw new InvalidOperationException("Category with no ID or name.");

			cat.Id = NAME_TO_ID.Replace(cat.Name.ToLower(), "_");
		}

		// Is there an existing entry?
		if (categories.TryGetValue(cat.Id, out Category? existing)) {
			// Overwrite names.
			if (!string.IsNullOrEmpty(cat.Name))
				existing.Name = cat.Name;

			// Overwrite I18n keys.
			if (cat.I18nKey != null)
				existing.I18nKey = string.IsNullOrEmpty(cat.I18nKey) ? null : cat.I18nKey;

			// Overwrite icons.
			if (cat.Icon != null)
				existing.Icon = cat.Icon;

			// Merge Recipes
			if (existing.Recipes is null)
				existing.Recipes = new();

			// Unwanted Recipes -- Old Style
			if (cat.UnwantedRecipes != null && cat.UnwantedRecipes.Length > 0) { 
				foreach(string recipe in cat.UnwantedRecipes)
					existing.Recipes.Remove(recipe);
			}

			// New Recipes
			if (cat.Recipes != null && cat.Recipes.Count > 0) { 
				foreach (string recipe in cat.Recipes) {
					if (string.IsNullOrEmpty(recipe))
						continue;
					else if (recipe.StartsWith("--"))
						existing.Recipes.Remove(recipe[2..]);
					else if (recipe.StartsWith(" --"))
						existing.Recipes.Add(recipe[1..]);
					else
						existing.Recipes.Add(recipe);
				}
			}

			return;
		}

		if (string.IsNullOrEmpty(cat.Id))
			cat.Name = cat.Id;

		if (cat.I18nKey == null) {
			string possible = $"category.{cat.Id}";
			if (Mod.DoesTranslationExist(possible))
				cat.I18nKey = possible;
		}

		if (cat.Recipes == null)
			cat.Recipes = new();

		cat.UnwantedRecipes = null;
		categories.Add(cat.Id, cat);
	}

	#endregion

}
