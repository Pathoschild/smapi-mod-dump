using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewValley.TerrainFeatures;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using Newtonsoft.Json.Linq;

namespace MapUtilities.Trees
{
    public class Species
    {
        public const int FORM_OPEN = 0;
        public const int FORM_CONICAL = 1;
        public const int FORM_SPREADING = 2;
        public const int FORM_LINEAR = 3;

        public Texture2D treeSheet;

        public Dictionary<int, Rectangle> weightedStumpRects;
        public Dictionary<Rectangle, Dictionary<int, Color>> weightedStumpColors;
        public Dictionary<int, Rectangle> weightedTrunkRects;
        public Dictionary<Rectangle, Dictionary<int, Color>> weightedTrunkColors;
        public Dictionary<int, Rectangle> weightedLimbRects;
        public Dictionary<Rectangle, Dictionary<int, Color>> weightedLimbColors;
        public Dictionary<int, Rectangle> weightedBranchRects;
        public Dictionary<Rectangle, Dictionary<int, Color>> weightedBranchColors;
        public Dictionary<int, Rectangle> weightedLeafRects;
        public Dictionary<Rectangle, Dictionary<int, Color>> weightedLeafColors;

        //Some key words here:
        //Trunk sections, as a unit, refer to the sprite dimensions given to a trunk section for this tree type.  These can and will differ from tree to tree.
        //Stump refers to the base of the tree
        //Trunk refers to the main column of the tree
        //Limb refers to the large scaffolds from which branches grow
        //Branch refers to the scaffolds that hold leaves away from the trunk to gather sunlight optimally.

        //Minimum height for a mature tree of this type, measured in trunk sections.
        public int minHeight = 5;
        //Maximum variation in mature height for the tree, measured in trunk sections.
        public int heightVariation = 10;

        public float maxTrunkWobble = 1f;
        //The base chance any given trunk will have a limb
        public float limbFrequency = 0.3f;
        //The initial angle a limb will grow at, relative to the trunk section it is on
        public float limbAngle = 0.4f;
        //Minimum trunk sections before a limb will grow.  
        public int minLimbHeight = 2;
        //Variation on the minimum limb height, varied only positively.
        public float minLimbHeightVariation = 1.2f;
        //Average trunk sections between limbs, vertically.
        public float averageLimbInterval = 0.5f;
        //Variation on the trunk sections between limbs, varied both positively and negatively.
        public float limbIntervalVariation = 1f;
        //When limbs alternate, they will only protrude from one side of the trunk OR the other.  Otherwise, they will do both.
        public bool limbAlternate = true;

        public float limbGrouping = 2f;

        public float limbGroupVariation = 1f;
        //Average number of sections a limb should extend compared to how much tree is above it
        public float limbGrowth = 1f;

        public float limbGrowthVariation = 1f;

        public float limbStregnth = 0.5f;
        //How randomly the branches extend from limbs.  0 results in branches entirely horizontally extending from the limb, 1 results in absolutely any direction
        public float branchWhorl = 0.2f;
        //Branch length multiplier.  How long a branch should be as compared to the remaining length of the limb past it.
        public float averageBranchLength = 0.3f;
        //How much the length of each branch should vary by.
        public float branchLengthVariation = 1.2f;
        //How many limb sections will have branches
        public float branchFrequency = 0.7f;
        //Minimum limb sections before branches will grow
        public int minBranchDistance = 3;

        public float minBranchDistanceVariation = 0.4f;


        public int form = FORM_OPEN;


        public Species(int which)
        {
            loadFromJSON(which);
        }

