/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ophaneom/Mini-Bars
**
*************************************************/

using System;
using System.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Monsters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Threading;

namespace MiniBars.Framework.Rendering
{
    public class Renderer
    {
        private static int _verification_range = 100 * Game1.pixelZoom;

        public static void OnRendered(object sender, RenderedWorldEventArgs e)
        {
            if (!Context.IsWorldReady) return;
            if (Game1.activeClickableMenu != null || Game1.currentMinigame != null) return;

            foreach (Monster _monster in Game1.currentLocation.characters.OfType<Monster>())
            {
                if (_monster.IsInvisible || !Utility.isOnScreen(_monster.position, 3 * Game1.tileSize) ||
                    Compatibility.IsBlackListed(_monster.Name)) continue;

                if ((_monster.Position.X > Game1.player.Position.X + _verification_range ||
                    _monster.Position.X < Game1.player.Position.X - _verification_range ||
                    _monster.Position.Y > Game1.player.Position.Y + _verification_range ||
                    _monster.Position.Y < Game1.player.Position.Y - _verification_range) &&
                    ModEntry.config.Range_Verification) continue;

                float _current_health = _monster.Health;
                float _current_max_health = Math.Max(_monster.Health, _monster.MaxHealth);
                _monster.MaxHealth = (int)_current_max_health;
                if (!ModEntry.config.Show_Full_Life && _current_health >= _current_max_health) continue;

                string _prefix = "";
                if (_monster.isHardModeMonster) _prefix = "Hardmode";
                BarInformations _informations = Textures.barInformations.Find(x => x.monsterName == _prefix + _monster.Name) ??
                    Textures.barInformations.Find(x => x.monsterType == _monster.GetType().Name) ??
                    Textures.barInformations.Find(x => x.monsterType == "Default Theme");
                if (_monster is GreenSlime _slime)
                {
                    if (_monster.Name != "Tiger Slime")
                    {
                        Color c1 = _slime.color;
                        float cR = c1.R * 1.25f;
                        float cG = c1.G * 1.25f;
                        float cB = c1.B * 1.25f;
                        byte cA = 255;
                        Color c2 = new Color((int)cR, (int)cG, (int)cB, cA);

                        _informations.barColor = c2;
                        _informations.borderColor = _slime.color;
                    }
                }
                else if (_monster is Bug _bug && _bug.isArmoredBug)
                {
                    _informations = Textures.barInformations.Find(x => x.monsterName == "Armored Bug") ?? Textures.barInformations.Find(x => x.monsterType == _monster.GetType().Name);
                }
                else if (_monster is Fly && (Game1.player.currentLocation.Name == "BugLand" || Game1.CurrentMineLevel > 120))
                {
                    _informations = Textures.barInformations.Find(x => x.monsterName == "Mutant Fly") ?? Textures.barInformations.Find(x => x.monsterType == _monster.GetType().Name);
                }
                else if (_monster is Grub && (Game1.player.currentLocation.Name == "BugLand" || Game1.CurrentMineLevel > 120))
                {
                    _informations = Textures.barInformations.Find(x => x.monsterName == "Mutant Grub") ?? Textures.barInformations.Find(x => x.monsterType == _monster.GetType().Name);
                }
                else if (_monster is RockCrab && _monster.name == "Stick Bug")
                {
                    _informations = Textures.barInformations.Find(x => x.monsterName == "Stick Bug") ?? Textures.barInformations.Find(x => x.monsterType == _monster.GetType().Name);
                }
                else if (_monster is RockGolem && _monster.wildernessFarmMonster)
                {
                    _informations = Textures.barInformations.Find(x => x.monsterName == "Wilderness Golem") ?? Textures.barInformations.Find(x => x.monsterType == _monster.GetType().Name);
                }
                else if (_monster is Shooter)
                {
                    _informations = Textures.barInformations.Find(x => x.monsterName == "Shadow Brute") ?? Textures.barInformations.Find(x => x.monsterType == _monster.GetType().Name);
                }
                else if (_monster is BigSlime _bigSlime)
                {
                    Color c1 = _bigSlime.c;
                    float cR = c1.R * 1.25f;
                    float cG = c1.G * 1.25f;
                    float cB = c1.B * 1.25f;
                    byte cA = 255;
                    Color c2 = new Color((int)cR, (int)cG, (int)cB, cA);

                    _informations.barColor = c2;
                    _informations.borderColor = _bigSlime.c;
                }
                if (_monster is RockCrab && _monster.Sprite.CurrentFrame % 4 == 0) continue;
                else if (_monster is RockGolem && _monster.Sprite.CurrentFrame == 16) continue;
                else if (_monster is Spiker) continue;

                Texture2D _current_sprite = _informations.texture;
                Color _bar_color = _informations.barColor;
                Color _border_color = _informations.borderColor;
                Color _hp_color = _informations.hpColor;
                int _height = _informations.heigth;
                Vector2 _monsterPos = _monster.getLocalPosition(Game1.viewport);

                if (Game1.player.currentLocation.Name == "CrimsonBadlands" || Game1.player.currentLocation.Name == "IridiumQuarry")
                {
                    _bar_color = new Color(192, 64, 45);
                    _border_color = new Color(192, 64, 45);
                }

                Game1.spriteBatch.Draw(
                    Textures.Pixel,
                    new Rectangle(
                (int)_monsterPos.X - (Textures.Pixel.Width * Game1.pixelZoom) / 2 + (_monster.Sprite.SpriteWidth * Game1.pixelZoom) / 2 - Database.distance_x * Game1.pixelZoom,
                (int)_monsterPos.Y - _monster.Sprite.SpriteHeight * Game1.pixelZoom - _height * Game1.pixelZoom + 8 * Game1.pixelZoom,
                        (Textures.Pixel.Width * Game1.pixelZoom) * Database.bar_size,
                        (Textures.Pixel.Height * Game1.pixelZoom) * 4),
                    new Color(0, 0, 0, 135));

                Game1.spriteBatch.Draw(
                    Textures.Pixel,
                    new Rectangle(
                        (int)_monsterPos.X - (Textures.Pixel.Width * Game1.pixelZoom) / 2 + (_monster.Sprite.SpriteWidth * Game1.pixelZoom) / 2 - Database.distance_x * Game1.pixelZoom,
                        (int)_monsterPos.Y - _monster.Sprite.SpriteHeight * Game1.pixelZoom - _height * Game1.pixelZoom + 8 * Game1.pixelZoom,
                        (Textures.Pixel.Width * Game1.pixelZoom) * (int)((_current_health / _current_max_health) * Database.bar_size),
                        (Textures.Pixel.Height * Game1.pixelZoom) * 4),
                    _bar_color);

                Game1.spriteBatch.Draw(
                    _current_sprite,
                    new Rectangle(
                        (int)_monsterPos.X - (_current_sprite.Width * Game1.pixelZoom) / 2 + (_monster.Sprite.SpriteWidth * Game1.pixelZoom) / 2,
                        (int)_monsterPos.Y - _monster.Sprite.SpriteHeight * Game1.pixelZoom - _height * Game1.pixelZoom,
                        _current_sprite.Width * Game1.pixelZoom,
                        _current_sprite.Height * Game1.pixelZoom),
                    _border_color);

                if (ModEntry.config.Bars_Theme == 2) continue;
                Game1.spriteBatch.Draw(
                Textures.hpSprite,
                new Rectangle(
                    (int)_monsterPos.X - (Textures.hpSprite.Width * Game1.pixelZoom) / 2 + (_monster.Sprite.SpriteWidth * Game1.pixelZoom) / 2,
                    (int)_monsterPos.Y - _monster.Sprite.SpriteHeight * Game1.pixelZoom - _height * Game1.pixelZoom,
                    Textures.hpSprite.Width * Game1.pixelZoom,
                    Textures.hpSprite.Height * Game1.pixelZoom),
                _hp_color);
            }
        }
    }
}
