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
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using System.Text;
using StardewModdingAPI;

// ReSharper disable PossibleLossOfFraction

namespace DynamicDialogues.Framework
{
	internal class EventScene
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
        public static void Add(Event instance, GameLocation location, GameTime time, string[] split)
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
	            if (ModEntry.Config.Debug)
	            {
		            string splitdata = null;
		            foreach (var text in split)
		            {
			            splitdata += text + "<>";
		            }
		            ModEntry.Mon.Log("split= " + splitdata,LogLevel.Info);
	            }				
                var tex = Game1.temporaryContent.Load<Texture2D>("mistyspring.dynamicdialogues\\Scenes\\" + split[1]);

                var id = split.Length >= 3 ? float.Parse(split[2]) : 69420f;

                var frames = split.Length >= 4 ? int.Parse(split[3]) : 1;
                var milliseconds = split.Length >= 5 ? float.Parse(split[4]) : 200f;

                //figuring out position
                var posText = GetFixedString(split[5].ToLower());
                var pos = GetPosition(posText, tex, frames);

                var loops = split.Length == 7 ? int.Parse(split[6]) : 99999;

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
                    id = id,
                    position = pos,
                    local = true,
                    scale = 4f,
                    destroyable = false,
                    overrideLocationDestroy = true
                }; //)

                if(ModEntry.Config.Debug)
                {
                    //log stuff
                    var text = "TemporaryAnimatedSprite: \n";
                    text += $" texture = {tex.Name},\r\n sourceRect = ({scene.sourceRect.X}, {scene.sourceRect.Y}, {scene.sourceRect.Width}, {scene.sourceRect.Height}),\r\n animationLength = {frames},\r\n sourceRectStartingPos = (0,0),\r\n interval = {milliseconds},\r\n totalNumberOfLoops = {loops},\r\n id = {id},\r\n position = ({pos.X},{pos.Y}),\r\n local = true,\r\n scale = 4f,\r\n  destroyable = false,\r\n overrideLocationDestroy = true";
                    ModEntry.Mon.Log(text);
                }

                scene.layerDepth = 99f; //ref: in carolineTea stars are 1f depth, elizabeth's fireworks are 99f

                //if it doesnt exist we create it
                if (instance.aboveMapSprites == null || instance.aboveMapSprites?.Count == 0)
                {
                    instance.aboveMapSprites = new TemporaryAnimatedSpriteList();
                }

                instance.aboveMapSprites?.Add(scene);
                instance.CurrentCommand++;

            }
            catch (Exception ex)
            {
                ModEntry.Mon.Log("Error: " + ex, LogLevel.Error);
            }
        }

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
			        "mid" => middleX - (tex.Width / frames) * 2,

			        "left" => 0,

			        "right" => h - tex.Width * 4,

			        _ => throw new ArgumentException("alignment is neither mid, left or right")
		        };

		        return position;
	        }
	        catch (Exception e)
	        {
		        ModEntry.Mon.Log("Error: " + e,LogLevel.Error);
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
		        var specialCharacters = new[] { "-", "_", "/", "\\", ";",":",",",".","*","+" };
		        foreach(var character in specialCharacters)
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
		        ModEntry.Mon.Log("Error: " + e,LogLevel.Error);
		        throw;
	        }
        }

        /// <summary>
        /// Remove a scene from an event.
        /// </summary>
        /// <param name="instance">Event.</param>
        /// <param name="location">Location to remove from.</param>
        /// <param name="time"></param>
        /// <param name="split">Parameters (</param>
		public static void Remove(Event instance, GameLocation location, GameTime time, string[] split)
        {
            /* Command:
             * RemoveScene <ID>
             */

			try
            {
                var tempId = split.Length >= 2 ? float.Parse(split[1]) : 69420f;

                location.removeTemporarySpritesWithIDLocal(tempId);
                instance.CurrentCommand++;
            }
            catch (Exception ex)
            {
                ModEntry.Mon.Log("Error: " + ex, LogLevel.Error);
            }

        }

        /* from addSpecificTemporarySprite (see <see cref="StardewValley.Event"/>):

Texture2D tempTex = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\temporary_sprites_1");
			location.TemporarySprites.Add(new TemporaryAnimatedSprite
			{
				texture = tempTex,
				sourceRect = new Microsoft.Xna.Framework.Rectangle(144, 215, 112, 112),
				animationLength = 2,
				sourceRectStartingPos = new Vector2(144f, 215f),
				interval = 200f,
				totalNumberOfLoops = 99999,
				id = 777f,
				position = new Vector2(Game1.graphics.GraphicsDevice.Viewport.Width / 2 - 264, Game1.graphics.GraphicsDevice.Viewport.Height / 3 - 264),
				local = true,
				scale = 4f,
				destroyable = false,
				overrideLocationDestroy = true
			});
			break;
}
*/
	}
}