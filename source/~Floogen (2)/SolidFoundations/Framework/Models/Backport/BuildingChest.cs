/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/SolidFoundations
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace SolidFoundations.Framework.Models.Backport
{
    // TODO: When updated to SDV v1.6, this class should be deleted in favor of using StardewValley.GameData.BuildingChest
    public class BuildingChest
    {
        public enum ChestType
        {
            Chest,
            Collect,
            Load
        }

        public string Name;

        public ChestType Type;

        [ContentSerializer(Optional = true)]
        public string Sound;

        [ContentSerializer(Optional = true)]
        public string InvalidItemMessage;

        [ContentSerializer(Optional = true)]
        public string InvalidCountMessage;

        [ContentSerializer(Optional = true)]
        public string ChestFullMessage;

        [ContentSerializer(Optional = true)]
        public Vector2 DisplayTile = new Vector2(-1f, -1f);

        [ContentSerializer(Optional = true)]
        public float DisplayHeight;
    }
}