/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyRoguelike
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using xTile.Dimensions;

namespace StardewRoguelike.ChallengeFloors
{
    public class ChallengeBase : INetObject<NetFields>, IDisposable
    {
        protected ChallengeBase()
        {
            initNetFields();
        }

        public NetFields NetFields { get; } = new();

        protected virtual void initNetFields() { }

        public virtual List<string> MapPaths { get; } = null;

        public virtual List<string> MusicTracks { get; } = null;

        public virtual Vector2? SpawnLocation { get; } = null;

        // Runs only for the main player
        public virtual void Initialize(MineShaft mine) { }

        // Runs for all players
        public virtual void Update(MineShaft mine, GameTime time) { }

        // Runs only for Game1.player
        public virtual void PlayerEntered(MineShaft mine)
        {
            ModEntry.Events.GameLoop.ReturnedToTitle += DisposeHandler;
        }

        // Runs only for Game1.player
        public virtual void PlayerLeft(MineShaft mine)
        {
            ModEntry.Events.GameLoop.ReturnedToTitle -= DisposeHandler;
        }

        public virtual bool AnswerDialogueAction(MineShaft mine, string questionAndAnswer, string[] questionParams) => false;

        public virtual void DrawBeforeLocation(MineShaft mine, SpriteBatch b) { }

        public virtual void DrawAfterLocation(MineShaft mine, SpriteBatch b) { }

        public virtual bool CheckForCollision(MineShaft mine, Microsoft.Xna.Framework.Rectangle position, Farmer who) => false;

        public virtual bool CheckAction(MineShaft mine, Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who) => false;

        public virtual bool PerformAction(MineShaft mine, string action, Farmer who, Location tileLocation) => false;

        public virtual bool ShouldSpawnLadder(MineShaft mine) => true;

        private void DisposeHandler(object sender, ReturnedToTitleEventArgs e)
        {
            Dispose();
            ModEntry.Events.GameLoop.ReturnedToTitle -= DisposeHandler;
        }

        public virtual void Dispose() { }
    }
}
