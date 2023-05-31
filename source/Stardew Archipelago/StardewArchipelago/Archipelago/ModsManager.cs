/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewArchipelago.Constants;
using StardewModdingAPI;

namespace StardewArchipelago.Archipelago
{
    public class ModsManager
    {
        private Dictionary<string, string> _activeMods;

        public ModsManager(Dictionary<string, string> activeMods)
        {
            _activeMods = activeMods;
        }

        public bool IsModded => _activeMods.Any();

        public bool HasMod(string modName)
        {
            return _activeMods.ContainsKey(modName);
        }

        public string GetVersion(string modName)
        {
            return _activeMods[modName];
        }

        public bool HasModdedSkill()
        {
            return HasMod(ModNames.LUCK) || HasMod(ModNames.BINNING) || HasMod(ModNames.COOKING) ||
                   HasMod(ModNames.MAGIC) || HasMod(ModNames.SOCIALIZING) || HasMod(ModNames.ARCHAEOLOGY);
        }

        public bool IsModStateCorrect(IModHelper modHelper, out string errorMessage)
        {
            var loadedModData = modHelper.ModRegistry.GetAll().ToList();
            errorMessage = $"The slot you are connecting to has been created expecting modded content,\r\nbut not all expected mods are installed and active.";
            var valid = true;
            foreach (var (mod, version) in _activeMods)
            {
                if (!IsModActiveAndCorrectVersion(loadedModData, mod, version, out var existingVersion))
                {
                    valid = false;
                    errorMessage +=
                        $"{Environment.NewLine}\tMod: {mod}, expected version: {version}, current Version: {existingVersion}";
                }
            }

            return valid;
        }

        private static bool IsModActiveAndCorrectVersion(List<IModInfo> loadedModData, string desiredModName, string desiredVersion, out string existingVersion)
        {
            var normalizedDesiredModName = GetNormalizedModName(desiredModName);
            foreach (var modInfo in loadedModData)
            {
                var modName = GetNormalizedModName(modInfo.Manifest.Name);
                if (!modName.Equals(normalizedDesiredModName, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                existingVersion = modInfo.Manifest.Version.ToString();
                if (existingVersion.Equals(desiredVersion, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                return false;
            }

            existingVersion = "[NOT FOUND]";
            return false;
        }

        private static string GetNormalizedModName(string modName)
        {
            var aliasedName = modName;
            if (ModInternalNames.InternalNames.ContainsKey(modName))
            {
                aliasedName = ModInternalNames.InternalNames[modName];
            }
            var cleanName = aliasedName
                .Replace(" ", "")
                .Replace("_", "")
                .Replace("-", "")
                .Replace("'", "")
                .Replace("(", "")
                .Replace(")", "")
                .Replace("[", "")
                .Replace("]", "");
            return cleanName;
        }
    }
}
