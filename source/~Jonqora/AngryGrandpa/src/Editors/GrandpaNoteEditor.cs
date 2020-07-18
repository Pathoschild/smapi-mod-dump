using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace AngryGrandpa
{
	/// <summary>The class for editing data assets related to grandpa's note.</summary>
	internal class GrandpaNoteEditor : IAssetEditor
	{
		/*********
        ** Accessors
        *********/
		protected static IModHelper Helper => ModEntry.Instance.Helper;
		protected static IMonitor Monitor => ModEntry.Instance.Monitor;
		protected static ModConfig Config => ModConfig.Instance;


		/*********
        ** Fields
        *********/
		protected static ITranslationHelper i18n = Helper.Translation;


		/*********
        ** Public methods
        *********/
		/// <summary>Get whether this instance can edit the given asset.</summary>
		/// <typeparam name="_T">The asset Type.</typeparam>
		/// <param name="asset">Basic metadata about the asset being loaded.</param>
		/// <returns>true for asset Strings\Locations or Data\mail, false otherwise</returns>
		public bool CanEdit<_T> (IAssetInfo asset)
		{
			return asset.AssetNameEquals($"Strings\\Locations") ||
				asset.AssetNameEquals($"Data\\mail");
		}

		/// <summary>Edit the Strings\Locations entry for grandpa's note, and add a corresponding entry to Data\mail.</summary>
		/// <typeparam name="_T">The asset Type</typeparam>
		/// <param name="asset">A helper which encapsulates metadata about an asset and enables changes to it</param>
		public void Edit<_T> (IAssetData asset)
		{
			string gameKey;
			string modKey;
			string value;

			// Get the game's data key, and the corresponding key for it in mod data
			if (asset.AssetNameEquals($"Strings\\Locations"))
			{
				gameKey = "Farm_GrandpaNote";
				modKey = "GrandpaNote";
			}
			else if (asset.AssetNameEquals($"Data\\mail"))
			{
				gameKey = "6324grandpaNoteMail";
				modKey = "GrandpaNoteMail";
			}
			else { return; }

			// Get and edit the appropriate string values
			if (Config.YearsBeforeEvaluation >= 10)
			{
				modKey += "TenPlusYears";
				string smapiSDate = new SDate(1, "spring", Config.YearsBeforeEvaluation + 1).ToLocaleString();
				value = i18n.Get(modKey, new { smapiSDate });
			}
			else // YearsBeforeEvaluation < 10
			{
				string ordinalYear = i18n.Get("GrandpaOrdinalYears").ToString().Split('|')[Config.YearsBeforeEvaluation];
				value = i18n.Get(modKey, new { ordinalYear });
			}

			// Apply the changes to game data
			var data = asset.AsDictionary<string, string>().Data;
			data[gameKey] = value;
		}
	}
}