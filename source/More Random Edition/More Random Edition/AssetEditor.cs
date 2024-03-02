/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Chikakoo/stardew-valley-randomizer
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.GameData;
using System;
using System.Collections.Generic;

namespace Randomizer
{
    public class AssetEditor
	{
		private readonly ModEntry _mod;
		private Dictionary<string, string> _recipeReplacements = new();
		private Dictionary<string, string> _bundleReplacements = new();
        private Dictionary<string, string> _blueprintReplacements = new();
		private Dictionary<string, string> _uiStringReplacements = new();
		private Dictionary<string, string> _grandpaStringReplacements = new();
		private Dictionary<string, string> _stringReplacements = new();
        private Dictionary<string, string> _farmEventsReplacements = new();
        private Dictionary<string, string> _locationStringReplacements = new();
		private Dictionary<int, string> _fishReplacements = new();
		private Dictionary<int, string> _questReplacements = new();
		private Dictionary<string, string> _mailReplacements = new();
		private Dictionary<string, string> _locationsReplacements = new();
		private Dictionary<int, string> _objectInformationReplacements = new();
		private Dictionary<int, string> _fruitTreeReplacements = new();
		private Dictionary<int, string> _cropReplacements = new();
		private Dictionary<string, string> _cookingChannelReplacements = new();
		private Dictionary<int, string> _weaponReplacements = new();
		private Dictionary<int, string> _bootReplacements = new();
		private Dictionary<string, string> _monsterReplacements = new();
		private Dictionary<string, string> _birthdayReplacements = new();
		private Dictionary<string, string> _preferenceReplacements = new();
        private Dictionary<int, string> _secretNotesReplacements = new();
        private Dictionary<string, string> _objectContextTagsAdjustments = new();
        private Dictionary<string, SpecialOrderData> _specialOrderAdjustments = new();

        /// <summary>
        /// Whether we're currently ignoring replacing object information
        /// This is done between day loads to prevent errors with the Special Orders
        /// Eventually this can be removed when we modify the orders themselves
        /// </summary>
        private bool IgnoreObjectInformationReplacements { get; set; }

		public AssetEditor(ModEntry mod)
		{
			_mod = mod;
		}

		/// <summary>
		/// Called when requesting new assets - will replace them with our version
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
        public void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
			if (TryReplaceAsset(e, "Data/CraftingRecipes", _recipeReplacements) ||
                TryReplaceAsset(e, "Data/Bundles", _bundleReplacements) ||
                TryReplaceAsset(e, "Data/Blueprints", _blueprintReplacements) ||
                TryReplaceAsset(e, "Strings/UI", _uiStringReplacements) ||
				TryReplaceAsset(e, "Data/Events/Farm", _farmEventsReplacements) ||
                TryReplaceAsset(e, "Data/Fish", _fishReplacements) ||
                TryReplaceAsset(e, "Data/Quests", _questReplacements) ||
                TryReplaceAsset(e, "Data/mail", _mailReplacements) ||
                TryReplaceAsset(e, "Data/Locations", _locationsReplacements) ||
                TryReplaceAsset(e, "Strings/Locations", _locationStringReplacements) ||
                TryReplaceAsset(e, "Data/fruitTrees", _fruitTreeReplacements) ||
                TryReplaceAsset(e, "Data/Crops", _cropReplacements) ||
                TryReplaceAsset(e, "Data/TV/CookingChannel", _cookingChannelReplacements) ||
                TryReplaceAsset(e, "Data/weapons", _weaponReplacements) ||
                TryReplaceAsset(e, "Data/Boots", _bootReplacements) ||
                TryReplaceAsset(e, "Data/Monsters", _monsterReplacements) ||
                TryReplaceAsset(e, "Data/NPCDispositions", _birthdayReplacements) ||
                TryReplaceAsset(e, "Data/NPCGiftTastes", _preferenceReplacements) ||
                TryReplaceAsset(e, "Data/SecretNotes", _secretNotesReplacements) ||
                TryReplaceAsset(e, "Data/ObjectContextTags", _objectContextTagsAdjustments) ||
                TryReplaceAsset(e, "Data/SpecialOrders", _specialOrderAdjustments))
			{
				return;
			}

            if (ShouldReplaceAsset(e, "Strings/StringsFromCSFiles"))
            {
                e.Edit((asset) => ApplyEdits(asset, _grandpaStringReplacements));
                e.Edit((asset) => ApplyEdits(asset, _stringReplacements));
            }
            else if (ShouldReplaceAsset(e, "Data/ObjectInformation"))
            {
                if (IgnoreObjectInformationReplacements)
                {
                    e.Edit((asset) => ApplyEdits(asset, new Dictionary<int, string>()));
                }
                else
                {
                    e.Edit((asset) => ApplyEdits(asset, _objectInformationReplacements));
                }
            } 
        }

