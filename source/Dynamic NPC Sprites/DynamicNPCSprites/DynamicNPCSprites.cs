using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System.Linq;
using Microsoft.Xna.Framework;
using System.IO;
using StardewValley.Characters;

namespace DynamicNPCSprites
{
    /// <summary>The mod entry point.</summary>
    public class DynamicNPCSprites : Mod
    {

        private bool isFerngillLoaded = false;
        private bool isPortraitureLoaded = false;
        private List<ConditionalTexture> model = new List<ConditionalTexture>();
        private static string customContentFolder = "configs/";

        //Setup Statics
        //-------------------------
        //Sprite Defaults
        private static int defaultSpriteWidth = 64;
        private static int defaultSpriteHeight = 384;
        private static int defaultCatSpriteWidth = 128;
        private static int defaultCatSpriteHeight = 256;
        private static int defaultDogSpriteWidth = 128;
        private static int defaultDogSpriteHeight = 288;

        //Portrait Defaults
        private static int defaultPetPortraitWidth = 16;
        private static int defaultPetPortraitHeight = 16;
        private static int defaultPortraitFrameSize = 64;
		private static int defaultMaxPortraitFrameWidth = 128;


        public List<string> dynamicNPCjsonFiles = new List<string>();

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            string folder = Path.Combine(this.Helper.DirectoryPath, customContentFolder);
            foreach (string file in Directory.GetFiles(folder, "*.json", SearchOption.AllDirectories))
                dynamicNPCjsonFiles.Add(file);

            //Event Handler
            SaveEvents.AfterLoad += LoadConfigs;
            TimeEvents.AfterDayStarted += TimeEvents_AfterDayStarted;

            this.isFerngillLoaded = this.Helper.ModRegistry.IsLoaded("KoihimeNakamura.ClimatesOfFerngill");
            this.isPortraitureLoaded = this.Helper.ModRegistry.IsLoaded("Platonymous.Portraiture");
        }

        /*********
        ** Private methods
        *********/

        private void LoadConfigs(object sender, EventArgs e)
        {

            SaveEvents.AfterLoad -= LoadConfigs;
            foreach (string file in dynamicNPCjsonFiles)
            {
                this.Monitor.Log("Loading: " + file, LogLevel.Trace);
                this.model.AddRange(this.Helper.ReadJsonFile<List<ConditionalTexture>>(file));
            }
        }


