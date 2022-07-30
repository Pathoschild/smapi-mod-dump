/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using Microsoft.Build.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using SysTask = System.Threading.Tasks.Task;

namespace Sounds_Patcher.Utility
{
    public class SoundsMenu : IClickableMenu
    {
        private const int MaxItemsPerPage = 7;

        private IModHelper Helper;
        private IMonitor Monitor;
        private Config Config;

        private Rectangle ScrollBarTrack;
        private ICue ActiveCue;

        private ClickableTextureComponent ScrollBarThumb;
        private ClickableTextureComponent ArrowUp;
        private ClickableTextureComponent ArrowDown;

        private int CurrentOffset;
        private bool Scrolling;
        private string ActiveCueName;
        private string SelectedMusicTrack;
        private string MusicTrackBeforeTest;

        private List<OptionsElement> Items = new List<OptionsElement>();
        private List<ClickableTextureComponent> PlayButtons = new List<ClickableTextureComponent>();
        private List<ClickableComponent> OptionSlots = new List<ClickableComponent>();

        public SoundsMenu(IModHelper helper, IMonitor monitor, Config config) : base(0, 0, 0, 0, true)
        {
            Helper = helper;
            Monitor = monitor;
            Config = config;

            Items.Add(new OptionsElement("Sounds", 32, 8, 36, 36));
            #region SoundBoxes

            foreach (KeyValuePair<string, bool> pair in Config.Sounds)
                Items.Add((OptionsElement)new OptionsCheckbox(pair.Key, -999, 32, 4) { isChecked = pair.Value });

            #endregion

            Items.Add(new OptionsElement("Songs", 32, 8, 36, 36));
            #region SongBoxes

            foreach (KeyValuePair<string, bool> pair in Config.Songs)
                Items.Add((OptionsElement)new OptionsCheckbox(pair.Key, -999, 32, 4) { isChecked = pair.Value });

            #endregion

            ResetLayout();

            if (Game1.getMusicTrackName() != "none")
            {
                MusicTrackBeforeTest = Game1.getMusicTrackName();
                Game1.changeMusicTrack("none");
            }

            exitFunction = new onExit(() => { if ((Game1.getMusicTrackName() == "none" || Game1.getMusicTrackName() == SelectedMusicTrack) && !string.IsNullOrWhiteSpace(MusicTrackBeforeTest)) Game1.changeMusicTrack(MusicTrackBeforeTest); if (ActiveCue != null) ActiveCue.Stop(AudioStopOptions.Immediate); });
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            ResetLayout();
            base.gameWindowSizeChanged(oldBounds, newBounds);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (ArrowUp.containsPoint(x, y))
            {
                ArrowUpPressed();
                Game1.playSound("shiny4");
            }
            else if (ArrowDown.containsPoint(x, y))
            {
                ArrowDownPressed();
                Game1.playSound("shiny4");
            }
            else if (ScrollBarThumb.containsPoint(x, y)) Scrolling = true;
            else if (ScrollBarTrack.Contains(x, y))
            {
                Scrolling = true;
                leftClickHeld(x, y);
                releaseLeftClick(x, y);
            }

            for (int i = 0; i < OptionSlots.Count; ++i)
            {
                if (OptionSlots[i].bounds.Contains(x, y) && CurrentOffset + i < Items.Count && Items[CurrentOffset + i].bounds.Contains(x - OptionSlots[i].bounds.X, y - OptionSlots[i].bounds.Y - 5))
                {
                    Items[CurrentOffset + i].receiveLeftClick(x - OptionSlots[i].bounds.X, y - OptionSlots[i].bounds.Y + 5);
                    if (Items[CurrentOffset + i] is OptionsCheckbox)
                        UpdateConfig(Items[CurrentOffset + i] as OptionsCheckbox);
                    break;
                }
            }

            for (int i = 0; i < PlayButtons.Count; ++i)
            {
                if(PlayButtons[i].containsPoint(x, y))
                {
                    string requestedCue = Items[CurrentOffset + i].label;
                    if (requestedCue == ActiveCueName)
                    {
                        StopActiveSound();
                        break;
                    }
                    if((Config.Sounds.ContainsKey(requestedCue) && !Config.Sounds[requestedCue]) || (Config.Songs.ContainsKey(requestedCue) && !Config.Songs[requestedCue]))
                    {
                        if (Game1.getMusicTrackName() != "none" || (ActiveCue != null && ActiveCue.IsPlaying)) StopActiveSound();
                        TestSound(requestedCue);
                        break;
                    }
                }
            }

            base.receiveLeftClick(x, y, playSound);
        }

