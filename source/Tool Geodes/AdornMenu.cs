using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System.Collections.Generic;
using StardewModdingAPI;
using SObject = StardewValley.Object;
using SpaceShared;

namespace ToolGeodes
{
    internal class AdornMenu : MenuWithInventory
    {
        private readonly ToolType tool;
        public Item[] items = new Item[6];

        public AdornMenu(ToolType theTool)
        : base(null, false, false, 0, 150)
        {
            this.inventory.highlightMethod = this.highlight;
            tool = theTool;
            exitFunction = onClosed;

            int[] ids = null;
            if (tool == ToolType.Weapon)      ids = Mod.Data.WeaponGeodes;
            if (tool == ToolType.Pickaxe)     ids = Mod.Data.PickaxeGeodes;
            if (tool == ToolType.Axe)         ids = Mod.Data.AxeGeodes;
            if (tool == ToolType.WateringCan) ids = Mod.Data.WaterCanGeodes;
            if (tool == ToolType.Hoe)         ids = Mod.Data.HoeGeodes;
            if ( ids != null )
            {
                int i = 0;
                foreach ( var id in ids )
                {
                    if (id == 0)
                        continue;

                    items[i++] = new SObject(new Vector2(0, 0), id, 1);
                }
            }
        }

        private bool justClicked = false;
        private int clickX = 0, clickY = 0;
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);
            justClicked = true;
            clickX = x;
            clickY = y;
        }

        public override void draw(SpriteBatch b)
        {
            int w = 80 * 2 + 64, h = 540;
            int x = Game1.viewport.Width / 2 - w / 2;
            int y = Game1.viewport.Height / 2 - h / 2 - 100;
            IClickableMenu.drawTextureBox(b, x, y, w, h, Color.White);
            
            Game1.drawDialogueBox(this.xPositionOnScreen - IClickableMenu.borderWidth / 2, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 64 + 150, this.width, this.height - (IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 192), false, true, (string)null, false, false);
            this.inventory.draw(b);

            b.DrawString(Game1.dialogueFont, tool.ToString(), new Vector2(x + IClickableMenu.borderWidth * 1, y + IClickableMenu.borderWidth * 1), Color.Black);

            bool foundPrismatic = false;
            for (int i = 0; i < items.Length; ++i )
            {
                if (i >= 3 && !foundPrismatic)
                    continue;

                Vector2 vec = new Vector2(x + 80, y + 64 * i + 96);
                b.Draw(Game1.menuTexture, vec, new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 10, -1, -1)), Color.White, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.5f);
                if (items[i] != null)
                {
                    items[i].drawInMenu(b, vec, 1);
                    if ( items[i] is SObject obj && obj.ParentSheetIndex == SObject.prismaticShardIndex )
                    {
                        foundPrismatic = true;
                    }
                }

                Rectangle rect = new Rectangle((int)vec.X, (int)vec.Y, 64, 64);
                if ( rect.Contains( clickX, clickY )  && justClicked)
                {
                    if ( heldItem == null )
                    {
                        heldItem = items[i];
                        items[i] = null;
                    }
                    else
                    {
                        if (items[i] == null)
                        {
                            items[i] = heldItem.getOne();
                            heldItem.Stack--;
                            if (heldItem.Stack == 0)
                                heldItem = null;
                        }
                        else
                        {
                            // If something is held and in the slot, do nothing
                        }
                    }
                }
            }

            if (this.hoverText != null)
                IClickableMenu.drawHoverText(b, this.hoverText, Game1.smallFont);
            if (this.hoveredItem != null)
                IClickableMenu.drawToolTip(b, getDescription(hoveredItem), this.hoveredItem.DisplayName, this.hoveredItem, this.heldItem != null);


            this.heldItem?.drawInMenu(b, new Vector2((float)(Game1.getOldMouseX() + 8), (float)(Game1.getOldMouseY() + 8)), 1f);
            base.drawMouse(b);

            justClicked = false;
        }

        public void onClosed()
        {
            foreach ( var item in items )
            {
                int[] ids = null;
                if (tool == ToolType.Weapon) ids = Mod.Data.WeaponGeodes;
                if (tool == ToolType.Pickaxe) ids = Mod.Data.PickaxeGeodes;
                if (tool == ToolType.Axe) ids = Mod.Data.AxeGeodes;
                if (tool == ToolType.WateringCan) ids = Mod.Data.WaterCanGeodes;
                if (tool == ToolType.Hoe) ids = Mod.Data.HoeGeodes;
                if (ids != null)
                {
                    for ( int i = 0; i < ids.Length; ++i )
                    {
                        ids[i] = items[i] == null ? 0 : items[i].ParentSheetIndex;
                    }
                }
            }

            if (Context.IsMainPlayer)
                Mod.instance.Helper.Data.WriteSaveData($"spacechase0.ToolGeodes.{Game1.player.UniqueMultiplayerID}", Mod.Data);
            else
            {
                Log.debug("Sending tool geode data to host");
                Mod.instance.Helper.Multiplayer.SendMessage(Mod.Data, Mod.MSG_TOOLGEODEDATA, playerIDs: new[] { Game1.MasterPlayer.UniqueMultiplayerID } );
            }
        }

        public string getDescription(Item i)
        {
            if ( !(i is SObject obj) || obj.bigCraftable.Value )
            {
                return i.getDescription();
            }

            string str = "";
            if (obj.ParentSheetIndex == Mod.Config.GEODE_MORE_SLOTS)
                str = "Adorning provides 3 more adornment slots.\n(One time use.)";
            else if (obj.ParentSheetIndex == Mod.Config.GEODE_LENGTH)
                str = "Adorning provides 2 tiles length to a fully charged use.";
            else if (obj.ParentSheetIndex == Mod.Config.GEODE_WIDTH)
                str = "Adorning provides 2 tiles width to a fully charged use.";
            else if (obj.ParentSheetIndex == Mod.Config.GEODE_INFINITE_WATER)
                str = "Adorning provides infinite water.\n(One time use.)";
            else if (obj.ParentSheetIndex == Mod.Config.GEODE_OBJ_TRUESIGHT)
                str = "Adorning provides true-sight of " + (tool == ToolType.Pickaxe ? "rocks" : "dig-spots") + ".(One time use.)\n";
            else if (obj.ParentSheetIndex == Mod.Config.GEODE_LESS_STAMINA)
                str = "Adorning provides less stamina usage.";
            else if (obj.ParentSheetIndex == Mod.Config.GEODE_INSTANT_CHARGE)
                str = "Adorning provides instant charging time.\n(One time use.)";
            else if (obj.ParentSheetIndex == Mod.Config.GEODE_REMOTE_USE)
                str = "Adorning provides remote tool usage ability.\n(One time use.)";
            //else if (obj.ParentSheetIndex == Mod.Config.GEODE_MOB_FREEZE)
            //    str = "Adorning provides a freezing strike.";
            else if (obj.ParentSheetIndex == Mod.Config.GEODE_MORE_DAMAGE)
                str = "Adorning provides a more ferocious strike.";
            else if (obj.ParentSheetIndex == Mod.Config.GEODE_MORE_KNOCKBACK)
                str = "Adorning provides a \"bouncier\" strike.";
            else if (obj.ParentSheetIndex == Mod.Config.GEODE_MORE_CRITCHANCE)
                str = "Adorning provides a better chance of critically striking.";
            else if (obj.ParentSheetIndex == Mod.Config.GEODE_SWIPE_SPEED)
                str = "Adorning provides a faster strike.\n(One time use.)";
            else if (obj.ParentSheetIndex == Mod.Config.GEODE_PIERCE_ARMOR)
                str = "Adorning provides a strike that pierces through the hardest armor.\n(One time use.)";

            return str == null ? i.getDescription() : (str + "\n\n" + i.getDescription());
        }

        public bool highlight(Item i)
        {
            var obj = i as SObject;
            if (obj == null || obj.bigCraftable.Value)
                return false;
            
            IList<int> indices = new List<int>();
            indices.Add(Mod.Config.GEODE_MORE_SLOTS);
            if ( tool != ToolType.Weapon )
            {
                indices.Add(Mod.Config.GEODE_LESS_STAMINA);
                indices.Add(Mod.Config.GEODE_INSTANT_CHARGE);
                indices.Add(Mod.Config.GEODE_REMOTE_USE);

                if ( tool == ToolType.Pickaxe || tool == ToolType.Hoe )
                    indices.Add(Mod.Config.GEODE_OBJ_TRUESIGHT);
                if (tool == ToolType.WateringCan || tool == ToolType.Hoe)
                {
                    indices.Add(Mod.Config.GEODE_LENGTH);
                    indices.Add(Mod.Config.GEODE_WIDTH);
                    indices.Add(Mod.Config.GEODE_INSTANT_CHARGE);
                }
                if (tool == ToolType.WateringCan)
                    indices.Add(Mod.Config.GEODE_INFINITE_WATER);
            }
            else
            {
                //indices.Add(Mod.Config.GEODE_MOB_FREEZE);
                indices.Add(Mod.Config.GEODE_MORE_DAMAGE);
                indices.Add(Mod.Config.GEODE_MORE_KNOCKBACK);
                indices.Add(Mod.Config.GEODE_MORE_CRITCHANCE);
                indices.Add(Mod.Config.GEODE_SWIPE_SPEED);
                indices.Add(Mod.Config.GEODE_PIERCE_ARMOR);
            }

            bool found = false;
            foreach ( var ind in indices )
            {
                if ( ind == obj.ParentSheetIndex )
                {
                    found = true;
                    break;
                }
            }
            return found;
        }
    }
}