        private void TimeEvents_AfterDayStarted(object sender, EventArgs e)
        {
            List<int> randomTexture = new List<int>();
            string weather = GetWeather();
            Monitor.Log("Weather: " + weather + ", Season: " + Game1.currentSeason, LogLevel.Trace);
            int defaultJsonTexture = -1;

            foreach (NPC npc in Utility.getAllCharacters())
            {
                foreach (ConditionalTexture jsonTexture in model)
                {
                    if (npc.name.Equals(jsonTexture.Name, StringComparison.InvariantCultureIgnoreCase) || npc.GetType().Name.Equals(jsonTexture.Name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (jsonTexture.Season.Contains(Game1.currentSeason, StringComparer.InvariantCultureIgnoreCase))
                        {
                            if (jsonTexture.Weather != null)
                            {
                                if (jsonTexture.Weather.Contains(weather, StringComparer.InvariantCultureIgnoreCase))
                                {
                                    if (jsonTexture.Portrait != null)
                                    {
                                        if (randomTexture.Count == 0)
                                        {
                                            randomTexture.Add(model.IndexOf(jsonTexture));
                                            defaultJsonTexture = -1;
                                        }
                                        else
                                        {
                                            randomTexture.Add(model.IndexOf(jsonTexture));
                                        }
                                    }

                                    if (jsonTexture.Sprite != null)
                                    {
                                        if (randomTexture.Count == 0)
                                        {
                                            randomTexture.Add(model.IndexOf(jsonTexture));
                                            defaultJsonTexture = -1;
                                        }
                                        else
                                        {
                                            if (randomTexture.Last() != model.IndexOf(jsonTexture))
                                            {
                                                randomTexture.Add(model.IndexOf(jsonTexture));
                                            }
                                        }
                                    }
                                }
                                else if (jsonTexture.Weather.Contains("Default", StringComparer.InvariantCultureIgnoreCase) && randomTexture.Count == 0)
                                {
                                    defaultJsonTexture = model.IndexOf(jsonTexture);
                                }
                            }
                            else if (randomTexture.Count == 0)
                            {
                                defaultJsonTexture = model.IndexOf(jsonTexture);
                            }
                        }
                    }
                }



                if (randomTexture.Count != 0 || defaultJsonTexture >= 0)
                {

                    int choice;

                    if (defaultJsonTexture >= 0)
                    {
                        choice = defaultJsonTexture;
                        defaultJsonTexture = -1;
                    }
                    else
                    {
                        Random randomSeed = new Random((int)Game1.stats.DaysPlayed);
                        choice = randomTexture[randomSeed.Next(randomTexture.Count)];
                    }

                    if (model[choice].Portrait != null)
                    {
                        Texture2D source = Helper.Content.Load<Texture2D>(model[choice].Portrait, ContentSource.ModFolder);

                        if (npc is Pet)
                        {
                            Monitor.Log("NPC is a " + npc.GetType().Name, LogLevel.Trace);

                            if (source.Bounds.Height == defaultPetPortraitHeight && source.Bounds.Width == defaultPetPortraitWidth)
                                ReplacePetMenu(source);
                            else
                                Monitor.Log("The texture " + model[choice].Portrait + " does not match the default " + npc.GetType().Name + " portrait size of " + defaultPetPortraitWidth + "x" + defaultPetPortraitHeight + " and wasn't loaded.", LogLevel.Error);

                        }
                        else
                        {
                            if (isPortraitureLoaded || (source.Bounds.Height % defaultPortraitFrameSize == 0 && source.Bounds.Width % defaultPortraitFrameSize == 0 && source.Bounds.Width <= defaultMaxPortraitFrameWidth))
                            {
                                npc.Portrait = source;
                                Monitor.Log("Loaded " + npc.displayName + "'s Portrait: " + model[choice].Portrait, LogLevel.Trace);
                            }
                            else
                                Monitor.Log("The texture " + model[choice].Portrait + " does not match the default " + npc.GetType().Name + " portrait size and wasn't loaded. Install the Portrature mod for HighRes Support. (http://www.nexusmods.com/stardewvalley/mods/999/)", LogLevel.Error);
                        }
                    }

                    if (model[choice].Sprite != null)
                    {

                        Monitor.Log("Load " + npc.displayName + "'s Sprite " + model[choice].Sprite, LogLevel.Trace);
                        Texture2D source = Helper.Content.Load<Texture2D>(model[choice].Sprite, ContentSource.ModFolder);

                        int spriteWidth = defaultSpriteWidth;
                        int spriteHeight = defaultSpriteHeight;

                        if (npc.GetType().Name == "Cat")
                        {
                            spriteWidth = defaultCatSpriteWidth;
                            spriteHeight = defaultCatSpriteHeight;
                        }
                        if (npc.GetType().Name == "Dog")
                        {
                            spriteWidth = defaultDogSpriteWidth;
                            spriteHeight = defaultDogSpriteHeight;
                        }

                        if (source.Bounds.Height == spriteHeight && source.Bounds.Width == spriteWidth)
                            npc.Sprite.Texture = source;
                        else
                            Monitor.Log("The texture " + model[choice].Sprite + " does not match the default " + npc.GetType().Name + " sprite size of " + spriteWidth + "x" + spriteHeight + " and wasn't loaded.", LogLevel.Error);

                    }
                    randomTexture.Clear();
                }
            }
        }


        public void ReplacePetMenu(Texture2D source)
        {
            Texture2D target = Game1.mouseCursors;
            Rectangle sourceArea = new Rectangle(0, 0, 16, 16); // Required Dimensions for Pet Portraits
            Rectangle targetArea = new Rectangle(160 + (Game1.player.catPerson ? 0 : 16), 192, 16, 16);

            // get source data
            int pixelCount = sourceArea.Width * sourceArea.Height;
            Color[] sourceData = new Color[pixelCount];
            source.GetData(0, sourceArea, sourceData, 0, pixelCount);

            // patch target texture
            target.SetData(0, targetArea, sourceData, 0, pixelCount);
        }

        /// <summary>Returns the current weather.</summary>
        private string GetWeather()
        {
            string weather = "clear";
            if (Game1.isRaining)
                weather = Game1.isLightning ? "stormy" : "rainy";
            else if (Game1.isSnowing)
                weather = "snowy";
            else if (Game1.isDebrisWeather)
            {
                switch (Game1.currentSeason)
                {
                    case "spring":
                        weather = "pollen";
                        break;

                    case "fall":
                        weather = "leaves";
                        break;

                    case "winter":
                        if (isFerngillLoaded)
                            weather = "flurries";
                        break;
                }
            }

            return weather;
        }


    }
}
