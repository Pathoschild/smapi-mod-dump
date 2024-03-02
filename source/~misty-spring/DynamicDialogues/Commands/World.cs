/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using DynamicDialogues.Models;
using StardewModdingAPI;
using StardewValley;

namespace DynamicDialogues.Commands;

internal static class World
{
    private static void Log(string msg, LogLevel lv = LogLevel.Trace) => ModEntry.Mon.Log(msg, lv);

    internal const string HuntSequenceId = "mistyspring.dynamicdialogues.objectHunt:";

    /*
    * will require postfixing receiveActionPress, and if ID matches our custom one
    * if prop on x,y, remove; Game1.playSound("coin");
    * then, if props is empty, EndPlayerControlSequence(); CurrentCommand++;
    * optionally, have a flag that sets either success or failure
    * 
    * also postfix festivalUpdate. if props isn't empty, set flag? and current command++
    */
    /// <summary>
	/// Adds a scene to the current event.
	/// </summary>
	/// <param name="event">Event</param>
	/// <param name="args">Parameters to use.</param>
	/// <param name="context">Event context.</param>
	/// <see cref="StardewValley.Event"/>
	/// <see cref="StardewValley.Event.setUpPlayerControlSequence(string)"/>
	public static void ObjectHunt(Event @event, string[] args, EventContext context)
    {
        try
        {
            //check if we've already setup
            if(!string.IsNullOrWhiteSpace(@event.playerControlSequenceID))
            {
                return;
            }

            /* format:
			 * objectHunt <key>
			 */

            //if not enough args
            if(args.Length < 2)
            {
                context.LogErrorAndSkip("Must state an ID to load data from.");
                return;
            }

            var mainData = ModEntry.Help.GameContent.Load<Dictionary<string,HuntContext>>(@"mistyspring.dynamicdialogues\Commands\objectHunt");
            var data = mainData[args[1]];

            //add objects
            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (var obj in data.Objects)
            {
                //var directions = obj.Split(' ');

                var itemId = obj.ItemId;
                var x = obj.X;
                var y = obj.Y;

                var itemdata = ItemRegistry.GetDataOrErrorItem(itemId);
                //var item = new Object(itemId, 1);

                var prop = new Prop(
                    texture: itemdata.GetTexture(), 
                    index: itemdata.SpriteIndex, 
                    tilesWideSolid: 1, 
                    tilesHighSolid: 1, 
                    tilesHighDraw: 1, 
                    tileX: x, 
                    tileY: y
                    );

                @event.festivalProps.Add(prop);
                //@event.props.Add(item);
            }

            //if timer was given by player, set it. otherwise, unlimited
            var timer = data.Timer;
            if (timer != 0)
            {
                context.Location.playSound("");
                @event.festivalTimer = timer;
            }

            //set up
            var ownId = $"{HuntSequenceId}{args[1]}";
            @event.setUpPlayerControlSequence(ownId);
            Game1.player.canOnlyWalk = !data.CanPlayerRun;
        }
        catch (Exception ex)
        {
            Log("Error: " + ex, LogLevel.Error);
        }
    }
}
