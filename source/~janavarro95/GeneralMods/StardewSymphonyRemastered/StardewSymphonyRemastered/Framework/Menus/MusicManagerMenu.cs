using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Locations;
using StardustCore.Animations;
using StardustCore.UIUtilities;
using StardustCore.UIUtilities.MenuComponents;
using StardustCore.UIUtilities.MenuComponents.Delegates;
using StardustCore.UIUtilities.MenuComponents.Delegates.Functionality;
using StardustCore.UIUtilities.SpriteFonts;

namespace StardewSymphonyRemastered.Framework.Menus
{
    /// <summary>Interface for the menu for selection music.</summary>
    public class MusicManagerMenu : IClickableMenuExtended
    {
        /// <summary>The different displays for this menu.</summary>
        public enum DrawMode
        {
            AlbumSelection,
            AlbumFancySelection,
            SongSelectionMode,

            DifferentSelectionTypesMode, //Used for locations, events, festivals,  menus (house, exclamation mark, star, and list/book icons respectively)
            WeatherSelection,
            FestivalSelection,
            EventSelection,
            MenuSelection,

            TimeSelection,
            LocationSelection,
            DaySelection,
            NothingElseToDisplay,

            SelectedEvent,
            SelectedFestival,
            SelectedMenu,

            SeasonSelection,
        }

        public List<Button> musicAlbumButtons;
        public Button currentMusicPackAlbum;
        public Button currentSelectedSong;
        public Button currentlySelectedOption; //The big button for season, menu, event, and festivals
        public Button currentlySelectedWeather; //Used to display what weather the user selected
        public Button currentlySelectedTime;
        public Button currentlySelectedLocation;
        public Button currentlySelectedDay;

        public Button currentlySelectedFestival;
        public Button currentlySelectedEvent;
        public Button currentlySelectedMenu;

        public Button addButton;
        public Button deleteButton;
        public Button playButton;
        public Button stopButton;
        public Button backButton;


        public bool selectedJustLocation;

        public DrawMode drawMode;
        public int currentAlbumIndex;
        public int currentSongPageIndex;
        public int locationPageIndex;
        public int festivalPageIndex;
        public int eventPageIndex;
        public int menuPageIndex;

        public List<Button> fancyButtons; //List that holds all of the buttons for the fancy album menu.
        public int framesSinceLastUpdate; //Used to control how fast we can cycle through the menu.

        public bool searchBoxSelected;

        /// <summary>Construct an instance.</summary>
        public MusicManagerMenu(float width, float height)
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
            foreach (MusicPack musicPack in StardewSymphony.musicManager.MusicPacks.Values)
            {
                var sortedQuery = musicPack.SongInformation.listOfSongsWithoutTriggers.OrderBy(name => name);
                musicPack.SongInformation.listOfSongsWithoutTriggers = sortedQuery.ToList(); //Alphabetize.
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
        }

        /// <summary>Runs every game tick to check for stuff.</summary>
        public override void update(GameTime time)
        {
            int updateNumber = 20;
            if (this.drawMode == DrawMode.AlbumFancySelection)
            {
                if (this.framesSinceLastUpdate == updateNumber)
                {
                    var state = Keyboard.GetState();
                    if (state.IsKeyDown(Keys.Left) || state.IsKeyDown(Keys.A))
                    {
                        this.currentAlbumIndex--;
                        if (this.currentAlbumIndex < 0) this.currentAlbumIndex = this.musicAlbumButtons.Count - 1;
                        this.updateFancyButtons();
                        this.framesSinceLastUpdate = 0;
                        Game1.playSound("shwip");
                    }

                    if (state.IsKeyDown(Keys.Right) || state.IsKeyDown(Keys.D))
                    {
                        this.currentAlbumIndex++;
                        if (this.currentAlbumIndex == this.musicAlbumButtons.Count) this.currentAlbumIndex = 0;
                        this.updateFancyButtons();
                        this.framesSinceLastUpdate = 0;
                        Game1.playSound("shwip");
                    }
                }
                else
                {
                    this.framesSinceLastUpdate++;
                }
            }

            if (this.drawMode == DrawMode.SongSelectionMode)
            {
                if (this.framesSinceLastUpdate == updateNumber)
                {
                    var state = Keyboard.GetState();
                    if (state.IsKeyDown(Keys.Left) || state.IsKeyDown(Keys.A))
                    {
                        if (this.currentSongPageIndex > 0)
                            this.currentSongPageIndex--;
                        this.updateFancyButtons();
                        this.framesSinceLastUpdate = 0;
                        Game1.playSound("shwip");
                    }

                    if (state.IsKeyDown(Keys.Right) || state.IsKeyDown(Keys.D))
                    {
                        this.currentSongPageIndex++;
                        this.updateFancyButtons();
                        this.framesSinceLastUpdate = 0;
                        Game1.playSound("shwip");
                    }
                }
                else
                    this.framesSinceLastUpdate++;
            }

            if (this.drawMode == DrawMode.LocationSelection)
            {
                if (this.framesSinceLastUpdate == updateNumber)
                {
                    var state = Keyboard.GetState();
                    if (state.IsKeyDown(Keys.Left) || state.IsKeyDown(Keys.A))
                    {
                        if (this.locationPageIndex > 0)
                            this.locationPageIndex--;
                        this.updateFancyButtons();
                        this.framesSinceLastUpdate = 0;
                        Game1.playSound("shwip");
                    }

                    if (state.IsKeyDown(Keys.Right) || state.IsKeyDown(Keys.D))
                    {
                        this.locationPageIndex++;
                        this.updateFancyButtons();
                        this.framesSinceLastUpdate = 0;
                        Game1.playSound("shwip");
                    }
                }
                else
                    this.framesSinceLastUpdate++;
            }

            if (this.drawMode == DrawMode.FestivalSelection)
            {
                if (this.framesSinceLastUpdate == updateNumber)
                {
                    var state = Keyboard.GetState();
                    if (state.IsKeyDown(Keys.Left) || state.IsKeyDown(Keys.A))
                    {
                        if (this.festivalPageIndex > 0)
                            this.festivalPageIndex--;
                        this.updateFancyButtons();
                        this.framesSinceLastUpdate = 0;
                        Game1.playSound("shwip");
                    }

                    if (state.IsKeyDown(Keys.Right) || state.IsKeyDown(Keys.D))
                    {
                        this.festivalPageIndex++;
                        this.updateFancyButtons();
                        this.framesSinceLastUpdate = 0;
                        Game1.playSound("shwip");
                    }
                }
                else
                    this.framesSinceLastUpdate++;
            }

            if (this.drawMode == DrawMode.EventSelection)
            {
                if (this.framesSinceLastUpdate == updateNumber)
                {
                    var state = Keyboard.GetState();
                    if (state.IsKeyDown(Keys.Left) || state.IsKeyDown(Keys.A))
                    {
                        if (this.eventPageIndex > 0)
                            this.eventPageIndex--;
                        this.updateFancyButtons();
                        this.framesSinceLastUpdate = 0;
                        Game1.playSound("shwip");
                    }

                    if (state.IsKeyDown(Keys.Right) || state.IsKeyDown(Keys.D))
                    {
                        this.eventPageIndex++;
                        this.updateFancyButtons();
                        this.framesSinceLastUpdate = 0;
                        Game1.playSound("shwip");
                    }
                }
                else
                    this.framesSinceLastUpdate++;
            }

            if (this.drawMode == DrawMode.MenuSelection)
            {
                if (this.framesSinceLastUpdate == updateNumber)
                {
                    var state = Keyboard.GetState();
                    if (state.IsKeyDown(Keys.Left) || state.IsKeyDown(Keys.A))
                    {
                        if (this.menuPageIndex > 0)
                            this.menuPageIndex--;
                        this.updateFancyButtons();
                        this.framesSinceLastUpdate = 0;
                        Game1.playSound("shwip");
                    }

                    if (state.IsKeyDown(Keys.Right) || state.IsKeyDown(Keys.D))
                    {
                        this.menuPageIndex++;
                        this.updateFancyButtons();
                        this.framesSinceLastUpdate = 0;
                        Game1.playSound("shwip");
                    }
                }
                else
                    this.framesSinceLastUpdate++;
            }
        }


