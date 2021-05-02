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
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Characters;
using StardustCore.Utilities;

namespace StardustCore.Events
{
    /// <summary>
    /// Contains functions that are used to parse event data and do additional things.
    /// </summary>
    public class ExtraEventActions
    {

        private static Point OldViewportPosition;
        private static bool StartedLerp;
        private static int CurrentViewportLerpAmount;

        public static Dictionary<string, JunimoAdvanceMoveData> junimoLerpData = new Dictionary<string, JunimoAdvanceMoveData>();

        /// <summary>
        /// Adds the item from Game1.ObjectInformation to the player's inventory from the given event string.
        /// </summary>
        /// <param name="EventData"></param>
        public static void addObjectToPlayerInventory(EventManager EventManager,string EventData)
        {
            string[] splits = EventData.Split(' ');
            string name = splits[0];
            int parentSheetIndex =Convert.ToInt32(splits[1]);
            int amount = Convert.ToInt32(splits[2]);
            bool makeActiveObject = Convert.ToBoolean(splits[3]);
            StardewValley.Object obj = new StardewValley.Object(parentSheetIndex, amount);
            Game1.player.addItemToInventoryBool(obj,makeActiveObject);
            Game1.CurrentEvent.CurrentCommand++;
        }


        /// <summary>
        /// Lerp the camera to a specified position.
        /// </summary>
        /// <param name="EventManager"></param>
        /// <param name="EventData"></param>
        public static void ViewportLerp(EventManager EventManager,string EventData)
        {
            string[] splits = EventData.Split(' ');
            string name = splits[0];

            int xEndPosition =Convert.ToInt32(splits[1]);
            int yEndPosition = Convert.ToInt32(splits[2]);
            int frames = Convert.ToInt32(splits[3]);
            bool concurrent = Convert.ToBoolean(splits[4]);
            if (concurrent)
            {
                if (EventManager.concurrentEventActions.ContainsKey(name)==false)
                {
                    EventManager.addConcurrentEvent(new ConcurrentEventInformation(name,EventData,EventManager,ViewportLerp));
                    ++Game1.CurrentEvent.CurrentCommand; //I've been told ++<int> is more efficient than <int>++;
                }
            }

            if (StartedLerp==false)
            {
                OldViewportPosition = new Point(Game1.viewport.Location.X,Game1.viewport.Location.Y);
                StartedLerp = true;
            }

            ++CurrentViewportLerpAmount;
            if (CurrentViewportLerpAmount >= frames)
            {
                CurrentViewportLerpAmount = frames;
                Vector2 currentLerp2 = Vector2.Lerp(new Vector2(OldViewportPosition.X, OldViewportPosition.Y), new Vector2(OldViewportPosition.X+xEndPosition, OldViewportPosition.Y+yEndPosition), (float)((float)CurrentViewportLerpAmount/(float)frames));
                Game1.viewport.Location = new xTile.Dimensions.Location((int)currentLerp2.X, (int)currentLerp2.Y);

                OldViewportPosition = new Point(0, 0);
                CurrentViewportLerpAmount = 0;
                StartedLerp = false;
                if(concurrent==false)++Game1.CurrentEvent.CurrentCommand; //I've been told ++<int> is more efficient than <int>++;
                else
                {
                    EventManager.finishConcurrentEvent(name);
                }
                return;
            }
            Vector2 currentLerp = Vector2.Lerp(new Vector2(OldViewportPosition.X, OldViewportPosition.Y), new Vector2(OldViewportPosition.X + xEndPosition, OldViewportPosition.Y + yEndPosition), (float)((float)CurrentViewportLerpAmount/(float)frames));
            Game1.viewport.Location = new xTile.Dimensions.Location((int)currentLerp.X, (int)currentLerp.Y);
        }

        /// <summary>
        /// Adds in a junimo actor at the current location. Allows for multiple.
        /// </summary>
        /// <param name="EventManager"></param>
        /// <param name="EventData"></param>
        public static void AddInJumimoActorForEvent(EventManager EventManager, string EventData)
        {
            string[] splits = EventData.Split(' ');
            string name = splits[0];

            string actorName = splits[1];
            int xPos = Convert.ToInt32(splits[2]);
            int yPos = Convert.ToInt32(splits[3]);
            Color color = new Color(Convert.ToInt32(splits[4]), Convert.ToInt32(splits[5]), Convert.ToInt32(splits[6]));
            bool flipped = Convert.ToBoolean(splits[7]);

            List<NPC> actors = Game1.CurrentEvent.actors;
            Junimo junimo = new Junimo(new Vector2(xPos * 64, yPos * 64), -1, false);
            junimo.Name = actorName;
            junimo.EventActor = true;
            junimo.flip = flipped;

            IReflectedField<NetColor> colorF=StardustCore.ModCore.ModHelper.Reflection.GetField<NetColor>(junimo, "color", true);
            NetColor c = colorF.GetValue();
            c.R = color.R;
            c.G = color.G;
            c.B = color.B;
            colorF.SetValue(c);

            actors.Add((NPC)junimo);
            ++Game1.CurrentEvent.CurrentCommand; //I've been told ++<int> is more efficient than <int>++;
        }

