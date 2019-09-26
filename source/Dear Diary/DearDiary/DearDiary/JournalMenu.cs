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
        public JournalMenu(IDataHelper helper, string modDirectory) : base((int)Utility.getTopLeftPositionForCenteringOnScreen(Game1.viewport.Width, Game1.viewport.Height).X, (int)Utility.getTopLeftPositionForCenteringOnScreen(Game1.viewport.Width, Game1.viewport.Height).X, Game1.viewport.Width, Game1.viewport.Height, true)
        {
            this.modDirectory = modDirectory;
            this.dataHelper = helper;
            displayedText = new TextField(xPositionOnScreen + 32, yPositionOnScreen + 84, width - 64, height - 96);
            this.currentDate = SDate.Now();
            diaryData = dataHelper.ReadSaveData<Dictionary<string, List<string>>>("diary-data") ?? new Dictionary<string, List<string>>();
            if (!diaryData.ContainsKey(currentDate.ToString()))
                diaryData.Add(currentDate.ToString(), new List<string> { "", });
            this.backButton = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + 32, this.yPositionOnScreen + this.height - 32 - 64, 48, 44), Game1.mouseCursors, new Rectangle(352, 495, 12, 11), 4f, false)
            {
                myID = 101,
                rightNeighborID = 102
            };
            this.nextButton = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width - 32 - 48, this.yPositionOnScreen + this.height - 32 - 64, 48, 44), Game1.mouseCursors, new Rectangle(365, 495, 12, 11), 4f, false)
            {
                myID = 102,
                leftNeighborID = this.backButton.myID
            };
            this.previousDay = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + 64, this.yPositionOnScreen + this.height - 64 - 96, 80, 76), Game1.mouseCursors, new Rectangle(352, 495, 12, 11), 4f, false)
            {
                myID = 103,
                rightNeighborID = 104
            };
            this.nextDay = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width - 64 - 80, this.yPositionOnScreen + this.height - 64 - 96, 80, 76), Game1.mouseCursors, new Rectangle(365, 495, 12, 11), 4f, false)
            {
                myID = 104,
                leftNeighborID = this.previousDay.myID
            };
            this.exportDiary = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + 640, this.yPositionOnScreen + this.height - 32 - 64, 48, 44), Game1.mouseCursors, new Rectangle(48, 640, 16, 16), 4f, false)
            {
                myID = 105,
                rightNeighborID = this.nextDay.myID
                
            };
            this.switchSpring = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + 1000, this.yPositionOnScreen + this.height - 32 - 64, 48, 44), Game1.mouseCursors, new Rectangle(704, 2256, 12, 8), 4f, false)
            {
                myID = 106,
                leftNeighborID = this.nextDay.myID
            };

            SetPage(0);
        }
        //Variables and Stuff
        int day = Game1.dayOfMonth; //Used to show Current Day
        string season = Game1.currentSeason; //Used to Show current Season
        int year = Game1.year; //Used to show Current Year
        public int currentPage; //What page you're currently on.
        public SDate currentDate; //Shows current date, can move around to change date.
        Dictionary<string, List<string>> diaryData;
        private IDataHelper dataHelper;
        public string modDirectory;
        //Buttons to change pages and days
        public ClickableTextureComponent previousDay;
        public ClickableTextureComponent nextDay;
        public ClickableTextureComponent nextButton;
        public ClickableTextureComponent backButton;
        public ClickableTextureComponent exportDiary;
        public ClickableTextureComponent switchSpring;
        public ClickableTextureComponent switchSummer;
        public ClickableTextureComponent switchFall;
        public ClickableTextureComponent switchWinter;

        private void SetPage(int newPage) //This creates new pages or sends you to a page
        {
            if (!this.diaryData.ContainsKey(currentDate.ToString()))
                this.diaryData.Add(currentDate.ToString(), new List<string> { "", });
            // set page number
            this.currentPage = Math.Max(0, newPage);

            // get current page
            List<string> pages = this.diaryData[currentDate.ToString()];
            if (this.currentPage >= pages.Count)
                pages.Add("");
            string pageText = pages[this.currentPage];

            // update display text
            this.displayedText.Text = pageText;
        }
        private void SaveCurrentPage() //This allows the game to save your data on the pages by date and page number
        {
            // Save current page

            List<string> pages = this.diaryData[currentDate.ToString()];
            pages[this.currentPage] = this.displayedText.Text;
            dataHelper.WriteSaveData("diary-data", this.diaryData);

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
            this.xPositionOnScreen = (int)Utility.getTopLeftPositionForCenteringOnScreen(1280, 720, 0, 0).X;
            this.yPositionOnScreen = (int)Utility.getTopLeftPositionForCenteringOnScreen(1280, 720, 0, 0).Y;
        }

        public TextField displayedText;

        public override void receiveKeyPress(Keys key)
        {
            if (this.displayedText.Selected || Game1.options.doesInputListContain(Game1.options.menuButton, key))
                return;
            base.receiveKeyPress(key);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (this.backButton.containsPoint(x, y))
            {
                if (this.currentPage > 0)
                {
                    this.SetPage(this.currentPage - 1);
                    Console.WriteLine("Back one Page.  Now on Entry: " + currentDate + " Page: " + currentPage);
                }

            }
            else if (this.nextButton.containsPoint(x, y))
            {
                this.SetPage(this.currentPage + 1);
                Console.WriteLine("Forward one Page.  Now on Entry: " + currentDate + " Page: " + currentPage);

            }
            else if (this.previousDay.containsPoint(x, y))
            {
                if (currentDate > new SDate(1, "spring", 1))
                {
                    this.currentDate = this.currentDate.AddDays(-1);
                    this.SetPage(0);
                    Console.WriteLine("Back one Day.  Now on Entry: " + currentDate + " Page: " + currentPage);

                }
            }
            else if (this.nextDay.containsPoint(x, y))
            {
                if (this.currentDate != SDate.Now())
                {
                    this.currentDate = this.currentDate.AddDays(1);
                    this.SetPage(0);
                    Console.WriteLine("Forward one Day.  Now on Entry: " + currentDate + " Page: " + currentPage);

                }
            }
            /*  else if (this.backMonth.containsPoint(x, y))
              {
                  if {currentDate - 28 > new SDate(1, "spring", 1)
                  SDate nextSeason = currentDate.AddDays(28 - currentDate.Day + 1);
              }*/
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
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.White * 0.4f);
            /*  var day = Game1.dayOfMonth;
              string season = Game1.currentSeason;
              var year = Game1.year;*/
            //string years = "Year";
            //string dayof = "Day";
            //b.Draw(Game1.menuTexture, new Rectangle(xPositionOnScreen, yPositionOnScreen, width, height), Color.White);
            Vector2 titleSize = Game1.dialogueFont.MeasureString("Today: XX Day of Season Year XX Viewing Entry: XX Season XX Page XXX");
            drawTextureBox(b, xPositionOnScreen + 16, yPositionOnScreen, (int)titleSize.X + 32, (int)titleSize.Y + 32, Color.White);
            Utility.drawTextWithShadow(b, $"Today: Day {day} of {season} Year {year}  Viewing Entry: {currentDate} Page {currentPage + 1}", Game1.dialogueFont, new Vector2(32, 16), Game1.textColor);
            drawTextureBox(b, xPositionOnScreen + 16, yPositionOnScreen + 72, width - 32, height - 84, Color.White);
            displayedText.Draw(b);
            backButton.draw(b);
            nextButton.draw(b);
            previousDay.draw(b);
            nextDay.draw(b);
            exportDiary.draw(b);
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

            //draw mouse
            drawMouse(b);
        }
    }
}