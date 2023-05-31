/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using System;

namespace DynamicDialogues.Framework
{
	internal static class Finder
	{
        /// <summary>
        /// Adds a scene to the current event.
        /// </summary>
        /// <param name="instance">Event</param>
        /// <param name="location">The place to add it in.</param>
        /// <param name="time">Game time</param>
        /// <param name="split">Parameters to use.</param>
        /// <see cref="StardewValley.Event"/>
        /// <see cref="StardewValley.EventScript_GreenTea"/>
        public static void ObjectHunt(Event instance, GameLocation location, GameTime time, string[] split)
		{
            try
            {
                /* format:
                * playerFind <key>
                */
                var currentPos = Game1.player.position;
                var direction = Game1.player.facingDirection.Value;

                var data = Game1.content.LoadString("mistyspring.dynamicdialogues\\Commands\\playerFind:"+ split[1]);
                var dataSplit = data.Split(',');
                
                foreach(var obj in dataSplit)
                {
                    var directions = obj.Split(' ');

                    var itemId = directions[2]; //int.Parse deprecated
                    var x = int.Parse(directions[0]);
                    var y = int.Parse(directions[1]);

                    var item = new StardewValley.Object(itemId,1);
                    
                    //add directions[2] at x,y = directions[0],directions[1] in location
                    //it seems lightTexture holds the Texture2D.
                    instance.festivalProps.Add(new Prop(item.lightSource.lightTexture, item.ParentSheetIndex, 1, 1, 1, x, y));
                }

       			instance.festivalTimer = 52000;

                instance.playerControlSequenceID = instance.id; //placeholder, i don't remember what went here.
		        instance.playerControlSequence = true;
		        Game1.player.CanMove = true;
		        Game1.viewportFreeze = false;
		        Game1.forceSnapOnNextViewportUpdate = true;
		        Game1.globalFade = false;
		        instance.doingSecretSanta = false;
		        Game1.player.canOnlyWalk = false;

                var lastTime = 0;

                //Game1.pauseThenDoFunction();
                //experimental: -1 every millisecond 
                while(instance.festivalTimer > 0)
                {
				    if (!Game1.player.UsingTool)
				    {   
					    Game1.player.forceCanMove();
			    	}

				    if (lastTime == Game1.currentGameTime.ElapsedGameTime.Milliseconds) continue;
				    
				    instance.festivalTimer--;
				    lastTime = Game1.currentGameTime.ElapsedGameTime.Milliseconds;
                }

                Game1.player.Halt();
                instance.EndPlayerControlSequence();

                //wait that time then do:
                //fade in
                Game1.fadeScreenToBlack();
                
                //return to og position
                Game1.warpFarmer(
	                Game1.player.currentLocation.Name,
	                (int)currentPos.X,
	                (int)currentPos.Y,
	                direction);
                
                instance.CurrentCommand++;
            }
            catch (Exception ex)
            {
                ModEntry.Mon.Log("Error: " + ex, StardewModdingAPI.LogLevel.Error);
            }
        }
    }
}