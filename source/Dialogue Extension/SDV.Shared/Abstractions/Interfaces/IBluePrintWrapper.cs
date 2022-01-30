/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/DialogueExtension
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace SDV.Shared.Abstractions
{
  public interface IBluePrintWrapper : IWrappedType<BluePrint>
  {
    void consumeResources();
    int getTileSheetIndexForStructurePlacementTile(int x, int y);
    bool isUpgrade();
    bool doesFarmerHaveEnoughResourcesToBuild();
    void drawDescription(SpriteBatch b, int x, int y, int width);
  }
}