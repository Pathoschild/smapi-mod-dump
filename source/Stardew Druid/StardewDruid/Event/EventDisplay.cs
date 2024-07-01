/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Extensions;
using System;
using System.ComponentModel;
using System.Threading;


namespace StardewDruid.Event
{
    public class EventDisplay : HUDMessage
    {

        public enum displayTypes
        {

            title,
            plain,
            bar,

        }

        new public displayTypes type;

        public Color colour;

        public string title;

        public string text;

        public int time;

        public float progress;

        public string eventId;

        public int uniqueId;

        public StardewDruid.Monster.Boss boss;

        public int level;

        public EventDisplay(string Title, string Text, int Time, displayTypes Type = displayTypes.title, int UniqueId = 0)
            : base(Text)
        {

            title = Title;

            colour = Game1.textColor;

            text = Text;

            time = Time * 10;

            type = Type;

            uniqueId = UniqueId;
        
        }

        public virtual bool update()
        {

            if(type == displayTypes.bar)
            {
                
                if (eventId != null)
                {
                    if (!Mod.instance.eventRegister.ContainsKey(eventId))
                    {

                        return false;

                    }

                    progress = Mod.instance.eventRegister[eventId].DisplayProgress(uniqueId);

                    if (progress == -1f)
                    {

                        return false;

                    }

                    time = 5;

                }

                if (boss != null)
                {

                    if (!ModUtility.MonsterVitals(boss, Game1.player.currentLocation))
                    {

                        return false;

                    }

                    progress = (float)boss.Health / (float)boss.MaxHealth;

                    time = 5;

                }

            }

            time--;

            if (time <= 0)
            {

                return false;

            }

            return true;

        }

        public virtual void shutdown()
        {

            time = 0;

            level = 0;

        }

