/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Prism-99/Think-n-Talk
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using Color = Microsoft.Xna.Framework.Color;
using StardewValley;
using StardewModdingAPI;
#if StardewWeb
using StardewWeb.Interfaces.SMAPIInt;
#elif Standalone
using SDV_Speaker.SMAPIInt;
#endif

//using StardewWeb.Utilities;
using Netcode;

using StardewModHelpers;

namespace SDV_Speaker.Speaker
{
    class BubbleGuy : NPC
    {
        StardewBitmap sbBackground = null;
        private readonly object oBackground = new object();
        private NetBool _IsThought = new NetBool();
        public readonly NetString _BubbleText = new NetString();
        private NetString _ModPath = new NetString();
        public List<string> lTextLines = null;
        private NetLong _PlayerId = new NetLong();
        private bool _bThoughtLoaded = false;
        private NetArray<int, NetInt> _BubbleImageData = new NetArray<int, NetInt>();

#if Standalone
        IMonitor monitor =SDV_Logger.Monitor;
#elif StardewWeb
        IMonitor monitor = SMAPI.ModDetails.Monitor;
#endif

        public BubbleGuy()
        {
#if LOG_DEBUG
            SDV_Logger.LogTrace("BubbleGuy"," empty contructor called.");
#endif
            base.forceOneTileWide.Value = true;
            base.name.Value = BubbleGuyStatics.BubbleGuyName;
            //_ModPath.Set("");
            //Text = "Blank";

            //base.NetFields.AddField(_Text);
            //base.NetFields.AddField(_IsThought);
        }
        public BubbleGuy(bool bIsThought, string sText, string sModDir) : this()
        {
            ModPath.Set(sModDir);
            _IsThought.Value = bIsThought;
            _IsThought.MarkDirty();
            _PlayerId.Value = Game1.player.UniqueMultiplayerID;
            _PlayerId.MarkDirty();
            SetTexture();
            Text = sText;
        }
        public BubbleGuy(bool bIsThought, string sText, Texture2D background) : this()
        {
            SDV_Logger.DumpObject("background", background);
            //ModPath.Set(sModDir);
            _IsThought.Value = bIsThought;
            _IsThought.MarkDirty();
            _PlayerId.Value = Game1.player.UniqueMultiplayerID;
            _PlayerId.MarkDirty();
            sbBackground = new StardewBitmap(background);
            sbBackground.ResizeImage(300, 200);
            sbBackground.FillArray(ref _BubbleImageData);
#if LOG_DEBUG
            monitor.Log($"_BubbleImageData length: {_BubbleImageData.Count}", LogLevel.Info);
#endif
            _BubbleImageData.MarkDirty();
            Text = sText;
            _bThoughtLoaded = _IsThought.Value;

        }

        protected override void initNetFields()
        {
#if LOG_DEBUG
            monitor.Log($"BubbleGuy initNetFields called. _BubbleText='{_BubbleText}'", LogLevel.Info);
#endif
            base.initNetFields();
            base.NetFields.AddFields(_BubbleText, _PlayerId, _IsThought, _BubbleImageData);
        }
        #region "Public Properties"

        public NetString ModPath
        {
            get
            {
#if LOG_DEBUG
                monitor.Log($"Getting modpath: '{_ModPath.Value}'", LogLevel.Info);
#endif
                if (_ModPath.Value == null || _ModPath.Value == "")
                {
                    _ModPath.Value = BubbleGuyStatics.ModPath;
                }
                return _ModPath;
            }
            set { _ModPath = value; }
        }
        public string Text
        {
            get { return _BubbleText; }
            set
            {
#if LOG_DEBUG
                monitor.Log("Setting Text", LogLevel.Info);
#endif
                _BubbleText.Value = value;
                _BubbleText.MarkDirty();
                //modData.Add(_ModDataKey, value);
                //birthday_Season.Value = value;
                //birthday_Season.MarkDirty();
                FormatText();
            }
        }
        public bool IsThought
        {
            get { return _IsThought.Value; }
            set
            {
                _IsThought.Value = value;
                _IsThought.MarkDirty();
                SetTexture();
            }
        }

        #endregion

