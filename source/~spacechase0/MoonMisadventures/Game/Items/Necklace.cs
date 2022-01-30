/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley;
using StardewValley.Menus;

namespace MoonMisadventures.Game.Items
{
    [XmlType("Mods_spacechase0_MoonMisadventures_Necklace")]
    public class Necklace : Item
    {
        public enum Type
        {
            Looting,
            Shocking,
            Speed,
            Health,
            Cooling,
            Lunar,
            Water,
            Sea,
        }

        public readonly NetEnum< Type > necklaceType = new();

        public override string DisplayName { get => I18n.GetByKey($"item.necklace.{this.necklaceType.Value}.name"); set { } }
        public override int Stack { get => 1; set { } }

        public Necklace() { }
        public Necklace( Type type )
        {
            necklaceType.Value = type;
            Name = "Necklace." + type;
        }

        public override void drawInMenu( SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow )
        {
            if ( drawShadow )
                ;
            spriteBatch.Draw( Assets.Necklaces, location + new Vector2( 32, 32 ), new Rectangle( ( ( int ) necklaceType.Value ) % 4 * 16, ( ( int ) necklaceType.Value ) / 4 * 16, 16, 16 ), color * transparency, 0, new Vector2( 8, 8 ) * scaleSize, scaleSize * Game1.pixelZoom, SpriteEffects.None, layerDepth );
        }

        public override int maximumStackSize()
        {
            return 1;
        }

        public override int addToStack( Item stack )
        {
            return 1;
        }

        public override string getDescription()
        {
            return I18n.GetByKey($"item.necklace.{this.necklaceType.Value}.description");
        }

        public override bool isPlaceable()
        {
            return false;
        }

        public override Item getOne()
        {
            var ret = new Necklace( necklaceType.Value );
            ret._GetOneFrom( this );
            return ret;
        }

        public override bool canBeTrashed()
        {
            return true;
        }

        public override bool canBeGivenAsGift()
        {
            return false;
        }

        public override bool canBeDropped()
        {
            return false;
        }

        public virtual void OnEquip( Farmer player )
        {
            if ( necklaceType.Value == Type.Health )
            {
                int diff = ( int )( player.maxHealth * 0.5f );
                player.maxHealth += diff;
                player.health = Math.Min( player.maxHealth, player.health + diff );
            }
        }

        public virtual void OnUnequip( Farmer player )
        {
            if ( necklaceType.Value == Type.Health )
            {
                int oldHealth = player.health;
                int oldMax = player.maxHealth;
                player.maxHealth = 100;
                LevelUpMenu.RevalidateHealth( player );
                int diff = oldMax - player.maxHealth;
                player.health -= diff;
            }
        }
    }
}
