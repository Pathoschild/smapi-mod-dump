/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewSymphonyRemastered.Framework.V2;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardustCore.Animations;
using StardustCore.UIUtilities;
using StardustCore.UIUtilities.MenuComponents;
using StardustCore.UIUtilities.MenuComponents.ComponentsV1;
using StardustCore.UIUtilities.MenuComponents.Delegates;
using StardustCore.UIUtilities.MenuComponents.Delegates.Functionality;
using StardustCore.UIUtilities.SpriteFonts;

namespace StardewSymphonyRemastered.Framework.Menus
{
    /// <summary>
    /// TODO:
    /// Add in visual display to see what conditionals for that song have been selected.
    ///     -Add in way to delete options from this menu
    /// Add in search box functionality for selecting albums
    /// Add in search box functionality for song selection.
    /// -Add in confirmation for go back if current selected conditional is un-added (unsaved)
    /// </summary>
    public class MusicManagerMenuV2 : IClickableMenuExtended
    {
        /// <summary>The different displays for this menu.</summary>
        ///Add in stop button
        ///Add in delete funtionality
        public enum DrawMode
        {
            AlbumFancySelection,
            SongSelectionMode,
            DifferentSelectionTypesModePage, //Used for locations, events, festivals,  menus (house, exclamation mark, star, and list/book icons respectively)
            WeatherSelection,
            FestivalSelection,
            EventSelection,
            MenuSelection,
            TimeSelection,
            LocationSelection,
            DaySelection,
            SeasonSelection,
            ConditionalViewMode
        }
        /// <summary>
        /// All the buttons that get displayed for the music pack albums.
        /// </summary>
        public List<Button> musicAlbumButtons;
        /// <summary>
        /// The currenty selected music pack button.
        /// </summary>
        public Button currentMusicPackAlbum;
        /// <summary>
        /// The currenty selected button for the song from the current msuic pack.
        /// </summary>
        public Button currentSelectedSong;
        /// <summary>
        /// The current selected button for the weather conditional.
        /// </summary>
        public Button currentlySelectedWeather; //Used to display what weather the user selected
        /// <summary>
        /// The current selected button for the time conditional.
        /// </summary>
        public Button currentlySelectedTime;
        /// <summary>
        /// The current selected button for the time location.
        /// </summary>
        public Button currentlySelectedLocation;
        /// <summary>
        /// The current selected button for the day conditional.
        /// </summary>
        public Button currentlySelectedDay;
        /// <summary>
        /// The current selected button for the season conditional.
        /// </summary>
        public Button currentlySelectedSeason;
        /// <summary>
        /// The current selected button for the festival conditional.
        /// </summary>
        public Button currentlySelectedFestival;
        /// <summary>
        /// The current selected button for the event conditional.
        /// </summary>
        public Button currentlySelectedEvent;
        /// <summary>
        /// The current selected button for the time conditional.
        /// </summary>
        public Button currentlySelectedMenu;
        /// <summary>
        /// The add button that adds a conditional to the currently selected song.
        /// </summary>
        public Button addButton;
        /// <summary>
        /// The delete button that clears all conditionals from a song.
        /// </summary>
        public Button deleteButton;
        /// <summary>
        /// The play button that plays the currently selected song.
        /// </summary>
        public Button playButton;
        /// <summary>
        /// The play button that stops the currently selected song.
        /// </summary>
        public Button stopButton;
        /// <summary>
        /// The back button to go to a previous menu page.
        /// </summary>
        public Button backButton;
        /// <summary>
        /// The save button that saves the current options to the song.
        /// </summary>
        public Button saveButton;

        public Button lastPageButton;
        public Button nextPageButton;

        private Button conditionalViewButton;

        private Vector2 seasonButtonPosition;
        private Vector2 timeButtonPosition;
        private Vector2 weatherButtonPosition;
        private Vector2 dayButtonPosition;
        private Vector2 locationButtonPosition;
        private Vector2 menuButtonPosition;
        private Vector2 festivalButtonPosition;
        private Vector2 eventButtonPosition;

        /// <summary>
        /// The currently selected music pack album.
        /// </summary>
        private MusicPackV2 CurrentMusicPack
        {
            get
            {
                if (this.currentMusicPackAlbum == null) return null;
                else
                {
                    return (MusicPackV2)this.currentMusicPackAlbum.buttonFunctionality.hover.paramaters[0]; //WHY did I think this was a good idea???
                }
            }
        }
        /// <summary>
        /// The current draw mode to determine what menu components to display and when.
        /// </summary>
        public DrawMode drawMode;
        /// <summary>
        /// The current index for the album selection page.
        /// </summary>
        public int currentAlbumIndex;
        /// <summary>
        /// The current page index for the song selection page.
        /// </summary>
        public int currentSongPageIndex;
        /// <summary>
        /// The current page index for the location selection page.
        /// </summary>
        public int locationPageIndex;
        /// <summary>
        /// The current page index for the festival selection page.
        /// </summary>
        public int festivalPageIndex;
        /// <summary>
        /// The current page index for the event selection page.
        /// </summary>
        public int eventPageIndex;
        /// <summary>
        /// The current page index for the menu selection page.
        /// </summary>
        public int menuPageIndex;
        /// <summary>
        /// The current page index for the time of day page.
        /// </summary>
        public int timePageIndex;
        /// <summary>
        /// A list that holds all of the buttons for display on the menu.
        /// </summary>
        public List<Button> fancyButtons;
        /// <summary>
        /// Used to control how fast we can cycle through the menu so that we can use wasd cycle controls.
        /// </summary>
        public int framesSinceLastUpdate;
        /// <summary>
        /// Used to determine if the search box was selected.
        /// </summary>
        public bool searchBoxSelected;
        /// <summary>
        /// The hover text for this menu.
        /// </summary>
        public string hoverText;

        public int conditionalViewPageIndex;


        /// <summary>
        /// Constructor for the menu.
        /// </summary>
        /// <param name="width"> The width of the menu.</param>
        /// <param name="height">The height of the menu.</param>
        public MusicManagerMenuV2(float width, float height)
        {
            this.width = (int)width;
            this.height = (int)height;
            this.texturedStrings = new List<StardustCore.UIUtilities.SpriteFonts.Components.TexturedString>();
            this.musicAlbumButtons = new List<Button>();
            //thismusicAlbumButtons.Add(new Button("myButton", new Rectangle(100, 100, 64, 64), StardewSymphony.textureManager.getTexture("MusicNote").Copy(StardewSymphony.ModHelper), "mynote", new Rectangle(0, 0, 16, 16), 4f, new Animation(new Rectangle(0, 0, 16, 16)), Color.White, Color.White,new ButtonFunctionality(new DelegatePairing(hello,null),null,null),false)); //A button that does nothing on the left click.  

            this.fancyButtons = new List<Button>();

            //Initialize music album icons.
            int numOfButtons = 0;
            int rows = 0;
            foreach (MusicPackV2 musicPack in StardewSymphony.musicManager.MusicPacks.Values)
            {
                var sortedQuery = musicPack.SongInformation.songs.OrderBy(name => name);
                //musicPack.SongInformation.listOfSongsWithoutTriggers = sortedQuery.ToList(); //Alphabetize.
                if (musicPack.Icon == null)
                {
                    Texture2DExtended texture = StardewSymphony.textureManager.getTexture("MusicDisk");
                    float scale = 1.00f / (texture.Width / 64f);

                    this.musicAlbumButtons.Add(new Button(musicPack.Name, new Rectangle(100 + (numOfButtons * 100), 125 + (rows * 100), 64, 64), texture, "", new Rectangle(0, 0, 16, 16), scale, new Animation(new Rectangle(0, 0, 16, 16)), StardustCore.IlluminateFramework.Colors.randomColor(), Color.White, new ButtonFunctionality(new DelegatePairing(null, new List<object> { musicPack }), null, new DelegatePairing(null, new List<object> { musicPack })), false));
                }
                else
                {
                    float scale = 1.00f / (musicPack.Icon.Width / 64f);
                    this.musicAlbumButtons.Add(new Button(musicPack.Name, new Rectangle(100 + (numOfButtons * 100), 125 + (rows * 100), 64, 64), musicPack.Icon, "", new Rectangle(0, 0, musicPack.Icon.Width, musicPack.Icon.Height), scale, new Animation(new Rectangle(0, 0, 16, 16)), StardustCore.IlluminateFramework.LightColorsList.Black, StardustCore.IlluminateFramework.LightColorsList.Black, new ButtonFunctionality(new DelegatePairing(null, new List<object> { musicPack }), null, new DelegatePairing(null, new List<object> { musicPack })), false));
                }

                numOfButtons++;
                if (numOfButtons > 8)
                {
                    numOfButtons = 0;
                    rows++;
                }
            }

            //determine background color
            if (Game1.timeOfDay < 1200) this.dialogueBoxBackgroundColor = Color.SpringGreen;
            if (Game1.timeOfDay >= 1200 && Game1.timeOfDay < 1800) this.dialogueBoxBackgroundColor = Color.White;
            if (Game1.timeOfDay >= 1800) this.dialogueBoxBackgroundColor = Color.DarkViolet;


            this.currentAlbumIndex = 0;
            this.locationPageIndex = 0;
            this.menuPageIndex = 0;
            this.festivalPageIndex = 0;
            this.eventPageIndex = 0;
            this.timePageIndex = 0;
            this.conditionalViewPageIndex = 0;
            this.drawMode = DrawMode.AlbumFancySelection;

            this.updateFancyButtons();
            this.framesSinceLastUpdate = 0;

            this.searchBoxSelected = false;
            this.menuTextures = new List<Texture2DExtended>();

            Vector2 playPos = new Vector2(this.width * .1f + 128 + 32, this.height * .05f + 128); //Put it to the right of the music disk
            this.playButton = new Button("PlayButton", new Rectangle((int)playPos.X, (int)playPos.Y, 64, 64), StardewSymphony.textureManager.getTexture("PlayButton"), "", new Rectangle(0, 0, 16, 16), 4f, new Animation(new Rectangle(0, 0, 16, 16)), Color.White, Color.White, new ButtonFunctionality(null, null, null));

            Vector2 stopPos = new Vector2(this.width * .1f + 192 + 32, this.height * .05f + 128); //Put it to the right of the music disk
            this.stopButton = new Button("StopButton", new Rectangle((int)stopPos.X, (int)stopPos.Y, 64, 64), StardewSymphony.textureManager.getTexture("StopButton"), "", new Rectangle(0, 0, 16, 16), 4f, new Animation(new Rectangle(0, 0, 16, 16)), Color.White, Color.White, new ButtonFunctionality(null, null, null));

            Vector2 addPos = new Vector2(this.width * .1f + 256 + 32, this.height * .05f + 128); //Put it to the right of the music disk
            this.addButton = new Button("AddIcon", new Rectangle((int)addPos.X, (int)addPos.Y, 64, 64), StardewSymphony.textureManager.getTexture("AddIcon"), "", new Rectangle(0, 0, 32, 32), 2f, new Animation(new Rectangle(0, 0, 32, 32)), Color.White, Color.White, new ButtonFunctionality(null, null, null));

            Vector2 delPos = new Vector2(this.width * .1f + 320 + 32, this.height * .05f + 128); //Put it to the right of the music disk
            this.deleteButton = new Button("DeleteIcon", new Rectangle((int)delPos.X, (int)delPos.Y, 64, 64), StardewSymphony.textureManager.getTexture("DeleteIcon"), "", new Rectangle(0, 0, 32, 32), 2f, new Animation(new Rectangle(0, 0, 32, 32)), Color.White, Color.White, new ButtonFunctionality(null, null, null));

            Vector2 backPos = new Vector2(this.width * .1f + 64, this.height * .05f); //Put it to the right of the music disk
            this.backButton = new Button("BackButton", new Rectangle((int)backPos.X, (int)backPos.Y, 64, 64), StardewSymphony.textureManager.getTexture("BackButton"), "", new Rectangle(0, 0, 16, 16), 4f, new Animation(new Rectangle(0, 0, 16, 16)), Color.White, Color.White, new ButtonFunctionality(null, null, null));

            Vector2 savePos = new Vector2(this.width * .1f + 256 + 32, this.height * .05f + 64); //Put it to the right of the music disk
            this.saveButton = new Button("SaveIcon", new Rectangle((int)savePos.X, (int)savePos.Y, 64, 64), StardewSymphony.textureManager.getTexture("SaveIcon"), "", new Rectangle(0, 0, 32, 32), 2f, new Animation(new Rectangle(0, 0, 32, 32)), Color.White, Color.White, new ButtonFunctionality(null, null, null));

            Vector2 conditionalPos = new Vector2(this.width * .1f + 320 + 32, this.height * .05f + 64); //Put it to the right of the music disk
            this.conditionalViewButton = new Button("ConditionalView", new Rectangle((int)conditionalPos.X, (int)conditionalPos.Y, 64, 64), StardewSymphony.textureManager.getTexture("QuestionMark"), "", new Rectangle(0, 0, 32, 32), 2f, new Animation(new Rectangle(0, 0, 32, 32)), Color.White, Color.White, new ButtonFunctionality(null, null, null));

            Vector2 leftPos = this.getFixedPositionFromMenu(new Vector2(736, 64)); //Put it to the right of the music disk
            this.lastPageButton = new Button("LastPage", new Rectangle((int)leftPos.X, (int)leftPos.Y, 64, 64), StardewSymphony.textureManager.getTexture("LastPage"), "", new Rectangle(0, 0, 32, 32), 2f, new Animation(new Rectangle(0, 0, 32, 32)), Color.White, Color.White, new ButtonFunctionality(null, null, null));
            Vector2 rightPos = this.getFixedPositionFromMenu(new Vector2(972, 64)); //Put it to the right of the music disk
            this.nextPageButton = new Button("NextPage", new Rectangle((int)rightPos.X, (int)rightPos.Y, 64, 64), StardewSymphony.textureManager.getTexture("NextPage"), "", new Rectangle(0, 0, 32, 32), 2f, new Animation(new Rectangle(0, 0, 32, 32)), Color.White, Color.White, new ButtonFunctionality(null, null, null));


            this.seasonButtonPosition = new Vector2(this.width * .1f + 64, this.height * .05f + (64 * 5));
            this.weatherButtonPosition = new Vector2(this.width * .1f + (64 * 2), this.height * .05f + (64 * 5));
            this.timeButtonPosition = new Vector2(this.width * .1f + (64 * 3), this.height * .05f + (64 * 5));
            this.dayButtonPosition = new Vector2(this.width * .1f + (64 * 4), this.height * .05f + (64 * 5));
            this.locationButtonPosition = new Vector2(this.width * .1f + 64, this.height * .05f + (64 * 6));
            this.menuButtonPosition = new Vector2(this.width * .1f + (64 * 2), this.height * .05f + (64 * 6));
            this.eventButtonPosition = new Vector2(this.width * .1f + (64 * 3), this.height * .05f + (64 * 6));
            this.festivalButtonPosition = new Vector2(this.width * .1f + (64 * 4), this.height * .05f + (64 * 6));
        }

