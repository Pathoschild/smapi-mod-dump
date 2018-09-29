using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardewValley;
using Compatability;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using StardewValley.Menus;
using System.Reflection;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace MenuControllerCompatability
{
    public class Class1 : Mod
    {
        public static IClickableMenu overlayMenu;
        public override void Entry(IModHelper helper)
        {
            StardewModdingAPI.Events.GameEvents.UpdateTick += MenuCompatability;
            StardewModdingAPI.Events.GraphicsEvents.Resize += GraphicsEvents_Resize;
            StardewModdingAPI.Events.ControlEvents.KeyPressed += ControlEvents_KeyPressed;
            StardewModdingAPI.Events.GraphicsEvents.OnPostRenderGuiEvent += GraphicsEvents_OnPostRenderHudEvent;
        }

        private void GraphicsEvents_OnPostRenderHudEvent(object sender, EventArgs e)
        {
            if (overlayMenu != null)
            {
                overlayMenu.draw(Game1.spriteBatch);
            }
        }

        private void ControlEvents_KeyPressed(object sender, StardewModdingAPI.Events.EventArgsKeyPressed e)
        {



            //   FieldInfo f = Game1.activeClickableMenu.GetType().GetField("subMenu", BindingFlags.NonPublic | BindingFlags.Instance);
                //Log.AsyncG(l.GetType());
                //  Game1.activeClickableMenu.GetType().GetProperty("subMenu").GetValue(Game1.activeClickableMenu, null);

                /*
                MethodInfo dynMethod = l.GetType().GetMethod("receiveScrollWheelAction",
    BindingFlags.Public | BindingFlags.Instance);

                dynMethod.Invoke(l, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Public, null, new object[] { -1 }, null);
                */
            
        
            if (e.KeyPressed.ToString() == "R")
            {
                changeSubMenuTitleScreen();
            }
        }

        private void changeNameBoxOnCharacterCustomizer()
        {


            object l = new CharacterCustomization(null, null, null, false);
            object nameBox = new StardewValley.Menus.TextBox(null, null, Game1.smoothFont, Color.White);
            object text = new string(new char[5] { 'a', 'b', 'c', 'd', 'e' });
            l = (StardewValley.Menus.CharacterCustomization)Game1.activeClickableMenu.GetType().GetField("subMenu", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Game1.activeClickableMenu);
            Log.AsyncM(l);
            if (l == null)
            {
                Log.AsyncC("cry");
            }
            nameBox = (TextBox)l.GetType().GetField("nameBox", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(l);
            if (nameBox == null)
            {
                Log.AsyncM("BLARG");

            }
            Log.AsyncG(nameBox);


            text = (string)nameBox.GetType().GetField("_text", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(nameBox);
            if ((text as string) == "")
            {
                Log.AsyncM("WTF");

            }
            nameBox.GetType().GetField("_text", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(nameBox, "fuck");
        }

        private void changeSubMenuTitleScreen()
        {

            try
            {
                object F = Game1.activeClickableMenu;
                object G = (StardewValley.Menus.CharacterCustomization)Game1.activeClickableMenu.GetType().GetField("subMenu", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Game1.activeClickableMenu);
                F.GetType().GetField("subMenu", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(Game1.activeClickableMenu, new SpriteKeyboard());
            }
            catch(Exception e)
            {
                Log.AsyncY("BOOM SOMETHING WENT WRONG");
                Log.AsyncR(e);
            }
            /*
            object l = new CharacterCustomization(null, null, null, false);
            object nameBox = new StardewValley.Menus.TextBox(null, null, Game1.smoothFont, Color.White);
            object text = new string(new char[5] { 'a', 'b', 'c', 'd', 'e' });
            l = (StardewValley.Menus.CharacterCustomization)Game1.activeClickableMenu.GetType().GetField("subMenu", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Game1.activeClickableMenu);
            Log.AsyncM(l);
            if (l == null)
            {
                Log.AsyncC("cry");
            }
            nameBox = (TextBox)l.GetType().GetField("nameBox", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(l);
            if (nameBox == null)
            {
                Log.AsyncM("BLARG");

            }
            Log.AsyncG(nameBox);


            text = (string)nameBox.GetType().GetField("_text", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(nameBox);
            if ((text as string) == "")
            {
                Log.AsyncM("WTF");

            }
            nameBox.GetType().GetField("_text", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(nameBox, "fuck");
            */
        }


        private void GraphicsEvents_Resize(object sender, EventArgs e)
        {
            if (CompatabilityManager.compatabilityMenu != null)
            {
                CompatabilityManager.compatabilityMenu.resize();
            }
        }

        private void MenuCompatability(object sender, EventArgs e)
        {
            if (Game1.activeClickableMenu == null) return;
            //Log.AsyncC(Game1.activeClickableMenu.GetType());
            GamePadState currentState = GamePad.GetState(PlayerIndex.One);
            if (currentState.IsConnected == false||Game1.options.gamepadControls==false) return;
            // if (Game1.options.gamepadControls == false && useMenuFocus==false) return;
            if (CompatabilityManager.doUpdate == true)
            {
                CompatabilityManager.compatabilityMenu.Compatability();
                CompatabilityManager.compatabilityMenu.Update();
                return;
            }
            if (Game1.activeClickableMenu is StardewValley.Menus.TitleMenu && CompatabilityManager.characterCustomizer == false)
            {
                if (CompatabilityManager.doUpdate == false)
                {
                    CompatabilityManager.compatabilityMenu = new Compatability.Vanilla.TitleMenu();
                    CompatabilityManager.doUpdate = true;
                }

            }
            if (Game1.activeClickableMenu is StardewValley.Menus.TitleMenu && CompatabilityManager.characterCustomizer == true)
            {
                // compatabilityMenu = new Menus.Compatability.Vanilla.TitleMenu();
                //  compatabilityMenu.Compatability();
            }
            if (Game1.activeClickableMenu is StardewValley.Menus.TitleMenu && CompatabilityManager.loadMenu == true)
            {
                // compatabilityMenu = new Menus.Compatability.Vanilla.TitleMenu();
                //  compatabilityMenu.Compatability();
            }
            if (Game1.activeClickableMenu is StardewValley.Menus.TitleMenu && CompatabilityManager.aboutMenu == true)
            {
          //      Log.AsyncO("BOOOO");
                CompatabilityManager.compatabilityMenu = new Compatability.Vanilla.AboutMenu();
                CompatabilityManager.doUpdate = true;
                //  compatabilityMenu.Compatability();
            }
            else
            {
                // compatabilityMenu = null;
            }

        }

        public static Texture2D loadTexture(string s)
        {
            string f = Path.Combine("ControllerCompatabilityFonts", "colorlessSpriteFont", s);
            return Game1.content.Load<Texture2D>(f);
        }


    }
}
