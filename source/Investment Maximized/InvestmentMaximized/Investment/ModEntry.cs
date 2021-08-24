/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MissCoriel/InvestmentMaximized
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace Investment
{
    class ModEntry : Mod
    {
        //This is a redo of the Investment Mod.
        //With my current experience, I felt it best to re-write instead of fix.
        /* 
          ****Notes for future needs****
         */
        //Create variables
        public int Bank = 0; //The bank will hold all investment payouts, You can withdrawl at any time. Not Needed
        public int saucePrice = 100; //Sets the default price of Queen of Sauce Company
        public int sauceShares = 0; //Default for Queen of Sauce shares, which is 0
        public int clintPrice = 50; //Default price for Clint's Blacksmithing
        public int clintShare = 0; //Default shares for Clint's Blacksmithing
        public int robinPrice = 500; //Default price for Robin's Construction and Furniture
        public int robinShares = 0; //Default shares for Robin's Construction and Furniture
        public int pierrePrice = 1000; //Default Price for Pierre's General Store
        public int pierreShares = 0; //Default Shares for Pierre's General Store
        public int jojaPrice = 1500; //Default Price for Joja Incorporated
        public int jojaShares = 0; //Default Shares for Joja Incorporated
        public int zuzuPrice = 2000; //Default price for Zuzu City Investments
        public int zuzuShares = 0; //Default shares for Zuzu City Investments
        public int stardropPrice = 5000; //Default Price for Stardrop Services LTD
        public int stardropShares = 0; //Default shares for Stardrop Services LTD
        public string saveFolderName; //Unique Folder for each game
        private ConfigModel Config; //Carries the button for the menu
        internal static IMonitor ImportMonitor;
        public override void Entry (IModHelper helper)
        {
            helper.Events.GameLoop.SaveCreated += this.SaveCreated;
            helper.Events.GameLoop.DayStarted += this.DayStarted;
            helper.Events.Input.ButtonReleased += this.OnButtonReleased;
            helper.Events.GameLoop.DayEnding += this.DayEnding;
            this.Config = helper.ReadConfig<ConfigModel>();
            ImportMonitor = this.Monitor;
        }

        public void SaveCreated(object sender, SaveCreatedEventArgs e)
        {
            //string farmName = Game1.player.farmName.ToString();
            saveFolderName = Constants.SaveFolderName;

            //create Directory for info
            if (!Directory.Exists(Environment.CurrentDirectory + "\\InvestmentData"))
            {
                Directory.CreateDirectory(Environment.CurrentDirectory + "\\InvestmentData");
            }
            Directory.CreateDirectory(Environment.CurrentDirectory + $"\\InvestmentData\\{saveFolderName}");
            if(!File.Exists(Environment.CurrentDirectory + $"\\InvestmentData\\{saveFolderName}\\shrdtm") && !File.Exists(Environment.CurrentDirectory + $"\\InvestmentData\\{saveFolderName}\\prcdtm"))
            {
                SaveInitial();
            }

        }
        public void SaveInitial()
        {
            saveFolderName = Constants.SaveFolderName;
            string sharePath = Environment.CurrentDirectory + $"\\InvestmentData\\{saveFolderName}\\shrdtm";
            string pricePath = Environment.CurrentDirectory + $"\\InvestmentData\\{saveFolderName}\\prcdtm";
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
            int i = 0; //start at beginning of manual list
            foreach (var item in shareData)//Converts the Manual list to a string array
            {
                parse[i] = item.ToString();
                i++;
            }
            File.WriteAllLines(sharePath, parse);
            List<string> sharePrice = new List<string>();
            sharePrice.Add(clintPrice.ToString());
            sharePrice.Add(saucePrice.ToString());
            sharePrice.Add(robinPrice.ToString());
            sharePrice.Add(pierrePrice.ToString());
            sharePrice.Add(jojaPrice.ToString());
            sharePrice.Add(zuzuPrice.ToString());
            sharePrice.Add(stardropPrice.ToString());
            File.WriteAllLines(pricePath, sharePrice);
        }
        public void SavePrices()
        {
            string pricePath = Environment.CurrentDirectory + $"\\InvestmentData\\{saveFolderName}\\prcdtm";
            if(File.Exists(pricePath))
            {
                File.Delete(pricePath);
            }
            List<string> sharePrice = new List<string>();
            sharePrice.Add(clintPrice.ToString());
            sharePrice.Add(saucePrice.ToString());
            sharePrice.Add(robinPrice.ToString());
            sharePrice.Add(pierrePrice.ToString());
            sharePrice.Add(jojaPrice.ToString());
            sharePrice.Add(zuzuPrice.ToString());
            sharePrice.Add(stardropPrice.ToString());
            File.WriteAllLines(pricePath, sharePrice);
            sharePrice.Clear();
        }
        public void DayStarted(object sender, DayStartedEventArgs e)
        {
            //On Day start, We will change all prices, then take the old prices and move them to a new file to determine value changes.  After that, save all the new information.
            SharePriceModify();
            if(File.Exists(Environment.CurrentDirectory + $"\\InvestmentData\\{saveFolderName}\\prcdtm"))
            {
                File.Delete(Environment.CurrentDirectory + $"\\InvestmentData\\{saveFolderName}\\prcdtm");
            }
            SavePrices();

        }
        public void DayEnding(object sender, DayEndingEventArgs e)
        {
            if (File.Exists(Environment.CurrentDirectory + $"\\InvestmentData\\{saveFolderName}\\yprcdtm"))
            {
                File.Delete(Environment.CurrentDirectory + $"\\InvestmentData\\{saveFolderName}\\yprcdtm");
            }
            File.Copy(Environment.CurrentDirectory + $"\\InvestmentData\\{saveFolderName}\\prcdtm", Environment.CurrentDirectory + $"\\InvestmentData\\{saveFolderName}\\yprcdtm");

        }
        public void SharePriceModify()
        {
            //First, Read Data File
            saveFolderName = Constants.SaveFolderName;
            List<string> SharePrices = new List<string>();
            SharePrices.AddRange(File.ReadAllLines(Environment.CurrentDirectory + $"\\InvestmentData\\{saveFolderName}\\prcdtm"));
            clintPrice = Int32.Parse(SharePrices[0]);
            saucePrice = Int32.Parse(SharePrices[1]);
            robinPrice = Int32.Parse(SharePrices[2]);
            pierrePrice = Int32.Parse(SharePrices[3]);
            jojaPrice = Int32.Parse(SharePrices[4]);
            zuzuPrice = Int32.Parse(SharePrices[5]);
            stardropPrice = Int32.Parse(SharePrices[6]);
            Random incriment = new Random();
            //Start With Clint
            int roll = incriment.Next(-5, 10);
            Monitor.Log($"Clint's Blacksmithing changed price: {roll}%", LogLevel.Debug);
            double percent = roll / 100.0;
            double clintChunk = clintPrice * percent;
            clintPrice = clintPrice + (int)clintChunk;
            //Sauce
            roll = incriment.Next(-5, 10);
            Monitor.Log($"Queen of Sauce Company changed price: {roll}%", LogLevel.Debug);
            percent = roll / 100.0;
            double sauceChunk = saucePrice * percent;
            saucePrice = saucePrice + (int)sauceChunk;
            //Robin
            roll = incriment.Next(-5, 10);
            Monitor.Log($"Robin's Construction and Furniture changed price: {roll}%", LogLevel.Debug);
            percent = roll / 100.0;
            double robinChunk = robinPrice * percent;
            robinPrice = robinPrice + (int)robinChunk;
            //Pierre
            roll = incriment.Next(-5, 10);
            Monitor.Log($"Pierre's General Store has changed price: {roll}%", LogLevel.Debug);
            percent = roll / 100.0;
            double pierreChunk = pierrePrice * percent;
            pierrePrice = pierrePrice + (int)pierreChunk;
            //Joja
            roll = incriment.Next(-5, 10);
            Monitor.Log($"Joja Incorporated has changed price: {roll}%", LogLevel.Debug);
            percent = roll / 100.0;
            double jojaChunk = jojaPrice * percent;
            jojaPrice = jojaPrice + (int)jojaChunk;
            //Zuzu
            roll = incriment.Next(-5, 10);
            Monitor.Log($"Zuzu City Investments has changed price: {roll}%", LogLevel.Debug);
            percent = roll / 100.0;
            double zuzuChunk = zuzuPrice * percent;
            zuzuPrice = zuzuPrice + (int)zuzuChunk;
            //Stardrop
            roll = incriment.Next(-5, 10);
            Monitor.Log($"Stardrop Services has changed price: {roll}%", LogLevel.Debug);
            percent = roll / 100.0;
            double stardropChunk = stardropPrice * percent;
            stardropPrice = stardropPrice + (int)stardropChunk;
            
            SavePrices();
            Game1.addHUDMessage(new HUDMessage("Share Prices Have Changed!"));
        }
        public void OnButtonReleased(object sender, ButtonReleasedEventArgs e)
        {
            if(e.Button == this.Config.stockMarket)
            {
                if (Game1.activeClickableMenu == null)
                    Game1.activeClickableMenu = new StockMarket(this.Helper.Data, this.Helper.DirectoryPath);
            }
        }
    }
}