        private void FormatText()
        {
            string[] arWords = (_BubbleText ?? "").Split(' ');
            int iCurLen = 0;
            List<string> lLineWords = new List<string> { };
            lTextLines = new List<string> { };

            foreach (string word in arWords)
            {
                if (iCurLen + word.Length + lLineWords.Count / 2 < 20)
                {
                    lLineWords.Add(word);
                    iCurLen += word.Length;
                }
                else
                {
                    lTextLines.Add(string.Join(" ", lLineWords));
                    iCurLen = 0;
                    lLineWords = new List<string> { word };
                }
            }
            if (lLineWords.Count > 0)
            {
                lTextLines.Add(string.Join(" ", lLineWords));
            }
        }
        private void SetTexture()
        {
            _bThoughtLoaded = _IsThought.Value;

            lock (oBackground)
            {

                if (_IsThought.Value)
                {
                    sbBackground = new StardewBitmap(Path.Combine(ModPath, "think_bubble.png"));
                }
                else
                {
                    sbBackground = new StardewBitmap(Path.Combine(ModPath, "talk_bubble.png"));
                }
                sbBackground.ResizeImage(300, 200);
                _BubbleImageData = sbBackground.TextureNetArray();

                //sbBackground.Save("bubbleguy.png");
            }
        }
        public override void update(GameTime time, GameLocation location)
        {
            try
            {
#if TRACE
                monitor.Log("BG update called", LogLevel.Info);
#endif
                //
                //  used to keep the bubble above the NPC
                //
                float newX = Game1.getFarmer(_PlayerId.Value).position.X;
                float newY = Game1.getFarmer(_PlayerId.Value).position.Y;
                position.Set(new Vector2(newX, newY));
            }
            catch { }
            //setTileLocation(Game1.player.getTileLocation());
        }
        public override void dayUpdate(int dayOfMonth)
        {

        }
        public override void updateMovement(GameLocation location, GameTime time)
        {
            try
            {
                //base.updateMovement(location, time);
                float newX = Game1.getFarmer(_PlayerId.Value).position.X + 20;
                float newY = Game1.getFarmer(_PlayerId.Value).position.Y - 10;
                position.Set(new Vector2(newX, newY));
                //setTileLocation(Game1.player.getTileLocation());
            }
            catch { }

        }
        public override bool CanSocialize => false;

        public override bool canTalk()
        {
            return false;
        }

        public override void doEmote(int whichEmote, bool playSound, bool nextEventCommand = true)
        {
        }

        
        public override void draw(SpriteBatch b)
        {

            try
            {
                Vector2 origin = new Vector2(8f, 8f);
                Vector2 bgPosition = new Vector2(Position.X - 100, Position.Y - 265);
                Vector2 txtPosition;
                if (sbBackground == null || _bThoughtLoaded != _IsThought.Value)
                {
#if LOG_DEBUG
                    monitor.Log($"Loading BG. Data length: {_BubbleImageData.Count}", LogLevel.Info);
#endif
                    try
                    {
                        _BubbleImageData.Tick();
                        sbBackground = new StardewBitmap(_BubbleImageData);
                        _bThoughtLoaded = _IsThought.Value;
                    }
                    catch (Exception ex)
                    {
                        //
                        //  if custom image fails, fallback to standard
                        //
#if LOG_DEBUG
                        monitor.Log($"Error loading BG: {ex}", LogLevel.Error);
#endif
                        SetTexture();
                    }
                }
                if (_IsThought.Value)
                {
                    txtPosition = new Vector2(Position.X - 80, Position.Y - 230);
                }
                else
                {
                    txtPosition = new Vector2(Position.X - 100, Position.Y - 265);
                }

                // float num = 1;
                float num2 = Math.Max(0f, (float)((bgPosition.Y + 1) - 24) / 10000f) + (float)bgPosition.X * 1E-05f;

                b.Draw(sbBackground.Texture(), Game1.GlobalToLocal(Game1.viewport, bgPosition), new Rectangle(0, 0, sbBackground.Width, sbBackground.Height), Color.White, 0f, origin, 1f, SpriteEffects.None, 0.99999f);

                if (lTextLines == null)
                {
#if LOG_DEBUG
                    monitor.Log($"_BubbleText: '{_BubbleText.Value}'", LogLevel.Info);
#endif
                    FormatText();
                }
                foreach (string line in lTextLines)
                {
                    Utility.drawTextWithShadow(b, line, Game1.smallFont, Game1.GlobalToLocal(Game1.viewport, txtPosition), Color.Black, 1f, 1f, -1, -1, 0.0f, 3);
                    txtPosition = new Vector2(txtPosition.X, txtPosition.Y + 25);
                }
            }
            catch { }
        }
    }
}