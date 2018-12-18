
using StardewValley;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vocalization.Framework
{
    public class ReplacementStrings
    {

        public string farmerName = "<Player's Name>";
        public string bandName = "<Sam's Band Name>";
        public string bookName = "<Elliott's Book Name>";
        public string rivalName = "<Rival's Name>";
        public string petName = "<Pet's Name>";
        public string farmName = "<Farm Name>";
        public string favoriteThing = "<Favorite Thing>";
        public string kid1Name = "<Kid 1's Name>";
        public string kid2Name = "<Kid 2's Name>";

        public List<string> adjStrings;
        public List<string> nounStrings;
        public List<string> placeStrings;
        public List<string> spouseNames;


        public ReplacementStrings()
        {
            loadAdjStrings();
            loadNounStrings();
            loadPlaceStrings();
            loadSpouseStrings();
        } 

        public void loadAdjStrings()
        {
            //load in adj strings from StringsFromCS and add them to this list. Then in sanitizaion is where you make all of the possible combinations for input.
            adjStrings = new List<string>();

            Dictionary<string, string> extraStrings = Vocalization.ModHelper.Content.Load<Dictionary<string, string>>(Path.Combine("Strings", "StringsFromCSFiles.xnb"),StardewModdingAPI.ContentSource.GameContent);

            for(int i = 679; i <= 698; i++)
            {
                string d = "Dialogue.cs.";
                string combo = d + i.ToString();
                string dialogue = "";
                bool exists = extraStrings.TryGetValue(combo, out dialogue);
                if (exists)
                {
                    adjStrings.Add(dialogue);
                }
            }

        }

        public void loadNounStrings()
        {
            nounStrings = new List<string>();

            Dictionary<string, string> extraStrings = Vocalization.ModHelper.Content.Load<Dictionary<string, string>>(Path.Combine("Strings", "StringsFromCSFiles.xnb"),StardewModdingAPI.ContentSource.GameContent);

            for (int i = 699; i <= 721; i++)
            {
                string d = "Dialogue.cs.";
                string combo = d + i.ToString();
                string dialogue = "";
                bool exists = extraStrings.TryGetValue(combo, out dialogue);
                if (exists)
                {
                    adjStrings.Add(dialogue);
                }
            }
        }

        public void loadPlaceStrings()
        {
            placeStrings = new List<string>();

            Dictionary<string, string> extraStrings = Vocalization.ModHelper.Content.Load<Dictionary<string, string>>(Path.Combine("Strings", "StringsFromCSFiles.xnb"), StardewModdingAPI.ContentSource.GameContent);

            for (int i = 735; i <= 759; i++)
            {
                string d = "Dialogue.cs.";
                string combo = d + i.ToString();
                string dialogue = "";
                bool exists = extraStrings.TryGetValue(combo, out dialogue);
                if (exists)
                {
                    adjStrings.Add(dialogue);
                }
            }
        }

        /// <summary>
        /// Load all associated spouse names.
        /// </summary>
        public void loadSpouseStrings()
        {
            spouseNames = new List<string>();
            foreach(var loc in Game1.locations)
            {
                foreach(var character in loc.characters)
                {
                    if (character.datable.Value)
                    {
                        spouseNames.Add(character.Name);
                    }
                }
            }
        }


    }
}
