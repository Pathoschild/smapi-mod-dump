using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.BellsAndWhistles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace StardewJournal.UI
{
    
    public class TextField : IKeyboardSubscriber
    {
        
        
        public TextField()
        {
            Text = "";
            X = 0;
            Y = 0;
            Width = 0;
            Height = 0;
        }
        public TextField(int x, int y, int width, int height)
        {
            Text = "";
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
        private bool selected = false;

        public bool Selected
        {
            get
            {
                return selected;
            }
            set
            {
                Console.WriteLine("Setting Text.Selected to " + value);
                selected = value;
                if (selected)
                {
                    Console.WriteLine("Setting Subscriber to " + this);
                    Game1.keyboardDispatcher.Subscriber = this;
                }
                else
                {
                    Console.WriteLine("Deselecting");
                }
            }
        }

        public void RecieveCommandInput(char command)
        {
            //DearDiary.Mod.TempMonitor.Log($"If you can see this then RecieveCommandInput got a {command}!", LogLevel.Debug);
        }

        public void RecieveSpecialInput(Keys key)
        {
            int value = (int)key;

            //DearDiary.Mod.TempMonitor.Log($"If you can see this then RecieveSpecialInput is {value}!", LogLevel.Debug);
            switch (value)
            {
                case 8:
                    if (Text.Length == 0) return;
                    Text = Text.Remove(Text.Length - 1);
                    return;
                case 13:
                    Text += "^";
                    return;
               /* case 127:
                    if (Text.Length == 0) return;
                    Text = Text.Remove(Text.Length - 1);
                    return;*/
                case 122:
                   
                        if (TextColour == 9)
                        {
                            TextColour = 0;
                        }
                        else
                        {
                            TextColour = TextColour + 1;
                        }
                                                 
                    return;
            }
           /* switch (key)
            {
                case Keys.Back:
                    if (Text.Length == 0) return;
                    Text = Text.Remove(Text.Length - 1);
                    return;
                case Keys.Enter:
                    Text += "^";
                    return;
                case Keys.F16:
                    if (Text.Length == 0) return;
                    Text = Text.Remove(Text.Length - 1);
                    return;
            }*/
        }
        public void RecieveTextInput(char inputChar)
        {
            Text += inputChar;
        }

        public void RecieveTextInput(string text)
        {
            this.Text += text;
        }
        public string Text { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public int TextColour { get; set; } = 0;

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            SpriteText.drawString(spriteBatch, Text, X + 32, Y + 32, 999999, Width - 64, Height, 0.75f, 0.865f, false, -1, "", TextColour);

        }
    }
}