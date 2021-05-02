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
using static StardustCore.Events.EventHelper;

namespace StardustCore.Events
{
    public class EventStartData
    {
        /// <summary>
        /// Data pertaining to npcs information necessary for the event.
        /// </summary>
        public class NPCData
        {
            private NPC npc;
            int xPosition;
            int yPosition;
            EventHelper.FacingDirection direction;
            public NPCData()
            {

            }
            public NPCData(NPC NPC, int XTile, int YTile, EventHelper.FacingDirection Direction)
            {
                this.npc = NPC;
                this.xPosition = XTile;
                this.yPosition = YTile;
                this.direction = Direction;
            }

            public override string ToString()
            {
                StringBuilder b = new StringBuilder();
                b.Append(this.npc.Name);
                b.Append(" ");
                b.Append(this.xPosition.ToString());
                b.Append(" ");
                b.Append(this.yPosition.ToString());
                b.Append(" ");
                b.Append(this.getFacingDirectionNumber(this.direction).ToString());
                return b.ToString();
            }

            protected int getFacingDirectionNumber(FacingDirection Dir)
            {
                return (int)Dir;
            }
        }


        /// <summary>
        /// Data pertaining to the farmer data for the event.
        /// </summary>
        public class FarmerData
        {
            int xPosition;
            int yPosition;
            EventHelper.FacingDirection direction;
            public FarmerData()
            {

            }
            /// <summary>
            ///Constructor. 
            /// </summary>
            /// <param name="XTile"></param>
            /// <param name="YTile"></param>
            /// <param name="Direction"></param>
            public FarmerData(int XTile, int YTile, EventHelper.FacingDirection Direction)
            {
                this.xPosition = XTile;
                this.yPosition = YTile;
                this.direction = Direction;
            }

            /// <summary>
            /// Gets a string representation of the farmer's starting data.
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                StringBuilder b = new StringBuilder();
                b.Append("farmer");
                b.Append(" ");
                b.Append(this.xPosition.ToString());
                b.Append(" ");
                b.Append(this.yPosition.ToString());
                b.Append(" ");
                b.Append(((int)this.direction).ToString());
                return b.ToString();

            }
        }

        /// <summary>
        /// The string builder to output the information.
        /// </summary>
        private StringBuilder builder;

        /// <summary>
        /// A special variable to determined if to continue the music that is already playing or to turn off the music.
        /// </summary>
        public enum MusicToPlayType
        {
            None,
            Continue,
        }


        public EventStartData()
        {
            this.builder = new StringBuilder();
        }

        /// <summary>
        /// Create the start data necessary for the event.
        /// </summary>
        /// <param name="MusicType">A special type to determine what music is played. None or Continue.</param>
        /// <param name="CameraTileX">The starting xtile for the camera</param>
        /// <param name="CameraTileY">The starting y tile for the camera</param>
        /// <param name="Farmer">The farmer data for the event. If null then the farmer won't be in this event.</param>
        /// <param name="NPCS">The npc data for the event. If null then no npcs will be in the event.</param>
        public EventStartData(MusicToPlayType MusicType, int CameraTileX, int CameraTileY, FarmerData Farmer, List<NPCData> NPCS)
        {
            this.builder = new StringBuilder();
            if(MusicType== MusicToPlayType.None)
            {
                this.add("none");
            }

            if(MusicType== MusicToPlayType.Continue)
            {
                this.add("continue");
            }

            this.add(CameraTileX.ToString());
            this.builder.Append(" ");
            this.builder.Append(CameraTileY.ToString());


            StringBuilder npcData = new StringBuilder();
            if (Farmer != null)
            {
                npcData.Append(Farmer.ToString());
            }
            if (NPCS != null)
            {
                foreach(var v in NPCS)
                {
                    npcData.Append(" ");
                    npcData.Append(v.ToString());
                }
            }
            this.add(npcData.ToString());
            this.add("skippable");

        }

        /// <summary>
        /// Create the start data necessary for the event.
        /// </summary>
        /// <param name="SongToPlay">The name of the song to play.</param>
        /// <param name="CameraTileX">The starting xtile for the camera</param>
        /// <param name="CameraTileY">The starting y tile for the camera</param>
        /// <param name="Farmer">The farmer data for the event. If null then the farmer won't be in this event.</param>
        /// <param name="NPCS">The npc data for the event. If null then no npcs will be in the event.</param>
        public EventStartData(string SongToPlay, int CameraTileX, int CameraTileY, FarmerData Farmer, List<NPCData> NPCS)
        {
            this.builder = new StringBuilder();
            this.add(SongToPlay);
            this.add(CameraTileX.ToString());
            this.builder.Append(" ");
            this.builder.Append(CameraTileY.ToString());

            StringBuilder npcData = new StringBuilder();
            if (Farmer != null)
            {
                npcData.Append(Farmer.ToString());
            }
            if (NPCS != null)
            {
                foreach (var v in NPCS)
                {
                    npcData.Append(" ");
                    npcData.Append(v.ToString());
                }
            }
            this.add(npcData.ToString());
            this.add("skippable");

        }

        /// <summary>
        /// Adds the data to a string builder to seperate out the data.
        /// </summary>
        /// <param name="Data"></param>
        public virtual void add(string Data)
        {
            if (this.builder.Length > 0)
            {
                this.builder.Append(this.getSeperator());
            }
            this.builder.Append(Data);
        }

        /// <summary>
        /// The seperator character for events.
        /// </summary>
        /// <returns></returns>
        public string getSeperator()
        {
            return "/";
        }

        /// <summary>
        /// Returns the event data.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.builder.ToString();
        }
    }
}
