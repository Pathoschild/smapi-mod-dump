/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.Minigames;

namespace PyromancersJourney.Framework
{
    internal class PyromancerMinigame : IMinigame, IDisposable
    {
        private World World = new();

        public void Dispose()
        {
            World.Dispose();
        }

        public void changeScreenSize() { }

        public bool doMainGameUpdates()
        {
            return false;
        }

        public void draw(SpriteBatch b)
        {
            this.World.Render();
        }

        public bool forceQuit()
        {
            this.unload();

            return true;
        }

        public void leftClickHeld(int x, int y)
        {
        }

        public string minigameId()
        {
            return "PyromancerJourney";
        }

        public bool overrideFreeMouseMovement()
        {
            return false;
        }

        public void receiveEventPoke(int data) { }

        public void receiveKeyPress(Keys k) { }

        public void receiveKeyRelease(Keys k) { }

        public void receiveLeftClick(int x, int y, bool playSound = true) { }

        public void receiveRightClick(int x, int y, bool playSound = true) { }

        public void releaseLeftClick(int x, int y) { }

        public void releaseRightClick(int x, int y) { }

        public bool tick(GameTime time)
        {
            this.World.Update();
            return this.World.HasQuit;
        }

        public void unload()
        {
            this.World?.Dispose();
            this.World = null;
        }
    }
}
