/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/maxvollmer/DeepWoodsMod
**
*************************************************/

using xTile.Dimensions;

namespace DeepWoodsMod.API
{
    public interface IDeepWoodsExit
    {
        int ExitDirection { get; }
        Location Location { get; set; }
        string TargetLocationName { get; set; }
        Location TargetLocation { get; set; }
    }
}
