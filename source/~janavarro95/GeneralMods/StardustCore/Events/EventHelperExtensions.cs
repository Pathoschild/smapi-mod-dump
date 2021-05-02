/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Characters;
using StardustCore.Utilities;

namespace StardustCore.Events
{
    public static class EventHelperExtensions
    {
        /// <summary>
        /// Creates the event string to add the object to the player's inventory.
        /// </summary>
        /// <returns></returns>
        public static void addObjectToPlayersInventory(this EventHelper EventHelper,int ParentSheetIndex, int Amount=1, bool MakeActiveObject=false)
        {
            StringBuilder b = new StringBuilder();
            b.Append("Omegasis.EventFramework.AddObjectToPlayersInventory ");
            b.Append(ParentSheetIndex);
            b.Append(" ");
            b.Append(Amount);
            b.Append(" ");
            b.Append(MakeActiveObject);
            EventHelper.add(b);
        }

        public static void ViewportLerp(this EventHelper EventHelper, Point NewPosition, double Speed)
        {
            StringBuilder b = new StringBuilder();
            b.Append("Omegasis.EventFramework.ViewportLerp ");
            b.Append(NewPosition.X);
            b.Append(" ");
            b.Append(NewPosition.Y);
            b.Append(" ");
            b.Append(Speed);
            EventHelper.add(b);

        }

        public static void ViewportLerpTile(this EventHelper EventHelper, Point NewTilePosition, double Speed)
        {
            StringBuilder b = new StringBuilder();
            b.Append("Omegasis.EventFramework.ViewportLerp ");
            b.Append(NewTilePosition.X*Game1.tileSize);
            b.Append(" ");
            b.Append(NewTilePosition.Y*Game1.tileSize);
            b.Append(" ");
            b.Append(Speed);
            EventHelper.add(b);

        }

        /// <summary>
        /// Lerps the camera an offset tile amount.
        /// </summary>
        /// <param name="EventHelper"></param>
        /// <param name="NewTilePositionOffset"></param>
        /// <param name="Speed">How many frames (aka update ticks) it takes to finish. Aka 60~=1 second</param>
        public static void ViewportLerpTileOffset(this EventHelper EventHelper, Point NewTilePositionOffset, int Frames=60,bool Concurrent=false)
        {
            StringBuilder b = new StringBuilder();
            b.Append("Omegasis.EventFramework.ViewportLerp ");
            b.Append((NewTilePositionOffset.X * Game1.tileSize));
            b.Append(" ");
            b.Append((NewTilePositionOffset.Y * Game1.tileSize));
            b.Append(" ");
            b.Append(Frames);
            b.Append(" ");
            b.Append(Concurrent);
            EventHelper.add(b);

        }

        /// <summary>
        /// Creates the code to add in a junimo actor at the given location.
        /// </summary>
        /// <param name="EventHelper"></param>
        /// <param name="ActorName"></param>
        /// <param name="Position"></param>
        /// <param name="Color"></param>
        public static void AddInJunimoActor(this EventHelper EventHelper,string ActorName,Vector2 Position,Color Color,bool Flipped=false)
        {

            StringBuilder b = new StringBuilder();
            b.Append("Omegasis.EventFramework.AddInJunimoActor ");
            b.Append(ActorName);
            b.Append(" ");
            b.Append(Position.X);
            b.Append(" ");
            b.Append(Position.Y);
            b.Append(" ");
            b.Append(Color.R);
            b.Append(" ");
            b.Append(Color.G);
            b.Append(" ");
            b.Append(Color.B);
            b.Append(" ");
            b.Append(Flipped);
            EventHelper.add(b);
        }

        public static void FlipJunimoActor(this EventHelper EventHelper, string ActorName,bool Flipped = false)
        {
            StringBuilder b = new StringBuilder();
            b.Append("Omegasis.EventFramework.FlipJunimoActor ");
            b.Append(ActorName);
            b.Append(" ");
            b.Append(Flipped);
            EventHelper.add(b);
        }
        public static void SetUpJunimoAdvanceMove(this EventHelper EventHelper)
        {
            StringBuilder b = new StringBuilder();
            b.Append("Omegasis.EventFramework.SetUpAdvanceJunimoMovement");
            EventHelper.add(b);
        }

        public static void FinishJunimoAdvanceMove(this EventHelper EventHelper)
        {
            StringBuilder b = new StringBuilder();
            b.Append("Omegasis.EventFramework.FinishAdvanceJunimoMovement");
            EventHelper.add(b);
        }

        public static void AddJunimoAdvanceMove(this EventHelper EventHelper, JunimoAdvanceMoveData JunimoData)
        {
            StringBuilder b = new StringBuilder();
            b.Append("Omegasis.EventFramework.AddInJunimoAdvanceMove ");

            b.Append(JunimoData.junimoActorID);
            b.Append(" ");
            b.Append(JunimoData.maxFrames);
            b.Append(" ");
            b.Append(JunimoData.tickSpeed);
            b.Append(" ");
            b.Append(JunimoData.loop);
            b.Append(" ");
            for (int i = 0; i < JunimoData.points.Count; i++)
            {
                b.Append(JunimoData.points[i].X);
                b.Append("_");
                b.Append(JunimoData.points[i].Y);
                if (i != JunimoData.points.Count - 1)
                {
                    b.Append(" ");
                }
            }

            EventHelper.add(b);
        }

        /// <summary>
        /// Same as above but allows for smaller tile position numbers instead.
        /// </summary>
        /// <param name="EventHelper"></param>
        /// <param name="JunimoData"></param>
        public static void AddJunimoAdvanceMoveTiles(this EventHelper EventHelper, JunimoAdvanceMoveData JunimoData)
        {
            StringBuilder b = new StringBuilder();
            b.Append("Omegasis.EventFramework.AddInJunimoAdvanceMove ");

            b.Append(JunimoData.junimoActorID);
            b.Append(" ");
            b.Append(JunimoData.maxFrames);
            b.Append(" ");
            b.Append(JunimoData.tickSpeed);
            b.Append(" ");
            b.Append(JunimoData.loop);
            b.Append(" ");
            for (int i = 0; i < JunimoData.points.Count; i++)
            {
                b.Append(JunimoData.points[i].X*Game1.tileSize);
                b.Append("_");
                b.Append(JunimoData.points[i].Y * Game1.tileSize);
                if (i != JunimoData.points.Count - 1)
                {
                    b.Append(" ");
                }
            }

            EventHelper.add(b);
        }


        public static void RemoveJunimoAdvanceMove(this EventHelper EventHelper, string ActorName)
        {
            StringBuilder b = new StringBuilder();
            b.Append("Omegasis.EventFramework.RemoveJunimoAdvanceMove ");
            b.Append(ActorName);
            b.Append(" ");
            EventHelper.add(b);
        }

    }
}
