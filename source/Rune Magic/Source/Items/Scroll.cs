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
using System.Xml.Serialization;
using Object = StardewValley.Object;

namespace RuneMagic.Source.Items
{
    [XmlType("Mods_Scroll")]
    public class Scroll : Object, IMagicItem
    {
        [XmlIgnore]
        public Spell Spell { get; set; }

        public float Charges { get; set; }

        public Scroll() : base()
        {
            InitializeSpell();
        }

        public Scroll(int parentSheetIndex, int stack) : base(parentSheetIndex, stack)
        {
            InitializeSpell();
        }

        public void InitializeSpell()
        {
            //set spellName to Name without " Scroll" at the end

            foreach (var spell in RuneMagic.Spells)
            {
                if (Name.Contains(spell.Name))
                {
                    Spell = spell;
                    //if (Game1.player.HasCustomProfession(MagicSkill.Scribe))
                    //    Spell.CastingTime *= 0.8f;
                    break;
                }
            }
        }

        public void Activate()
        {
            if (Spell.Cast())
            {
                Game1.playSound("flameSpell");
                //remove an item stack from this object if Farmer doesnt have Lorekeeper profession and if it does give it a 20% chance of not consuming the scroll
                //if (!Game1.player.HasCustomProfession(MagicSkill.Lorekeeper) || Game1.random.Next(1, 100) > 20)
                Stack--;
                if (Stack <= 0)
                    Game1.player.removeItemFromInventory(this);
            }
        }

        public void Update()
        {
        }

        public bool Fizzle()
        {
            return false;
        }

        public override int maximumStackSize()
        {
            return 10;
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

        public override void drawWhenHeld(SpriteBatch spriteBatch, Vector2 objectPosition, Farmer f)
        {
            if (RuneMagic.PlayerStats.IsCasting /*&& Game1.player.HasCustomProfession(MagicSkill.Sage)*/)
                base.drawWhenHeld(spriteBatch, objectPosition, f);
        }

        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
        {
            base.drawInMenu(spriteBatch, location, scaleSize, transparency, layerDepth, drawStackNumber, color, drawShadow);
            DrawCastbar(spriteBatch, location, Game1.player);
        }
    }
}