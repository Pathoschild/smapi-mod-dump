/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JoXW100/StardewValleyMods
**
*************************************************/

namespace MoveableMailbox
{
    public interface IJsonAssetsApi
    {
        void LoadAssets(string path);
        int GetBigCraftableId(string name);
    }
}