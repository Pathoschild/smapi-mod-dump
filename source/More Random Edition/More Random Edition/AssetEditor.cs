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
using System;
using System.Collections.Generic;
using System.Linq;

namespace Randomizer
{
	public class AssetEditor : IAssetEditor
	{
		private readonly ModEntry _mod;
		private Dictionary<string, string> _recipeReplacements = new Dictionary<string, string>();
		private Dictionary<string, string> _bundleReplacements = new Dictionary<string, string>();
		private Dictionary<string, string> _blueprintReplacements = new Dictionary<string, string>();
		private Dictionary<string, string> _uiStringReplacements = new Dictionary<string, string>();
		private Dictionary<string, string> _grandpaStringReplacements = new Dictionary<string, string>();
		private Dictionary<string, string> _stringReplacements = new Dictionary<string, string>();
		private Dictionary<string, string> _locationStringReplacements = new Dictionary<string, string>();
		private Dictionary<int, string> _fishReplacements = new Dictionary<int, string>();
		private Dictionary<int, string> _questReplacements = new Dictionary<int, string>();
		private Dictionary<string, string> _mailReplacements = new Dictionary<string, string>();
		private Dictionary<string, string> _locationsReplacements = new Dictionary<string, string>();
		private Dictionary<int, string> _objectInformationReplacements = new Dictionary<int, string>();
		private Dictionary<int, string> _fruitTreeReplacements = new Dictionary<int, string>();
		private Dictionary<int, string> _cropReplacements = new Dictionary<int, string>();
		private Dictionary<string, string> _cookingChannelReplacements = new Dictionary<string, string>();
		private Dictionary<int, string> _weaponReplacements = new Dictionary<int, string>();
		private Dictionary<int, string> _bootReplacements = new Dictionary<int, string>();
		private Dictionary<string, string> _monsterReplacements = new Dictionary<string, string>();
		private Dictionary<string, string> _birthdayReplacements = new Dictionary<string, string>();

		/// <summary>
		/// Whether we're currently ignoring replacing object information
		/// This is done between day loads to prevent errors with the Special Orders
		/// Eventually this can be removed when we modify the orders themselves
		/// </summary>
		private bool IgnoreObjectInformationReplacements { get; set; }

		public AssetEditor(ModEntry mod)
		{
			this._mod = mod;
		}

		public bool CanEdit<T>(IAssetInfo asset)
		{
			if (asset.AssetNameEquals("Data/CraftingRecipes")) { return Globals.Config.CraftingRecipies.Randomize; }
			if (asset.AssetNameEquals("Data/Bundles")) { return Globals.Config.Bundles.Randomize; }
			if (asset.AssetNameEquals("Data/Blueprints")) { return Globals.Config.RandomizeBuildingCosts; }
			if (asset.AssetNameEquals("Strings/StringsFromCSFiles")) { return true; }
			if (asset.AssetNameEquals("Strings/UI")) { return true; }
			if (asset.AssetNameEquals("Data/ObjectInformation")) { return true; }
			if (asset.AssetNameEquals("Data/Fish")) { return Globals.Config.Fish.Randomize; }
			if (asset.AssetNameEquals("Data/Quests") || asset.AssetNameEquals("Data/mail")) { return Globals.Config.RandomizeQuests; }
			if (asset.AssetNameEquals("Data/Locations")) { return Globals.Config.Fish.Randomize || Globals.Config.RandomizeForagables || Globals.Config.AddRandomArtifactItem; }
			if (asset.AssetNameEquals("Strings/Locations")) { return Globals.Config.Crops.Randomize; } // For now, as the only thing is the sweet gem berry text
			if (asset.AssetNameEquals("Data/fruitTrees")) { return Globals.Config.RandomizeFruitTrees; }
			if (asset.AssetNameEquals("Data/Crops")) { return Globals.Config.Crops.Randomize; }
			if (asset.AssetNameEquals("Data/TV/CookingChannel")) { return Globals.Config.Crops.Randomize || Globals.Config.Fish.Randomize; }
			if (asset.AssetNameEquals("Data/weapons")) { return Globals.Config.Weapons.Randomize; }
			if (asset.AssetNameEquals("Data/Boots")) { return Globals.Config.Boots.Randomize; }
			if (asset.AssetNameEquals("Data/Monsters")) { return Globals.Config.Monsters.Randomize; }
			if (asset.AssetNameEquals("Data/NPCDispositions")) { return Globals.Config.RandomizeNPCBirthdays; }

			return false;
		}

