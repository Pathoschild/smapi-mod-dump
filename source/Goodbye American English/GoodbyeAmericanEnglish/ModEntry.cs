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
using HarmonyLib;
using StardewModdingAPI.Events;
using System.Linq;
using Netcode;
using Microsoft.Xna.Framework.Input;
using System.Xml.Linq;

namespace GoodbyeAmericanEnglish
{
    public class ModEntry
        : Mod
    {
        private ModConfig config;
        private static IModHelper helperstatic;
        private static IMonitor monitorstatic;
        private static Dictionary<string, string> namereplacer = new Dictionary<string, string>();

        // Array to hold NPC names
        private static readonly string[] NPCs =
        {
            "Abigail",
            "Alex",
            "Caroline",
            "Clint",
            "Demetrius",
            "Dwarf",
            "Elliott",
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
            "Leo",
            "LeoMainland",
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
        private static readonly string[] locations =
        {
            "AbandonedJojaMart",
            "AnimalShop",
            "ArchaeologyHouse",
            "BackWoods",
            "BathHouse_Pool",
            "Beach",
            "BoatTunnel",
            "BusStop",
            "CommunityCenter",
            "ElliottHouse",
            "Farm",
            "FarmHouse",
            "FishShop",
            "Forest",
            "HaleyHouse",
            "HarveyRoom",
            "Hospital",
            "IslandFarmHouse",
            "IslandHut",
            "IslandNorth",
            "IslandSouth",
            "IslandWest",
            "JoshHouse",
            "LeahHouse",
            "ManorHouse",
            "Mine",
            "Mountain",
            "QiNutRoom",
            "Railroad",
            "Saloon",
            "SamHouse",
            "SandyHouse",
            "ScienceHouse",
            "SebastianRoom",
            "SeedShop",
            "Sewer",
            "Summit",
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
            this.config = this.Helper.ReadConfig<ModConfig>();
            helperstatic = this.Helper;
            monitorstatic = this.Monitor;
            

            var replacer = this.Helper.Data.ReadJsonFile<NameReplacer>("NameReplacer.json");

            if (replacer == null)
            {
                replacer = new NameReplacer();
                this.Helper.Data.WriteJsonFile("NameReplacer.json", replacer);
            }
            else
            {
                namereplacer = this.Helper.ModContent.Load<Dictionary<string, string>>("NameReplacer.json") ?? null;
            }
            
            helper.Events.Content.AssetRequested += this.AssetRequested;

            if (this.config.AllowAdvancedNameReplacer == true)
            {
                var harmony = new Harmony(this.ModManifest.UniqueID);

                harmony.Patch(
                    original: AccessTools.PropertyGetter(typeof(StardewValley.Object), nameof(StardewValley.Object.DisplayName)),
                    postfix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.DisplayName_Postfix))
                    );
            }
        }

        private static string PreservestringFromEnum(StardewValley.Object.PreserveType preservetype)
        {
            switch (preservetype)
            {
                case StardewValley.Object.PreserveType.Wine:
                    return "Wine";
                case StardewValley.Object.PreserveType.Jelly:
                    return "Jelly";
                case StardewValley.Object.PreserveType.Pickle:
                    return "Pickles";
                case StardewValley.Object.PreserveType.Juice:
                    return "Juice";
                case StardewValley.Object.PreserveType.Roe:
                    return "Roe";
                case StardewValley.Object.PreserveType.DriedFruit:
                    return "DriedFruit";
                case StardewValley.Object.PreserveType.DriedMushroom:
                    return "DriedMushroom";
                case StardewValley.Object.PreserveType.SmokedFish:
                    return "SmokedFish";
                case StardewValley.Object.PreserveType.Bait:
                    return "Bait";
                case StardewValley.Object.PreserveType.Honey:
                    return "Honey";
                default:
                    return "AgedRoe";
            }
        }

