/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TheMightyAmondee/GoodbyeAmericanEnglish
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewModdingAPI.Events;
using System.Linq;

namespace GoodbyeAmericanEnglish
{ 
    public class ModEntry
        : Mod, IAssetEditor
    {

        // Array to hold NPC names
        private static string[] NPCs = 
        {
            "Abigail",
            "Alex",
            "Caroline",
            "Clint",
            "Demetrius",
            "Dwarf",
            "Elliot",
            "Emily",
            "George",
            "Gus",
            "Haley",
            "Harvey",
            "Jas",
            "Jodi",
            "Kent",
            "Krobus",
            "Leah",
            "Lewis",
            "Linus",
            "Marnie",
            "Maru",
            "Mister Qi",
            "Pam",
            "Penny",
            "Pierre",
            "Robin",
            "Sam",
            "Sandy",
            "Shane",
            "Sebastian",
            "Vincent",
            "Willy",
            "Wizard"
        };

        // Array to hold location names
        private static string[] locations =
        {
            "AbandonedJojaMart",
            "AnimalShop",
            "ArchaeologyHouse",
            "BackWoods",
            "BathHouse_Pool",
            "Beach",
            "BusStop",
            "CommunityCenter",
            "ElliottHouse",
            "Farm",
            "FarmHouse",
            "Forest",
            "HaleyHouse",
            "HarveyRoom",
            "Hospital",
            "JoshHouse",
            "LeahHouse",
            "ManorHouse",
            "Mine",
            "Mountain",
            "Railroad",
            "Saloon",
            "SamHouse",
            "SandyHouse",
            "ScienceHouse",
            "SebastianRoom",
            "SeedShop",
            "Sewer",
            "SunRoom",
            "Temp",
            "Tent",
            "Town",
            "Trailer",
            "Trailer_Big",
            "WizardHouse",
            "Woods"
        };

        public override void Entry(IModHelper helper)
        {

        }

        // Return true if an asset name matches
        public bool CanEdit<T>(IAssetInfo asset)
        {
            
            foreach(var name in NPCs)
            {
                // If asset name contains any iteration in NPCs array, return true
                if (false
                    || asset.AssetNameEquals($"Characters\\Dialogue\\Marriage{name}")
                    || asset.AssetNameEquals($"Characters\\Dialogue\\{name}")
                    || asset.AssetNameEquals($"Strings\\schedules\\{name}"))
                {
                    return true;
                }
            }

            foreach (var location in locations)
            {
                // If asset name contains any iteration in locations array, return true
                if (false
                    || asset.AssetNameEquals($"Data\\Events\\{location}"))
                {
                    return true;
                }
            }

            // return true if assest name is ANY of the following....
            return (false
                    || asset.AssetNameEquals("Strings\\StringsFromCSFiles")
                    || asset.AssetNameEquals("Strings\\UI")
                    || asset.AssetNameEquals("Strings\\Locations")
                    || asset.AssetNameEquals("Strings\\StringsFromMaps")
                    || asset.AssetNameEquals("Strings\\Notes")
                    || asset.AssetNameEquals("Strings\\Characters")
                    || asset.AssetNameEquals("Data\\ObjectInformation")
                    || asset.AssetNameEquals("Data\\TV\\TipChannel")
                    || asset.AssetNameEquals("Data\\TV\\CookingChannel")
                    || asset.AssetNameEquals("Data\\mail")
                    || asset.AssetNameEquals("Data\\ClothingInformation")
                    || asset.AssetNameEquals("Data\\ExtraDialogue")
                    || asset.AssetNameEquals("Data\\SecretNotes")
                    || asset.AssetNameEquals("Data\\NPCGiftTastes")
                    || asset.AssetNameEquals("Data\\Quests")
                    || asset.AssetNameEquals("Data\\Blueprints")
                    || asset.AssetNameEquals("Data\\Bundles")
                    || asset.AssetNameEquals("Data\\weapons")
                    || asset.AssetNameEquals("Data\\hats")
                    || asset.AssetNameEquals("Data\\Concessions")
                    || asset.AssetNameEquals("Data\\Movies")
                    || asset.AssetNameEquals("Data\\MoviesReactions")
                    || asset.AssetNameEquals("Data\\Festivals\\spring13")
                    || asset.AssetNameEquals("Data\\Festivals\\summer11")
                    || asset.AssetNameEquals("Data\\Festivals\\summer28")
                    || asset.AssetNameEquals("Data\\Festivals\\fall27")
                    || asset.AssetNameEquals("Data\\Festivals\\winter25")
                    || asset.AssetNameEquals("Minigames\\Intro")
                    || asset.AssetNameEquals("Data\\BigCraftablesInformation")
                    || asset.AssetNameEquals("Characters\\Dialogue\\MarriageDialogue"));
        }

        // Edit game assets
        public void Edit<T>(IAssetData asset)
        {
           
            // Method to hold common word replacements and conditions
            void SpellingFixer()
            {
                var data = asset.AsDictionary<string, string>().Data;

                foreach (string key in new List<string>(data.Keys))
                {
                    // Skip replacement if string is any of the following
                    if (false 
                        || data[key].Contains("fall on")
                        || data[key].Contains("fall out")
                        || data[key].Contains("fall prey")
                        || data[key].Contains("fallen")
                        || data[key].Contains("falling")
                        || data[key].Contains("fall_") 
                        || data[key].Contains("citizen") 
                        || data[key].Contains("size") 
                        || data[key].Contains("moment")
                        || data[key].Contains("cardamom")
                        || data[key].Contains("[color") 
                        || data[key].Contains("bgColor") 
                        || data[key].Contains("Prize") 
                        || data[key].Contains("prize")
                        || data[key].Contains("_apologize") 
                        || data[key].Contains("JoshMom")
                        || data[key].Contains("fallFest")
                        || data[key].Contains("WildColor")
                        || data[key].Contains("Favor")
                        || data[key].Contains("Fall Of Planet")
                        || data[key].Contains("communityCenter"))
                    {
                        continue;
                    }

                    // Replace specified string with new string
                    data[key] = data[key].Replace("the fall", "autumn");
                    data[key] = data[key].Replace("olor", "olour");
                    data[key] = data[key].Replace("behavior", "behaviour");
                    data[key] = data[key].Replace("fall", "autumn");
                    data[key] = data[key].Replace("Fall", "Autumn");
                    data[key] = data[key].Replace("ize", "ise");
                    data[key] = data[key].Replace("zation", "sation");
                    data[key] = data[key].Replace("Center", "Centre");
                    data[key] = data[key].Replace("twenty miles", "thirty kilometres");
                    data[key] = data[key].Replace("mom", "mum");
                    data[key] = data[key].Replace("Mom", "Mum");
                    data[key] = data[key].Replace("six inches", "fifteen centimetres");
                    data[key] = data[key].Replace("center", "centre");
                    data[key] = data[key].Replace("{0} in.", "{0} cm.");
                    data[key] = data[key].Replace("inches", "centimetres");
                    data[key] = data[key].Replace("theater", "theatre");
                    data[key] = data[key].Replace("Theater", "Theatre");
                    data[key] = data[key].Replace("counselor", "counsellor");
                    data[key] = data[key].Replace("honor", "honour");
                    data[key] = data[key].Replace("humor", "humour");
                    data[key] = data[key].Replace("avor", "avour");
                    data[key] = data[key].Replace("neighbor", "neighbour");
                    data[key] = data[key].Replace("traveling", "travelling");
                    data[key] = data[key].Replace("travele", "travelle");
                }
            }

            // Edit dialogue
            foreach (string name in NPCs)
            {
                // Edit character specific dialogue
                if (asset.AssetNameEquals($"Characters\\Dialogue\\{name}"))
                {
                    SpellingFixer();
                }

                // Edit character specific marriage dialogue
                else if (asset.AssetNameEquals($"Characters\\Dialogue\\MarriageDialogue{name}"))
                {
                    SpellingFixer();
                }

                // Edit schedule dialogue
                else if (asset.AssetNameEquals($"Strings\\schedules\\{name}"))
                {
                    SpellingFixer();
                }
            }
           
            // Edit events
            foreach (string location in locations)
            {
                if (asset.AssetNameEquals($"Data\\Events\\{location}"))
                {
                    SpellingFixer();
                }
            }

            // Edit strings
            if (asset.AssetNameEquals("Strings\\StringsFromCSFiles"))
            {
                SpellingFixer();
            }

            // Edit general marriage dialogue
            else if (asset.AssetNameEquals($"Characters\\Dialogue\\MarriageDialogue"))
            {
                SpellingFixer();
            }

            // Edit UI strings
            else if (asset.AssetNameEquals("Strings\\UI"))
            {
                SpellingFixer();
            }

            // Edit location strings
            else if (asset.AssetNameEquals("Strings\\Locations"))
            {
                SpellingFixer();
            }

            // Edit strings from maps
            else if (asset.AssetNameEquals("Strings\\StringsFromMaps"))
            {
                SpellingFixer();
            }

            // Edit library books
            else if (asset.AssetNameEquals("Strings\\Notes"))
            {
                SpellingFixer();
            }

            // Edit character strings
            else if (asset.AssetNameEquals("Strings\\Characters"))
            {
                SpellingFixer();
            }

            // Edit extra dialogue
            else if (asset.AssetNameEquals("Data\\ExtraDialogue"))
            {
                SpellingFixer();
            }

            // Edit secret notes
            else if (asset.AssetNameEquals("Data\\SecretNotes"))
            {
                IDictionary<int, string> data = asset.AsDictionary<int, string>().Data;

                
                foreach (int key in new List<int>(data.Keys))
                {
                    // Replace specified string with new string
                    data[key] = data[key].Replace("favorite", "favourite");
                    data[key] = data[key].Replace("Mom", "Mum");
                }
            }

            // Edit clothing information
            else if (asset.AssetNameEquals("Data\\ClothingInformation"))
            {
                IDictionary<int, string> data = asset.AsDictionary<int, string>().Data;

                foreach (int key in new List<int>(data.Keys))
                {
                    // Replace specified string with new string
                    data[key] = data[key].Replace("olor", "olour");
                }
            }

            // Edit Object information data
            else if (asset.AssetNameEquals("Data\\ObjectInformation"))
            {
                IDictionary<int, string> data = asset.AsDictionary<int, string>().Data;
                foreach (int key in new List<int>(data.Keys))
                {
                    // Skip replacement if string is any of the following
                    if (false 
                        || data[key].Contains("falling") 
                        || data[key].Contains("size") 
                        || data[key].Contains("rized") 
                        || data[key].Contains("denizen"))
                    {
                        continue;
                    }

                    // Replace string with new word
                    data[key] = data[key].Replace("the fall", "autumn");
                    data[key] = data[key].Replace("A fall", "An autumn");
                    data[key] = data[key].Replace("fall", "autumn");
                    data[key] = data[key].Replace("color", "colour");
                    data[key] = data[key].Replace("favorite", "favourite");
                    data[key] = data[key].Replace("ize", "ise");
                    data[key] = data[key].Replace("theater", "theatre");
                    data[key] = data[key].Replace("zation", "sation");

                    // Only replace string value for a specific key
                    if (key == 497)
                    {
                        data[key] = data[key].Replace("Fall", "Autumn");
                    }

                    else if (key == 487)
                    {
                        data[key] = "Corn Seeds/75/-300/Seeds -74/Corn Seeds/Plant these in the summer or in autumn. Takes 14 days to mature, and continues to produce after first harvest.";
                    }
                }
            }

            // Edit TV channel data
            else if (asset.AssetNameEquals("Data\\TV\\TipChannel"))
            {
                var data = asset.AsDictionary<string, string>().Data;

                // Replace specified key value with new value
                data["53"] = data["53"].Replace("Fall", "Autumn");

                foreach (string key in new List<string>(data.Keys) { "36", "67", "78", "116", "186", "102" })
                {
                    // Replace specified string with new string
                    data[key] = data[key].Replace("fall", "autumn");
                    data[key] = data[key].Replace("favorite", "favourite");
                }
            }

            else if (asset.AssetNameEquals("Data\\TV\\CookingChannel"))
            {
                var data = asset.AsDictionary<string, string>().Data;

                foreach (string key in new List<string>(data.Keys) { "18", "27", "31", "32" })
                {
                    // Replace specified string with new string
                    data[key] = data[key].Replace("favorite", "favourite");
                    data[key] = data[key].Replace("ize", "ise");
                }
            }

            // Edit mail data
            else if (asset.AssetNameEquals("Data\\mail"))
            {
                SpellingFixer();
            }

            // Edit egg festival data
            else if (asset.AssetNameEquals("Data\\Festivals\\spring13"))
            {
                SpellingFixer();
            }

            // Edit luau data
            else if (asset.AssetNameEquals("Data\\Festivals\\summer11"))
            {
                SpellingFixer();
            }

            // Edit dance of the moonlight jellies data
            else if (asset.AssetNameEquals("Data\\Festivals\\summer28"))
            {
                SpellingFixer();
            }

            // Edit spirits eve data
            else if (asset.AssetNameEquals("Data\\Festivals\\fall27"))
            {
                SpellingFixer();
            }

            // Edit feast of the winter star data
            else if (asset.AssetNameEquals("Data\\Festivals\\winter25"))
            {
                SpellingFixer();
            }

            // Edit blueprints data
            else if (asset.AssetNameEquals("Data\\Blueprints"))
            {
                SpellingFixer();
            }

            // Edit gift taste data
            else if (asset.AssetNameEquals("Data\\NPCGiftTastes"))
            {
                SpellingFixer();
            }

            // Edit quest data
            else if (asset.AssetNameEquals("Data\\Quests"))
            {
                IDictionary<int, string> data = asset.AsDictionary<int, string>().Data;

                foreach (int key in new List<int>(data.Keys))
                {
                    // Replace specified string with new string
                    data[key] = data[key].Replace("favorite", "favourite");
                    data[key] = data[key].Replace("mom", "mum");
                }
            }

            // Edit weapons data
            else if (asset.AssetNameEquals("Data\\weapons"))
            {
                IDictionary<int, string> data = asset.AsDictionary<int, string>().Data;

                foreach (int key in new List<int>(data.Keys))
                {
                    // Replace specified string with new string
                    data[key] = data[key].Replace("favorite", "favourite");
                    data[key] = data[key].Replace("honor", "honour");
                }
            }

            // Edit a single entry in hats
            else if (asset.AssetNameEquals("Data\\hats"))
            {
                IDictionary<int, string> data = asset.AsDictionary<int, string>().Data;

                // Replace specified key value with new value
                data[30] = "Watermelon Band/The colour scheme was inspired by the beloved summer melon./true/false";
            }

            // Edit a single entry in hats
            else if (asset.AssetNameEquals("Data\\Bundles"))
            {
                var data = asset.AsDictionary<string, string>().Data;

                foreach (string key in new List<string>(data.Keys))
                {
                    // Replace specified string with new string
                    data[key] = data[key].Replace("Fall", "Autumn");
                }
            }

            // Edit a single entry in BigCraftables
            else if (asset.AssetNameEquals("Data\\BigCraftablesInformation"))
            {
                IDictionary<int, string> data = asset.AsDictionary<int, string>().Data;

                // Replace specified key value with new value
                data[209] = "Mini-Jukebox/1500/-300/Crafting -9/Allows you to play your favourite tunes./true/true/0/Mini-Jukebox";
            }

            // Patch Intro tilesheet with new sign image
            else if (asset.AssetNameEquals("Minigames\\Intro"))
            {
                var editor = asset.AsImage();

                // Get image to patch to tilesheet
                Texture2D roadsign = Helper.Content.Load<Texture2D>("assets/Intro_sign.png", ContentSource.ModFolder);

                // Patch image on tilesheet
                editor.PatchImage(roadsign, targetArea: new Rectangle(48, 177, 64, 80));
            }

            // Edit movie data
            else if (asset.AssetNameEquals("Data\\Movies"))
            {
                var movieDatas = asset.Data as Dictionary<string, StardewValley.GameData.Movies.MovieData>;

                // Method to edit movie description and a movie scene
                void MovieEditor(string name, string descoriginal, string descreplace, int scenenumber, string scenename, string original, string replace)
                {
                    var movieData = movieDatas[name];

                    movieData.Description = movieData.Description.Replace(descoriginal, descreplace);

                    var sceneID = movieData.Scenes[scenenumber].ID;
                    var scene = movieData.Scenes.FirstOrDefault(s => s.ID == sceneID);

                    if (scene != null && sceneID == scenename)
                    {
                        scene.Text = scene.Text.Replace(original, replace);
                    }
                }

                if (movieDatas.ContainsKey("spring_movie_0"))
                {
                    MovieEditor("spring_movie_0", " ", " ", 4, "spring0_4", "demoralized", "demoralised");
                }

                if (movieDatas.ContainsKey("spring_movie_1"))
                {
                    MovieEditor("spring_movie_1", " ", " ", 1, "spring1_1", "80 miles", "128 kilometres");
                }

                if (movieDatas.ContainsKey("fall_movie_1"))
                {
                    MovieEditor("fall_movie_1", " ", " ", 1, "fall1_1", "30 miles", "48 kilometres");
                }

                if (movieDatas.ContainsKey("summer_movie_1"))
                {
                    MovieEditor("summer_movie_1", "centered", "centred", 6, "summer1_6", "humor", "humour");
                }

                if (movieDatas.ContainsKey("winter_movie_1"))
                {
                    MovieEditor("winter_movie_1", "theater", "theatre", 0, " ", " ", " ");
                }
            }

            // Edit movie reaction data
            else if (asset.AssetNameEquals("Data\\MoviesReactions"))
            {
                var Reactions = asset.Data as List<StardewValley.GameData.Movies.MovieCharacterReaction>;

                // Method to edit before movie reactions
                void ReactionsEditorBefore(int NPCindex, int reactionindex, string original, string replacement)
                {
                    Reactions[NPCindex].Reactions[reactionindex].SpecialResponses.BeforeMovie.Text = Reactions[NPCindex].Reactions[reactionindex].SpecialResponses.BeforeMovie.Text.Replace(original, replacement);
                }

                // Method to edit after movie reactions
                void ReactionsEditorAfter(int NPCindex, int reactionindex, string original, string replacement)
                {
                    Reactions[NPCindex].Reactions[reactionindex].SpecialResponses.AfterMovie.Text = Reactions[NPCindex].Reactions[reactionindex].SpecialResponses.AfterMovie.Text.Replace(original, replacement);
                }

                // Penny
                ReactionsEditorBefore(0, 0, "mom", "mum");
                ReactionsEditorAfter(0, 8, "favorite", "favourite");
                // Krobus
                ReactionsEditorBefore(2, 0, "recognize", "recognise");
                ReactionsEditorBefore(2, 1, "center", "centre");
                // Haley
                ReactionsEditorBefore(19, 0, "favorite", "favourite");
                ReactionsEditorBefore(19, 1, "favorite", "favourite");
                // George
                ReactionsEditorBefore(4, 4, "Theaters", "Theatres");
                // Evelyn
                ReactionsEditorBefore(6, 2, "theater", "theatre");
                // Sam
                ReactionsEditorBefore(14, 3, "theater", "theatre");
                // Maru
                ReactionsEditorBefore(20, 2, "theater", "theatre");
                // Vincent
                ReactionsEditorBefore(15, 4, "mom", "mum");
                // Demetrius
                ReactionsEditorBefore(23, 4, "analyze", "analyse");
                // Dwarf
                ReactionsEditorAfter(25, 1, "mesmerizing", "mesmerising");
            }

            // Edit concession data
            else if (asset.AssetNameEquals("Data\\Concessions"))
            {
                var Snacks = asset.Data as List<StardewValley.GameData.Movies.ConcessionItemData>;

                // Method to edit a concession item description
                void ConcessionsDescriptionEditor(int index, string original, string replacement)
                {
                    Snacks[index].Description = Snacks[index].Description.Replace(original, replacement);
                }

                // Jasmine tea
                ConcessionsDescriptionEditor(1, "flavored", "flavoured");
                // Black liquorice
                Snacks[11].DisplayName = "Black Liquorice";
                // Kale smoothie
                ConcessionsDescriptionEditor(16, "fiber", "fibre");
                // Rock candy
                ConcessionsDescriptionEditor(23, "Flavored", "Flavoured");
            }
        }
    }
}

