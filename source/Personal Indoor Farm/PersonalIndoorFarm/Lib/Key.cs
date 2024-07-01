/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley/personal-indoor-farm
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using static PersonalIndoorFarm.ModEntry;

namespace PersonalIndoorFarm.Lib
{
    internal class Key
    {
        public const string ItemId = "DLX.PIF_Key";
        public const string QualifiedItemId = "(O)" + ItemId;

        public static void useOnDoor(Farmer who, string doorId)
        {
            var key = generateLockKey(doorId);
            var lockStatus = Key.getDoorLocked(who, doorId);
            if (lockStatus == DoorLockEnum.Locked) {
                Game1.currentLocation.playSound("give_gift", Game1.player.Tile);
                who.modData[key] = "F";

            } else if (lockStatus == DoorLockEnum.Unlocked) {
                Game1.currentLocation.playSound("select", Game1.player.Tile);
                who.modData.Remove(key);

            } else if (lockStatus == DoorLockEnum.LockedWhenOffline) {
                Game1.currentLocation.playSound("doorClose", Game1.player.Tile);
                who.modData[key] = "T";

            }
        }
        public static DoorLockEnum getDoorLocked(Farmer owner, string doorId)
        {
            if (!owner.modData.TryGetValue(generateLockKey(doorId), out var value))
                return DoorLockEnum.LockedWhenOffline;

            if (value == "F")
                return DoorLockEnum.Unlocked;

            if (value == "T")
                return DoorLockEnum.Locked;

            return DoorLockEnum.LockedWhenOffline;
        }

        public static string generateLockKey(string doorId) => "DLX.PIF_Lock_" + doorId;

        public static void drawOverlay(SpriteBatch b, Furniture door, string doorId, Farmer owner)
        {
            var color = getDoorLocked(owner, doorId) switch {
                DoorLockEnum.Locked => Config.LockedDoorColor,
                DoorLockEnum.Unlocked => Config.UnlockedDoorColor,
                DoorLockEnum.LockedWhenOffline or _ => Config.LockedWhenOfflineDoorColor,
            };
            var pos = new Vector2(door.TileLocation.X * 64 + (door.boundingBox.Width / 2) - 32, door.TileLocation.Y * 64 + (door.boundingBox.Height / 2) - 32);

            b.Draw(AssetRequested.SpriteSheetTexture, Game1.GlobalToLocal(pos), new Rectangle(128, 0, 16, 16), color, 0f, new Vector2(), 4f, SpriteEffects.None, 1f);
        }
    }
}
