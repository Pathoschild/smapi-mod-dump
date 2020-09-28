using System;
using System.Collections.Generic;

//This is the class in which we define the area watered by our sprinkler.
//It's a bit excessive to make a whole class for this, maybe, but we may need to make the model more complex later.
namespace BasicSprinklerImproved
{
    class WateringPattern
    {
        readonly int maxWateringArea = 4;    //Sanity checking for custom patterns.

        /*
        * These define different watering patterns using a length-4 int array. 
        * 
        * By index, each signifies how many tiles in a direction to water.
        * 
        * 0: North
        * 1: South
        * 2: East
        * 3: West
        */

        readonly int[] horizontalPattern = new int[4] { 0, 0, 2, 2 };
        readonly int[] vericalPattern = new int[4] { 2, 2, 0, 0 };
        readonly int[] northPattern = new int[4] { 4, 0, 0, 0 };
        readonly int[] southPattern = new int[4] { 0, 4, 0, 0 };
        readonly int[] eastPattern = new int[4] { 0, 0, 4, 0 };
        readonly int[] westPattern = new int[4] { 0, 0, 0, 4 };
        int[] customPattern;

        //This structure is used for "efficient" sorting of type. I guess.
        List<int[]> normalPatterns;

        //What pattern we will actually use.
        public int[] myPattern;

        //The name of that pattern.
        public string myType;

        //A fallback to handle any error case: mod will create a sprinkler that acts like vanilla. (This is sorta overkill but it's robust...)
        readonly int[] defaultPattern = new int[4] { 1, 1, 1, 1 };

        //These are the names of different patterns for checking.
        readonly string[] patternTypes = new string[7] { "horizontal", "vertical", "north", "south", "east", "west", "custom" };

        //Can't log any errors here, so set a default error message and check for it at later state.
        public string errorMsg;

        public override string ToString()
        {
            return String.Format("'" + myType + "' type with dimensions: north {0}, south {1}, east {2}, west {3}.", myPattern[0], myPattern[1], myPattern[2], myPattern[3]);
        }

        //Resets any error status.
        private void ClearError()
        {
            errorMsg = "";
        }

        public int[] GetDefaultPattern()
        {
            return this.defaultPattern;
        }

        public string[] GetPatternTypes()
        {
            return this.patternTypes;
        }

        //Custom pattern logic
        public void Customize(int[] patternDef)
        {
            ClearError();

            //san check: correct array size?
            if (patternDef.Length != 4)
            {
                myPattern = defaultPattern;
                errorMsg = String.Format("Tried to make custom pattern, didn't have correct array size: got {0}, needed 4.", patternDef.Length);
            }
            else
            {
                int i = 0;      //incrementing index
                int area = 0;  //total area of pattern
                foreach (int x in patternDef)
                {
                    customPattern[i] = patternDef[i];
                    area += customPattern[i];
                    i++;
                }

                //san check: excessive area? technically this allows for watering areas less than 4.
                if (area > maxWateringArea)
                {
                    myPattern = defaultPattern;
                    errorMsg = String.Format("Tried to make custom pattern, area was excessive: got {0}, needed 4.", area);
                }
                //san check: no area defined? want to warn about that.
                else if (area == 0)
                {
                    myPattern = defaultPattern;
                    errorMsg = "Custom pattern has no area. Ensure that at least one direction's area is more than 0.";
                }
                else { myPattern = customPattern; }
            }
        }

        //Set to a specified basic/normal pattern
        public void SetNormalPattern(string typeDef)
        {
            ClearError();

            string customErr = "Undefined custom pattern attempted in SetNormalPattern(). Use Customize() for custom patterns.";

            //initial check to make sure this is not a custom pattern.
            if (typeDef == patternTypes[6])
            {
                myPattern = defaultPattern;
                errorMsg = customErr;
                return;
            }

            //to determine whether we actually find anything we're looking for.
            bool matchMade = false;

            int i = 0;
            foreach (string n in patternTypes)
            {
                if (!matchMade) { myPattern = normalPatterns[i]; }
                if (typeDef == patternTypes[i]) { matchMade = true; }
                i++;
            }

            if (!matchMade)
            {
                myPattern = defaultPattern;
                errorMsg = "Unrecognized pattern type attempted: '" + typeDef + "' - Acceptable type names are 'horizontal', 'vertical', 'north', 'south', 'east', 'west', and 'custom'.";
            }
            //theoretically, we could end up with a custom pattern here that's undefined.
            else if (myPattern == customPattern)
            {
                myPattern = defaultPattern;
                errorMsg = customErr;
            }
        }

        //initializations
        private void Init()
        {
            ClearError();
            customPattern = new int[4];
            normalPatterns = new List<int[]>
            {
                horizontalPattern, vericalPattern, northPattern, southPattern, eastPattern, westPattern, customPattern
            };
        }

        //A default no-args constructor, works like vanilla.
        public WateringPattern()
        {
            this.Init();
            myPattern = defaultPattern;
        }

        //A "real" constructor that determines its type.
        public WateringPattern(string typeDef, int[] patternDef)
        {
            //Just in case.
            myType = typeDef.ToLower();

            this.Init();

            //Define a custom pattern.
            if (myType == patternTypes[6])
            {
                Customize(patternDef);
            }
            //Define one of the preset patterns.
            else
            {
                SetNormalPattern(myType);
            }

        }

    }
}
