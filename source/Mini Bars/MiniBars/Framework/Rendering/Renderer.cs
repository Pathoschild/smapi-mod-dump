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
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Monsters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiniBars.Framework.Rendering
{
    public class Renderer
    {
        private static int _verification_range = ModEntry.config.Distance * Game1.pixelZoom;
        public static void OnRendered(object sender, RenderedWorldEventArgs e)
        {
            if (!Context.IsWorldReady) return;
            if (Game1.activeClickableMenu != null || Game1.currentMinigame != null) return;

            foreach (Monster monster in Game1.currentLocation.characters.OfType<Monster>())
            {
                if (monster.IsInvisible || !Utility.isOnScreen(monster.position, 3 * Game1.tileSize)) continue;

                if ((monster.Position.X > Game1.player.Position.X + _verification_range ||
                    monster.Position.X < Game1.player.Position.X - _verification_range ||
                    monster.Position.Y > Game1.player.Position.Y + _verification_range ||
                    monster.Position.Y < Game1.player.Position.Y - _verification_range) &&
                    ModEntry.config.Range_Verification) continue;


                Texture2D _current_sprite;
                Color _bar_color = new Color(172, 50, 50);
                Color _border_color = Color.White;
                Color _hp_color = new Color(0, 0, 0, 200);
                int _height = 3;
                float _current_health = monster.Health;
                float _current_max_health = Math.Max(monster.Health, monster.MaxHealth);
                monster.MaxHealth = (int)_current_max_health;

                if (_current_health >= _current_max_health && !ModEntry.config.Show_Full_Life) continue;

                if (Compatibility.BlackListedMonster(monster.Name)) continue;

                if (monster is GreenSlime slime)
                {
                    if (monster.Name == "Frost Jelly")
                    {
                        _current_sprite = Textures.blueslime_theme;
                        _bar_color = new Color(69, 142, 212);
                        _border_color = slime.color;
                    }
                    else if (monster.Name == "Tiger Slime")
                    {
                        _current_sprite = Textures.tigerslime_theme;
                        _bar_color = new Color(174, 84, 18);
                    }
                    else
                    {
                        Color c1 = slime.color;
                        float cR = c1.R * 1.25f;
                        float cG = c1.G * 1.25f;
                        float cB = c1.B * 1.25f;
                        byte cA = 255;
                        Color c2 = new Color((int)cR, (int)cG, (int)cB, cA);

                        _current_sprite = Textures.greenslime_theme;
                        _bar_color = c2;
                        _border_color = slime.color;
                    }
                }
                else if (monster is Bat bat)
                {
                    if (monster.Name == "Frost Bat")
                    {
                        if (bat.isHardModeMonster)
                        {
                            _current_sprite = Textures.hardmode_frostbat;
                            _bar_color = new Color(231, 116, 10);
                        }
                        else
                        {
                            _current_sprite = Textures.frostbat_theme;
                            _bar_color = new Color(77, 122, 145);
                        }
                    }
                    else if (monster.Name == "Lava Bat")
                    {
                        _current_sprite = Textures.lavabat_theme;
                        _bar_color = new Color(190, 23, 23);
                    }
                    else if (monster.Name == "Magma Sprite")
                    {
                        _current_sprite = Textures.magmasprite_theme;
                        _bar_color = new Color(255, 204, 51);
                    }
                    else if (monster.Name == "Magma Sparker")
                    {
                        _current_sprite = Textures.magmasparker_theme;
                        _bar_color = new Color(221, 250, 117);
                    }
                    else if (monster.Name == "Iridium Bat")
                    {
                        _current_sprite = Textures.iridiumbat_theme;
                        _bar_color = new Color(226, 72, 205);
                    }
                    else if (monster.Name == "Haunted Skull")
                    {
                        _current_sprite = Textures.hauntedskull_theme;
                        _bar_color = new Color(172, 172, 172);
                        _hp_color = Color.Red;
                    }
                    else
                    {
                        _current_sprite = Textures.bat_theme;
                    }
                }
                else if (monster is Bug bug)
                {
                    if (bug.isArmoredBug.Value)
                    {
                        _current_sprite = Textures.armoredbug_theme;
                    }
                    else if (bug.isHardModeMonster)
                    {
                        _current_sprite = Textures.hardmode_bug;
                        _bar_color = new Color(0, 127, 199);
                    }
                    else
                    {
                        _current_sprite = Textures.bug_theme;
                        _bar_color = new Color(130, 199, 171);
                    }
                    _height = 15;
                }
                else if (monster is Fly)
                {
                    if (Game1.player.currentLocation.Name == "BugLand" || Game1.CurrentMineLevel > 120)
                    {
                        _current_sprite = Textures.mutantfly_theme;
                        _bar_color = new Color(0, 194, 31);
                    }
                    else
                    {
                        _current_sprite = Textures.cavefly_theme;
                        _bar_color = new Color(207, 43, 156);
                    }
                }
                else if (monster is Duggy duggy)
                {
                    if (monster.Name == "Magma Duggy")
                    {
                        _current_sprite = Textures.magmaduggy_theme;
                        _bar_color = new Color(246, 59, 59);
                    }
                    else if (duggy.isHardModeMonster)
                    {
                        _current_sprite = Textures.hardmode_duggy;
                        _bar_color = new Color(50, 99, 172);
                    }
                    else
                    {
                        _current_sprite = Textures.duggy_theme;
                    }
                }
                else if (monster is Grub)
                {
                    if (Game1.player.currentLocation.Name == "BugLand" || Game1.CurrentMineLevel > 120)
                    {
                        _current_sprite = Textures.mutantgrub_theme;
                        _bar_color = new Color(75, 242, 63);
                    }
                    else
                    {
                        _current_sprite = Textures.grub_theme;
                        _bar_color = new Color(255, 215, 135);
                    }
                }
                else if (monster is RockCrab crab)
                {
                    if (monster.Sprite.CurrentFrame % 4 == 0) continue;
                    if (monster.Name == "Lava Crab")
                    {
                        if (crab.isHardModeMonster)
                        {
                            
                            _current_sprite = Textures.hardmode_lavacrab;
                            _bar_color = new Color(135, 17, 168);
                        }
                        else
                        {
                            _current_sprite = Textures.lavacrab_theme;
                            _bar_color = new Color(172, 50, 101);
                        }
                    }
                    else if (monster.Name == "Iridium Crab")
                    {
                        _current_sprite = Textures.iridiumcrab_theme;
                        _bar_color = new Color(226, 72, 205);
                    }
                    else
                    {
                        if (crab.isHardModeMonster)
                        {
                            if (monster.Name == "Stick Bug")
                            {
                                _current_sprite = Textures.hardmode_stickbug;
                                _bar_color = new Color(139, 65, 0);
                            }
                            else
                            {
                                _current_sprite = Textures.hardmode_crab;
                                _bar_color = new Color(46, 74, 116);
                            }
                        }
                        else
                        {
                            _current_sprite = Textures.rockcrab_theme;
                        }
                    }
                }
                else if (monster is RockGolem golem)
                {
                    if (monster.Sprite.CurrentFrame == 16) continue;
                    if (golem.wildernessFarmMonster)
                    {
                        _current_sprite = Textures.wildernessgolem_theme;
                        _bar_color = new Color(78, 62, 41);
                    }
                    else
                    {
                        _current_sprite = Textures.stonegolem_theme;
                        _bar_color = new Color(121, 121, 121);
                    }
                }
                else if (monster is DustSpirit dust)
                {
                    if (dust.isHardModeMonster)
                    {
                        _current_sprite = Textures.hardmode_dust;
                        _bar_color = new Color(255, 255, 255);
                    }
                    else
                    {
                        _current_sprite = Textures.dust_theme;
                        _bar_color = new Color(86, 88, 104);
                    }
                }
                else if (monster is Ghost)
                {
                    if (monster.Name == "Putrid Ghost")
                    {
                        _current_sprite = Textures.hardmode_putridghost;
                        _bar_color = new Color(164, 238, 135);
                    }
                    else
                    {
                        _current_sprite = Textures.ghost_theme;
                        _bar_color = new Color(197, 238, 155);
                    }
                }
                else if (monster is Skeleton skeleton)
                {
                    if (skeleton.isHardModeMonster)
                    {
                        _current_sprite = Textures.hardmode_skeleton;
                        _bar_color = new Color(207, 250, 255);
                    }
                    else
                    {
                        _current_sprite = Textures.skeleton_theme;
                        _bar_color = new Color(255, 255, 255);
                    }
                }
                else if (monster is MetalHead metalhead)
                {
                    if (monster.Name == "Hot Head")
                    {
                        _current_sprite = Textures.hothead_theme;
                        _bar_color = new Color(255, 93, 43);
                    }
                    else
                    {
                        if (metalhead.isHardModeMonster)
                        {
                            _current_sprite = Textures.hardmode_metalhead;
                            _bar_color = new Color(60, 60, 60);
                        }
                        else
                        {
                            _current_sprite = Textures.metalhead_theme;
                            _bar_color = new Color(220, 123, 5);
                        }
                    }
                    _hp_color = Color.Red;
                }
                else if (monster is ShadowBrute)
                {
                    _current_sprite = Textures.shadowbrute_theme;
                    _bar_color = new Color(52, 52, 52);
                }
                else if (monster is ShadowShaman shadowshaman)
                {
                    if (shadowshaman.isHardModeMonster)
                    {
                        _current_sprite = Textures.hardmode_shadowshaman;
                        _bar_color = new Color(98, 164, 59);
                    }
                    else
                    {
                        _current_sprite = Textures.shadowshaman_theme;
                        _bar_color = new Color(98, 164, 59);
                    }
                }
                else if (monster is SquidKid squidkid)
                {
                    if (squidkid.isHardModeMonster)
                    {
                        _current_sprite = Textures.hardmode_squidkid;
                        _bar_color = new Color(255, 189, 94);
                    }
                    else
                    {
                        _current_sprite = Textures.squidkid_theme;
                        _bar_color = new Color(255, 145, 130);
                    }
                }
                else if (monster is Mummy)
                {
                    _current_sprite = Textures.mummy_theme;
                    _bar_color = new Color(225, 203, 101);
                }
                else if (monster is DinoMonster)
                {
                    _current_sprite = Textures.pepperrex_theme;
                    _bar_color = new Color(137, 180, 56);
                }
                else if (monster is Serpent)
                {
                    _current_sprite = Textures.serpent_theme;
                    _bar_color = new Color(82, 251, 163);
                    _height = -5;
                }
                else if (monster is DwarvishSentry)
                {
                    _current_sprite = Textures.dwarvishsentry_theme;
                    _bar_color = new Color(172, 112, 105);
                    _height = 14;
                }
                else if (monster is LavaLurk)
                {
                    _current_sprite = Textures.lavalurk_theme;
                    _bar_color = new Color(220, 123, 5);
                }
                else if (monster is BlueSquid)
                {
                    _current_sprite = Textures.hardmode_squid;
                    _bar_color = new Color(160, 210, 255);
                }
                else if (monster.Name == "False Magma Cap")
                {
                    _current_sprite = Textures.falsemagmacap_theme;
                    _bar_color = new Color(255, 242, 201);
                }
                else if (monster.Name == "Spider")
                {
                    _current_sprite = Textures.hardmode_spider;
                    _bar_color = new Color(102, 209, 169);
                }
                else if (monster is Shooter)
                {
                    _current_sprite = Textures.shadowbrute_theme;
                    _bar_color = new Color(52, 52, 52);
                }
                else if (monster is Spiker)
                {
                    continue;
                }
                else
                {
                    _current_sprite = Textures.default_theme;
                }

                if (Game1.player.currentLocation.Name == "CrimsonBadlands" || Game1.player.currentLocation.Name == "IridiumQuarry")
                {
                    _bar_color = new Color(192, 64, 45);
                    _border_color = new Color(192, 64, 45);
                }

                if (ModEntry.config.Only_One_Theme)
                {
                    _current_sprite = Textures.default_theme;
                    _border_color = Color.White;
                    _bar_color = new Color(172, 50, 50);
                    _hp_color = new Color(0, 0, 0, 200);
                }

                Vector2 _monsterPos = monster.getLocalPosition(Game1.viewport);

                Game1.spriteBatch.Draw(
                    Textures.GetPixel(),
                    new Rectangle(
                        (int)_monsterPos.X - (Textures.GetPixel().Width * Game1.pixelZoom) / 2 + (monster.Sprite.SpriteWidth * Game1.pixelZoom) / 2 - Database.distance_x * Game1.pixelZoom,
                        (int)_monsterPos.Y - monster.Sprite.SpriteHeight * Game1.pixelZoom - _height * Game1.pixelZoom + 8 * Game1.pixelZoom,
                        (Textures.GetPixel().Width * Game1.pixelZoom) * Database.bar_size,
                        (Textures.GetPixel().Height * Game1.pixelZoom) * 4),
                    new Color(0,0,0,135));

                Game1.spriteBatch.Draw(
                    Textures.GetPixel(),
                    new Rectangle(
                        (int)_monsterPos.X - (Textures.GetPixel().Width * Game1.pixelZoom) / 2 + (monster.Sprite.SpriteWidth * Game1.pixelZoom) / 2 - Database.distance_x * Game1.pixelZoom,
                        (int)_monsterPos.Y - monster.Sprite.SpriteHeight * Game1.pixelZoom - _height * Game1.pixelZoom + 8 * Game1.pixelZoom,
                        (Textures.GetPixel().Width * Game1.pixelZoom) * (int)((_current_health / _current_max_health) * Database.bar_size),
                        (Textures.GetPixel().Height * Game1.pixelZoom) * 4),
                    _bar_color);

                Game1.spriteBatch.Draw(
                    _current_sprite,
                    new Rectangle(
                        (int)_monsterPos.X - (_current_sprite.Width * Game1.pixelZoom) / 2 + (monster.Sprite.SpriteWidth * Game1.pixelZoom) / 2,
                        (int)_monsterPos.Y - monster.Sprite.SpriteHeight * Game1.pixelZoom - _height * Game1.pixelZoom,
                        _current_sprite.Width * Game1.pixelZoom,
                        _current_sprite.Height * Game1.pixelZoom),
                    _border_color);

                if (!ModEntry.config.Show_Monsters_HP)
                {
                    if (ModEntry.config.Bars_Theme == 2) continue;
                    Game1.spriteBatch.Draw(
                    Textures.hp_sprite,
                    new Rectangle(
                        (int)_monsterPos.X - (Textures.hp_sprite.Width * Game1.pixelZoom) / 2 + (monster.Sprite.SpriteWidth * Game1.pixelZoom) / 2,
                        (int)_monsterPos.Y - monster.Sprite.SpriteHeight * Game1.pixelZoom - _height * Game1.pixelZoom,
                        Textures.hp_sprite.Width * Game1.pixelZoom,
                        Textures.hp_sprite.Height * Game1.pixelZoom),
                    _hp_color);
                }
                else
                {
                    Vector2 textsize = Game1.tinyFont.MeasureString(monster.Health.ToString());
                    Game1.spriteBatch.DrawString(
                        Game1.tinyFont,
                        monster.Health.ToString(),
                        new Vector2((int)_monsterPos.X - (Textures.hp_sprite.Width * Game1.pixelZoom) / 2 + (monster.Sprite.SpriteWidth * Game1.pixelZoom) / 2 + 13 * Game1.pixelZoom,
                            (int)_monsterPos.Y - monster.Sprite.SpriteHeight * Game1.pixelZoom - _height * Game1.pixelZoom + 10 * Game1.pixelZoom),
                        _hp_color,
                        0f,
                        new Vector2(textsize.X / 2, textsize.Y / 2),
                        1.25f,
                        SpriteEffects.None,
                        0f);
                }
            }
        }
    }
}
