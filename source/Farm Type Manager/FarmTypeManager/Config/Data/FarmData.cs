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