/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/stardew-access/stardew-access
**
*************************************************/

using StardewValley;
using StardewValley.GameData.Objects;

namespace stardew_access.Utils;

public static class ObjectUtils
{
    public static ObjectData? GetObjectById(string objectId)
    {
        if (objectId.StartsWith("(BC)")) return null;

        if (objectId.StartsWith("(O)"))
            objectId = objectId.Substring(3); // "(O)" is 3 characters long, so start after it

        if (!Game1.objectData.TryGetValue(objectId, out ObjectData? objectInfo) || objectInfo is null)
            Log.Error($"Object ID {objectId} does not exist.");

        return objectInfo;
    }

    public static ObjectData? GetObjectById(int objectId) => GetObjectById(objectId.ToString());
}
