/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MissCoriel/InvestmentMaximized
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Investment
{
    class StockMarket : IClickableMenu
    {
        public ClickableTextureComponent increaseClint;
        public ClickableTextureComponent decreaseClint;
        public ClickableTextureComponent increaseSauce;
        public ClickableTextureComponent decreaseSauce;
        public ClickableTextureComponent increaseRobin;
        public ClickableTextureComponent decreaseRobin;
        public ClickableTextureComponent increasePierre;
        public ClickableTextureComponent decreasePierre;
        public ClickableTextureComponent increaseJoja;
        public ClickableTextureComponent decreaseJoja;
        public ClickableTextureComponent increaseZuzu;
        public ClickableTextureComponent decreaseZuzu;
        public ClickableTextureComponent increaseStar;
        public ClickableTextureComponent decreaseStar;
        public ClickableTextureComponent commitChange;
        public ClickableTextureComponent closeCancel;

        // Locks in on the Textbox
        private bool TextboxSelectedExplicitly;
        // The Textbox
        private TextBox BankFunction;
        //Controller support
        private ClickableComponent BankFunctionArea;
        // Area for the textbox
        private Rectangle BankFunctionBounds;
        // Give a Gold Icon
        private ClickableTextureComponent goldIcon;

        //Variables needed
        public int Bank; //The bank will hold all investment payouts, You can withdrawl at any time.
        public int saucePrice; //Sets the default price of Queen of Sauce Company
        public int sauceShares; //Default for Queen of Sauce shares, which is 0
        public int clintPrice; //Default price for Clint's Blacksmithing
        public int clintShare; //Default shares for Clint's Blacksmithing
        public int robinPrice; //Default price for Robin's Construction and Furniture
        public int robinShares; //Default shares for Robin's Construction and Furniture
        public int pierrePrice; //Default Price for Pierre's General Store
        public int pierreShares; //Default Shares for Pierre's General Store
        public int jojaPrice; //Default Price for Joja Incorporated
        public int jojaShares; //Default Shares for Joja Incorporated
        public int zuzuPrice; //Default price for Zuzu City Investments
        public int zuzuShares; //Default shares for Zuzu City Investments
        public int stardropPrice; //Default Price for Stardrop Services LTD
        public int stardropShares; //Default shares for Stardrop Services LTD
        public string saveFolderName = Constants.SaveFolderName; //Unique Folder for each game
        public int saucePriceLast; //Sets the default price of Queen of Sauce Company
        public int clintPriceLast; //Default price for Clint's Blacksmithing
        public int robinPriceLast; //Default price for Robin's Construction and Furniture
        public int pierrePriceLast; //Default Price for Pierre's General Store
        public int jojaPriceLast; //Default Price for Joja Incorporated
        public int zuzuPriceLast; //Default price for Zuzu City Investments
        public int stardropPriceLast; //Default Price for Stardrop Services LTD
                                      //strings
        public string clintChange = "0";
        public string sauceChange = "0";
        public string robinChange = "0";
        public string pierreChange = "0";
        public string jojaChange = "0";
        public string zuzuChange = "0";
        public string stardropChange = "0";


        //Create Menu System
        public StockMarket(IDataHelper helper, string modBaseDirectory) : base((int)Utility.getTopLeftPositionForCenteringOnScreen(Game1.viewport.Width, Game1.viewport.Height).X, (int)Utility.getTopLeftPositionForCenteringOnScreen(Game1.viewport.Width, Game1.viewport.Height).X, Game1.viewport.Width, Game1.viewport.Height, true)
        {
            string modDirectory = modBaseDirectory;
            LoadData();
            int menuX = (Game1.viewport.Width - (width - 1200)) / 2;
            int menuY = (Game1.viewport.Height - (height - 592)) / 2;
            this.increaseClint = new ClickableTextureComponent(new Rectangle(menuX + 880, menuY + 16, 32, 32), Game1.mouseCursors, new Rectangle(184, 345, 7, 8), 4f, false)
            {
                myID = 101,
                rightNeighborID = 102,
            };
            this.decreaseClint = new ClickableTextureComponent(new Rectangle(menuX + 850, menuY + 16, 32, 32), Game1.mouseCursors, new Rectangle(177, 345, 7, 8), 4f, false)
            {
                myID = 102,
                rightNeighborID = 103,
            };
            this.increaseSauce = new ClickableTextureComponent(new Rectangle(menuX + 880, menuY + 64, 32, 32), Game1.mouseCursors, new Rectangle(184, 345, 7, 8), 4f, false)
            {
                myID = 103,
                rightNeighborID = 104,
            };
            this.decreaseSauce = new ClickableTextureComponent(new Rectangle(menuX + 850, menuY + 64, 32, 32), Game1.mouseCursors, new Rectangle(177, 345, 7, 8), 4f, false)
            {
                myID = 104,
                rightNeighborID = 105,
            };
            this.increaseRobin = new ClickableTextureComponent(new Rectangle(menuX + 880, menuY + 112, 32, 32), Game1.mouseCursors, new Rectangle(184, 345, 7, 8), 4f, false)
            {
                myID = 105,
                rightNeighborID = 106,
            };
            this.decreaseRobin = new ClickableTextureComponent(new Rectangle(menuX + 850, menuY + 112, 32, 32), Game1.mouseCursors, new Rectangle(177, 345, 7, 8), 4f, false)
            {
                myID = 106,
                rightNeighborID = 107,
            };
            this.increasePierre = new ClickableTextureComponent(new Rectangle(menuX + 880, menuY + 160, 32, 32), Game1.mouseCursors, new Rectangle(184, 345, 7, 8), 4f, false)
            {
                myID = 108,
                rightNeighborID = 109,
            };
            this.decreasePierre = new ClickableTextureComponent(new Rectangle(menuX + 850, menuY + 160, 32, 32), Game1.mouseCursors, new Rectangle(177, 345, 7, 8), 4f, false)
            {
                myID = 109,
                rightNeighborID = 110,
            };
            this.increaseJoja = new ClickableTextureComponent(new Rectangle(menuX + 880, menuY + 208, 32, 32), Game1.mouseCursors, new Rectangle(184, 345, 7, 8), 4f, false)
            {
                myID = 110,
                rightNeighborID = 111,
            };
            this.decreaseJoja = new ClickableTextureComponent(new Rectangle(menuX + 850, menuY + 208, 32, 32), Game1.mouseCursors, new Rectangle(177, 345, 7, 8), 4f, false)
            {
                myID = 111,
                rightNeighborID = 112,
            };
            this.increaseZuzu = new ClickableTextureComponent(new Rectangle(menuX + 880, menuY + 256, 32, 32), Game1.mouseCursors, new Rectangle(184, 345, 7, 8), 4f, false)
            {
                myID = 112,
                rightNeighborID = 113,
            };
            this.decreaseZuzu = new ClickableTextureComponent(new Rectangle(menuX + 850, menuY + 256, 32, 32), Game1.mouseCursors, new Rectangle(177, 345, 7, 8), 4f, false)
            {
                myID = 113,
                rightNeighborID = 114,
            };
            this.increaseStar = new ClickableTextureComponent(new Rectangle(menuX + 880, menuY + 304, 32, 32), Game1.mouseCursors, new Rectangle(184, 345, 7, 8), 4f, false)
            {
                myID = 114,
                rightNeighborID = 115,
            };
            this.decreaseStar = new ClickableTextureComponent(new Rectangle(menuX + 850, menuY + 304, 32, 32), Game1.mouseCursors, new Rectangle(177, 345, 7, 8), 4f, false)
            {
                myID = 115,
                rightNeighborID = 116,
            };


        }
        public void LoadData()
        {
            if(File.Exists(Environment.CurrentDirectory + $"\\InvestmentData\\{saveFolderName}\\prcdtm"))
            {
                List<string> SharePrices = new List<string>();
                SharePrices.AddRange(File.ReadAllLines(Environment.CurrentDirectory + $"\\InvestmentData\\{saveFolderName}\\prcdtm"));
                clintPrice = Int32.Parse(SharePrices[0]);
                saucePrice = Int32.Parse(SharePrices[1]);
                robinPrice = Int32.Parse(SharePrices[2]);
                pierrePrice = Int32.Parse(SharePrices[3]);
                jojaPrice = Int32.Parse(SharePrices[4]);
                zuzuPrice = Int32.Parse(SharePrices[5]);
                stardropPrice = Int32.Parse(SharePrices[6]);
                ModEntry.ImportMonitor.Log("Add in these values, " + string.Join(", ", SharePrices) + $"Stardrop Price = {stardropPrice}", LogLevel.Debug);
            }
            if(!File.Exists(Environment.CurrentDirectory + $"\\InvestmentData\\{saveFolderName}\\prcdtm"))
            {
                clintPrice = 50;
                saucePrice = 100;
                robinPrice = 500;
                pierrePrice = 1000;
                jojaPrice = 1500;
                zuzuPrice = 2000;
                stardropPrice = 5000;
            }
            if (File.Exists(Environment.CurrentDirectory + $"\\InvestmentData\\{saveFolderName}\\yprcdtm"))
            {
                List<string> SharePrices = new List<string>();
                SharePrices.AddRange(File.ReadAllLines(Environment.CurrentDirectory + $"\\InvestmentData\\{saveFolderName}\\yprcdtm"));
                clintPriceLast = Int32.Parse(SharePrices[0]);
                saucePriceLast = Int32.Parse(SharePrices[1]);
                robinPriceLast = Int32.Parse(SharePrices[2]);
                pierrePriceLast = Int32.Parse(SharePrices[3]);
                jojaPriceLast = Int32.Parse(SharePrices[4]);
                zuzuPriceLast = Int32.Parse(SharePrices[5]);
                stardropPriceLast = Int32.Parse(SharePrices[6]);
            }
            if (!File.Exists(Environment.CurrentDirectory + $"\\InvestmentData\\{saveFolderName}\\yprcdtm"))
            {
                clintPriceLast = 0;
                saucePriceLast = 0;
                robinPriceLast = 0;
                pierrePriceLast = 0;
                jojaPriceLast = 0;
                zuzuPriceLast = 0;
                stardropPriceLast = 0;
            }
            if(File.Exists(Environment.CurrentDirectory + $"\\InvestmentData\\{saveFolderName}\\shrdtm"))
            {
                List<string> ShareCount = new List<string>();
                ShareCount.AddRange(File.ReadAllLines(Environment.CurrentDirectory + $"\\InvestmentData\\{saveFolderName}\\shrdtm"));
                clintShare = Int32.Parse(ShareCount[0]);
                sauceShares = Int32.Parse(ShareCount[1]);
                robinShares = Int32.Parse(ShareCount[2]);
                pierreShares = Int32.Parse(ShareCount[3]);
                jojaShares = Int32.Parse(ShareCount[4]);
                zuzuShares = Int32.Parse(ShareCount[5]);
                stardropShares = Int32.Parse(ShareCount[6]);
            }
            if (!File.Exists(Environment.CurrentDirectory + $"\\InvestmentData\\{saveFolderName}\\shrdtm"))
            {
                clintShare = 0;
                sauceShares = 0;
                robinShares = 0;
                pierreShares = 0;
                jojaShares = 0;
                zuzuShares = 0;
                stardropShares = 0;
            }
            if(File.Exists(Environment.CurrentDirectory + $"\\InvestmentData\\{saveFolderName}\\bnkstmt"))
            {
                string rawBank = File.ReadAllText(Environment.CurrentDirectory + $"\\InvestmentData\\{saveFolderName}\\bnkstmt");
                Bank = Int32.Parse(rawBank);
            }
            if (!File.Exists(Environment.CurrentDirectory + $"\\InvestmentData\\{saveFolderName}\\bnkstmt"))
            {
                Bank = 0;
            }
        }
        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            this.xPositionOnScreen = (int)Utility.getTopLeftPositionForCenteringOnScreen(1280, 720, 0, 0).X;
            this.yPositionOnScreen = (int)Utility.getTopLeftPositionForCenteringOnScreen(1280, 720, 0, 0).Y;

        }
        public void SaveShares()
        {
            string sharePath = Environment.CurrentDirectory + $"\\InvestmentData\\{saveFolderName}\\shrdtm";
            //Check and remove old share data
            if(File.Exists(sharePath))
            {
                File.Delete(sharePath);
            }
            //Create Initial files
            List<int> shareData = new List<int>();
            shareData.Add(clintShare);
            shareData.Add(sauceShares);
            shareData.Add(robinShares);
            shareData.Add(pierreShares);
            shareData.Add(jojaShares);
            shareData.Add(zuzuShares);
            shareData.Add(stardropShares);
            string[] parse = new string[shareData.Count];
            ModEntry.ImportMonitor.Log("share check " + string.Join(", ", parse), LogLevel.Debug);
            int i = 0; //start at beginning of manual list
            foreach (var item in shareData)//Converts the Manual list to a string array
            {
                parse[i] = item.ToString();
                i++;
            }            
            File.WriteAllLines(sharePath, parse);
        }
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if(this.increaseClint.containsPoint(x, y))
            {
                if(Game1.player.Money >= clintPrice)
                {
                    clintShare = clintShare + 1;
                    Game1.player.Money = Game1.player.Money - clintPrice;
                    SaveShares();
                }           
                  
            }
            if(this.decreaseClint.containsPoint(x, y))
            {
                if(clintShare > 0)
                {
                    Game1.player.Money = Game1.player.Money + clintPrice;
                    clintShare = clintShare - 1;
                    SaveShares();
                }
            }
            if (this.increaseSauce.containsPoint(x, y))
            {
                if (Game1.player.Money >= saucePrice)
                {
                    sauceShares = sauceShares + 1;
                    Game1.player.Money = Game1.player.Money - saucePrice;
                    SaveShares();
                }

            }
            if (this.decreaseSauce.containsPoint(x, y))
            {
                if (sauceShares > 0)
                {
                    Game1.player.Money = Game1.player.Money + saucePrice;
                    sauceShares = sauceShares - 1;
                    SaveShares();
                }
            }
            if (this.increaseRobin.containsPoint(x, y))
            {
                if (Game1.player.Money >= robinPrice)
                {
                    robinShares = robinShares + 1;
                    Game1.player.Money = Game1.player.Money - robinPrice;
                    SaveShares();
                }

            }
            if (this.decreaseRobin.containsPoint(x, y))
            {
                if (robinShares > 0)
                {
                    Game1.player.Money = Game1.player.Money + robinPrice;
                    robinShares = robinShares - 1;
                    SaveShares();
                }
            }
            if (this.increasePierre.containsPoint(x, y))
            {
                if (Game1.player.Money >= pierrePrice)
                {
                    pierreShares = pierreShares + 1;
                    Game1.player.Money = Game1.player.Money - pierrePrice;
                    SaveShares();
                }

            }
            if (this.decreasePierre.containsPoint(x, y))
            {
                if (pierreShares > 0)
                {
                    Game1.player.Money = Game1.player.Money + pierrePrice;
                    pierreShares = pierreShares - 1;
                    SaveShares();
                }
            }
            if (this.increaseJoja.containsPoint(x, y))
            {
                if (Game1.player.Money >= jojaPrice)
                {
                    jojaShares = jojaShares + 1;
                    Game1.player.Money = Game1.player.Money - jojaPrice;
                    SaveShares();
                }

            }
            if (this.decreaseJoja.containsPoint(x, y))
            {
                if (jojaShares > 0)
                {
                    Game1.player.Money = Game1.player.Money + jojaPrice;
                    jojaShares = jojaShares - 1;
                    SaveShares();
                }
            }
            if (this.increaseZuzu.containsPoint(x, y))
            {
                if (Game1.player.Money >= zuzuPrice)
                {
                    zuzuShares = zuzuShares + 1;
                    Game1.player.Money = Game1.player.Money - zuzuPrice;
                    SaveShares();
                }

            }
            if (this.decreaseZuzu.containsPoint(x, y))
            {
                if (zuzuShares > 0)
                {
                    Game1.player.Money = Game1.player.Money + zuzuPrice;
                    zuzuShares = zuzuShares - 1;
                    SaveShares();
                }
            }
            if (this.increaseStar.containsPoint(x, y))
            {
                if (Game1.player.Money >= stardropPrice)
                {
                    stardropShares = stardropShares + 1;
                    Game1.player.Money = Game1.player.Money - stardropPrice;
                    SaveShares();
                }

            }
            if (this.decreaseStar.containsPoint(x, y))
            {
                if (stardropShares > 0)
                {
                    Game1.player.Money = Game1.player.Money + stardropPrice;
                    stardropShares = stardropShares - 1;
                    SaveShares();
                }
            }
            if(this.BankFunctionBounds.Contains(x, y))
            {
                if (!this.BankFunction.Selected || !this.TextboxSelectedExplicitly)
                    this.SelectBankFunction(explicitly: true);
            }

        }
        private void SelectBankFunction(bool explicitly)
        {
            this.BankFunction.Selected = true;
            this.TextboxSelectedExplicitly = explicitly;
            this.BankFunction.Width = this.BankFunctionBounds.Width;
        }
        private
        public override void draw(SpriteBatch b)
        {
            int menuX = (Game1.viewport.Width - (width - 800)) / 2;
            int menuY = (Game1.viewport.Height - (height - 592)) / 2;
            int clintCompare = clintPrice - clintPriceLast;
            
            int sauceCompare = saucePrice - saucePriceLast;
            int robinCompare = robinPrice - robinPriceLast;
            int pierreCompare = pierrePrice - pierrePriceLast;
            int jojaCompare = jojaPrice - jojaPriceLast;
            int zuzuCompare = zuzuPrice - zuzuPriceLast;
            int stardropCompare = stardropPrice - stardropPriceLast;
            if (clintCompare >= 0)
                clintChange = $"+{clintCompare}";
            if (clintCompare < 0)
                clintChange = $"{clintCompare}";
            if (sauceCompare >= 0)
                sauceChange = $"+{sauceCompare}";
            if (sauceCompare < 0)
                sauceChange = $"{sauceCompare}";
            if (robinCompare >= 0)
                robinChange = $"+{robinCompare}";
            if (robinCompare < 0)
                robinChange = $"{robinCompare}";
            if (pierreCompare >= 0)
                pierreChange = $"+{pierreCompare}";
            if (pierreCompare < 0)
                pierreChange = $"{pierreCompare}";
            if (jojaCompare >= 0)
                jojaChange = $"+{jojaCompare}";
            if (jojaCompare < 0)
                jojaChange = $"{jojaCompare}";
            if (zuzuCompare >= 0)
                zuzuChange = $"+{zuzuCompare}";
            if (zuzuCompare < 0)
                zuzuChange = $"{zuzuCompare}";
            if (stardropCompare >= 0)
                stardropChange = $"+{stardropCompare}";
            if (stardropCompare < 0)
                stardropChange = $"{stardropCompare}";


            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.White * 0.4f);
            Vector2 titleSize = Game1.dialogueFont.MeasureString("Stock Market");
            drawTextureBox(b, menuX, menuY - 80, (int)titleSize.X + 32, (int)titleSize.Y + 32, Color.White);
            Utility.drawTextWithShadow(b, "Stock Market", Game1.dialogueFont, new Vector2(menuX + 16, menuY - 64), Game1.textColor);
            drawTextureBox(b, menuX, menuY, width - 800, height - 650, Color.White);
            //Draw Stock Names
            Utility.drawTextWithShadow(b, "Clint(CBC)", Game1.dialogueFont, new Vector2(menuX + 16, menuY + 16), Game1.textColor);
            Utility.drawTextWithShadow(b, "Sauce(QOS)", Game1.dialogueFont, new Vector2(menuX + 16, menuY + 64), Game1.textColor);
            Utility.drawTextWithShadow(b, "Robin(RBN)", Game1.dialogueFont, new Vector2(menuX + 16, menuY + 112), Game1.textColor);
            Utility.drawTextWithShadow(b, "Pierre(PGS)", Game1.dialogueFont, new Vector2(menuX + 16, menuY + 160), Game1.textColor);
            Utility.drawTextWithShadow(b, "Joja(JOJ)", Game1.dialogueFont, new Vector2(menuX + 16, menuY + 208), Game1.textColor);
            Utility.drawTextWithShadow(b, "Zuzu(ZUZ)", Game1.dialogueFont, new Vector2(menuX + 16, menuY + 256), Game1.textColor);
            Utility.drawTextWithShadow(b, "Stardrop(SDS)", Game1.dialogueFont, new Vector2(menuX + 16, menuY + 304), Game1.textColor);
            //Draw Stock Prices
            Utility.drawTextWithShadow(b, $"{clintPrice}g", Game1.dialogueFont, new Vector2(menuX + 350, menuY + 16), Game1.textColor);
            Utility.drawTextWithShadow(b, clintChange + "g", Game1.dialogueFont, new Vector2(menuX + 500, menuY + 16), Game1.textColor);
            Utility.drawTextWithShadow(b, $"Own: {clintShare}", Game1.dialogueFont, new Vector2(menuX + 700, menuY + 16), Game1.textColor);
            Utility.drawTextWithShadow(b, $"{saucePrice}g", Game1.dialogueFont, new Vector2(menuX + 350, menuY + 64), Game1.textColor);
            Utility.drawTextWithShadow(b, sauceChange + "g", Game1.dialogueFont, new Vector2(menuX + 500, menuY + 64), Game1.textColor);
            Utility.drawTextWithShadow(b, $"Own: {sauceShares}", Game1.dialogueFont, new Vector2(menuX + 700, menuY + 64), Game1.textColor);
            Utility.drawTextWithShadow(b, $"{robinPrice}g", Game1.dialogueFont, new Vector2(menuX + 350, menuY + 112), Game1.textColor);
            Utility.drawTextWithShadow(b, robinChange + "g", Game1.dialogueFont, new Vector2(menuX + 500, menuY + 112), Game1.textColor);
            Utility.drawTextWithShadow(b, $"Own: {robinShares}", Game1.dialogueFont, new Vector2(menuX + 700, menuY + 112), Game1.textColor);
            Utility.drawTextWithShadow(b, $"{pierrePrice}g", Game1.dialogueFont, new Vector2(menuX + 350, menuY + 160), Game1.textColor);
            Utility.drawTextWithShadow(b, pierreChange + "g", Game1.dialogueFont, new Vector2(menuX + 500, menuY + 160), Game1.textColor);
            Utility.drawTextWithShadow(b, $"Own: {pierreShares}", Game1.dialogueFont, new Vector2(menuX + 700, menuY + 160), Game1.textColor);
            Utility.drawTextWithShadow(b, $"{jojaPrice}g", Game1.dialogueFont, new Vector2(menuX + 350, menuY + 208), Game1.textColor);
            Utility.drawTextWithShadow(b, jojaChange + "g", Game1.dialogueFont, new Vector2(menuX + 500, menuY + 208), Game1.textColor);
            Utility.drawTextWithShadow(b, $"Own: {jojaShares}", Game1.dialogueFont, new Vector2(menuX + 700, menuY + 208), Game1.textColor);
            Utility.drawTextWithShadow(b, $"{zuzuPrice}g", Game1.dialogueFont, new Vector2(menuX + 350, menuY + 256), Game1.textColor);
            Utility.drawTextWithShadow(b, zuzuChange + "g", Game1.dialogueFont, new Vector2(menuX + 500, menuY + 256), Game1.textColor);
            Utility.drawTextWithShadow(b, $"Own: {zuzuShares}", Game1.dialogueFont, new Vector2(menuX + 700, menuY + 256), Game1.textColor);
            Utility.drawTextWithShadow(b, $"{stardropPrice}g", Game1.dialogueFont, new Vector2(menuX + 350, menuY + 304), Game1.textColor);
            Utility.drawTextWithShadow(b, stardropChange + "g", Game1.dialogueFont, new Vector2(menuX + 500, menuY + 304), Game1.textColor);
            Utility.drawTextWithShadow(b, $"Own: {stardropShares}", Game1.dialogueFont, new Vector2(menuX + 700, menuY + 304), Game1.textColor);
            
            increaseClint.draw(b);
            decreaseClint.draw(b);
            increaseSauce.draw(b);
            decreaseSauce.draw(b);
            increaseRobin.draw(b);
            decreaseRobin.draw(b);
            increasePierre.draw(b);
            decreasePierre.draw(b);
            increaseJoja.draw(b);
            decreaseJoja.draw(b);
            increaseZuzu.draw(b);
            decreaseZuzu.draw(b);
            increaseStar.draw(b);
            decreaseStar.draw(b);
            drawMouse(b);
        }
    }
}
