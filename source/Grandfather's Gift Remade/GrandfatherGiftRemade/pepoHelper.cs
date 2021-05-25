/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://sourceforge.net/projects/sdvmod-grandfather-s-gift-re/
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace pepoHelper
{
    static class pepoCommon
    {
        public static IMonitor monitor = null;
        public static SpriteBatch spriteBatch;

        public static void LogTr(string message)
        {
            pepoCommon.monitor?.Log(message, LogLevel.Trace);
        }
    }

    /// <summary>
    /// Some static methods to help draw things on screen
    /// </summary>
    static class pepoDrawer
    {
        public static int MaxCharWidth(this Rectangle area, SpriteFont font, string c = "!") {
            // Used to pre-allocate StringBuilder size to 4 * max possible number of chars in a line
            // the "4" constant allows unicode chars.
            return 4 * area.X / (int)Math.Ceiling(font.MeasureString(c).X);
        }

        public static float LineHeight(this SpriteFont font, int lineHeightPercent = 100) {
            // "M" is the boxiest letter: tallest and widest.
            return font.MeasureString("M").Y * lineHeightPercent / 100.0f;
        }

        public static void MultilineString(
            SpriteFont font, string message, Rectangle area,
            int lineHeightPercent = 110, SpriteBatch b = null)
            {
            if (b != null) pepoCommon.spriteBatch = b;
            b = pepoCommon.spriteBatch;
            float lineHeight = font.LineHeight(lineHeightPercent);

            Vector2 curPos = new Vector2(area.X, area.Y);
            StringBuilder workSB = new StringBuilder(area.MaxCharWidth(font));
            foreach (char c in message) {
                workSB.Append(c);
                if (font.MeasureString(workSB).X > area.Width) {
                    // remove last character, from https://stackoverflow.com/a/17215160/149900
                    workSB.Length--;
                    b.DrawString(font, workSB, curPos, Color.Black);
                    curPos.Y += lineHeight;
                    workSB.Clear();
                    workSB.Append(c);
                }
            }
            b.DrawString(font, workSB, curPos, Color.Black);
            return;
        }

        public static void MultilineStringWithWordWrap(
            SpriteFont font, string message, Rectangle area,
            int lineHeightPercent = 110, SpriteBatch b = null)
            {
            if (b != null) pepoCommon.spriteBatch = b;
            b = pepoCommon.spriteBatch;
            float lineHeight = font.LineHeight(lineHeightPercent);

            Vector2 curPos = new Vector2(area.X, area.Y);
            StringBuilder workStr = new StringBuilder(area.MaxCharWidth(font));
            foreach (string chunkStr in message.Split())
            {
                workStr.Append(" ");
                workStr.Append(chunkStr);
                if (font.MeasureString(workStr).X > area.Width)
                {
                    workStr.Length -= chunkStr.Length;
                    workStr.Length--;
                    b.DrawString(font, workStr, curPos, Color.Black);
                    curPos.Y += lineHeight;
                    workStr.Clear();
                    workStr.Append(chunkStr);
                }
            }
            b.DrawString(font, workStr, curPos, Color.Black);
            return;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Reliability", "CA2000:Dispose objects before losing scope",
            Justification = "The main game thread requires the Texture2D object to not auto-dispose"
            )]
        public static void BlackScreen(SpriteBatch b = null)
        {
            if (b != null) pepoCommon.spriteBatch = b;
            b = pepoCommon.spriteBatch;
            // Too small, and the drawing engine takes too much CPU time. But too big, the drawing engine
            // probably will push the graphics driver too heavily. Need to find a right balance here...
            const int TEXTURE_SIZE = 16;
            const uint TEXTURE_DATA_ARGB = 0xffffffff;
            Rectangle target = new Rectangle(0, 0, Game1.viewport.Width, Game1.viewport.Height);
            Texture2D flatblank = new Texture2D(Game1.graphics.GraphicsDevice, TEXTURE_SIZE, TEXTURE_SIZE);
            uint[] data = new uint[TEXTURE_SIZE * TEXTURE_SIZE];
            for (int i = 0; i < data.Length; i++) {
                data[i] = TEXTURE_DATA_ARGB;
            }
            flatblank.SetData<uint>(data);
            Color black = new Color(0, 0, 0);
            b.Draw(flatblank, target, black);
        }
    }

    /// <summary>
    /// Draws a DialogueBox "menu" on top of a black background
    /// </summary>
    /// <remarks>
    /// This is done by overriding the draw() method of the DialogueBox class
    /// </remarks>
    internal class DialogOnBlack : DialogueBox
    {
        public DialogOnBlack(string dialog) : base(dialog)
        {
        }

        public override void draw(SpriteBatch b)
        {
            pepoDrawer.BlackScreen(b);
            base.draw(b);
        }
    }

    /// <summary>
    /// Some simple extension methods to the Farmer class for handling otherwise-too-verbose procedures
    /// </summary>
    public static class FarmerExtension
    {
        /// <summary>
        /// Move Farmer by (h, v) tiles.
        /// </summary>
        /// <param name="f">The farmer object that will be extended</param>
        /// <param name="h">Number of horizontal tiles to move. Positive = rightward</param>
        /// <param name="v">Number of vertical tiles to move. Positive = downward</param>
        /// <param name="faceDir">If specified, represents the final heading of the farmer. 1234 = R?L?</param>
        public static void moveRelTiles(this Farmer f, int h=0, int v=0, int faceDir=-1)
        {
            if(f == null) throw new ArgumentNullException(nameof(f), "Farmer object cannot be null!");
            pepoCommon.LogTr($"farmer was at ({f.getTileX()},{f.getTileY()})");
            f.setTileLocation(f.relTiles(h: h, v: v));
            pepoCommon.LogTr($"farmer now at ({f.getTileX()},{f.getTileY()})");
            if (faceDir != -1) f.faceDirection(faceDir);
        }

        /// <summary>
        /// Return the tile coordinate relative to the farmer
        /// </summary>
        /// <param name="f">The farmer object that will be extended</param>
        /// <param name="h">Number of horizontal tiles relative to farmer. Positive = rightward</param>
        /// <param name="v">Number of vertical tiles reletive to farmer. Positive = downward</param>
        /// <returns>A Vector2 object containing the actual coordinates of the target tile</returns>
        public static Vector2 relTiles(this Farmer f, int h=0, int v=0)
        {
            if(f == null) throw new ArgumentNullException(nameof(f), "Farmer object cannot be null!");
            return new Vector2(f.getTileX() + h, f.getTileY() + v);
        }

    }

    /// <summary>
    /// Chains several "menus" so they appear one after another
    /// </summary>
    /// <remarks>
    /// Use the Add() method to add the menus to chain.
    /// </remarks>
    public class MenuChainer
    {
        /* Properties Publique */
        public Queue<IClickableMenu> menus { get; } = new Queue<IClickableMenu> { };
        public IDisplayEvents dispEvt { get; set; } = null;
        public bool chainBegun { get { return _chainBegun; } }
        public bool menuCleared { get { return _menuCleared; } }

        /* Internal thingies */
        private bool _chainBegun = false;
        private bool _menuCleared = false;

        public MenuChainer() { }

        public void Start(IDisplayEvents displayEvents)
        {
            if (displayEvents == null) throw new ArgumentNullException(nameof(displayEvents));
            _chainBegun = true;
            _menuCleared = false;
            pepoCommon.LogTr("starting menuchain");
            (dispEvt = displayEvents).MenuChanged += OnMenuChanged;
            pepoCommon.LogTr("registered for MenuChanged event");
            var first = menus.Dequeue();
            Game1.activeClickableMenu = first;
            pepoCommon.LogTr($"displayed first menu {first}");
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Design", "CA1062:Validate arguments of public methods",
            Justification = "This is an event; it's unpossible for any of the args to be null"
            )]
        public void OnMenuChanged(object sender, MenuChangedEventArgs e)
        // Remember: This event is triggered twice per menu; once when the menu is displayed,
        // and once when the menu is dismissed.
        {
            pepoCommon.LogTr($"MenuChanged e.NewMenu = {e.NewMenu}");
            if (e.OldMenu == null) {
                // if OldMenu == null, this is a newly-displayed menu. Do NOT change
                // the active menu. Also, do NOT try to execute exitFunction.
                pepoCommon.LogTr("No OldMenu, this is a newly-diplayed menu.");
                return;
            }
            pepoCommon.LogTr($"MenuChanged e.OldMenu = {e.OldMenu}");
            pepoCommon.LogTr($"Trying to invoke {e.OldMenu} exitFunction, if any");
            e.OldMenu.exitFunction?.Invoke();
            // the next line should no longer be necessary since we're using a Queue
            // but I'm keeping the line here so I can still remember where to do this
            // just in case we need to go back to using List<>
            // menus.Remove(e.OldMenu);
            if (menus.Count > 0)
            {
                pepoCommon.LogTr($"still have {menus.Count} menu(s) in chain");
                var next = menus.Dequeue();
                Game1.activeClickableMenu = next;
                pepoCommon.LogTr($"displayed next menu {next}");
            }
            else
            {
                pepoCommon.LogTr($"no more menus in chain");
                dispEvt.MenuChanged -= OnMenuChanged;
                pepoCommon.LogTr("deregistered from MenuChanged event");
                _chainBegun = false;
                pepoCommon.LogTr("indicate menu chain has ended");
                _menuCleared = true;
                pepoCommon.LogTr("indicate last menu has been cleared");
            }
        }

        /// <summary>
        /// Add a menu (object that implements IClickableMenu) to the chain
        /// </summary>
        /// <param name="menuParams">One or more menu objects</param>
        /// <remarks>
        /// This method uses the "params" keyword, so just enumerate the menu objects
        /// as arguments to this method. DO NOT put them in a collection!
        /// </remarks>
        public void Add(params IClickableMenu[] menuParams) {
            pepoCommon.LogTr($"adding {menuParams.Length} menus");
            foreach (IClickableMenu m in menuParams) {
                menus.Enqueue(m);
            }
            pepoCommon.LogTr($"now we have {menus.Count} menus");
        }
    }

}
