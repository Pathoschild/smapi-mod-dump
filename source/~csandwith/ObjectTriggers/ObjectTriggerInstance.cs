/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/csandwith/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;

namespace ObjectTriggers
{
    public class ObjectTriggerInstance
    {
        public ObjectTriggerInstance(string key, Vector2 tile)
        {
            triggerKey = key;
            tilePosition = tile;
        }

        public string triggerKey;
        public Vector2 tilePosition;
        public int elapsed;
    }
}