        /// <summary>
        /// Whether we should replace the asset based on the setting
        /// </summary>
        /// <param name="e">The requested asset, so we can grab the name off of it</param>
        /// <param name="assetName">The name that were currently checking - if they don't match, exit early</param>
        /// <returns>True if we should replace it; false otherwise</returns>
        private static bool ShouldReplaceAsset(AssetRequestedEventArgs e, string assetName)
        {
			if (!e.NameWithoutLocale.IsEquivalentTo(assetName))
            {
                return false;
            }

            if (e.NameWithoutLocale.IsEquivalentTo("Data/CraftingRecipes")) { return Globals.Config.CraftingRecipes.Randomize; }
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Bundles")) { return Globals.Config.Bundles.Randomize; }
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Blueprints")) { return Globals.Config.RandomizeBuildingCosts; }
            if (e.NameWithoutLocale.IsEquivalentTo("Strings/StringsFromCSFiles")) { return true; }
            if (e.NameWithoutLocale.IsEquivalentTo("Strings/UI")) { return true; }
			if (e.NameWithoutLocale.IsEquivalentTo("Data/Events/Farm")) { return Globals.Config.Animals.RandomizePets; }
            if (e.NameWithoutLocale.IsEquivalentTo("Data/ObjectInformation")) { return true; }
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Fish")) { return Globals.Config.Fish.Randomize; }
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Quests") || e.Name.IsEquivalentTo("Data/mail")) { return Globals.Config.RandomizeQuests; }
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Locations")) { return Globals.Config.Fish.Randomize || Globals.Config.RandomizeForagables || Globals.Config.AddRandomArtifactItem; }
            if (e.NameWithoutLocale.IsEquivalentTo("Strings/Locations")) { return Globals.Config.Crops.Randomize; } // For now, as the only thing is the sweet gem berry text
            if (e.NameWithoutLocale.IsEquivalentTo("Data/fruitTrees")) { return Globals.Config.RandomizeFruitTrees; }
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Crops")) { return Globals.Config.Crops.Randomize; }
            if (e.NameWithoutLocale.IsEquivalentTo("Data/TV/CookingChannel")) { return Globals.Config.Crops.Randomize || Globals.Config.Fish.Randomize; }
            if (e.NameWithoutLocale.IsEquivalentTo("Data/weapons")) { return Globals.Config.Weapons.Randomize; }
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Boots")) { return Globals.Config.Boots.Randomize; }
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Monsters")) { return Globals.Config.Monsters.Randomize; }
            if (e.NameWithoutLocale.IsEquivalentTo("Data/NPCDispositions")) { return Globals.Config.NPCs.RandomizeBirthdays; }
            if (e.NameWithoutLocale.IsEquivalentTo("Data/NPCGiftTastes")) { return Globals.Config.NPCs.RandomizeIndividualPreferences || Globals.Config.NPCs.RandomizeUniversalPreferences; }
            if (e.NameWithoutLocale.IsEquivalentTo("Data/SecretNotes")) { return Globals.Config.NPCs.RandomizeIndividualPreferences; }
            if (e.NameWithoutLocale.IsEquivalentTo("Data/ObjectContextTags") ||
                e.NameWithoutLocale.IsEquivalentTo("Data/SpecialOrders")) 
            {
                // Only need to adjust fish quests at the moment
                // The context tags can't be adjusted unless the world is ready, so we'll wait
                return Globals.Config.Fish.Randomize; 
            } 
            return false;
        }

		/// <summary>
		/// Tries to replace the asset with the one with the given name
		/// </summary>
		/// <param name="e"></param>
		/// <param name="assetName"></param>
		/// <param name="replacement"></param>
		/// <returns>True if successful, false otherwise</returns>
        private static bool TryReplaceAsset(AssetRequestedEventArgs e, string assetName, Dictionary<string, string> replacement)
        {
            if (ShouldReplaceAsset(e, assetName))
            {
                e.Edit((asset) => ApplyEdits(asset, replacement));
                return true;
            }
            return false;
        }

        /// <summary>
        /// Tries to replace the asset with the one with the given name
        /// </summary>
        /// <param name="e"></param>
        /// <param name="assetName"></param>
        /// <param name="replacement"></param>
        /// <returns>True if successful, false otherwise</returns>
        private static bool TryReplaceAsset(AssetRequestedEventArgs e, string assetName, Dictionary<int, string> replacement)
        {
            if (ShouldReplaceAsset(e, assetName))
            {
                e.Edit((asset) => ApplyEdits(asset, replacement));
                return true;
            }
            return false;
        }