        /// <summary>
        /// What happens when the menu is clicked with the left mouse button.
        /// </summary>
        /// <param name="x">X position for the mouse.</param>
        /// <param name="y">Y position for the mouse.</param>
        /// <param name="playSound"></param>
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            bool buttonSelected = false;

            if (this.currentSelectedSong != null && this.currentMusicPackAlbum != null && this.playButton.containsPoint(x, y))
            {
                if (playSound) Game1.playSound("coin");
                this.playSong();
                return;
            }

            if (this.stopButton.containsPoint(x, y) && this.currentSelectedSong != null && this.currentMusicPackAlbum != null)
            {
                if (playSound) Game1.playSound("coin");
                this.stopSong();
                return;
            }

            if (this.drawMode == DrawMode.DifferentSelectionTypesModePage && this.saveButton.containsPoint(x, y))
            {
                if (playSound) Game1.playSound("coin");
                this.CurrentMusicPack.SaveSettings();
            }

            if (this.addButton.containsPoint(x, y) && this.drawMode == DrawMode.DifferentSelectionTypesModePage)
            {
                if (playSound) Game1.playSound("coin");
                this.addSong();
                return;
            }

            if (this.deleteButton.containsPoint(x, y) && this.drawMode == DrawMode.DifferentSelectionTypesModePage)
            {
                if (playSound) Game1.playSound("coin");
                this.clearAllOptions();
                return;
            }

            if (this.conditionalViewButton.containsPoint(x, y) && this.drawMode == DrawMode.DifferentSelectionTypesModePage)
            {
                if (playSound) Game1.playSound("coin");
                this.drawMode = DrawMode.ConditionalViewMode;
                this.updateFancyButtons();
                return;
            }

            if (this.nextPageButton.containsPoint(x, y))
            {
                this.nextPage();
            }
            if (this.lastPageButton.containsPoint(x, y))
            {
                this.previousPage();
            }

            //Delete season
            if (this.currentlySelectedSeason != null)
            {
                if (this.currentlySelectedSeason.containsPoint(x, y))
                {
                    this.currentlySelectedSeason = null;
                    if (playSound) Game1.playSound("coin");
                }
            }
            //Delete weather
            if (this.currentlySelectedWeather != null)
            {
                if (this.currentlySelectedWeather.containsPoint(x, y))
                {
                    this.currentlySelectedWeather = null;
                    if (playSound) Game1.playSound("coin");
                }
            }
            //Delete time
            if (this.currentlySelectedTime != null)
            {
                if (this.currentlySelectedTime.containsPoint(x, y))
                {
                    this.currentlySelectedTime = null;
                    if (playSound) Game1.playSound("coin");
                }
            }
            //Delete location
            if (this.currentlySelectedLocation != null)
            {
                if (this.currentlySelectedLocation.containsPoint(x, y))
                {
                    this.currentlySelectedLocation = null;
                    if (playSound) Game1.playSound("coin");
                }
            }
            //Delete day
            if (this.currentlySelectedDay != null)
            {
                if (this.currentlySelectedDay.containsPoint(x, y))
                {
                    this.currentlySelectedDay = null;
                    if (playSound) Game1.playSound("coin");
                }
            }
            //Delete festival
            if (this.currentlySelectedFestival != null)
            {
                if (this.currentlySelectedFestival.containsPoint(x, y))
                {
                    this.currentlySelectedFestival = null;
                    if (playSound) Game1.playSound("coin");
                }
            }
            //Delete event
            if (this.currentlySelectedEvent != null)
            {
                if (this.currentlySelectedEvent.containsPoint(x, y))
                {
                    this.currentlySelectedEvent = null;
                    if (playSound) Game1.playSound("coin");
                }
            }
            //Delete menu
            if (this.currentlySelectedMenu != null)
            {
                if (this.currentlySelectedMenu.containsPoint(x, y))
                {
                    this.currentlySelectedMenu = null;
                    Game1.playSound("coin");
                }
            }

            if (this.backButton.containsPoint(x, y))
            {
                if (playSound) Game1.playSound("coin");
                this.goBack();
                return;
            }

            if (this.drawMode == DrawMode.AlbumFancySelection)
            {
                int count = 0;
                Button ok = Button.Empty();
                foreach (var button in this.fancyButtons)
                {
                    if (button.containsPoint(x, y) && button.buttonFunctionality.leftClick != null)
                    {
                        if (playSound) Game1.playSound("coin");
                        button.onLeftClick();
                        this.currentAlbumIndex += count - 3;
                        while (this.currentAlbumIndex < 0)
                            this.currentAlbumIndex = (this.musicAlbumButtons.Count - (this.currentAlbumIndex * -1));
                        ok = button;
                    }
                    if (button.buttonFunctionality.leftClick != null)
                        count++;
                }
                this.selectAlbum(ok);
                this.updateFancyButtons();
                return;
            }

            if (this.drawMode == DrawMode.SongSelectionMode)
            {
                int amountToShow = 6;
                this.updateFancyButtons();

                int amount;
                if (0 + ((this.currentSongPageIndex + 1) * amountToShow) >= this.fancyButtons.Count)
                {
                    amount = (0 + ((this.currentSongPageIndex + 1) * (amountToShow)) - this.fancyButtons.Count);
                    amount = amountToShow - amount;
                    if (amount < 0) amount = 0;
                }
                else if (this.fancyButtons.Count < amountToShow)
                    amount = this.fancyButtons.Count;
                else
                    amount = amountToShow;

                if (amount == 0 && this.currentSongPageIndex > 1)
                    this.currentSongPageIndex--;

                var drawList = this.fancyButtons.GetRange(0 + (this.currentSongPageIndex * (amountToShow)), amount);

                bool songSelected = false;
                //Get a list of components to draw. And if I click one select the song.
                foreach (var component in drawList)
                {
                    if (component.containsPoint(x, y))
                    {
                        if (playSound) Game1.playSound("coin");
                        this.selectSong(component);
                        songSelected = true;
                    }
                }
                if (songSelected)
                    this.updateFancyButtons();
                return;
            }

            if (this.drawMode == DrawMode.DifferentSelectionTypesModePage)
            {
                foreach (var button in this.fancyButtons)
                {
                    if (button.containsPoint(x, y))
                    {
                        switch (button.name)
                        {
                            case "SeasonIcon":
                                //this.currentlySelectedOption = button.clone(position);
                                this.drawMode = DrawMode.SeasonSelection;
                                buttonSelected = true;
                                break;

                            case "FestivalIcon":
                                //this.currentlySelectedOption = button.clone(position);
                                this.drawMode = DrawMode.FestivalSelection;
                                buttonSelected = true;
                                break;

                            case "EventIcon":
                                //this.currentlySelectedOption = button.clone(position);
                                this.drawMode = DrawMode.EventSelection;
                                buttonSelected = true;
                                break;

                            case "MenuIcon":
                                //this.currentlySelectedOption = button.clone(position);
                                this.drawMode = DrawMode.MenuSelection;
                                buttonSelected = true;
                                break;

                            case "LocationButton":
                                //this.currentlySelectedOption = button.clone(position);
                                this.drawMode = DrawMode.LocationSelection;
                                buttonSelected = true;
                                break;
                            case "WeatherButton":
                                // this.currentlySelectedOption = button.clone(position);
                                this.drawMode = DrawMode.WeatherSelection;
                                buttonSelected = true;
                                break;

                            case "TimeButton":
                                //this.currentlySelectedOption = button.clone(position);
                                this.drawMode = DrawMode.TimeSelection;
                                buttonSelected = true;
                                break;

                            case "DayButton":
                                //this.currentlySelectedOption = button.clone(position);
                                this.drawMode = DrawMode.DaySelection;
                                buttonSelected = true;
                                break;
                        }
                    }
                }
                if (buttonSelected)
                {
                    if (playSound) Game1.playSound("coin");
                    this.updateFancyButtons();
                }
                return;
            }

            if (this.drawMode == DrawMode.SeasonSelection)
            {
                foreach (var button in this.fancyButtons)
                {
                    if (button.containsPoint(x, y))
                    {
                        Vector2 position = this.seasonButtonPosition;
                        switch (button.name)
                        {
                            case "SpringButton":
                                this.currentlySelectedSeason = button.clone(position, false);
                                this.drawMode = DrawMode.DifferentSelectionTypesModePage;
                                buttonSelected = true;
                                break;

                            case "SummerButton":
                                this.currentlySelectedSeason = button.clone(position, false);
                                this.drawMode = DrawMode.DifferentSelectionTypesModePage;
                                buttonSelected = true;
                                break;

                            case "FallButton":
                                this.currentlySelectedSeason = button.clone(position, false);
                                this.drawMode = DrawMode.DifferentSelectionTypesModePage;
                                buttonSelected = true;
                                break;

                            case "WinterButton":
                                this.currentlySelectedSeason = button.clone(position, false);
                                this.drawMode = DrawMode.DifferentSelectionTypesModePage;
                                buttonSelected = true;
                                break;
                        }
                    }
                }
                if (buttonSelected)
                {
                    if (playSound) Game1.playSound("coin");
                    this.updateFancyButtons();
                }
                return;
            }

            if (this.drawMode == DrawMode.FestivalSelection)
            {
                int amountToShow = 6;
                this.updateFancyButtons();

                int amount;
                if (0 + ((this.festivalPageIndex + 1) * amountToShow) >= this.fancyButtons.Count)
                {
                    amount = (0 + ((this.festivalPageIndex + 1) * (amountToShow)) - this.fancyButtons.Count);
                    amount = amountToShow - amount;
                    if (amount < 0) amount = 0;
                }
                else if (this.fancyButtons.Count < amountToShow)
                    amount = this.fancyButtons.Count;
                else
                    amount = amountToShow;

                if (amount == 0 && this.festivalPageIndex > 1)
                    this.festivalPageIndex--;

                var drawList = this.fancyButtons.GetRange(0 + (this.festivalPageIndex * (amountToShow)), amount);

                bool songSelected = false;
                //Get a list of components to draw. And if I click one select the song.
                foreach (var component in drawList)
                {
                    if (component.containsPoint(x, y))
                    {
                        Vector2 position = this.festivalButtonPosition;
                        this.currentlySelectedFestival = component.clone(position, false);
                        songSelected = true;
                        this.drawMode = DrawMode.DifferentSelectionTypesModePage;
                    }
                }
                if (songSelected)
                {
                    if (playSound) Game1.playSound("coin");
                    this.updateFancyButtons();
                }
                return;
            }

            if (this.drawMode == DrawMode.MenuSelection)
            {
                int amountToShow = 6;
                this.updateFancyButtons();

                int amount;
                if (0 + ((this.menuPageIndex + 1) * amountToShow) >= this.fancyButtons.Count)
                {
                    amount = (0 + ((this.menuPageIndex + 1) * (amountToShow)) - this.fancyButtons.Count);
                    amount = amountToShow - amount;
                    if (amount < 0) amount = 0;
                }
                else if (this.fancyButtons.Count < amountToShow)
                    amount = this.fancyButtons.Count;
                else
                    amount = amountToShow;

                if (amount == 0 && this.menuPageIndex > 1)
                    this.menuPageIndex--;

                var drawList = this.fancyButtons.GetRange(0 + (this.menuPageIndex * (amountToShow)), amount);

                bool songSelected = false;
                //Get a list of components to draw. And if I click one select the song.
                foreach (var component in drawList)
                {
                    if (component.containsPoint(x, y))
                    {
                        Vector2 position = this.menuButtonPosition;
                        this.currentlySelectedMenu = component.clone(position, false);
                        songSelected = true;
                        this.drawMode = DrawMode.DifferentSelectionTypesModePage;
                    }
                }
                if (songSelected)
                {
                    if (playSound) Game1.playSound("coin");
                    this.updateFancyButtons();
                }
                return;
            }

