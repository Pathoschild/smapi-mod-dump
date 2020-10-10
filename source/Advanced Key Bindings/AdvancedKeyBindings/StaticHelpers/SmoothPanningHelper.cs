/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Drachenkaetzchen/AdvancedKeyBindings
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace AdvancedKeyBindings.StaticHelpers
{
    public class SmoothPanningHelper
    {
        private Vector2 _targetCoordinate;
        private Vector2 _sourceCoordinate;
        private bool _currentlyPanning = false;
        private float _panProgress;
        private static SmoothPanningHelper _smoothPanningHelperInstance;

        private Action _afterPanCallback;
        
        private SmoothPanningHelper(IModHelper helper)
        {
            helper.Events.GameLoop.UpdateTicked += GameLoopOnUpdateTicked;
        }

        public void RelativePanTo(int x, int y)
        {
            _targetCoordinate = new Vector2(Game1.viewport.X + x,Game1.viewport.Y + y);
            _sourceCoordinate = new Vector2(Game1.viewport.X, Game1.viewport.Y);
            _panProgress = 0;
            _currentlyPanning = true;
            _afterPanCallback = null;
        }
        
        public void AbsolutePanTo(int x, int y)
        {
            _targetCoordinate = new Vector2(x, y);
            _sourceCoordinate = new Vector2(Game1.viewport.X, Game1.viewport.Y);
            _panProgress = 0;
            _currentlyPanning = true;
            _afterPanCallback = null;
        }

        public void AbsolutePanTo(int x, int y, Action callback)
        {
            AbsolutePanTo(x,y);
            _afterPanCallback = callback;
        }
        
        
        private void GameLoopOnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (_currentlyPanning)
            {
                var doCallback = false;
                _panProgress += 0.1f;
                
                if (_panProgress >= 1)
                {
                    _panProgress = 1;
                    _currentlyPanning = false;
                    doCallback = true;
                }
                
                var foo = Vector2.SmoothStep(_sourceCoordinate, _targetCoordinate, _panProgress);
               
                
                PanScreen((int) foo.X, (int) foo.Y);

                if (doCallback)
                {
                    _afterPanCallback?.Invoke();

                    _afterPanCallback = null;
                }

            }
        }

        public static void Initialize(IModHelper modHelper)
        {
            _smoothPanningHelperInstance = new SmoothPanningHelper(modHelper);
        }
        public static SmoothPanningHelper GetInstance()
        {
            if (_smoothPanningHelperInstance == null)
            {
                throw new Exception("The smooth panning class has not been initialized. Use Initialize() first.");
            }
            return _smoothPanningHelperInstance;
        }
        
        private static void PanScreen(int x, int y)
        {
            Game1.previousViewportPosition.X = (float) Game1.viewport.Location.X;
            Game1.previousViewportPosition.Y = (float) Game1.viewport.Location.Y;
            Game1.viewport.X = x;
            Game1.viewport.Y = y;
            Game1.clampViewportToGameMap();
            Game1.updateRaindropPosition();
        }
    }
}