		private void ApplyEdits<TKey, TValue>(IAssetData asset, IDictionary<TKey, TValue> edits)
		{
			IAssetDataForDictionary<TKey, TValue> assetDict = asset.AsDictionary<TKey, TValue>();
			foreach (KeyValuePair<TKey, TValue> edit in edits)
			{
				assetDict.Data[edit.Key] = edit.Value;
			}
		}

		public void Edit<T>(IAssetData asset)
		{
			if (asset.AssetNameEquals("Data/CraftingRecipes"))
			{
				this.ApplyEdits(asset, this._recipeReplacements);
			}
			else if (asset.AssetNameEquals("Data/Bundles"))
			{
				this.ApplyEdits(asset, this._bundleReplacements);
			}
			else if (asset.AssetNameEquals("Data/Blueprints"))
			{
				this.ApplyEdits(asset, this._blueprintReplacements);
			}
			else if (asset.AssetNameEquals("Strings/StringsFromCSFiles"))
			{
				this.ApplyEdits(asset, this._grandpaStringReplacements);
				this.ApplyEdits(asset, this._stringReplacements);
			}
			else if (asset.AssetNameEquals("Strings/UI"))
			{
				this.ApplyEdits(asset, this._uiStringReplacements);
			}
			else if (asset.AssetNameEquals("Data/ObjectInformation"))
			{
				if (IgnoreObjectInformationReplacements)
				{
					this.ApplyEdits(asset, new Dictionary<int, string>());
				}
				else
				{
					this.ApplyEdits(asset, this._objectInformationReplacements);
				}
			}
			else if (asset.AssetNameEquals("Data/Fish"))
			{
				this.ApplyEdits(asset, this._fishReplacements);
			}
			else if (asset.AssetNameEquals("Data/Quests"))
			{
				this.ApplyEdits(asset, this._questReplacements);
			}
			if (asset.AssetNameEquals("Data/mail"))
			{
				this.ApplyEdits(asset, this._mailReplacements);
			}
			else if (asset.AssetNameEquals("Data/Locations"))
			{
				this.ApplyEdits(asset, this._locationsReplacements);
			}
			else if (asset.AssetNameEquals("Strings/Locations"))
			{
				this.ApplyEdits(asset, this._locationStringReplacements);
			}
			else if (asset.AssetNameEquals("Data/fruitTrees"))
			{
				this.ApplyEdits(asset, this._fruitTreeReplacements);
			}
			else if (asset.AssetNameEquals("Data/Crops"))
			{
				this.ApplyEdits(asset, this._cropReplacements);
			}
			else if (asset.AssetNameEquals("Data/TV/CookingChannel"))
			{
				this.ApplyEdits(asset, this._cookingChannelReplacements);
			}
			else if (asset.AssetNameEquals("Data/weapons"))
			{
				this.ApplyEdits(asset, this._weaponReplacements);
			}
			else if (asset.AssetNameEquals("Data/Boots"))
			{
				this.ApplyEdits(asset, this._bootReplacements);
			}
			else if (asset.AssetNameEquals("Data/Monsters"))
			{
				this.ApplyEdits(asset, this._monsterReplacements);
			}
			else if (asset.AssetNameEquals("Data/NPCDispositions"))
			{
				this.ApplyEdits(asset, this._birthdayReplacements);
			}
		}

