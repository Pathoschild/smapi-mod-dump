/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/chencrstu/TeleportNPCLocation
**
*************************************************/

using System;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace TeleportNPCLocation.framework
{
	public class NPCMenuItem : ClickableComponent
	{
        /*********
        ** Accessors
        *********/
        /// <summary>The subject to display.</summary>
        public NPC npc { get; }

        /// <summary>The search result's index in the list.</summary>
        public int Index { get; }

        /// <summary>show more info.</summary>
        public bool showMoreInfo { get; }

        /// <summary>show more info.</summary>
        public Checkbox checkbox { get; }

        /// <summary>callback.</summary>
        public Action<NPCMenuItem> Callback { get; set; }

        /// <summary>pre height.</summary>
        public const float preHeight = NPC.portrait_height + NPCMenuItem.padding * 2;
        /// <summary>padding.</summary>
        public const float padding = 3;
        /// <summary>borderWidth.</summary>
        public const float borderWidth = 1;

        public NPCMenuItem(NPC npc, int index, bool showMoreInfo)
            : base(Rectangle.Empty, npc.Name)
        {
            this.npc = npc;
            this.Index = index;
            this.showMoreInfo = showMoreInfo;
            this.checkbox = new Checkbox();
        }


        /// <summary>Draw the search result to the screen.</summary>
        /// <param name="contentBatch">The sprite batch being drawn.</param>
        /// <param name="position">The position at which to draw the search result.</param>
        /// <param name="width">The width to draw.</param>
        /// <param name="font">The font to draw.</param>
        /// <param name="check">checkbox state to draw.</param>
        public Vector2 Draw(SpriteBatch contentBatch, Vector2 position, float width, SpriteFont font, bool check)
        {
            // update bounds
            this.bounds.X = (int)position.X;
            this.bounds.Y = (int)position.Y;
            this.bounds.Width = (int)width;
            this.bounds.Height = (int)preHeight;

            // draw background
            int mouseX = Game1.getMouseX();
            int mouseY = Game1.getMouseY();
            bool highlight = mouseX > this.bounds.X && mouseX < this.bounds.X + this.bounds.Width
                && mouseY > this.bounds.Y && mouseY < this.bounds.Y + this.bounds.Height;
            if (highlight)
            {
                contentBatch.DrawLine(this.bounds.X, this.bounds.Y, new Vector2(this.bounds.Width, this.bounds.Height), Color.Beige);
            }

            // draw Portrait
            Vector2 portraitPosition = new Vector2(this.bounds.X + NPCMenuItem.padding, this.bounds.Y + NPCMenuItem.padding);
            Vector2 portraitSize = new Vector2(NPC.portrait_width, NPC.portrait_height);
            ClickableTextureComponent teleportButton = new ClickableTextureComponent(Rectangle.Empty, npc.Portrait, new Rectangle(0, 0, NPC.portrait_width, NPC.portrait_height), 1);
            teleportButton.bounds = new Rectangle((int)portraitPosition.X, (int)portraitPosition.Y, (int)portraitSize.X, (int)portraitSize.Y);
            teleportButton.draw(contentBatch);

            // draw value label
            float valueWidth = width - NPC.portrait_width - padding * 4 - NPCMenuItem.borderWidth;
            Vector2 valuePosition = new Vector2(this.bounds.X + NPC.portrait_width + NPCMenuItem.padding * 3, this.bounds.Y + NPCMenuItem.padding);
            string value = npc.displayName ?? npc.Name;
            if (this.showMoreInfo)
            {
                value += $"\nlocation:{npc.currentLocation.NameOrUniqueName}";
            }
            Vector2 valueSize = contentBatch.DrawTextBlock(font, value, valuePosition, valueWidth);
            Vector2 rowSize = new Vector2(NPC.portrait_width + valueWidth + NPCMenuItem.padding * 4, Math.Max(portraitSize.Y + NPCMenuItem.padding * 2, valueSize.Y + NPCMenuItem.padding * 2));

            // draw text box
            checkbox.Position = new Vector2(this.bounds.X + rowSize.X - checkbox.Width - 4, this.bounds.Y + (rowSize.Y - checkbox.Height) / 2);
            checkbox.Checked = check;
            checkbox.Callback = (Checkbox e) => this.HandleCheckBoxClick(e);
            checkbox.Draw(contentBatch);

            // draw table row
            Color lineColor = Color.Gray;
            contentBatch.DrawLine(this.bounds.X, this.bounds.Y, new Vector2(rowSize.X, NPCMenuItem.borderWidth), lineColor); // top
            contentBatch.DrawLine(this.bounds.X, this.bounds.Y + rowSize.Y, new Vector2(rowSize.X, NPCMenuItem.borderWidth), lineColor); // bottom
            contentBatch.DrawLine(this.bounds.X, this.bounds.Y, new Vector2(NPCMenuItem.borderWidth, rowSize.Y), lineColor); // left
            contentBatch.DrawLine(this.bounds.X + NPC.portrait_width + NPCMenuItem.padding * 2, this.bounds.Y, new Vector2(NPCMenuItem.borderWidth, rowSize.Y), lineColor); // middle
            contentBatch.DrawLine(this.bounds.X + rowSize.X, this.bounds.Y, new Vector2(NPCMenuItem.borderWidth, rowSize.Y), lineColor); // right

            this.bounds.Height = (int)(Math.Max(portraitSize.Y, valueSize.Y) + NPCMenuItem.padding * 2);

            // return size
            return new Vector2(this.bounds.Width, this.bounds.Height);
        }

        public bool isCheckBoxContainsPoint(int x, int y)
        {
            if (x < this.bounds.X + this.bounds.Width - this.checkbox.Width - 4) return false;
            if (x > this.bounds.X + this.bounds.Width - 4) return false;
            if (y < this.bounds.Y + (this.bounds.Height - checkbox.Height) / 2) return false;
            if (y > this.bounds.Y + (this.bounds.Height + checkbox.Height) / 2) return false;
            return true;
        }

        public void HandleCheckBoxClick(Checkbox checkbox)
        {
            this.Callback(this);
        }
    }
}