        public override void leftClickHeld(int x, int y)
        {
            if (Scrolling)
            {
                int initialY = y - (ScrollBarTrack.Y + ScrollBarThumb.bounds.Height / 2);
                int scrollHeight = ScrollBarTrack.Height - ScrollBarThumb.bounds.Height;
                double ratio = Math.Min(Math.Max(initialY * 1.0 / scrollHeight, 0), 1);
                int newOffset = (int)Math.Round(ratio * (Items.Count - MaxItemsPerPage));
                if (newOffset != CurrentOffset)
                {
                    CurrentOffset = newOffset;
                    Game1.playSound("shiny4");
                    SetScrollBarToCurrentIndex();
                }
            }

            base.leftClickHeld(x, y);
        }

        public override void releaseLeftClick(int x, int y)
        {
            base.releaseLeftClick(x, y);
            Scrolling = false;
        }

        public override void receiveScrollWheelAction(int direction)
        {
            if (direction > 0)
            {
                ArrowUpPressed();
                Game1.playSound("shiny4");
            }
            else if (direction < 0)
            {
                ArrowDownPressed();
                Game1.playSound("shiny4");
            }

            base.receiveScrollWheelAction(direction);
        }

        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);
            ArrowUp.tryHover(x, y, 0.25f);
            ArrowDown.tryHover(x, y, 0.25f);
            ScrollBarThumb.tryHover(x, y);
            for (int i = 0; i < PlayButtons.Count; ++i)
                PlayButtons[i].tryHover(x, y, 0.25f);
        }

        public override void draw(SpriteBatch b)
        {
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
            base.draw(b);
            Game1.drawDialogueBox(xPositionOnScreen, yPositionOnScreen, width, height, false, true);
            SpriteText.drawStringWithScrollCenteredAt(b, "Sounds Patcher", (int)(xPositionOnScreen + (width / 2)), (int)(yPositionOnScreen - 4), "Sounds Patcher");

            b.End();
            b.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);

            for (int i = 0; i < MaxItemsPerPage; ++i)
            {
                Items[CurrentOffset + i].draw(b, OptionSlots[i].bounds.X, OptionSlots[i].bounds.Y);

                if (Items[CurrentOffset + i].whichOption != -1)
                    PlayButtons[i].draw(b);
            }

            b.End();
            b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);

            ArrowUp.draw(b);
            ArrowDown.draw(b);
            drawTextureBox(b, Game1.mouseCursors, new Rectangle(403, 383, 6, 6), ScrollBarTrack.X, ScrollBarTrack.Y, ScrollBarTrack.Width, ScrollBarTrack.Height, Color.White, Game1.pixelZoom);
            ScrollBarThumb.draw(b);

            if (!Game1.options.hardwareCursor)
                drawMouse(b);
        }

        private void ResetLayout()
        {
            width = 800 + borderWidth * 2;
            height = 600 + borderWidth * 2;
            xPositionOnScreen = Game1.uiViewport.Width / 2 - (width - (int)(Game1.tileSize * 2.4f)) / 2;
            yPositionOnScreen = Game1.uiViewport.Height / 2 - height / 2;

            const int Offset = Game1.tileSize / 4;

            upperRightCloseButton = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width + Offset - 2, yPositionOnScreen - Game1.pixelZoom, Game1.pixelZoom * 12, Game1.pixelZoom * 12), Game1.mouseCursors, new Rectangle(337, 494, 12, 12), Game1.pixelZoom);
            ArrowUp = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width + Offset, yPositionOnScreen + Game1.tileSize, Game1.pixelZoom * 11, Game1.pixelZoom * 12), Game1.mouseCursors, new Rectangle(421, 459, 11, 12), Game1.pixelZoom);
            ArrowDown = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width + Offset, yPositionOnScreen + height - Game1.tileSize, Game1.pixelZoom * 11, Game1.pixelZoom * 12), Game1.mouseCursors, new Rectangle(421, 472, 11, 12), Game1.pixelZoom);
            ScrollBarThumb = new ClickableTextureComponent(new Rectangle(ArrowUp.bounds.X + Game1.pixelZoom * 3, ArrowUp.bounds.Y + ArrowUp.bounds.Height + Game1.pixelZoom, 24, 40), Game1.mouseCursors, new Rectangle(435, 463, 6, 10), Game1.pixelZoom);
            ScrollBarTrack = new Rectangle(ScrollBarThumb.bounds.X, ArrowUp.bounds.Y + ArrowUp.bounds.Height + Game1.pixelZoom, ScrollBarThumb.bounds.Width, height - Game1.tileSize * 2 - ArrowUp.bounds.Height - Game1.pixelZoom * 2);
            SetScrollBarToCurrentIndex();

            OptionSlots.Clear();
            for (int i = 0; i < MaxItemsPerPage; ++i)
            {
                var optionSlot = new ClickableComponent(new Rectangle(xPositionOnScreen + Game1.tileSize / 4, yPositionOnScreen + 84 + i * ((height - Game1.tileSize * 2) / MaxItemsPerPage) + Game1.tileSize / 4, width - Game1.tileSize / 2, (height - Game1.tileSize * 2) / MaxItemsPerPage + Game1.pixelZoom), string.Concat(i));
                OptionSlots.Add(optionSlot);
                var playButton = new ClickableTextureComponent("play", new Rectangle(optionSlot.bounds.X + optionSlot.bounds.Width - Game1.tileSize * 2, optionSlot.bounds.Y, 64, 64), "", "", Game1.mouseCursors, new Rectangle(175, 379, 16, 15), Game1.pixelZoom);
                PlayButtons.Add(playButton);
            }
        }

        private void SetScrollBarToCurrentIndex()
        {
            ScrollBarThumb.bounds.Y = (int)((ScrollBarTrack.Height - ScrollBarThumb.bounds.Height) * 1.0 * CurrentOffset / (Items.Count - MaxItemsPerPage));
            ScrollBarThumb.bounds.Y += ScrollBarTrack.Y;
        }

        private void ArrowUpPressed()
        {
            if (CurrentOffset > 0)
            {
                CurrentOffset--;
                SetScrollBarToCurrentIndex();
            }
        }

        private void ArrowDownPressed()
        {
            if (CurrentOffset < Items.Count - MaxItemsPerPage)
            {
                CurrentOffset++;
                SetScrollBarToCurrentIndex();
            }
        }

        private void TestSound(string name)
        {
            try
            {
                if (Config.Songs.ContainsKey(name))
                {
                    bool hasHeard = true;
                    if (!Game1.player.songsHeard.Contains(name)) hasHeard = false;
                    Game1.changeMusicTrack(name);
                    if (!hasHeard) Game1.player.songsHeard.Remove(name);
                    SelectedMusicTrack = name;
                }
                else
                {
                    ICue cue = Game1.soundBank.GetCue(name);
                    if (ActiveCue != null && ActiveCue.IsPlaying)
                        ActiveCue.Stop(AudioStopOptions.Immediate);
                    cue.Play();
                    ActiveCue = cue;
                    ActiveCueName = name;
                }
            }
            catch(Exception ex) { Monitor.Log($"{name} is either incorrect or doesn't exist", LogLevel.Error); }
        }

        private void StopActiveSound()
        {
            if(!string.IsNullOrWhiteSpace(ActiveCueName))
            {
                if (ActiveCue != null && ActiveCue.IsPlaying)
                    ActiveCue.Stop(AudioStopOptions.Immediate);
                ActiveCue = null;
                ActiveCueName = "";
            }

            if (!string.IsNullOrWhiteSpace(SelectedMusicTrack))
                if (Game1.getMusicTrackName() == SelectedMusicTrack)
                    Game1.changeMusicTrack("none");
        }

        private void UpdateConfig(OptionsCheckbox box)
        {
            StopActiveSound();
            if (Config.Sounds.ContainsKey(box.label))
                Config.Sounds[box.label] = box.isChecked;
            else if (Config.Songs.ContainsKey(box.label))
                Config.Songs[box.label] = box.isChecked;

            Helper.WriteConfig(Config);
            Monitor.Log($"Updated Config : { (box.isChecked ? "Disabled" : "Enabled") } { (Config.Sounds.ContainsKey(box.label) ? "Sound" : "Song") } {box.label}");
        }
    }
}