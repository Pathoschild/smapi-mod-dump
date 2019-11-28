using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewModdingAPI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MapUtilities.Particles;

namespace MapUtilities.Parallax
{
    public static class BackgroundHandler
    {
        public static void updateBackground(GameLocation location = null)
        {
            if(location == null)
                location = Game1.currentLocation;
            if(location != null && location.map != null && location.map.Properties.ContainsKey("Background"))
            {
                Logger.log("Found Background property, " + location.map.Properties["Background"].ToString());

                Dictionary<string, object> backgroundSource = (Dictionary<string, object>)Loader.load<Dictionary<string, object>>("Content/Parallax/" + location.map.Properties["Background"].ToString() + ".json");

                if(backgroundSource == null || backgroundSource.Keys.Count < 1)
                {
                    Logger.log("Background source " + location.map.Properties["Background"].ToString() + " returned empty!");
                }
                else
                {
                    Logger.log("Background source returned with " + backgroundSource.Keys.Count + " objects.");
                }

                List<ParallaxLayer> layers = buildBackground(backgroundSource, location.map.Properties["Background"].ToString());

                Logger.log("Background built with " + layers.Count + " layers.");

                Game1.background = new ParallaxBackground(layers);
            }
            else
            {
                Logger.log("Location " + location.name + " did not have a Background property");
                Game1.background = null;
            }
        }

        public static List<ParallaxLayer> buildBackground(Dictionary<string, object> backgroundSource, string backgroundName)
        {
            Logger.log("Building background...");
            List<ParallaxLayer> outLayers = new List<ParallaxLayer>();
            bool useHorizon = false;
            int horizon = 0;
            if (Game1.currentLocation.map.Properties.ContainsKey("Horizon"))
            {
                try
                {
                    horizon = Convert.ToInt32(Game1.currentLocation.map.Properties["Horizon"].ToString());
                    useHorizon = true;
                }
                catch (FormatException)
                {
                    Logger.log("Horizon does not seem to be an integer value.  Please ensure that the Horizon property in " + Game1.currentLocation.name + " is a whole number with no decimal places.", LogLevel.Error);
                    horizon = 0;
                    useHorizon = false;
                }
            }
            Logger.log("Horizon set to " + horizon);
            if (!backgroundSource.ContainsKey("Format") || !backgroundSource.ContainsKey("Layers"))
            {
                Logger.log("Incomplete background definition!  Background file " + backgroundName + ".json was missing the " + (!backgroundSource.ContainsKey("Format") ? "Format" : "Layers") + " field.", StardewModdingAPI.LogLevel.Error);
                return outLayers;
            }
            Logger.log("Background had both Format and Layers...");
            float formatVersion;
            try
            {
                formatVersion = Convert.ToSingle(backgroundSource["Format"]);
            }
            catch (FormatException)
            {
                Logger.log("Format version not given an acceptable value!  Format should be written as a version number, but was instead " + backgroundSource["Format"], StardewModdingAPI.LogLevel.Error);
                return outLayers;
            }
            Logger.log("Background had valid format: " + formatVersion);
            Newtonsoft.Json.Linq.JArray layerDefs;
            try
            {
                layerDefs = (Newtonsoft.Json.Linq.JArray)backgroundSource["Layers"];
                //Logger.log(layerDefs[0].GetType().ToString());
            }
            catch (InvalidCastException)
            {
                Logger.log("There was a problem interpreting the Layers field of " + backgroundName + ".json!  Please check for bracket mismatch, or try running it through a json validator.", StardewModdingAPI.LogLevel.Error);
                return outLayers;
            }
            Logger.log("Found " + layerDefs.Count + " children within Layers");
            foreach (Newtonsoft.Json.Linq.JObject layerDef in layerDefs)
            {
                Logger.log("Creating layer...");
                if (!layerDef.ContainsKey("Image") || !layerDef.ContainsKey("Depth"))
                {
                    Logger.log("Layer in " + backgroundName + " is missing a vital field!  Could not find " + (!layerDef.ContainsKey("Image") ? "Image" : "Depth") + ".", StardewModdingAPI.LogLevel.Error);
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

                ParallaxLayer layer = new ParallaxLayer((Texture2D)Loader.load<Texture2D>("Parallax/" + imageSource, "Content/Parallax/" + imageSource + ".png"), depth, useHorizon, horizon);

                layer.zoomScale = scale;
                layer.fillMode = fillStyle;

                Logger.log("Created layer with image " + imageSource + ", depth " + depth + ", fill style " + fillStyle + ", " + (useHorizon ? "horizon " + horizon : "") + ", scale " + scale);

                if (layerDef.ContainsKey("Particles"))
                {
                    foreach(Newtonsoft.Json.Linq.JObject particleSystemDef in (Newtonsoft.Json.Linq.JArray)layerDef["Particles"])
                    {
                        Vector2 particlePosition = new Vector2(Convert.ToInt32(particleSystemDef["Position"]["X"]) * scale, Convert.ToInt32(particleSystemDef["Position"]["Y"]) * scale);
                        //ParticleSystem particleSystem = new ParticleSystem(Game1.mouseCursors, new Microsoft.Xna.Framework.Rectangle(32, 0, 10, 10), particlePosition, 8, 0.1f, 5f, 1, 24000, 0, 0, -0.05f, 0.55f,
                        //    new Dictionary<int, float>
                        //    {
                        //        {ParticleSystem.Out, 0 },
                        //        {ParticleSystem.North, 0 },
                        //        {ParticleSystem.East, 0 },
                        //        {ParticleSystem.Up, 0 },
                        //        {ParticleSystem.Right, 0 }
                        //    },
                        //    new Dictionary<int, float>
                        //    {
                        //        {ParticleSystem.Out, 0 },
                        //        {ParticleSystem.North, 0 },
                        //        {ParticleSystem.East, 0 },
                        //        {ParticleSystem.Up, 0 },
                        //        {ParticleSystem.Right, 0 }
                        //    }
                        //);
                        ParticleSystem particleSystem = new ParticleSystem(particleSystemDef["Particle"].ToString());
                        particleSystem.tileLocation = particlePosition;
                        layer.particleSystems.Add(particleSystem);
                    }
                }

                outLayers.Add(layer);
            }
            return outLayers;
        }

        
    }
}
