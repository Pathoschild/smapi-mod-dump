using System;
using GetGS;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System.Collections.Generic;
using GetGS.ContentPatcher;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Xml;
using System.IO;
using System.Text.RegularExpressions;
using StardewValley.Tools;
using System.Collections.ObjectModel;

// Used to access vairables within Gender Setter https://www.nexusmods.com/stardewvalley/mods/13434
//

//Drawn from https://github.com/Pathoschild/StardewMods/blob/stable/ContentPatcher/docs/extensibility.md and
// https://gitlab.com/daleao/sdv-mods/-/blob/main/ImmersiveValley/Common/Extensions/SMAPI/ModHelperExtensions.cs#L37

//If you want to make your own version, make sure to add Newtonsoft.Json via Nuget
// and change all the "sqbr" and "Gender Setter" stuff to the relevant creator and mod in this file and the manifest.json


namespace GetGS
{
    namespace ContentPatcher
    {
        /// <summary>The Content Patcher API which other mods can access.</summary>
        public interface IContentPatcherAPI
        {
            /*********
            ** Methods
            *********/
            /// <summary>Register a simple token.</summary>
            /// <param name="mod">The manifest of the mod defining the token (see <see cref="Mod.ModManifest"/> in your entry class).</param>
            /// <param name="name">The token name. This only needs to be unique for your mod; Content Patcher will prefix it with your mod ID automatically, like <c>YourName.ExampleMod/SomeTokenName</c>.</param>
            /// <param name="getValue">A function which returns the current token value. If this returns a null or empty list, the token is considered unavailable in the current context and any patches or dynamic tokens using it are disabled.</param>
            void RegisterToken(IManifest mod, string name, Func<IEnumerable<string>> getValue);

        }
    }

    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.GameLaunched += APIStuff; //runs the code in APIStuff at the start of the game
        }

        /*********
        ** Private Gender Setter related methods and data
        *********/

        //Makes first letter of a string into a capital
        private string Capitalise(string str)
        {
            return char.ToUpper(str[0]) + str.Substring(1);
        }


        //combines dict1 and dict2, adds upper and lower case as needed
        private Dictionary<string, string> make_p_dict(string pronoun, Dictionary<string, string> dict1, Dictionary<string, string> dict2)
        {
            var dict3 = new Dictionary<string, string>(); //Copies dict1 entries into a new dictionary, dict3
            foreach (string key in dict1.Keys)
            {
                dict3.Add(key, dict1[key]);
            }

            foreach (KeyValuePair<string, string> kvp in dict2) //Does the same thing as the previous code except with dict2, I was just experimenting with methods
            {
                dict3[kvp.Key] = kvp.Value;
            }
            dict3["They"] = pronoun.ToLower();
            string[] thems = { "They", "Them", "Their" };
            foreach (string w in thems)
            {
                dict3[w + "U"] = Capitalise(dict3[w]);
            }
            return dict3;
        }

        private readonly string[] pronouns = new string[] { "He", "She", "They", "They (singular)", "It", "Xe", "Fae", "E" };
        private readonly string[] genders = new string[] { "Male", "Female", "Neutral" };

    private void APIStuff(object sender, EventArgs e)
            // Makes all the variables visible outside the mod
            {
                var api = this.Helper.ModRegistry.GetApi<IContentPatcherAPI>("Pathoschild.ContentPatcher");

                var modInfo = this.Helper.ModRegistry.Get("sqbr.GenderSetter");
                var modPath = (string)modInfo.GetType().GetProperty("DirectoryPath")!.GetValue(modInfo)!; //You would think ".." would work but it does not
                var config = JObject.Parse(File.ReadAllText(Path.Combine(modPath, "config.json")));

                Regex regex = new Regex("^[a-zA-Z0-9]*$");// checks if it's alphanumeric

                var variable_dict = new Dictionary<string, string>();

                foreach (JProperty property in config.Properties()) //Goes through each property listed in config.json
                {
                    string Name = property.Name;
                    string Value = (string)property.Value;

                    if (regex.IsMatch(Name)) // checks if it's alphanumeric, to filter out headings
                    {
                    variable_dict[Name] = Value;
                    api.RegisterToken(this.ModManifest, Name, () => new[] { (string)Value }); //adds a token with name Name and value Value
                    }
                }

                var singular_words = new Dictionary<string, string> {
                    { "Are","is"},
                    { "Were","was" },
                    { "Have","has"},
                    { "Re","'s"},
                    { "Ve","'s"},
                    { "S","s"},
                    { "Es","es"}};

                var plural_words = new Dictionary<string, string> {
                    { "Are","are"},
                    { "Were","were" },
                    { "Have","have"},
                    { "Re","'re"},
                    { "Ve","'ve"},
                    { "S",""},
                    { "Es",""}};

                var she_words = new Dictionary<string, string> {
                    {"Them","her" },{"Their","her" },{"Theirs","hers" },{"Themself","herself"} };
                var he_words = new Dictionary<string, string> {
                    {"Them","him" },{"Their","his" },{"Theirs","his" },{"Themself","himself"} };
                var they_words = new Dictionary<string, string> {
                    {"Them","them" },{"Their","their" },{"Theirs","theirs" },{"Themself","themself"} };
                var it_words = new Dictionary<string, string> {
                    {"Them","it" },{"Their","its" },{"Theirs","its" },{"Themself","itself"} };
                var xe_words = new Dictionary<string, string> {
                    {"Them","xem" },{"Their","xyr" },{"Theirs","xyrs" },{"Themself","xemself"} };
                var fae_words = new Dictionary<string, string> {
                    {"Them","faer" },{"Their","faer" },{"Theirs","faers" },{"Themself","faerself"} };
                var e_words = new Dictionary<string, string> {
                    {"Them","em" },{"Their","eir" },{"Theirs","eirs" },{"Themself","emself"} };
                var ze_words = new Dictionary<string, string> {
                    {"Them","hir" },{"Their","hir" },{"Theirs","hirs" },{"Themself","hirself"} };

                var pronoun_words = new Dictionary<string, Dictionary<string, string>>{
                    { "She", make_p_dict("She",she_words ,singular_words) },
                    { "He", make_p_dict("He", he_words, singular_words) },
                    { "They", make_p_dict("They", they_words, plural_words) },
                    {"They (singular)", make_p_dict("They", they_words, singular_words) },
                    { "It", make_p_dict("She",it_words ,singular_words) },
                    { "Xe", make_p_dict("He", xe_words, singular_words) },
                    { "Fae", make_p_dict("She",fae_words ,singular_words) },
                    { "E", make_p_dict("He", e_words, singular_words) },
                    { "Ze", make_p_dict("He", ze_words, singular_words) } };


                var female_words = new Dictionary<string, string> {
                    { "Adult","woman"},
                    { "Guy","girl" },
                    { "Kid","girl" },
                    { "Child","daughter" },
                    { "Sibling","sister" },
                    { "Nibling","niece" },
                    { "Auncle","aunt" },
                    { "AuncleU","Aunt" },
                    { "Parent","mother" },
                    { "ParentName","mom" },
                    { "ParentU","Mom" },
                    { "Spouse","wife" },
                    { "SpouseU","Wife" },
                    { "Mx","Mrs." },
                    { "Grandparent","Granny"} };

                var male_words = new Dictionary<string, string> {
                    { "Adult","man"},
                    { "Guy","guy" },
                    { "Kid","boy" },
                    { "Child","son" },
                    { "Sibling","brother" },
                    { "Nibling","nephew" },
                    { "Auncle","uncle" },
                    { "AuncleU","Uncle" },
                    { "Parent","father" },
                    { "ParentName","dad" },
                    { "ParentU","Dad" },
                    { "Spouse","husband" },
                    { "SpouseU","Husband" },
                    { "Mx","Mr." },
                    { "Grandparent","Grandpa"} };

                var neutral_words = new Dictionary<string, string> {
                    { "Adult","person"},
                    { "Guy","person" },
                    { "Kid","kid" },
                    { "Child","child" },
                    { "Sibling","sibling" },
                    { "Nibling","nibling" },
                    { "Auncle","auncle" },
                    { "AuncleU","Auncle" },
                    { "Parent","parent" },
                    { "ParentName","parent" },
                    { "ParentU","Parent" },
                    { "Spouse","spouse" },
                    { "SpouseU","Spouse" },
                    { "Mx","Mx." },
                    { "Grandparent","Grandie"} };

                var gender_words = new Dictionary<string, Dictionary<string, string>>{
                         { "Female",female_words},{ "Male",male_words},{ "Neutral",neutral_words}
                };

                var female_exceptions_dict = new Dictionary<string, string>
                {
                    { "LewisMx", "Ms." }, {"MorrisMx", "Ms." },{"PennyMx", "Miss" },{"BirdieGuy", "lady" },{"GovernorAdult", "girl"}
                };
                var male_exceptions_dict = new Dictionary<string, string>
                {
                    { "PennyMx", "Mister" }, {"BirdieGuy", "man" },{"GovernorAdult", "guy"}
                };

                var neutral_exceptions_dict = new Dictionary<string, string> {
                    { "VincentGuy","kid" },{"EvelynGrandparent","Grandie "+ variable_dict["EvelynName"] }, {"GeorgeGrandparent", "Grandie "+ variable_dict["GeorgeName"] } };

                foreach (string name in new string[] { "Pierre", "Caroline", "Kent", "Jodi", "Robin", "Demetrius", "Evelyn" })
                {
                neutral_exceptions_dict[name + "ParentName"] = "parent " + variable_dict[name + "Name"];
                }

                var gender_exceptions = new Dictionary<string, Dictionary<string, string>>{
                         { "Female",female_exceptions_dict},{ "Male", male_exceptions_dict},{ "Neutral",neutral_exceptions_dict}
                };

                string[] name_list = new string[] { "Abigail", "Alex", "Birdie", "Bouncer", "Caroline", "Charlie", "Clint", "Demetrius", "Dwarf", "Elliott", "Emily", "Evelyn", "Farmer", "George", "Gil", "Governor", "Grandpa", "Gunther", "Gus", "Haley", "Harvey", "Henchman", "Jas", "Jodi", "Kent", "Krobus", "Leah", "Leo", "Lewis", "Linus", "Marcello", "Marlon", "Marnie", "Maru", "MisterQi", "Morris", "OldMariner", "Pam", "Penny", "Pierre", "ProfessorSnail", "Robin", "Sam", "Sandy", "Sebastian", "Shane", "Vincent", "Willy", "Witch","Wizard" };

                foreach (string name in name_list) {
                    var g = variable_dict[name+"Gender"]; //gender of this character as in config.json
                    if (g == "false") { g = "Neutral"; }
                        
                    var g_dict = gender_words[g];
                    foreach (KeyValuePair<string, string> kvp in g_dict)
                    {
                        string word = name +  kvp.Key;
                        string variable = kvp.Value;
                        var e_dict = gender_exceptions[g];
                        if (e_dict.ContainsKey(word))
                        {
                            variable = e_dict[word];
                        }

                        api.RegisterToken(this.ModManifest, word, () => new[] { variable }); //registers tokens like AbigailSpouseU
                    }
                    var p = variable_dict[name + "Pronoun"]; //pronouns of this character as in config.json
                    g_dict = pronoun_words[p];
                    foreach (KeyValuePair<string, string> kvp in g_dict)
                    {
                        string word = name + kvp.Key;

                        api.RegisterToken(this.ModManifest, word, () => new[] { kvp.Value }); //registers tokens like AbigailTheyU

                }
                    if (name != "Farmer")
                {
                    if ((variable_dict["PossessiveS"] == "false") && (name[name.Length - 1] == 's'))
                        api.RegisterToken(this.ModManifest, name + "Possession", () => new[] { "'" });
                    else
                        api.RegisterToken(this.ModManifest, name + "Possession", () => new[] { "'s" });
                }
                    if (variable_dict["WizardName"].Length>0) 
                        api.RegisterToken(this.ModManifest, "WizardInitial", () => new[] { variable_dict["WizardName"].Substring(0, 1) });
                    else
                        api.RegisterToken(this.ModManifest, "WizardInitial", () => new[] { "M" });

            }
                
        }
    }
}