		public void InvalidateCache()
		{
			this._mod.Helper.Content.InvalidateCache("Data/CraftingRecipes");
			this._mod.Helper.Content.InvalidateCache("Data/Bundles");
			this._mod.Helper.Content.InvalidateCache("Data/Blueprints");
			this._mod.Helper.Content.InvalidateCache("Strings/StringsFromCSFiles");
			this._mod.Helper.Content.InvalidateCache("Strings/UI");
			this._mod.Helper.Content.InvalidateCache("Data/ObjectInformation");
			this._mod.Helper.Content.InvalidateCache("Data/Events/Farm");
			this._mod.Helper.Content.InvalidateCache("Data/Fish");
			this._mod.Helper.Content.InvalidateCache("Data/Quests");
			this._mod.Helper.Content.InvalidateCache("Data/mail");
			this._mod.Helper.Content.InvalidateCache("Data/Locations");
			this._mod.Helper.Content.InvalidateCache("Strings/Locations");
			this._mod.Helper.Content.InvalidateCache("Data/fruitTrees");
			this._mod.Helper.Content.InvalidateCache("Data/Crops");
			this._mod.Helper.Content.InvalidateCache("Data/TV/CookingChannel");
			this._mod.Helper.Content.InvalidateCache("Data/weapons");
			this._mod.Helper.Content.InvalidateCache("Data/Boots");
			this._mod.Helper.Content.InvalidateCache("Data/Monsters");
			this._mod.Helper.Content.InvalidateCache("Data/NPCDispositions");
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
			this._mod.Helper.Content.InvalidateCache("Strings/UI");
		}

		public void CalculateEdits()
		{
			ItemList.Initialize();
			ValidateItemList();

			EditedObjectInformation editedObjectInfo = new EditedObjectInformation();
			FishRandomizer.Randomize(editedObjectInfo);
			_fishReplacements = editedObjectInfo.FishReplacements;

			CropRandomizer.Randomize(editedObjectInfo);
			_fruitTreeReplacements = editedObjectInfo.FruitTreeReplacements;
			_cropReplacements = editedObjectInfo.CropsReplacements;
			_objectInformationReplacements = editedObjectInfo.ObjectInformationReplacements;

			_blueprintReplacements = BlueprintRandomizer.Randomize();
			_monsterReplacements = MonsterRandomizer.Randomize(); // Must be done before recipes since rarities of drops change
			_locationsReplacements = LocationRandomizer.Randomize(); // Must be done before recipes because of wild seeds
			_recipeReplacements = CraftingRecipeRandomizer.Randomize();
			_stringReplacements = StringsAdjustments.GetCSFileStringReplacements();
			_locationStringReplacements = StringsAdjustments.GetLocationStringReplacements();
			_bundleReplacements = BundleRandomizer.Randomize();
			MusicRandomizer.Randomize();

			QuestInformation questInfo = QuestRandomizer.Randomize();
			_questReplacements = questInfo.QuestReplacements;
			_mailReplacements = questInfo.MailReplacements;

			CraftingRecipeAdjustments.FixCookingRecipeDisplayNames();
			_cookingChannelReplacements = CookingChannel.GetTextEdits();

			_weaponReplacements = WeaponRandomizer.Randomize();
			_bootReplacements = BootRandomizer.Randomize();
			_birthdayReplacements = BirthdayRandomizer.Randomize();
		}

		/// <summary>
		/// Turns on the flag to ignore object information replacements and invalidates the cache
		/// so that the original values are reloaded
		/// </summary>
		public void UndoObjectInformationReplacements()
		{
			IgnoreObjectInformationReplacements = true;
			this._mod.Helper.Content.InvalidateCache("Data/ObjectInformation");
		}

		/// <summary>
		/// Turns off the flag to ignore object information replacements and invalidates the cache
		/// so that the randomized values are reloaded
		/// </summary>
		public void RedoObjectInformationReplacements()
		{
			IgnoreObjectInformationReplacements = false;
			this._mod.Helper.Content.InvalidateCache("Data/ObjectInformation");
		}

		/// <summary>
		/// Validates that all the items in the ObjectIndexes exist in the main item list
		/// </summary>
		private void ValidateItemList()
		{
			foreach (ObjectIndexes index in Enum.GetValues(typeof(ObjectIndexes)).Cast<ObjectIndexes>())
			{
				if (!ItemList.Items.ContainsKey((int)index))
				{
					Globals.ConsoleWarn($"Missing item: {(int)index}: {index.ToString()}");
				}
			}
		}
	}
}