        /// <summary>
        /// Flip a given junimo actor. Necessary to make junimos face left.
        /// </summary>
        /// <param name="EventManager"></param>
        /// <param name="EventData"></param>
        public static void FlipJunimoActor(EventManager EventManager, string EventData)
        {
            string[] splits = EventData.Split(' ');
            string name = splits[0];
            string actorName = splits[1];
            bool flipped = Convert.ToBoolean(splits[2]);
            NPC junimo=Game1.CurrentEvent.actors.Find(i => i.Name.Equals(actorName));
            junimo.flip = flipped;
            ++Game1.CurrentEvent.CurrentCommand; //I've been told ++<int> is more efficient than <int>++;
        }

        /// <summary>
        /// Adds the concurrent event to handle junimo movement.
        /// </summary>
        /// <param name="EventManager"></param>
        /// <param name="EventData"></param>
        public static void SetUpAdvanceJunimoMovement(EventManager EventManager, string EventData)
        {
            string[] splits = EventData.Split(' ');
            string name = splits[0];
            ++Game1.CurrentEvent.CurrentCommand; //I've been told ++<int> is more efficient than <int>++;
            EventManager.addConcurrentEvent(new ConcurrentEventInformation("AdvanceJunimoMove", "", EventManager, AdvanceJunimoMovement));
        }

        /// <summary>
        /// Finishes handling advvance junimo movement.
        /// </summary>
        /// <param name="EventManager"></param>
        /// <param name="EventData"></param>
        public static void FinishAdvanceJunimoMovement(EventManager EventManager, string EventData)
        {
            string[] splits = EventData.Split(' ');
            string name = splits[0];
            ++Game1.CurrentEvent.CurrentCommand; //I've been told ++<int> is more efficient than <int>++;
            EventManager.finishConcurrentEvent("AdvanceJunimoMove");
        }

        public static void AddInJunimoAdvanceMove(EventManager EventManager, string EventData)
        {

            if (EventManager.concurrentEventActions.ContainsKey("Omegasis.EventFramework.SetUpAdvanceJunimoMovement")==false)
            {
                EventManager.addConcurrentEvent(new ConcurrentEventInformation("AdvanceJunimoMove", "", EventManager, AdvanceJunimoMovement));
            }
            string[] splits = EventData.Split(' ');
            string name = splits[0];

            string actorName = splits[1];
            int MaxFrames = Convert.ToInt32(splits[2]);
            int Speed = Convert.ToInt32(splits[3]);
            bool Loop = Convert.ToBoolean(splits[4]);

            List<Point> points = new List<Point>();
            for(int i = 5; i < splits.Length; i++)
            {
                string pointData = splits[i];
                string[] point = pointData.Split('_');
                int x = Convert.ToInt32(point[0]);
                int y = Convert.ToInt32(point[1]);
                points.Add(new Point(x, y));
            }

            junimoLerpData.Add(actorName, new JunimoAdvanceMoveData(actorName,points,MaxFrames,Speed,Loop));

            ++Game1.CurrentEvent.CurrentCommand; //I've been told ++<int> is more efficient than <int>++;
        }

        /// <summary>
        /// Updates all of the junimo movement logic.
        /// </summary>
        /// <param name="EventManager"></param>
        /// <param name="EventData"></param>
        public static void AdvanceJunimoMovement(EventManager EventManager, string EventData)
        {
            foreach(KeyValuePair<string,JunimoAdvanceMoveData> pair in junimoLerpData)
            {
                pair.Value.update();
            }
        }

        /// <summary>
        /// Removes, aka stops the junimo actor from doing their advance movement.
        /// </summary>
        /// <param name="EventManager"></param>
        /// <param name="EventData"></param>
        public static void RemoveAdvanceJunimoMovement(EventManager EventManager, string EventData)
        {
            string[] splits = EventData.Split(' ');
            string name = splits[0];
            string actorName = splits[1];
            if (junimoLerpData.ContainsKey(actorName))
            {
                junimoLerpData.Remove(actorName);
            }

            ++Game1.CurrentEvent.CurrentCommand; //I've been told ++<int> is more efficient than <int>++;
        }
    }
}
