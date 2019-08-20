using StardewModdingAPI;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>A class containing any save data relating specifically to a content pack.</summary>
        private class ContentPackSaveData
        {
            //class added in version 1.4.2; defaults used here to automatically fill in values with SMAPI's json interface
            public bool MainDataFolderReset { get; set; } = false;

            public ContentPackSaveData()
            {

            }
        }
    }
}