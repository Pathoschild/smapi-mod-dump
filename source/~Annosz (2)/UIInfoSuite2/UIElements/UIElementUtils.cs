/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Annosz/UIInfoSuite2
**
*************************************************/

using System.Linq;
using StardewValley;

namespace UIInfoSuite2.UIElements;

public static class UIElementUtils
{
  public static bool IsRenderingNormally()
  {
    bool[] conditions =
    {
      !Game1.game1.takingMapScreenshot,
      !Game1.eventUp,
      !Game1.viewportFreeze,
      !Game1.freezeControls,
      Game1.viewportHold <= 0,
      Game1.displayHUD
    };

    return conditions.All(condition => condition);
  }
}