        /// <summary>
        /// Tries to replace the asset with the one with the given name
        /// </summary>
        /// <param name="e"></param>
        /// <param name="assetName"></param>
        /// <param name="replacement"></param>
        /// <returns>True if successful, false otherwise</returns>
        private static bool TryReplaceAsset(AssetRequestedEventArgs e, string assetName, Dictionary<string, SpecialOrderData> replacement)
        {
            if (ShouldReplaceAsset(e, assetName))
            {
                e.Edit((asset) => ApplyEdits(asset, replacement));
                return true;
            }
            return false;
        }

        private static void ApplyEdits<TKey, TValue>(IAssetData asset, IDictionary<TKey, TValue> edits)
        {
            IAssetDataForDictionary<TKey, TValue> assetDict = asset.AsDictionary<TKey, TValue>();
            foreach (KeyValuePair<TKey, TValue> edit in edits)
            {
                assetDict.Data[edit.Key] = edit.Value;
            }
        }

        /// <summary>
        /// Invalidates the cache for all the assets
        /// </summary>
        public void InvalidateCache()
		{
            InvalidateCacheForDefaultAndCurrentLocales("Data/CraftingRecipes");
            InvalidateCacheForDefaultAndCurrentLocales("Data/Bundles");
            InvalidateCacheForDefaultAndCurrentLocales("Data/Blueprints");
			InvalidateCacheForDefaultAndCurrentLocales("Strings/StringsFromCSFiles");
			InvalidateCacheForDefaultAndCurrentLocales("Strings/UI");
			InvalidateCacheForDefaultAndCurrentLocales("Data/ObjectInformation");
			InvalidateCacheForDefaultAndCurrentLocales("Data/Events/Farm");
			InvalidateCacheForDefaultAndCurrentLocales("Data/Fish");
			InvalidateCacheForDefaultAndCurrentLocales("Data/Quests");
			InvalidateCacheForDefaultAndCurrentLocales("Data/mail");
			InvalidateCacheForDefaultAndCurrentLocales("Data/Locations");
			InvalidateCacheForDefaultAndCurrentLocales("Strings/Locations");
			InvalidateCacheForDefaultAndCurrentLocales("Data/fruitTrees");
			InvalidateCacheForDefaultAndCurrentLocales("Data/Crops");
			InvalidateCacheForDefaultAndCurrentLocales("Data/TV/CookingChannel");
			InvalidateCacheForDefaultAndCurrentLocales("Data/weapons");
			InvalidateCacheForDefaultAndCurrentLocales("Data/Boots");
			InvalidateCacheForDefaultAndCurrentLocales("Data/Monsters");
            InvalidateCacheForDefaultAndCurrentLocales("Data/NPCDispositions");
            InvalidateCacheForDefaultAndCurrentLocales("Data/NPCGiftTastes");
            InvalidateCacheForDefaultAndCurrentLocales("Data/SecretNotes");
            InvalidateCacheForDefaultAndCurrentLocales("Data/ObjectContextTags");
            InvalidateCacheForDefaultAndCurrentLocales("Data/SpecialOrders");
        }

        /// <summary>
        /// Invalidates the cache for default and current locales
        /// done to avoid annoying caching issues when not playing in English
        /// we do both because some assets don't have every locale defined
        /// </summary>
        /// <param name="assetName">The asset to invalidate</param>
		public void InvalidateCacheForDefaultAndCurrentLocales(string assetName)
		{
            _mod.Helper.GameContent.InvalidateCache(assetName);
            _mod.Helper.GameContent.InvalidateCache(Globals.GetLocalizedFileName(assetName));
        }

        /// <summary>
        /// To be called when returning to the the title screen
        /// This will help localized resources be reloaded properly
        /// </summary>
        public void ResetValuesAndInvalidateCache()
        {
            _recipeReplacements.Clear();
            _bundleReplacements.Clear();
            _blueprintReplacements.Clear();
            _uiStringReplacements.Clear();
            _grandpaStringReplacements.Clear();
            _stringReplacements.Clear();
            _farmEventsReplacements.Clear();
            _locationStringReplacements.Clear();
            _fishReplacements.Clear();
            _questReplacements.Clear();
            _mailReplacements.Clear();
            _locationsReplacements.Clear();
            _objectInformationReplacements.Clear();
            _fruitTreeReplacements.Clear();
            _cropReplacements.Clear();
            _cookingChannelReplacements.Clear();
            _weaponReplacements.Clear();
            _bootReplacements.Clear();
            _monsterReplacements.Clear();
            _birthdayReplacements.Clear();
            _preferenceReplacements.Clear();
            _secretNotesReplacements.Clear();
            _objectContextTagsAdjustments.Clear();
            _specialOrderAdjustments.Clear();

            InvalidateCache();
        }

