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
using StardewValley.GameData.Buildings;
using StardewValley.GameData.Characters;
using StardewValley.GameData.Crops;
using StardewValley.GameData.FruitTrees;
using StardewValley.GameData.GarbageCans;
using StardewValley.GameData.Museum;
using StardewValley.GameData.Objects;
using StardewValley.GameData.Shops;
using StardewValley.GameData.SpecialOrders;
using StardewValley.GameData.Weapons;
using System;
using System.Collections.Generic;
using SVLocationData = StardewValley.GameData.Locations.LocationData;

namespace Randomizer
{
    public class AssetEditor
	{
		private readonly ModEntry _mod;
		private Dictionary<string, string> _recipeReplacements = new();
		private Dictionary<string, string> _bundleReplacements = new();
        private Dictionary<string, BuildingData> _buildingReplacements = new();
        private Dictionary<string, string> _uiStringReplacements = new();
        private Dictionary<string, string> _grandpaStringReplacements = new();
        private Dictionary<string, string> _stringReplacements = new();
        private Dictionary<string, string> _farmEventsReplacements = new();
        private Dictionary<string, string> _locationStringReplacements = new();
        private Dictionary<string, string> _fishReplacements = new();
        private Dictionary<string, string> _questReplacements = new();
        private Dictionary<string, string> _mailReplacements = new();
        private Dictionary<string, SVLocationData> _locationsReplacements = new();
        private Dictionary<string, ObjectData> _objectReplacements = new();
        private Dictionary<string, FruitTreeData> _fruitTreeReplacements = new();
        private Dictionary<string, CropData> _cropReplacements = new();
        private Dictionary<string, string> _cookingChannelReplacements = new();
        private Dictionary<string, WeaponData> _weaponReplacements = new();
        private Dictionary<string, string> _bootReplacements = new();
        private Dictionary<string, string> _monsterReplacements = new();
        private Dictionary<string, CharacterData> _birthdayReplacements = new();
        private Dictionary<string, string> _preferenceReplacements = new();
        private Dictionary<int, string> _secretNotesReplacements = new();
        private Dictionary<string, SpecialOrderData> _specialOrderAdjustments = new();
        private Dictionary<string, MuseumRewards> _museumRewardReplacements = new();
        private Dictionary<string, ShopData> _shopReplacements = new();
        private GarbageCanData _garbageCanReplacements = null;

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
                TryReplaceAsset(e, "Data/Buildings", _buildingReplacements) ||
                TryReplaceAsset(e, "Strings/UI", _uiStringReplacements) ||
				TryReplaceAsset(e, "Data/Events/Farm", _farmEventsReplacements) ||
                TryReplaceAsset(e, "Data/Fish", _fishReplacements) ||
                TryReplaceAsset(e, "Data/Quests", _questReplacements) ||
                TryReplaceAsset(e, "Data/mail", _mailReplacements) ||
                TryReplaceAsset(e, "Data/Locations", _locationsReplacements) ||
                TryReplaceAsset(e, "Strings/Locations", _locationStringReplacements) ||
                TryReplaceAsset(e, "Data/Objects", _objectReplacements) ||
                TryReplaceAsset(e, "Data/FruitTrees", _fruitTreeReplacements) ||
                TryReplaceAsset(e, "Data/Crops", _cropReplacements) ||
                TryReplaceAsset(e, "Data/TV/CookingChannel", _cookingChannelReplacements) ||
                TryReplaceAsset(e, "Data/Weapons", _weaponReplacements) ||
                TryReplaceAsset(e, "Data/Boots", _bootReplacements) ||
                TryReplaceAsset(e, "Data/Monsters", _monsterReplacements) ||
                TryReplaceAsset(e, "Data/Characters", _birthdayReplacements) ||
                TryReplaceAsset(e, "Data/NPCGiftTastes", _preferenceReplacements) ||
                TryReplaceAsset(e, "Data/SecretNotes", _secretNotesReplacements) ||
                TryReplaceAsset(e, "Data/SpecialOrders", _specialOrderAdjustments) ||
                TryReplaceAsset(e, "Data/MuseumRewards", _museumRewardReplacements) ||
                TryReplaceAsset(e, "Data/Shops", _shopReplacements) ||
                TryReplaceAsset(e, "Data/GarbageCans", _garbageCanReplacements))
			{
				return;
			}