            if (this.drawMode == DrawMode.EventSelection)
            {
                int amountToShow = 6;
                this.updateFancyButtons();

                int amount;
                if (0 + ((this.eventPageIndex + 1) * amountToShow) >= this.fancyButtons.Count)
                {
                    amount = (0 + ((this.eventPageIndex + 1) * (amountToShow)) - this.fancyButtons.Count);
                    amount = amountToShow - amount;
                    if (amount < 0) amount = 0;
                }
                else if (this.fancyButtons.Count < amountToShow)
                    amount = this.fancyButtons.Count;
                else
                    amount = amountToShow;

                if (amount == 0 && this.eventPageIndex > 1)
                    this.eventPageIndex--;

                var drawList = this.fancyButtons.GetRange(0 + (this.eventPageIndex * (amountToShow)), amount);

                bool songSelected = false;
                //Get a list of components to draw. And if I click one select the song.
                foreach (var component in drawList)
                {
                    if (component.containsPoint(x, y))
                    {
                        Vector2 position = this.eventButtonPosition;
                        this.currentlySelectedEvent = component.clone(position, false);
                        songSelected = true;
                        this.drawMode = DrawMode.DifferentSelectionTypesModePage;
                    }
                }
                if (songSelected)
                {
                    if (playSound) Game1.playSound("coin");
                    this.updateFancyButtons();
                }
                return;
            }

            //Left click an option.
            if (this.drawMode == DrawMode.WeatherSelection)
            {
                foreach (var button in this.fancyButtons)
                {
                    if (button.containsPoint(x, y))
                    {
                        Vector2 position = this.weatherButtonPosition;
                        switch (button.name)
                        {
                            case "SunnyIcon":
                            case "RainyIcon":
                            case "SnowIcon":
                            case "WeatherDebrisIcon":
                            case "StormIcon":
                            case "WeatherFestivalIcon":
                            case "WeddingIcon":
                                this.currentlySelectedWeather = button.clone(position, false);
                                this.drawMode = DrawMode.DifferentSelectionTypesModePage;
                                buttonSelected = true;
                                break;
                        }
                    }
                }
                if (buttonSelected)
                {
                    if (playSound) Game1.playSound("coin");
                    this.updateFancyButtons();
                }
                return;
            }

            //Left click an option.
            if (this.drawMode == DrawMode.TimeSelection)
            {
                int amountToShow = 6;
                this.updateFancyButtons();

                int amount;
                if (0 + ((this.timePageIndex + 1) * amountToShow) >= this.fancyButtons.Count)
                {
                    amount = (0 + ((this.timePageIndex + 1) * (amountToShow)) - this.fancyButtons.Count);
                    amount = amountToShow - amount;
                    if (amount < 0) amount = 0;
                }
                else if (this.fancyButtons.Count < amountToShow)
                    amount = this.fancyButtons.Count;
                else
                    amount = amountToShow;

                if (amount == 0 && this.timePageIndex > 1)
                    this.timePageIndex--;

                var drawList = this.fancyButtons.GetRange(0 + (this.timePageIndex * (amountToShow)), amount);

                foreach (var button in drawList)
                {
                    if (button.containsPoint(x, y))
                    {
                        Vector2 position = this.timeButtonPosition;
                        MusicPackV2 musicPack = (MusicPackV2)this.currentMusicPackAlbum.buttonFunctionality.leftClick.paramaters[0];
                        musicPack.SongInformation.TimesOfDay.Contains(button.name);
                        if (button.name == "DayIcon" || button.name == "NightIcon" || musicPack.SongInformation.TimesOfDay.Contains(button.name))
                        {
                            this.currentlySelectedTime = button.clone(position, false);
                            this.drawMode = DrawMode.DifferentSelectionTypesModePage;
                            buttonSelected = true;
                        }

                    }
                }
                if (buttonSelected)
                {
                    if (playSound) Game1.playSound("coin");
                    this.updateFancyButtons();
                }
                return;
            }

            //Left click an option.
            if (this.drawMode == DrawMode.LocationSelection)
            {
                int amountToShow = 6;
                this.updateFancyButtons();

                int amount;
                if (0 + ((this.locationPageIndex + 1) * amountToShow) >= this.fancyButtons.Count)
                {
                    amount = (0 + ((this.locationPageIndex + 1) * (amountToShow)) - this.fancyButtons.Count);
                    amount = amountToShow - amount;
                    if (amount < 0) amount = 0;
                }
                else if (this.fancyButtons.Count < amountToShow)
                    amount = this.fancyButtons.Count;
                else
                    amount = amountToShow;

                if (amount == 0 && this.locationPageIndex > 1)
                    this.locationPageIndex--;

                var drawList = this.fancyButtons.GetRange(0 + (this.locationPageIndex * (amountToShow)), amount);


                bool songSelected = false;
                //Get a list of components to draw. And if I click one select the song.
                foreach (var component in drawList)
                {
                    if (component.containsPoint(x, y))
                    {
                        Vector2 position = this.locationButtonPosition;
                        this.currentlySelectedLocation = component.clone(position, false);
                        songSelected = true;
                        this.drawMode = DrawMode.DifferentSelectionTypesModePage;
                    }
                }
                if (songSelected)
                {
                    if (playSound) Game1.playSound("coin");
                    this.updateFancyButtons();
                }
                return;
            }

            if (this.drawMode == DrawMode.DaySelection)
            {
                foreach (var button in this.fancyButtons)
                {
                    if (button.containsPoint(x, y))
                    {
                        //if (button.name != "SundayIcon" || button.name != "MondayIcon" || button.name != "TuesdayIcon" || button.name != "WednesdayIcon" || button.name != "ThursdayIcon" || button.name != "FridayIcon" || button.name != "SaturdayIcon")
                        //    if (button == null) continue;
                        Vector2 position = this.dayButtonPosition;
                        //Get any valid location button.
                        this.currentlySelectedDay = button.clone(position, false);
                        this.drawMode = DrawMode.DifferentSelectionTypesModePage;
                        buttonSelected = true;
                    }
                }
                if (buttonSelected)
                {
                    if (playSound) Game1.playSound("coin");
                    this.updateFancyButtons();
                }
            }

            if (this.drawMode == DrawMode.ConditionalViewMode)
            {
                int amountToShow = 6;
                this.updateFancyButtons();

                int amount;
                if (0 + ((this.conditionalViewPageIndex + 1) * amountToShow) >= this.fancyButtons.Count)
                {
                    amount = (0 + ((this.conditionalViewPageIndex + 1) * (amountToShow)) - this.fancyButtons.Count);
                    amount = amountToShow - amount;
                    if (amount < 0) amount = 0;
                }
                else if (this.fancyButtons.Count < amountToShow)
                    amount = this.fancyButtons.Count;
                else
                    amount = amountToShow;

                if (amount == 0 && this.conditionalViewPageIndex > 1)
                    this.conditionalViewPageIndex--;

                var drawList = this.fancyButtons.GetRange(0 + (this.conditionalViewPageIndex * (amountToShow)), amount);

                bool songSelected = false;
                //Get a list of components to draw. And if I click one select the song.
                foreach (var component in drawList)
                {
                    if (component.containsPoint(x, y))
                    {
                        if (playSound) Game1.playSound("coin");
                        this.CurrentMusicPack.SongInformation.songs[this.currentSelectedSong.name].songConditionals.Remove(component.name);
                        songSelected = true;
                    }
                }
                if (songSelected)
                    this.updateFancyButtons();
                return;
            }
        }

        /// <summary>
        /// Goes to the next page depending on the current menu page being viewed.
        /// </summary>
        private void nextPage()
        {
            if (this.drawMode == DrawMode.AlbumFancySelection)
            {
                this.currentAlbumIndex++;
                if (this.currentAlbumIndex == this.musicAlbumButtons.Count) this.currentAlbumIndex = 0;
                this.updateFancyButtons();
                this.framesSinceLastUpdate = 0;
                Game1.playSound("shwip");
            }
            if (this.drawMode == DrawMode.EventSelection)
            {
                if ((this.eventPageIndex + 1) * 6 >= this.fancyButtons.Count) return;
                this.eventPageIndex++;
                this.updateFancyButtons();
                this.framesSinceLastUpdate = 0;
                Game1.playSound("shwip");
            }
            if (this.drawMode == DrawMode.FestivalSelection)
            {
                if ((this.festivalPageIndex + 1) * 6 >= this.fancyButtons.Count) return;
                this.festivalPageIndex++;
                this.updateFancyButtons();
                this.framesSinceLastUpdate = 0;
                Game1.playSound("shwip");
            }
            if (this.drawMode == DrawMode.LocationSelection)
            {
                if ((this.locationPageIndex + 1) * 6 >= this.fancyButtons.Count) return;
                this.locationPageIndex++;
                this.updateFancyButtons();
                this.framesSinceLastUpdate = 0;
                Game1.playSound("shwip");
            }
            if (this.drawMode == DrawMode.MenuSelection)
            {
                if ((this.menuPageIndex + 1) * 6 >= this.fancyButtons.Count) return;
                this.menuPageIndex++;
                this.updateFancyButtons();
                this.framesSinceLastUpdate = 0;
                Game1.playSound("shwip");
            }
            if (this.drawMode == DrawMode.SongSelectionMode)
            {
                if ((this.currentSongPageIndex + 1) * 6 >= this.fancyButtons.Count) return;
                this.currentSongPageIndex++;
                this.updateFancyButtons();
                this.framesSinceLastUpdate = 0;
                Game1.playSound("shwip");
            }
            if (this.drawMode == DrawMode.TimeSelection)
            {
                if ((this.timePageIndex + 1) * 6 >= this.fancyButtons.Count) return;
                this.timePageIndex++;
                this.updateFancyButtons();
                this.framesSinceLastUpdate = 0;
                Game1.playSound("shwip");
            }

            if (this.drawMode == DrawMode.ConditionalViewMode)
            {
                if ((this.conditionalViewPageIndex + 1) * 6 >= this.fancyButtons.Count) return;
                this.conditionalViewPageIndex++;
                this.updateFancyButtons();
                this.framesSinceLastUpdate = 0;
                Game1.playSound("shwip");
            }
        }

        /// <summary>
        /// Goes to the previous page depending on the current menu page being viewed.
        /// </summary>
        private void previousPage()
        {
            if (this.drawMode == DrawMode.AlbumFancySelection)
            {
                this.currentAlbumIndex--;
                if (this.currentAlbumIndex < 0) this.currentAlbumIndex = this.musicAlbumButtons.Count - 1;
                this.updateFancyButtons();
                this.framesSinceLastUpdate = 0;
                Game1.playSound("shwip");
            }
            if (this.drawMode == DrawMode.EventSelection)
            {
                if (this.eventPageIndex == 0) return;
                if (this.eventPageIndex > 0)
                    this.eventPageIndex--;
                this.updateFancyButtons();
                this.framesSinceLastUpdate = 0;
                Game1.playSound("shwip");
            }
            if (this.drawMode == DrawMode.FestivalSelection)
            {
                if (this.festivalPageIndex == 0) return;
                if (this.festivalPageIndex > 0)
                    this.festivalPageIndex--;
                this.updateFancyButtons();
                this.framesSinceLastUpdate = 0;
                Game1.playSound("shwip");
            }
            if (this.drawMode == DrawMode.LocationSelection)
            {
                if (this.locationPageIndex == 0) return;
                if (this.locationPageIndex > 0)
                    this.locationPageIndex--;
                this.updateFancyButtons();
                this.framesSinceLastUpdate = 0;
                Game1.playSound("shwip");
            }
            if (this.drawMode == DrawMode.MenuSelection)
            {
                if (this.menuPageIndex == 0) return;
                if (this.menuPageIndex > 0)
                    this.menuPageIndex--;
                this.updateFancyButtons();
                this.framesSinceLastUpdate = 0;
                Game1.playSound("shwip");
            }
            if (this.drawMode == DrawMode.SongSelectionMode)
            {
                if (this.currentSongPageIndex == 0) return;
                if (this.currentSongPageIndex > 0)
                    this.currentSongPageIndex--;
                this.updateFancyButtons();
                this.framesSinceLastUpdate = 0;
                Game1.playSound("shwip");
            }
            if (this.drawMode == DrawMode.TimeSelection)
            {
                if (this.timePageIndex == 0) return;
                if (this.timePageIndex > 0)
                    this.timePageIndex--;
                this.updateFancyButtons();
                this.framesSinceLastUpdate = 0;
                Game1.playSound("shwip");
            }
            if (this.drawMode == DrawMode.ConditionalViewMode)
            {
                if (this.conditionalViewPageIndex == 0) return;
                if (this.conditionalViewPageIndex > 0)
                    this.conditionalViewPageIndex--;
                this.updateFancyButtons();
                this.framesSinceLastUpdate = 0;
                Game1.playSound("shwip");
            }
        }