        /// <summary>
        /// Calculates edits that need to happen before a save file is loaded
        /// </summary>
        public void CalculateEditsBeforeLoad()
		{
			CalculateAndInvalidateUIEdits();
			_grandpaStringReplacements = StringsAdjustments.RandomizeGrandpasStory();
        }

        /// <summary>
        /// Calculates the UI string replacements and invalidates the cache so it can be updated
        /// Should be called on game load and after a language change
        /// </summary>
        public void CalculateAndInvalidateUIEdits()
		{
			_uiStringReplacements = StringsAdjustments.ModifyRemixedBundleUI();
			_mod.Helper.GameContent.InvalidateCache("Strings/UI");
		}

		/// <summary>
		/// Calculates all the things to edit and creates the replacement dictionaries
		/// </summary>
		public void CalculateEdits()
		{
			ItemList.Initialize();
			ValidateItemList();

			EditedObjectInformation editedObjectInfo = new();
			FishRandomizer.Randomize(editedObjectInfo);
			_fishReplacements = editedObjectInfo.FishReplacements;

			CropRandomizer.Randomize(editedObjectInfo);
			_fruitTreeReplacements = editedObjectInfo.FruitTreeReplacements;
			_cropReplacements = editedObjectInfo.CropsReplacements;
			_objectInformationReplacements = editedObjectInfo.ObjectInformationReplacements;

			_blueprintReplacements = BlueprintRandomizer.Randomize();
			_monsterReplacements = MonsterRandomizer.Randomize(); // Must be done before recipes since rarities of drops change
			_locationsReplacements = LocationRandomizer.Randomize(_objectInformationReplacements); // Must be done before recipes because of wild seeds
			_recipeReplacements = CraftingRecipeRandomizer.Randomize();
			_stringReplacements = StringsAdjustments.GetCSFileStringReplacements();
			_farmEventsReplacements = StringsAdjustments.GetFarmEventsReplacements();
			_locationStringReplacements = StringsAdjustments.GetLocationStringReplacements();
			CraftingRecipeAdjustments.FixCookingRecipeDisplayNames();
			_cookingChannelReplacements = CookingChannelAdjustments.GetTextEdits();

			// Needs to run after Cooking Recipe fix so that cooked items are properly named,
			// and needs to run before bundles so that NPC Loved Item bundles are properly generated
			_preferenceReplacements = PreferenceRandomizer.Randomize();
			_secretNotesReplacements = SecretNotesRandomizer.FixSecretNotes(_preferenceReplacements);

            // Bundles need to be ran after preverences so modified NPC values are correct
			_bundleReplacements = BundleRandomizer.Randomize();
            MusicRandomizer.Randomize();

			QuestInformation questInfo = QuestRandomizer.Randomize();
			_questReplacements = questInfo.QuestReplacements;
			_mailReplacements = questInfo.MailReplacements;

			_weaponReplacements = WeaponRandomizer.Randomize();
			_bootReplacements = BootRandomizer.Randomize();
			_birthdayReplacements = BirthdayRandomizer.Randomize();

            _objectContextTagsAdjustments = ObjectContextTagsAdjustments.GetObjectContextTagAdjustments();
            _specialOrderAdjustments = SpecialOrderAdjustments.GetSpecialOrderAdjustments();
        }

		/// <summary>
		/// Turns on the flag to ignore object information replacements and invalidates the cache
		/// so that the original values are reloaded
		/// </summary>
		public void UndoObjectInformationReplacements()
		{
			IgnoreObjectInformationReplacements = true;
            InvalidateCacheForDefaultAndCurrentLocales("Data/ObjectInformation");
        }

		/// <summary>
		/// Turns off the flag to ignore object information replacements and invalidates the cache
		/// so that the randomized values are reloaded
		/// </summary>
		public void RedoObjectInformationReplacements()
		{
			IgnoreObjectInformationReplacements = false;
            InvalidateCacheForDefaultAndCurrentLocales("Data/ObjectInformation");
        }

		/// <summary>
		/// Validates that all the items in ObjectIndexes exist in the main item lists
		/// </summary>
		private static void ValidateItemList()
		{
			foreach (ObjectIndexes index in Enum.GetValues(typeof(ObjectIndexes)))
			{
				if (!ItemList.Items.ContainsKey(index))
				{
					Globals.ConsoleWarn($"Missing item: {(int)index}: {index}");
				}
			}
        }
	}
}