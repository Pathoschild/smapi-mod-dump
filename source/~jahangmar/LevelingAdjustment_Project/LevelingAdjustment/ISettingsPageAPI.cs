/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jahangmar/StardewValleyMods
**
*************************************************/

namespace ModSettingsTab.Framework.Interfaces
{
    public interface ISettingsPageApi
    {
        //IModOptions GetOptions(string uniqueId);

        void DisableStaticConfig(string uniqueId);

        void DisableStaticConfig(string uniqueId, string path);
    }
}