/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/PurrplingMod
**
*************************************************/

using Microsoft.Xna.Framework;
using NpcAdventure.Internal.Assets;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;

namespace NpcAdventure.Driver
{
    public class HintDriver
    {
        public enum Hint
        {
            NONE,
            DIALOGUE,
            GIFT,
            ASK2FOLLOW,
        }

        public event EventHandler<CheckHintArgs> CheckHint;

        private Hint showHint;
        public ICursorPosition CursorPosition { get; private set; }
        public HintDriver(IModEvents events)
        {
            events.Input.CursorMoved += this.Input_CursorMoved;
            events.GameLoop.UpdateTicking += this.Update;
        }

        public void ResetHint()
        {
            this.showHint = Hint.NONE;
        }

        public void ShowHint(Hint hint)
        {
            this.showHint = hint;
        }

        private void Input_CursorMoved(object sender, CursorMovedEventArgs e)
        {
            this.CursorPosition = e.NewPosition;

            if (!Context.IsWorldReady || Game1.currentLocation?.currentEvent != null)
                return;

            Vector2 cursorTile = e.OldPosition.Tile;
            GameLocation location = Game1.currentLocation;
            NPC n = location?.isCharacterAtTile(cursorTile);

            if (n == null)
            {
                // Try next Y position if no NPC fetched
                n = location.isCharacterAtTile(cursorTile + new Vector2(0f, 1f));

                if (n == null)
                {
                    this.ResetHint();
                    return;
                }
            }

            this.OnCheckHint(n);
        }

        private void Update(object sender, UpdateTickingEventArgs e)
        {

            if (!Context.IsWorldReady)
                return;

            switch (this.showHint)
            {
                case Hint.DIALOGUE:
                    Game1.mouseCursor = 4;
                    break;
                case Hint.GIFT:
                    Game1.mouseCursor = 3;
                    break;
                case Hint.ASK2FOLLOW:
                    Game1.mouseCursor = AskToFollowCursor.TILE_POSITION;
                    break;
                default:
                    Game1.mouseCursor = 0;
                    break;
            }

            if (this.showHint != Hint.NONE)
            {
                Vector2 tileLocation = this.CursorPosition.Tile;
                Game1.mouseCursorTransparency = !Utility.tileWithinRadiusOfPlayer((int)tileLocation.X, (int)tileLocation.Y, 1, Game1.player) ? 0.5f : 1f;
            }

            Game1.updateCursorTileHint();
        }

        private void OnCheckHint(NPC forNPC)
        {
            if (this.CheckHint == null)
                return;

            CheckHintArgs args = new CheckHintArgs()
            {
                Npc = forNPC,
            };

            this.CheckHint(this, args);
        }
    }

    public class CheckHintArgs
    {
        public NPC Npc { get; set; }
    }
}
