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
using System.Collections.Generic;
using System.Xml.Serialization;
using Object = StardewValley.Object;

namespace RuneMagic.Source.Items
{
    [XmlType("Mods_Scroll")]
    public class Scroll : Object, ISpellCastingItem
    {
        [XmlIgnore]
        public Spell Spell { get; set; }

        public List<(int, int)> Ingredients { get; set; }

        public Scroll() : base()
        {
            InitializeSpell();
        }

        public Scroll(int parentSheetIndex, int stack) : base(parentSheetIndex, stack)
        {
            InitializeSpell();
            Ingredients = new List<(int, int)>
            {
                (RuneMagic.JsonAssetsApi.GetObjectId("Blank Parchment"), 1),
                (RuneMagic.JsonAssetsApi.GetObjectId("Magic Dust"), Spell.Level)
            };
        }

        public void InitializeSpell()
        {
            //set spellName to Name without " Scroll" at the end

            foreach (var spell in Spell.List)
            {
                if (Name.Contains(spell.Name))
                {
                    Spell = spell;
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
            if (Spell.Cast())
            {
                Game1.playSound("flameSpell");
                Stack--;
                if (Stack <= 0)
                    Game1.player.removeItemFromInventory(this);
            }
        }

        public void Update()
        {
        }

        public override int maximumStackSize()
        {
            return 10;
        }

        public override void drawWhenHeld(SpriteBatch spriteBatch, Vector2 objectPosition, Farmer f)
        {
            if (Player.MagicStats.CastingTime > 0)
                base.drawWhenHeld(spriteBatch, objectPosition, f);
        }
    }
}