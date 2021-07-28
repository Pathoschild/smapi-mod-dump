/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;

namespace GenericModConfigMenu.ModOption
{
    internal abstract class BaseModOption
    {
        public string Name { get; }
        public string Description { get; }

        public string Id { get; }

        public bool AvailableInGame { get; set; } = false;

        public IManifest Owner { get; }

        public abstract void SyncToMod();
        public abstract void Save();

        protected BaseModOption(string name, string desc, string id, IManifest mod)
        {
            this.Name = name;
            this.Description = desc;
            this.Id = id;
            this.Owner = mod;
        }
    }
}
