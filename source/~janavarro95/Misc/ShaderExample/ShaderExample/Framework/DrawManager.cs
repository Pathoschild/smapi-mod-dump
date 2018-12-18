using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShaderExample.Framework
{
    
    /// <summary>
    /// Handles drawing everything to the screen.
    /// </summary>
    public class DrawManager
    {
        SortedDictionary<float, List<object>> thingsToDraw = new SortedDictionary<float, List<object>>();
        Dictionary<Type, Delegates.DrawFunction> drawFunctions = new Dictionary<Type, Delegates.DrawFunction>();


        public DrawManager()
        {
            //Add support for characters

        }


        /// <summary>
        /// Add a new item to be drawn to the draw manager.
        /// </summary>
        /// <param name="yPos"></param>
        /// <param name="item"></param>
        public void addDraw(float yPos,object item)
        {
            if (thingsToDraw.ContainsKey(yPos))
            {
                List<object> objs;
                bool f = thingsToDraw.TryGetValue(yPos, out objs);
                objs.Add(item);
            }
            else
            {
                List<object> objs = new List<object>();
                objs.Add(item);
                thingsToDraw.Add(yPos, objs);
            }
        }

        /// <summary>
        /// Draw all supported ibject types that are handled by drawFunctions. Must be an appropriate key type for drawFunctions.
        /// </summary>
        public void draw()
        {
            //Begin effect.
            foreach(var pair in thingsToDraw)
            {
                foreach(var item in pair.Value)
                {
                    Delegates.DrawFunction func;
                    bool f = drawFunctions.TryGetValue(item.GetType(), out func);
                    if (f == false)
                    {
                        continue; //Unsuporte type found.
                    }
                    else
                    {
                        func.Invoke(item);
                    }
                }
            }
            //Once I am done drawing everything to the screen, flush my list of things to draw.
            thingsToDraw.Clear();
        }

        /*
         *Draw functions. 
         *
         */
        #region 


        #endregion

    }
}
