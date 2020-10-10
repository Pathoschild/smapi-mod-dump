/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/FarmTypeManager
**
*************************************************/

using StardewModdingAPI;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        //a simple class used to package FarmConfig and InternalSaveData together with the content pack providing them
        private class FarmData
        {
            public FarmConfig Config { get; set; }
            public InternalSaveData Save { get; set; }
            public IContentPack Pack { get; set; } //NOTE: this should be null when no content pack was involved, i.e. the data came from files in the mod's own folder

            public FarmData(FarmConfig config, InternalSaveData save, IContentPack pack)
            {
                Config = config;
                Save = save;
                Pack = pack;
            }
        }
    }
}