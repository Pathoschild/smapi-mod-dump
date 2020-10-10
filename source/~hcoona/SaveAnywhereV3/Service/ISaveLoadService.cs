/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/hcoona/StardewValleyMods
**
*************************************************/

using SaveAnywhereV3.DataContract;

namespace SaveAnywhereV3.Service
{
    public interface ISaveLoadService
    {
        bool Check();

        void Clear();

        void Commit();

        void Save();

        void Load();

        void SaveTo(AggregatedModel model);

        void LoadFrom(AggregatedModel model);
    }
}
