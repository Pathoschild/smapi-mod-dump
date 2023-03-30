/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/facufierro/RuneMagic
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceCore;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Object = StardewValley.Object;

namespace RuneMagic.Source.Items
{
    [XmlType("Mods_Rune")]
    public class Rune : Object, ISpellCastingItem
    {
        [XmlIgnore]
        public Spell Spell { get; set; }

        public int ChargesMax { get; set; }
        public float Charges { get; set; }
        public int Cracks { get; set; } = 0;

        public List<(int, int)> Ingredients { get; set; }

        public Rune() : base()
        {
            InitializeSpell();
            if (Spell != null)
            {
                int invertedLevel = 6 - Spell.Level;
                ChargesMax = Game1.random.Next(3, 5 + invertedLevel + Player.MagicStats.ActiveSchool.Level / 3);
                Charges = ChargesMax;
            }
        }

        public Rune(int parentSheetIndex, int stack) : base(parentSheetIndex, stack)
        {
            InitializeSpell();
            if (Spell != null)
            {
                int invertedLevel = 6 - Spell.Level;
                ChargesMax = Game1.random.Next(3, 5 + invertedLevel + Player.MagicStats.ActiveSchool.Level / 3);
                Charges = ChargesMax;
            }
        }

        public void InitializeSpell()
        {
            foreach (var spell in Spell.List)
            {
                if (Name.Contains(spell.Name))
                {
                    Spell = spell;
                    //RuneMagic.Instance.Monitor.Log($"{Name} Initialized", LogLevel.Debug);
                    break;
                }
            }
        }

        public bool IngredientsMet()
        {
            if (Game1.player.hasItemInInventory(Ingredients[0].Item1, Ingredients[0].Item2) && Game1.player.hasItemInInventory(Ingredients[1].Item1, Ingredients[1].Item2))
                return true;
            else
                return false;
        }

        public void Activate()
        {
            if (!Fizzle())
            {
                if (Math.Floor(Charges) > 0)
                {
                    if (Spell.Cast())
                    {
                        Game1.playSound("flameSpell");
                        Charges--;
                    }
                }
            }
            else
                Cracks++;
        }

        public bool Fizzle()
        {
            if (Game1.random.Next(1, 100) < 10)
            {
                Game1.player.stamina -= 10;
                Game1.playSound("stoneCrack");
                return true;
            }
            else
                return false;
        }

        public void Update()
        {
            //Charges
            if (Charges < ChargesMax)
            {
                Charges += 0.0005f;
            }
            if (Charges > ChargesMax)
                Charges = ChargesMax;
            if (Charges < 0)
                Charges = 0;
            if (Cracks >= 3)
                Game1.player.removeItemFromInventory(this);
        }

        public void DrawCharges(SpriteBatch spriteBatch, Vector2 location, float layerDepth)
        {
            spriteBatch.DrawString(Game1.tinyFont, Math.Floor(Charges).ToString(), new Vector2(location.X + 64 - Game1.tinyFont.MeasureString(Math.Floor(Charges).ToString()).X, location.Y + 64 - Game1.tinyFont.MeasureString(Math.Floor(Charges).ToString()).Y),
                           Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, layerDepth + 0.0001f);
        }

        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
        {
            base.drawInMenu(spriteBatch, location, scaleSize, transparency, layerDepth, drawStackNumber, color, drawShadow);
            DrawCharges(spriteBatch, location, layerDepth);

            if (Cracks == 1 || Cracks == 2)
            {
                var crackedRune = RuneMagic.Textures[$"cracked_rune{Cracks}"];
                spriteBatch.Draw(crackedRune, location + new Vector2(32f, 32f), new Rectangle?(Game1.getSourceRectForStandardTileSheet(crackedRune, 0, 16, 16)),
                                     Color.White * transparency, 0.0f, new Vector2(8f, 8f), 4f * scaleSize, SpriteEffects.None, layerDepth + 0.0001f);
            }
        }

        public override void drawWhenHeld(SpriteBatch spriteBatch, Vector2 objectPosition, Farmer f)
        {
            if (Player.MagicStats.CastingTime > 0)
                base.drawWhenHeld(spriteBatch, objectPosition, f);
        }

        public override bool canBeShipped()
        {
            return false;
        }

        public override bool canBeGivenAsGift()
        {
            return false;
        }

        public override int maximumStackSize()
        {
            return 1;
        }
    }
}

//*************************
//private void DrawRune(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float layerDepth)
//{
//    var transparency = (float)Charges / (float)ChargesMax;
//    spriteBatch.Draw(Glyph.Value, location + new Vector2(32f, 32f), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Glyph.Value, 0, 16, 16)),
//      Spell.GetColor() * transparency, 0.0f, new Vector2(8f, 8f), 4f * scaleSize, SpriteEffects.None, layerDepth);
//}