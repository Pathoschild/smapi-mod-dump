/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;

namespace ZoomPerMap;

public class ModConfig
{
    public float defaultOutdoorZoomLevel = 1f;
    public float defaultIndoorZoomLevel = 1.5f;
    public float defaultMineZoomLevel = 1f;
    public Dictionary<string, float> zoomLevels = new Dictionary<string, float>();
}