        public void loadFromJSON(int treeType)
        {
            JObject treeJSON;
            try
            {
                treeJSON = Loader.load<JObject>("Trees/Tree_" + treeType + ".json", "Content/Trees/Tree_" + treeType + ".json") as JObject;
            }
            catch (Microsoft.Xna.Framework.Content.ContentLoadException e)
            {
                Logger.log("No tree json file found at Trees/Tree_" + treeType + "!", LogLevel.Error);
                if (treeType != 1)
                {
                    Logger.log("Trying to default to Tree_1...");
                    loadFromJSON(1);
                    return;
                }
                else
                    throw e;
            }

            Logger.log("Successfully located the tree json file for Tree_" + treeType);

            if (!treeJSON.ContainsKey("Format") || !treeJSON.ContainsKey("Structure") || !treeJSON.ContainsKey("Graphics"))
            {
                throw new KeyNotFoundException("Tree json is missing a vital field!  Missing:" + (treeJSON.ContainsKey("Format") ? " Format" : "") + (treeJSON.ContainsKey("Structure") ? " Structure" : "") + (treeJSON.ContainsKey("Graphics") ? " Graphics" : ""));
            }

            float format = Convert.ToSingle(treeJSON["Format"].ToString());
            if (format != 1.0f)
            {
                throw new Exception("Format " + format + " not expected!  No such format currently exists.  Maybe you need to update Map Utilites?");
            }



            JObject graphics = treeJSON["Graphics"] as JObject;

            string imageName = graphics["Sheet"].ToString();

            try
            {
                treeSheet = Loader.load<Texture2D>("Trees/" + imageName, "Content/Trees/" + imageName + ".png") as Texture2D;
            }
            catch (Microsoft.Xna.Framework.Content.ContentLoadException e)
            {
                Logger.log("Could not find tree sprite sheet '" + imageName + "', as defined in Tree_" + treeType + ".json!", LogLevel.Error);
                throw e;
            }

            makeWeightedRectsFromJSON(graphics["Stump"] as JArray, ref weightedStumpRects, ref weightedStumpColors);
            makeWeightedRectsFromJSON(graphics["Trunk"] as JArray, ref weightedTrunkRects, ref weightedTrunkColors);
            makeWeightedRectsFromJSON(graphics["Limb"] as JArray, ref weightedLimbRects, ref weightedLimbColors);
            makeWeightedRectsFromJSON(graphics["Branch"] as JArray, ref weightedBranchRects, ref weightedBranchColors);
            makeWeightedRectsFromJSON(graphics["Leaf"] as JArray, ref weightedLeafRects, ref weightedLeafColors);

            JObject structure = treeJSON["Structure"] as JObject;
            if (structure.ContainsKey("Trunk"))
            {
                JObject trunk = (structure["Trunk"] as JObject);
                tryApplyInt(ref minHeight, "Min Height", trunk);
                tryApplyInt(ref heightVariation, "Height Variation", trunk);
                tryApplyFloat(ref maxTrunkWobble, "Max Angle", trunk);
                maxTrunkWobble = TreeRenderer.degreesToRadians(maxTrunkWobble);
                tryApplyBool(ref limbAlternate, "Alternating", trunk);

                if (trunk.ContainsKey("Form"))
                {
                    switch (trunk["Form"].ToString().ToLower().Substring(0, 2))
                    {
                        case "op":
                            form = FORM_OPEN;
                            break;
                        case "co":
                            form = FORM_CONICAL;
                            break;
                        case "sp":
                            form = FORM_SPREADING;
                            break;
                        case "li":
                            form = FORM_LINEAR;
                            break;
                    }
                }
            }
            if (structure.ContainsKey("Limb"))
            {
                JObject limb = (structure["Limb"] as JObject);
                tryApplyFloat(ref limbFrequency, "Frequency", limb);
                tryApplyFloat(ref limbGrouping, "Grouping", limb);
                tryApplyFloat(ref limbGroupVariation, "Grouping Variation", limb);
                tryApplyInt(ref minLimbHeight, "Distance", limb);
                tryApplyFloat(ref minLimbHeightVariation, "Distance Variation", limb);
                tryApplyFloat(ref averageLimbInterval, "Interval", limb);
                tryApplyFloat(ref limbIntervalVariation, "Interval Variation", limb);
                tryApplyFloat(ref limbAngle, "Angle", limb);
                limbAngle = TreeRenderer.degreesToRadians(limbAngle);
                tryApplyFloat(ref limbGrowth, "Length Factor", limb);
                tryApplyFloat(ref limbGrowthVariation, "Length Variation", limb);
            }
            if (structure.ContainsKey("Branch"))
            {
                JObject branch = (structure["Branch"] as JObject);
                tryApplyFloat(ref branchFrequency, "Frequency", branch);
                tryApplyInt(ref minBranchDistance, "Distance", branch);
                tryApplyFloat(ref minBranchDistanceVariation, "Distance Variation", branch);
                tryApplyFloat(ref branchWhorl, "Whorling", branch);
                tryApplyFloat(ref averageBranchLength, "Length Factor", branch);
                tryApplyFloat(ref branchLengthVariation, "Length Variation", branch);
            }
        }

