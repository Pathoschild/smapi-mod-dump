/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/DynamicDialogues
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
        /// <see cref="EventScript_GreenTea"/>
        internal static void Add(Event instance, GameLocation location, GameTime time, string[] split)
		{

            /* if values exist, use. if not, use defaults
			 * legend: <required>, [optional]
			 * 
			 * AddScene <filename> <ID> [frames] [milliseconds per frame] [w] [h]
			 *    0          1      2      3                4              5   6
			 *    
			 * width/height defaults to 480 x 270
			 * 
			 */

            try
            {
                Texture2D tempTex = Game1.temporaryContent.Load<Texture2D>("Events\\Scenes\\" + split[1]);

                float tempID = split.Length >= 3 ? float.Parse(split[2]) : 69420f;
                int Frames = split.Length >= 4 ? int.Parse(split[3]) : 1;
                float milliseconds = split.Length == 5 ? float.Parse(split[4]) : 200f;
                int width = split.Length == 6 ? int.Parse(split[5]) : 480;
                int height = split.Length == 7 ? int.Parse(split[6]) : 270;


                location.TemporarySprites.Add(new TemporaryAnimatedSprite
                {
                    texture = tempTex,
                    sourceRect = new Rectangle(0, 0, width, height), //orig (144, 215, 112, 112)
                    animationLength = Frames,
                    sourceRectStartingPos = new Vector2(0, 0), //orig 144f, 215f
                    interval = milliseconds,
                    totalNumberOfLoops = 99999,
                    id = tempID,
                    position = new Vector2(Game1.graphics.GraphicsDevice.Viewport.Width / 2 - 264, Game1.graphics.GraphicsDevice.Viewport.Height / 3 - 264),
                    local = true,
                    scale = 4f,
                    destroyable = false,
                    overrideLocationDestroy = true
                });
            }
            catch(Exception ex)
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
		internal static void Remove(Event instance, GameLocation location, GameTime time, string[] split)
        {
            /* Command:
             * RemoveScene <ID>
             */

			try
            {
                var tempID = split.Length >= 2 ? float.Parse(split[2]) : 69420f;

                location.removeTemporarySpritesWithIDLocal(tempID);
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