        /// <summary>
        /// What happens when hovering over a part of the menu.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public override void performHoverAction(int x, int y)
        {
            bool hoverTextOver = false;
            if (this.currentlySelectedSeason != null)
            {
                if (this.currentlySelectedSeason.containsPoint(x, y))
                {
                    this.hoverText = this.currentlySelectedSeason.label;
                    hoverTextOver = true;
                }
            }
            if (this.currentlySelectedTime != null)
            {
                if (this.currentlySelectedTime.containsPoint(x, y))
                {
                    this.hoverText = this.currentlySelectedTime.label;
                    hoverTextOver = true;
                }
            }

            if (this.currentlySelectedWeather != null)
            {
                if (this.currentlySelectedWeather.containsPoint(x, y))
                {
                    this.hoverText = this.currentlySelectedWeather.label;
                    hoverTextOver = true;
                }
            }
            if (this.currentlySelectedDay != null)
            {
                if (this.currentlySelectedDay.containsPoint(x, y))
                {
                    this.hoverText = this.currentlySelectedDay.label;
                    hoverTextOver = true;
                }
            }
            if (this.currentlySelectedLocation != null)
            {
                if (this.currentlySelectedLocation.containsPoint(x, y))
                {
                    this.hoverText = this.currentlySelectedLocation.label;
                    hoverTextOver = true;
                }
            }
            if (this.currentlySelectedFestival != null)
            {
                if (this.currentlySelectedFestival.containsPoint(x, y))
                {
                    this.hoverText = this.currentlySelectedFestival.label;
                    hoverTextOver = true;
                }
            }
            if (this.currentlySelectedEvent != null)
            {
                if (this.currentlySelectedEvent.containsPoint(x, y))
                {
                    this.hoverText = this.currentlySelectedEvent.label;
                    hoverTextOver = true;
                }
            }
            if (this.currentlySelectedMenu != null)
            {
                if (this.currentlySelectedMenu.containsPoint(x, y))
                {
                    this.hoverText = this.currentlySelectedMenu.label;
                    hoverTextOver = true;
                }
            }

            if (this.addButton.containsPoint(x, y) && this.drawMode == DrawMode.DifferentSelectionTypesModePage)
            {
                string retText = this.doesSongContainConditional(); //return text
                this.hoverText = string.IsNullOrEmpty(retText) ? "" : retText + Environment.NewLine;

                this.hoverText += "Add conditionals to song." + Environment.NewLine + Environment.NewLine + "This button adds a new set of conditionals" + Environment.NewLine + "for when this song should play." + Environment.NewLine + "Every time a conditional is added it is" + Environment.NewLine + "then checked every time this mod tries to select music." + Environment.NewLine + "Multiple conditionals can exist for the same song.";
                hoverTextOver = true;
            }
            if (this.saveButton.containsPoint(x, y) && this.drawMode == DrawMode.DifferentSelectionTypesModePage)
            {
                this.hoverText = "Save all changes for current music pack." + Environment.NewLine + Environment.NewLine + "Saves all song conditionals to the music pack's .json files" + Environment.NewLine + "so that way information isn't lost in " + Environment.NewLine + "case the game crashes before sleeping." + Environment.NewLine + "All music pack settings will be saved automatically" + Environment.NewLine + "when the day ends.";
                hoverTextOver = true;
            }

            if (this.deleteButton.containsPoint(x, y) && this.drawMode == DrawMode.DifferentSelectionTypesModePage)
            {
                this.hoverText = "Clears all currently selected options." + Environment.NewLine + "This does NOT delete other conditionals." + Environment.NewLine + "This just clears the current options for addition of multiple conditionals.";
                hoverTextOver = true;
            }

            if (this.conditionalViewButton.containsPoint(x, y) && this.drawMode == DrawMode.DifferentSelectionTypesModePage)
            {
                this.hoverText = "View all conditions for this song." + Environment.NewLine + "View all the possible conditions for when this song should play.";
                hoverTextOver = true;
            }

            if (this.playButton.containsPoint(x, y) && this.drawMode != DrawMode.SongSelectionMode && this.drawMode != DrawMode.AlbumFancySelection)
            {
                this.hoverText = "Play song";
                hoverTextOver = true;
            }
            if (this.stopButton.containsPoint(x, y) && this.drawMode != DrawMode.SongSelectionMode && this.drawMode != DrawMode.AlbumFancySelection)
            {
                this.hoverText = "Stop song";
                hoverTextOver = true;
            }

            if (this.backButton.containsPoint(x, y) && this.drawMode != DrawMode.AlbumFancySelection)
            {
                this.hoverText = "Go back";
                hoverTextOver = true;
            }

            //Display song conditional as hover text
            if (this.drawMode == DrawMode.ConditionalViewMode)
            {

                int amountToShow = 6;
                this.updateFancyButtons();

                int amount;
                if (0 + ((this.conditionalViewPageIndex + 1) * amountToShow) >= this.fancyButtons.Count)
                {
                    amount = (0 + ((this.conditionalViewPageIndex + 1) * (amountToShow)) - this.fancyButtons.Count);
                    amount = amountToShow - amount;
                    if (amount < 0) amount = 0;
                }
                else if (this.fancyButtons.Count < amountToShow)
                    amount = this.fancyButtons.Count;
                else
                    amount = amountToShow;

                if (amount == 0 && this.conditionalViewPageIndex > 1)
                    this.conditionalViewPageIndex--;

                var drawList = this.fancyButtons.GetRange(0 + (this.conditionalViewPageIndex * (amountToShow)), amount);

                bool songSelected = false;
                //Get a list of components to draw. And if I click one select the song.
                foreach (Button b in drawList)
                {
                    if (b.containsPoint(x, y))
                    {
                        StringBuilder builder = new StringBuilder();
                        builder.Append("Click to DELETE conditional:");
                        string[] splits = b.name.Split(SongConditionals.seperator);
                        foreach (string s in splits)
                        {
                            builder.Append(s);
                            builder.Append(Environment.NewLine);
                        }
                        builder.Append(Environment.NewLine);
                        this.hoverText = builder.ToString();
                        hoverTextOver = true;
                    }
                }
            }


            if (hoverTextOver == false)
            {
                this.hoverText = "";
            }
        }

        /// <summary>
        /// Checks if the conditional key has already been added in for the current music pack.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private string doesSongContainConditional()
        {
            string key = this.generateSongTriggerKeyFromSelection();
            if (this.CurrentMusicPack.SongInformation.songs[this.currentSelectedSong.name].songConditionals.ContainsKey(this.generateSongTriggerKeyFromSelection()))
            {
                return "Conditional key already added." + Environment.NewLine + "(That means clicking this will do nothing)";
            }
            else return "";
        }

