/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using System.Linq;

namespace Fishnets
{
    public interface IApi
    {
        /// <summary>
        /// Remove a fish from being able to be caught by a fish net
        /// </summary>
        /// <param name="name">The name of the fish to exclude</param>
        /// <returns>True if the fish was excluded, false if it wasn't (check trace logs)</returns>
        bool AddExclusion(string name);

        /// <summary>
        /// Get the ParentSheetIndex of the fish net object
        /// </summary>
        string GetId();
    }

    public class Api : IApi
    {
        public bool AddExclusion(string name)
        {
            if (Statics.ExcludedFish.Any(x => x.ToLower() == name.ToLower()))
            {
                ModEntry.IMonitor.Log($"{name} has already been excluded");
                return false;
            }
            ModEntry.IMonitor.Log($"Excluded {name} from fishnet drop list");
            Statics.ExcludedFish.Add(name);
            ModEntry.IHelper.Data.WriteJsonFile($"assets/excludedfish.json", Statics.ExcludedFish);
            return true;
        }

        public string GetId() => ModEntry.ObjectInfo.Id;
    }
}
