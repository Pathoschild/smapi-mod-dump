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
		private Dictionary<string, string> _grandpaStringReplacements = new Dictionary<string, string>();
		private Dictionary<string, string> _stringReplacements = new Dictionary<string, string>();
		private Dictionary<int, string> _fishReplacements = new Dictionary<int, string>();
		private Dictionary<int, string> _questReplacements = new Dictionary<int, string>();
		private Dictionary<string, string> _mailReplacements = new Dictionary<string, string>();
		private Dictionary<string, string> _locationsReplacements = new Dictionary<string, string>();
		private Dictionary<int, string> _objectInformationReplacements = new Dictionary<int, string>();
		private Dictionary<int, string> _fruitTreeReplacements = new Dictionary<int, string>();
		private Dictionary<int, string> _cropReplacements = new Dictionary<int, string>();
		private Dictionary<int, string> _weaponReplacements = new Dictionary<int, string>();
		private Dictionary<int, string> _bootReplacements = new Dictionary<int, string>();
		private Dictionary<string, string> _monsterReplacements = new Dictionary<string, string>();
		private Dictionary<string, string> _birthdayReplacements = new Dictionary<string, string>();
		public Dictionary<string, string> MusicReplacements = new Dictionary<string, string>();

		public AssetEditor(ModEntry mod)
		{
			this._mod = mod;
		}

		public bool CanEdit<T>(IAssetInfo asset)
		{
			if (asset.AssetNameEquals("Data/CraftingRecipes")) { return Globals.Config.RandomizeCraftingRecipes; }
			if (asset.AssetNameEquals("Data/Bundles")) { return Globals.Config.RandomizeBundles; }
			if (asset.AssetNameEquals("Data/Blueprints")) { return Globals.Config.RandomizeBuildingCosts; }
			if (asset.AssetNameEquals("Strings/StringsFromCSFiles")) { return true; }
			if (asset.AssetNameEquals("Data/ObjectInformation")) { return true; }
			if (asset.AssetNameEquals("Data/Fish")) { return Globals.Config.RandomizeFish; }
			if (asset.AssetNameEquals("Data/Quests") || asset.AssetNameEquals("Data/mail")) { return Globals.Config.RandomizeQuests; }
			if (asset.AssetNameEquals("Data/Locations")) { return Globals.Config.RandomizeFish || Globals.Config.RandomizeForagables || Globals.Config.AddRandomArtifactItem; }
			if (asset.AssetNameEquals("Data/fruitTrees")) { return Globals.Config.RandomizeFruitTrees; }
			if (asset.AssetNameEquals("Data/Crops")) { return Globals.Config.RandomizeCrops; }
			if (asset.AssetNameEquals("Data/weapons")) { return Globals.Config.RandomizeWeapons; }
			if (asset.AssetNameEquals("Data/Boots")) { return Globals.Config.RandomizeBoots; }
			if (asset.AssetNameEquals("Data/Monsters")) { return Globals.Config.RandomizeMonsters; }
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
			else if (asset.AssetNameEquals("Data/ObjectInformation"))
			{
				this.ApplyEdits(asset, this._objectInformationReplacements);
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
			else if (asset.AssetNameEquals("Data/fruitTrees"))
			{
				this.ApplyEdits(asset, this._fruitTreeReplacements);
			}
			else if (asset.AssetNameEquals("Data/Crops"))
			{
				this.ApplyEdits(asset, this._cropReplacements);
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
			this._mod.Helper.Content.InvalidateCache("Data/ObjectInformation");
			this._mod.Helper.Content.InvalidateCache("Data/Events/Farm");
			this._mod.Helper.Content.InvalidateCache("Data/Fish");
			this._mod.Helper.Content.InvalidateCache("Data/Quests");
			this._mod.Helper.Content.InvalidateCache("Data/mail");
			this._mod.Helper.Content.InvalidateCache("Data/Locations");
			this._mod.Helper.Content.InvalidateCache("Data/fruitTrees");
			this._mod.Helper.Content.InvalidateCache("Data/Crops");
			this._mod.Helper.Content.InvalidateCache("Data/weapons");
			this._mod.Helper.Content.InvalidateCache("Data/Boots");
			this._mod.Helper.Content.InvalidateCache("Data/Monsters");
			this._mod.Helper.Content.InvalidateCache("Data/NPCDispositions");
		}

		public void CalculateEditsBeforeLoad()
		{
			_grandpaStringReplacements = StringsRandomizer.RandomizeGrandpasStory();
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
			_recipeReplacements = CraftingRecipeRandomizer.Randomize();
			_stringReplacements = StringsRandomizer.Randomize();
			_locationsReplacements = LocationRandomizer.Randomize();
			_bundleReplacements = BundleRandomizer.Randomize();
			MusicReplacements = MusicRandomizer.Randomize();

			QuestInformation questInfo = QuestRandomizer.Randomize();
			_questReplacements = questInfo.QuestReplacements;
			_mailReplacements = questInfo.MailReplacements;

			_weaponReplacements = WeaponRandomizer.Randomize();
			_bootReplacements = BootRandomizer.Randomize();
			_birthdayReplacements = BirthdayRandomizer.Randomize();
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