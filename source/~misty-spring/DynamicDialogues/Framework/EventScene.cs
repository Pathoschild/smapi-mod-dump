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

namespace DynamicDialogues
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

                Texture2D Tex = Game1.temporaryContent.Load<Texture2D>("mistyspring.dynamicdialogues\\Scenes\\" + split[1]);

                float ID = split.Length >= 3 ? float.Parse(split[2]) : 69420f;

                int Frames = split.Length >= 4 ? int.Parse(split[3]) : 1;
                float milliseconds = split.Length >= 5 ? float.Parse(split[4]) : 200f;


                /*
                 * these will be for a future update
                 * 
           
                //figuring out position starts here

                var w = Game1.graphics.GraphicsDevice.Viewport.Width;
                var h = Game1.graphics.GraphicsDevice.Viewport.Height;

                var middle = Game1.viewportCenter.ToVector2();
                int middleX = w / 2;
                int middleY = h / 2;

                var bounds = Game1.graphics.GraphicsDevice.Viewport.Bounds;

                Vector2 pos = split[5] switch
                {
                    "middle" => middle,
                    "middle-left" => new (0,middleY),
                    "middle-right" => new (w,middleY),
                    
                    "mid" => Game1.viewportCenter.ToVector2(),
                    "mid-left" => new (0,middleY),
                    "mid-right" => new (w,middleY),

                    "top" => new (middleX,0),
                    "top-left" => new (0,0),
                    "top-right" => new (w,0),

                    "bottom" => new(middleX,h),
                    "bottom-left" => new(0,h),
                    "bottom-right"=> new(w,h),

                    _ => new (0, 0)
                };

                if(pos.X is not 0)
                {
                    pos.X =- Tex.Width * 2;
                }
                if (pos.Y is not 0)
                {
                    pos.X =- Tex.Height * 4;
                }

                int loops = split.Length == 7 ? int.Parse(split[6]) : 99999;

                */

                int width = (int)(Tex.Width / Frames);
                int height = Tex.Height;

                Vector2 pos = new Vector2(Game1.graphics.GraphicsDevice.Viewport.Width / 2 - 264, Game1.graphics.GraphicsDevice.Viewport.Height / 3 - 264);

                if(height != 112)
                {
                    pos = new(0,0);
                }


                //old: location.TemporarySprites.Add(
                var Scene = new TemporaryAnimatedSprite
                {
                    texture = Tex,
                    sourceRect = new(0, 0, width, height), //sourcerect refers to the position in the file. since these are custom, it'll always be at the position (0,0)
                    animationLength = Frames,
                    sourceRectStartingPos = new Vector2(0, 0), //orig 144f, 215f
                    interval = milliseconds,
                    totalNumberOfLoops = 99999,
                    id = ID,
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
                    text += $" texture = {Tex.Name},\r\n sourceRect = ({Scene.sourceRect.X}, {Scene.sourceRect.Y}, {Scene.sourceRect.Width}, {Scene.sourceRect.Height}),\r\n animationLength = {Frames},\r\n sourceRectStartingPos = (0,0),\r\n interval = {milliseconds},\r\n totalNumberOfLoops = 99999,\r\n id = {ID},\r\n position = ({pos.X},{pos.Y}),\r\n local = true,\r\n scale = 4f,\r\n  destroyable = false,\r\n overrideLocationDestroy = true";
                }

                Scene.layerDepth = 99f; //ref: in carolineTea stars are 1f depth, elizabeth's fireworks are 99f

                //if it doesnt exist we create it
                if (instance.aboveMapSprites == null || instance.aboveMapSprites?.Count == 0)
                {
                    instance.aboveMapSprites = new();
                }

                instance.aboveMapSprites.Add(Scene);
                instance.currentCommand++;

            }
            catch (Exception ex)
            {
                ModEntry.Mon.Log("Error: " + ex, StardewModdingAPI.LogLevel.Error);
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
                var tempID = split.Length >= 2 ? float.Parse(split[1]) : 69420f;

                location.removeTemporarySpritesWithIDLocal(tempID);
                instance.currentCommand++;
            }
            catch (Exception ex)
            {
                ModEntry.Mon.Log("Error: " + ex, StardewModdingAPI.LogLevel.Error);
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