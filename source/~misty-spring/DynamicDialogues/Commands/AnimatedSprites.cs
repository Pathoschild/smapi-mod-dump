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
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace DynamicDialogues.Commands;

internal static class AnimatedSprites
{
    private static ModConfig Cfg => ModEntry.Config;
    private static void Log(string msg, LogLevel lv = LogLevel.Trace) => ModEntry.Mon.Log(msg, lv);
    
    /// <summary>
	/// Add a scene to an event.
	/// </summary>
	/// <param name="event">Event</param>
	/// <param name="args">Parameters to use.</param>
	/// <param name="context">Event context.</param>
	/// <see cref="EventScript_GreenTea"/>
	public static void AddScene(Event @event, string[] args, EventContext context)
    {
        /* if values exist, use. if not, use defaults
			 * legend: <required>, [optional]
			 * 
			 * AddScene <filename> <ID> [frames] [milliseconds per frame] [pos] [l]
			 *    0          1      2      3                4               5    6
			 *    
			 * width/height is img's (old: defaults to 112 x 112 (older:480 x 270))
			 * pos: middle,top,bottom [left,right]
			 * 
			 */

        try
        {
            if (Cfg.Debug)
            {
                string splitdata = null;
                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (var text in args)
                {
                    splitdata += text + "<>";
                }
                Log("split= " + splitdata, LogLevel.Info);
            }
            var tex = Game1.temporaryContent.Load<Texture2D>(@"mistyspring.dynamicdialogues\Scenes\" + args[1]);

            ArgUtility.TryGetOptionalInt(args,2,out var id, out _, 69420);
            ArgUtility.TryGetOptionalInt(args,3,out var frames, out _, 1);
            ArgUtility.TryGetOptionalFloat(args,4,out var milliseconds, out _, 200f);

            //figuring out position
            ArgUtility.TryGetOptional(args, 5, out var rawText, out _, "mid");
            var posText = GetFixedString(rawText);
            var pos = GetPosition(posText, tex, frames);

            ArgUtility.TryGetOptionalInt(args,6,out var loops, out _, 99999);

            var width = tex.Width / frames;
            var height = tex.Height;

            /*var pos = new Vector2(Game1.graphics.GraphicsDevice.Viewport.Width / 2 - 264, Game1.graphics.GraphicsDevice.Viewport.Height / 3 - 264);

			if(height != 112)
			{
				pos = new Vector2(0,0);
			}*/


            //old: location.TemporarySprites.Add(
            var scene = new TemporaryAnimatedSprite
            {
                texture = tex,
                sourceRect = new Rectangle(0, 0, width, height), //sourcerect refers to the position in the file. since these are custom, it'll always be at the position (0,0)
                animationLength = frames,
                sourceRectStartingPos = new Vector2(0, 0), //orig 144f, 215f
                interval = milliseconds,
                totalNumberOfLoops = loops,
                holdLastFrame = loops == 1, //if 1 loop, hold last frame
                id = id,
                position = pos,
                local = true,
                scale = 4f,
                destroyable = true,
                overrideLocationDestroy = true
            }; //)

            if (Cfg.Debug)
            {
                //log stuff
                var text = "TemporaryAnimatedSprite: \n";
                text += $" texture = {tex.Name},\r\n sourceRect = ({scene.sourceRect.X}, {scene.sourceRect.Y}, {scene.sourceRect.Width}, {scene.sourceRect.Height}),\r\n animationLength = {frames},\r\n sourceRectStartingPos = (0,0),\r\n interval = {milliseconds},\r\n totalNumberOfLoops = {loops},\r\n id = {id},\r\n position = ({pos.X},{pos.Y}),\r\n local = true,\r\n scale = 4f,\r\n  destroyable = false,\r\n overrideLocationDestroy = true";
                Log(text);
            }

            scene.layerDepth = 99f; //ref: in carolineTea stars are 1f depth, elizabeth's fireworks are 99f

            //if it doesnt exist we create it
            if (@event.aboveMapSprites == null || @event.aboveMapSprites?.Count == 0)
            {
                @event.aboveMapSprites = new TemporaryAnimatedSpriteList();
            }

            context.Location.TemporarySprites.Add(scene);
            @event.CurrentCommand++;

        }
        catch (Exception ex)
        {
            Log("Error: " + ex, LogLevel.Error);
        }
    }


    /// <summary>
    /// Get the position for a given texture.
    /// </summary>
    /// <param name="text">The alignment.</param>
    /// <param name="tex">The texture.</param>
    /// <param name="frames">Frames this texture has.</param>
    private static Vector2 GetPosition(string text, Texture2D tex, int frames)
    {
        try
        {
            var position = new Vector2();
            var textRaw = text.ToCharArray();
            string height;
            string alignment;

            //if true middle.
            //done this way because otherwise, string.Replace(height,null) would empty it
            if (text == "mid")
            {
                height = "mid";
                alignment = "mid";
            }
            else
            {
                height = $"{textRaw[0]}{textRaw[1]}{textRaw[2]}";
                alignment = text.Replace(height, null);
            }

            //var middle = Game1.viewportCenter.ToVector2();
            var w = Game1.viewport.Width;
            var h = Game1.viewport.Height;
            var middleX = w / 2;
            var middleY = h / 2;

            position.Y = height switch
            {
                "mid" => middleY - tex.Height * 2,

                "top" => 0,

                "bot" => h - tex.Height * 4,

                _ => throw new ArgumentException("height is neither mid, top, or bottom")
            };

            position.X = alignment switch
            {
                // ReSharper disable once PossibleLossOfFraction
                "mid" => middleX - tex.Width / frames * 2,

                "left" => 0,

                "right" => h - tex.Width * 4,

                _ => throw new ArgumentException("alignment is neither mid, left or right")
            };

            return position;
        }
        catch (Exception e)
        {
            Log("Error: " + e, LogLevel.Error);
            throw;
        }
    }

    /// <summary>
    /// Removes special characters from a string, and corrects its wording.
    /// </summary>
    /// <param name="text">The string to check.</param>
    /// <returns>An alignment string without (specific) special characters.</returns>
    private static string GetFixedString(string text)
    {
        try
        {
            var builder = new StringBuilder(text);

            //account for any char that could be put between zone(top,mid,bot) and alignment(left,right)
            var specialCharacters = new[] { "-", "_", "/", "\\", ";", ":", ",", ".", "*", "+" };
            foreach (var character in specialCharacters)
            {
                builder.Replace(character, null);
            }

            //fix possible variations of words
            builder.Replace("middle", "mid");
            builder.Replace("bottom", "bot");
            builder.Replace("up", "top");
            builder.Replace("down", "bot");

            switch (builder.ToString())
            {
                //if "midmid" (true middle), replace for "mid"
                case "midmid":
                    builder.Replace("midmid", "mid");
                    break;
                case "topmid":
                    builder.Replace("topmid", "top");
                    break;
                case "botmid":
                    builder.Replace("botmid", "bot");
                    break;
            }

            return builder.ToString();
        }
        catch (Exception e)
        {
            Log("Error: " + e, LogLevel.Error);
            throw;
        }
    }

    /// <summary>
    /// Remove a scene from an event.
    /// </summary>
    /// <param name="event">Event.</param>
    /// <param name="args">Parameters</param>
    /// <param name="context">Event context.</param>
    public static void RemoveScene(Event @event, string[] args, EventContext context)
    {
        try
        {
            ArgUtility.TryGetOptionalInt(args, 1, out var tempId, out _, 69420);

            //foreach (var TAS in @event.aboveMapSprites)
			//{
			//	if(TAS.id == tempId)
			//		@event.aboveMapSprites.Remove(TAS);
			//}
            context.Location.removeTemporarySpritesWithID(tempId);
            @event.CurrentCommand++;
        }
        catch (Exception ex)
        {
            Log("Error: " + ex, LogLevel.Error);
        }
    }

    /// <summary>
    /// Add fire TAS at the given coords.
    /// </summary>
    /// <param name="event"></param>
    /// <param name="args"></param>
    /// <param name="context"></param>
    internal static void AddFire(Event @event, string[] args, EventContext context)
    {
        /* format:
         * command <x> <y> [ID] [extraX] [extraY]
         */

        if(args.Length < 3)
        {
            context.LogErrorAndSkip("Must have at least 2 parameters: X, Y");
            return;
        }

        try
        {
            //get parameters
            var x = int.Parse(args[1]);
            var y = int.Parse(args[2]);
            var where = new Vector2(x, y);
            //if no ID is given, use gametime milliseconds
            ArgUtility.TryGetOptionalInt(args,3,out var id, out _, (int)Game1.currentGameTime.ElapsedGameTime.TotalMilliseconds);
            //extra x
            ArgUtility.TryGetOptionalInt(args,4,out var extraX, out _, 32);
            //extra y
            ArgUtility.TryGetOptionalInt(args,5,out var extraY, out _, 32);

            //create fire
            var fire = new TemporaryAnimatedSprite(
                "LooseSprites\\Cursors", //texture
                new Rectangle(276, 1985, 12, 11), //position in it
                where * 64f + new Vector2(extraX, extraY), //in map
                flipped: false,
                0f,
                Color.White)
            {
                id = id,
                interval = 50f,
                totalNumberOfLoops = 99999,
                animationLength = 4,
                light = false,
                lightRadius = 1f,
                scale = 4f,
                layerDepth = 0.5f
            };

            @event.aboveMapSprites?.Add(fire);
            @event.CurrentCommand++;
        }
        catch(Exception ex)
        {
            context.LogErrorAndSkip($"Error: {ex}");
        }
    }
}
