/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/facufierro/RuneMagic
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using RuneMagic.Source;
using RuneMagic.Source.Items;
using SpaceCore;
using SpaceShared.APIs;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Minigames;
using static SpaceCore.Skills;

namespace RuneMagic.Source.Interface
{
    public class ScrollScribingMenu : MagicMenu
    {
        private MagicButton Scroll;
        private Scroll ScrollItem;
        private int _scrollCounter;
        private Spell _lastSpell = null;

        public ScrollScribingMenu(SpellBook spellBook)
            : base(spellBook)
        {
            Scroll = new();
            ScrollItem = new();
        }

        public override void draw(SpriteBatch b)
        {
            base.draw(b);
            DrawScroll(b);
            DrawTooltip(b);
            drawMouse(b);
        }

        private void DrawScroll(SpriteBatch b)
        {
            var x = 28;
            var y = 14;

            Scroll.Bounds = GridRectangle(x, y, 5, 5);
            b.Draw(RuneMagic.Textures["inscription_table"], GridRectangle(26, 12, 12, 12), Color.White);
            Scroll.Render(b, RuneMagic.Textures["blank_parchment"]);

            if (Scroll.Spell != null)
            {
                Color color;
                b.Draw(RuneMagic.Textures["magic_dust"], GridRectangle(x + 1, y + 10, 3, 3), Color.White);
                if (ScrollItem.IngredientsMet())
                    color = Color.White;
                else
                    color = Color.Red;
                b.DrawString(Game1.smallFont, $"x {ScrollItem.Ingredients[1].Item2}", Grid[x + 4, y + 11].ToVector2(), color);
                b.DrawString(Game1.tinyFont, $"{_scrollCounter}", Grid[x + 5, y + 4].ToVector2(), Color.White);
            }
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);
            foreach (var knownSlot in SpellBook.KnownSpellSlots)
            {
                if (knownSlot.Bounds.Contains(x, y))
                {
                    //RuneMagic.Instance.Monitor.Log($"{_scrollCounter}");

                    Scroll.Spell = knownSlot.Spell;

                    if (_lastSpell == null || _lastSpell == Scroll.Spell)
                    {
                        _scrollCounter++;
                    }
                    else
                    {
                        _scrollCounter = 1;
                    }
                    _lastSpell = Scroll.Spell;
                    ScrollItem.Spell = Scroll.Spell;
                    ScrollItem.Ingredients = new List<(int, int)>
                    {
                        (RuneMagic.JsonAssetsApi.GetObjectId("Blank Parchment"), 1),
                        (RuneMagic.JsonAssetsApi.GetObjectId("Magic Dust"), ScrollItem.Spell.Level)
                    };
                }
            }
            if (Scroll.Bounds.Contains(x, y))
            {
                //if the player has space in inventory

                for (int i = 0; i < _scrollCounter; i++)
                {
                    if (ScrollItem.Spell != null && ScrollItem.IngredientsMet())
                    {
                        if (Game1.player.addItemToInventoryBool(new Scroll(RuneMagic.JsonAssetsApi.GetObjectId($"{Scroll.Spell.Name} Scroll"), 1)))
                        {
                            Game1.player.removeItemsFromInventory(ScrollItem.Ingredients[0].Item1, ScrollItem.Ingredients[0].Item2);
                            Game1.player.removeItemsFromInventory(ScrollItem.Ingredients[1].Item1, ScrollItem.Ingredients[1].Item2);
                        }
                        Game1.playSound("shwip");
                        if (i == _scrollCounter - 1)
                        {
                            _scrollCounter = 0;
                            Scroll.Spell = null;
                        }
                    }
                }
            }
        }
    }
}