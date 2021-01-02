/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Gaphodil/BetterJukebox
**
*************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace Gaphodil.BetterJukebox.Framework
{
    /// <summary>The menu which lets the player choose a song to play.</summary>
    public class BetterJukeboxMenu : IClickableMenu
    {
        /************
         * Attributes
         ************/

        // ---- The parts from ShopMenu:

        /// <summary>The list of visible options to choose from.</summary>
        public List<ClickableComponent> VisibleOptions = new List<ClickableComponent>();

        /// <summary>The scroll-up button.</summary>
        public ClickableTextureComponent UpArrow;

        /// <summary>The scroll-down button.</summary>
        public ClickableTextureComponent DownArrow;

        /// <summary>The scroll bar.</summary>
        public ClickableTextureComponent ScrollBar;

        /// <summary>The area the scroll bar can move in.</summary>
        private Rectangle ScrollBarRunner;

        /// <summary>Whether the scrollbar is currently held down.</summary>
        private bool IsScrolling;

        // ---- The parts from ChooseFromListMenu:

        /// <summary>The list of songs to display.</summary>
        private List<string> Options = new List<string>();

        /// <summary>The index of the currently active selection from the Options.</summary>
        private int SelectedIndex;

        /// <summary>The lowest index from Options that is on the menu at the top of VisibleOptions.</summary>
        private int LowestVisibleIndex;

        /// <summary>The method that will be called when a button is pressed.</summary>
        private readonly BetterJukeboxMenu.actionOnChoosingListOption ChooseAction;

        // ---- Other parts:

        /// <summary>The original list of songs, passed to the constructor.</summary>
        private readonly List<string> OriginalOptions;

        /// <summary>The play button.</summary>
        public ClickableTextureComponent PlayButton;

        /// <summary>The index of the currently playing song.</summary>
        private int PlayingIndex;

        /// <summary>The stop button.</summary>
        public ClickableTextureComponent StopButton;

        // new in 1.5
        /// <summary>The random button.</summary>
        public ClickableTextureComponent RandomButton;

        /// <summary>The list of tabs, used for switching sorting methods.</summary>
        public List<ClickableTextureComponent> SortTabs = new List<ClickableTextureComponent>();

        /// <summary>The index of the currently selected tab.</summary>
        private int SelectedTab = 0;

        private readonly string ModDataTabKey = "6017/jukebox-tab";

        /// <summary>The text that displays when a tab is hovered over.</summary>
        private string HoverText;

        // ClickableComponent ids used for controller snap-to functions
        public const int BaseID = 601700;
        public const int PlayID = BaseID + 10;
        public const int StopID = BaseID + 20;
        public const int RandomID = BaseID + 30;
        public const int UpArrowID = BaseID + 40;
        public const int DownArrowID = BaseID + 50;
        public const int TabsID = BaseID + 60;

        /// <summary>Whether the random option is currently active.</summary>
        private bool IsRandom = false;

        /// <summary>The number of visible Options.</summary>
        private const int _itemsPerPage = 7;

        /// <summary>The number of pixels between visual elements.</summary>
        private const int _spacingPixels = 32;

        private const int _textureBoxBorderWidth = 4 * 4;

        /// <summary>The size of the longest song name, and width of the visible options section.</summary>
        private readonly int _longestNameWidth;

        /// <summary>The height of the visible options section of the menu.</summary>
        private const int _visibleOptionsHeight = _spacingPixels * 2 * _itemsPerPage;

        private int VisibleOptionsXPositionOnScreen;

        private int VisibleOptionsYPositionOnScreen;

        /// <summary>The play and stop button graphics as a tilesheet.</summary>
        private readonly Texture2D _BetterJukeboxGraphics;

        /// <summary>Whether internal music identifiers are displayed alongside the regular music name.</summary>
        private readonly bool _showInternalID = false;

        /// <summary>The width of the menu.</summary>
        public const int w = 1050;

        /// <summary>The height of the menu.</summary>
        public const int h = _spacingPixels * 4 + _visibleOptionsHeight;

        // ---- SMAPI tools:

        /// <summary>The function to retrieve a translation from a file.</summary>
        private readonly Func<string, Translation> GetTranslation;

        /// <summary>The SMAPI Monitor for logging messages.</summary>
        private readonly IMonitor Monitor;

        /**************** 
         * Public methods
         ****************/

        /// <summary>Construct an instance.</summary>
        /// <param name="options">The list of songs to display.</param>
        /// <param name="chooseAction">The method that will be called when a button is pressed.</param>
        public BetterJukeboxMenu(
            List<string> options,
            BetterJukeboxMenu.actionOnChoosingListOption chooseAction,
            Texture2D graphics,
            Func<string, Translation> getTranslation,
            IMonitor monitor,
            string defaultSelection = "",
            bool showInternalID = false)
            : base (
                Game1.uiViewport.Width  / 2 - (w + borderWidth * 2) / 2, // 1.5: switch from viewport to uiViewport
                Game1.uiViewport.Height / 2 - (h + borderWidth * 2) / 2, 
                w + borderWidth * 2, 
                h + borderWidth * 2,
                true)
        {
            // assign parameters
            OriginalOptions = options;
            Options.AddRange(options);
            ChooseAction = chooseAction;
            _BetterJukeboxGraphics = graphics;
            GetTranslation = getTranslation;
            Monitor = monitor;

            _showInternalID = showInternalID;

            SelectedIndex = Options.IndexOf(defaultSelection);
            if (Game1.player.currentLocation.miniJukeboxTrack.Value.Equals("")) // no active mini-jukebox 
            {
                if (Game1.startedJukeboxMusic) // hypothetically only ever true in the saloon
                {
                    PlayingIndex = Options.IndexOf(Game1.getMusicTrackName());
                    SelectedIndex = PlayingIndex;
                    Monitor.Log("Found active saloon jukebox!");
                }
                else
                {
                    PlayingIndex = -1;
                    Monitor.Log("Found no active jukebox(es)!");
                }
            }
            // new in 1.5: value is random
            else if (Game1.player.currentLocation.miniJukeboxTrack.Value.Equals("random"))
            {
                PlayingIndex = -1;
                Monitor.Log("Found active random mini-jukebox(es)!");
                IsRandom = true;
            }
            else
            {
                PlayingIndex = SelectedIndex;
                Monitor.Log("Found active mini-jukebox(es)!");
            }

            // this line is now called in SortOptions
            // LowestVisibleIndex = Math.Max(0, Math.Min(SelectedIndex, MaxScrollIndex()));

            // setup constants

            //string s = "Summer (The Sun Can Bend An Orange Sky)";         // the longest song name, probably? line from ChooseFromListMenu
            //_longestNameWidth = (int)Game1.dialogueFont.MeasureString(s).X;

            string s = "summer2";
            s = Utility.getSongTitleFromCueName(s) + " (" + s + ")";
            //_longestNameWidth = StardewValley.BellsAndWhistles.SpriteText.getWidthOfString(s);
            _longestNameWidth = (int) Game1.dialogueFont.MeasureString(s).X;
            _longestNameWidth += _textureBoxBorderWidth * 2; // spaaaacing

            //Monitor.LogOnce("The longest song width is: " + _longestNameWidth); //1176p in spritetext, 969 in dialoguefont

            //_itemsPerPage = (h - _spacingPixels * 6) / (_spacingPixels * 2); // height of text: roughly 13p * 3 = 39? 
            // ^ is now just 6 and h is set based on it

            // play The Big Selectbowski
            Game1.playSound("bigSelect");

            // reorder options based on last sorting method
            if (Game1.player.modData.ContainsKey(ModDataTabKey))
            {
                SelectedTab = int.Parse(Game1.player.modData[ModDataTabKey]);
                Monitor.Log("Retrieved " + SelectedTab + " from modData!");
            }
            else
            {
                Game1.player.modData[ModDataTabKey] = SelectedTab.ToString();
                Monitor.Log("Stored " + SelectedTab + " in modData!");
            }

            SortOptions();

            // setup ui
            SetUpPositions();

            // from ShopMenu: 
            if (Game1.options.snappyMenus && Game1.options.gamepadControls)
            {
                base.populateClickableComponentList();
                snapToDefaultClickableComponent();
            }

        }

        /// <summary>The action that is taken when an option is selected.</summary>
        /// <param name="s">The string passed to the action.</param>
        public delegate void actionOnChoosingListOption(string s);

        /***************** 
         * Private methods
         *****************/

        // ---- UI methods

        /// <summary>Regenerate the UI.</summary>
        private void SetUpPositions()
        {
            // set up play/stop buttons
            PlayButton = new ClickableTextureComponent(
                "play",
                new Rectangle(
                    xPositionOnScreen + width   - borderWidth - spaceToClearSideBorder - _spacingPixels * 4,
                    yPositionOnScreen           + _spacingPixels,
                    16 * 4,
                    15 * 4),
                "",
                null,
                Game1.mouseCursors,
                new Rectangle(175, 379, 16, 15),
                4f)
            { // ... and set ids for controller support
                myID = PlayID,
                leftNeighborID = RandomID,
                rightNeighborID = StopID,
                downNeighborID = BaseID,
                fullyImmutable = true
            };

            // avoids drawing stop button in Saloon
            if (Game1.player.currentLocation.name.Equals("Saloon"))
            {
                StopButton = null;
                RandomButton = null;
            }
            else
            {
                StopButton = new ClickableTextureComponent(
                    "stop",
                    new Rectangle(
                        xPositionOnScreen + width   - borderWidth - spaceToClearSideBorder - _spacingPixels * 2,
                        yPositionOnScreen           + _spacingPixels,
                        16 * 4,
                        15 * 4),
                    "",
                    null,
                    _BetterJukeboxGraphics,
                    new Rectangle(0, 0, 16, 15),
                    4f)
                {
                    myID = StopID,
                    leftNeighborID = PlayID,
                    downNeighborID = BaseID
                };
                RandomButton = new ClickableTextureComponent(
                    "random",
                    new Rectangle(
                        xPositionOnScreen + width   - borderWidth - spaceToClearSideBorder - _spacingPixels * 6,
                        yPositionOnScreen           + _spacingPixels,
                        16 * 4,
                        15 * 4),
                    "",
                    null,
                    Game1.mouseCursors,
                    new Rectangle(381, 361, 10, 10),
                    4f)
                {
                    myID = RandomID,
                    leftNeighborID = TabsID + SelectedTab, // whichever is currently active
                    rightNeighborID = PlayID,
                    downNeighborID = BaseID
                };
            }

            // set up sorting tabs, positions from ShopMenu
            // default sort
            SortTabs.Add(new ClickableTextureComponent(
                "ear",
                new Rectangle(
                    xPositionOnScreen - 64,
                    yPositionOnScreen - 64 * 0 + 16,
                    64,
                    64),
                "",
                "Heard order (Default)",
                _BetterJukeboxGraphics,
                new Rectangle(16, 0, 16, 16),
                4f)
            {
                myID = TabsID,
                rightNeighborID = RandomID,
                downNeighborID = TabsID + 1,
                fullyImmutable = true
            });
            // soundtrack order (TODO)
            SortTabs.Add(new ClickableTextureComponent(
                "cd",
                new Rectangle(
                    xPositionOnScreen - 64,
                    yPositionOnScreen - 64 * 1 + 16,
                    64,
                    64),
                "",
                "Soundtrack order (NOT IMPLEMENTED)",
                _BetterJukeboxGraphics,
                new Rectangle(32, 0, 16, 16),
                4f)
            {
                myID = TabsID + 1,
                rightNeighborID = RandomID,
                upNeighborID = TabsID,
                downNeighborID = TabsID + 2,
                fullyImmutable = true
            });
            // alphabetical by song title
            SortTabs.Add(new ClickableTextureComponent(
                "alpha1",
                new Rectangle(
                    xPositionOnScreen - 64,
                    yPositionOnScreen - 64 * 2 + 16,
                    64,
                    64),
                "",
                "Alphabetical (song title)",
                _BetterJukeboxGraphics,
                new Rectangle(0, 16, 16, 16),
                4f)
            {
                myID = TabsID + 2,
                rightNeighborID = RandomID,
                upNeighborID = TabsID + 1,
                downNeighborID = TabsID + 3,
                fullyImmutable = true
            });
            // alphabetical by cue name
            SortTabs.Add(new ClickableTextureComponent(
                "alpha2",
                new Rectangle(
                    xPositionOnScreen - 64,
                    yPositionOnScreen - 64 * 3 + 16,
                    64,
                    64),
                "",
                "Alphabetical (cue name)",
                _BetterJukeboxGraphics,
                new Rectangle(16, 16, 16, 16),
                4f)
            {
                myID = TabsID + 3,
                rightNeighborID = RandomID,
                upNeighborID = TabsID + 2,
                fullyImmutable = true
            });

            // reposition the tabs
            RepositionTabs();

            // set up VisibleOptions to select from
            UpdateVisibleOptions();

            // set up scrolling widgets (IDs from ShopMenu)
            UpArrow = new ClickableTextureComponent(
                new Rectangle(
                    xPositionOnScreen + width   + _spacingPixels / 2,
                    VisibleOptionsYPositionOnScreen,
                    44,
                    48),
                Game1.mouseCursors,
                new Rectangle(421,459,11,12),   // up arrow
                4f)
            { 
                myID = UpArrowID,
                upNeighborID = PlayID,
                downNeighborID = DownArrowID,
                leftNeighborID = BaseID
            };

            DownArrow = new ClickableTextureComponent(
                new Rectangle(
                    xPositionOnScreen + width   + _spacingPixels / 2,
                    yPositionOnScreen + height  - 48,
                    44,
                    48), 
                Game1.mouseCursors,
                new Rectangle(421,472,11,12),   // down arrow
                4f)
            {
                myID = DownArrowID,
                upNeighborID = UpArrowID,
                leftNeighborID = BaseID
            };

            ScrollBar = new ClickableTextureComponent(
                new Rectangle(
                    UpArrow.bounds.X + 12,
                    UpArrow.bounds.Y + UpArrow.bounds.Height + 4,
                    24,
                    40), 
                Game1.mouseCursors,
                new Rectangle(435,463,6,10),    // scroll bar
                4f);

            int runnerY = UpArrow.bounds.Y + UpArrow.bounds.Height + 12;
            ScrollBarRunner = new Rectangle(
                ScrollBar.bounds.X,
                runnerY,
                ScrollBar.bounds.Width,
                DownArrow.bounds.Y - runnerY - 12);

            SetScrollBarToLowestVisibleIndex();
        }

        /// <summary>Repositions tabs so that the active one is positioned in front.</summary>
        private void RepositionTabs()
        {
            for (int i = 0; i < SortTabs.Count; ++i)
            {
                if (i == SelectedTab)
                    SortTabs[i].bounds.X = xPositionOnScreen - 56;
                else SortTabs[i].bounds.X = xPositionOnScreen - 64;
                SortTabs[i].bounds.Y = yPositionOnScreen + (i * 64) + 16;
            }
        }

        /// <summary>Updates the list of visible options, to be called if scrolling occurs.</summary>
        private void UpdateVisibleOptions()
        {
            VisibleOptions.Clear();
            UpdateVisibleOptionsPositions();

            for (int i = 0; i < _itemsPerPage; ++i)
            {
                int options_index = i + Math.Min(MaxScrollIndex(), LowestVisibleIndex);
                if (options_index >= Options.Count)
                    break;

                VisibleOptions.Add(new ClickableComponent(
                    new Rectangle(
                        VisibleOptionsXPositionOnScreen + _textureBoxBorderWidth,
                        VisibleOptionsYPositionOnScreen + _textureBoxBorderWidth + _spacingPixels * 2 * i,
                        _longestNameWidth + 4,
                        _spacingPixels * 2),
                    Options[options_index])
                    { // from ShopMenu
                        myID = BaseID + i,
                        upNeighborID = (i == 0) ? ClickableComponent.CUSTOM_SNAP_BEHAVIOR : BaseID + i - 1,
                        downNeighborID = (i == _itemsPerPage - 1) ? ClickableComponent.CUSTOM_SNAP_BEHAVIOR : BaseID + i + 1,
                        leftNeighborID = TabsID + SelectedTab, // whichever is currently active
                        rightNeighborID = UpArrowID,
                        fullyImmutable = true
                    }
                );
                // Monitor.Log(string.Format("VisibleOption {0} has baseID {1} and downNeighborID {2}",i, VisibleOptions[i].myID, VisibleOptions[i].downNeighborID));
            }
        }

        /// <summary>Updates the ScrollBar to the current index.</summary>
        private void SetScrollBarToLowestVisibleIndex() 
        {
            // Implementation derived from ShopMenu
            if (Options.Count <= 0 || MaxScrollIndex() == 0)
                return;

            float ratio = (float) LowestVisibleIndex / MaxScrollIndex();
            int runner_range_for_top_of_bar = ScrollBarRunner.Height - ScrollBar.bounds.Height;
            
            ScrollBar.bounds.Y = ScrollBarRunner.Y + (int) (ratio * runner_range_for_top_of_bar);
        }

        // ---- Helper methods

        /// <summary>
        /// Comparison function for sorting options by the song's title.
        /// </summary>
        /// <param name="cue1">The cue of the first song.</param>
        /// <param name="cue2">The cue of the second song.</param>
        /// <returns></returns>
        private static int SortBySongTitle(string cue1, string cue2)
        {
            string songTitle1 = Utility.getSongTitleFromCueName(cue1);
            string songTitle2 = Utility.getSongTitleFromCueName(cue2);
            return songTitle1.CompareTo(songTitle2);
        }

        /// <summary>
        /// Sort Options based on the SelectedTab.
        /// </summary>
        private void SortOptions()
        {
            // remember player's selected sort in modData
            Game1.player.modData[ModDataTabKey] = SelectedTab.ToString();
            Monitor.Log("Stored " + SelectedTab + " in modData!");
            // save last selected index
            if (SelectedIndex != PlayingIndex) SelectedIndex = PlayingIndex;
            string orig_cue = "";
            if (SelectedIndex != -1) orig_cue = Options[SelectedIndex];
            // do the sort
            switch (SelectedTab) {
                case 0: // default sort
                    // hard copy OriginalOptions
                    Options.Clear();
                    Options.AddRange(OriginalOptions);
                    break;
                case 1: // soundtrack order
                    break;
                case 2: // alphabetical by SongTitle
                    Options.Sort(SortBySongTitle);
                    break;
                case 3: // alphabetical by normal
                    Options.Sort();
                    break;
            }
            // get index of new list
            if (SelectedIndex != -1 && !orig_cue.Equals(""))
            {
                SelectedIndex = Options.IndexOf(orig_cue);
                PlayingIndex = SelectedIndex;
            }
            // set as top of visible
            LowestVisibleIndex = Math.Max(0, Math.Min(SelectedIndex, MaxScrollIndex()));
        }

        /// <summary>Returns the max index of options that can display at the top of the list.</summary>
        /// <returns>the index</returns>
        private int MaxScrollIndex()
        {
            return Math.Max(0, Options.Count - _itemsPerPage);
        }

        /// <summary>
        /// Returns the number of mini-jukeboxes on the farmer's current map. 
        /// Used as a poor workaround for differentiating the Saloon jukebox from mini-jukeboxes.
        /// </summary>
        /// <returns>the number of mini-jukeboxes in the current location</returns>
        private int GetNumberOfLocalMiniJukeboxes()
        {
            return Game1.player.currentLocation.miniJukeboxCount.Value;
        }

        /// <summary>Updates the VisibleOptions PositionOnScreen variables.</summary>
        private void UpdateVisibleOptionsPositions()
        {
            VisibleOptionsXPositionOnScreen = xPositionOnScreen + width / 2 - _longestNameWidth / 2 - _textureBoxBorderWidth;
            VisibleOptionsYPositionOnScreen = yPositionOnScreen + height    - _visibleOptionsHeight - _textureBoxBorderWidth * 2 - _spacingPixels;
        }

        // ---- Button methods

        /// <summary>Handles the UpArrow button.</summary>
        private void UpArrowPressed()
        {
            // Implementation copied from ShopMenu
            UpArrow.scale = UpArrow.baseScale;
            --LowestVisibleIndex;
            SetScrollBarToLowestVisibleIndex();
            UpdateVisibleOptions();
        }

        /// <summary>Handles the DownArrow button.</summary>
        private void DownArrowPressed()
        {
            // Implementation copied from ShopMenu
            DownArrow.scale = DownArrow.baseScale;
            ++LowestVisibleIndex;
            SetScrollBarToLowestVisibleIndex();
            UpdateVisibleOptions();
        }

        /// <summary>Handles the PlayButton.</summary>
        private void PlayButtonPressed()
        {
            ChooseAction(Options[SelectedIndex]);
            PlayingIndex = SelectedIndex;
            IsRandom = false;

            Monitor.Log("Jukebox now playing: " + Options[SelectedIndex]);

            // duplicate oops
            // Game1.playSound("select");
        }

        /// <summary>Handles the StopButton.</summary>
        private void StopButtonPressed()
        {
            if (GetNumberOfLocalMiniJukeboxes() == 0) // this shouldn't happen!
            {
                Monitor.Log("StopButtonPressed() despite no mini-jukeboxes!", LogLevel.Error);
                return;
            }

            ChooseAction("turn_off");
            PlayingIndex = -1;
            IsRandom = false;

            Monitor.Log("Jukebox turned off!");

            // Game1.playSound("select");
        }

        private void RandomButtonPressed() 
        {
            if (GetNumberOfLocalMiniJukeboxes() == 0) // this shouldn't happen!
            {
                Monitor.Log("RandomButtonPressed() despite no mini-jukeboxes!", LogLevel.Error);
                return;
            }

            ChooseAction("random"); // NOTE: this will not include "typically removed" tracks even if enabled
            // above sets GameLocation.randomMiniJukeboxTrack.Value
            // vanilla bug(?): only happens IF miniJukeboxTrack is "random", which is set AFTER the randomize attempt is made
            if (Game1.player.currentLocation.randomMiniJukeboxTrack.Value.Equals(""))
                ChooseAction("random"); // do it again
            PlayingIndex = -1;
            IsRandom = true;

            Monitor.Log("Random selected! Now playing: " + Game1.player.currentLocation.randomMiniJukeboxTrack.Value);

            // Game1.playSound("select");
        }

        /******************
         * Override methods
         ******************/

        /// <summary>The method invoked when the player left-clicks on the menu.</summary>
        /// <param name="x">The X-position of the cursor.</param>
        /// <param name="y">The Y-position of the cursor.</param>
        /// <param name="playSound">Whether to enable sound.</param>
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            // Implementation derived mostly from ShopMenu
            base.receiveLeftClick(x, y, playSound);
            if (Game1.activeClickableMenu == null)
                return;

            // scrolling related widgets
            if (DownArrow.containsPoint(x, y) && LowestVisibleIndex < MaxScrollIndex()) // not bottom of list
            {
                DownArrowPressed();
                Game1.playSound("shwip");
            }
            else if (UpArrow.containsPoint(x, y) && LowestVisibleIndex > 0) // not top of list
            {
                UpArrowPressed();
                Game1.playSound("shwip");
            }
            else if (ScrollBar.containsPoint(x, y))
                IsScrolling = true;
            else if (!DownArrow.containsPoint(x, y) 
                && x > xPositionOnScreen + width 
                && (x < xPositionOnScreen + width + 128 
                && y > yPositionOnScreen) 
                && y < yPositionOnScreen + height)
            {
                IsScrolling = true;
                leftClickHeld(x, y);
                releaseLeftClick(x, y);
            }

            // play or stop buttons
            else if (PlayButton.containsPoint(x, y) && SelectedIndex >= 0)
            {
                PlayButtonPressed();
                Game1.playSound("select");
            }
            else if (StopButton != null && StopButton.containsPoint(x,y))
            {
                StopButtonPressed();
                Game1.playSound("select");
            }
            // and now random button
            else if (RandomButton != null && RandomButton.containsPoint(x,y))
            {
                RandomButtonPressed();
                Game1.playSound("select");
            }

            // option select (give 'em the mixup)
            else
            {
                // first tabs
                for (int i = 0; i < SortTabs.Count; ++i)
                {
                    ClickableComponent sort_tab = SortTabs[i];
                    if (sort_tab.containsPoint(x,y))
                    {
                        SelectedTab = i;
                        SortOptions();
                        RepositionTabs();
                        UpdateVisibleOptions();
                        SetScrollBarToLowestVisibleIndex();
                        Game1.playSound("shwip");
                    }
                }
                // then actual options
                for (int i = 0; i < VisibleOptions.Count; ++i)
                {
                    ClickableComponent visible_option = VisibleOptions[i];
                    if (visible_option.containsPoint(x,y))
                    {
                        // repeat selection
                        if (LowestVisibleIndex + i == SelectedIndex)
                        {
                            //Monitor.Log("Playing a song: " + Options[SelectedIndex]);
                            if (visible_option.name != Options[SelectedIndex])
                                Monitor.Log("The song on the button does not match the one being played!", LogLevel.Error);
                            PlayButtonPressed();
                            Game1.playSound("select");
                        }

                        // new selection
                        else
                        {
                            SelectedIndex = LowestVisibleIndex + i;
                            Game1.playSound("shiny4");
                        }

                    }
                }
            }
            
        }

        /// <summary>The method invoked when the player holds left-click.</summary>
        /// <param name="x">The X-position of the cursor.</param>
        /// <param name="y">The Y-position of the cursor.</param>
        public override void leftClickHeld(int x, int y)
        {
            // Implementation copied from ShopMenu
            base.leftClickHeld(x, y);
            if (!IsScrolling)
                return;

            int original_y_bound = ScrollBar.bounds.Y;
            ScrollBar.bounds.Y = Math.Min(
                DownArrow.bounds.Y - 4 - ScrollBar.bounds.Height,   // lowest possible position
                Math.Max(
                    y,      // mouse position
                    UpArrow.bounds.Y + UpArrow.bounds.Height + 4)); // highest possible position

            LowestVisibleIndex = Math.Min(
                MaxScrollIndex(),
                Math.Max(
                    0, 
                    (int)( Options.Count *  // # of options times...
                    (double)( (y - ScrollBarRunner.Y) / (float) ScrollBarRunner.Height ) ) // the fraction of runner scrolled
                )
            );

            SetScrollBarToLowestVisibleIndex();
            UpdateVisibleOptions();

            if (original_y_bound != ScrollBar.bounds.Y)
                Game1.playSound("shiny4");
        }

        /// <summary>The method invoked when the player releases left-click.</summary>
        /// <param name="x">The X-position of the cursor.</param>
        /// <param name="y">The Y-position of the cursor.</param>
        public override void releaseLeftClick(int x, int y)
        {
            // Implementation copied from ShopMenu
            base.releaseLeftClick(x, y);
            IsScrolling = false;
        }

        /// <summary>The method invoked when the player right-clicks on the lookup UI.</summary>
        /// <param name="x">The X-position of the cursor.</param>
        /// <param name="y">The Y-position of the cursor.</param>
        /// <param name="playSound">Whether to enable sound.</param>
        public override void receiveRightClick(int x, int y, bool playSound = true) { }

        /// <summary>The method invoked when the player hovers the cursor over the menu.</summary>
        /// <param name="x">The X-position of the cursor.</param>
        /// <param name="y">The Y-position of the cursor.</param>
        public override void performHoverAction(int x, int y) 
        {
            base.performHoverAction(x, y);

            HoverText = "";

            if (SelectedIndex >= 0)
                PlayButton.tryHover(x, y);
            StopButton?.tryHover(x, y);
            RandomButton?.tryHover(x, y, 0.2f); // works now!
            // forgot about these
            UpArrow.tryHover(x, y);
            DownArrow.tryHover(x, y);
            // maybe this displays hovertext?
            for (int i = 0; i < SortTabs.Count; ++i)
            {
                if (SortTabs[i].containsPoint(x, y))
                    HoverText = SortTabs[i].hoverText;
            }
        }

        /// <summary>The method invoked when the player scrolls the mousewheel.</summary>
        /// <param name="direction">The direction of the player's scroll.</param>
        public override void receiveScrollWheelAction(int direction)
        {
            // Implementation copied from ShopMenu
            base.receiveScrollWheelAction(direction);
            if (direction > 0 && LowestVisibleIndex > 0)
            {
                UpArrowPressed();
                Game1.playSound("shiny4");
            }
            else
            {
                if (direction >= 0 || LowestVisibleIndex >= Math.Max(0, MaxScrollIndex()))
                    return;
                DownArrowPressed();
                Game1.playSound("shiny4");
            }
        }

        // ---- Controller related
        // mostly derived from ShopMenu

        // appears to be only called when trying to move in a direction with ClickableComponent.CUSTOM_SNAP_BEHAVIOR as the next id
        protected override void customSnapBehavior(int direction, int oldRegion, int oldID)
        {
            switch (direction)
            {
                case 2: // down
                    if (LowestVisibleIndex < MaxScrollIndex()) // not bottom of list
                        DownArrowPressed(); // no change in cursor location needed
                    break; 
                case 0: // up
                    if (LowestVisibleIndex > 0) // not top of list
                        UpArrowPressed(); // no cursor change needed
                    else // moving from top of list to play button
                    {
                        base.currentlySnappedComponent = PlayButton;
                        snapCursorToCurrentSnappedComponent();
                    }
                    break;
            }
        }
        public override void snapToDefaultClickableComponent()
        {
            base.currentlySnappedComponent = VisibleOptions[0];
            snapCursorToCurrentSnappedComponent(); 
        }

        // used for tab button cycling; will add when sort methods are implemented
        public override void receiveGamePadButton(Buttons b)
        {
            base.receiveGamePadButton(b);
            // derived from CheatsMenu, sorry/thanks CJB/Pathos!
            if (b == Buttons.LeftShoulder || b == Buttons.RightShoulder)
            {
                // rotate tab index
                if (b == Buttons.LeftShoulder)
                    SelectedTab--;
                if (b == Buttons.RightShoulder)
                    SelectedTab++;

                if (SelectedTab >= SortTabs.Count)
                    SelectedTab = 0;
                if (SelectedTab < 0)
                    SelectedTab = SortTabs.Count - 1;

                SortOptions();
                RepositionTabs();
                UpdateVisibleOptions();
                SetScrollBarToLowestVisibleIndex();
            }
        }

        // ---- Drawing related

        /// <summary>The method called when the game window changes size.</summary>
        /// <param name="oldBounds">The former viewport.</param>
        /// <param name="newBounds">The new viewport.</param>
        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            base.gameWindowSizeChanged(oldBounds, newBounds);
            //xPositionOnScreen = newBounds.Width  / 2 - width  / 2; // tried to do it manually because I noticed a vanilla bug
            //yPositionOnScreen = newBounds.Height / 2 - height / 2; // turns out: this was worse!

            // refresh ui
            SetUpPositions();
            initializeUpperRightCloseButton();
        }

        /// <summary>Draw the menu to the screen.</summary>
        /// <param name="b">The sprite batch.</param>
        public override void draw(SpriteBatch b)
        {
            // Implementation derived from ChooseFromListMenu

            // from ShopMenu: draw menuBackground if enabled
            if (Game1.options.showMenuBackground)
            {
                base.drawBackground(b);
            }
            else
            {
                b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.4f); // 0.4 is default(?); used in GameMenu
            }

            // draw menu box
            drawTextureBox(
                b,
                xPositionOnScreen,
                yPositionOnScreen,
                width,
                height,
                Color.White);

            // draw options' submenu box
            drawTextureBox(
                b,
                Game1.mouseCursors,
                new Rectangle(384, 373, 18, 18),    // shopMenu's nail corners
                VisibleOptionsXPositionOnScreen,
                VisibleOptionsYPositionOnScreen,
                _longestNameWidth       + _textureBoxBorderWidth * 2,
                _visibleOptionsHeight   + _textureBoxBorderWidth * 2,
                Color.White,
                4f,
                false);

            // draw menu title
            StardewValley.BellsAndWhistles.SpriteText.drawStringWithScrollCenteredAt(
                b,
                Game1.content.LoadString(
                    "Strings\\UI:JukeboxMenu_Title"),
                xPositionOnScreen + width / 2,
                yPositionOnScreen - _spacingPixels);

            // draw "Currently Playing:"
            String cur_play = GetTranslation("BetterJukeboxMenu:Currently_Playing");

            Utility.drawTextWithShadow(
                b,
                cur_play,
                Game1.dialogueFont,
                new Vector2(
                    xPositionOnScreen + width / 2           - Game1.dialogueFont.MeasureString(cur_play).X / 2f,
                    yPositionOnScreen + _spacingPixels),
                Game1.textColor);

            // draw the name of the active song
            String song_name;
            if (PlayingIndex > -1)
                song_name = Utility.getSongTitleFromCueName(Options[PlayingIndex]);
            else
            {
                if (IsRandom)
                    song_name = Utility.getSongTitleFromCueName(Game1.player.currentLocation.randomMiniJukeboxTrack.Value);
                else
                    song_name = Utility.getSongTitleFromCueName("turn_off");
            }

            Utility.drawTextWithShadow(
                b,
                song_name,
                Game1.dialogueFont,
                new Vector2(
                    xPositionOnScreen + width / 2       - Game1.dialogueFont.MeasureString(song_name).X / 2f,
                    yPositionOnScreen + _spacingPixels  + Game1.dialogueFont.MeasureString(cur_play).Y  * 1.25f),
                Game1.textColor);

            //StardewValley.BellsAndWhistles.SpriteText.drawStringHorizontallyCenteredAt(
            //    b,
            //    song_name,
            //    xPositionOnScreen + width / 2,
            //    yPositionOnScreen + _spacingPixels * 3 / 2 + (int) Game1.dialogueFont.MeasureString(cur_play).Y);

            // draw the list of VisibleOptions
            // derived from forSale section of ShopMenu.draw()
            for (int i = 0; i < VisibleOptions.Count; ++i)
            {
                // don't draw if LowestVisibleIndex is incorrect
                if (LowestVisibleIndex + i >= Options.Count)
                {
                    Monitor.LogOnce("Ceased drawing options because LowestVisibleIndex is incorrectly high!", LogLevel.Error);
                    break;  // not continue because will continue to be incorrect
                }

                ClickableComponent button = VisibleOptions[i];

                // determining button colour; priority: selected - hovered (and not dragging scrollbar) - not hovered
                Color button_colour;
                if (LowestVisibleIndex + i == SelectedIndex)
                {
                    button_colour = Color.Peru; // probably looks fine
                }
                else if (button.containsPoint(Game1.getOldMouseX(),Game1.getOldMouseY()) && !IsScrolling)
                {
                    button_colour = Color.Wheat;
                }
                else
                {
                    button_colour = Color.White;
                }

                // drawing the box around each option
                drawTextureBox(
                    b,
                    Game1.mouseCursors,
                    new Rectangle(384, 396, 15, 15),            // box shape w/ elaborate corners
                    button.bounds.X,
                    button.bounds.Y,
                    button.bounds.Width,
                    button.bounds.Height,
                    button_colour,
                    4f,
                    false);

                // drawing option name 
                string cue_name = Options[LowestVisibleIndex + i];

                song_name = Utility.getSongTitleFromCueName(cue_name);

                if (_showInternalID)    // left align song_name, right align cue_name
                {
                    if (cue_name.Equals(song_name))
                        ;   // do nothing
                    else
                    {
                        Utility.drawTextWithShadow(
                            b,
                            song_name,
                            Game1.dialogueFont,
                            new Vector2(
                                button.bounds.X + _textureBoxBorderWidth,
                                button.bounds.Y + button.bounds.Height / 2 - Game1.dialogueFont.MeasureString(song_name).Y / 2f),
                            Game1.textColor);
                    }

                    Utility.drawTextWithShadow(
                        b,
                        cue_name,
                        Game1.dialogueFont,
                        new Vector2(
                            button.bounds.X + button.bounds.Width       - Game1.dialogueFont.MeasureString(cue_name).X - _textureBoxBorderWidth,
                            button.bounds.Y + button.bounds.Height / 2  - Game1.dialogueFont.MeasureString(cue_name).Y / 2f),
                        Game1.textColor);
                }
                else // center text
                {
                    Utility.drawTextWithShadow(
                        b,
                        song_name,
                        Game1.dialogueFont,
                        new Vector2(
                            button.bounds.X + button.bounds.Width / 2 - Game1.dialogueFont.MeasureString(song_name).X / 2f,
                            button.bounds.Y + button.bounds.Height / 2 - Game1.dialogueFont.MeasureString(song_name).Y / 2f),
                        Game1.textColor);
                }
            }

            // draw the play and stop buttons
            PlayButton.draw(b);
            StopButton?.draw(b);
            // and the random button
            if (RandomButton != null)
            {
                RandomButton.draw(b);
                // draw check/cross
                if (IsRandom)
                {
                    b.Draw(ChatBox.emojiTexture,
                           new Vector2( RandomButton.bounds.X + 6 * 4, 
                                        RandomButton.bounds.Y + 6 * 4),
                           new Rectangle(117, 81, 9, 9),
                           Color.White,
                           0f,
                           Vector2.Zero,
                           4f,
                           SpriteEffects.None,
                           0.99f);
                }
                else
                {
                    b.Draw(ChatBox.emojiTexture,
                           new Vector2( RandomButton.bounds.X + 6 * 4,
                                        RandomButton.bounds.Y + 6 * 4),
                           new Rectangle(45, 81, 9, 9),
                           Color.White,
                           0f,
                           Vector2.Zero,
                           4f,
                           SpriteEffects.None,
                           0.99f);
                }
            }

            // draw the SortTabs
            for (int i = 0; i < SortTabs.Count; ++i)
            {
                SortTabs[i].draw(b);
            }

            // draw the scrolling elements
            if (VisibleOptions.Count >= Options.Count)
                ; // do nothing
            else
            {
                UpArrow.draw(b);
                DownArrow.draw(b);

                // copied from ShopMenu.draw()
                drawTextureBox(
                    b,
                    Game1.mouseCursors,
                    new Rectangle(403, 383, 6, 6),
                    ScrollBarRunner.X,
                    ScrollBarRunner.Y,
                    ScrollBarRunner.Width,
                    ScrollBarRunner.Height,
                    Color.White,
                    4f);
                ScrollBar.draw(b);
            }

            // from ShopMenu: draw tooltip (hover)
            if (!HoverText.Equals(""))
            {
                IClickableMenu.drawHoverText(b, HoverText, Game1.smallFont);
            }

            // draw the upper right close button
            base.draw(b);

            // draw cursor
            drawMouse(b);
        }
    }
}