        public override void draw(SpriteBatch b, int j, ref int heightUsed)
        {

            if(level == 0)
            {

                return;

            }

            int useLevel = level - 1;

            float fade = 0.375f + (0.125f * time);

            Rectangle titleSafeArea = Game1.graphics.GraphicsDevice.Viewport.GetTitleSafeArea();

            Rectangle container;

            Vector2 textOffset;

            int containerOffset;

            int containerHeight;

            switch (type)
            {
                case displayTypes.title:

                    Vector2 titleOffset = Game1.smallFont.MeasureString(title+":") * 1.25f;

                    textOffset = Game1.smallFont.MeasureString(text) * 1.25f;

                    containerOffset = (int)Math.Max(textOffset.X + titleOffset.X + 48, 800);

                    containerHeight = (int)(titleOffset.Y + 16);

                    container = new(titleSafeArea.Center.X - (containerOffset / 2), titleSafeArea.Bottom - 160 - (useLevel * (containerHeight + 8)), containerOffset, containerHeight);

                    drawContainer(b, container);

                    b.Draw(Game1.staminaRect, new Vector2(container.X + 5, container.Y + 5), new Rectangle(container.X + 5, container.Y + 5, container.Width - 10, container.Height - 10), new Color(254, 199, 118), 0f, Vector2.Zero, 1f, 0, 990f);

                    b.DrawString(Game1.smallFont, title+":", new Vector2(container.X + 7, container.Y + 9), Game1.textShadowColor, 0f, Vector2.Zero, 1.25f, SpriteEffects.None, 998f);

                    b.DrawString(Game1.smallFont, title+":", new Vector2(container.X + 8, container.Y + 8), Game1.textColor, 0f, Vector2.Zero, 1.25f, SpriteEffects.None, 999f);

                    b.DrawString(Game1.smallFont, text, new Vector2(container.X + 15 + titleOffset.X, container.Y + 9), Game1.textShadowColor, 0f, Vector2.Zero, 1.25f, SpriteEffects.None, 998f);

                    b.DrawString(Game1.smallFont, text, new Vector2(container.X + 16 + titleOffset.X, container.Y + 8), Game1.textColor, 0f, Vector2.Zero, 1.25f, SpriteEffects.None, 999f);

                    break;

                case displayTypes.plain:

                    textOffset = Game1.smallFont.MeasureString(text) * 1.25f;

                    containerOffset = (int)Math.Max(textOffset.X + 32, 800);

                    containerHeight = (int)(textOffset.Y + 16);

                    container = new(titleSafeArea.Center.X - (containerOffset / 2), titleSafeArea.Bottom - 160 - (useLevel * (containerHeight+8)), containerOffset, containerHeight);

                    drawContainer(b, container);

                    b.Draw(Game1.staminaRect, new Vector2(container.X + 5, container.Y + 5), new Rectangle(container.X + 5, container.Y + 5, container.Width - 10, container.Height - 10), new Color(254, 240, 192), 0f, Vector2.Zero, 1f, 0, 990f);

                    b.DrawString(Game1.smallFont, text, new Vector2(container.Center.X - (textOffset.X / 2) - 1, container.Y + 9), Game1.textShadowColor, 0f, Vector2.Zero, 1.25f, SpriteEffects.None, 998f);

                    b.DrawString(Game1.smallFont, text, new Vector2(container.Center.X - (textOffset.X / 2), container.Y + 8), Game1.textColor, 0f, Vector2.Zero, 1.25f, SpriteEffects.None, 999f);

                    break;

                case displayTypes.bar:

                    container = new(titleSafeArea.Right - 360, titleSafeArea.Top + 300 + (useLevel * 60), 360, 52);

                    drawContainer(b, container);

                    b.Draw(Game1.staminaRect, new Vector2(container.X + 5, container.Y + 5), new Rectangle(container.X + 5, container.Y + 5, container.Width - 10, container.Height - 10), new(254, 240, 192), 0f, Vector2.Zero, 1f, 0, 990f);

                    b.Draw(Game1.staminaRect, new Vector2(container.X + 5, container.Y + 5), new Rectangle(container.X + 5, container.Y + 5, (int)((container.Width - 10) * progress), container.Height-10), colour * 0.25f, 0f, Vector2.Zero, 1f, 0, 997f);

                    b.DrawString(Game1.smallFont, text, new Vector2(container.X + 7, container.Y + 9), Game1.textShadowColor, 0f, Vector2.Zero, 1.1f, SpriteEffects.None, 998f);

                    b.DrawString(Game1.smallFont, text, new Vector2(container.X + 8, container.Y + 8), Game1.textColor, 0f, Vector2.Zero, 1.1f, SpriteEffects.None, 999f);

                    break;


            }

        }

        public virtual void drawContainer(SpriteBatch b, Rectangle container, float fade = 1f)
        {

            Color outerTop = new(167, 81, 37);

            Color outerBot = new(139, 58, 29);

            Color inner = new(246, 146, 30);

            // --------------------------------
            // top

            b.Draw(Game1.staminaRect, new Rectangle(container.X + 2, container.Y, container.Width - 4, 2), outerTop * fade);

            b.Draw(Game1.staminaRect, new Rectangle(container.X + 2, container.Y + 2, container.Width - 4, 3), inner * fade);

            // --------------------------------
            // left

            b.Draw(Game1.staminaRect, new Rectangle(container.X, container.Y + 2, 2, container.Height - 4), outerTop * fade);

            b.Draw(Game1.staminaRect, new Rectangle(container.X + 2, container.Y + 5, 3, container.Height - 10), inner * fade);

            // --------------------------------
            // bottom

            b.Draw(Game1.staminaRect, new Rectangle(container.X + 2, container.Y + container.Height - 2, container.Width - 4, 2), outerBot * fade);

            b.Draw(Game1.staminaRect, new Rectangle(container.X + 2, container.Y + container.Height - 5, container.Width - 4, 3), inner * fade);

            // --------------------------------
            // right

            b.Draw(Game1.staminaRect, new Rectangle(container.Right - 2, container.Y + 2, 2, container.Height - 4), outerBot * fade);

            b.Draw(Game1.staminaRect, new Rectangle(container.Right - 5, container.Y + 5, 3, container.Height - 10), inner * fade);

        }

    }

}