        /// <summary>
        /// Update all of the buttons for the menu.
        /// </summary>
        public void updateFancyButtons()
        {
            //Album selection mode.
            if (this.drawMode == DrawMode.AlbumFancySelection)
            {
                this.fancyButtons.Clear();
                Vector4 placement = new Vector4((Game1.viewport.Width / 3), (Game1.viewport.Height / 4) + 128, this.width, this.height / 2);
                //generate buttons
                int offsetX = 200;
                if (this.musicAlbumButtons.Count > 0)
                {
                    for (int i = -3; i < 4; i++)
                    {
                        try
                        {
                            Button button = this.musicAlbumButtons.ElementAt(Math.Abs((this.currentAlbumIndex + i) % this.musicAlbumButtons.Count)).clone();
                            button.bounds = new Rectangle((int)placement.X + (i * 100) + offsetX, (int)placement.Y, 64, 64);
                            this.fancyButtons.Add(button);
                        }
                        catch
                        {
                            if (this.currentAlbumIndex + i == 0)
                            {
                                Button button = this.musicAlbumButtons.ElementAt(Math.Abs(0 % this.musicAlbumButtons.Count)).clone();
                                button.bounds = new Rectangle((int)placement.X + (i * 100) + offsetX, (int)placement.Y, 64, 64);
                                this.fancyButtons.Add(button);
                            }
                            else
                            {
                                try
                                {
                                    Button button = this.musicAlbumButtons.ElementAt(Math.Abs(((this.currentAlbumIndex + i) - this.musicAlbumButtons.Count) % this.musicAlbumButtons.Count)).clone();
                                    button.bounds = new Rectangle((int)placement.X + (i * 100) + offsetX, (int)placement.Y, 64, 64);
                                    this.fancyButtons.Add(button);
                                }
                                catch
                                {
                                    Button button = this.musicAlbumButtons.ElementAt(Math.Abs((this.currentAlbumIndex + i) + this.musicAlbumButtons.Count) % this.musicAlbumButtons.Count).clone();
                                    button.bounds = new Rectangle((int)placement.X + (i * 100) + offsetX, (int)placement.Y, 64, 64);
                                    this.fancyButtons.Add(button);
                                }
                            }
                        }
                    }
                    this.fancyButtons.Add(new Button("Outline", new Rectangle((int)placement.X + offsetX - 16, (int)placement.Y - 16, 64, 64), StardewSymphony.textureManager.getTexture("OutlineBox"), "", new Rectangle(0, 0, 16, 16), 6f, new Animation(new Rectangle(0, 0, 16, 16)), Color.White, Color.White, new ButtonFunctionality(null, null, new DelegatePairing(null, new List<object>())), false));
                    int count = 0;
                    foreach (var button in this.fancyButtons)
                    {

                        if (count == 3)
                        {
                            MusicPackV2 musicPack = (MusicPackV2)this.fancyButtons.ElementAt(count).buttonFunctionality.hover.paramaters[0];
                            this.texturedStrings.Clear();
                            this.texturedStrings.Add(SpriteFonts.vanillaFont.ParseString($"Current Album Name: {musicPack.Name}", new Vector2(button.bounds.X / 2, button.bounds.Y + 128), button.textColor));
                            button.hoverText = "";
                        }
                        count++;
                    }
                }
            }

            //Song selection mode.
            if (this.drawMode == DrawMode.SongSelectionMode)
            {
                this.fancyButtons.Clear();
                //Vector4 placement = new Vector4((Game1.viewport.Width / 3), (Game1.viewport.Height / 4) + 128, this.width, this.height / 2);
                MusicPackV2 musicPack = (MusicPackV2)this.currentMusicPackAlbum.buttonFunctionality.leftClick.paramaters[0];


                Vector4 placement2 = new Vector4(this.width * .2f + 400, this.height * .05f, 5 * 100, this.height * .9f);
                for (int i = 0; i < musicPack.SongInformation.songs.Count; i++)
                {
                    //Allow 8 songs to be displayed per page.
                    Texture2DExtended texture = StardewSymphony.textureManager.getTexture("MusicNote");
                    float scale = 1.00f / (texture.getTexture().Width / 64f);
                    string songName = musicPack.SongInformation.songs.ElementAt(i).Key;
                    Rectangle srcRect = new Rectangle(0, 0, texture.getTexture().Width, texture.getTexture().Height);
                    this.fancyButtons.Add(new Button(songName, new Rectangle((int)placement2.X + 25, (int)placement2.Y + ((i % 6) * 100) + 100, 64, 64), texture, songName, srcRect, scale, new Animation(srcRect), Color.White, Color.White, new ButtonFunctionality(null, null, null)));
                }
            }

            if (this.drawMode == DrawMode.DifferentSelectionTypesModePage)
            {
                this.fancyButtons.Clear();

                int buttonXPosition = 450;

                //Season Icon placement.
                Vector4 seasonPlacement = new Vector4(this.width * .2f + buttonXPosition, this.getFixedPositionFromMenu(0, 64 * 1).Y, 5 * 100, this.height * .9f);
                switch (Game1.currentSeason)
                {
                    case "spring":
                        {
                            Texture2DExtended springTexture = StardewSymphony.textureManager.getTexture("SpringIcon");
                            if (springTexture == null)
                            {
                                if (StardewSymphony.Config.EnableDebugLog)
                                    StardewSymphony.ModMonitor.Log("SPRING TEXTURE NULL!");
                                return;
                            }
                            float scale = 1.00f / (springTexture.getTexture().Width / 64f);
                            Rectangle srcRect = new Rectangle(0, 0, springTexture.getTexture().Width, springTexture.getTexture().Height);
                            this.fancyButtons.Add(new Button("SeasonIcon", new Rectangle((int)seasonPlacement.X, (int)seasonPlacement.Y, 64, 64), springTexture, "Seasonal Music", srcRect, scale, new Animation(srcRect), Color.White, Color.White, new ButtonFunctionality(null, null, null)));
                        }
                        break;

                    case "summer":
                        {
                            Texture2DExtended summerTexture = StardewSymphony.textureManager.getTexture("SummerIcon");
                            if (summerTexture == null)
                            {
                                if (StardewSymphony.Config.EnableDebugLog)
                                    StardewSymphony.ModMonitor.Log("SUMMER TEXTURE NULL!");
                                return;
                            }
                            float scale = 1.00f / (summerTexture.getTexture().Width / 64f);
                            Rectangle srcRect = new Rectangle(0, 0, summerTexture.getTexture().Width, summerTexture.getTexture().Height);
                            this.fancyButtons.Add(new Button("SeasonIcon", new Rectangle((int)seasonPlacement.X, (int)seasonPlacement.Y, 64, 64), summerTexture, "Seasonal Music", srcRect, scale, new Animation(srcRect), Color.White, Color.White, new ButtonFunctionality(null, null, null)));
                        }
                        break;

                    case "fall":
                        {
                            Texture2DExtended fallTexture = StardewSymphony.textureManager.getTexture("FallIcon");
                            if (fallTexture == null)
                            {
                                if (StardewSymphony.Config.EnableDebugLog)
                                    StardewSymphony.ModMonitor.Log("FALL TEXTURE NULL!");
                                return;
                            }
                            float scale = 1.00f / (fallTexture.getTexture().Width / 64f);
                            Rectangle srcRect = new Rectangle(0, 0, fallTexture.getTexture().Width, fallTexture.getTexture().Height);
                            this.fancyButtons.Add(new Button("SeasonIcon", new Rectangle((int)seasonPlacement.X, (int)seasonPlacement.Y, 64, 64), fallTexture, "Seasonal Music", srcRect, scale, new Animation(srcRect), Color.White, Color.White, new ButtonFunctionality(null, null, null)));
                        }
                        break;

                    case "winter":
                        {
                            Texture2DExtended winterTexture = StardewSymphony.textureManager.getTexture("WinterIcon");
                            if (winterTexture == null)
                            {
                                if (StardewSymphony.Config.EnableDebugLog)
                                    StardewSymphony.ModMonitor.Log("WINTER TEXTURE NULL!");
                                return;
                            }
                            float scale = 1.00f / (winterTexture.getTexture().Width / 64f);
                            Rectangle srcRect = new Rectangle(0, 0, winterTexture.getTexture().Width, winterTexture.getTexture().Height);
                            this.fancyButtons.Add(new Button("SeasonIcon", new Rectangle((int)seasonPlacement.X, (int)seasonPlacement.Y, 64, 64), winterTexture, "Seasonal Music", srcRect, scale, new Animation(srcRect), Color.White, Color.White, new ButtonFunctionality(null, null, null)));
                        }
                        break;
                }

                Vector4 festivalPlacement = new Vector4(this.width * .2f + buttonXPosition, this.getFixedPositionFromMenu(0, 64 * 2 + 16).Y, 6 * 100, this.height * .9f);
                Vector4 eventPlacement = new Vector4(this.width * .2f + buttonXPosition, this.getFixedPositionFromMenu(0, 64 * 3 + (16 * 2)).Y, 7 * 100, this.height * .9f);
                Vector4 menuPlacement = new Vector4(this.width * .2f + buttonXPosition, this.getFixedPositionFromMenu(0, 64 * 4 + (16 * 3)).Y, 8 * 100, this.height * .9f);
                Vector4 locationPlacement = new Vector4(this.width * .2f + buttonXPosition, this.getFixedPositionFromMenu(0, 64 * 5 + (16 * 4)).Y, 9 * 100, this.height * .9f);
                Vector4 weatherPlacement = new Vector4(this.width * .2f + buttonXPosition, this.getFixedPositionFromMenu(0, 64 * 6 + (16 * 5)).Y, 9 * 100, this.height * .9f);
                Vector4 timePlacement = new Vector4(this.width * .2f + buttonXPosition, this.getFixedPositionFromMenu(0, 64 * 7 + (16 * 6)).Y, 9 * 100, this.height * .9f);
                Vector4 dayPlacement = new Vector4(this.width * .2f + buttonXPosition, this.getFixedPositionFromMenu(0, 64 * 8 + (16 * 7)).Y, 9 * 100, this.height * .9f);

                //Festival Icon placement.
                Texture2DExtended festivalTexture = StardewSymphony.textureManager.getTexture("FestivalIcon");
                float festivalScale = 1.00f / (festivalTexture.getTexture().Width / 64f);
                Rectangle festivalSrcRect = new Rectangle(0, 0, festivalTexture.getTexture().Width, festivalTexture.getTexture().Height);
                this.fancyButtons.Add(new Button("FestivalIcon", new Rectangle((int)festivalPlacement.X, (int)festivalPlacement.Y, 64, 64), festivalTexture, "Festival Music", festivalSrcRect, festivalScale, new Animation(festivalSrcRect), Color.White, Color.White, new ButtonFunctionality(null, null, null)));

                //Event Icon placement.
                Texture2DExtended eventTexture = StardewSymphony.textureManager.getTexture("EventIcon");
                float eventScale = 1.00f / (eventTexture.getTexture().Width / 64f);
                Rectangle eventSrcRectangle = new Rectangle(0, 0, eventTexture.getTexture().Width, eventTexture.getTexture().Height);
                this.fancyButtons.Add(new Button("EventIcon", new Rectangle((int)eventPlacement.X, (int)eventPlacement.Y, 64, 64), eventTexture, "Event Music", eventSrcRectangle, eventScale, new Animation(eventSrcRectangle), Color.White, Color.White, new ButtonFunctionality(null, null, null)));

                //Menu Icon placement.
                Texture2DExtended menuTexture = StardewSymphony.textureManager.getTexture("MenuIcon");
                float menuScale = 1.00f / (menuTexture.getTexture().Width / 64f);
                Rectangle menuSrcRectangle = new Rectangle(0, 0, menuTexture.getTexture().Width, menuTexture.getTexture().Height);
                this.fancyButtons.Add(new Button("MenuIcon", new Rectangle((int)menuPlacement.X, (int)menuPlacement.Y, 64, 64), menuTexture, "Menu Music", menuSrcRectangle, menuScale, new Animation(menuSrcRectangle), Color.White, Color.White, new ButtonFunctionality(null, null, null)));

                //Location Icon placement.
                Texture2DExtended locationTexture = StardewSymphony.textureManager.getTexture("HouseIcon");
                float locationScale = 1.00f / (locationTexture.getTexture().Width / 64f);
                Rectangle locationRect = new Rectangle(0, 0, locationTexture.getTexture().Width, locationTexture.getTexture().Height);
                this.fancyButtons.Add(new Button("LocationButton", new Rectangle((int)locationPlacement.X, (int)locationPlacement.Y, 64, 64), locationTexture, "Location Music", locationRect, locationScale, new Animation(locationRect), Color.White, Color.White, new ButtonFunctionality(null, null, null)));

                Texture2DExtended weatherTexture = StardewSymphony.textureManager.getTexture("WeatherIcon");
                float weatherScale = 1.00f / (weatherTexture.getTexture().Width / 64f);
                Rectangle weatherRect = new Rectangle(0, 0, weatherTexture.getTexture().Width, weatherTexture.getTexture().Height);
                this.fancyButtons.Add(new Button("WeatherButton", new Rectangle((int)weatherPlacement.X, (int)weatherPlacement.Y, 64, 64), weatherTexture, "Weather Music", weatherRect, weatherScale, new Animation(weatherRect), Color.White, Color.White, new ButtonFunctionality(null, null, null)));

                Texture2DExtended timeTexture = StardewSymphony.textureManager.getTexture("DayNightIcon");
                float timeScale = 1.00f / (timeTexture.getTexture().Width / 64f);
                Rectangle timeRect = new Rectangle(0, 0, timeTexture.getTexture().Width, timeTexture.getTexture().Height);
                this.fancyButtons.Add(new Button("TimeButton", new Rectangle((int)timePlacement.X, (int)timePlacement.Y, 64, 64), timeTexture, "Time Of Day Music", timeRect, timeScale, new Animation(timeRect), Color.White, Color.White, new ButtonFunctionality(null, null, null)));

                Texture2DExtended dayTexture = null;
                if (Game1.dayOfMonth % 7 == 0)
                {
                    //Sunday
                    dayTexture = StardewSymphony.textureManager.getTexture("CalendarSunday");

                }
                if (Game1.dayOfMonth % 7 == 1)
                {
                    //Monday
                    dayTexture = StardewSymphony.textureManager.getTexture("CalendarMonday");
                }
                if (Game1.dayOfMonth % 7 == 2)
                {
                    //Tuesday
                    dayTexture = StardewSymphony.textureManager.getTexture("CalendarTuesday");
                }
                if (Game1.dayOfMonth % 7 == 3)
                {
                    //Wednesday
                    dayTexture = StardewSymphony.textureManager.getTexture("CalendarWednesday");
                }
                if (Game1.dayOfMonth % 7 == 4)
                {
                    //Thursday
                    dayTexture = StardewSymphony.textureManager.getTexture("CalendarThursday");
                }
                if (Game1.dayOfMonth % 7 == 5)
                {
                    //Friday
                    dayTexture = StardewSymphony.textureManager.getTexture("CalendarFriday");
                }
                if (Game1.dayOfMonth % 7 == 6)
                {
                    //Saturday
                    dayTexture = StardewSymphony.textureManager.getTexture("CalendarSaturday");
                }
                float dayScale = 1.00f / (dayTexture.getTexture().Width / 64f);
                Rectangle dayRect = new Rectangle(0, 0, dayTexture.getTexture().Width, dayTexture.getTexture().Height);
                this.fancyButtons.Add(new Button("DayButton", new Rectangle((int)dayPlacement.X, (int)dayPlacement.Y, 64, 64), dayTexture, "Day of Week Music", dayRect, dayScale, new Animation(dayRect), Color.White, Color.White, new ButtonFunctionality(null, null, null)));
            }

            if (this.drawMode == DrawMode.SeasonSelection)
            {
                this.fancyButtons.Clear();

                int buttonXPosition = 450;

                //Season Icon placement.
                Vector4 springPlacement = new Vector4(this.width * .2f + buttonXPosition, this.height * .25f, 5 * 100, this.height * .9f);
                Vector4 summerPlacement = new Vector4(this.width * .2f + buttonXPosition, this.height * .35f, 6 * 100, this.height * .9f);
                Vector4 fallPlacement = new Vector4(this.width * .2f + buttonXPosition, this.height * .45f, 7 * 100, this.height * .9f);
                Vector4 winterPlacement = new Vector4(this.width * .2f + buttonXPosition, this.height * .55f, 8 * 100, this.height * .9f);

                Texture2DExtended springTexture = StardewSymphony.textureManager.getTexture("SpringIcon");
                if (springTexture == null)
                {
                    if (StardewSymphony.Config.EnableDebugLog)
                        StardewSymphony.ModMonitor.Log("SPRING TEXTURE NULL!");
                    return;
                }
                float scale = 1.00f / (springTexture.getTexture().Width / 64f);
                Rectangle srcRect = new Rectangle(0, 0, springTexture.getTexture().Width, springTexture.getTexture().Height);
                this.fancyButtons.Add(new Button("SpringButton", new Rectangle((int)springPlacement.X, (int)springPlacement.Y, 64, 64), springTexture, "Spring Music", srcRect, scale, new Animation(srcRect), Color.White, Color.White, new ButtonFunctionality(null, null, null)));

                //Summer Icon placement.
                Texture2DExtended festivalTexture = StardewSymphony.textureManager.getTexture("SummerIcon");
                float festivalScale = 1.00f / (festivalTexture.getTexture().Width / 64f);
                Rectangle festivalSrcRect = new Rectangle(0, 0, festivalTexture.getTexture().Width, festivalTexture.getTexture().Height);
                this.fancyButtons.Add(new Button("SummerButton", new Rectangle((int)summerPlacement.X, (int)summerPlacement.Y, 64, 64), festivalTexture, "Summer Music", festivalSrcRect, festivalScale, new Animation(festivalSrcRect), Color.White, Color.White, new ButtonFunctionality(null, null, null)));

                //Fall Icon placement.
                Texture2DExtended eventTexture = StardewSymphony.textureManager.getTexture("FallIcon");
                float eventScale = 1.00f / (eventTexture.getTexture().Width / 64f);
                Rectangle eventSrcRectangle = new Rectangle(0, 0, eventTexture.getTexture().Width, eventTexture.getTexture().Height);
                this.fancyButtons.Add(new Button("FallButton", new Rectangle((int)fallPlacement.X, (int)fallPlacement.Y, 64, 64), eventTexture, "Fall Music", eventSrcRectangle, eventScale, new Animation(eventSrcRectangle), Color.White, Color.White, new ButtonFunctionality(null, null, null)));

                //Winter Icon placement.
                Texture2DExtended menuTexture = StardewSymphony.textureManager.getTexture("WinterIcon");
                float menuScale = 1.00f / (menuTexture.getTexture().Width / 64f);
                Rectangle menuSrcRectangle = new Rectangle(0, 0, menuTexture.getTexture().Width, menuTexture.getTexture().Height);
                this.fancyButtons.Add(new Button("WinterButton", new Rectangle((int)winterPlacement.X, (int)winterPlacement.Y, 64, 64), menuTexture, "Winter Music", menuSrcRectangle, menuScale, new Animation(menuSrcRectangle), Color.White, Color.White, new ButtonFunctionality(null, null, null)));
            }

            //Festival selection mode.
            if (this.drawMode == DrawMode.FestivalSelection)
            {
                this.fancyButtons.Clear();
                //Vector4 placement = new Vector4((Game1.viewport.Width / 3), (Game1.viewport.Height / 4) + 128, this.width, this.height / 2);

                Vector4 placement2 = new Vector4(this.width * .2f + 400, this.height * .05f, 5 * 100, this.height * .9f);
                for (int i = 0; i < SongSpecificsV2.festivals.Count; i++)
                {
                    //Allow 8 songs to be displayed per page.
                    Texture2DExtended texture = StardewSymphony.textureManager.getTexture("GreenBallon");
                    float scale = 1.00f / (texture.getTexture().Height / 64f);
                    Rectangle srcRect = new Rectangle(0, 0, texture.getTexture().Width, texture.getTexture().Height);
                    this.fancyButtons.Add(new Button(SongSpecificsV2.festivals.ElementAt(i), new Rectangle((int)placement2.X + 50, (int)placement2.Y + ((i % 6) * 100) + 100, 64, 64), texture, SongSpecificsV2.festivals.ElementAt(i), srcRect, scale, new Animation(srcRect), Color.White, Color.White, new ButtonFunctionality(null, null, null)));
                }
            }

            //Menu selection mode.
            if (this.drawMode == DrawMode.MenuSelection)
            {
                this.fancyButtons.Clear();
                //Vector4 placement = new Vector4((Game1.viewport.Width / 3), (Game1.viewport.Height / 4) + 128, this.width, this.height / 2);

                Vector4 placement2 = new Vector4(this.width * .2f + 400, this.height * .05f, 5 * 100, this.height * .9f);
                for (int i = 0; i < SongSpecificsV2.menus.Count; i++)
                {

                    //Allow 8 songs to be displayed per page.
                    Texture2DExtended texture = StardewSymphony.textureManager.getTexture("MenuIcon");
                    float scale = 1.00f / (texture.getTexture().Width / 64f);
                    Rectangle srcRect = new Rectangle(0, 0, texture.getTexture().Width, texture.getTexture().Height);
                    this.fancyButtons.Add(new Button(SongSpecificsV2.menus.ElementAt(i), new Rectangle((int)placement2.X + 50, (int)placement2.Y + ((i % 6) * 100) + 100, 64, 64), texture, SongSpecificsV2.menus.ElementAt(i), srcRect, scale, new Animation(srcRect), Color.White, Color.White, new ButtonFunctionality(null, null, null)));
                }
            }

            //Event selection mode.
            if (this.drawMode == DrawMode.EventSelection)
            {
                this.fancyButtons.Clear();
                //Vector4 placement = new Vector4((Game1.viewport.Width / 3), (Game1.viewport.Height / 4) + 128, this.width, this.height / 2);

                Vector4 placement2 = new Vector4(this.width * .2f + 400, this.height * .05f, 5 * 100, this.height * .9f);
                for (int i = 0; i < SongSpecificsV2.events.Count; i++)
                {

                    //Allow 8 songs to be displayed per page.
                    Texture2DExtended texture = StardewSymphony.textureManager.getTexture("StarIcon");
                    float scale = 1.00f / (texture.getTexture().Width / 64f);
                    Rectangle srcRect = new Rectangle(0, 0, texture.getTexture().Width, texture.getTexture().Height);
                    this.fancyButtons.Add(new Button(SongSpecificsV2.events.ElementAt(i), new Rectangle((int)placement2.X + 50, (int)placement2.Y + ((i % 6) * 100) + 100, 64, 64), texture, SongSpecificsV2.events.ElementAt(i), srcRect, scale, new Animation(srcRect), Color.White, Color.White, new ButtonFunctionality(null, null, null)));
                }
            }

            if (this.drawMode == DrawMode.WeatherSelection)
            {
                this.fancyButtons.Clear();

                Vector4 placement2 = new Vector4(this.width * .2f + 400, this.height * .05f, 5 * 100, this.height * .9f);
                for (int i = 0; i < 7; i++)
                {
                    //Allow 8 songs to be displayed per page.
                    Texture2DExtended texture = new Texture2DExtended();
                    string name = "";
                    string display = "";
                    if (i == 0)
                    {
                        texture = StardewSymphony.textureManager.getTexture("SunnyIcon");
                        name = "SunnyIcon";
                        display = "Sunny Music";
                    }
                    if (i == 1)
                    {
                        texture = StardewSymphony.textureManager.getTexture("RainyIcon");
                        name = "RainyIcon";
                        display = "Rainy Music";
                    }
                    if (i == 2)
                    {
                        if (Game1.currentSeason == "spring")
                        {
                            texture = StardewSymphony.textureManager.getTexture("DebrisSpringIcon");
                            name = "WeatherDebrisIcon";
                            display = "Debris Music";
                        }
                        if (Game1.currentSeason == "summer")
                        {
                            texture = StardewSymphony.textureManager.getTexture("DebrisSummerIcon");
                            name = "WeatherDebrisIcon";
                            display = "Debris Music";
                        }
                        if (Game1.currentSeason == "fall")
                        {
                            texture = StardewSymphony.textureManager.getTexture("DebrisFallIcon");
                            name = "WeatherDebrisIcon";
                            display = "Debris Music";
                        }
                        if (Game1.currentSeason == "winter")
                        {
                            texture = StardewSymphony.textureManager.getTexture("DebrisSpringIcon");
                            name = "WeatherDebrisIcon";
                            display = "Debris Music";
                        }
                    }
                    if (i == 3)
                    {
                        texture = StardewSymphony.textureManager.getTexture("SnowIcon");
                        name = "SnowIcon";
                        display = "Snow Music";
                    }
                    if (i == 4)
                    {
                        texture = StardewSymphony.textureManager.getTexture("StormIcon");
                        name = "StormIcon";
                        display = "Storm Music";
                    }
                    if (i == 5)
                    {
                        texture = StardewSymphony.textureManager.getTexture("WeatherFestivalIcon");
                        name = "WeatherFestivalIcon";
                        display = "Festival Day Music";
                    }
                    if (i == 6)
                    {
                        texture = StardewSymphony.textureManager.getTexture("WeddingIcon");
                        name = "WeddingIcon";
                        display = "Wedding Music";
                    }

                    float scale = 1.00f / (texture.getTexture().Width / 64f);
                    Rectangle srcRect = new Rectangle(0, 0, texture.getTexture().Width, texture.getTexture().Height);
                    this.fancyButtons.Add(new Button(name, new Rectangle((int)placement2.X + 50, (int)placement2.Y + ((i % 7) * 100), 64, 64), texture, display, srcRect, scale, new Animation(srcRect), Color.White, Color.White, new ButtonFunctionality(null, null, null)));

                }
            }

            if (this.drawMode == DrawMode.TimeSelection)
            {
                this.fancyButtons.Clear();

                Vector4 placement2 = new Vector4(this.width * .2f + 400, this.height * .05f, 5 * 300, this.height * .9f);
                for (int i = 0; i < 2; i++)
                {
                    //Allow 8 songs to be displayed per page.
                    Texture2DExtended texture = new Texture2DExtended();
                    string name = "";
                    string display = "";
                    if (i == 0)
                    {
                        texture = StardewSymphony.textureManager.getTexture("DayIcon");
                        name = "DayIcon";
                        display = "Day Music";
                    }
                    if (i == 1)
                    {
                        texture = StardewSymphony.textureManager.getTexture("NightIcon");
                        name = "NightIcon";
                        display = "Night Music";
                    }

                    if (texture == null)
                    {
                        StardewSymphony.ModMonitor.Log("HMM A TEXTURE IS NULL: " + i.ToString());
                    }
                    float scale = 1.00f / (texture.getTexture().Width / 64f);
                    Rectangle srcRect = new Rectangle(0, 0, texture.getTexture().Width, texture.getTexture().Height);
                    this.fancyButtons.Add(new Button(name, new Rectangle((int)placement2.X + 50, (int)placement2.Y + ((i % 6) * 100) + 100, 64, 64), texture, display, srcRect, scale, new Animation(srcRect), Color.White, Color.White, new ButtonFunctionality(null, null, null)));
                }

                for (int i = 2; i < 26; i++)
                {
                    //Allow 8 songs to be displayed per page.
                    Texture2DExtended texture = new Texture2DExtended();
                    string name = "";
                    string display = "";
                    if (i - 2 < 12)
                    {
                        if ((i - 2) == 0)
                        {
                            texture = StardewSymphony.textureManager.getTexture("12" + "AM");
                            name = "12A.M.";
                            display = "12 A.M.";
                        }
                        else
                        {

                            texture = StardewSymphony.textureManager.getTexture((i - 2).ToString() + "AM");
                            name = (i - 2).ToString() + "A.M.";
                            display = (i - 2).ToString() + " A.M.";
                        }
                    }
                    if (i - 2 >= 12)
                    {
                        if ((i - 2) == 12)
                        {
                            texture = StardewSymphony.textureManager.getTexture("12" + "PM");
                            name = "12P.M.";
                            display = "12 P.M.";
                        }
                        else
                        {
                            texture = StardewSymphony.textureManager.getTexture(((i - 2) % 12).ToString() + "PM");
                            name = ((i - 2) % 12).ToString() + "P.M.";
                            display = ((i - 2) % 12).ToString() + " P.M.";
                        }
                    }


                    if (texture == null)
                    {
                        StardewSymphony.ModMonitor.Log("HMM A TEXTURE IS NULL: " + i.ToString());
                    }
                    float scale = 1.00f / (texture.getTexture().Width / 64f);
                    Rectangle srcRect = new Rectangle(0, 0, texture.getTexture().Width, texture.getTexture().Height);
                    this.fancyButtons.Add(new Button(name, new Rectangle((int)placement2.X + 50, (int)placement2.Y + ((i % 6) * 100) + 100, 64, 64), texture, display, srcRect, scale, new Animation(srcRect), Color.White, Color.White, new ButtonFunctionality(null, null, null)));
                }

            }

            //Location selection mode.
            if (this.drawMode == DrawMode.LocationSelection)
            {
                this.fancyButtons.Clear();
                int numOfEmptyCabin = 1;
                Vector4 placement2 = new Vector4(this.width * .2f + 400, this.height * .05f, 5 * 100, this.height * .9f);
                for (int i = 0; i < SongSpecificsV2.locations.Count; i++)
                {
                    string locName = SongSpecificsV2.locations.ElementAt(i);

                    if (locName.Contains("Cabin"))
                    {
                        try
                        {
                            GameLocation loc = Game1.getLocationFromName(locName);
                            Farmer farmer = (loc as Cabin).getFarmhand().Value;
                            if (farmer != null)
                            {

                                string displayName = farmer.Name + "'s Cabin";
                                if (farmer.Name == "")
                                {
                                    displayName = "Empty Cabin " + (numOfEmptyCabin);
                                    numOfEmptyCabin++;
                                }
                                Texture2DExtended texture2 = StardewSymphony.textureManager.getTexture("HouseIcon");
                                float scale2 = 1.00f / (texture2.getTexture().Width / 64f);
                                Rectangle srcRect2 = new Rectangle(0, 0, texture2.getTexture().Width, texture2.getTexture().Height);
                                this.fancyButtons.Add(new Button(locName, new Rectangle((int)placement2.X + 25, (int)placement2.Y + ((i % 6) * 100) + 100, 64, 64), texture2, displayName, srcRect2, scale2, new Animation(srcRect2), Color.White, Color.White, new ButtonFunctionality(null, null, null)));

                                continue;
                            }
                            if (farmer == null)
                            {
                                string displayName = "Empty Cabin " + (numOfEmptyCabin);
                                numOfEmptyCabin++;
                                Texture2DExtended texture2 = StardewSymphony.textureManager.getTexture("HouseIcon");
                                float scale2 = 1.00f / (texture2.getTexture().Width / 64f);
                                Rectangle srcRect2 = new Rectangle(0, 0, texture2.getTexture().Width, texture2.getTexture().Height);
                                this.fancyButtons.Add(new Button(locName, new Rectangle((int)placement2.X + 25, (int)placement2.Y + ((i % 6) * 100) + 100, 64, 64), texture2, displayName, srcRect2, scale2, new Animation(srcRect2), Color.White, Color.White, new ButtonFunctionality(null, null, null)));
                                continue;
                            }
                        }
                        catch { }
                    }

                    Texture2DExtended locTexture = StardewSymphony.textureManager.getTexture(locName,false);
                    if (locTexture != null)
                    {
                        float scale = 1.00f / (locTexture.getTexture().Width / 64f);
                        Rectangle srcRect = new Rectangle(0, 0, locTexture.getTexture().Width, locTexture.getTexture().Height);
                        this.fancyButtons.Add(new Button(locName, new Rectangle((int)placement2.X + 25, (int)placement2.Y + ((i % 6) * 100) + 100, 64, 64), locTexture, locName, srcRect, scale, new Animation(srcRect), Color.White, Color.White, new ButtonFunctionality(null, null, null)));
                    }
                    else
                    {
                        //Allow 8 songs to be displayed per page.
                        Texture2DExtended texture = StardewSymphony.textureManager.getTexture("HouseIcon");
                        float scale = 1.00f / (texture.getTexture().Width / 64f);
                        Rectangle srcRect = new Rectangle(0, 0, texture.getTexture().Width, texture.getTexture().Height);
                        this.fancyButtons.Add(new Button(locName, new Rectangle((int)placement2.X + 25, (int)placement2.Y + ((i % 6) * 100) + 100, 64, 64), texture, locName, srcRect, scale, new Animation(srcRect), Color.White, Color.White, new ButtonFunctionality(null, null, null)));
                    }
                }
            }

            if (this.drawMode == DrawMode.DaySelection)
            {
                this.fancyButtons.Clear();

                Vector4 placement2 = new Vector4(this.width * .2f + 400, this.height * .05f, 5 * 100, this.height * .9f);
                for (int i = 0; i < 7; i++)
                {
                    //Allow 8 songs to be displayed per page.
                    Texture2DExtended texture = new Texture2DExtended();
                    string name = "";
                    string display = "";
                    if (i == 0)
                    {
                        texture = StardewSymphony.textureManager.getTexture("SundayIcon");
                        name = "SundayIcon";
                        display = "Sunday Music";
                    }
                    if (i == 1)
                    {
                        texture = StardewSymphony.textureManager.getTexture("MondayIcon");
                        name = "MondayIcon";
                        display = "Monday Music";
                    }
                    if (i == 2)
                    {
                        texture = StardewSymphony.textureManager.getTexture("TuesdayIcon");
                        name = "TuesdayIcon";
                        display = "Tuesday Music";

                    }
                    if (i == 3)
                    {
                        texture = StardewSymphony.textureManager.getTexture("WednesdayIcon");
                        name = "WednesdayIcon";
                        display = "Wednesday Music";
                    }
                    if (i == 4)
                    {
                        texture = StardewSymphony.textureManager.getTexture("ThursdayIcon");
                        name = "ThursdayIcon";
                        display = "Thursday Music";
                    }
                    if (i == 5)
                    {
                        texture = StardewSymphony.textureManager.getTexture("FridayIcon");
                        name = "FridayIcon";
                        display = "Friday Music";
                    }
                    if (i == 6)
                    {
                        texture = StardewSymphony.textureManager.getTexture("SaturdayIcon");
                        name = "SaturdayIcon";
                        display = "Saturday Music";
                    }

                    float scale = 1.00f / (texture.getTexture().Width / 64f);
                    Rectangle srcRect = new Rectangle(0, 0, texture.getTexture().Width, texture.getTexture().Height);
                    this.fancyButtons.Add(new Button(name, new Rectangle((int)placement2.X + 50, (int)placement2.Y + ((i % 7) * 100), 64, 64), texture, display, srcRect, scale, new Animation(srcRect), Color.White, Color.White, new ButtonFunctionality(null, null, null)));
                }
            }

            if (this.drawMode == DrawMode.ConditionalViewMode)
            {
                this.fancyButtons.Clear();
                //Vector4 placement = new Vector4((Game1.viewport.Width / 3), (Game1.viewport.Height / 4) + 128, this.width, this.height / 2);
                Vector4 placement2 = new Vector4(this.width * .2f + 400, this.height * .05f, 5 * 100, this.height * .9f);
                Dictionary<string, SongConditionals> conditionals = this.CurrentMusicPack.SongInformation.songs[this.currentSelectedSong.name].songConditionals;
                for (int i = 0; i < conditionals.Count; i++)
                {
                    //Allow 8 songs to be displayed per page.
                    Texture2DExtended texture = StardewSymphony.textureManager.getTexture("MusicNote");
                    float scale = 1.00f / (texture.getTexture().Width / 64f);
                    Rectangle srcRect = new Rectangle(0, 0, texture.getTexture().Width, texture.getTexture().Height);
                    string keyName = conditionals.ElementAt(i).Key;
                    this.fancyButtons.Add(new Button(keyName, new Rectangle((int)placement2.X + 25, (int)placement2.Y + ((i % 6) * 100) + 100, 64, 64), texture, keyName, srcRect, scale, new Animation(srcRect), Color.White, Color.White, new ButtonFunctionality(null, null, null)));
                }
            }
        }

