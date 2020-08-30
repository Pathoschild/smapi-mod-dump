using StardewModdingAPI;
using StardewValley;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using static System.Math;

namespace DynamicConversationTopics
{
	/// <summary>The class for editing Event data assets.</summary>
	internal class EventEditor : IAssetEditor
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
		/// <returns>true for asset Data\Events\***, false otherwise</returns>
		public bool CanEdit<_T> (IAssetInfo asset)
		{
			return ModConfig.AssetMatch(asset, "Data\\Events", ModConfig.EventLocations);
		}

		/// <summary>
		/// Edit the event scripts in the file.
		/// For main insertions, place the addConversationTopic command immediately after the 3rd \
		/// For fork insertions, place the addConversationTopic after the regex pattern match.
		/// </summary>
		/// <typeparam name="_T">The asset Type</typeparam>
		/// <param name="asset">A helper which encapsulates metadata about an asset and enables changes to it</param>
		public void Edit<_T> (IAssetData asset)
		{
			var data = asset.AsDictionary<string, string>().Data;

			// Insert addConversationTopic commands for main events and fork paths
			if (false)
			{

			}
		}
	}
}