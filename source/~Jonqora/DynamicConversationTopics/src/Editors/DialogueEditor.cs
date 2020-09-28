using StardewModdingAPI;
using StardewValley;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using static System.Math;

namespace DynamicConversationTopics
{
	/// <summary>The class for editing Dialogue data assets.</summary>
	internal class DialogueEditor : IAssetEditor
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
			return asset.DataType == typeof(Dictionary<string, string>) &&
			asset.AssetName.StartsWith(Path.Combine("Characters", "Dialogue", "_").TrimEnd('_'));
			//return ModConfig.AssetMatch(asset, "Characters\\Dialogue", ModConfig.NPCs);
		}

		/// <summary>
		/// Edit the character dialogue files to add new entries for various conversationTopics
		/// (Eventually) pull from i18n and if a key matching NPCName.conversationTopic exists, add its value under dialogue key conversationTopic.
		/// </summary>
		/// <typeparam name="_T">The asset Type</typeparam>
		/// <param name="asset">A helper which encapsulates metadata about an asset and enables changes to it</param>
		public void Edit<_T> (IAssetData asset)
		{
			var data = asset.AsDictionary<string, string>().Data;

			// Insert ConversationTopic dialogue
			if (false)
			{

			}

			//Insert dialogue for test topics
			foreach (string topic in ModEntry.Instance.TestTopics)
            {
				data[topic] = $"Test dialogue: {topic}#$b#More test dialogue: {topic}#$e#Even more test dialogue after a break: {topic}";
            }
		}
	}
}