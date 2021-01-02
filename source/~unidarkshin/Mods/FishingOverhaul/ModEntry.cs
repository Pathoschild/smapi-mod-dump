/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/unidarkshin/Stardew-Mods
**
*************************************************/

using System;
using System.Reflection;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using Microsoft.Xna.Framework;
using StardewValley.Objects;
using Netcode;
using StardewModdingAPI.Events;
using StardewValley;
using TehPers.FishingOverhaul.Api.Enums;
using System.IO;

namespace ExtremeFishingOverhaul
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod, IAssetEditor
    {

        public static Mod instance;
        public static Random rnd;

        String[] fType = { "mixed", "dart", "floater", "sinker" };
        String[] names = { "Jaguar", "Loveless", "Horny", "Chinese", "Japanese", "Korean", "Frightened", "American", "Sweedish", "Iranian", "Polish", "Bloodied", "Decaying", "Dead", "Dying", "Eager", "Kawaii", "Russian", "German", "Noodle", "Floppy", "Disk", "Bubble", "Fruit", "Stinky", "Taco", "Knife", "Hawk", "Night", "Sky", "Lizard", "Clown", "Floob", "Peruvian", "Forgotten", "Cranky", "Loopy", "Irresponsible", "Fish", "Dog", "Cat", "Mother", "Father", "Transvestite", "Llama", "Giraffe", "Expert", "Happy", "Sad", "Real", "Fake", "Cheating", "Freaky", "Legendary", "Common", "Rare", "Ultimate", "Original", "Freaky", "Slimy", "Rude", "Discriminatory", "Anorexic", "Enslaved", "Ghost", "Bird", "Squish", "Helium", "Metal", "Wood", "Stone", "Borking", "Skinny", "Janky", "Swaggy", "Oblivious", "Boring", "Raping", "Racist", "Fat", "Astral", "Elysian", "Celestial", "Angelic", "Immortal", "Obese", "Janky", "Strange", "Eerie", "Fishy", "Star", "Stringy", "Politically Correct", "Cougar", "BBW", "Creamy", "Moist", "Rookie", "Undead", "Listless", "Horny", "Bland", "Seasoned", "Careful", "Belligerent", "Uncanny", "Light", "Flakey", "Flaming", "Copious", "Arrogant", "Scaley", "Hallowed", "Sacred", "Small", "Excited", "Smothered", "Lava", "Molten", "Drake", "Sandy", "Ice", "Chocolate", "Slippery", "Shadey", "Elemental", "Expermental", "Air", "Fire", "Nature", "Earth", "Tree", "Mutant", "Defecating", "Physics Defying", "Passionate", "Cute", "Sexual", "Lonely", "Anxious", "Terrible", "Impressive", "Mentally", "Dangerous", "Intelligent", "Lucky", "Dramatic", "Embarrassed", "Conscious", "Wonderful", "Wonder", "Unique", "Freezing", "Beautiful", "Enchanted", "Ghostly" };
        String[] n_post = { "Fish", "Ray", "Angelfish", "Moonfish", "Eel", "Sucker", "Dragonfish", "Minnou", "Angler", "Prowfish" };


        List<float> diffs = new List<float>();
        List<int> fIDS = new List<int>();
        List<string> type = new List<string>();
        List<int> levRS = new List<int>();
        List<string> fNames = new List<string>();
        List<string> fTypes = new List<string>();
        List<bool> rares = new List<bool>();
        List<bool> rare2s = new List<bool>();
        List<string> specialIDs = new List<string>();
        List<bool> legend;
        List<bool> legend2;

        int MaxFish;
        int maxFL;
        int minFL;

        /// <summary>Used to register fish with Teh's Fishing Overhaul (if it's loaded)</summary>
        private readonly TehFishingHelper tehHelper;

        public ModEntry()
        {
            tehHelper = new TehFishingHelper(this);
        }

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        /// 

        public bool CanEdit<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals(@"Data\Fish") || asset.AssetNameEquals(@"Data\ObjectInformation") || asset.AssetNameEquals(@"Data\Locations") || asset.AssetNameEquals(@"Maps\springobjects");
        }

        /// <summary>Edit a matched asset.</summary>
        /// <param name="asset">A helper which encapsulates metadata about an asset and enables changes to it.</param>
        public void Edit<T>(IAssetData asset)
        {
            // Make sure the fish have been generated already
            if (!fIDS.Any())
                return;

            

            if (asset.AssetNameEquals(@"Data\Fish"))
            {
                IDictionary<int, string> data = asset.AsDictionary<int, string>().Data;


                for (int i = 0; i < MaxFish; i++)
                {
                    double spawn;

                    if (legend[i] == true)
                    {
                        spawn = 0.1;
                    }
                    else if (legend2[i] == true)
                    {
                        spawn = 0.05;
                    }
                    else
                    {
                        spawn = ModEntry.rnd.Next(3, 5 - Math.Max(0, (int)Math.Round(levRS[i] / 50D))) / 10D;
                    }



                    int maxSize = rnd.Next(10, 100);
                    
                    data[fIDS[i]] = $"Fish {fIDS[i]}/{diffs[i]}/{type[i]}/1/{maxSize.ToString()}/600 2600/spring/both/1 .1 1 .1/{rnd.Next(3, 6).ToString()}/{spawn.ToString()}/0.5/{levRS[i]}";

                    // Teh's Fishing Overhaul compatibilty
                    int fishId = this.fIDS[i];
                    this.tehHelper.AddedData[fishId].Weight = (float)spawn;
                    this.tehHelper.AddedTraits[fishId].MaxSize = maxSize;
                }



            }
            else if (asset.AssetNameEquals(@"Data\ObjectInformation"))
            {

                IDictionary<int, string> data = asset.AsDictionary<int, string>().Data;

                for (int i = 0; i < this.MaxFish; i++)
                {
                    int price = 0 + (int)(((diffs[i] * diffs[i]) / 40.0) * (((levRS[i] + 8) * (levRS[i] + 8)) / (100.0 * ((levRS[i] + 1) / 2.0))));

                    int food = rnd.Next((int)((levRS[i] - 10) / 3.0), (int)(((diffs[i] + 20) / 20.0) * ((levRS[i] + 10) / 5.0)));
                    //this.Monitor.Log($"-------->food {food}");

                    if (rnd.NextDouble() < 0.125)
                    {
                        food = -1 * food;
                    }

                    if (food < -100)
                    {
                        price = (int)(price * 1.5);

                        specialIDs[i] += "Poisonous (150%), ";
                    }
                    else if (food < -1000)
                    {
                        price = (int)(price * 2.0);

                        specialIDs[i] += "Deadly Poison (200%), ";
                    }

                    //int food = 0 + (int)((diff / 20.0) * rnd.Next(-1 * (int)(levR + 10 / 5.0), (int)(levR + 10 / 5.0)));

                    //this.Monitor.Log($"Levs: {levR}");

                    if (fTypes[i] == "mixed")
                    {
                        price = (int)(price * 1.5);
                    }
                    else if (fTypes[i] == "dart")
                    {
                        price = (int)(price * 1.25);
                    }
                    else if (fTypes[i] == "floater")
                    {
                        price = (int)(price * 0.8);
                    }
                    else
                    {
                        price = (int)(price * 0.6);
                    }

                    //"Astral", "Elysian", "Celestial", "Angelic", "Immortal",
                    if (rares[i])
                    {
                        price *= rnd.Next(2, 4);
                        food *= rnd.Next(2, 4);
                    }
                    else if (rare2s[i])
                    {
                        price *= rnd.Next(10, 20);
                        food *= rnd.Next(10, 20);
                    }

                    if (price < 20)
                    {
                        price = 20;
                    }

                    data[fIDS[i]] = $"{fNames[i]}/{price}/{food}/Fish -4/{fNames[i]}/Price: {price}\nDifficulty: {diffs[i]}\nLevel Requirement: {levRS[i]}\nFish Type: {fTypes[i]}\nSpecial: {specialIDs[i]}/ Day Night^Spring Fall";
                }





            }
            else if (asset.AssetNameEquals(@"Data\Locations"))
            {
                IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                //ICollection<string> keys = data.Keys;

                foreach (string id in data.Keys.ToArray())
                {

                    string[] fields = data[id].Split('/');


                    for (int i = 0; i < fIDS.Count; i++)
                    {

                        if (rnd.NextDouble() < 0.33)
                        {
                            fields[4] += " " + fIDS[i] + " -1";
                            //this.Monitor.Log($"|||||-----> {fIDS[i]}");

                            this.tehHelper.AddLocation(fIDS[i], id.ToString());
                            this.tehHelper.AddedData[fIDS[i]].Seasons.Add("spring");
                        }
                        if (rnd.NextDouble() < 0.33)
                        {
                            fields[5] += " " + fIDS[i] + " -1";
                            //this.Monitor.Log($"|||||-----> {fIDS[i]}");

                            this.tehHelper.AddLocation(fIDS[i], id.ToString());
                            this.tehHelper.AddedData[fIDS[i]].Seasons.Add("summer");
                        }
                        if (rnd.NextDouble() < 0.33)
                        {
                            fields[6] += " " + fIDS[i] + " -1";
                            //this.Monitor.Log($"|||||-----> {fIDS[i]}");

                            this.tehHelper.AddLocation(fIDS[i], id.ToString());
                            this.tehHelper.AddedData[fIDS[i]].Seasons.Add("fall");
                        }
                        if (rnd.NextDouble() < 0.33)
                        {
                            fields[7] += " " + fIDS[i] + " -1";
                            //this.Monitor.Log($"|||||-----> {fIDS[i]}");

                            this.tehHelper.AddLocation(fIDS[i], id.ToString());
                            this.tehHelper.AddedData[fIDS[i]].Seasons.Add("winter");
                        }


                    }

                    data[id] = string.Join("/", fields);

                }
                //});

            }
            else if (asset.AssetNameEquals(@"Maps\springobjects"))
            {
                var texture = this.Helper.Content.Load<Texture2D>(@"assets\springobjects.xnb", ContentSource.ModFolder);

                //asset.AsImage().ReplaceWith(texture);

                //asset
                    //.AsImage()
                    //.Data.
            }
        }

        private void GenerateFish()
        {
            //this.Monitor.Log("2nd");
            bool rare = false;
            bool rare2 = false;
            MaxFish = 500;



            for (int i = 0; i < MaxFish; i++)
            {
                string specialID = "";

                double y = rnd.NextDouble();
                string x = "";
                if (y < 0.5)
                {
                    x = names[rnd.Next(0, names.Length - 1)] + " " + n_post[rnd.Next(0, n_post.Length - 1)];
                }
                else if (y < 0.8)
                {
                    x = names[rnd.Next(0, names.Length - 1)] + " " + names[rnd.Next(0, names.Length - 1)] + " " + n_post[rnd.Next(0, n_post.Length - 1)];
                }
                else
                {
                    x = names[rnd.Next(0, names.Length - 1)] + " " + names[rnd.Next(0, names.Length - 1)] + " " + names[rnd.Next(0, names.Length - 1)] + " " + n_post[rnd.Next(0, n_post.Length - 1)];
                }

                if (x.ToLower().Contains("elysian") || x.ToLower().Contains("angelic") || x.ToLower().Contains("immortal"))
                {
                    rare = true;
                    //x = x.ToUpper();
                    legend[i] = true;

                    specialID += "Legendary (200 to 400%), ";
                    //Monitor.Log("Celestial F Created.");
                }
                else if (x.ToLower().Contains("celestial") || x.ToLower().Contains("astral"))
                {
                    rare2 = true;
                    //x = x.ToUpper();
                    legend2[i] = true;

                    specialID += "Super Legendary (1000 to 2000%), ";
                    //Monitor.Log("Celestial2 F Created.");
                }

                int id = 1999 - i;
                int levR = rnd.Next(minFL, maxFL);
                int diff;
                string ft;

                if (rare)
                {
                    diff = rnd.Next(85, 150);
                    ft = fType[rnd.Next(0, 1)];
                }
                else if (rare2)
                {
                    diff = rnd.Next(135, 250);
                    ft = fType[rnd.Next(0, 1)];
                }
                else
                {
                    diff = rnd.Next(20, 120);
                    ft = fType[rnd.Next(0, fType.Length - 1)];
                }

                diff = (int)(diff * (((levR + 100.0) / 250.0) + 0.30));

                fIDS.Add(id);
                diffs.Add(diff);
                type.Add(ft);
                levRS.Add(levR);
                fNames.Add(x);
                fTypes.Add(ft);
                rares.Add(rare);
                rare2s.Add(rare2);
                specialIDs.Add(specialID);

                rare = false;
                rare2 = false;

                // Teh's Fishing Overhaul compatibility
                FishMotionType motionType;
                switch (ft)
                {
                    case "mixed":
                        motionType = FishMotionType.MIXED;
                        break;
                    case "dart":
                        motionType = FishMotionType.DART;
                        break;
                    case "floater":
                        motionType = FishMotionType.FLOATER;
                        break;
                    case "sinker":
                        motionType = FishMotionType.SINKER;
                        break;
                    case "smooth":
                        motionType = FishMotionType.SMOOTH;
                        break;
                    default:
                        motionType = FishMotionType.MIXED;
                        break;
                }

                // Max size gets set when Fish.xnb is loaded
                TehFishingHelper.FishTraits traits = new TehFishingHelper.FishTraits(diff, motionType, 1, 1, x);
                TehFishingHelper.FishData data = new TehFishingHelper.FishData(levR);
                this.tehHelper.AddFish(id, traits, data);
            }
        }

        public override void Entry(IModHelper helper)
        {

            ModConfig config = helper.ReadConfig<ModConfig>();
            rnd = new Random(config.seed);
            Monitor.Log("Seed loaded.");

            int x = 10;
            if (this.Helper.ModRegistry.IsLoaded("Devin_Lematty.Level_Extender") && !config.maxLevOverride)
            {
                x = 100;

                config.maxFishingLevel = x;

                this.Helper.WriteConfig<ModConfig>(config);
            }
            else if (!File.Exists($"config.json"))
            {
                this.Helper.WriteConfig<ModConfig>(config);
            }


            maxFL = config.maxFishingLevel;
            minFL = config.minFishingLevel;
            MaxFish = config.maxFish;

            legend = new List<bool>(new bool[MaxFish]);
            legend2 = new List<bool>(new bool[MaxFish]);

            GenerateFish();

            // Experimental compatibility with Teh's Fishing Overhaul (until Nuget package is updated)
            helper.Events.GameLoop.SaveLoaded += (sender, e) => this.tehHelper.TryRegisterFish();
        }





    }
}
