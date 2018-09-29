using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Locations;
using StardustCore.Animations;
using StardustCore.UIUtilities;
using StardustCore.UIUtilities.MenuComponents;
using StardustCore.UIUtilities.MenuComponents.Delegates;
using StardustCore.UIUtilities.MenuComponents.Delegates.Functionality;
using StardustCore.UIUtilities.SpriteFonts;
using StardustCore.UIUtilities.SpriteFonts.Components;

namespace StardewSymphonyRemastered.Framework.Menus
{
    /// <summary>
    /// Interface for the menu for selection music.
    /// </summary>
    public class MusicManagerMenu : IClickableMenuExtended
    {
        /// <summary>
        /// The different displays for this menu.
        /// </summary>
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
            SelectedMenu
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

        /// <summary>
        /// Empty Constructor
        /// </summary>
        public MusicManagerMenu()
        {

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public MusicManagerMenu(float width, float height)
        {
            this.width = (int)width;
            this.height = (int)height;
            this.texturedStrings = new List<StardustCore.UIUtilities.SpriteFonts.Components.TexturedString>();
            this.musicAlbumButtons = new List<StardustCore.UIUtilities.MenuComponents.Button>();
            //thismusicAlbumButtons.Add(new Button("myButton", new Rectangle(100, 100, 64, 64), StardewSymphony.textureManager.getTexture("MusicNote").Copy(StardewSymphony.ModHelper), "mynote", new Rectangle(0, 0, 16, 16), 4f, new StardustCore.Animations.Animation(new Rectangle(0, 0, 16, 16)), Color.White, Color.White,new ButtonFunctionality(new DelegatePairing(hello,null),null,null),false)); //A button that does nothing on the left click.  

            fancyButtons = new List<Button>();
            

            //Initialize music album icons.
            int numOfButtons = 0;
            int rows = 0;
            foreach(var v in StardewSymphony.musicManager.musicPacks)
            {
                var sortedQuery = v.Value.songInformation.listOfSongsWithoutTriggers.OrderBy(x => x.name);
                v.Value.songInformation.listOfSongsWithoutTriggers=sortedQuery.ToList(); //Alphabetize.
                if (v.Value.musicPackInformation.getTexture() == null)
                {
                    Texture2DExtended texture = StardewSymphony.textureManager.getTexture("MusicDisk");
                    float scale = 1.00f / ((float)texture.texture.Width / 64f);
       
                    this.musicAlbumButtons.Add(new Button(v.Key, new Rectangle(100 + (numOfButtons * 100), 125 + (rows * 100), 64, 64),texture, "", new Rectangle(0, 0, 16, 16), scale, new StardustCore.Animations.Animation(new Rectangle(0, 0, 16, 16)), StardustCore.IlluminateFramework.Colors.randomColor(), Color.White,new ButtonFunctionality(new DelegatePairing(null, new List<object>
                    {
                        (object)v
                    }
                    ), null, new DelegatePairing(null, new List<object>(){
                    (object)v
                    }
                    )), false));
                }
                else
                {
                    float scale = 1.00f / ((float)v.Value.musicPackInformation.getTexture().texture.Width / 64f);
                    this.musicAlbumButtons.Add(new Button(v.Key, new Rectangle(100 + (numOfButtons * 100), 125 + (rows * 100), 64, 64), v.Value.musicPackInformation.getTexture(), "", new Rectangle(0, 0, v.Value.musicPackInformation.getTexture().texture.Width, v.Value.musicPackInformation.getTexture().texture.Height), scale, new StardustCore.Animations.Animation(new Rectangle(0, 0, 16, 16)), StardustCore.IlluminateFramework.LightColorsList.Black, StardustCore.IlluminateFramework.LightColorsList.Black, new ButtonFunctionality(new DelegatePairing(null, new List<object>
                    {
                        (object)v
                    }
                    ), null, new DelegatePairing(null, new List<object>(){
                    (object)v
                    }
                    )), false));
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
            if (Game1.timeOfDay >= 1200&& Game1.timeOfDay < 1800) this.dialogueBoxBackgroundColor = Color.White;
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

            Vector2 playPos= new Vector2(this.width * .1f + 128 + 32, this.height * .05f + 128); //Put it to the right of the music disk
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

        /// <summary>
        /// Runs every game tick to check for stuff.
        /// </summary>
        /// <param name="time"></param>
        public override void update(GameTime time)
        {
            int updateNumber = 20;
            if (this.drawMode == DrawMode.AlbumFancySelection)
            {
                if (framesSinceLastUpdate == updateNumber)
                {
                    var state = Microsoft.Xna.Framework.Input.Keyboard.GetState();
                    if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Left) || state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.A))
                    {
                        this.currentAlbumIndex--;
                        if (this.currentAlbumIndex < 0) this.currentAlbumIndex = this.musicAlbumButtons.Count - 1;
                        this.updateFancyButtons();
                        this.framesSinceLastUpdate = 0;
                        Game1.playSound("shwip");
                    }

                    if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Right) || state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.D))
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
                if (framesSinceLastUpdate == updateNumber)
                {
                    var state = Microsoft.Xna.Framework.Input.Keyboard.GetState();
                    if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Left) || state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.A))
                    {
                        if (this.currentSongPageIndex > 0)
                        {
                            this.currentSongPageIndex--;
                        }
                        this.updateFancyButtons();
                        this.framesSinceLastUpdate = 0;
                        Game1.playSound("shwip");
                    }

