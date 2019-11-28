using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace MapUtilities.Parallax
{
    public static class ForegroundHandler
    {
        public static ParallaxForeground foreground;

        public static void init()
        {
            foreground = null;
            //foreground = new ParallaxForeground();
            //foreground.layers.Add(new ParallaxLayer((Texture2D)Loader.load<Texture2D>("Parallax/realtest_trees_back", "Content/Parallax/realtest_trees_back.png"), 1.5f, false, 0));
            //foreground.layers.Add(new ParallaxLayer((Texture2D)Loader.load<Texture2D>("Parallax/realtest_trees_front", "Content/Parallax/realtest_trees_front.png"), 2f, false, 0));
        }

        public static void updateForeground()
        {
            GameLocation location = Game1.currentLocation;
            if (location != null && location.map != null && location.map.Properties.ContainsKey("Foreground"))
            {
                Logger.log("Found Foreground property, " + location.map.Properties["Foreground"].ToString());
                bool useHorizon = false;
                int horizon = 0;

                Dictionary<string, object> foregroundSource = (Dictionary<string, object>)Loader.load<Dictionary<string, object>>("Content/Parallax/" + location.map.Properties["Foreground"].ToString() + ".json");

                List<ParallaxLayer> layers = buildForeground(foregroundSource, location.map.Properties["Foreground"].ToString());

                foreground = new ParallaxForeground(layers);
            }
            else
            {
                //Logger.log("Location did not have a Foreground property");
                //Game1.background = null;
            }
        }

        public static List<ParallaxLayer> buildForeground(Dictionary<string,object> foregroundSource, string foregroundName)
        {
            List<ParallaxLayer> outLayers = new List<ParallaxLayer>();
            if (!foregroundSource.ContainsKey("Format") || !foregroundSource.ContainsKey("Layers"))
            {
                Logger.log("Incomplete foreground definition!  Foreground file " + foregroundName + ".json was missing the " + (!foregroundSource.ContainsKey("Format") ? "Format" : "Layers") + " field.", StardewModdingAPI.LogLevel.Error);
                return outLayers;
            }
            float formatVersion;
            try
            {
                formatVersion = Convert.ToSingle(foregroundSource["Format"]);
            }
            catch (FormatException)
            {
                Logger.log("Format version not given an acceptable value!  Format should be written as a version number, but was instead " + foregroundSource["Format"], StardewModdingAPI.LogLevel.Error);
                return outLayers;
            }
            Newtonsoft.Json.Linq.JArray layerDefs;
            try
            {
                layerDefs = (Newtonsoft.Json.Linq.JArray)foregroundSource["Layers"];
                //Logger.log(layerDefs[0].GetType().ToString());
            }
            catch (InvalidCastException)
            {
                Logger.log("There was a problem interpreting the Layers field of " + foregroundName + ".json!  Please check for bracket mismatch, or try running it through a json validator.", StardewModdingAPI.LogLevel.Error);
                return outLayers;
            }
            foreach (Newtonsoft.Json.Linq.JObject layerDef in layerDefs)
            {
                if (!layerDef.ContainsKey("Image") || !layerDef.ContainsKey("Depth"))
                {
                    Logger.log("Layer in " + foregroundName + " is missing a vital field!  Could not find " + (!layerDef.ContainsKey("Image") ? "Image" : "Depth") + ".", StardewModdingAPI.LogLevel.Error);
                    return outLayers;
                }
                int fillStyle = ParallaxLayer.None;
                if (layerDef.ContainsKey("Fill Style"))
                {
                    switch (layerDef["Fill Style"].ToString().ToLower())
                    {
                        case "tile":
                            fillStyle = ParallaxLayer.Tile;
                            break;
                        case "stretch":
                            fillStyle = ParallaxLayer.Stretch;
                            break;
                        case "fill":
                            fillStyle = ParallaxLayer.Fill;
                            break;
                        case "fit":
                            fillStyle = ParallaxLayer.Fit;
                            break;
                    }
                }

                string imageSource = Util.StringUtilities.getSeasonalName(layerDef["Image"].ToString().Split('.')[0]);
                float depth = Convert.ToSingle(layerDef["Depth"]);
                int scale = 2;
                if (layerDef.ContainsKey("Scale"))
                {
                    scale = Convert.ToInt32(layerDef["Scale"]);
                }

                ParallaxLayer layer = new ParallaxLayer((Texture2D)Loader.load<Texture2D>("Parallax/" + imageSource, "Content/Parallax/" + imageSource + ".png"), depth, false, 0);

                layer.zoomScale = scale;
                layer.fillMode = fillStyle;
                outLayers.Add(layer);
            }
            return outLayers;
        }

        public static void update()
        {
            if (foreground != null)
            {
                foreground.update(Game1.viewport);
            }
        }

        public static void draw(SpriteBatch b)
        {
            if(foreground != null)
            {
                foreground.draw(b);
            }
        }
    }
}
