using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace StardustCore.ModInfo
{
    /// <summary>
    /// Handles things like displaying object class types.
    /// </summary>
    public class Metadata
    {
        Color ModInfoColor;
        string ModName;

   
        /// <summary>
        /// 
        /// </summary>
        /// <param name="modColor"></param>
        /// <param name="modName"></param>
        private Metadata(Color ? modColor,string modName="")
        {
            if (modColor == null) ModInfoColor = Color.Black;
            else ModInfoColor =(Color) modColor;
            ModName = modName;
        }

        /// <summary>
        /// Parse the name of the mod that this object is from from the Namespace type.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string parseModNameFromType(Type t)
        {
            string s = t.ToString();
            string[] array = s.Split('.');
            return array[0];
        }

        /// <summary>
        /// Parse the class inside of the mod's namespace that this object is from.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string parseClassNameFromType(Type t)
        {
            string s = t.ToString();
            string[] array = s.Split('.');
            return array[array.Length-1];
        }

        /// <summary>
        /// TODO: Add the ModClass item check to chest inventory. See if I can grab activeclickable menu and instead hook the inventory component from it.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void GameEvents_UpdateTick(object sender, EventArgs e)
        {
            if (Game1.activeClickableMenu != null)
            {
                //  if (Game1.activeClickableMenu.allClickableComponents == null) return;
                try {
                    List<IClickableMenu> pages = ModCore.ModHelper.Reflection.GetField<List<IClickableMenu>>(Game1.activeClickableMenu, "pages").GetValue();
                    if (Game1.activeClickableMenu is GameMenu)
                    {
                        StardewValley.Menus.IClickableMenu s = pages[(Game1.activeClickableMenu as GameMenu).currentTab];

                       

                        foreach (var v in s.allClickableComponents)
                        {
                            if (v.containsPoint(Game1.getMouseX(), Game1.getMouseY()))
                            {

                                if (v == null) continue;
                                string fire = v.name;
                              

                                bool num = true;
                                foreach (var v2 in fire)
                                {
                                    if (v2 != '0' && v2 != '1' && v2 != '2' && v2 != '3' && v2 != '4' && v2 != '5' && v2 != '6' && v2 != '7' && v2 != '8' && v2 != '9')
                                    {
                                        num = false;
                                        break;
                                    }
                                    else continue;
                                }
                                if (num == true)
                                {
                                    int inv = Convert.ToInt32(v.name);
                                    Item I = (s as StardewValley.Menus.InventoryPage).inventory.actualInventory[inv];
                                
                                    string s1 = parseModNameFromType(I.GetType());
                                    string s2 = parseClassNameFromType(I.GetType());
                                    string s3 = Assembly.GetAssembly(I.GetType()).Location;
                                    s3 = Path.GetFileName(s3);

                  
                                    //DRAW THE INFO BOX!!!
                                    try
                                    {
                                        SpriteBatch b = new SpriteBatch(Game1.graphics.GraphicsDevice);
                                        b.Begin();
                                        float boxX =Game1.getMouseX()- (Game1.viewport.Width * .25f);
                                        float boxY =Game1.getMouseY() - (Game1.viewport.Height * .05f);
                                        float boxWidth= (Game1.viewport.Width * .25f);
                                        float boxHeight = (Game1.viewport.Height*.35f);
                                        Game1.drawDialogueBox((int)boxX,(int) boxY,(int)boxWidth, (int)boxHeight, false, true, null,false);

                                        float xText1XPos = boxX + (Game1.viewport.Width * .08f);
                                        float xText2XPos = boxX + (Game1.viewport.Width * .08f);
                                        if (s1.Length > 12)
                                        {
                                            s1 = "\n" + s1;
                                            xText1XPos = boxX + (Game1.viewport.Width * .025f);
                                        }

                                        if (s2.Length > 12)
                                        {
                                            s2 = "\n" + s2;
                                            xText2XPos = boxX + (Game1.viewport.Width * .025f);
                                        }
                                        if (s3.Length > 12)
                                        {
                                            s3 = "\n" + s3;
                                            xText1XPos = boxX + (Game1.viewport.Width * .025f);
                                        }

                                        Utility.drawTextWithShadow(Game1.spriteBatch, "Mod: ", Game1.smallFont, new Vector2(boxX + (Game1.viewport.Width * .025f), Game1.getMouseY() + (int)(Game1.viewport.Height * .1f)), Color.Black, 1, -1);
                                        Utility.drawTextWithShadow(Game1.spriteBatch, s3, Game1.smallFont, new Vector2(xText1XPos, Game1.getMouseY()+(int)(Game1.viewport.Height*.1f)), Color.Black, 1, -1);

                                        Utility.drawTextWithShadow(Game1.spriteBatch, "Class: ", Game1.smallFont, new Vector2(boxX + (Game1.viewport.Width * .025f), Game1.getMouseY() + (int)(Game1.viewport.Height * .2f)), Color.Black, 1, -1);
                                        Utility.drawTextWithShadow(Game1.spriteBatch, s2, Game1.smallFont, new Vector2(xText2XPos, Game1.getMouseY() + (int)(Game1.viewport.Height * .2f)), I.getCategoryColor(), 1, -1);
                                        b.End();
                                    }
                                    catch(Exception errr)
                                    {
                                        errr.ToString();
                                    }

                                }
                            }
                            //  if (v == null) continue;
                            // Log.AsyncC(v.name);
                            //  Log.AsyncM(v.item.Name);
                            // (s as StardewValley.Menus.InventoryPage).
                        }
                    }
                }
                catch(Exception err) //Try to parse a menu that isn't the default GameMenu
                {
                    err.ToString();
                    try
                    {
                        List<Item> inventory = (List<Item>)Game1.activeClickableMenu.GetType().GetProperty("inventory").GetValue(Game1.activeClickableMenu, null);                        
                    }
                    catch(Exception errr)
                    {
                        errr.ToString();
                        try
                        {
                        
                            IClickableMenu s = (Game1.activeClickableMenu as ItemGrabMenu).ItemsToGrabMenu;
                            if (s == null) return;
                            int i = 0;
                            foreach (var v in  s.allClickableComponents)
                            {
                                i++;
                                    if (v.containsPoint(Game1.getMouseX(), Game1.getMouseY()))
                                    {
                                   
                                    if (v == null || v.name=="") continue;
                                        string fire = v.name;

                                        bool num = true;
                                        foreach (var v2 in fire)
                                        {
                                            if (v2 != '0' && v2 != '1' && v2 != '2' && v2 != '3' && v2 != '4' && v2 != '5' && v2 != '6' && v2 != '7' && v2 != '8' && v2 != '9')
                                            {
                                                num = false;
                                                break;
                                            }
                                            else continue;
                                        }
                                  
                                    ///NUM ISN't TRUE!?!?!?!?
                                        if (num == true)
                                        {
                                        int inv = Convert.ToInt32(v.name);
                                        Item I = (s as InventoryMenu).actualInventory[inv]; //Inventory Menu is the actual menu under ItemGrabMenu
                                       // Item I = (s as StardewValley.Menus.ItemGrabMenu).inventory.actualInventory[inv]; ///I isn't being grabbed???
                                        
                                            string s1 = parseModNameFromType(I.GetType());
                                            string s2 = parseClassNameFromType(I.GetType());
                                            string s3 = Assembly.GetAssembly(I.GetType()).Location;
                                            s3 = Path.GetFileName(s3);

                                            //Draw the info Box!
                                            try
                                            {
                                            
                                                float boxX = Game1.getMouseX() - (Game1.viewport.Width * .25f);
                                                float boxY = Game1.getMouseY() - (Game1.viewport.Height * .05f);
                                                float boxWidth = (Game1.viewport.Width * .25f);
                                                float boxHeight = (Game1.viewport.Height * .35f);
                                                Game1.drawDialogueBox((int)boxX, (int)boxY, (int)boxWidth, (int)boxHeight, false, true, null, false);

                                                float xText1XPos = boxX + (Game1.viewport.Width * .08f);
                                                float xText2XPos = boxX + (Game1.viewport.Width * .08f);
                                                if (s1.Length > 12)
                                                {
                                                    s1 = "\n" + s1;
                                                    xText1XPos = boxX + (Game1.viewport.Width * .025f);
                                                }

                                                if (s2.Length > 12)
                                                {
                                                    s2 = "\n" + s2;
                                                    xText2XPos = boxX + (Game1.viewport.Width * .025f);
                                                }
                                                if (s3.Length > 12)
                                                {
                                                    s3 = "\n" + s3;
                                                    xText1XPos = boxX + (Game1.viewport.Width * .025f);
                                                }

                                                Utility.drawTextWithShadow(Game1.spriteBatch, "Mod: ", Game1.smallFont, new Vector2(boxX + (Game1.viewport.Width * .025f), Game1.getMouseY() + (int)(Game1.viewport.Height * .1f)), Color.Black, 1, -1);
                                                Utility.drawTextWithShadow(Game1.spriteBatch, s3, Game1.smallFont, new Vector2(xText1XPos, Game1.getMouseY() + (int)(Game1.viewport.Height * .1f)), Color.Black, 1, -1);

                                                Utility.drawTextWithShadow(Game1.spriteBatch, "Class: ", Game1.smallFont, new Vector2(boxX + (Game1.viewport.Width * .025f), Game1.getMouseY() + (int)(Game1.viewport.Height * .2f)), Color.Black, 1, -1);
                                                Utility.drawTextWithShadow(Game1.spriteBatch, s2, Game1.smallFont, new Vector2(xText2XPos, Game1.getMouseY() + (int)(Game1.viewport.Height * .2f)), I.getCategoryColor(), 1, -1);
                                            
                                            }
                                            catch (Exception errrr)
                                            {
                                            errrr.ToString();
                                            }

                                        }
                                    }
                                    //  if (v == null) continue;
                                    // Log.AsyncC(v.name);
                                    //  Log.AsyncM(v.item.Name);
                                    // (s as StardewValley.Menus.InventoryPage).
                                

                            }
                        }
                        catch(Exception errrr)
                        {
                            errrr.ToString();
                        }
                    }
                }
            }
            
        }

    }
}
