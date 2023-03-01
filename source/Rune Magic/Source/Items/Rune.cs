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
using System.Xml.Serialization;
using Object = StardewValley.Object;

namespace RuneMagic.Source.Items
{
    [XmlType("Mods_Rune")]
    public class Rune : Object, IMagicItem
    {
        [XmlIgnore]
        public Spell Spell { get; set; }

        public int ChargesMax { get; set; }
        public float Charges { get; set; }
        public bool RunemasterActive { get; set; } = false;

        public Rune() : base()
        {
            InitializeSpell();
            ChargesMax = Game1.random.Next(3, 10);
            //if (Game1.player.HasCustomProfession(AlterationSkill.profesion1))
            //    ChargesMax += 5;
            Charges = ChargesMax;
        }

        public Rune(int parentSheetIndex, int stack) : base(parentSheetIndex, stack)
        {
            InitializeSpell();
            //Max charges is a random number between 3 and 5 (inclusive)
            ChargesMax = Game1.random.Next(3, 10);
            //if (Game1.player.HasCustomProfession(MagicSkill.Runelord))
            //    ChargesMax += 5;
            Charges = ChargesMax;
        }

        public void InitializeSpell()
        {
            foreach (var spell in RuneMagic.Spells)
            {
                if (Name.Contains(spell.Name))
                {
                    Spell = spell;
                    RuneMagic.Instance.Monitor.Log($"{Name} Initialized", LogLevel.Debug);
                    break;
                }
            }
        }

        public void Activate()
        {
            if (!Fizzle())
                if (Math.Floor(Charges) > 0)
                {
                    if (Spell.Cast())
                    {
                        Game1.playSound("flameSpell");

                        if (RunemasterActive)
                        {
                            if (Math.Floor(Charges) >= 3)
                                Charges -= 3;
                            else
                                Charges--;
                            RunemasterActive = false;
                        }
                        else
                            Charges--;
                    }
                }
        }

        public bool Fizzle()
        {
            if (Game1.random.Next(1, 100) < 0)
            {
                Game1.player.stamina -= 10;
                Game1.playSound("stoneCrack");
                Game1.player.removeItemFromInventory(this);
                Game1.player.addItemToInventory(new Object(390, 1));
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
                //if (Game1.player.HasCustomProfession(MagicSkill.Runesmith))
                //    Charges += 0.0010f;
                //else
                Charges += 0.0005f;
            }
            if (Charges > ChargesMax)
                Charges = ChargesMax;
            if (Charges < 0)
                Charges = 0;
            //Runemaster
            if (RunemasterActive && Charges < 3)
            {
                RunemasterActive = false;
                Spell.CastingTime = 1;
            }
        }

        public void DrawCastbar(SpriteBatch spriteBatch, Vector2 objectPosition, Farmer f)
        {
            //draw a castbar on the item if isCasting is true taking into account that if player has Scribe profession the castbar is 50% shorter
            if (RuneMagic.PlayerStats.IsCasting)
            {
                if (RuneMagic.PlayerStats.IsCasting && Game1.player.CurrentItem == this)
                {
                    var castingTime = Spell.CastingTime;
                    var castbarWidth = (int)(RuneMagic.PlayerStats.CastingTimer / (castingTime * 60) * 58);
                    spriteBatch.Draw(RuneMagic.Textures["castbar_frame"], new Rectangle((int)objectPosition.X, (int)objectPosition.Y, 64, 84), Color.White);
                    spriteBatch.Draw(Game1.staminaRect, new Rectangle((int)objectPosition.X + 3, (int)objectPosition.Y + 75, castbarWidth, 5), new Color(new Vector4(0, 0, 200, 0.8f)));
                }
            }
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

            if (RunemasterActive)
            {
                spriteBatch.Draw(Game1.mouseCursors, new Rectangle((int)location.X + 40, (int)location.Y + 16, 16, 16), new Rectangle(346, 400, 8, 8), Color.White, 0f, Vector2.Zero, SpriteEffects.None, layerDepth + 0.0001f);
            }
            else
            {
                if (Charges >= 1)
                    DrawCastbar(spriteBatch, location, Game1.player);
            }
            //draw an emote over the player head
            //spriteBatch.Draw(Game1.mouseCursors, new Rectangle((int)location.X + 40, (int)location.Y + 16, 16, 16), new Rectangle(346, 392, 8, 8), Color.White, 0f, Vector2.Zero, SpriteEffects.None, layerDepth + 0.0001f);
        }

        public override void drawWhenHeld(SpriteBatch spriteBatch, Vector2 objectPosition, Farmer f)
        {
            if (RuneMagic.PlayerStats.IsCasting)
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