        /// <summary>
        /// Updates the menu ~60 times a second.
        /// </summary>
        /// <param name="time"></param>
        public override void update(GameTime time)
        {
            int updateNumber = 20;

            if (this.drawMode == DrawMode.AlbumFancySelection || this.drawMode == DrawMode.SongSelectionMode || this.drawMode == DrawMode.LocationSelection || this.drawMode == DrawMode.FestivalSelection || this.drawMode == DrawMode.TimeSelection || this.drawMode == DrawMode.ConditionalViewMode || this.drawMode == DrawMode.MenuSelection || this.drawMode == DrawMode.EventSelection || this.drawMode == DrawMode.ConditionalViewMode)
            {
                if (this.framesSinceLastUpdate == updateNumber)
                {
                    var state = Keyboard.GetState();
                    if (state.IsKeyDown(Keys.Left) || state.IsKeyDown(Keys.A)) this.previousPage();
                    if (state.IsKeyDown(Keys.Right) || state.IsKeyDown(Keys.D)) this.nextPage();
                }
                else
                    this.framesSinceLastUpdate++;
            }
        }
        /// <summary>
        /// Plays the currently selected song.
        /// </summary>
        public void playSong()
        {
            this.CurrentMusicPack.PlaySong(this.currentSelectedSong.name);
        }
        /// <summary>
        /// Stops the currently selected song.
        /// </summary>
        public void stopSong()
        {
            this.CurrentMusicPack.StopSong();
        }
        /// <summary>
        /// Adds a song conditional to the current song.
        /// </summary>
        private void addSong()
        {
            //Used to actually save the song.
            this.CurrentMusicPack.SongInformation.songs[this.currentSelectedSong.name].AddSongConditional(this.generateSongTriggerKeyFromSelection());
        }

