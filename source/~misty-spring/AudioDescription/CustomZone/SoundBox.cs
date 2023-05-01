/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;

namespace AudioDescription
{
    internal class SoundBox
    {
        internal static void Update()
        {
            try
            {
                foreach (var sound in ModEntry.SoundMessages)
                {
                    var index = ModEntry.SoundMessages.IndexOf(sound);

                    //if the var exceeds max in box
                    if (index > ModEntry.MaxMsgs)
                    {
                        //remove the oldest one
                        ModEntry.SoundMessages.RemoveAt(0);
                    }
                    else
                    {
                        //update time left and transparency. if transp. is 0, remove
                        sound.timeLeft--;
                        if (sound.timeLeft < 0f)
                        {
                            sound.transparency -= 0.02f;
                            if (sound.transparency < 0f)
                            {
                                ModEntry.SoundMessages.RemoveAt(index);
                            }
                        }
                    }
                }
            }
            catch(Exception)
            { }
        }

        //in update ticked, if config isnt the one for custombox, return
        public static void RenderedHUD(object sender, RenderedHudEventArgs e)
        {
            //if any event is going on . could also use ' Game1.CurrentEvent != null '
            if (Game1.eventUp) 
                return;

            //if using HUDM instead
            if(ModEntry.Config.Type == ModEntry.NotifType[0])
                return;
            if (Game1.activeClickableMenu != null)
                return;
            else
            {
                #region variables
                if (ModEntry.SafePositionTop == new Vector2(99f, 99f))
                {
                    //
                    int safeX = Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea.Left;
                    int safeY = Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea.Top;

                    Vector2 pos = new(safeX + 30, safeY + 20);
                    ModEntry.SafePositionTop = pos;

                    var pos2 = Game1.smallFont.MeasureString("Liquid filling container");
                    ModEntry.WidthandHeight = pos2;
                }
#endregion

                //stored in a try/catch/finally to avoid lag and flickering
                try
                {
                    Update();
                }
                catch(Exception)
                { }
                finally
                {
                    DrawBlock(
                    e.SpriteBatch,
                    ModEntry.SafePositionTop, //IsToolbarAbove ? SafePositionBottom :  
                    Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea.Left,
                    Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea.Top
                    );
                }
            }
        }

        private static void DrawBlock(SpriteBatch spriteBatch, Vector2 pos, int x, int y)
        {
            var w = (int)ModEntry.WidthandHeight.X;

            //*5 because 5 max, 25 because 5 spacing for each
            var h = (int)ModEntry.WidthandHeight.Y * 5 + 25;

            //draw box
            IClickableMenu.drawTextureBox(
                spriteBatch,
                x + 10,
                y + 10, //IsToolbarAbove ? Bottom : 
                w + 80, //add 80px just for good measure
                h,
                Color.White
                );


            //now draw text
            DrawText(
                spriteBatch,
                ModEntry.SoundMessages,
                (int)ModEntry.WidthandHeight.Y,
                new Rectangle(x, y + 10, w, h)
                );
        }

        private static void DrawText(SpriteBatch spriteBatch, List<SoundInfo> soundMessages, int height, Rectangle position)
        {
            var pos = new Vector2(position.Left, position.Bottom);
            pos.X += 20;

            foreach(var sound in soundMessages)
            {
                //reduce position so other ones are drawn on top
                pos.Y = pos.Y - height - 5;

                //this but use transparency
                Utility.drawTextWithShadow(
                    spriteBatch, 
                    sound.message, 
                    Game1.smallFont, 
                    pos, 
                    sound.color * sound.transparency
                    );
            }
        }
    }
}