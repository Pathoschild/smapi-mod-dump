using System.Collections.Generic;
using CustomNPCFramework.Framework.ModularNpcs;
using CustomNPCFramework.Framework.ModularNpcs.ModularRenderers;
using Microsoft.Xna.Framework;
using StardewValley;

namespace CustomNPCFramework.Framework.NPCS
{
    /// <summary>Extended merchant npc from ExtendedNPC.</summary>
    class MerchantNpc : ExtendedNpc
    {
        /// <summary>The list of items this npc has for sale.</summary>
        public List<Item> stock;

        /// <summary>Construct an instance.</summary>
        /// <param name="Stock">The list of items this npc will sell.</param>
        /// <param name="sprite">The sprite for the npc to use.</param>
        /// <param name="renderer">The renderer for the npc to use.</param>
        /// <param name="position">The position for the npc to use.</param>
        /// <param name="facingDirection">The facing direction for the player to face.</param>
        /// <param name="name">The name for the npc.</param>
        public MerchantNpc(List<Item> Stock, Sprite sprite, BasicRenderer renderer, Vector2 position, int facingDirection, string name)
            : base(sprite, renderer, position, facingDirection, name)
        {
            this.stock = Stock;
        }

        /// <summary>Construct an instance.</summary>
        /// <param name="Stock">The list of items for the npc to sell.</param>
        /// <param name="npcBase">The npc base for the character to be expanded upon.</param>
        public MerchantNpc(List<Item> Stock, ExtendedNpc npcBase)
            : base(npcBase.spriteInformation, npcBase.characterRenderer, npcBase.portraitInformation, npcBase.position, npcBase.facingDirection, npcBase.Name)
        {
            this.stock = Stock;
        }

        /// <summary>Used to interact with the npc. When interacting pulls up a shop menu for the npc with their current stock.</summary>
        public override bool checkAction(Farmer who, GameLocation l)
        {
            if (Game1.activeClickableMenu == null)
            {
                Game1.activeClickableMenu = new StardewValley.Menus.ShopMenu(this.stock);
                return true;
            }
            else
            {
                return base.checkAction(Game1.player, Game1.player.currentLocation);
            }
        }
    }
}