        /// <summary>Update the position of the album artwork when displaying it using the fancy buttons menu.</summary>
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
                            MusicPack musicPack = (MusicPack)this.fancyButtons.ElementAt(count).buttonFunctionality.hover.paramaters[0];
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
                MusicPack musicPack = (MusicPack)this.currentMusicPackAlbum.buttonFunctionality.leftClick.paramaters[0];
                List<string> musicPackSongList = musicPack.SongInformation.listOfSongsWithoutTriggers;

                Vector4 placement2 = new Vector4(this.width * .2f + 400, this.height * .05f, 5 * 100, this.height * .9f);
                for (int i = 0; i < musicPackSongList.Count; i++)
                {
                    //Allow 8 songs to be displayed per page.
                    Texture2DExtended texture = StardewSymphony.textureManager.getTexture("MusicNote");
                    float scale = 1.00f / (texture.getTexture().Width / 64f);
                    string songName = musicPackSongList.ElementAt(i);
                    Rectangle srcRect = new Rectangle(0, 0, texture.getTexture().Width, texture.getTexture().Height);
                    this.fancyButtons.Add(new Button(songName, new Rectangle((int)placement2.X + 25, (int)placement2.Y + ((i % 6) * 100) + 100, 64, 64), texture, songName, srcRect, scale, new Animation(srcRect), Color.White, Color.White, new ButtonFunctionality(null, null, null)));
                }
            }

            //Festival selection mode.
            if (this.drawMode == DrawMode.FestivalSelection)
            {
                this.fancyButtons.Clear();
                //Vector4 placement = new Vector4((Game1.viewport.Width / 3), (Game1.viewport.Height / 4) + 128, this.width, this.height / 2);

                Vector4 placement2 = new Vector4(this.width * .2f + 400, this.height * .05f, 5 * 100, this.height * .9f);
                for (int i = 0; i < SongSpecifics.festivals.Count; i++)
                {
                    //Allow 8 songs to be displayed per page.
                    Texture2DExtended texture = StardewSymphony.textureManager.getTexture("GreenBallon");
                    float scale = 1.00f / (texture.getTexture().Height / 64f);
                    Rectangle srcRect = new Rectangle(0, 0, texture.getTexture().Width, texture.getTexture().Height);
                    this.fancyButtons.Add(new Button(SongSpecifics.festivals.ElementAt(i), new Rectangle((int)placement2.X + 50, (int)placement2.Y + ((i % 6) * 100) + 100, 64, 64), texture, SongSpecifics.festivals.ElementAt(i), srcRect, scale, new Animation(srcRect), Color.White, Color.White, new ButtonFunctionality(null, null, null)));
                }
            }

            //Menu selection mode.
            if (this.drawMode == DrawMode.MenuSelection)
            {
                this.fancyButtons.Clear();
                //Vector4 placement = new Vector4((Game1.viewport.Width / 3), (Game1.viewport.Height / 4) + 128, this.width, this.height / 2);

                Vector4 placement2 = new Vector4(this.width * .2f + 400, this.height * .05f, 5 * 100, this.height * .9f);
                for (int i = 0; i < SongSpecifics.menus.Count; i++)
                {

                    //Allow 8 songs to be displayed per page.
                    Texture2DExtended texture = StardewSymphony.textureManager.getTexture("MenuIcon");
                    float scale = 1.00f / (texture.getTexture().Width / 64f);
                    Rectangle srcRect = new Rectangle(0, 0, texture.getTexture().Width, texture.getTexture().Height);
                    this.fancyButtons.Add(new Button(SongSpecifics.menus.ElementAt(i), new Rectangle((int)placement2.X + 50, (int)placement2.Y + ((i % 6) * 100) + 100, 64, 64), texture, SongSpecifics.menus.ElementAt(i), srcRect, scale, new Animation(srcRect), Color.White, Color.White, new ButtonFunctionality(null, null, null)));
                }
            }