            if (ShouldReplaceAsset(e, "Strings/StringsFromCSFiles"))
            {
                e.Edit((asset) => ApplyEdits(asset, _grandpaStringReplacements));
                e.Edit((asset) => ApplyEdits(asset, _stringReplacements));
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
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Buildings")) { return Globals.Config.RandomizeBuildingCosts; }
            if (e.NameWithoutLocale.IsEquivalentTo("Strings/StringsFromCSFiles")) { return true; }
            if (e.NameWithoutLocale.IsEquivalentTo("Strings/UI")) { return true; }
			if (e.NameWithoutLocale.IsEquivalentTo("Data/Events/Farm")) { return Globals.Config.Animals.RandomizePets; }
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Objects")) { return true; }
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Fish")) { return Globals.Config.Fish.Randomize; }
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Quests") || e.NameWithoutLocale.IsEquivalentTo("Data/mail")) { return Globals.Config.RandomizeQuests; }
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Locations")) { return Globals.Config.Fish.Randomize || Globals.Config.RandomizeForagables || Globals.Config.AddRandomArtifactItem; }
            if (e.NameWithoutLocale.IsEquivalentTo("Strings/Locations")) { return Globals.Config.Crops.Randomize; } // For now, as the only thing is the sweet gem berry text
            if (e.NameWithoutLocale.IsEquivalentTo("Data/FruitTrees")) { return Globals.Config.RandomizeFruitTrees; }
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Crops")) { return Globals.Config.Crops.Randomize; }
            if (e.NameWithoutLocale.IsEquivalentTo("Data/TV/CookingChannel")) { return Globals.Config.Crops.Randomize || Globals.Config.Fish.Randomize; }
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Weapons")) { return Globals.Config.Weapons.Randomize; }
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Boots")) { return Globals.Config.Boots.Randomize; }
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Monsters")) { return Globals.Config.Monsters.Randomize; }
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Characters")) { return Globals.Config.NPCs.RandomizeBirthdays; }
            if (e.NameWithoutLocale.IsEquivalentTo("Data/NPCGiftTastes")) { return Globals.Config.NPCs.RandomizeIndividualPreferences || Globals.Config.NPCs.RandomizeUniversalPreferences; }
            if (e.NameWithoutLocale.IsEquivalentTo("Data/SecretNotes")) { return Globals.Config.NPCs.RandomizeIndividualPreferences; }
            if (e.NameWithoutLocale.IsEquivalentTo("Data/SpecialOrders")) { return Globals.Config.Fish.Randomize; }
            if (e.NameWithoutLocale.IsEquivalentTo("Data/MuseumRewards")) { return Globals.Config.RandomizeMuseumRewards; }
            if (e.NameWithoutLocale.IsEquivalentTo("Data/GarbageCans")) { return Globals.Config.RandomizeGarbageCans; }
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Shops")) 
            { 
                // The logic to include changes is included in the randomizer (there are many settings)
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
        private static bool TryReplaceAsset<TKey, TValue>(
            AssetRequestedEventArgs e, 
            string assetName, 
            IDictionary<TKey, TValue> replacement)
        {
            if (ShouldReplaceAsset(e, assetName))
            {
                e.Edit((asset) => ApplyEdits(asset, replacement));
                return true;
            }
            return false;
        }

        private static void ApplyEdits<TKey, TValue>(
            IAssetData asset, 
            IDictionary<TKey, TValue> edits)
        {
            IAssetDataForDictionary<TKey, TValue> assetDict = asset.AsDictionary<TKey, TValue>();
            foreach (KeyValuePair<TKey, TValue> edit in edits)
            {
                assetDict.Data[edit.Key] = edit.Value;
            }
        }

        /// <summary>
        /// Currently, only the garbage cans have non-dictionary data, so they're here
        /// on their own for now
        /// </summary>
        /// <param name="e">The asset requested info</param>
        /// <param name="assetName">The current asset in Stardew</param>
        /// <param name="garbageCanData">The asset we wish to replace the Stardew asset with</param>
        /// <returns>True if we should replace the asset, false otherwise</returns>
        private static bool TryReplaceAsset(
            AssetRequestedEventArgs e, 
            string assetName,
            GarbageCanData garbageCanData)
        {
            if (ShouldReplaceAsset(e, assetName))
            {
                e.Edit(asset =>
                {
                    if (garbageCanData != null)
                    {
                        asset.ReplaceWith(garbageCanData);
                    }
                });
            }
            return false;
        }

        /// <summary>
        /// Invalidates the cache for all the assets
        /// </summary>
        public void InvalidateCache()
		{
            InvalidateCacheForDefaultAndCurrentLocales("Data/CraftingRecipes");
            InvalidateCacheForDefaultAndCurrentLocales("Data/Bundles");
            InvalidateCacheForDefaultAndCurrentLocales("Data/Buildings");
			InvalidateCacheForDefaultAndCurrentLocales("Strings/StringsFromCSFiles");
			InvalidateCacheForDefaultAndCurrentLocales("Strings/UI");
			InvalidateCacheForDefaultAndCurrentLocales("Data/Objects");
			InvalidateCacheForDefaultAndCurrentLocales("Data/Events/Farm");
			InvalidateCacheForDefaultAndCurrentLocales("Data/Fish");
			InvalidateCacheForDefaultAndCurrentLocales("Data/Quests");
			InvalidateCacheForDefaultAndCurrentLocales("Data/mail");
			InvalidateCacheForDefaultAndCurrentLocales("Data/Locations");
			InvalidateCacheForDefaultAndCurrentLocales("Strings/Locations");
			InvalidateCacheForDefaultAndCurrentLocales("Data/FruitTrees");
			InvalidateCacheForDefaultAndCurrentLocales("Data/Crops");
			InvalidateCacheForDefaultAndCurrentLocales("Data/TV/CookingChannel");
			InvalidateCacheForDefaultAndCurrentLocales("Data/Weapons");
			InvalidateCacheForDefaultAndCurrentLocales("Data/Boots");
			InvalidateCacheForDefaultAndCurrentLocales("Data/Monsters");
            InvalidateCacheForDefaultAndCurrentLocales("Data/Characters");
            InvalidateCacheForDefaultAndCurrentLocales("Data/NPCGiftTastes");
            InvalidateCacheForDefaultAndCurrentLocales("Data/SecretNotes");
            InvalidateCacheForDefaultAndCurrentLocales("Data/ObjectContextTags");
            InvalidateCacheForDefaultAndCurrentLocales("Data/SpecialOrders");
            InvalidateCacheForDefaultAndCurrentLocales("Data/MuseumRewards");
            InvalidateCacheForDefaultAndCurrentLocales("Data/Shops");
            InvalidateCacheForDefaultAndCurrentLocales("Data/GarbageCans");
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
        /// To be called when returning to the title screen
        /// This will help localized resources be reloaded properly
        /// </summary>
        public void ResetValuesAndInvalidateCache()
        {
            _recipeReplacements.Clear();
            _bundleReplacements.Clear();
            _buildingReplacements.Clear();
            _uiStringReplacements.Clear();
            _grandpaStringReplacements.Clear();
            _stringReplacements.Clear();
            _farmEventsReplacements.Clear();
            _locationStringReplacements.Clear();
            _fishReplacements.Clear();
            _questReplacements.Clear();
            _mailReplacements.Clear();
            _locationsReplacements.Clear();
            _objectReplacements.Clear();
            _fruitTreeReplacements.Clear();
            _cropReplacements.Clear();
            _cookingChannelReplacements.Clear();
            _weaponReplacements.Clear();
            _bootReplacements.Clear();
            _monsterReplacements.Clear();
            _birthdayReplacements.Clear();
            _preferenceReplacements.Clear();
            _secretNotesReplacements.Clear();
            _specialOrderAdjustments.Clear();
            _museumRewardReplacements.Clear();
            _shopReplacements.Clear();
            _garbageCanReplacements = null;

            InvalidateCache();
        }

        /// <summary>
        /// Calculates edits that need to happen before a save file is loaded
        /// </summary>
        public void CalculateEditsBeforeLoad()
		{
			_grandpaStringReplacements = StringsAdjustments.RandomizeGrandpasStory();
			CalculateAndInvalidateUIEdits();
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
        /// Shops are randomized once per day - this is a handler that's meant to be called
        /// at the start of the day
        /// </summary>
        public void CalculateAndInvalidateShopEdits()
        {
            _shopReplacements = ShopRandomizer.GetDailyShopReplacements();
            _mod.Helper.GameContent.InvalidateCache("Data/Shops");
        }

		/// <summary>
		/// Calculates all the things to edit and creates the replacement dictionaries
		/// </summary>
		public void CalculateEdits()
		{
			ItemList.Initialize();
			ValidateItemList();

			EditedObjects editedObjectInfo = new();
			_objectReplacements = editedObjectInfo.ObjectsReplacements;

            // Must be done before recipes because of wild seeds
            // Also, the ForagableRandomizer needs to be the one to define _locationsReplacements
            _locationsReplacements = ForagableRandomizer.Randomize(_objectReplacements);
            ArtifactSpotRandomizer.Randomize(_locationsReplacements);
			
            FishRandomizer.Randomize(editedObjectInfo, _locationsReplacements);
			_fishReplacements = editedObjectInfo.FishReplacements;

			CropRandomizer.Randomize(editedObjectInfo);
            _fruitTreeReplacements = FruitTreeRandomizer.Randomize(_objectReplacements);
			_cropReplacements = EditedObjects.CropsReplacements;

			_buildingReplacements = BuildingRandomizer.Randomize();
			_monsterReplacements = MonsterRandomizer.Randomize(); // Must be done before recipes since rarities of drops change
			_recipeReplacements = CraftingRecipeRandomizer.Randomize();
			_stringReplacements = StringsAdjustments.GetCSFileStringReplacements();
			_farmEventsReplacements = StringsAdjustments.GetFarmEventsReplacements();
			_locationStringReplacements = StringsAdjustments.GetLocationStringReplacements();
			_cookingChannelReplacements = CookingChannelAdjustments.GetTextEdits();

            // Needs to run after Cooking Recipe fix so that cooked items are properly named,
            // and needs to run before bundles so that NPC Loved Item bundles are properly generated
            _preferenceReplacements = PreferenceRandomizer.Randomize();
			_secretNotesReplacements = SecretNotesRandomizer.FixSecretNotes(_preferenceReplacements);
            _garbageCanReplacements = GarbageCanRandomizer.Randomize();

            // Bundles need to be ran after preferences so modified NPC values are correct
			_bundleReplacements = BundleRandomizer.Randomize();
            MusicRandomizer.Randomize();

			QuestInformation questInfo = QuestRandomizer.Randomize();
			_questReplacements = questInfo.QuestReplacements;
			_mailReplacements = questInfo.MailReplacements;

			_weaponReplacements = WeaponRandomizer.Randomize();
			_bootReplacements = BootRandomizer.Randomize();
			_birthdayReplacements = BirthdayRandomizer.Randomize();

            ObjectContextTagsAdjustments.AdjustContextTags(_objectReplacements);
            _specialOrderAdjustments = SpecialOrderAdjustments.GetSpecialOrderAdjustments();

            _museumRewardReplacements = MuseumRewardRandomizer.RandomizeMuseumRewards();
        }

		/// <summary>
		/// Validates that all the items in ObjectIndexes exist in the main item lists
		/// </summary>
		private static void ValidateItemList()
		{
			foreach (ObjectIndexes index in Enum.GetValues(typeof(ObjectIndexes)))
			{
				if (!ItemList.Items.ContainsKey(index.GetId()))
				{
					Globals.ConsoleWarn($"Missing item: {(int)index}: {index}");
				}
			}
        }
	}
}