        /// <summary>Generate the trigger key based on used selection.</summary>
        public string generateSongTriggerKeyFromSelection()
        {
            string key = "";
            string seperator = "_";
            //Seasonal selection region


            #region
            if (this.currentlySelectedSeason != null)
            {
                if (this.currentlySelectedSeason.name == "SpringButton" || this.currentlySelectedSeason.name == "SummerButton" || this.currentlySelectedSeason.name == "FallButton" || this.currentlySelectedSeason.name == "WinterButton")
                {
                    if (this.currentlySelectedSeason.name == "SpringButton") key += "spring";
                    else if (this.currentlySelectedSeason.name == "SummerButton") key += "summer";
                    else if (this.currentlySelectedSeason.name == "FallButton") key += "fall";
                    else if (this.currentlySelectedSeason.name == "WinterButton") key += "winter";
                }
            }
            if (this.currentlySelectedWeather != null)
            {
                if (this.currentlySelectedWeather.name == "SunnyIcon") key += seperator + "sunny";
                if (this.currentlySelectedWeather.name == "RainyIcon") key += seperator + "rain";
                if (this.currentlySelectedWeather.name == "WeatherDebrisIcon") key += seperator + "debris";
                if (this.currentlySelectedWeather.name == "WeatherFestivalIcon") key += seperator + "festival";
                if (this.currentlySelectedWeather.name == "SnowIcon") key += seperator + "snow";
                if (this.currentlySelectedWeather.name == "StormIcon") key += seperator + "lightning";
                if (this.currentlySelectedWeather.name == "WeddingIcon") key += seperator + "wedding";
            }
            if (this.currentlySelectedTime != null)
            {
                if (this.currentlySelectedTime.name == "DayIcon") key += seperator + "day";
                if (this.currentlySelectedTime.name == "NightIcon") key += seperator + "night";
                for (int i = 1; i <= 12; i++)
                {
                    if (this.currentlySelectedTime.name == i.ToString() + "A.M.") key += seperator + i.ToString() + "A.M.";
                    if (this.currentlySelectedTime.name == i.ToString() + "P.M.") key += seperator + i.ToString() + "P.M.";
                }
            }
            if (this.currentlySelectedLocation != null)
            {
                if (this.currentlySelectedLocation.label.Contains("Cabin"))
                {
                    key += seperator + this.currentlySelectedLocation.name;
                }
                else
                {
                    key += seperator + this.currentlySelectedLocation.label;
                }
            }
            if (this.currentlySelectedDay != null)
            {
                if (this.currentlySelectedDay.name == "SundayIcon") key += seperator + "sunday";
                if (this.currentlySelectedDay.name == "MondayIcon") key += seperator + "monday";
                if (this.currentlySelectedDay.name == "TuesdayIcon") key += seperator + "tuesday";
                if (this.currentlySelectedDay.name == "WednesdayIcon") key += seperator + "wednesday";
                if (this.currentlySelectedDay.name == "ThursdayIcon") key += seperator + "thursday";
                if (this.currentlySelectedDay.name == "FridayIcon") key += seperator + "friday";
                if (this.currentlySelectedDay.name == "SaturdayIcon") key += seperator + "saturday";
            }

            if (this.currentlySelectedMenu != null) key += seperator + this.currentlySelectedMenu.label;
            if (this.currentlySelectedFestival != null) key += seperator + this.currentlySelectedFestival.label;
            if (this.currentlySelectedEvent != null) key += seperator + this.currentlySelectedEvent.label;
            #endregion

            return key;
        }

        /// <summary>
        /// Clears all selected conditionals options.
        /// </summary>
        private void clearAllOptions()
        {
            //Check selections for draw mode and then remove if necessary
            if (this.currentlySelectedSeason != null) this.currentlySelectedSeason = null;
            if (this.currentlySelectedWeather != null) this.currentlySelectedWeather = null;
            if (this.currentlySelectedTime != null) this.currentlySelectedTime = null;
            if (this.currentlySelectedDay != null) this.currentlySelectedDay = null;
            if (this.currentlySelectedLocation != null) this.currentlySelectedLocation = null;
            if (this.currentlySelectedEvent != null) this.currentlySelectedEvent = null;
            if (this.currentlySelectedFestival != null) this.currentlySelectedFestival = null;
            if (this.currentlySelectedMenu != null) this.currentlySelectedMenu = null;
        }
        /// <summary>
        /// Returns to a previous menu screen.
        /// </summary>
        public void goBack()
        {
            if (this.drawMode == DrawMode.DaySelection || this.drawMode == DrawMode.EventSelection || this.drawMode == DrawMode.FestivalSelection || this.drawMode == DrawMode.LocationSelection || this.drawMode == DrawMode.MenuSelection || this.drawMode == DrawMode.SeasonSelection || this.drawMode == DrawMode.TimeSelection || this.drawMode == DrawMode.WeatherSelection || this.drawMode == DrawMode.ConditionalViewMode)
            {
                this.drawMode = DrawMode.DifferentSelectionTypesModePage;
                this.updateFancyButtons();
            }
            else if (this.drawMode == DrawMode.DifferentSelectionTypesModePage)
            {
                this.drawMode = DrawMode.SongSelectionMode;
                this.currentSelectedSong = null;
                this.updateFancyButtons();
            }
            else if (this.drawMode == DrawMode.SongSelectionMode)
            {
                this.drawMode = DrawMode.AlbumFancySelection;
                this.currentMusicPackAlbum = null;
                this.updateFancyButtons();
            }
        }
        /// <summary>
        /// Selects the music pack album and sets it up.
        /// </summary>
        /// <param name="b"></param>
        public void selectAlbum(Button b)
        {
            if (b.label == "Null")
                return;

            this.currentMusicPackAlbum = b.clone(new Vector2(this.width * .1f + 64, this.height * .05f + 128));
            this.texturedStrings.Clear();
            this.texturedStrings.Add(SpriteFonts.vanillaFont.ParseString("Name:" + b.name, new Vector2(this.width * .1f, this.height * .05f + 256), b.textColor, false));
            this.drawMode = DrawMode.SongSelectionMode;
        }

        /// <summary>Select a given song from the menu.</summary>
        public void selectSong(Button b)
        {
            if (b.label == "Null")
                return;

            this.currentSelectedSong = b.clone(new Vector2(this.width * .1f + 64, this.height * .05f + 256));
            this.drawMode = DrawMode.DifferentSelectionTypesModePage;
        }

