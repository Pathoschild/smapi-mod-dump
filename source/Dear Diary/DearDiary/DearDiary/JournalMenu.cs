/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MissCoriel/Dear-Diary
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;
using StardewModdingAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using Newtonsoft.Json;
using System.IO;

namespace StardewJournal.UI
{
    public class JournalMenu : IClickableMenu
    {
        
        
        public JournalMenu(IDataHelper helper, string modDirectory) : base((int)Utility.getTopLeftPositionForCenteringOnScreen(980, 470).X, (int)Utility.getTopLeftPositionForCenteringOnScreen(980, 470).X, 980, 470, true)
        {
            int menuX = (int)Utility.getTopLeftPositionForCenteringOnScreen(980, 470).X;
            int menuY = (int)Utility.getTopLeftPositionForCenteringOnScreen(980, 470).Y;

            this.modDirectory = modDirectory;
            this.dataHelper = helper;
            displayedText = new TextField(xPositionOnScreen - 450, yPositionOnScreen - 400 , width + 925, height + 300);
            this.currentDate = SDate.Now();
            diaryData = dataHelper.ReadSaveData<Dictionary<string, List<string>>>("diary-data") ?? new Dictionary<string, List<string>>();
            if (!diaryData.ContainsKey(currentDate.ToString()))
                diaryData.Add(currentDate.ToString(), new List<string> { "", });
            this.backButton = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen - 400, this.yPositionOnScreen + this.height , 48, 44), Game1.mouseCursors, new Rectangle(352, 495, 12, 11), 4f, false)
            {
                myID = 101,
                rightNeighborID = 102
            };
            this.nextButton = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + 1350 , this.yPositionOnScreen + this.height , 48, 44), Game1.mouseCursors, new Rectangle(365, 495, 12, 11), 4f, false)
            {
                myID = 102,
                leftNeighborID = this.backButton.myID
            };
            this.previousDay = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen -375, this.yPositionOnScreen + this.height - 40, 48, 44), Game1.mouseCursors, new Rectangle(352, 495, 12, 11), 4f, false)
            {
                myID = 103,
                rightNeighborID = 104
            };
            this.nextDay = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + 1325, this.yPositionOnScreen + this.height - 40, 48, 44), Game1.mouseCursors, new Rectangle(365, 495, 12, 11), 4f, false)
            {
                myID = 104,
                leftNeighborID = this.previousDay.myID
            };
            this.exportDiary = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + 400, this.yPositionOnScreen + this.height , 48, 44), Game1.mouseCursors, new Rectangle(48, 640, 16, 16), 4f, false)
            {
                myID = 105,
                rightNeighborID = this.nextDay.myID
                
            };
            this.backMonth = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen , this.yPositionOnScreen + this.height , 48, 44), Game1.mouseCursors, new Rectangle(421, 459, 11, 12), 4f, false)
            {
                myID = 106,
                leftNeighborID = this.nextDay.myID
            };
            this.nextMonth = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + 800, this.yPositionOnScreen + this.height, 48, 44), Game1.mouseCursors, new Rectangle(421, 472, 11, 12), 4f, false)
            {
                myID = 107,
                leftNeighborID = this.backMonth.myID
            };
            RemoveBlanks();
            SetPage(0);
            
        }
        //Variables and Stuff
        int day = Game1.dayOfMonth; //Used to show Current Day
        string season = Game1.currentSeason; //Used to Show current Season
        int year = Game1.year; //Used to show Current Year
        public int currentPage; //What page you're currently on.
        public SDate currentDate; //Shows current date, can move around to change date.
        public string currentEntry; //update to last known
        public int entryCount; // monitors entry count
        public bool dateUp;
        public bool dateDown;
        Dictionary<string, List<string>> diaryData;
        private IDataHelper dataHelper;
        public string modDirectory;
        //Buttons to change pages and days
        public ClickableTextureComponent previousDay;
        public ClickableTextureComponent nextDay;
        public ClickableTextureComponent nextButton;
        public ClickableTextureComponent backButton;
        public ClickableTextureComponent exportDiary;
        public ClickableTextureComponent backMonth;
        public ClickableTextureComponent nextMonth;

        private void SetPage(int newPage) //This creates new pages or sends you to a page
        {

            if (!this.diaryData.ContainsKey(currentDate.ToString()) && currentDate == SDate.Now())
                this.diaryData.Add(currentDate.ToString(), new List<string> { "", });
            // set page number
            this.currentPage = Math.Max(0, newPage);

            // get current page
            while (!this.diaryData.ContainsKey(currentDate.ToString()))
            {
                if (dateDown == true)
                {
                    currentDate = this.currentDate.AddDays(-1);
                }
                if (dateUp == true)
                {
                    currentDate = this.currentDate.AddDays(1);
                }
            }

            if (this.diaryData.ContainsKey(currentDate.ToString()))
            {
                dateDown = false;
                dateUp = false;

            }
            List<string> pages = this.diaryData[currentDate.ToString()];
            if (this.currentPage >= pages.Count)
                pages.Add("");
            string pageText = pages[this.currentPage];

            // update display text
            this.displayedText.Text = pageText;
        }
        private void RemoveBlanks()
        {
            // remove old blank pages
            foreach (KeyValuePair<string, List<string>> entry in this.diaryData.ToArray())
            {
                // delete empty pages in entry
                List<string> pages = entry.Value;
                for (int i = pages.Count - 1; i >= 0; i--)
                {
                    if (pages[i] == "")
                    {
                        DearDiary.Mod.TempMonitor.Log($"Blank page found! Deleting page {i + 1} in {entry.Key}", LogLevel.Debug);
                        pages.RemoveAt(i);
                    }
                }

                // delete entire entry if it's empty
                if (!pages.Any())
                {
                    DearDiary.Mod.TempMonitor.Log($"Entry {entry.Key} has no pages, deleting entire entry.", LogLevel.Debug);
                    diaryData.Remove(entry.Key);
                }
            }
        }
        private void SaveCurrentPage() //This allows the game to save your data on the pages by date and page number
        {
            // Save current page
            try
            {
                List<string> pages = this.diaryData[currentDate.ToString()];
                pages[this.currentPage] = this.displayedText.Text;
                dataHelper.WriteSaveData("diary-data", this.diaryData);

            }
            catch (Exception ex)
            {
                DearDiary.Mod.TempMonitor.Log(ex.Message, LogLevel.Error);
            }
        }
        public override void update(GameTime time) //This saves your text often.
        {
            base.update(time);
            this.SaveCurrentPage();
        }

        public void WriteToFile()
        {
            
            string output = "";
            var diaryData = dataHelper.ReadSaveData<Dictionary<string, List<string>>>("diary-data") ?? new Dictionary<string, List<string>>();
            foreach (var data in diaryData)
            {
                output += data.Key + Environment.NewLine + "----------" + Environment.NewLine;
                int pageCounter = 1;
                foreach (var page in data.Value)
                {
                    output += "Page " + (pageCounter++) + "/" + data.Value.Count + Environment.NewLine;
                    output += page + Environment.NewLine + Environment.NewLine;
                }
            }
            File.WriteAllText(Path.Combine(modDirectory, "diary.txt"), output);
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            this.xPositionOnScreen = (int)Utility.getTopLeftPositionForCenteringOnScreen(980, 470).X;
            this.yPositionOnScreen = (int)Utility.getTopLeftPositionForCenteringOnScreen(980, 470).Y;
        }

        public TextField displayedText;


        public override void receiveKeyPress(Keys key)

        {
            int value = (int)key;
            DearDiary.Mod.TempMonitor.Log($"Pressed {value}", LogLevel.Trace); //still there but invisible incase something is wrong in the future.
            /*if (this.displayedText.Selected || Game1.options.doesInputListContain(Game1.options.menuButton, key))
                return;*/
            if (key == Keys.E)
                return;
            if (Constants.TargetPlatform == GamePlatform.Mac && value == 8)
                this.displayedText.RecieveSpecialInput(key);
            if (Constants.TargetPlatform == GamePlatform.Mac && value == 13)
                this.displayedText.RecieveSpecialInput(key);
            if (Constants.TargetPlatform == GamePlatform.Linux && value == 8)
                this.displayedText.RecieveSpecialInput(key);
            if (Constants.TargetPlatform == GamePlatform.Linux && value == 13)
                this.displayedText.RecieveSpecialInput(key);

            base.receiveKeyPress(key);
        }
        public void SetDate(int offset)
        {
            // validate
            if (offset < 0 && this.currentDate.DaysSinceStart <= offset)
                return;
            if (offset > 0 && this.currentDate >= SDate.Now())
                return;

            // change date
            this.currentDate = this.currentDate.AddDays(offset);
            if (this.currentDate > SDate.Now())
                this.currentDate = SDate.Now();

            // override year to fix bug in SMAPI 2.x
            if (Constants.ApiVersion.IsOlderThan("3.0") && this.currentDate.Season == "winter" && this.currentDate.Day == 28)
            {
                int year = offset < 0
                   ? SDate.Now().Year - 1
                   : SDate.Now().Year;
                currentDate = new SDate(currentDate.Day, currentDate.Season, year);
            }
        }
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (this.backButton.containsPoint(x, y))
            {
                if (this.currentPage > 0)
                {
                    this.SetPage(this.currentPage - 1);
                }

            }
            else if (this.nextButton.containsPoint(x, y))
            {
                this.SetPage(this.currentPage + 1);

            }
            else if (this.previousDay.containsPoint(x, y))
            {
                if (currentDate == new SDate(1, "spring", 1))
                    return;
                if (currentDate > new SDate(1, "spring", 1))
                {
                    this.SetDate(-1);
                    dateDown = true;
                    this.SetPage(0);
                }
                
            }
            else if (this.nextDay.containsPoint(x, y))
            {
                this.SetDate(1);
                dateUp = true;
                this.SetPage(0);
            }
            else if (this.backMonth.containsPoint(x, y))
            {
                if (currentDate > new SDate(1, "summer", 1))
                {
                    this.SetDate(-28 - currentDate.Day + 1);
                    this.SetPage(0);
                }
            }
            else if (this.nextMonth.containsPoint(x, y))
            {

                this.SetDate(28 - currentDate.Day + 1);
                this.SetPage(0);
            }

            else if (this.exportDiary.containsPoint(x, y))
                this.WriteToFile();

            else if (this.isWithinBounds(x, y))
                displayedText.Selected = true;
            
        }
        
        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            Game1.exitActiveMenu();
        }

        public override void draw(SpriteBatch b)
        {

            int menuX = (int)Utility.getTopLeftPositionForCenteringOnScreen(980, 470).X;
            int menuY = (int)Utility.getTopLeftPositionForCenteringOnScreen(980, 470).Y;

            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.White * 0.4f);
            /*  var day = Game1.dayOfMonth;
              string season = Game1.currentSeason;
              var year = Game1.year;*/
            //string years = "Year";
            //string dayof = "Day";
            //b.Draw(Game1.menuTexture, new Rectangle(xPositionOnScreen, yPositionOnScreen, width, height), Color.White);
            Vector2 titleSize = Game1.dialogueFont.MeasureString("Today: XX Day of Season Year XX Viewing Entry: XX Season XX Page XXX");
            //string dateText = this.Helper.Translation.Get("date", { day = 1, season = "summer", year = 1 });
            drawTextureBox(b, menuX - 455, menuY - 310, (int)titleSize.X + 32, (int)titleSize.Y + 32, Color.White);
            Utility.drawTextWithShadow(b, $"Today: Day {day} of {season} Year {year}  Viewing Entry: {currentDate} Page {currentPage + 1}", Game1.dialogueFont, new Vector2(32, 16), Game1.textColor);
            drawTextureBox(b, menuX - 445, menuY - 230, width + 900, height + 500 , Color.White);
            displayedText.Draw(b);
            backButton.draw(b);
            nextButton.draw(b);
            previousDay.draw(b);
            nextDay.draw(b);
            exportDiary.draw(b);
            backMonth.draw(b);
            nextMonth.draw(b);
            if (this.backButton.containsPoint(Game1.getOldMouseX(), Game1.getOldMouseY()))
            {
                IClickableMenu.drawHoverText(b, "Back (Page)", Game1.smallFont);
            }
            if (this.nextButton.containsPoint(Game1.getOldMouseX(), Game1.getOldMouseY()))
            {
                IClickableMenu.drawHoverText(b, "Next (Page)", Game1.smallFont);
            }
            if (this.previousDay.containsPoint(Game1.getOldMouseX(), Game1.getOldMouseY()))
            {
                IClickableMenu.drawHoverText(b, "Back (Day)", Game1.smallFont);
            }
            if (this.nextDay.containsPoint(Game1.getOldMouseX(), Game1.getOldMouseY()))
            {
                IClickableMenu.drawHoverText(b, "Next (Day)", Game1.smallFont);
            }
            if (this.exportDiary.containsPoint(Game1.getOldMouseX(), Game1.getOldMouseY()))
            {
                IClickableMenu.drawHoverText(b, "Export to text!", Game1.smallFont);
            }
            if (this.backMonth.containsPoint(Game1.getOldMouseX(), Game1.getOldMouseY()))
            {
                IClickableMenu.drawHoverText(b, "Back (Season)", Game1.smallFont);
            }
            if (this.nextMonth.containsPoint(Game1.getOldMouseX(), Game1.getOldMouseY()))
            {
                IClickableMenu.drawHoverText(b, "Next (Season)", Game1.smallFont);
            }

            //base.draw(b);
            //draw mouse
            drawMouse(b);
        }
    }
}