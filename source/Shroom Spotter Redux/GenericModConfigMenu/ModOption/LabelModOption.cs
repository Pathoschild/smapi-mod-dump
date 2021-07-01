/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;

namespace GenericModConfigMenu.ModOption
{
    internal class LabelModOption : BaseModOption
    {

        public override void SyncToMod()
        {
        }

        public override void Save()
        {
        }

        public LabelModOption( string name, string desc, IManifest mod )
        :   base( name, desc, name, mod )
        {
        }
    }
}
