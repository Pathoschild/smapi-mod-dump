using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using CIL = Harmony.CodeInstruction;

namespace DynamicConversationTopics
{
    public class Utilities
    {
        private static IMonitor Monitor => ModEntry.Instance.Monitor;
        internal protected static ModConfig Config => ModConfig.Instance;

        /// <summary>
        /// Checks if a given game asset is a dialogue file (string, string dictionary) from Characters/Dialogue/-NAME-
        /// </summary>
        /// <param name="asset">The asset to check.</param>
        /// <returns>true for any dialogue asset in Characters/Dialogue; false otherwise</returns>
        public static bool IsDialogueAsset(IAssetInfo asset)
        {
            return asset.DataType == typeof(Dictionary<string, string>) &&
                asset.AssetName.StartsWith(Path.Combine("Characters", "Dialogue", "_").TrimEnd('_'), StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Checks if a given game asset is an event file (string, string dictionary) from Data/Events/-LOCATION-
        /// </summary>
        /// <param name="asset">The asset to check.</param>
        /// <returns>true for any event data asset in Data/Events; false otherwise</returns>
        public static bool IsEventAsset(IAssetInfo asset)
        {
            return asset.DataType == typeof(Dictionary<string, string>) &&
                asset.AssetName.StartsWith(Path.Combine("Data", "Events", "_").TrimEnd('_'), StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Returns the file name (without extensions) of a given asset to help identify it.
        /// </summary>
        /// <param name="asset">The asset to use.</param>
        /// <returns>File name. E.g. "Wizard" from asset Characters\Dialogue\Wizard.xnb</returns>
        public static string GetTargetWithoutPath(IAssetInfo asset)
        {
            return Path.GetFileNameWithoutExtension(asset.AssetName);
        }

        /// <summary>
        /// Checks if an asset matches a given file location and file name.
        /// </summary>
        /// <param name="asset">The asset to compare.</param>
        /// <param name="path">Path to the containing folder.</param>
        /// <param name="targets">File name to check.</param>
        /// <returns>true if a match, false otherwise.</returns>
        public static bool AssetMatch(IAssetInfo asset, string path, string target)
        {
            return asset.AssetNameEquals(path + "\\" + target);
        }
        
        /// <summary>
        /// Checks if an asset matches a given file location and any file name from a given list.
        /// </summary>
        /// <param name="asset">The asset to compare.</param>
        /// <param name="path">Path to the containing folder.</param>
        /// <param name="targets">List of file names.</param>
        /// <returns>true if a match exists, false otherwise.</returns>
        public static bool AssetMatch(IAssetInfo asset, string path, List<string> targets)
        {
            foreach (string target in targets)
            {
                if (AssetMatch(asset, path, target))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Searches for a sublist (e.g., of CodeInstructions) within another list of the same Type. 
        /// </summary>
        /// <param name="target">The list to search through</param>
        /// <param name="match">The list of entries to locate in the target</param>
        /// <param name="startPos">Zero-indexed search start point in the target list</param>
        /// <param name="returnOffset">Zero-indexed entry in the match list to return the target index of, if found</param>
        /// <returns>null if no match found, else returns the index of the match (with offset if specified)</returns>
        public static int? findListMatch<T>(List<T> target, List<T> match, int startPos = 0, int returnOffset = 0)
        {
            if (startPos > target.Count - match.Count)
            {
                //Not enough room to find match in remaining list
                return null;
            }
            for (int i = startPos; i <= target.Count - match.Count; i++)
            {
                bool foundMatch = true;
                for (int j = 0; j < match.Count; j++)
                {
                    //Comparison for CodeInstruction lists
                    if (target is List<CIL>)
                    {
                        var ATarget = (CIL)(object)target[i + j];
                        var BMatch = (CIL)(object)match[j];

                        if (!ATarget.opcode.Equals(BMatch.opcode) ||
                            (BMatch.operand != null && !ATarget.operand.Equals(BMatch.operand)))
                        {
                            if (Config.DebugMode) Monitor.Log($"ATarget | {ATarget.opcode} | {ATarget.operand}\n" +
                                $"BMatch | {BMatch.opcode} | {BMatch.operand}\n", LogLevel.Debug);
                            foundMatch = false;
                            break;
                        }
                    }
                    else if (!target[i + j].Equals(match[j]))
                    {
                        foundMatch = false;
                        break;
                    }
                }
                if (foundMatch)
                {
                    return i + returnOffset;
                }
            }
            return null; //No match found
        }

        /// <summary>
        /// Searches a list (e.g., of CodeInstructions) for a sublist that matches a set of criteria. 
        /// </summary>
        /// <param name="target">The list to search through</param>
        /// <param name="criteria">The list of criteria (lambdas returning bool) to evaluate sublists in the target</param>
        /// <param name="startPos">Zero-indexed search start point in the target list</param>
        /// <param name="returnOffset">Zero-indexed entry in the match list to return the target index of, if found</param>
        /// <returns>null if no match found, else returns the index of the match (with offset if specified)</returns>
        public static int? findSublist<T>(List<T> target, List<Func<T,bool>> criteria, int startPos = 0, int returnOffset = 0)
        {
            if (startPos > target.Count - criteria.Count)
            {
                //Not enough room to find match in remaining list
                return null;
            }
            for (int i = startPos; i <= target.Count - criteria.Count; i++)
            {
                bool foundMatch = true;
                for (int j = 0; j < criteria.Count; j++)
                {
                    var targetItem = target[i + j];
                    var criteriaItem = criteria[j];

                    if (!criteriaItem(targetItem)) //Call the criteria function to evaluate the target item
                    {
                        if (Config.DebugMode && j > 0) Monitor.Log($"Returned false on target[{i + j}], criteria[{j}]\n" +
                            (targetItem is CIL ?
                            $"targetItem | {((CIL)(object)targetItem).opcode} | {((CIL)(object)targetItem).operand}" :
                            $"targetItem | {targetItem}"), LogLevel.Debug);

                        foundMatch = false;
                        break;
                    }
                    if (Config.DebugMode && targetItem is CIL) Monitor.Log($"Returned TRUE on target[{i + j}], criteria[{j}]\n" +
                            (targetItem is CIL ?
                            $"targetItem | {((CIL)(object)targetItem).opcode} | {((CIL)(object)targetItem).operand}" :
                            $"targetItem | {targetItem}"), LogLevel.Info);
                }
                if (foundMatch)
                {
                    return i + returnOffset;
                }
            }
            return null; //No match found
        }
    }
}