        private static void DisplayName_Postfix(StardewValley.Object __instance, ref string __result)
        {
            try
            {
                if (namereplacer != null && __instance.preserve.Value.HasValue == true)
                {
                    string preservedName = (__instance.preservedParentSheetIndex.Value != null) ? ItemRegistry.GetDataOrErrorItem("(O)" + __instance.preservedParentSheetIndex.Value).DisplayName : null;
                    if (preservedName != null)
                    {
                        if (namereplacer.ContainsKey($"P_{"(O)" + __instance.preservedParentSheetIndex.Value}_{PreservestringFromEnum(__instance.preserve.Value.GetValueOrDefault())}") == true)
                        {
                            var itemidvalue = namereplacer[$"P_{"(O)" + __instance.preservedParentSheetIndex.Value}_{PreservestringFromEnum(__instance.preserve.Value.GetValueOrDefault())}"];
                            var newname = __instance.displayName;

                            string nameextension = (__instance.IsRecipe ? (((CraftingRecipe.craftingRecipes.ContainsKey(__instance.displayName) && CraftingRecipe.craftingRecipes[__instance.displayName].Split('/')[2].Split(' ').Count() > 1) ? (" x" + CraftingRecipe.craftingRecipes[__instance.displayName].Split('/')[2].Split(' ')[1]) : "") + Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12657")) : "");
                            string nameprefix = __instance.orderData.Value == "QI_COOKING" ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Fresh_Prefix", __instance.displayName) : "";

                            string[] fields = itemidvalue.Split('/');

                            if (fields.Length == 2)
                            {
                                switch (fields[0])
                                {
                                    case "suffix":
                                        newname = $"{preservedName} {fields[1]}";
                                        break;
                                    case "prefix":
                                        newname = $"{fields[1]} {preservedName}";
                                        break;
                                    case "replace":
                                    default:
                                        newname = string.Format(fields[1], preservedName);
                                        break;
                                }
                                __result = nameprefix + newname + nameextension;
                            }
                        }

                        // Legacy NameReplacement
                        else if (namereplacer.ContainsKey($"{ __instance.preservedParentSheetIndex.Value}_{PreservestringFromEnum(__instance.preserve.Value.GetValueOrDefault())}") == true)
                        {
                            var itemidvalue = namereplacer[$"{ __instance.preservedParentSheetIndex.Value}_{PreservestringFromEnum(__instance.preserve.Value.GetValueOrDefault())}"];
                            var newname = __instance.displayName;

                            string nameextension = (__instance.IsRecipe ? (((CraftingRecipe.craftingRecipes.ContainsKey(__instance.displayName) && CraftingRecipe.craftingRecipes[__instance.displayName].Split('/')[2].Split(' ').Count() > 1) ? (" x" + CraftingRecipe.craftingRecipes[__instance.displayName].Split('/')[2].Split(' ')[1]) : "") + Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12657")) : "");
                            string nameprefix = __instance.orderData.Value == "QI_COOKING" ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Fresh_Prefix", __instance.displayName) : "";

                            string[] fields = itemidvalue.Split('/');

                            if (fields.Length == 3 && fields[0] == "P")
                            {
                                switch (fields[1])
                                {
                                    case "suffix":
                                        newname = $"{preservedName} {fields[2]}";
                                        break;
                                    case "prefix":
                                        newname = $"{fields[2]} {preservedName}";
                                        break;
                                    case "replace":
                                    default:
                                        newname = string.Format(fields[2], preservedName);
                                        break;
                                }
                                monitorstatic.LogOnce($"Legacy format in namereplacer used for independent preserve edit, this won't be supported in the future", LogLevel.Warn);
                                __result = nameprefix + newname + nameextension;
                            }
                        }
                       
                    }                  
                }
            }
            catch (Exception ex)
            {
                monitorstatic.Log($"Failed to replace name. Details:\n{ex}", LogLevel.Error);
            }
           
        }
        private void AssetRequested(object sender, AssetRequestedEventArgs e)
        {

            bool IsEditableAsset()
            {
                foreach (var name in NPCs)
                {
                    // If asset name contains any iteration in NPCs array, return true
                    if (false
                        || e.NameWithoutLocale.IsEquivalentTo($"Characters\\Dialogue\\Marriage{name}")
                        || e.NameWithoutLocale.IsEquivalentTo($"Characters\\Dialogue\\{name}")
                        || e.NameWithoutLocale.IsEquivalentTo($"Strings\\schedules\\{name}"))
                    {
                        return true;
                    }
                }

                foreach (var location in locations)
                {
                    // If asset name contains any iteration in locations array, return true
                    if (false
                        || e.NameWithoutLocale.IsEquivalentTo($"Data\\Events\\{location}"))
                    {
                        return true;
                    }
                }

                // return true if assest name is ANY of the following....
                return (false
                        || e.NameWithoutLocale.IsEquivalentTo("Strings\\StringsFromCSFiles")
                        || e.NameWithoutLocale.IsEquivalentTo("Strings\\UI")
                        || e.NameWithoutLocale.IsEquivalentTo("Strings\\Locations")
                        || e.NameWithoutLocale.IsEquivalentTo("Strings\\StringsFromMaps")
                        || e.NameWithoutLocale.IsEquivalentTo("Strings\\Notes")
                        || e.NameWithoutLocale.IsEquivalentTo("Strings\\Characters")
                        || e.NameWithoutLocale.IsEquivalentTo("Strings\\SpecialOrderStrings")
                        || e.NameWithoutLocale.IsEquivalentTo("Strings\\Objects")
                        || e.NameWithoutLocale.IsEquivalentTo("Data\\TV\\TipChannel")
                        || e.NameWithoutLocale.IsEquivalentTo("Data\\TV\\CookingChannel")
                        || e.NameWithoutLocale.IsEquivalentTo("Data\\mail")
                        || e.NameWithoutLocale.IsEquivalentTo("Data\\ClothingInformation")
                        || e.NameWithoutLocale.IsEquivalentTo("Data\\ExtraDialogue")
                        || e.NameWithoutLocale.IsEquivalentTo("Data\\SecretNotes")
                        || e.NameWithoutLocale.IsEquivalentTo("Data\\NPCGiftTastes")
                        || e.NameWithoutLocale.IsEquivalentTo("Data\\Quests")
                        || e.NameWithoutLocale.IsEquivalentTo("Data\\Blueprints")
                        || e.NameWithoutLocale.IsEquivalentTo("Data\\Bundles")
                        || e.NameWithoutLocale.IsEquivalentTo("Strings\\Weapons")
                        || e.NameWithoutLocale.IsEquivalentTo("Data\\hats")
                        || e.NameWithoutLocale.IsEquivalentTo("Data\\Fish")
                        || e.NameWithoutLocale.IsEquivalentTo("Data\\RandomBundles")
                        || e.NameWithoutLocale.IsEquivalentTo("Data\\ObjectContextTags")
                        || e.NameWithoutLocale.IsEquivalentTo("Strings\\MovieConcessions")
                        || e.NameWithoutLocale.IsEquivalentTo("Strings\\Movies")
                        || e.NameWithoutLocale.IsEquivalentTo("Strings\\MoviesReactions")
                        || e.NameWithoutLocale.IsEquivalentTo("Data\\Festivals\\spring13")
                        || e.NameWithoutLocale.IsEquivalentTo("Data\\Festivals\\spring24")
                        || e.NameWithoutLocale.IsEquivalentTo("Data\\Festivals\\summer11")
                        || e.NameWithoutLocale.IsEquivalentTo("Data\\Festivals\\summer28")
                        || e.NameWithoutLocale.IsEquivalentTo("Data\\Festivals\\fall27")
                        || e.NameWithoutLocale.IsEquivalentTo("Data\\Festivals\\winter25")
                        || e.NameWithoutLocale.IsEquivalentTo("Minigames\\Intro")
                        || e.NameWithoutLocale.IsEquivalentTo("Strings\\BigCraftables")
                        || e.NameWithoutLocale.IsEquivalentTo("Characters\\Dialogue\\MarriageDialogue")
                        || e.NameWithoutLocale.IsEquivalentTo("Strings\\1_6_Strings")
                        || e.NameWithoutLocale.IsEquivalentTo("Strings\\Shirts")
                        || e.NameWithoutLocale.IsEquivalentTo("Strings\\Furniture")
                        || e.NameWithoutLocale.IsEquivalentTo("Strings\\Buildings")
                        || e.NameWithoutLocale.IsEquivalentTo("Strings\\animationDescriptions")
                        );
            }

            if (IsEditableAsset() == true)
            {
                e.Edit(asset =>
                {
                    // Method to hold common word replacements and conditions
                    void SpellingFixer()
                    {
                        var data = asset.AsDictionary<string, string>().Data;

                        foreach (string key in new List<string>(data.Keys))
                        {
                            // Replace specified string with new string

                            data[key] = data[key].Replace("olor", "olour");
                            data[key] = data[key].Replace("behavior", "behaviour");
                            data[key] = data[key].Replace("ize", "ise");
                            data[key] = data[key].Replace("izing", "ising");
                            data[key] = data[key].Replace("zation", "sation");
                            data[key] = data[key].Replace(" center", " centre");
                            data[key] = data[key].Replace(" Center", " Centre");
                            data[key] = data[key].Replace("mom", "mum");
                            data[key] = data[key].Replace("Mom", "Mum");
                            data[key] = data[key].Replace("theater", "theatre");
                            data[key] = data[key].Replace("Theater", "Theatre");
                            data[key] = data[key].Replace("counselor", "counsellor");
                            data[key] = data[key].Replace("onor", "onour");
                            data[key] = data[key].Replace("umor", "umour");
                            data[key] = data[key].Replace("avor", "avour");
                            data[key] = data[key].Replace("eighbor", "eighbour");
                            data[key] = data[key].Replace("traveling", "travelling");
                            data[key] = data[key].Replace("travele", "travelle");
                            data[key] = data[key].Replace("Traveli", "Travelli");
                            data[key] = data[key].Replace("cozy", "cosy");
                            data[key] = data[key].Replace("fiber", "fibre");
                            data[key] = data[key].Replace("efense", "efence");
                            data[key] = data[key].Replace("airplane", "aeroplane");
                            data[key] = data[key].Replace("rtifact", "rtefact");
                            data[key] = data[key].Replace("anceled", "ancelled");
                            data[key] = data[key].Replace("anceling", "ancelling");
                            data[key] = data[key].Replace(" tons ", " tonnes ");
                            data[key] = data[key].Replace("controlable", "controllable");
                            data[key] = data[key].Replace("Math:", "Maths:");
                            data[key] = data[key].Replace(" odor", " odour");
                            data[key] = data[key].Replace("scepter", "sceptre");
                            data[key] = data[key].Replace("Scepter", "Sceptre");
                            data[key] = data[key].Replace("Fiber", "Fibre");
                            data[key] = data[key].Replace("jewelry", "jewellry");
                            data[key] = data[key].Replace("Paper Mache", "Papier Mâché");
                            data[key] = data[key].Replace("marvelous", "marvellous");
                            data[key] = data[key].Replace("Gray ", "Grey ");
                            data[key] = data[key].Replace(" gray", " grey");


                            if (this.config.MetricSystem == true)
                            {
                                data[key] = data[key].Replace("twenty miles", "thirty kilometres");
                                data[key] = data[key].Replace("six inches", "fifteen centimetres");
                                data[key] = data[key].Replace("{0} in.", "{0} cm.");
                                data[key] = data[key].Replace("inches", "centimetres");
                            }

                            if (this.config.FalltoAutumn == true)
                            {
                                data[key] = data[key].Replace("the fall", "autumn");
                                data[key] = data[key].Replace("fall", "autumn");
                                data[key] = data[key].Replace("Fall", "Autumn");

                                data[key] = data[key].Replace("autumnen", "fallen");
                                data[key] = data[key].Replace("autumn out", "fall out");
                                data[key] = data[key].Replace("autumning", "falling");
                                data[key] = data[key].Replace("autumn on", "fall on");
                                data[key] = data[key].Replace("autumns", "falls");
                                data[key] = data[key].Replace("autumn asleep", "fall asleep");
                                data[key] = data[key].Replace("autumn prey", "fall prey");
                                data[key] = data[key].Replace("autumnFest", "fallFest");
                                data[key] = data[key].Replace("Autumn Of Planet", "Fall Of Planet");
                                data[key] = data[key].Replace("autumn_", "fall_");
                                data[key] = data[key].Replace("curtains autumn", "curtains fall");
                                data[key] = data[key].Replace("autumn for", "fall for");
                                data[key] = data[key].Replace("LinusAutumn", "LinusFall");
                                data[key] = data[key].Replace("Autumnen", "Fallen");
                                data[key] = data[key].Replace("Autumns", "Falls");
                            }

                            // Correct word replacement that shouldn't occur
                            data[key] = data[key].Replace("mument", "moment");
                            data[key] = data[key].Replace("JoshMum", "JoshMom");
                            data[key] = data[key].Replace(" sise", " size");
                            data[key] = data[key].Replace("citisen", "citizen");
                            data[key] = data[key].Replace("cardamum", "cardamom");
                            data[key] = data[key].Replace("bgColour", "bgColor");
                            data[key] = data[key].Replace("Prise", "Prize");
                            data[key] = data[key].Replace(" prise", " prize");
                            data[key] = data[key].Replace("_apologise", "_apologize");
                            data[key] = data[key].Replace("WildColour", "WildColor");
                            data[key] = data[key].Replace("honourary", "honorary");
                            data[key] = data[key].Replace("ancell ", "ancel ");
                            data[key] = data[key].Replace("Fibre_Seeds", "Fiber_Seeds");
                        }
                    }

                    foreach (string name in NPCs)
                    {
                        // Edit character specific dialogue
                        if (e.NameWithoutLocale.IsEquivalentTo($"Characters\\Dialogue\\{name}"))
                        {
                            SpellingFixer();
                        }

                        // Edit character specific marriage dialogue
                        else if (e.NameWithoutLocale.IsEquivalentTo($"Characters\\Dialogue\\MarriageDialogue{name}"))
                        {
                            SpellingFixer();
                        }

                        // Edit schedule dialogue
                        else if (e.NameWithoutLocale.IsEquivalentTo($"Strings\\schedules\\{name}"))
                        {
                            SpellingFixer();
                        }
                    }

                    // Edit events
                    foreach (string location in locations)
                    {
                        if (e.NameWithoutLocale.IsEquivalentTo($"Data\\Events\\{location}"))
                        {
                            SpellingFixer();
                        }
                    }

                    // Edit strings
                    if (e.NameWithoutLocale.IsEquivalentTo("Strings\\StringsFromCSFiles"))
                    {
                        SpellingFixer();
                        IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;

                        if (namereplacer != null)
                        {
                            foreach (string itemid in new List<string>(namereplacer.Keys))
                            {
                                string[] keyfields = itemid.Split('_');
                                string[] fields = namereplacer[itemid].Split('/');

                                if (fields.Length == 2 && keyfields[0] == "PP" && (fields[0] == "prefix" || fields[0] == "suffix"))
                                {                          
                                    if (fields[0] == "suffix")
                                    {
                                        switch (keyfields[1])
                                        {
                                            case "Juice":
                                                data["Object.cs.12726"] = data["Object.cs.12726"].Replace("Juice", $"{fields[1]}");
                                                break;
                                            case "Wine":
                                                data["Object.cs.12730"] = data["Object.cs.12730"].Replace("Wine", $"{fields[1]}");
                                                break;
                                            case "Pickles":
                                                data["Object.cs.12735"] = "{0} " + $"{fields[1]}";
                                                break;
                                            case "Jelly":
                                                data["Object.cs.12739"] = data["Object.cs.12739"].Replace("Jelly", $"{fields[1]}");
                                                break;
                                            case "Wild Honey":
                                                data["Object.cs.12750"] = data["Object.cs.12750"].Replace("Wild Honey", $"{fields[1]}");
                                                break;
                                            case "Honey":
                                                data["Object.cs.12760"] = data["Object.cs.12760"].Replace("Honey", $"{fields[1]}");
                                                break;
                                            case "Roe":
                                                data["Roe_DisplayName"] = data["Roe_DisplayName"].Replace("Roe", $"{fields[1]}");
                                                data["AgedRoe_DisplayName"] = data["AgedRoe_DisplayName"].Replace("Roe", $"{fields[1]}");
                                                break;
                                        }
                                    }

                                    // Not suffix, must be prefix
                                    else
                                    {
                                        switch (keyfields[1])
                                        {
                                            case "Juice":
                                                data["Object.cs.12726"] = $"{fields[1]}" + " {0}";
                                                break;
                                            case "Wine":
                                                data["Object.cs.12730"] = $"{fields[1]}" + " {0}";
                                                break;
                                            case "Pickles":
                                                data["Object.cs.12735"] = data["Object.cs.12735"].Replace("Pickled", $"{fields[1]}");
                                                break;
                                            case "Jelly":
                                                data["Object.cs.12739"] = $"{fields[1]}" + " {0}";
                                                break;
                                            case "Wild Honey":
                                                data["Object.cs.12750"] = data["Object.cs.12750"].Replace("Wild Honey", $"{fields[1]}");
                                                break;
                                            case "Honey":
                                                data["Object.cs.12760"] = $"{fields[1]}" + " {0}";
                                                break;
                                            case "Roe":
                                                data["Roe_DisplayName"] = $"{fields[1]}" + " {0}";
                                                data["AgedRoe_DisplayName"] = $"Aged {fields[1]}" + " {0}";
                                                break;
                                        }
                                    }
                                }

                                // Legacy NameReplacer
                                else if (fields.Length == 3 && fields[0] == "PP" && (fields[1] == "prefix" || fields[1] == "suffix"))
                                {
                                    this.Monitor.LogOnce($"Legacy format in namereplacer used for generic preserve edit, this won't be supported in the future", LogLevel.Warn);
                                    if (fields[1] == "suffix")
                                    {
                                        switch (keyfields[0])
                                        {
                                            case "Juice":
                                                data["Object.cs.12726"] = data["Object.cs.12726"].Replace("Juice", $"{fields[2]}");
                                                break;
                                            case "Wine":
                                                data["Object.cs.12730"] = data["Object.cs.12730"].Replace("Wine", $"{fields[2]}");
                                                break;
                                            case "Pickles":
                                                data["Object.cs.12735"] = "{0} " + $"{fields[2]}";
                                                break;
                                            case "Jelly":
                                                data["Object.cs.12739"] = data["Object.cs.12739"].Replace("Jelly", $"{fields[2]}");
                                                break;
                                            case "Wild Honey":
                                                data["Object.cs.12750"] = data["Object.cs.12750"].Replace("Wild Honey", $"{fields[2]}");
                                                break;
                                            case "Honey":
                                                data["Object.cs.12760"] = data["Object.cs.12760"].Replace("Honey", $"{fields[2]}");
                                                break;
                                            case "Roe":
                                                data["Roe_DisplayName"] = data["Roe_DisplayName"].Replace("Roe", $"{fields[2]}");
                                                data["AgedRoe_DisplayName"] = data["AgedRoe_DisplayName"].Replace("Roe", $"{fields[2]}");
                                                break;
                                        }
                                    }

                                    // Not suffix, must be prefix
                                    else
                                    {
                                        switch (keyfields[0])
                                        {
                                            case "Juice":
                                                data["Object.cs.12726"] = data["Object.cs.12726"].Replace("Juice", $"{fields[2]}");
                                                break;
                                            case "Wine":
                                                data["Object.cs.12730"] = data["Object.cs.12730"].Replace("Wine", $"{fields[2]}");
                                                break;
                                            case "Pickles":
                                                data["Object.cs.12735"] = "{0} " + $"{fields[2]}";
                                                break;
                                            case "Jelly":
                                                data["Object.cs.12739"] = data["Object.cs.12739"].Replace("Jelly", $"{fields[2]}");
                                                break;
                                            case "Wild Honey":
                                                data["Object.cs.12750"] = data["Object.cs.12750"].Replace("Wild Honey", $"{fields[2]}");
                                                break;
                                            case "Honey":
                                                data["Object.cs.12760"] = data["Object.cs.12760"].Replace("Honey", $"{fields[2]}");
                                                break;
                                            case "Roe":
                                                data["Roe_DisplayName"] = data["Roe_DisplayName"].Replace("Roe", $"{fields[2]}");
                                                data["AgedRoe_DisplayName"] = data["AgedRoe_DisplayName"].Replace("Roe", $"{fields[2]}");
                                                break;
                                        }
                                    }
                                }
                                                                                              
                            }
                        }
                    }

                    // Edit general marriage dialogue
                    else if (e.NameWithoutLocale.IsEquivalentTo($"Characters\\Dialogue\\MarriageDialogue"))
                    {
                        SpellingFixer();
                    }

                    // Edit UI strings
                    else if (e.NameWithoutLocale.IsEquivalentTo("Strings\\UI"))
                    {
                        SpellingFixer();
                    }

                    // Edit location strings
                    else if (e.NameWithoutLocale.IsEquivalentTo("Strings\\Locations"))
                    {
                        SpellingFixer();
                    }

                    // Edit strings from maps
                    else if (e.NameWithoutLocale.IsEquivalentTo("Strings\\StringsFromMaps"))
                    {
                        SpellingFixer();
                    }

                    // Edit library books
                    else if (e.NameWithoutLocale.IsEquivalentTo("Strings\\Notes"))
                    {
                        SpellingFixer();
                    }

                    // Edit character strings
                    else if (e.NameWithoutLocale.IsEquivalentTo("Strings\\Characters"))
                    {
                        SpellingFixer();
                    }

                    // Edit extra dialogue
                    else if (e.NameWithoutLocale.IsEquivalentTo("Data\\ExtraDialogue"))
                    {
                        SpellingFixer();
                    }

                    // Edit Shirt strings
                    else if (e.NameWithoutLocale.IsEquivalentTo("Strings\\Shirts"))
                    {
                        SpellingFixer();
                    }

                    // Edit furniture
                    else if (e.NameWithoutLocale.IsEquivalentTo("Strings\\Furniture"))
                    {
                        SpellingFixer();
                    }

                    // Edit animationDescriptions
                    else if (e.NameWithoutLocale.IsEquivalentTo("Strings\\animationDescriptions"))
                    {
                        IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;

                        data["sam_guitar"] = "%Sam is busy practising the guitar.";
                    }

                    // Edit buildings
                    else if (e.NameWithoutLocale.IsEquivalentTo("Strings\\Buildings"))
                    {
                        IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;

                        data["Cabin_Description"] = "A home for a friend! Subsidised by the town agricultural fund.";
                    }

                    // Edit secret notes
                    else if (e.NameWithoutLocale.IsEquivalentTo("Data\\SecretNotes"))
                    {
                        IDictionary<int, string> data = asset.AsDictionary<int, string>().Data;


                        foreach (int key in new List<int>(data.Keys))
                        {
                            // Replace specified string with new string
                            data[key] = data[key].Replace("favorite", "favourite");
                            data[key] = data[key].Replace("Mom", "Mum");
                            data[key] = data[key].Replace("efense", "efence");
                        }
                    }

                    // Edit clothing information
                    else if (e.NameWithoutLocale.IsEquivalentTo("Data\\ClothingInformation"))
                    {
                        IDictionary<int, string> data = asset.AsDictionary<int, string>().Data;

                        foreach (int key in new List<int>(data.Keys))
                        {
                            // Replace specified string with new string
                            data[key] = data[key].Replace("olor", "olour");
                        }
                    }

                    // Edit Object information data
                    else if (e.NameWithoutLocale.IsEquivalentTo("Strings\\Objects"))
                    {

                        IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;

                        foreach (string key in new List<string>(data.Keys))
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

                            if (this.config.FalltoAutumn == true)
                            {
                                data[key] = data[key].Replace("the fall", "autumn");
                                data[key] = data[key].Replace("A fall", "An autumn");
                                data[key] = data[key].Replace("fall", "autumn");
                                data[key] = data[key].Replace("(Fa)", "(Au)");

                                // Only replace string value for a specific key
                                if (key == "FallSeeds_Name")
                                {
                                    data[key] = data[key].Replace("Fall", "Autumn");
                                }

                                else if (key == "CornSeeds_Description")
                                {
                                    data[key] = "Plant these in the summer or in autumn. Takes 14 days to mature, and continues to produce after first harvest.";
                                }
                            }
                            // Replace string with new word

                            data[key] = data[key].Replace("color", "colour");
                            data[key] = data[key].Replace("avor", "avour");
                            data[key] = data[key].Replace("Fossilized", "Fossilised");
                            data[key] = data[key].Replace("Deluxe Fertilizer", "Deluxe Fertiliser");
                            data[key] = data[key].Replace("Quality Fertilizer", "Quality Fertiliser");
                            data[key] = data[key].Replace("Basic Fertilizer", "Basic Fertiliser");
                            data[key] = data[key].Replace("Tree Fertilizer", "Tree Fertiliser");
                            data[key] = data[key].Replace("appetizer", "appetiser");
                            data[key] = data[key].Replace("fertilize", "fertilise");
                            data[key] = data[key].Replace("theater", "theatre");
                            data[key] = data[key].Replace("zation", "sation");
                            data[key] = data[key].Replace("Artifact", "Artefact");
                            data[key] = data[key].Replace("iber", "ibre");
                            data[key] = data[key].Replace("izing", "ising");
                            data[key] = data[key].Replace(" luster", " lustre");
                            data[key] = data[key].Replace("honor", "honour");
                            data[key] = data[key].Replace("Omelet", "Omelette");
                            data[key] = data[key].Replace("savory", "savoury");

                            try
                            {
                                if (namereplacer != null)
                                {
                                    foreach (string itemid in new List<string>(namereplacer.Keys))
                                    {
                                        string[] fields = namereplacer[itemid].Split('/');

                                        if (fields.Count() == 1)
                                        {
                                            data[$"{itemid.Replace(" ", "")}_Name"] = fields[0];
                                        }

                                        // Legacy NameReplacer
                                        else if (fields.Count() == 3 && fields[0] == "O")
                                        {
                                            var itemname = fields[1].Replace(" ","");
                                            data[$"{itemname}_Name"] = fields[2];
                                            this.Monitor.LogOnce($"Legacy format in namereplacer used for {fields[1]}, this won't be supported in the future", LogLevel.Warn);
                                        }
                                    }
                                }
                            }
                            catch
                            {
                                this.Monitor.LogOnce("NameReplacer.json not found");
                            }

                        }
                    }

                    // Edit TV channel data
                    else if (e.NameWithoutLocale.IsEquivalentTo("Data\\TV\\TipChannel"))
                    {
                        SpellingFixer();
                    }

                    else if (e.NameWithoutLocale.IsEquivalentTo("Data\\TV\\CookingChannel"))
                    {
                        SpellingFixer();
                    }

                    // Edit mail data
                    else if (e.NameWithoutLocale.IsEquivalentTo("Data\\mail"))
                    {
                        SpellingFixer();
                    }

                    // Edit egg festival data
                    else if (e.NameWithoutLocale.IsEquivalentTo("Data\\Festivals\\spring13"))
                    {
                        SpellingFixer();
                    }

                    // Edit flower dance data
                    else if (e.NameWithoutLocale.IsEquivalentTo("Data\\Festivals\\spring24"))
                    {
                        SpellingFixer();
                    }

                    // Edit luau data
                    else if (e.NameWithoutLocale.IsEquivalentTo("Data\\Festivals\\summer11"))
                    {
                        SpellingFixer();
                    }

                    // Edit dance of the moonlight jellies data
                    else if (e.NameWithoutLocale.IsEquivalentTo("Data\\Festivals\\summer28"))
                    {
                        SpellingFixer();
                    }

                    // Edit spirits eve data
                    else if (e.NameWithoutLocale.IsEquivalentTo("Data\\Festivals\\fall27"))
                    {
                        SpellingFixer();
                    }

                    // Edit feast of the winter star data
                    else if (e.NameWithoutLocale.IsEquivalentTo("Data\\Festivals\\winter25"))
                    {
                        SpellingFixer();
                    }

                    // Edit blueprints data
                    else if (e.NameWithoutLocale.IsEquivalentTo("Data\\Blueprints"))
                    {
                        SpellingFixer();
                    }

                    // Edit gift taste data
                    else if (e.NameWithoutLocale.IsEquivalentTo("Data\\NPCGiftTastes"))
                    {
                        SpellingFixer();
                    }

                    // Edit 1.6 strings
                    else if (e.NameWithoutLocale.IsEquivalentTo("Strings\\1_6_Strings"))
                    {
                        SpellingFixer();
                    }

                    // Edit quest data
                    else if (e.NameWithoutLocale.IsEquivalentTo("Data\\Quests"))
                    {
                        IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;

                        foreach (string key in new List<string>(data.Keys))
                        {
                            // Replace specified string with new string
                            data[key] = data[key].Replace("avor", "avour");
                            data[key] = data[key].Replace("mom", "mum");
                        }
                    }

                    // Edit weapons data
                    else if (e.NameWithoutLocale.IsEquivalentTo("Strings\\Weapons"))
                    {
                        IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;

                        foreach (string key in new List<string>(data.Keys))
                        {
                            // Replace specified string with new string
                            data[key] = data[key].Replace("favorite", "favourite");
                            data[key] = data[key].Replace("honor", "honour");
                            data[key] = data[key].Replace("chiseled", "chiselled");
                        }
                    }

                    // Edit a single entry in hats
                    else if (e.NameWithoutLocale.IsEquivalentTo("Data\\hats"))
                    {
                        IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;

                        // Replace specified key value with new value
                        data["9"] = "Goblin Mask/Freak out the neighbourhood with this creepy mask. Rubber ear joints for effect./true/true//Goblin Mask";
                        data["30"] = "Watermelon Band/The colour scheme was inspired by the beloved summer melon./true/false//Watermelon Band";
                        data["31"] = "Mouse Ears/Made from synthetic fibres./true/true//Mouse Ears";
                        data["80"] = "Bluebird Mask/Wear this to look just like your favourite island trader./hide/true//Bluebird Mask";
                    }

                    // Edit a single entry in bundles
                    else if (e.NameWithoutLocale.IsEquivalentTo("Data\\Bundles") && this.config.FalltoAutumn == true)
                    {
                        var data = asset.AsDictionary<string, string>().Data;

                        foreach (string key in new List<string>(data.Keys))
                        {
                            // Replace specified string with new string
                            data[key] = data[key].Replace("Fall", "Autumn");
                        }
                    }

                    // Edit BigCraftables
                    else if (e.NameWithoutLocale.IsEquivalentTo("Strings\\BigCraftables"))
                    {
                        IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;

                        // Replace specified key value with new value
                        data["MiniJukebox_Description"] = "Allows you to play your favourite tunes.";
                        data["BoneMill_Description"] = "Turns bone items into fertilisers.";
                        data["SlothSkeletonL_Description"] = "This extinct sloth roamed the lush, prehistoric forests of Stardew Valley. Its powerful jaw tore through the toughest plant fibres.";
                        data["SuitOfArmor_Name"] = "Suit of Armour";
                    }

                    // Patch Intro tilesheet with new sign image
                    else if (e.NameWithoutLocale.IsEquivalentTo("Minigames\\Intro") && this.config.MetricSystem == true)
                    {
                        var editor = asset.AsImage();

                        // Get image to patch to tilesheet
                        Texture2D roadsign = Helper.ModContent.Load<Texture2D>("assets/Intro_sign.png");

                        // Patch image on tilesheet
                        editor.PatchImage(roadsign, targetArea: new Rectangle(48, 177, 64, 80));
                    }

                    // Edit movie data
                    else if (e.NameWithoutLocale.IsEquivalentTo("Strings\\Movies"))
                    {
                        var moviestrings = asset.AsDictionary<string, string>().Data;

                        moviestrings["BraveLittleSapling_Scene5"] = moviestrings["BraveLittleSapling_Scene5"].Replace("demoralized", "demoralised");
                        moviestrings["Wumbus_Scene7"] = moviestrings["Wumbus_Scene7"].Replace("humor", "humour");
                        moviestrings["Wumbus_Description"] = moviestrings["Wumbus_Description"].Replace("centered", "centred");
                        moviestrings["ZuzuCityExpress_Description"] = moviestrings["ZuzuCityExpress_Description"].Replace("theaters", "theatres");

                        if (this.config.MetricSystem == true)
                        {
                            moviestrings["NaturalWonders_Scene2"] = moviestrings["NaturalWonders_Scene2"].Replace("80 miles", "128 kilometres");
                            moviestrings["ItHowlsInTheRain_Scene2"] = moviestrings["ItHowlsInTheRain_Scene2"].Replace("30 miles", "48 kilometres");

                        }                      

                    }

                    // Edit movie reaction data
                    else if (e.NameWithoutLocale.IsEquivalentTo("Strings\\MoviesReactions"))
                    {
                        var Reactions = asset.AsDictionary<string, string>().Data;

                        Reactions["Penny_*_BeforeMovie"] = Reactions["Penny_*_BeforeMovie"].Replace("mom", "mum");
                        Reactions["Penny_dislike_AfterMovie"] = Reactions["Penny_*_BeforeMovie"].Replace("favorite", "favourite");
                        Reactions["Krobus_fall_movie_0_BeforeMovie"] = Reactions["Krobus_fall_movie_0_BeforeMovie"].Replace("recognize", "recognise");
                        Reactions["Krobus_*_BeforeMovie"] = Reactions["Krobus_*_BeforeMovie"].Replace("center", "centre");
                        Reactions["Haley_winter_movie_1_BeforeMovie"] = Reactions["Haley_winter_movie_1_BeforeMovie"].Replace("favorite", "favourite");
                        Reactions["Haley_winter_movie_0_BeforeMovie"] = Reactions["Haley_winter_movie_0_BeforeMovie"].Replace("favorite", "favourite");
                        Reactions["George_like_BeforeMovie"] = Reactions["George_like_BeforeMovie"].Replace("Theaters", "Theatres");
                        Reactions["Evelyn_love_BeforeMovie"] = Reactions["Evelyn_love_BeforeMovie"].Replace("theater", "theatre");
                        Reactions["Sam_like_BeforeMovie"] = Reactions["Sam_like_BeforeMovie"].Replace("theater", "theatre");
                        Reactions["Maru_like_BeforeMovie"] = Reactions["Maru_like_BeforeMovie"].Replace("theater", "theatre");
                        Reactions["Vincent_love_BeforeMovie"] = Reactions["Vincent_love_BeforeMovie"].Replace("mom", "mum");
                        Reactions["Demetrius_like_BeforeMovie"] = Reactions["Demetrius_like_BeforeMovie"].Replace("analyze", "analyse");
                        Reactions["Dwarf_love_AfterMovie"] = Reactions["Dwarf_love_AfterMovie"].Replace("mesmerizing", "mesmerising");
                    }

                    // Edit concession data
                    else if (e.NameWithoutLocale.IsEquivalentTo("Strings\\MovieConcessions"))
                    {
                        var Snacks = asset.AsDictionary<string, string>().Data; 

                        // Jasmine tea
                        Snacks["JasmineTea_Description"] = Snacks["JasmineTea_Description"].Replace("flavored", "flavoured");
                        // Black liquorice
                        Snacks["BlackLicorice_Name"] = Snacks["BlackLicorice_Name"].Replace("Licorice", "Liquorice");
                        // Kale smoothie
                        Snacks["KaleSmoothie_Description"] = Snacks["KaleSmoothie_Description"].Replace("fiber", "fibre");
                        // Rock candy
                        Snacks["RockCandy_Description"] = Snacks["RockCandy_Description"].Replace("Flavored", "Flavoured");
                        // Sour slimes
                        Snacks["SourSlimes_Description"] = Snacks["SourSlimes_Description"].Replace("colors", "colours");

                        try
                        {
                            if (namereplacer != null)
                            {
                                foreach (string itemid in new List<string>(namereplacer.Keys))
                                {
                                    string[] keyfields = itemid.Split('_');
                                    string[] valuefields = namereplacer[itemid].Split("/");
                                    if (keyfields.Length == 2 && keyfields[1] == "C")
                                    {
                                        if (valuefields.Length == 1)
                                        {
                                            var name = keyfields[0].Replace(" ", "");
                                            Snacks[$"{name}_Name"] = valuefields[0];
                                        }
                                        // Legacy NameReplacer
                                        else
                                        {
                                            var name = valuefields[0].Replace(" ", "");
                                            Snacks[$"{name}_Name"] = valuefields[1];
                                            this.Monitor.LogOnce($"Legacy format in namereplacer used for concession, this won't be supported in the future", LogLevel.Warn);
                                        }

                                    }
                                }
                            }
                        }
                        catch
                        {
                            this.Monitor.LogOnce("NameReplacer.json not found");
                        }
                    }

                    //Edit random bundles
                    else if (e.NameWithoutLocale.IsEquivalentTo("Data\\RandomBundles") && this.config.FalltoAutumn == true)
                    {
                        var bundle = asset.Data as List<StardewValley.GameData.Bundles.RandomBundleData>;

                        void BundleNameReplacer(int roomindex, int bundlesetindex, int bundleindex, string newname)
                        {
                            bundle[roomindex].BundleSets[bundlesetindex].Bundles[bundleindex].Name = newname;
                        }

                        BundleNameReplacer(0, 0, 2, "Autumn Foraging");
                        BundleNameReplacer(1, 0, 2, "Autumn Crops");

                    }

                    // Edit fish data to convert inches to centimetres
                    else if (e.NameWithoutLocale.IsEquivalentTo("Data\\Fish") && this.config.MetricSystem == true)
                    {
                        IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;

                        foreach (string key in new List<string>(data.Keys))
                        {
                            // Skip replacement for trap fish, they don't have a size
                            if (false
                                || key == "715"
                                || key == "717"
                                || key == "723"
                                || key == "372"
                                || key == "720"
                                || key == "718"
                                || key == "719"
                                || key == "721"
                                || key == "716"
                                || key == "722")
                            {
                                continue;
                            }

                            string[] fields = data[key].Split('/');

                            fields[3] = ((int)Math.Round((int.Parse(fields[3]) * 2.54))).ToString();
                            fields[4] = ((int)Math.Round((int.Parse(fields[4]) * 2.54))).ToString();

                            data[key] = string.Join("/", fields);
                        }
                    }

                    // Edit special order strings
                    else if (e.NameWithoutLocale.IsEquivalentTo("Strings\\SpecialOrderStrings"))
                    {

                        IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;

                        // Replace specified string with new string
                        data["Gus_Name"] = data["Gus_Name"].Replace("Omelet", "Omelette");
                        data["Gus_RE_Greeting_0"] = data["Gus_RE_Greeting_0"].Replace("omelet", "omelette");
                        data["Gus_RE_Greeting_1"] = data["Gus_RE_Greeting_1"].Replace("omelet", "omelette");
                        data["Evelyn_Text"] = data["Evelyn_Text"].Replace("avor", "avour");
                        data["Gunther_Text"] = data["Gunther_Text"].Replace("paleontologists", "palaeontologists");
                        data["Pam_Text"] = data["Pam_Text"].Replace("whallop", "wallop");

                    }
                });
            }
        }
    }
}

