/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/zack-hill/stardew-valley-stash-items
**
*************************************************/

using System;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Panstorm.Extensions;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Tools;

namespace Panstorm
{
    public class ModEntry : Mod
    {
        private static ModConfig Config { get; set; }
        
        private Point? _panPoint;
        private ICue _music;
        private bool _hasLoadedMusic;

        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<ModConfig>();
            
            helper.Events.GameLoop.UpdateTicked += GameLoopOnUpdateTicked;
            helper.Events.Input.ButtonsChanged += OnButtonsChanged;
            helper.Events.Display.RenderedHud += DisplayOnRenderedHud;

            RegisterMusicCue();
        }

        private void RegisterMusicCue()
        {
            if (!Config.PlayAudio)
                return;
            
            var cueDefinition = new CueDefinition
            {
                name = "Panstorm",
                instanceLimit = 1,
                limitBehavior = CueDefinition.LimitBehavior.ReplaceOldest
            };
            SoundEffect audio;
            var filePathCombined = Path.Combine(Helper.DirectoryPath, "assets/Panstorm.wav");
            using (var stream = new FileStream(filePathCombined, FileMode.Open)) {
                audio = SoundEffect.FromStream(stream);
            }
            cueDefinition.SetSound(audio, Game1.audioEngine.GetCategoryIndex("Music"));
            Game1.soundBank.AddCue(cueDefinition);
            
            _music = Game1.soundBank.GetCue("Panstorm");
        }

        private void PreloadMusic()
        {
            if (!Config.PlayAudio)
                return;
            
            if (_hasLoadedMusic)
                return;
            
            if (_music.IsPlaying)
            {
                _music.Stop(AudioStopOptions.Immediate);
                _hasLoadedMusic = true;
            }
            else
            {
                _music.Play();
            }
        }
        
        private void GameLoopOnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            PreloadMusic();
            
            var panPoint = Game1.player?.currentLocation?.orePanPoint?.Value;
            
            if (_panPoint != panPoint)
            {
                if (Config.PlayAudio)
                {
                    if (_panPoint == null || panPoint == Point.Zero)
                    {
                        if (_music.IsPlaying)
                        {
                            _music.Stop(AudioStopOptions.Immediate);
                        }
                    }
                    else
                    {
                        _music.Play();
                        _music.Volume = Config.Volume;
                    }
                }

                _panPoint = panPoint;
            }
        }
        
        private void OnButtonsChanged(object sender, ButtonsChangedEventArgs e)
        {
            if (e.Pressed.Contains(Config.StopPlayingButton))
            {
                if (_music.IsPlaying)
                {
                    _music.Stop(AudioStopOptions.Immediate);
                }
            }
        }

        private void DisplayOnRenderedHud(object sender, RenderedHudEventArgs e)
        {
            if (_panPoint == null)
                return;
            
            var panPoint = _panPoint.Value;

            if (Config.DisplayMode == DisplayMode.Disabled ||
                (Config.DisplayMode == DisplayMode.EnabledWhenHoldingPan && Game1.player.CurrentTool is not Pan))
                return;

            var textColor = Color.White;
            var font = Game1.smallFont;

            // Draw the panning info GUI to the screen
            float boxWidth = 0;
            float lineHeight = font.LineSpacing;
            var boxTopLeft = new Vector2(Config.DisplayXOffset, Config.DisplayYOffset);
            var boxBottomLeft = boxTopLeft;

            // Setup the sprite batch
            var batch = Game1.spriteBatch;
            batch.End();
            batch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);

            var hudTextLine1 = "Found a panning spot!"; 
            var hudTextLine2 = "";
            var hudTextLine3 = "";

            if (panPoint.Equals(Point.Zero))
            {
                hudTextLine1 = "No panning spot found!";
            }
            else
            {
                var relativePosition = GetCardinalDirection(panPoint);
                var distance = GetDistanceToPoint(panPoint);
                hudTextLine2 = $"In the {relativePosition} direction.";
                hudTextLine3 = $"Roughly {distance} tiles away.";
            }

            batch.DrawStringWithShadow(font, hudTextLine1, boxBottomLeft, textColor, 1.0f);
            boxWidth = Math.Max(boxWidth, font.MeasureString(hudTextLine1).X + 5);
            boxBottomLeft += new Vector2(0, lineHeight);

            if (hudTextLine2 != string.Empty)
            {
                batch.DrawStringWithShadow(font, hudTextLine2, boxBottomLeft, textColor, 1.0f);
                boxWidth = Math.Max(boxWidth, font.MeasureString(hudTextLine2).X + 5);
                boxBottomLeft += new Vector2(0, lineHeight);
            }

            if (hudTextLine3 != string.Empty)
            {
                batch.DrawStringWithShadow(font, hudTextLine3, boxBottomLeft, textColor, 1.0f);
                boxWidth = Math.Max(boxWidth, font.MeasureString(hudTextLine3).X + 5);
                boxBottomLeft += new Vector2(0, lineHeight);
            }

            var box = Game1.staminaRect;
            // Draw the background rectangle DrawHelpers.WhitePixel
            batch.Draw(box,
                new Rectangle(
                    (int)boxTopLeft.X, 
                    (int)boxTopLeft.Y, 
                    (int)boxWidth,
                    (int)(boxBottomLeft.Y - boxTopLeft.Y)), 
                null,
                new Color(0, 0, 0, 0.25F), 
                0f, 
                Vector2.Zero, 
                SpriteEffects.None, 
                0.85F);

            batch.End();
            batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None,
                RasterizerState.CullCounterClockwise);
        }

        private static string GetCardinalDirection(Point orePoint)
        {
            if (orePoint.X == Game1.player.getTileX())
            {
                return orePoint.Y < Game1.player.getTileY()
                    ? "N"
                    : "S";
            }

            if (orePoint.Y == Game1.player.getTileY())
            {
                return orePoint.X < Game1.player.getTileX()
                    ? "W"
                    : "E";
            }

            if (orePoint.X < Game1.player.getTileX())
            {
                return orePoint.Y < Game1.player.getTileY()
                    ? "NW"
                    : "SW";
            }

            return orePoint.Y < Game1.player.getTileY()
                ? "NE"
                : "SE";
        }

        private static long GetDistanceToPoint(Point orePoint)
        {
            return (long)Math.Round(Math.Sqrt(Math.Pow(orePoint.X - Game1.player.getTileX(), 2) +
                                              Math.Pow(orePoint.Y - Game1.player.getTileY(), 2)));
        }
    }
}