        public Dictionary<int, Color> makeWeightedColorsFromJSON(JArray jsonColors)
        {
            Dictionary<int, Color> colors = new Dictionary<int, Color>();
            int currentWeight = 0;
            foreach (JObject jColor in jsonColors)
            {
                int r = 255;
                int g = 255;
                int b = 255;
                int weight = 100;
                tryApplyInt(ref r, "R", jColor);
                tryApplyInt(ref g, "G", jColor);
                tryApplyInt(ref b, "B", jColor);
                tryApplyInt(ref weight, "Weight", jColor);

                currentWeight += weight;

                colors[currentWeight] = new Color(r,g,b);
            }
            return colors;
        }

        public void makeWeightedRectsFromJSON(JArray jsonRects, ref Dictionary<int,Rectangle> outRects, ref Dictionary<Rectangle,Dictionary<int,Color>> outColors)
        {
            //Dictionary<int, Rectangle> rects = new Dictionary<int, Rectangle>();
            outRects = new Dictionary<int, Rectangle>();
            outColors = new Dictionary<Rectangle, Dictionary<int, Color>>();
            int currentWeight = 0;
            foreach (JObject jRect in jsonRects)
            {
                int x = 0;
                int y = 0;
                int width = 16;
                int height = 16;
                int weight = 100;
                tryApplyInt(ref x, "X", jRect);
                tryApplyInt(ref y, "Y", jRect);
                tryApplyInt(ref width, "Width", jRect);
                tryApplyInt(ref height, "Height", jRect);
                tryApplyInt(ref weight, "Weight", jRect);

                Rectangle thisRect = new Rectangle(x, y, width, height);

                currentWeight += weight;

                outRects[currentWeight] = thisRect;

                if (jRect.ContainsKey("Colors"))
                {
                    JArray colors = jRect["Colors"] as JArray;
                    Dictionary<int, Color> colorDict = makeWeightedColorsFromJSON(colors);
                    outColors[thisRect] = colorDict;
                }
                else
                {
                    Dictionary<int, Color> colorDict = new Dictionary<int, Color>();
                    colorDict[100] = Color.White;
                    outColors[thisRect] = colorDict;
                }
            }
        }

        public Rectangle getRect(Dictionary<int, Rectangle> rects, Random random)
        {
            List<int> sortedWeights = rects.Keys.ToList();
            sortedWeights.Sort();

            int randomWeight = random.Next(sortedWeights[sortedWeights.Count - 1]);
            foreach (int weight in sortedWeights)
            {
                if (weight >= randomWeight)
                    return rects[weight];
            }
            Logger.log("There was an issue with the random selecting of a source rect!", LogLevel.Warn);
            return rects[sortedWeights[0]];
        }

        public Color getColor(Dictionary<Rectangle, Dictionary<int, Color>> spriteColors, Rectangle sprite, Random random)
        {
            if (!spriteColors.Keys.Contains(sprite))
            {
                Logger.log("Mismatch between sprite and color dictionary!");
                return Color.White;
            }
            Dictionary<int, Color> colors = spriteColors[sprite];
            List<int> sortedWeights = colors.Keys.ToList();
            sortedWeights.Sort();

            int randomWeight = random.Next(sortedWeights[sortedWeights.Count - 1]);
            foreach (int weight in sortedWeights)
            {
                if (weight >= randomWeight)
                    return colors[weight];
            }
            Logger.log("There was an issue with the random selecting of a color!", LogLevel.Warn);
            return colors[sortedWeights[0]];
        }

        public void tryApplyFloat(ref float variable, string key, JObject jObject)
        {
            if (!jObject.ContainsKey(key))
                return;
            variable = Convert.ToSingle(jObject[key]);
        }

        public void tryApplyInt(ref int variable, string key, JObject jObject)
        {
            if (!jObject.ContainsKey(key))
                return;
            variable = Convert.ToInt32(jObject[key]);
        }

        public void tryApplyBool(ref bool variable, string key, JObject jObject)
        {
            if (!jObject.ContainsKey(key))
                return;
            variable = Convert.ToBoolean(jObject[key]);
        }


    }
}