        /// <summary>
        /// Draw the menu.
        /// </summary>
        /// <param name="b"></param>
        public override void draw(SpriteBatch b)
        {
            Vector4 placement = new Vector4(this.width * .1f, this.height * .05f - 96, 4 * 100 + 50, this.height + 32);
            Vector4 placement2 = new Vector4(this.width * .2f + 400, this.height * .05f - 96, 5 * 100, this.height + 32);
            Vector2 pagePlacement = new Vector2(800, 64);

            if (this.drawMode == DrawMode.AlbumFancySelection)
            {
                Vector4 placement3 = new Vector4(Game1.viewport.Width / 4 - 50, Game1.viewport.Height / 4, 8 * 100, 128 * 2);
                this.drawDialogueBoxBackground((int)placement3.X, (int)placement3.Y, (int)placement3.Z, (int)placement3.W, new Color(new Vector4(this.dialogueBoxBackgroundColor.ToVector3(), 0)));

                foreach (var button in this.fancyButtons)
                    button.draw(b);
                foreach (var str in this.texturedStrings)
                    str.draw(b);
            }

            if (this.drawMode == DrawMode.SongSelectionMode)
            {
                this.drawDialogueBoxBackground((int)placement.X, (int)placement.Y, (int)placement.Z, (int)placement.W, new Color(new Vector4(this.dialogueBoxBackgroundColor.ToVector3(), 255)));
                this.drawDialogueBoxBackground((int)placement2.X, (int)placement2.Y, (int)placement2.Z, (int)placement2.W, new Color(new Vector4(this.dialogueBoxBackgroundColor.ToVector3(), 255)));

                int amountToShow = 6;
                this.currentMusicPackAlbum.draw(b);

                int amount;
                if (0 + ((this.currentSongPageIndex + 1) * amountToShow) >= this.fancyButtons.Count)
                {
                    amount = (0 + ((this.currentSongPageIndex + 1) * (amountToShow)) - this.fancyButtons.Count);
                    amount = amountToShow - amount;
                    if (amount < 0) amount = 0;
                }
                else if (this.fancyButtons.Count < amountToShow)
                    amount = this.fancyButtons.Count;
                else
                    amount = amountToShow;

                if (amount == 0 && this.currentSongPageIndex > 1)
                    this.currentSongPageIndex--;

                var drawList = this.fancyButtons.GetRange(0 + (this.currentSongPageIndex * (amountToShow)), amount);

                foreach (var button in drawList)
                    button.draw(b);

                foreach (var str in this.texturedStrings)
                    str.draw(b);

                b.DrawString(Game1.smallFont, "Page: " + (this.currentSongPageIndex + 1) + " of " + ((this.fancyButtons.Count / amountToShow) + 1), this.getFixedPositionFromMenu(pagePlacement), Color.White);
            }

            if (this.drawMode == DrawMode.DifferentSelectionTypesModePage || this.drawMode == DrawMode.SeasonSelection || this.drawMode == DrawMode.WeatherSelection || this.drawMode == DrawMode.DaySelection)
            {
                this.drawDialogueBoxBackground((int)placement.X, (int)placement.Y, (int)placement.Z, (int)placement.W, new Color(new Vector4(this.dialogueBoxBackgroundColor.ToVector3(), 255)));
                this.drawDialogueBoxBackground((int)placement2.X, (int)placement2.Y, (int)placement2.Z, (int)placement2.W, new Color(new Vector4(this.dialogueBoxBackgroundColor.ToVector3(), 255)));

                this.currentMusicPackAlbum.draw(b);
                this.currentSelectedSong.draw(b);

                foreach (Button button in this.fancyButtons)
                    button.draw(b);

                foreach (var str in this.texturedStrings)
                    str.draw(b);

                //draw election buttons here???
            }

            if (this.drawMode == DrawMode.EventSelection)
            {
                this.drawDialogueBoxBackground((int)placement.X, (int)placement.Y, (int)placement.Z, (int)placement.W, new Color(new Vector4(this.dialogueBoxBackgroundColor.ToVector3(), 255)));
                this.drawDialogueBoxBackground((int)placement2.X, (int)placement2.Y, (int)placement2.Z, (int)placement2.W, new Color(new Vector4(this.dialogueBoxBackgroundColor.ToVector3(), 255)));

                int amountToShow = 6;
                //make 3rd dialogue box option;
                this.currentMusicPackAlbum.draw(b);
                this.currentSelectedSong.draw(b);
                //this.currentlySelectedOption.draw(b);
                int amount;
                if (0 + ((this.eventPageIndex + 1) * amountToShow) >= this.fancyButtons.Count)
                {
                    amount = (0 + ((this.eventPageIndex + 1) * (amountToShow)) - this.fancyButtons.Count);
                    amount = amountToShow - amount;
                    if (amount < 0) amount = 0;
                }
                else if (this.fancyButtons.Count < amountToShow)
                    amount = this.fancyButtons.Count;
                else
                    amount = amountToShow;

                if (amount == 0 && this.eventPageIndex > 1)
                    this.eventPageIndex--;

                var drawList = this.fancyButtons.GetRange(0 + (this.eventPageIndex * (amountToShow)), amount);

                foreach (var button in drawList)
                    button.draw(b);

                foreach (var str in this.texturedStrings)
                    str.draw(b);

                b.DrawString(Game1.smallFont, "Page: " + (this.fancyButtons.Count == 0 ? 0 : (this.eventPageIndex + 1)) + " of " + ((this.fancyButtons.Count / amountToShow) + ((this.fancyButtons.Count % amountToShow == 0 ? 0 : 1))), this.getFixedPositionFromMenu(pagePlacement), Color.White);
            }

            if (this.drawMode == DrawMode.MenuSelection)
            {
                this.drawDialogueBoxBackground((int)placement.X, (int)placement.Y, (int)placement.Z, (int)placement.W, new Color(new Vector4(this.dialogueBoxBackgroundColor.ToVector3(), 255)));
                this.drawDialogueBoxBackground((int)placement2.X, (int)placement2.Y, (int)placement2.Z, (int)placement2.W, new Color(new Vector4(this.dialogueBoxBackgroundColor.ToVector3(), 255)));

                int amountToShow = 6;
                //make 3rd dialogue box option;
                this.currentMusicPackAlbum.draw(b);
                this.currentSelectedSong.draw(b);
                //this.currentlySelectedOption.draw(b);

                int amount;
                if (0 + ((this.menuPageIndex + 1) * amountToShow) >= this.fancyButtons.Count)
                {
                    amount = (0 + ((this.menuPageIndex + 1) * (amountToShow)) - this.fancyButtons.Count);
                    amount = amountToShow - amount;
                    if (amount < 0) amount = 0;
                }
                else if (this.fancyButtons.Count < amountToShow)
                    amount = this.fancyButtons.Count;
                else
                    amount = amountToShow;

                if (amount == 0 && this.menuPageIndex > 1)
                    this.menuPageIndex--;

                var drawList = this.fancyButtons.GetRange(0 + (this.menuPageIndex * (amountToShow)), amount);

                foreach (var button in drawList)
                    button.draw(b);

                foreach (var str in this.texturedStrings)
                    str.draw(b);

                b.DrawString(Game1.smallFont, "Page: " + (this.menuPageIndex + 1) + " of " + ((this.fancyButtons.Count / amountToShow) + ((this.fancyButtons.Count % amountToShow == 0 ? 0 : 1))), this.getFixedPositionFromMenu(pagePlacement), Color.White);
            }

            if (this.drawMode == DrawMode.FestivalSelection)
            {
                this.drawDialogueBoxBackground((int)placement.X, (int)placement.Y, (int)placement.Z, (int)placement.W, new Color(new Vector4(this.dialogueBoxBackgroundColor.ToVector3(), 255)));
                this.drawDialogueBoxBackground((int)placement2.X, (int)placement2.Y, (int)placement2.Z, (int)placement2.W, new Color(new Vector4(this.dialogueBoxBackgroundColor.ToVector3(), 255)));

                int amountToShow = 6;
                //make 3rd dialogue box option;
                this.currentMusicPackAlbum.draw(b);
                this.currentSelectedSong.draw(b);
                //this.currentlySelectedOption.draw(b);

                int amount;
                if (0 + ((this.festivalPageIndex + 1) * amountToShow) >= this.fancyButtons.Count)
                {
                    amount = (0 + ((this.festivalPageIndex + 1) * (amountToShow)) - this.fancyButtons.Count);
                    amount = amountToShow - amount;
                    if (amount < 0) amount = 0;
                }
                else if (this.fancyButtons.Count < amountToShow)
                    amount = this.fancyButtons.Count;
                else
                    amount = amountToShow;

                if (amount == 0 && this.festivalPageIndex > 1)
                    this.festivalPageIndex--;

                var drawList = this.fancyButtons.GetRange(0 + (this.festivalPageIndex * (amountToShow)), amount);

                foreach (var button in drawList)
                    button.draw(b);

                foreach (var str in this.texturedStrings)
                    str.draw(b);

                b.DrawString(Game1.smallFont, "Page: " + (this.festivalPageIndex + 1) + " of " + ((this.fancyButtons.Count / amountToShow) + ((this.fancyButtons.Count % amountToShow == 0 ? 0 : 1))), this.getFixedPositionFromMenu(pagePlacement), Color.White);
            }

            if (this.drawMode == DrawMode.TimeSelection)
            {
                this.drawDialogueBoxBackground((int)placement.X, (int)placement.Y, (int)placement.Z, (int)placement.W, new Color(new Vector4(this.dialogueBoxBackgroundColor.ToVector3(), 255)));
                this.drawDialogueBoxBackground((int)placement2.X, (int)placement2.Y, (int)placement2.Z, (int)placement2.W, new Color(new Vector4(this.dialogueBoxBackgroundColor.ToVector3(), 255)));

                //make 3rd dialogue box option;
                this.currentMusicPackAlbum.draw(b);
                this.currentSelectedSong.draw(b);

                int amountToShow = 6;
                int amount;
                if (0 + ((this.timePageIndex + 1) * amountToShow) >= this.fancyButtons.Count)
                {
                    amount = (0 + ((this.timePageIndex + 1) * (amountToShow)) - this.fancyButtons.Count);
                    amount = amountToShow - amount;
                    if (amount < 0) amount = 0;
                }
                else if (this.fancyButtons.Count < amountToShow)
                    amount = this.fancyButtons.Count;
                else
                    amount = amountToShow;

                if (amount == 0 && this.timePageIndex > 1)
                    this.timePageIndex--;

                var drawList = this.fancyButtons.GetRange(0 + (this.timePageIndex * (amountToShow)), amount);

                foreach (var button in drawList)
                    button.draw(b);


                foreach (var str in this.texturedStrings)
                    str.draw(b);

                b.DrawString(Game1.smallFont, "Page: " + (this.timePageIndex + 1) + " of " + ((this.fancyButtons.Count / amountToShow) + ((this.fancyButtons.Count % amountToShow == 0 ? 0 : 1))), this.getFixedPositionFromMenu(pagePlacement), Color.White);
            }

            if (this.drawMode == DrawMode.LocationSelection)
            {
                this.drawDialogueBoxBackground((int)placement.X, (int)placement.Y, (int)placement.Z, (int)placement.W, new Color(new Vector4(this.dialogueBoxBackgroundColor.ToVector3(), 255)));
                this.drawDialogueBoxBackground((int)placement2.X, (int)placement2.Y, (int)placement2.Z, (int)placement2.W, new Color(new Vector4(this.dialogueBoxBackgroundColor.ToVector3(), 255)));

                int amountToShow = 6;
                this.currentMusicPackAlbum.draw(b);
                this.currentSelectedSong.draw(b);

                //Deals with logic regarding different pages.
                int amount;
                if (0 + ((this.locationPageIndex + 1) * amountToShow) >= this.fancyButtons.Count)
                {
                    amount = (0 + ((this.locationPageIndex + 1) * (amountToShow)) - this.fancyButtons.Count);
                    amount = amountToShow - amount;
                    if (amount < 0) amount = 0;
                }
                else if (this.fancyButtons.Count < amountToShow)
                    amount = this.fancyButtons.Count;
                else
                    amount = amountToShow;

                if (amount == 0 && this.locationPageIndex > 1)
                    this.locationPageIndex--;

                var drawList = this.fancyButtons.GetRange(0 + (this.locationPageIndex * (amountToShow)), amount);

                foreach (var button in drawList)
                    button.draw(b);

                foreach (var str in this.texturedStrings)
                    str.draw(b);

                b.DrawString(Game1.smallFont, "Page: " + (this.locationPageIndex + 1) + " of " + ((this.fancyButtons.Count / amountToShow) + ((this.fancyButtons.Count % amountToShow == 0 ? 0 : 1))), this.getFixedPositionFromMenu(pagePlacement), Color.White);
            }

            if (this.drawMode == DrawMode.ConditionalViewMode)
            {
                this.drawDialogueBoxBackground((int)placement.X, (int)placement.Y, (int)placement.Z, (int)placement.W, new Color(new Vector4(this.dialogueBoxBackgroundColor.ToVector3(), 255)));
                this.drawDialogueBoxBackground((int)placement2.X, (int)placement2.Y, (int)placement2.Z, (int)placement2.W, new Color(new Vector4(this.dialogueBoxBackgroundColor.ToVector3(), 255)));

                int amountToShow = 6;
                this.currentMusicPackAlbum.draw(b);
                this.currentSelectedSong.draw(b);

                //Deals with logic regarding different pages.
                int amount;
                if (0 + ((this.conditionalViewPageIndex + 1) * amountToShow) >= this.fancyButtons.Count)
                {
                    amount = (0 + ((this.conditionalViewPageIndex + 1) * (amountToShow)) - this.fancyButtons.Count);
                    amount = amountToShow - amount;
                    if (amount < 0) amount = 0;
                }
                else if (this.fancyButtons.Count < amountToShow)
                    amount = this.fancyButtons.Count;
                else
                    amount = amountToShow;

                if (amount == 0 && this.conditionalViewPageIndex > 1)
                    this.conditionalViewPageIndex--;

                var drawList = this.fancyButtons.GetRange(0 + (this.conditionalViewPageIndex * (amountToShow)), amount);

                foreach (var button in drawList)
                    button.draw(b);

                foreach (var str in this.texturedStrings)
                    str.draw(b);

                b.DrawString(Game1.smallFont, "Page: " + (this.conditionalViewPageIndex + 1) + " of " + ((this.fancyButtons.Count / amountToShow) + ((this.fancyButtons.Count % amountToShow == 0 ? 0 : 1))), this.getFixedPositionFromMenu(pagePlacement), Color.White);
            }

            //draw other menu buttons
            this.drawSelectedButtons(b);
            this.drawHoverText(b);
            this.drawMouse(b);
        }

        /// <summary>
        /// Draws any necessary hovertext for the menu.
        /// </summary>
        /// <param name="b"></param>
        private void drawHoverText(SpriteBatch b)
        {
            if (this.hoverText.Length <= 0)
                return;
            IClickableMenu.drawHoverText(b, this.hoverText, Game1.dialogueFont, 0, 0, -1, (string)null, -1, (string[])null, (Item)null, 0, -1, -1, -1, -1, 1f, (CraftingRecipe)null);
        }

        /// <summary>
        /// Draw necessary menu buttons.
        /// </summary>
        /// <param name="b"></param>
        private void drawSelectedButtons(SpriteBatch b)
        {
            if (this.currentSelectedSong != null)
            {
                this.playButton.draw(b);
                this.stopButton.draw(b);
                if (this.drawMode == DrawMode.DaySelection || this.drawMode == DrawMode.EventSelection || this.drawMode == DrawMode.FestivalSelection || this.drawMode == DrawMode.LocationSelection || this.drawMode == DrawMode.MenuSelection || this.drawMode == DrawMode.SeasonSelection || this.drawMode == DrawMode.TimeSelection || this.drawMode == DrawMode.WeatherSelection || this.drawMode == DrawMode.DifferentSelectionTypesModePage)
                {
                    this.backButton.draw(b);
                    if (this.currentlySelectedSeason != null) this.currentlySelectedSeason.draw(b);
                    if (this.currentlySelectedWeather != null) this.currentlySelectedWeather.draw(b);
                    if (this.currentlySelectedTime != null) this.currentlySelectedTime.draw(b);
                    if (this.currentlySelectedDay != null) this.currentlySelectedDay.draw(b);
                    if (this.currentlySelectedLocation != null) this.currentlySelectedLocation.draw(b);
                    if (this.currentlySelectedEvent != null) this.currentlySelectedEvent.draw(b);
                    if (this.currentlySelectedFestival != null) this.currentlySelectedFestival.draw(b);
                    if (this.currentlySelectedMenu != null) this.currentlySelectedMenu.draw(b);
                }

                if (this.drawMode == DrawMode.ConditionalViewMode)
                {
                    this.backButton.draw(b);
                }

                if (this.drawMode == DrawMode.DifferentSelectionTypesModePage)
                {
                    this.addButton.draw(b);
                    this.saveButton.draw(b);
                    this.deleteButton.draw(b);
                    this.conditionalViewButton.draw(b);
                }

                if (this.drawMode == DrawMode.AlbumFancySelection || this.drawMode == DrawMode.EventSelection || this.drawMode == DrawMode.FestivalSelection || this.drawMode == DrawMode.LocationSelection || this.drawMode == DrawMode.MenuSelection || this.drawMode == DrawMode.SongSelectionMode || this.drawMode == DrawMode.TimeSelection || this.drawMode == DrawMode.ConditionalViewMode)
                {
                    this.lastPageButton.draw(b);
                    this.nextPageButton.draw(b);
                }
            }
            else
            {
                if (this.drawMode == DrawMode.SongSelectionMode)
                {
                    this.backButton.draw(b);
                    this.lastPageButton.draw(b);
                    this.nextPageButton.draw(b);
                }
            }


        }
    }
}