                    if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Right) || state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.D))
                    {
                        this.currentSongPageIndex++;
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

            if (this.drawMode == DrawMode.LocationSelection)
            {
                if (framesSinceLastUpdate == updateNumber)
                {
                    var state = Microsoft.Xna.Framework.Input.Keyboard.GetState();
                    if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Left) || state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.A))
                    {
                        if (this.locationPageIndex > 0)
                        {
                            this.locationPageIndex--;
                        }
                        this.updateFancyButtons();
                        this.framesSinceLastUpdate = 0;
                        Game1.playSound("shwip");
                    }

                    if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Right) || state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.D))
                    {
                        this.locationPageIndex++;
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

            if (this.drawMode == DrawMode.FestivalSelection)
            {
                if (framesSinceLastUpdate == updateNumber)
                {
                    var state = Microsoft.Xna.Framework.Input.Keyboard.GetState();
                    if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Left) || state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.A))
                    {
                        if (this.festivalPageIndex > 0)
                        {
                            this.festivalPageIndex--;
                        }
                        this.updateFancyButtons();
                        this.framesSinceLastUpdate = 0;
                        Game1.playSound("shwip");
                    }

                    if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Right) || state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.D))
                    {
                        this.festivalPageIndex++;
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

            if (this.drawMode == DrawMode.EventSelection)
            {
                if (framesSinceLastUpdate == updateNumber)
                {
                    var state = Microsoft.Xna.Framework.Input.Keyboard.GetState();
                    if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Left) || state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.A))
                    {
                        if (this.eventPageIndex > 0)
                        {
                            this.eventPageIndex--;
                        }
                        this.updateFancyButtons();
                        this.framesSinceLastUpdate = 0;
                        Game1.playSound("shwip");
                    }

                    if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Right) || state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.D))
                    {
                        this.eventPageIndex++;
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

            if (this.drawMode == DrawMode.MenuSelection)
            {
                if (framesSinceLastUpdate == updateNumber)
                {
                    var state = Microsoft.Xna.Framework.Input.Keyboard.GetState();
                    if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Left) || state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.A))
                    {
                        if (this.menuPageIndex > 0)
                        {
                            this.menuPageIndex--;
                        }
                        this.updateFancyButtons();
                        this.framesSinceLastUpdate = 0;
                        Game1.playSound("shwip");
                    }

                    if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Right) || state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.D))
                    {
                        this.menuPageIndex++;
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
        }


        /// <summary>
        /// Update the position of the album artwork when displaying it using the fancy buttons menu.
        /// </summary>
        public virtual void updateFancyButtons()
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
                            Button button = this.musicAlbumButtons.ElementAt(Math.Abs((this.currentAlbumIndex + i)%this.musicAlbumButtons.Count)).clone();
                            button.bounds = new Rectangle((int)placement.X + (i * 100) + offsetX, (int)placement.Y, 64, 64);
                            fancyButtons.Add(button);
                        }
                        catch (Exception err)
                        {
                            err.ToString();
                            if (this.currentAlbumIndex + i == 0)
                            {
                                Button button = this.musicAlbumButtons.ElementAt(Math.Abs(0 % this.musicAlbumButtons.Count)).clone();
                                button.bounds = new Rectangle((int)placement.X + (i * 100) + offsetX, (int)placement.Y, 64, 64);
                                fancyButtons.Add(button);
                            }
                            else
                            {

                                try
                                {
                                    Button button = this.musicAlbumButtons.ElementAt(Math.Abs(((this.currentAlbumIndex + i) - this.musicAlbumButtons.Count) % this.musicAlbumButtons.Count)).clone();
                                    button.bounds = new Rectangle((int)placement.X + (i * 100) + offsetX, (int)placement.Y, 64, 64);
                                    fancyButtons.Add(button);
                                }
                                catch (Exception err2)
                                {
                                    err2.ToString();
                                    Button button = this.musicAlbumButtons.ElementAt(Math.Abs((this.currentAlbumIndex + i) + this.musicAlbumButtons.Count) % this.musicAlbumButtons.Count).clone();
                                    button.bounds = new Rectangle((int)placement.X + (i * 100) + offsetX, (int)placement.Y, 64, 64);
                                    fancyButtons.Add(button);

                                }
                            }
                        }
                    }
                    this.fancyButtons.Add(new Button("Outline", new Rectangle((int)placement.X + offsetX - 16, (int)placement.Y - 16, 64, 64), StardewSymphony.textureManager.getTexture("OutlineBox"), "", new Rectangle(0, 0, 16, 16), 6f, new StardustCore.Animations.Animation(new Rectangle(0, 0, 16, 16)), Color.White, Color.White, new ButtonFunctionality(null, null, new DelegatePairing(null, new List<object>())), false));
                    int count = 0;
                    foreach (var v in fancyButtons)
                    {

                        if (count == 3)
                        {
                            var pair = (KeyValuePair<string, MusicPack>)fancyButtons.ElementAt(count).buttonFunctionality.hover.paramaters[0];
                            //v.hoverText = (string)pair.Key;
                            //Do something like current album name =
                            this.texturedStrings.Clear();
                            this.texturedStrings.Add(SpriteFonts.vanillaFont.ParseString("Current Album Name:" + (string)pair.Key, new Microsoft.Xna.Framework.Vector2(v.bounds.X / 2, v.bounds.Y + 128), v.textColor));
                            v.hoverText = "";
                        }
                        count++;
                    }
                }
            }

            //Song selection mode.
            if(this.drawMode == DrawMode.SongSelectionMode)
            {
                this.fancyButtons.Clear();
                //Vector4 placement = new Vector4((Game1.viewport.Width / 3), (Game1.viewport.Height / 4) + 128, this.width, this.height / 2);
                var info = (KeyValuePair<string, MusicPack>)this.currentMusicPackAlbum.buttonFunctionality.leftClick.paramaters[0];
                var musicPackSongList = info.Value.songInformation.listOfSongsWithoutTriggers;

                Vector4 placement2 = new Vector4(this.width * .2f + 400, this.height * .05f, 5 * 100, this.height * .9f);
                for (int i = 0; i < musicPackSongList.Count; i++)
                {

                        //Allow 8 songs to be displayed per page.
                        Texture2DExtended texture = StardewSymphony.textureManager.getTexture("MusicNote");
                        float scale = 1.00f / ((float)texture.texture.Width / 64f);
                        Song s = musicPackSongList.ElementAt(i);
                        Rectangle srcRect = new Rectangle(0, 0, texture.texture.Width, texture.texture.Height);
                        this.fancyButtons.Add(new Button(s.name, new Rectangle((int)placement2.X+25, (int)placement2.Y + ((i%6) * 100)+100, 64, 64), texture, s.name, srcRect, scale, new Animation(srcRect), Color.White, Color.White, new ButtonFunctionality(null, null, null)));
                    
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
                    float scale = 1.00f / ((float)texture.texture.Height / 64f);
                    Rectangle srcRect = new Rectangle(0, 0, texture.texture.Width, texture.texture.Height);
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
                    float scale = 1.00f / ((float)texture.texture.Width / 64f);
                    Rectangle srcRect = new Rectangle(0, 0, texture.texture.Width, texture.texture.Height);
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
                    float scale = 1.00f / ((float)texture.texture.Width / 64f);
                    Rectangle srcRect = new Rectangle(0, 0, texture.texture.Width, texture.texture.Height);
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
                if (Game1.currentSeason == "spring")
                {

                    Texture2DExtended springTexture = StardewSymphony.textureManager.getTexture("SpringIcon");
                    if (springTexture == null)
                    {
                        if (StardewSymphony.Config.EnableDebugLog)
                            StardewSymphony.ModMonitor.Log("SPRING TEXTURE NULL!");
                        return;
                    }
                    float scale = 1.00f / ((float)springTexture.texture.Width / 64f);
                    Rectangle srcRect = new Rectangle(0, 0, springTexture.texture.Width, springTexture.texture.Height);
                    this.fancyButtons.Add(new Button("SeasonIcon", new Rectangle((int)seasonPlacement.X, (int)seasonPlacement.Y, 64, 64), springTexture, "Spring Music", srcRect, scale, new Animation(srcRect), Color.White, Color.White, new ButtonFunctionality(null, null, null)));
                }
                if (Game1.currentSeason == "summer")
                {

                    Texture2DExtended summerTexture = StardewSymphony.textureManager.getTexture("SummerIcon");
                    if (summerTexture == null)
                    {
                        if (StardewSymphony.Config.EnableDebugLog)
                            StardewSymphony.ModMonitor.Log("SUMMER TEXTURE NULL!");
                        return;
                    }
                    float scale = 1.00f / ((float)summerTexture.texture.Width / 64f);
                    Rectangle srcRect = new Rectangle(0, 0, summerTexture.texture.Width, summerTexture.texture.Height);
                    this.fancyButtons.Add(new Button("SeasonIcon", new Rectangle((int)seasonPlacement.X, (int)seasonPlacement.Y, 64, 64), summerTexture, "Summer Music", srcRect, scale, new Animation(srcRect), Color.White, Color.White, new ButtonFunctionality(null, null, null)));
                }
                if (Game1.currentSeason == "fall")
                {

                    Texture2DExtended fallTexture = StardewSymphony.textureManager.getTexture("FallIcon");
                    if (fallTexture == null)
                    {
                        if (StardewSymphony.Config.EnableDebugLog)
                            StardewSymphony.ModMonitor.Log("FALL TEXTURE NULL!");
                        return;
                    }
                    float scale = 1.00f / ((float)fallTexture.texture.Width / 64f);
                    Rectangle srcRect = new Rectangle(0, 0, fallTexture.texture.Width, fallTexture.texture.Height);
                    this.fancyButtons.Add(new Button("SeasonIcon", new Rectangle((int)seasonPlacement.X, (int)seasonPlacement.Y, 64, 64), fallTexture, "Fall Music", srcRect, scale, new Animation(srcRect), Color.White, Color.White, new ButtonFunctionality(null, null, null)));
                }
                if (Game1.currentSeason == "winter")
                {

                    Texture2DExtended winterTexture = StardewSymphony.textureManager.getTexture("WinterIcon");
                    if (winterTexture == null)
                    {
                        if (StardewSymphony.Config.EnableDebugLog)
                            StardewSymphony.ModMonitor.Log("WINTER TEXTURE NULL!");
                        return;
                    }
                    float scale = 1.00f / ((float)winterTexture.texture.Width / 64f);
                    Rectangle srcRect = new Rectangle(0, 0, winterTexture.texture.Width, winterTexture.texture.Height);
                    this.fancyButtons.Add(new Button("SeasonIcon", new Rectangle((int)seasonPlacement.X, (int)seasonPlacement.Y, 64, 64), winterTexture, "Winter Music", srcRect, scale, new Animation(srcRect), Color.White, Color.White, new ButtonFunctionality(null, null, null)));
                }

                Vector4 festivalPlacement = new Vector4(this.width * .2f + buttonXPosition, this.height * .35f, 6 * 100, this.height * .9f);
                Vector4 eventPlacement = new Vector4(this.width * .2f + buttonXPosition, this.height * .45f, 7 * 100, this.height * .9f);
                Vector4 menuPlacement = new Vector4(this.width * .2f + buttonXPosition, this.height * .55f, 8 * 100, this.height * .9f);

                //Festival Icon placement.
                Texture2DExtended festivalTexture = StardewSymphony.textureManager.getTexture("FestivalIcon");
                float festivalScale = 1.00f / ((float)festivalTexture.texture.Width / 64f);
                Rectangle festivalSrcRect = new Rectangle(0, 0, festivalTexture.texture.Width, festivalTexture.texture.Height);
                this.fancyButtons.Add(new Button("FestivalIcon", new Rectangle((int)festivalPlacement.X, (int)festivalPlacement.Y, 64, 64), festivalTexture, "Festival Music", festivalSrcRect, festivalScale, new Animation(festivalSrcRect), Color.White, Color.White, new ButtonFunctionality(null, null, null)));
                if (festivalTexture == null)
                {
                    if (StardewSymphony.Config.EnableDebugLog)
                        StardewSymphony.ModMonitor.Log("FESTIVAL TEXTURE NULL!");
                    return;
                }

                //Event Icon placement.
                Texture2DExtended eventTexture = StardewSymphony.textureManager.getTexture("EventIcon");
                float eventScale = 1.00f / ((float)eventTexture.texture.Width / 64f);
                Rectangle eventSrcRectangle = new Rectangle(0, 0, eventTexture.texture.Width, eventTexture.texture.Height);
                this.fancyButtons.Add(new Button("EventIcon", new Rectangle((int)eventPlacement.X, (int)eventPlacement.Y, 64, 64), eventTexture, "Event Music", eventSrcRectangle, eventScale, new Animation(eventSrcRectangle), Color.White, Color.White, new ButtonFunctionality(null, null, null)));

                if (eventTexture == null)
                {
                    if (StardewSymphony.Config.EnableDebugLog)
                        StardewSymphony.ModMonitor.Log("EVENT TEXTURE NULL!");
                    return;
                }

                //Menu Icon placement.
                Texture2DExtended menuTexture = StardewSymphony.textureManager.getTexture("MenuIcon");
                float menuScale = 1.00f / ((float)menuTexture.texture.Width / 64f);
                Rectangle menuSrcRectangle = new Rectangle(0, 0, menuTexture.texture.Width, menuTexture.texture.Height);
                this.fancyButtons.Add(new Button("MenuIcon", new Rectangle((int)menuPlacement.X, (int)menuPlacement.Y, 64, 64), menuTexture, "Menu Music", menuSrcRectangle, menuScale, new Animation(menuSrcRectangle), Color.White, Color.White, new ButtonFunctionality(null, null, null)));

                if (menuTexture == null)
                {
                    if (StardewSymphony.Config.EnableDebugLog)
                        StardewSymphony.ModMonitor.Log("MENU TEXTURE NULL!");
                    return;
                }
            }


            if(this.drawMode == DrawMode.WeatherSelection)
            {
                this.fancyButtons.Clear();

                Vector4 placement2 = new Vector4(this.width * .2f + 400, this.height * .05f, 5 * 100, this.height * .9f);
                for (int i = 0; i < 7; i++)
                {

                    //Allow 8 songs to be displayed per page.
                    Texture2DExtended texture=new Texture2DExtended();
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

                    float scale = 1.00f / ((float)texture.texture.Width / 64f);
                    Rectangle srcRect = new Rectangle(0, 0, texture.texture.Width, texture.texture.Height);
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
                    float scale = 1.00f / ((float)texture.texture.Width / 64f);
                    Rectangle srcRect = new Rectangle(0, 0, texture.texture.Width, texture.texture.Height);
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
                                float scale2 = 1.00f / ((float)texture2.texture.Width / 64f);
                                Rectangle srcRect2 = new Rectangle(0, 0, texture2.texture.Width, texture2.texture.Height);
                                this.fancyButtons.Add(new Button(locName, new Rectangle((int)placement2.X + 25, (int)placement2.Y + ((i % 6) * 100) + 100, 64, 64), texture2, displayName, srcRect2, scale2, new Animation(srcRect2), Color.White, Color.White, new ButtonFunctionality(null, null, null)));
                                continue;
                            }
                            if (farmer == null)
                            {
                                string displayName = "Empty Cabin "+(numOfEmptyCabin);
                                numOfEmptyCabin++;
                                Texture2DExtended texture2 = StardewSymphony.textureManager.getTexture("HouseIcon");
                                float scale2 = 1.00f / ((float)texture2.texture.Width / 64f);
                                Rectangle srcRect2 = new Rectangle(0, 0, texture2.texture.Width, texture2.texture.Height);
                                this.fancyButtons.Add(new Button(locName, new Rectangle((int)placement2.X + 25, (int)placement2.Y + ((i % 6) * 100) + 100, 64, 64), texture2, displayName, srcRect2, scale2, new Animation(srcRect2), Color.White, Color.White, new ButtonFunctionality(null, null, null)));
                                continue;
                            }
                        }
                        catch(Exception err)
                        {

                        }
                    }


                    //Allow 8 songs to be displayed per page.
                    Texture2DExtended texture = StardewSymphony.textureManager.getTexture("HouseIcon");
                    float scale = 1.00f / ((float)texture.texture.Width / 64f);
                    Rectangle srcRect = new Rectangle(0, 0, texture.texture.Width, texture.texture.Height);
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

                    float scale = 1.00f / ((float)texture.texture.Width / 64f);
                    Rectangle srcRect = new Rectangle(0, 0, texture.texture.Width, texture.texture.Height);
                    this.fancyButtons.Add(new Button(name, new Rectangle((int)placement2.X + 50, (int)placement2.Y + ((i % 7) * 100), 64, 64), texture, display, srcRect, scale, new Animation(srcRect), Color.White, Color.White, new ButtonFunctionality(null, null, null)));

                }
            }

            if (this.drawMode == DrawMode.NothingElseToDisplay || this.drawMode==DrawMode.SelectedEvent || this.drawMode==DrawMode.SelectedMenu || this.drawMode==DrawMode.SelectedFestival)
            {
                this.fancyButtons.Clear();
            }
        }

        /// <summary>
        /// Functionality that occurs when right clicking a menu component.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="playSound"></param>
        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            if (this.drawMode == DrawMode.AlbumSelection)
            {
                foreach (var v in this.musicAlbumButtons)
                {
                    if (v.containsPoint(x, y)) v.onRightClick();
                }
            }

            if (this.drawMode == DrawMode.AlbumFancySelection)
            {
                int count = 0;
                foreach (var v in this.fancyButtons)
                {
                    count++;
                    //if (v.containsPoint(x, y)) v.onRightClick();
                    //this.currentAlbumIndex += count;
                    //if (this.currentAlbumIndex >= this.musicAlbumButtons.Count) this.currentAlbumIndex -= this.musicAlbumButtons.Count;
                    //this.updateFancyButtons();
                }
            }
        }

        /// <summary>
        /// Actions that occur when hovering over an icon.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public override void performHoverAction(int x, int y)
        {
            if (this.drawMode == DrawMode.AlbumSelection)
            {
                foreach (var v in this.musicAlbumButtons)
                {
                    if (v.containsPoint(x, y))
                    {
                        var pair = (KeyValuePair<string, MusicPack>)v.buttonFunctionality.hover.paramaters[0];
                        v.hoverText = (string)pair.Key;
                        v.onHover();
                        //StardewSymphony.ModMonitor.Log(pair.Key);
                    }
                    else
                    {
                        v.hoverText = "";
                    }
                }
            }

            if (this.drawMode == DrawMode.AlbumFancySelection)
            {
                int count = 0;
                foreach (var v in this.fancyButtons)
                {
                    if (v.containsPoint(x, y))
                    {
                        if (v == null)
                        {
                            continue;
                        }
                        if (v.buttonFunctionality == null)
                        {
                            continue;
                        }
                        if (v.buttonFunctionality.hover == null)
                        {
                            continue;
                        }
                        if (v.buttonFunctionality.hover.paramaters == null)
                        {
                            continue;
                        }
                        if (v.buttonFunctionality.hover.paramaters.Count==0)
                        {
                            continue;
                        }
                        var pair = (KeyValuePair<string, MusicPack>)v.buttonFunctionality.hover.paramaters[0];
                        v.hoverText = (string)pair.Key;
                        v.onHover();
                    }
                    else
                    {

                            v.hoverText = "";
                    }
                    count++;
                }

                
            }
        }

        /// <summary>
        /// Functionality that occurs when left clicking a menu component.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="playSound"></param>
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            bool buttonSelected = false;


            if(this.currentSelectedSong!=null && this.currentMusicPackAlbum!=null && this.playButton.containsPoint(x, y))
            {
                Game1.playSound("coin");
                playSong();
                return;
            }

            if (this.currentSelectedSong != null && this.currentMusicPackAlbum != null && this.stopButton.containsPoint(x, y))
            {
                Game1.playSound("coin");
                stopSong();
                return;
            }

            if (this.currentSelectedSong != null && this.currentMusicPackAlbum != null && this.addButton.containsPoint(x, y))
            {
                Game1.playSound("coin");
                addSong();
                return;
            }

            if (this.currentSelectedSong != null && this.currentMusicPackAlbum != null && this.deleteButton.containsPoint(x, y))
            {
                Game1.playSound("coin");
                deleteSong();
                return;
            }

            if (this.backButton.containsPoint(x, y))
            {
                Game1.playSound("coin");
                goBack();
                return;
            }


            if (this.drawMode == DrawMode.AlbumSelection)
            {
                foreach (var v in this.musicAlbumButtons)
                {
                    if (v.containsPoint(x, y))
                    {
                        Game1.playSound("coin");
                        this.selectAlbum(v);
                        v.onLeftClick();
                    }
                }
                return;
            }

            if (this.drawMode == DrawMode.AlbumFancySelection)
            {
                int count = 0;
                Button ok = Button.Empty();
                foreach (var v in this.fancyButtons)
                {
                    if (v.containsPoint(x, y) && v.buttonFunctionality.leftClick != null)
                    {
                        Game1.playSound("coin");
                        v.onLeftClick();
                        this.currentAlbumIndex += count - 3;
                        while (currentAlbumIndex < 0)
                        {
                            this.currentAlbumIndex = (this.musicAlbumButtons.Count - (this.currentAlbumIndex * -1));
                        }
                        ok = v;
                    }
                    if (v.buttonFunctionality.leftClick != null)
                    {
                        count++;
                    }
                }
                this.selectAlbum(ok);
                return;
            }

            if (this.drawMode == DrawMode.SongSelectionMode)
            {
                Button ok = Button.Empty();
                int amountToShow = 6;
                this.updateFancyButtons();

                int count = this.fancyButtons.Count - 1;
                int amount = 0;
                if (0 + ((this.currentSongPageIndex + 1) * amountToShow) >= this.fancyButtons.Count)
                {
                    amount = (0 + ((this.currentSongPageIndex + 1) * (amountToShow)) - fancyButtons.Count);
                    amount = amountToShow - amount;
                    if (amount < 0) amount = 0;
                }
                else if (this.fancyButtons.Count < amountToShow)
                {
                    amount = this.fancyButtons.Count;
                }
                else
                {
                    amount = amountToShow;
                }
                if (amount == 0 && this.currentSongPageIndex > 1)
                {
                    this.currentSongPageIndex--;
                }
                var drawList = this.fancyButtons.GetRange(0 + (this.currentSongPageIndex * (amountToShow)), amount);


                bool songSelected = false;
                //Get a list of components to draw. And if I click one select the song.
                foreach (var v in drawList)
                {
                    if (v.containsPoint(x, y))
                    {
                        Game1.playSound("coin");
                        selectSong(v);
                        songSelected = true;
                    }
                }
                if (songSelected == true)
                {
                    this.updateFancyButtons();
                }
                return;
            }

            //Left click an option.
            if(this.drawMode == DrawMode.DifferentSelectionTypesMode)
            {
                foreach(var button in this.fancyButtons)
                {
                    if (button.containsPoint(x, y))
                    {
                        if (button == null) continue;
                        Vector2 position = new Vector2(this.width * .1f + 64, this.height * .05f + 320);
                        if (button.name == "SeasonIcon")
                        {
                            this.currentlySelectedOption = button.clone(position);
                            this.drawMode = DrawMode.WeatherSelection;
                            buttonSelected = true;
                        }
                        if (button.name == "FestivalIcon")
                        {
                            this.currentlySelectedOption = button.clone(position);
                            this.drawMode = DrawMode.FestivalSelection;
                            buttonSelected = true;

                        }
                        if (button.name == "EventIcon")
                        {
                            this.currentlySelectedOption = button.clone(position);
                            this.drawMode = DrawMode.EventSelection;
                            buttonSelected = true;
                        }
                        if (button.name == "MenuIcon")
                        {
                            this.currentlySelectedOption = button.clone(position);
                            this.drawMode = DrawMode.MenuSelection;
                            buttonSelected = true;
                        }
                    }
                }
                if (buttonSelected == true)
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
                        if (button == null) continue;
                        Vector2 position = new Vector2(this.width * .1f + 64, this.height * .05f + 384);
                        if (button.name == "SunnyIcon" || button.name == "RainyIcon" || button.name == "SnowIcon" || button.name == "WeatherDebrisIcon" || button.name == "StormIcon" || button.name == "WeatherFestivalIcon" || button.name == "WeddingIcon")
                        {
                            this.currentlySelectedWeather = button.clone(position);
                            this.drawMode = DrawMode.TimeSelection;
                            buttonSelected = true;
                        }
                    }                    
                }
                if (buttonSelected == true)
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
                        if (button == null) continue;
                        Vector2 position = new Vector2(this.width * .1f + 64, this.height * .05f + 448);
                        if (button.name == "DayIcon" || button.name == "NightIcon" )
                        {
                            this.currentlySelectedTime = button.clone(position);
                            this.drawMode = DrawMode.LocationSelection;
                            buttonSelected = true;
                        }
                    }
                }
                if (buttonSelected == true)
                {
                    Game1.playSound("coin");
                    this.updateFancyButtons();
                }
                return;
            }

            //Left click an option.
            if (this.drawMode == DrawMode.LocationSelection)
            {
                Button ok = Button.Empty();
                int amountToShow = 6;
                this.updateFancyButtons();

                int count = this.fancyButtons.Count - 1;
                int amount = 0;
                if (0 + ((this.locationPageIndex + 1) * amountToShow) >= this.fancyButtons.Count)
                {
                    amount = (0 + ((this.locationPageIndex + 1) * (amountToShow)) - fancyButtons.Count);
                    amount = amountToShow - amount;
                    if (amount < 0) amount = 0;
                }
                else if (this.fancyButtons.Count < amountToShow)
                {
                    amount = this.fancyButtons.Count;
                }
                else
                {
                    amount = amountToShow;
                }
                if (amount == 0 && this.locationPageIndex > 1)
                {
                    this.locationPageIndex--;
                }
                var drawList = this.fancyButtons.GetRange(0 + (this.locationPageIndex * (amountToShow)), amount);


                bool songSelected = false;
                //Get a list of components to draw. And if I click one select the song.
                foreach (var v in drawList)
                {
                    if (v.containsPoint(x, y))
                    {
                        Vector2 position = new Vector2(this.width * .1f + 64, this.height * .05f + 512);
                        this.currentlySelectedLocation = v.clone(position);
                        songSelected = true;
                        this.drawMode = DrawMode.DaySelection;
                    }
                }
                if (songSelected == true)
                {
                    Game1.playSound("coin");
                    this.updateFancyButtons();
                }
                return;
            }


            if (this.drawMode == DrawMode.FestivalSelection)
            {
                Button ok = Button.Empty();
                int amountToShow = 6;
                this.updateFancyButtons();

                int count = this.fancyButtons.Count - 1;
                int amount = 0;
                if (0 + ((this.festivalPageIndex + 1) * amountToShow) >= this.fancyButtons.Count)
                {
                    amount = (0 + ((this.festivalPageIndex + 1) * (amountToShow)) - fancyButtons.Count);
                    amount = amountToShow - amount;
                    if (amount < 0) amount = 0;
                }
                else if (this.fancyButtons.Count < amountToShow)
                {
                    amount = this.fancyButtons.Count;
                }
                else
                {
                    amount = amountToShow;
                }
                if (amount == 0 && this.festivalPageIndex > 1)
                {
                    this.festivalPageIndex--;
                }
                var drawList = this.fancyButtons.GetRange(0 + (this.festivalPageIndex * (amountToShow)), amount);


                bool songSelected = false;
                //Get a list of components to draw. And if I click one select the song.
                foreach (var v in drawList)
                {
                    if (v.containsPoint(x, y))
                    {
                        Vector2 position = new Vector2(this.width * .1f + 64, this.height * .05f + 384);
                        this.currentlySelectedFestival = v.clone(position);
                        songSelected = true;
                        this.drawMode = DrawMode.SelectedFestival;
                    }
                }
                if (songSelected == true)
                {
                    Game1.playSound("coin");
                    this.updateFancyButtons();
                }
                return;
            }

            if (this.drawMode == DrawMode.MenuSelection)
            {
                Button ok = Button.Empty();
                int amountToShow = 6;
                this.updateFancyButtons();

                int count = this.fancyButtons.Count - 1;
                int amount = 0;
                if (0 + ((this.menuPageIndex + 1) * amountToShow) >= this.fancyButtons.Count)
                {
                    amount = (0 + ((this.menuPageIndex + 1) * (amountToShow)) - fancyButtons.Count);
                    amount = amountToShow - amount;
                    if (amount < 0) amount = 0;
                }
                else if (this.fancyButtons.Count < amountToShow)
                {
                    amount = this.fancyButtons.Count;
                }
                else
                {
                    amount = amountToShow;
                }
                if (amount == 0 && this.menuPageIndex > 1)
                {
                    this.menuPageIndex--;
                }
                var drawList = this.fancyButtons.GetRange(0 + (this.menuPageIndex * (amountToShow)), amount);


                bool songSelected = false;
                //Get a list of components to draw. And if I click one select the song.
                foreach (var v in drawList)
                {
                    if (v.containsPoint(x, y))
                    {
                        Vector2 position = new Vector2(this.width * .1f + 64, this.height * .05f + 384);
                        this.currentlySelectedMenu = v.clone(position);
                        songSelected = true;
                        this.drawMode = DrawMode.SelectedMenu;
                    }
                }
                if (songSelected == true)
                {
                    Game1.playSound("coin");
                    this.updateFancyButtons();
                }
                return;
            }

            if (this.drawMode == DrawMode.EventSelection)
            {
                Button ok = Button.Empty();
                int amountToShow = 6;
                this.updateFancyButtons();

                int count = this.fancyButtons.Count - 1;
                int amount = 0;
                if (0 + ((this.eventPageIndex + 1) * amountToShow) >= this.fancyButtons.Count)
                {
                    amount = (0 + ((this.eventPageIndex + 1) * (amountToShow)) - fancyButtons.Count);
                    amount = amountToShow - amount;
                    if (amount < 0) amount = 0;
                }
                else if (this.fancyButtons.Count < amountToShow)
                {
                    amount = this.fancyButtons.Count;
                }
                else
                {
                    amount = amountToShow;
                }
                if (amount == 0 && this.eventPageIndex > 1)
                {
                    this.eventPageIndex--;
                }
                var drawList = this.fancyButtons.GetRange(0 + (this.eventPageIndex * (amountToShow)), amount);


                bool songSelected = false;
                //Get a list of components to draw. And if I click one select the song.
                foreach (var v in drawList)
                {
                    if (v.containsPoint(x, y))
                    {
                        Vector2 position = new Vector2(this.width * .1f + 64, this.height * .05f + 384);
                        this.currentlySelectedEvent = v.clone(position);
                        songSelected = true;
                        this.drawMode = DrawMode.SelectedEvent;
                    }
                }
                if (songSelected == true)
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
                        if(button.name!="SundayIcon"|| button.name != "MondayIcon" || button.name != "TuesdayIcon" || button.name != "WednesdayIcon" || button.name != "ThursdayIcon" || button.name != "FridayIcon" || button.name != "SaturdayIcon")
                        if (button == null) continue;
                        Vector2 position = new Vector2(this.width * .1f + 64, this.height * .05f + 576);
                        //Get any valid location button.
                        this.currentlySelectedDay = button.clone(position);
                        this.drawMode = DrawMode.NothingElseToDisplay;
                        buttonSelected = true;
                    }
                }
                if (buttonSelected == true)
                {
                    Game1.playSound("coin");
                    this.updateFancyButtons();
                }
                return;
            }


        }

        /// <summary>
        /// Checks whether or not the currently selected song has already been added for the currently selected music pack.
        /// </summary>
        /// <returns>Whether or not the currently selected song has been added to the list of song triggers for the music pack.</returns>
        public bool doesPackContainMusic()
        {
            if (this.currentMusicPackAlbum == null || this.currentSelectedSong == null) return false;
            var info = (KeyValuePair<string, MusicPack>)this.currentMusicPackAlbum.buttonFunctionality.leftClick.paramaters[0];
            //Check for generic festival music.
            if (this.drawMode == DrawMode.FestivalSelection)
            {
                var festivalSonglist = info.Value.songInformation.festivalSongs;
                if (festivalSonglist == null) return false;
                if (!festivalSonglist.Contains(info.Value.songInformation.getSongFromList(festivalSonglist, this.currentSelectedSong.name))) return false;
                else return true;
            }
            //Check for generic event music.
            if (this.drawMode == DrawMode.EventSelection)
            {
                var eventSonglist = info.Value.songInformation.festivalSongs;
                if (eventSonglist == null) return false;
                if (!eventSonglist.Contains(info.Value.songInformation.getSongFromList(eventSonglist, this.currentSelectedSong.name))) return false;
                else return true;
            }
            //Check for seasonal music triggers.
            var songList = info.Value.songInformation.getSongList(generateSongTriggerKeyFromSelection());
            if (songList.Value == null) return false;
            if (!songList.Value.Contains(info.Value.songInformation.getSongFromList(songList.Value, this.currentSelectedSong.name))) return false;
            else return true;

        }

        /// <summary>
        /// Draws the menu and it's respective components depending on the drawmode that is currently set.
        /// </summary>
        /// <param name="b"></param>
        public override void draw(SpriteBatch b)
        {
            Vector4 placement = new Vector4(this.width * .1f, this.height * .05f - 96, 4 * 100 + 50, this.height + 32);

            Vector4 placement2 = new Vector4(this.width * .2f + 400, this.height * .05f - 96, 5 * 100, this.height + 32);
          


            if (this.drawMode == DrawMode.AlbumSelection)
            {
                this.drawDialogueBoxBackground();
                foreach (var v in this.musicAlbumButtons)
                {
                    v.draw(b);
                }
            }

            if (this.drawMode == DrawMode.AlbumFancySelection)
            {
                Vector4 placement3 = new Vector4(Game1.viewport.Width/4-50, Game1.viewport.Height/4, 8 * 100, 128*2);
                this.drawDialogueBoxBackground((int)placement3.X, (int)placement3.Y, (int)placement3.Z, (int)placement3.W, new Color(new Vector4(this.dialogueBoxBackgroundColor.ToVector3(), 0)));
               
                foreach(var v in fancyButtons)
                {
                    v.draw(b);
                }
                foreach (var v in this.texturedStrings)
                {
                        v.draw(b);
                }
            }

            if (this.drawMode == DrawMode.SongSelectionMode)
            {
                this.drawDialogueBoxBackground((int)placement.X, (int)placement.Y, (int)placement.Z, (int)placement.W, new Color(new Vector4(this.dialogueBoxBackgroundColor.ToVector3(), 255)));
                this.drawDialogueBoxBackground((int)placement2.X, (int)placement2.Y, (int)placement2.Z, (int)placement2.W, new Color(new Vector4(this.dialogueBoxBackgroundColor.ToVector3(), 255)));
           
                int amountToShow = 6;
                this.currentMusicPackAlbum.draw(b);

                int count = this.fancyButtons.Count-1;
                int amount = 0;
                if (0 + ( (this.currentSongPageIndex+1) * amountToShow) >= this.fancyButtons.Count)
                {
                    amount = (0 + ((this.currentSongPageIndex+1) * (amountToShow)) - fancyButtons.Count);
                    amount = amountToShow - amount;
                    if (amount < 0) amount = 0;
                }
                else if (this.fancyButtons.Count < amountToShow)
                {
                    amount = this.fancyButtons.Count;
                }
                else
                {
                    amount = amountToShow;
                }
                if (amount==0 && this.currentSongPageIndex>1)
                {
                    this.currentSongPageIndex--;
                }
                var drawList = this.fancyButtons.GetRange(0 + (this.currentSongPageIndex * (amountToShow)), amount);

                foreach(var v in drawList)
                {
                    v.draw(b);
                }

                foreach(var v in this.texturedStrings)
                {
                    v.draw(b);
                }

            }


            if (this.drawMode == DrawMode.DifferentSelectionTypesMode)
            {
                this.drawDialogueBoxBackground((int)placement.X, (int)placement.Y, (int)placement.Z, (int)placement.W, new Color(new Vector4(this.dialogueBoxBackgroundColor.ToVector3(), 255)));
                this.drawDialogueBoxBackground((int)placement2.X, (int)placement2.Y, (int)placement2.Z, (int)placement2.W, new Color(new Vector4(this.dialogueBoxBackgroundColor.ToVector3(), 255)));

                this.currentMusicPackAlbum.draw(b);
                this.currentSelectedSong.draw(b);

                foreach (Button button in fancyButtons)
                {
                    button.draw(b);
                }

                foreach (var v in this.texturedStrings)
                {
                    v.draw(b);
                }

            }

            if (this.drawMode == DrawMode.WeatherSelection)
            {
                this.drawDialogueBoxBackground((int)placement.X, (int)placement.Y, (int)placement.Z, (int)placement.W, new Color(new Vector4(this.dialogueBoxBackgroundColor.ToVector3(), 255)));
                this.drawDialogueBoxBackground((int)placement2.X, (int)placement2.Y, (int)placement2.Z, (int)placement2.W, new Color(new Vector4(this.dialogueBoxBackgroundColor.ToVector3(), 255)));

                //make 3rd dialogue box option;
                this.currentMusicPackAlbum.draw(b);
                this.currentSelectedSong.draw(b);
                this.currentlySelectedOption.draw(b);

                foreach (Button button in fancyButtons)
                {
                    button.draw(b);
                }

                foreach (var v in this.texturedStrings)
                {
                    v.draw(b);
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

                foreach (Button button in fancyButtons)
                {
                    button.draw(b);
                }

                foreach (var v in this.texturedStrings)
                {
                    v.draw(b);
                }

            }

            if (this.drawMode == DrawMode.LocationSelection)
            {
                this.drawDialogueBoxBackground((int)placement.X, (int)placement.Y, (int)placement.Z, (int)placement.W, new Color(new Vector4(this.dialogueBoxBackgroundColor.ToVector3(), 255)));
                this.drawDialogueBoxBackground((int)placement2.X, (int)placement2.Y, (int)placement2.Z, (int)placement2.W, new Color(new Vector4(this.dialogueBoxBackgroundColor.ToVector3(), 255)));

                int amountToShow = 6;
                this.currentMusicPackAlbum.draw(b);
                this.currentSelectedSong.draw(b);
                this.currentlySelectedOption.draw(b);
                this.currentlySelectedWeather.draw(b);
                this.currentlySelectedTime.draw(b);
                
                //Deals with logic regarding different pages.
                int count = this.fancyButtons.Count - 1;
                int amount = 0;
                if (0 + ((this.locationPageIndex + 1) * amountToShow) >= this.fancyButtons.Count)
                {
                    amount = (0 + ((this.locationPageIndex + 1) * (amountToShow)) - fancyButtons.Count);
                    amount = amountToShow - amount;
                    if (amount < 0) amount = 0;
                }
                else if (this.fancyButtons.Count < amountToShow)
                {
                    amount = this.fancyButtons.Count;
                }
                else
                {
                    amount = amountToShow;
                }
                if (amount == 0 && this.locationPageIndex > 1)
                {
                    this.locationPageIndex--;
                }
                var drawList = this.fancyButtons.GetRange(0 + (this.locationPageIndex * (amountToShow)), amount);

                
                foreach (var v in drawList)
                {
                    v.draw(b);
                }

                foreach (var v in this.texturedStrings)
                {
                    v.draw(b);
                }

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

                foreach (Button button in fancyButtons)
                {
                    button.draw(b);
                }

                foreach (var v in this.texturedStrings)
                {
                    v.draw(b);
                }

            }


            if (this.drawMode == DrawMode.NothingElseToDisplay)
            {
                this.drawDialogueBoxBackground((int)placement.X, (int)placement.Y, (int)placement.Z, (int)placement.W, new Color(new Vector4(this.dialogueBoxBackgroundColor.ToVector3(), 255)));

                //make 3rd dialogue box option;
                this.currentMusicPackAlbum.draw(b);
                this.currentSelectedSong.draw(b);
                this.currentlySelectedOption.draw(b);
                this.currentlySelectedWeather.draw(b);
                this.currentlySelectedTime.draw(b);
                this.currentlySelectedLocation.draw(b);
                this.currentlySelectedDay.draw(b);

                foreach (Button button in fancyButtons)
                {
                    button.draw(b);
                }

                foreach (var v in this.texturedStrings)
                {
                    v.draw(b);
                }

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
                if (!doesPackContainMusic())
                {
                    this.addButton.draw(b);
                }
                else
                {
                    this.deleteButton.draw(b);
                }
                this.playButton.draw(b);
                this.stopButton.draw(b);


                int count = this.fancyButtons.Count - 1;
                int amount = 0;
                if (0 + ((this.eventPageIndex + 1) * amountToShow) >= this.fancyButtons.Count)
                {
                    amount = (0 + ((this.eventPageIndex + 1) * (amountToShow)) - fancyButtons.Count);
                    amount = amountToShow - amount;
                    if (amount < 0) amount = 0;
                }
                else if (this.fancyButtons.Count < amountToShow)
                {
                    amount = this.fancyButtons.Count;
                }
                else
                {
                    amount = amountToShow;
                }
                if (amount == 0 && this.eventPageIndex > 1)
                {
                    this.eventPageIndex--;
                }
                var drawList = this.fancyButtons.GetRange(0 + (this.eventPageIndex * (amountToShow)), amount);

                foreach (var v in drawList)
                {
                    v.draw(b);
                }

                foreach (var v in this.texturedStrings)
                {
                    v.draw(b);
                }



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
                if (!doesPackContainMusic())
                {
                    this.addButton.draw(b);
                }
                else
                {
                    this.deleteButton.draw(b);
                }
                this.playButton.draw(b);
                this.stopButton.draw(b);

                int count = this.fancyButtons.Count - 1;
                int amount = 0;
                if (0 + ((this.menuPageIndex + 1) * amountToShow) >= this.fancyButtons.Count)
                {
                    amount = (0 + ((this.menuPageIndex + 1) * (amountToShow)) - fancyButtons.Count);
                    amount = amountToShow - amount;
                    if (amount < 0) amount = 0;
                }
                else if (this.fancyButtons.Count < amountToShow)
                {
                    amount = this.fancyButtons.Count;
                }
                else
                {
                    amount = amountToShow;
                }
                if (amount == 0 && this.menuPageIndex > 1)
                {
                    this.menuPageIndex--;
                }
                var drawList = this.fancyButtons.GetRange(0 + (this.menuPageIndex * (amountToShow)), amount);

                foreach (var v in drawList)
                {
                    v.draw(b);
                }

                foreach (var v in this.texturedStrings)
                {
                    v.draw(b);
                }



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
                if (!doesPackContainMusic())
                {
                    this.addButton.draw(b);
                }
                else
                {
                    this.deleteButton.draw(b);
                }
                this.playButton.draw(b);
                this.stopButton.draw(b);

                int count = this.fancyButtons.Count - 1;
                int amount = 0;
                if (0 + ((this.festivalPageIndex + 1) * amountToShow) >= this.fancyButtons.Count)
                {
                    amount = (0 + ((this.festivalPageIndex + 1) * (amountToShow)) - fancyButtons.Count);
                    amount = amountToShow - amount;
                    if (amount < 0) amount = 0;
                }
                else if (this.fancyButtons.Count < amountToShow)
                {
                    amount = this.fancyButtons.Count;
                }
                else
                {
                    amount = amountToShow;
                }
                if (amount == 0 && this.festivalPageIndex > 1)
                {
                    this.festivalPageIndex--;
                }
                var drawList = this.fancyButtons.GetRange(0 + (this.festivalPageIndex * (amountToShow)), amount);

                foreach (var v in drawList)
                {
                    v.draw(b);
                }

                foreach (var v in this.texturedStrings)
                {
                    v.draw(b);
                }



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
                if (!doesPackContainMusic())
                {
                    this.addButton.draw(b);
                }
                else
                {
                    this.deleteButton.draw(b);
                }

                foreach (Button button in fancyButtons)
                {
                    button.draw(b);
                }

                foreach (var v in this.texturedStrings)
                {
                    v.draw(b);
                }

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
                if (!doesPackContainMusic())
                {
                    this.addButton.draw(b);
                }
                else
                {
                    this.deleteButton.draw(b);
                }

                foreach (Button button in fancyButtons)
                {
                    button.draw(b);
                }

                foreach (var v in this.texturedStrings)
                {
                    v.draw(b);
                }

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
                if (!doesPackContainMusic())
                {
                    this.addButton.draw(b);
                }
                else
                {
                    this.deleteButton.draw(b);
                }

                foreach (Button button in fancyButtons)
                {
                    button.draw(b);
                }

                foreach (var v in this.texturedStrings)
                {
                    v.draw(b);
                }

            }


            //Draw the add, delete, play, and stop buttons.
            if (this.currentSelectedSong != null && this.currentMusicPackAlbum != null)
            {
                if (this.drawMode == DrawMode.WeatherSelection || this.drawMode == DrawMode.TimeSelection || this.drawMode == DrawMode.LocationSelection || this.drawMode == DrawMode.DaySelection || this.drawMode == DrawMode.NothingElseToDisplay)
                {
                    if (!doesPackContainMusic())
                    {
                        this.addButton.draw(b);
                    }
                    else
                    {
                        this.deleteButton.draw(b);
                    }
                }
                this.playButton.draw(b);
                this.stopButton.draw(b);
            }

            this.backButton.draw(b);
            this.drawMouse(b);
        }


        public void PlayRandomSongFromSelectedMusicPack(List<object> param)
        {
            var info=(KeyValuePair<string, MusicPack>)param[0];
            //StardewSymphony.ModMonitor.Log(info.ToString());
            StardewSymphony.musicManager.swapMusicPacks(info.Key);
            StardewSymphony.musicManager.playRandomSongFromPack(info.Key);
            //info.Value.playRandomSong();
        }

        /// <summary>
        /// Select a album artwork and change the draw mode to go to the song selection screen.
        /// </summary>
        /// <param name="b"></param>
        public void selectAlbum(Button b)
        {
            if (b.label == "Null") return;
            this.currentMusicPackAlbum = b.clone(new Vector2(this.width*.1f+64,this.height*.05f+128));
            this.texturedStrings.Clear();
            this.texturedStrings.Add(SpriteFonts.vanillaFont.ParseString("Name:" + (string)b.name, new Microsoft.Xna.Framework.Vector2(this.width*.1f, this.height*.05f + 256), b.textColor,false));
            this.drawMode = DrawMode.SongSelectionMode;
        }

        /// <summary>
        /// Select a given song from the menu.
        /// </summary>
        /// <param name="b"></param>
        public void selectSong(Button b)
        {
            if (b.label == "Null") return;
            this.currentSelectedSong = b.clone(new Vector2(this.width * .1f + 64, this.height * .05f + 256));
            this.drawMode = DrawMode.DifferentSelectionTypesMode;
        }

        /// <summary>
        /// Plays the currently selected song.
        /// </summary>
        public void playSong()
        {
            if (StardewSymphony.Config.EnableDebugLog)
                StardewSymphony.ModMonitor.Log("Song Selected!" + this.currentSelectedSong.name);
            var info = (KeyValuePair<string, MusicPack>)this.currentMusicPackAlbum.buttonFunctionality.leftClick.paramaters[0];
            if (StardewSymphony.Config.EnableDebugLog)
                StardewSymphony.ModMonitor.Log("Select Pack:" + info.Key);
            StardewSymphony.musicManager.swapMusicPacks(info.Key);
            StardewSymphony.musicManager.playSongFromCurrentPack(this.currentSelectedSong.name);
        }

        /// <summary>
        /// Stops the currently playing song.
        /// </summary>
        public void stopSong()
        {
            if (StardewSymphony.Config.EnableDebugLog)
                StardewSymphony.ModMonitor.Log("Song Selected!" + this.currentSelectedSong.name);
            var info = (KeyValuePair<string, MusicPack>)this.currentMusicPackAlbum.buttonFunctionality.leftClick.paramaters[0];
            if (StardewSymphony.Config.EnableDebugLog)
                StardewSymphony.ModMonitor.Log("Select Pack:" + info.Key);
            StardewSymphony.musicManager.swapMusicPacks(info.Key);
            StardewSymphony.musicManager.stopSongFromCurrentMusicPack();
        }

        /// <summary>
        /// Adds a song to the trigger list so that music will play at the appropriate time.
        /// </summary>
        public void addSong()
        {
            

            var info = (KeyValuePair<string, MusicPack>)this.currentMusicPackAlbum.buttonFunctionality.leftClick.paramaters[0];

            //Add generic festival music.
            if (this.drawMode == DrawMode.FestivalSelection)
            {

                info.Value.songInformation.addSongToFestivalList(this.currentlySelectedFestival.label);
                return;
            }

            //Add generic event music.
            if(this.drawMode== DrawMode.EventSelection)
            {
                info.Value.songInformation.addSongToEventList(this.currentlySelectedEvent.label);
                return;
            }

            info.Value.songInformation.addSongToTriggerList(generateSongTriggerKeyFromSelection(), this.currentSelectedSong.label);
        }

        /// <summary>
        /// Delete the song from the list of triggers.
        /// </summary>
        public void deleteSong()
        {
            var info = (KeyValuePair<string, MusicPack>)this.currentMusicPackAlbum.buttonFunctionality.leftClick.paramaters[0];
            info.Value.songInformation.removeSongFromTriggerList(generateSongTriggerKeyFromSelection(), this.currentSelectedSong.label);
        }

        /// <summary>
        /// Generate the trigger key based on used selection.
        /// </summary>
        /// <returns></returns>
        public string generateSongTriggerKeyFromSelection()
        {
            string key = "";
            string seperator = "_";
            //Seasonal selection region
            #region
            if (this.currentlySelectedOption !=null)
            {
                if (this.currentlySelectedOption.name == "SeasonIcon")
                {
                    if (Game1.currentSeason == "spring") key += "spring";
                    if (Game1.currentSeason == "summer") key += "summer";
                    if (Game1.currentSeason == "fall") key += "fall";
                    if (Game1.currentSeason == "winter") key += "winter";
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
                key += seperator + this.currentlySelectedLocation.label;
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

            if (this.currentlySelectedMenu != null)
            {
                return this.currentlySelectedMenu.label;
            }
            if (this.currentlySelectedFestival != null) return this.currentlySelectedFestival.label;
            if (this.currentlySelectedEvent != null) return this.currentlySelectedEvent.label;


            return key;
        }


        public void goBack()
        {
            if(this.drawMode==DrawMode.AlbumSelection|| this.drawMode == DrawMode.AlbumFancySelection)
            {
                this.readyToClose();
                Game1.activeClickableMenu = null;
                return;
            }
            if (this.drawMode == DrawMode.SongSelectionMode)
            {
                this.texturedStrings.Clear();
                this.drawMode = DrawMode.AlbumFancySelection;
                updateFancyButtons();
                return;
            }

            if (this.drawMode == DrawMode.DifferentSelectionTypesMode)
            {
                this.drawMode = DrawMode.SongSelectionMode;
                this.currentSongPageIndex = 0;
                updateFancyButtons();
                return;
            }

            if (this.drawMode == DrawMode.WeatherSelection)
            {
                this.drawMode = DrawMode.DifferentSelectionTypesMode;
                updateFancyButtons();
                return;
            }

            if (this.drawMode == DrawMode.TimeSelection)
            {
                this.drawMode = DrawMode.WeatherSelection;
                updateFancyButtons();
                return;
            }

            if (this.drawMode == DrawMode.LocationSelection)
            {
                this.drawMode = DrawMode.TimeSelection;
                updateFancyButtons();
                return;
            }

            if (this.drawMode == DrawMode.DaySelection)
            {
                this.drawMode = DrawMode.LocationSelection;
                this.locationPageIndex = 0;
                updateFancyButtons();
                return;
            }

            if (this.drawMode == DrawMode.NothingElseToDisplay)
            {
                this.drawMode = DrawMode.DaySelection;
                updateFancyButtons();
                return;
            }

            if (this.drawMode == DrawMode.EventSelection || this.drawMode == DrawMode.FestivalSelection || this.drawMode == DrawMode.MenuSelection)
            {
                this.drawMode = DrawMode.DifferentSelectionTypesMode;
                updateFancyButtons();
                return;
            }

            if (this.drawMode == DrawMode.SelectedEvent)
            {
                this.drawMode = DrawMode.EventSelection;
                updateFancyButtons();
                this.eventPageIndex = 0;
                return;
            }

            if (this.drawMode == DrawMode.SelectedFestival)
            {
                this.drawMode = DrawMode.FestivalSelection;
                updateFancyButtons();
                this.festivalPageIndex = 0;
                return;
            }

            if (this.drawMode == DrawMode.SelectedMenu)
            {
                this.drawMode = DrawMode.MenuSelection;
                updateFancyButtons();
                this.menuPageIndex = 0;
                return;
            }

        }

    }
}
