/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Tlitookilakin/AeroCore
**
*************************************************/

using AeroCore.Models;
using AeroCore.Patches;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace AeroCore
{
    [ModInit]
    internal class User
    {
        private static readonly Vector2 tileSize = new(64, 64);
        private static readonly Vector2 offset = new(-32, -32);
        private static Vector2 tile = new();
        internal static bool isLightActive = false;
        private static Texture2D tex;
        internal static void Init()
        {
            tex = ModEntry.helper.ModContent.Load<Texture2D>("assets/cursorlight.png");
            Patches.Lighting.LightingEvent += DoLightDraw;
            ModEntry.helper.Events.Input.CursorMoved += UpdateMousePos;
            ModEntry.helper.Events.Input.ButtonPressed += ButtonPressed;
            ModEntry.helper.Events.Input.ButtonReleased += ButtonReleased;
        }
        private static void UpdateMousePos(object _, CursorMovedEventArgs ev)
            => tile = ev.NewPosition.Tile * tileSize + offset;
        private static void ButtonPressed(object _, ButtonPressedEventArgs ev)
        {
            if (Game1.activeClickableMenu is not null || !Context.IsWorldReady)
                return;

            if (ModEntry.Config.CursorLightBind.JustPressed())
                isLightActive = ModEntry.Config.CursorLightHold || !isLightActive;
            if (ModEntry.Config.PlaceBind.JustPressed())
                TryPlaceItem(ev.Cursor.GrabTile);
            if (ModEntry.Config.ReloadBind.JustPressed())
                Utils.Maps.ReloadCurrentLocation(true);
        }
        private static void ButtonReleased(object _, ButtonReleasedEventArgs ev)
        {
            if (ModEntry.Config.CursorLightHold)
                isLightActive = false;
        }
        private static void DoLightDraw(LightingEventArgs ev)
        {
            if (isLightActive)
                ev.batch.Draw(
                    tex, ev.GlobalToLocal(tile), null,
                    Color.Black * ModEntry.Config.CursorLightIntensity,
                    0f, Vector2.Zero, ev.scale * 4f, SpriteEffects.None, 0f
                );
        }
        private static void TryPlaceItem(Vector2 tile)
        {
            var where = Game1.player.currentLocation;
            var held = Game1.player.CurrentItem;
            if (held is null)
                return;
            var place = ItemWrapper.WrapItem(held.getOne(), true, true);
            if (tile != Game1.player.getTileLocation() && where.isTileLocationTotallyClearAndPlaceableIgnoreFloors(tile))
            {
                where.Objects[tile] = place;
                place.TileLocation = tile;
                place.IsSpawnedObject = true;
                PersistSpawned.SetPersist(place);
                Game1.player.reduceActiveItemByOne();
                where.playSoundAt("axchop", tile);
            } else
            {
                Game1.playSound("cancel");
            }
        }
    }
}
