/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DiogoAlbano/StardewValleyMods
**
*************************************************/

using System.Linq;

namespace BNWCore
{
    public class Api : IApi
    {
        public bool AddExclusion(string name)
        {
            if (Statics.ExcludedFish.Any(x => x.ToLower() == name.ToLower()))
            {
                return false;
            }
            Statics.ExcludedFish.Add(name);
            return true;
        }
        public int GetBNWCoreMagicNetId() => ModEntry.BNWCoreMagicNetId;
    }
    public interface IApi
    {
        bool AddExclusion(string name);
        int GetBNWCoreMagicNetId();
    }
}