            //Event selection mode.
            if (this.drawMode == DrawMode.EventSelection)
            {
                this.fancyButtons.Clear();
                //Vector4 placement = new Vector4((Game1.viewport.Width / 3), (Game1.viewport.Height / 4) + 128, this.width, this.height / 2);

                Vector4 placement2 = new Vector4(this.width * .2f + 400, this.height * .05f, 5 * 100, this.height * .9f);
                for (int i = 0; i < SongSpecifics.events.Count; i++)
                {

                    //Allow 8 songs to be displayed per page.
                    Texture2DExtended texture = StardewSymphony.textureManager.getTexture("StarIcon");
                    float scale = 1.00f / (texture.getTexture().Width / 64f);
                    Rectangle srcRect = new Rectangle(0, 0, texture.getTexture().Width, texture.getTexture().Height);
                    this.fancyButtons.Add(new Button(SongSpecifics.events.ElementAt(i), new Rectangle((int)placement2.X + 50, (int)placement2.Y + ((i % 6) * 100) + 100, 64, 64), texture, SongSpecifics.events.ElementAt(i), srcRect, scale, new Animation(srcRect), Color.White, Color.White, new ButtonFunctionality(null, null, null)));
                }
            }
            //Options selection mode.
            if (this.drawMode == DrawMode.DifferentSelectionTypesMode)
            {
                this.fancyButtons.Clear();

                int buttonXPosition = 450;

                //Season Icon placement.
                Vector4 seasonPlacement = new Vector4(this.width * .2f + buttonXPosition, this.height * .25f, 5 * 100, this.height * .9f);
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

                Vector4 festivalPlacement = new Vector4(this.width * .2f + buttonXPosition, this.height * .35f, 6 * 100, this.height * .9f);
                Vector4 eventPlacement = new Vector4(this.width * .2f + buttonXPosition, this.height * .45f, 7 * 100, this.height * .9f);
                Vector4 menuPlacement = new Vector4(this.width * .2f + buttonXPosition, this.height * .55f, 8 * 100, this.height * .9f);
                Vector4 locationPlacement = new Vector4(this.width * .2f + buttonXPosition, this.height * .65f, 9 * 100, this.height * .9f);

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

                //Menu Icon placement.
                Texture2DExtended locationTexture = StardewSymphony.textureManager.getTexture("HouseIcon");
                float locationScale = 1.00f / (locationTexture.getTexture().Width / 64f);
                Rectangle locationRect = new Rectangle(0, 0, locationTexture.getTexture().Width, locationTexture.getTexture().Height);
                this.fancyButtons.Add(new Button("LocationButton", new Rectangle((int)locationPlacement.X, (int)locationPlacement.Y, 64, 64), locationTexture, "Location Music", locationRect, locationScale, new Animation(locationRect), Color.White, Color.White, new ButtonFunctionality(null, null, null)));
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

                //Festival Icon placement.
                Texture2DExtended festivalTexture = StardewSymphony.textureManager.getTexture("SummerIcon");
                float festivalScale = 1.00f / (festivalTexture.getTexture().Width / 64f);
                Rectangle festivalSrcRect = new Rectangle(0, 0, festivalTexture.getTexture().Width, festivalTexture.getTexture().Height);
                this.fancyButtons.Add(new Button("SummerButton", new Rectangle((int)summerPlacement.X, (int)summerPlacement.Y, 64, 64), festivalTexture, "Summer Music", festivalSrcRect, festivalScale, new Animation(festivalSrcRect), Color.White, Color.White, new ButtonFunctionality(null, null, null)));

                //Event Icon placement.
                Texture2DExtended eventTexture = StardewSymphony.textureManager.getTexture("FallIcon");
                float eventScale = 1.00f / (eventTexture.getTexture().Width / 64f);
                Rectangle eventSrcRectangle = new Rectangle(0, 0, eventTexture.getTexture().Width, eventTexture.getTexture().Height);
                this.fancyButtons.Add(new Button("FallButton", new Rectangle((int)fallPlacement.X, (int)fallPlacement.Y, 64, 64), eventTexture, "Fall Music", eventSrcRectangle, eventScale, new Animation(eventSrcRectangle), Color.White, Color.White, new ButtonFunctionality(null, null, null)));

                //Menu Icon placement.
                Texture2DExtended menuTexture = StardewSymphony.textureManager.getTexture("WinterIcon");
                float menuScale = 1.00f / (menuTexture.getTexture().Width / 64f);
                Rectangle menuSrcRectangle = new Rectangle(0, 0, menuTexture.getTexture().Width, menuTexture.getTexture().Height);
                this.fancyButtons.Add(new Button("WinterButton", new Rectangle((int)winterPlacement.X, (int)winterPlacement.Y, 64, 64), menuTexture, "Winter Music", menuSrcRectangle, menuScale, new Animation(menuSrcRectangle), Color.White, Color.White, new ButtonFunctionality(null, null, null)));
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
                    this.fancyButtons.Add(new Button(name, new Rectangle((int)placement2.X + 50, (int)placement2.Y + ((i % 7) * 100), 64, 64), texture, display, srcRect, scale, new Animation(srcRect), Color.White, Color.White, new ButtonFunctionality(null, null, null)));
                }
            }

            //Location selection mode.
            if (this.drawMode == DrawMode.LocationSelection)
            {
                this.fancyButtons.Clear();
                int numOfEmptyCabin = 1;
                Vector4 placement2 = new Vector4(this.width * .2f + 400, this.height * .05f, 5 * 100, this.height * .9f);
                for (int i = 0; i < SongSpecifics.locations.Count; i++)
                {
                    string locName = SongSpecifics.locations.ElementAt(i);

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

                    //Allow 8 songs to be displayed per page.
                    Texture2DExtended texture = StardewSymphony.textureManager.getTexture("HouseIcon");
                    float scale = 1.00f / (texture.getTexture().Width / 64f);
                    Rectangle srcRect = new Rectangle(0, 0, texture.getTexture().Width, texture.getTexture().Height);
                    this.fancyButtons.Add(new Button(locName, new Rectangle((int)placement2.X + 25, (int)placement2.Y + ((i % 6) * 100) + 100, 64, 64), texture, locName, srcRect, scale, new Animation(srcRect), Color.White, Color.White, new ButtonFunctionality(null, null, null)));
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

            if (this.drawMode == DrawMode.NothingElseToDisplay || this.drawMode == DrawMode.SelectedEvent || this.drawMode == DrawMode.SelectedMenu || this.drawMode == DrawMode.SelectedFestival)
                this.fancyButtons.Clear();
        }

        /// <summary>Functionality that occurs when right clicking a menu component.</summary>
        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            if (this.drawMode == DrawMode.AlbumSelection)
            {
                foreach (var button in this.musicAlbumButtons)
                {
                    if (button.containsPoint(x, y))
                        button.onRightClick();
                }
            }
        }

        /// <summary>Actions that occur when hovering over an icon.</summary>
        public override void performHoverAction(int x, int y)
        {
            if (this.drawMode == DrawMode.AlbumSelection)
            {
                foreach (var button in this.musicAlbumButtons)
                {
                    if (button.containsPoint(x, y))
                    {
                        MusicPack musicPack = (MusicPack)button.buttonFunctionality.hover.paramaters[0];
                        button.hoverText = musicPack.Name;
                        button.onHover();
                    }
                    else
                        button.hoverText = "";
                }
            }

            if (this.drawMode == DrawMode.AlbumFancySelection)
            {
                foreach (var button in this.fancyButtons)
                {
                    if (button.containsPoint(x, y))
                    {
                        if (button.buttonFunctionality?.hover?.paramaters == null || button.buttonFunctionality.hover.paramaters.Count == 0)
                            continue;

                        MusicPack musicPack = (MusicPack)button.buttonFunctionality.hover.paramaters[0];
                        button.hoverText = musicPack.Name;
                        button.onHover();
                    }
                    else
                        button.hoverText = "";
                }
            }
        }

        /// <summary>Functionality that occurs when left clicking a menu component.</summary>
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            bool buttonSelected = false;

            if (this.currentSelectedSong != null && this.currentMusicPackAlbum != null && this.playButton.containsPoint(x, y))
            {
                Game1.playSound("coin");
                this.playSong();
                return;
            }

            if (this.currentSelectedSong != null && this.currentMusicPackAlbum != null && this.stopButton.containsPoint(x, y))
            {
                Game1.playSound("coin");
                this.stopSong();
                return;
            }

            if (this.currentSelectedSong != null && this.currentMusicPackAlbum != null && this.addButton.containsPoint(x, y))
            {
                Game1.playSound("coin");
                this.addSong();
                return;
            }

            if (this.currentSelectedSong != null && this.currentMusicPackAlbum != null && this.deleteButton.containsPoint(x, y))
            {
                Game1.playSound("coin");
                this.deleteSong();
                return;
            }

            if (this.backButton.containsPoint(x, y))
            {
                Game1.playSound("coin");
                this.goBack();
                return;
            }

            if (this.drawMode == DrawMode.AlbumSelection)
            {
                foreach (var button in this.musicAlbumButtons)
                {
                    if (button.containsPoint(x, y))
                    {
                        Game1.playSound("coin");
                        this.selectAlbum(button);
                        button.onLeftClick();
                    }
                }
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
                        Game1.playSound("coin");
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
                        Game1.playSound("coin");
                        this.selectSong(component);
                        songSelected = true;
                    }
                }
                if (songSelected)
                    this.updateFancyButtons();
                return;
            }

            if (this.drawMode == DrawMode.SeasonSelection)
            {
                foreach (var button in this.fancyButtons)
                {
                    if (button.containsPoint(x, y))
                    {
                        Vector2 position = new Vector2(this.width * .1f + 64, this.height * .05f + 320);
                        switch (button.name)
                        {
                            case "SpringButton":
                                this.currentlySelectedOption = button.clone(position);
                                this.drawMode = DrawMode.WeatherSelection;
                                buttonSelected = true;
                                break;

                            case "SummerButton":
                                this.currentlySelectedOption = button.clone(position);
                                this.drawMode = DrawMode.WeatherSelection;
                                buttonSelected = true;
                                break;

                            case "FallButton":
                                this.currentlySelectedOption = button.clone(position);
                                this.drawMode = DrawMode.WeatherSelection;
                                buttonSelected = true;
                                break;

                            case "WinterButton":
                                this.currentlySelectedOption = button.clone(position);
                                this.drawMode = DrawMode.WeatherSelection;
                                buttonSelected = true;
                                break;
                        }
                    }
                }
                if (buttonSelected)
                {
                    Game1.playSound("coin");
                    this.updateFancyButtons();
                }
                return;
            }

            //Left click an option.
            if (this.drawMode == DrawMode.DifferentSelectionTypesMode)
            {
                foreach (var button in this.fancyButtons)
                {
                    if (button.containsPoint(x, y))
                    {
                        Vector2 position = new Vector2(this.width * .1f + 64, this.height * .05f + 320);
                        switch (button.name)
                        {
                            case "SeasonIcon":
                                this.currentlySelectedOption = button.clone(position);
                                this.drawMode = DrawMode.SeasonSelection;
                                buttonSelected = true;
                                break;

                            case "FestivalIcon":
                                this.currentlySelectedOption = button.clone(position);
                                this.drawMode = DrawMode.FestivalSelection;
                                buttonSelected = true;
                                break;

                            case "EventIcon":
                                this.currentlySelectedOption = button.clone(position);
                                this.drawMode = DrawMode.EventSelection;
                                buttonSelected = true;
                                break;

                            case "MenuIcon":
                                this.currentlySelectedOption = button.clone(position);
                                this.drawMode = DrawMode.MenuSelection;
                                buttonSelected = true;
                                break;

                            case "LocationButton":
                                this.currentlySelectedOption = button.clone(position);
                                this.drawMode = DrawMode.LocationSelection;
                                this.selectedJustLocation = true;
                                buttonSelected = true;
                                break;
                        }
                    }
                }
                if (buttonSelected)
                {
                    Game1.playSound("coin");
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
                        Vector2 position = new Vector2(this.width * .1f + 64, this.height * .05f + 384);
                        switch (button.name)
                        {
                            case "SunnyIcon":
                            case "RainyIcon":
                            case "SnowIcon":
                            case "WeatherDebrisIcon":
                            case "StormIcon":
                            case "WeatherFestivalIcon":
                            case "WeddingIcon":
                                this.currentlySelectedWeather = button.clone(position);
                                this.drawMode = DrawMode.TimeSelection;
                                buttonSelected = true;
                                break;
                        }
                    }
                }
                if (buttonSelected)
                {
                    Game1.playSound("coin");
                    this.updateFancyButtons();
                }
                return;
            }

            //Left click an option.
            if (this.drawMode == DrawMode.TimeSelection)
            {
                foreach (var button in this.fancyButtons)
                {
                    if (button.containsPoint(x, y))
                    {
                        Vector2 position = new Vector2(this.width * .1f + 64, this.height * .05f + 448);
                        if (button.name == "DayIcon" || button.name == "NightIcon")
                        {
                            this.currentlySelectedTime = button.clone(position);
                            this.drawMode = DrawMode.LocationSelection;
                            buttonSelected = true;
                        }
                    }
                }
                if (buttonSelected)
                {
                    Game1.playSound("coin");
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
                        Vector2 position = new Vector2(this.width * .1f + 64, this.height * .05f + 512);
                        this.currentlySelectedLocation = component.clone(position);
                        songSelected = true;
                        this.drawMode = !this.selectedJustLocation
                            ? DrawMode.DaySelection
                            : DrawMode.NothingElseToDisplay;
                    }
                }
                if (songSelected)
                {
                    Game1.playSound("coin");
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
                        Vector2 position = new Vector2(this.width * .1f + 64, this.height * .05f + 384);
                        this.currentlySelectedFestival = component.clone(position);
                        songSelected = true;
                        this.drawMode = DrawMode.SelectedFestival;
                    }
                }
                if (songSelected)
                {
                    Game1.playSound("coin");
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
                        Vector2 position = new Vector2(this.width * .1f + 64, this.height * .05f + 384);
                        this.currentlySelectedMenu = component.clone(position);
                        songSelected = true;
                        this.drawMode = DrawMode.SelectedMenu;
                    }
                }
                if (songSelected)
                {
                    Game1.playSound("coin");
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
                        Vector2 position = new Vector2(this.width * .1f + 64, this.height * .05f + 384);
                        this.currentlySelectedEvent = component.clone(position);
                        songSelected = true;
                        this.drawMode = DrawMode.SelectedEvent;
                    }
                }
                if (songSelected)
                {
                    Game1.playSound("coin");
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
                        Vector2 position = new Vector2(this.width * .1f + 64, this.height * .05f + 576);
                        //Get any valid location button.
                        this.currentlySelectedDay = button.clone(position);
                        this.drawMode = DrawMode.NothingElseToDisplay;
                        buttonSelected = true;
                    }
                }
                if (buttonSelected)
                {
                    Game1.playSound("coin");
                    this.updateFancyButtons();
                }
            }
        }

        /// <summary>Checks whether or not the currently selected song has already been added for the currently selected music pack.</summary>
        /// <returns>Whether or not the currently selected song has been added to the list of song triggers for the music pack.</returns>
        public bool doesPackContainMusic()
        {
            if (this.currentMusicPackAlbum == null || this.currentSelectedSong == null) return false;
            MusicPack musicPack = (MusicPack)this.currentMusicPackAlbum.buttonFunctionality.leftClick.paramaters[0];
            //Check for generic festival music.
            if (this.drawMode == DrawMode.SelectedFestival)
            {
                var festivalSonglist = musicPack.SongInformation.festivalSongs;
                return festivalSonglist != null && festivalSonglist.Contains(musicPack.SongInformation.getSongFromList(festivalSonglist, this.currentSelectedSong.name));
            }
            //Check for generic event music.
            if (this.drawMode == DrawMode.SelectedEvent)
            {
                var eventSonglist = musicPack.SongInformation.eventSongs;
                return eventSonglist != null && eventSonglist.Contains(musicPack.SongInformation.getSongFromList(eventSonglist, this.currentSelectedSong.name));
            }
            //Check for seasonal music triggers.
            var songList = musicPack.SongInformation.getSongList(this.generateSongTriggerKeyFromSelection());
            return songList.Value != null && songList.Value.Contains(musicPack.SongInformation.getSongFromList(songList.Value, this.currentSelectedSong.name));
        }

        /// <summary>Draws the menu and it's respective components depending on the drawmode that is currently set.</summary>
        public override void draw(SpriteBatch b)
        {
            Vector4 placement = new Vector4(this.width * .1f, this.height * .05f - 96, 4 * 100 + 50, this.height + 32);
            Vector4 placement2 = new Vector4(this.width * .2f + 400, this.height * .05f - 96, 5 * 100, this.height + 32);

            if (this.drawMode == DrawMode.AlbumSelection)
            {
                this.drawDialogueBoxBackground();
                foreach (var button in this.musicAlbumButtons)
                    button.draw(b);
            }

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
            }

            if (this.drawMode == DrawMode.DifferentSelectionTypesMode)
            {
                this.drawDialogueBoxBackground((int)placement.X, (int)placement.Y, (int)placement.Z, (int)placement.W, new Color(new Vector4(this.dialogueBoxBackgroundColor.ToVector3(), 255)));
                this.drawDialogueBoxBackground((int)placement2.X, (int)placement2.Y, (int)placement2.Z, (int)placement2.W, new Color(new Vector4(this.dialogueBoxBackgroundColor.ToVector3(), 255)));

                this.currentMusicPackAlbum.draw(b);
                this.currentSelectedSong.draw(b);

                foreach (Button button in this.fancyButtons)
                    button.draw(b);

                foreach (var str in this.texturedStrings)
                    str.draw(b);
            }

            if (this.drawMode == DrawMode.SeasonSelection)
            {
                this.drawDialogueBoxBackground((int)placement.X, (int)placement.Y, (int)placement.Z, (int)placement.W, new Color(new Vector4(this.dialogueBoxBackgroundColor.ToVector3(), 255)));
                this.drawDialogueBoxBackground((int)placement2.X, (int)placement2.Y, (int)placement2.Z, (int)placement2.W, new Color(new Vector4(this.dialogueBoxBackgroundColor.ToVector3(), 255)));

                //make 3rd dialogue box option;
                this.currentMusicPackAlbum.draw(b);
                this.currentSelectedSong.draw(b);

                foreach (Button button in this.fancyButtons)
                    button.draw(b);

                foreach (var str in this.texturedStrings)
                    str.draw(b);
            }

            if (this.drawMode == DrawMode.WeatherSelection)
            {
                this.drawDialogueBoxBackground((int)placement.X, (int)placement.Y, (int)placement.Z, (int)placement.W, new Color(new Vector4(this.dialogueBoxBackgroundColor.ToVector3(), 255)));
                this.drawDialogueBoxBackground((int)placement2.X, (int)placement2.Y, (int)placement2.Z, (int)placement2.W, new Color(new Vector4(this.dialogueBoxBackgroundColor.ToVector3(), 255)));

                //make 3rd dialogue box option;
                this.currentMusicPackAlbum.draw(b);
                this.currentSelectedSong.draw(b);
                this.currentlySelectedOption.draw(b);

                foreach (Button button in this.fancyButtons)
                    button.draw(b);

                foreach (var str in this.texturedStrings)
                {
                    str.draw(b);
                }
            }

            if (this.drawMode == DrawMode.TimeSelection)
            {
                this.drawDialogueBoxBackground((int)placement.X, (int)placement.Y, (int)placement.Z, (int)placement.W, new Color(new Vector4(this.dialogueBoxBackgroundColor.ToVector3(), 255)));
                this.drawDialogueBoxBackground((int)placement2.X, (int)placement2.Y, (int)placement2.Z, (int)placement2.W, new Color(new Vector4(this.dialogueBoxBackgroundColor.ToVector3(), 255)));


                //make 3rd dialogue box option;
                this.currentMusicPackAlbum.draw(b);
                this.currentSelectedSong.draw(b);
                this.currentlySelectedOption.draw(b);
                this.currentlySelectedWeather.draw(b);

                foreach (Button button in this.fancyButtons)
                    button.draw(b);

                foreach (var str in this.texturedStrings)
                    str.draw(b);
            }

            if (this.drawMode == DrawMode.LocationSelection)
            {
                this.drawDialogueBoxBackground((int)placement.X, (int)placement.Y, (int)placement.Z, (int)placement.W, new Color(new Vector4(this.dialogueBoxBackgroundColor.ToVector3(), 255)));
                this.drawDialogueBoxBackground((int)placement2.X, (int)placement2.Y, (int)placement2.Z, (int)placement2.W, new Color(new Vector4(this.dialogueBoxBackgroundColor.ToVector3(), 255)));

                int amountToShow = 6;
                this.currentMusicPackAlbum.draw(b);
                this.currentSelectedSong.draw(b);
                if (!this.selectedJustLocation)
                {
                    this.currentlySelectedOption.draw(b);
                    this.currentlySelectedWeather.draw(b);
                    this.currentlySelectedTime.draw(b);
                }

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
            }

            if (this.drawMode == DrawMode.DaySelection)
            {
                this.drawDialogueBoxBackground((int)placement.X, (int)placement.Y, (int)placement.Z, (int)placement.W, new Color(new Vector4(this.dialogueBoxBackgroundColor.ToVector3(), 255)));
                this.drawDialogueBoxBackground((int)placement2.X, (int)placement2.Y, (int)placement2.Z, (int)placement2.W, new Color(new Vector4(this.dialogueBoxBackgroundColor.ToVector3(), 255)));

                //make 3rd dialogue box option;
                this.currentMusicPackAlbum.draw(b);
                this.currentSelectedSong.draw(b);
                this.currentlySelectedOption.draw(b);
                this.currentlySelectedWeather.draw(b);
                this.currentlySelectedTime.draw(b);
                this.currentlySelectedLocation.draw(b);

                foreach (Button button in this.fancyButtons)
                    button.draw(b);

                foreach (var str in this.texturedStrings)
                    str.draw(b);
            }

            if (this.drawMode == DrawMode.NothingElseToDisplay)
            {
                this.drawDialogueBoxBackground((int)placement.X, (int)placement.Y, (int)placement.Z, (int)placement.W, new Color(new Vector4(this.dialogueBoxBackgroundColor.ToVector3(), 255)));

                //make 3rd dialogue box option;
                if (this.selectedJustLocation)
                {
                    this.currentMusicPackAlbum.draw(b);
                    this.currentSelectedSong.draw(b);
                    this.currentlySelectedOption.draw(b);
                    this.currentlySelectedLocation.draw(b);
                }
                else
                {
                    this.currentMusicPackAlbum.draw(b);
                    this.currentSelectedSong.draw(b);
                    this.currentlySelectedOption.draw(b);
                    this.currentlySelectedWeather.draw(b);
                    this.currentlySelectedTime.draw(b);
                    this.currentlySelectedLocation.draw(b);
                    this.currentlySelectedDay.draw(b);
                }
                foreach (Button button in this.fancyButtons)
                    button.draw(b);

                foreach (var str in this.texturedStrings)
                    str.draw(b);
            }

            if (this.drawMode == DrawMode.EventSelection)
            {
                this.drawDialogueBoxBackground((int)placement.X, (int)placement.Y, (int)placement.Z, (int)placement.W, new Color(new Vector4(this.dialogueBoxBackgroundColor.ToVector3(), 255)));
                this.drawDialogueBoxBackground((int)placement2.X, (int)placement2.Y, (int)placement2.Z, (int)placement2.W, new Color(new Vector4(this.dialogueBoxBackgroundColor.ToVector3(), 255)));

                int amountToShow = 6;
                //make 3rd dialogue box option;
                this.currentMusicPackAlbum.draw(b);
                this.currentSelectedSong.draw(b);
                this.currentlySelectedOption.draw(b);
                /*
                if (!doesPackContainMusic())
                {
                    this.addButton.draw(b);
                }
                else
                {
                    this.deleteButton.draw(b);
                }
                */
                this.playButton.draw(b);
                this.stopButton.draw(b);


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
            }

            if (this.drawMode == DrawMode.MenuSelection)
            {
                this.drawDialogueBoxBackground((int)placement.X, (int)placement.Y, (int)placement.Z, (int)placement.W, new Color(new Vector4(this.dialogueBoxBackgroundColor.ToVector3(), 255)));
                this.drawDialogueBoxBackground((int)placement2.X, (int)placement2.Y, (int)placement2.Z, (int)placement2.W, new Color(new Vector4(this.dialogueBoxBackgroundColor.ToVector3(), 255)));

                int amountToShow = 6;
                //make 3rd dialogue box option;
                this.currentMusicPackAlbum.draw(b);
                this.currentSelectedSong.draw(b);
                this.currentlySelectedOption.draw(b);
                /*
                if (!doesPackContainMusic())
                {
                    this.addButton.draw(b);
                }
                else
                {
                    this.deleteButton.draw(b);
                }
                */
                this.playButton.draw(b);
                this.stopButton.draw(b);

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
            }


            if (this.drawMode == DrawMode.FestivalSelection)
            {
                this.drawDialogueBoxBackground((int)placement.X, (int)placement.Y, (int)placement.Z, (int)placement.W, new Color(new Vector4(this.dialogueBoxBackgroundColor.ToVector3(), 255)));
                this.drawDialogueBoxBackground((int)placement2.X, (int)placement2.Y, (int)placement2.Z, (int)placement2.W, new Color(new Vector4(this.dialogueBoxBackgroundColor.ToVector3(), 255)));

                int amountToShow = 6;
                //make 3rd dialogue box option;
                this.currentMusicPackAlbum.draw(b);
                this.currentSelectedSong.draw(b);
                this.currentlySelectedOption.draw(b);
                /*
                if (!doesPackContainMusic())
                {
                    this.addButton.draw(b);
                }
                else
                {
                    this.deleteButton.draw(b);
                }
                */
                this.playButton.draw(b);
                this.stopButton.draw(b);

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
            }

            if (this.drawMode == DrawMode.SelectedEvent)
            {
                this.drawDialogueBoxBackground((int)placement.X, (int)placement.Y, (int)placement.Z, (int)placement.W, new Color(new Vector4(this.dialogueBoxBackgroundColor.ToVector3(), 255)));


                //make 3rd dialogue box option;
                this.currentMusicPackAlbum.draw(b);
                this.currentSelectedSong.draw(b);
                this.currentlySelectedOption.draw(b);
                this.currentlySelectedEvent.draw(b);
                this.playButton.draw(b);
                this.stopButton.draw(b);
                if (!this.doesPackContainMusic())
                    this.addButton.draw(b);
                else
                    this.deleteButton.draw(b);

                foreach (Button button in this.fancyButtons)
                    button.draw(b);

                foreach (var str in this.texturedStrings)
                    str.draw(b);
            }


            if (this.drawMode == DrawMode.SelectedFestival)
            {
                this.drawDialogueBoxBackground((int)placement.X, (int)placement.Y, (int)placement.Z, (int)placement.W, new Color(new Vector4(this.dialogueBoxBackgroundColor.ToVector3(), 255)));

                //make 3rd dialogue box option;
                this.currentMusicPackAlbum.draw(b);
                this.currentSelectedSong.draw(b);
                this.currentlySelectedOption.draw(b);
                this.currentlySelectedFestival.draw(b);
                this.playButton.draw(b);
                this.stopButton.draw(b);
                if (!this.doesPackContainMusic())
                    this.addButton.draw(b);
                else
                    this.deleteButton.draw(b);

                foreach (Button button in this.fancyButtons)
                    button.draw(b);

                foreach (var str in this.texturedStrings)
                    str.draw(b);
            }


            if (this.drawMode == DrawMode.SelectedMenu)
            {
                this.drawDialogueBoxBackground((int)placement.X, (int)placement.Y, (int)placement.Z, (int)placement.W, new Color(new Vector4(this.dialogueBoxBackgroundColor.ToVector3(), 255)));


                //make 3rd dialogue box option;
                this.currentMusicPackAlbum.draw(b);
                this.currentSelectedSong.draw(b);
                this.currentlySelectedOption.draw(b);
                this.currentlySelectedMenu.draw(b);
                this.playButton.draw(b);
                this.stopButton.draw(b);
                if (!this.doesPackContainMusic())
                    this.addButton.draw(b);
                else
                    this.deleteButton.draw(b);

                foreach (Button button in this.fancyButtons)
                    button.draw(b);

                foreach (var str in this.texturedStrings)
                    str.draw(b);
            }


            //Draw the add, delete, play, and stop buttons.
            if (this.currentSelectedSong != null && this.currentMusicPackAlbum != null && this.drawMode != DrawMode.AlbumFancySelection)
            {
                if (this.drawMode == DrawMode.WeatherSelection || this.drawMode == DrawMode.TimeSelection || this.drawMode == DrawMode.LocationSelection || this.drawMode == DrawMode.DaySelection || this.drawMode == DrawMode.NothingElseToDisplay)
                {
                    if (this.drawMode == DrawMode.LocationSelection && this.selectedJustLocation) { }
                    else
                    {
                        if (!this.doesPackContainMusic())
                            this.addButton.draw(b);
                        else
                            this.deleteButton.draw(b);
                    }
                }
                this.playButton.draw(b);
                this.stopButton.draw(b);
            }

            this.backButton.draw(b);
            this.drawMouse(b);
        }

        /// <summary>Select a album artwork and change the draw mode to go to the song selection screen.</summary>
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
            this.drawMode = DrawMode.DifferentSelectionTypesMode;
        }

        /// <summary>Plays the currently selected song.</summary>
        public void playSong()
        {
            if (StardewSymphony.Config.EnableDebugLog)
                StardewSymphony.ModMonitor.Log($"Song Selected! {this.currentSelectedSong.name}");
            MusicPack musicPack = (MusicPack)this.currentMusicPackAlbum.buttonFunctionality.leftClick.paramaters[0];
            if (StardewSymphony.Config.EnableDebugLog)
                StardewSymphony.ModMonitor.Log($"Select Pack: {musicPack.Name}");
            StardewSymphony.musicManager.SwapMusicPacks(musicPack.Name);
            StardewSymphony.musicManager.PlaySongFromCurrentPack(this.currentSelectedSong.name);
        }

        /// <summary>Stops the currently playing song.</summary>
        public void stopSong()
        {
            if (StardewSymphony.Config.EnableDebugLog)
                StardewSymphony.ModMonitor.Log($"Song Selected! {this.currentSelectedSong.name}");
            MusicPack musicPack = (MusicPack)this.currentMusicPackAlbum.buttonFunctionality.leftClick.paramaters[0];
            if (StardewSymphony.Config.EnableDebugLog)
                StardewSymphony.ModMonitor.Log($"Select Pack: {musicPack.Name}");
            StardewSymphony.musicManager.SwapMusicPacks(musicPack.Name);
            StardewSymphony.musicManager.stopSongFromCurrentMusicPack();
        }

        /// <summary>Adds a song to the trigger list so that music will play at the appropriate time.</summary>
        public void addSong()
        {
            MusicPack musicPack = (MusicPack)this.currentMusicPackAlbum.buttonFunctionality.leftClick.paramaters[0];
            //StardewSymphony.ModMonitor.Log(generateSongTriggerKeyFromSelection());
            //Add generic festival music.
            if (this.drawMode == DrawMode.SelectedFestival)
            {
                musicPack.SongInformation.addSongToFestivalList(this.currentSelectedSong.label);
                return;
            }

            //Add generic event music.
            if (this.drawMode == DrawMode.SelectedEvent)
            {
                musicPack.SongInformation.addSongToEventList(this.currentSelectedSong.label);
                return;
            }

            if (this.currentSelectedSong?.label != null)
                musicPack.SongInformation.addSongToTriggerList(this.generateSongTriggerKeyFromSelection(), this.currentSelectedSong.label);
        }

        /// <summary>Delete the song from the list of triggers.</summary>
        public void deleteSong()
        {
            MusicPack musicPack = (MusicPack)this.currentMusicPackAlbum.buttonFunctionality.leftClick.paramaters[0];

            if (this.drawMode == DrawMode.SelectedFestival)
            {
                musicPack.SongInformation.removeSongFromFestivalList(this.currentSelectedSong.label);
                return;
            }

            //Add generic event music.
            if (this.drawMode == DrawMode.SelectedEvent)
            {
                musicPack.SongInformation.removeSongFromEventList(this.currentSelectedSong.label);
                return;
            }

            musicPack.SongInformation.removeSongFromTriggerList(this.generateSongTriggerKeyFromSelection(), this.currentSelectedSong.label);
        }

        /// <summary>Generate the trigger key based on used selection.</summary>
        public string generateSongTriggerKeyFromSelection()
        {
            string key = "";
            string seperator = "_";
            //Seasonal selection region

            if (this.currentlySelectedLocation != null && this.selectedJustLocation)
            {
                if (this.currentlySelectedLocation.label.Contains("Cabin"))
                {
                    key = this.currentlySelectedLocation.name;
                    return key;
                }
                key = this.currentlySelectedLocation.label;
                return key;
            }


            if (this.currentlySelectedMenu != null)
            {
                return this.currentlySelectedMenu.label;
            }
            if (this.currentlySelectedFestival != null) return this.currentlySelectedFestival.label;
            if (this.currentlySelectedEvent != null) return this.currentlySelectedEvent.label;

            #region
            if (this.currentlySelectedOption != null)
            {
                if (this.currentlySelectedOption.name == "SpringButton" || this.currentlySelectedOption.name == "SummerButton" || this.currentlySelectedOption.name == "FallButton" || this.currentlySelectedOption.name == "WinterButton")
                {
                    if (this.currentlySelectedOption.name == "SpringButton") key += "spring";
                    else if (this.currentlySelectedOption.name == "SummerButton") key += "summer";
                    else if (this.currentlySelectedOption.name == "FallButton") key += "fall";
                    else if (this.currentlySelectedOption.name == "WinterButton") key += "winter";
                    else
                    {
                        //StardewSymphony.ModMonitor.Log("Error: You are not in a valid menu area to set the song information. Please make sure that a valid season is selected for the song options", StardewModdingAPI.LogLevel.Alert);
                        return "";
                    }
                }
                else
                {
                    //StardewSymphony.ModMonitor.Log("Error: You are not in a valid menu area to set the song information. Please make sure that a valid season is selected for the song options", StardewModdingAPI.LogLevel.Alert);
                    return "";
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
            #endregion


            return key;
        }

        /// <summary>Functionality to go back to the previous menu page.</summary>
        public void goBack()
        {
            if (this.drawMode == DrawMode.AlbumSelection || this.drawMode == DrawMode.AlbumFancySelection)
            {
                this.readyToClose();
                Game1.activeClickableMenu = null;
                return;
            }
            if (this.drawMode == DrawMode.SongSelectionMode)
            {
                this.texturedStrings.Clear();
                this.drawMode = DrawMode.AlbumFancySelection;
                this.currentMusicPackAlbum = null;
                this.updateFancyButtons();
                return;
            }

            if (this.drawMode == DrawMode.DifferentSelectionTypesMode)
            {
                this.drawMode = DrawMode.SongSelectionMode;
                this.currentSelectedSong = null;
                this.currentSongPageIndex = 0;
                this.updateFancyButtons();
                return;
            }

            if (this.drawMode == DrawMode.WeatherSelection)
            {
                this.drawMode = DrawMode.SeasonSelection;
                //Error check is already done for this in the getKey function
                this.updateFancyButtons();
                return;
            }

            if (this.drawMode == DrawMode.TimeSelection)
            {
                this.drawMode = DrawMode.WeatherSelection;
                this.currentlySelectedWeather = null;
                this.updateFancyButtons();
                return;
            }

            if (this.drawMode == DrawMode.LocationSelection)
            {
                if (this.selectedJustLocation)
                {
                    this.selectedJustLocation = false;
                    this.drawMode = DrawMode.DifferentSelectionTypesMode;
                    this.currentlySelectedOption = null;
                    this.updateFancyButtons();
                    return;
                }
                this.drawMode = DrawMode.TimeSelection;
                this.currentlySelectedTime = null;
                this.updateFancyButtons();
                return;
            }

            if (this.drawMode == DrawMode.DaySelection)
            {
                this.drawMode = DrawMode.LocationSelection;
                this.currentlySelectedLocation = null;
                this.locationPageIndex = 0;
                this.updateFancyButtons();
                return;
            }

            if (this.drawMode == DrawMode.NothingElseToDisplay)
            {
                if (this.selectedJustLocation)
                {
                    this.drawMode = DrawMode.LocationSelection;
                    this.currentlySelectedLocation = null;
                    this.updateFancyButtons();
                    return;
                }
                this.drawMode = DrawMode.DaySelection;
                this.currentlySelectedDay = null;
                this.updateFancyButtons();
                return;
            }

            if (this.drawMode == DrawMode.EventSelection || this.drawMode == DrawMode.FestivalSelection || this.drawMode == DrawMode.MenuSelection)
            {
                this.drawMode = DrawMode.DifferentSelectionTypesMode;
                this.currentlySelectedOption = null;
                this.updateFancyButtons();
                return;
            }

            if (this.drawMode == DrawMode.SelectedEvent)
            {
                this.drawMode = DrawMode.EventSelection;
                this.currentlySelectedEvent = null;
                this.updateFancyButtons();
                this.eventPageIndex = 0;
                return;
            }

            if (this.drawMode == DrawMode.SelectedFestival)
            {
                this.drawMode = DrawMode.FestivalSelection;
                this.currentlySelectedFestival = null;
                this.updateFancyButtons();
                this.festivalPageIndex = 0;
                return;
            }

            if (this.drawMode == DrawMode.SelectedMenu)
            {
                this.drawMode = DrawMode.MenuSelection;
                this.currentlySelectedMenu = null;
                this.updateFancyButtons();
                this.menuPageIndex = 0;
                return;
            }

            if (this.drawMode == DrawMode.SeasonSelection)
            {
                this.drawMode = DrawMode.DifferentSelectionTypesMode;
                this.currentlySelectedOption = null;
                this.updateFancyButtons();
                this.menuPageIndex = 0;
                return;
            }
        }
    }
}
