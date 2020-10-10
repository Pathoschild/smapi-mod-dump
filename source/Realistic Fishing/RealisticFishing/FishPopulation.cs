/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/kevinforrestconnors/RealisticFishing
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using PyTK.CustomElementHandler;
using RealiticFishing;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.Tools;

namespace RealisticFishing
{

    public class FishPopulation : ISyncableElement
    {

        public int CurrentFishIDCounter;
        public Dictionary<string, List<FishModel>> population;
        public List<Tuple<int, string, int, int, int>> AllFish;
        public List<string> TrapFish;

        public PySync syncObject { get; set; }

        public FishPopulation()
        {

            this.CurrentFishIDCounter = 0;

            this.AllFish = new List<Tuple<int, string, int, int, int>>();

            foreach (KeyValuePair<int, string> item in Game1.content.Load<Dictionary<int, string>>("Data\\Fish"))
            {
                string[] fishFields = item.Value.Split('/');

                if (fishFields[1] != "trap" && fishFields[0] != "Green Algae" && fishFields[0] != "White Algae" && fishFields[0] != "Seaweed")
                {
                    this.AllFish.Add(new Tuple<int, string, int, int, int>(item.Key, fishFields[0] + " ", int.Parse(fishFields[3]), int.Parse(fishFields[4]), 1));
                } else if (fishFields[1] == "trap") {
                    this.TrapFish.Add(fishFields[0]);
                }
            }

            this.population = new Dictionary<string, List<FishModel>>();


            Random rand = new Random();

            for (int i = 0; i < this.AllFish.Count; i++)
            {

                List<FishModel> thisFishPopulation = new List<FishModel>();

                int populationSize = 50;

                for (int j = 0; j < populationSize; j++)
                {
                    string name = this.AllFish[i].Item2;
                    int minLength = this.AllFish[i].Item3;
                    int maxLength = this.AllFish[i].Item4;

                    int quality;
                    double r = rand.NextDouble();

                    if (r > 0.99)
                    {
                        quality = 4;
                    }
                    else if (r > 0.95)
                    {
                        quality = 2;
                    }
                    else if (r > 0.85)
                    {
                        quality = 1;
                    }
                    else
                    {
                        quality = 0;
                    }

                    double length = EvolutionHelpers.GetMutatedFishLength((maxLength + minLength) / 2, minLength, maxLength);

                    thisFishPopulation.Add(new FishModel(this.CurrentFishIDCounter, name, minLength, maxLength, length, quality));
                    this.CurrentFishIDCounter++;
                }

                this.population.Add(this.AllFish[i].Item2, thisFishPopulation);
            }
        }

        public double GetAverageLengthOfFish(String fishName)
        {

            List<FishModel> fishOfType;
            this.population.TryGetValue(fishName, out fishOfType);

            double avg = 0.0;

            foreach (FishModel fish in fishOfType)
            {
                avg += fish.length;
            }

            avg /= fishOfType.Count;

            return avg;
        }

        // todo choose better # for value
        public bool IsAverageFishBelowValue(String fishName, double value = 0.66)
        {

            double avgLength = this.GetAverageLengthOfFish(fishName);

            List<FishModel> fishOfType;
            this.population.TryGetValue(fishName, out fishOfType);

            double originalPopulationAverage = (fishOfType[0].minLength + fishOfType[0].maxLength) / 2;

            return (avgLength / originalPopulationAverage) < value;
        }

        public bool IsAverageFishAboveValue(String fishName, double value = 0.66)
        {

            double avgLength = this.GetAverageLengthOfFish(fishName);

            List<FishModel> fishOfType;
            this.population.TryGetValue(fishName, out fishOfType);

            double originalPopulationAverage = (fishOfType[0].minLength + fishOfType[0].maxLength) / 2;

            return (avgLength / originalPopulationAverage) > (1 + value / 2);
        }

        public String PrintChangedFish(List<String> filter)
        {
            String ret = "";

            for (int i = 0; i < this.AllFish.Count; i++)
            {

                string fishName = this.AllFish[i].Item2;

                List<FishModel> fishOfType;
                this.population.TryGetValue(fishName, out fishOfType);

                if (fishOfType.Count > 0 && (filter.Contains(fishName) || filter.Count == 0))
                {
                    ret += fishOfType[0].name + " | Number of Fish: " + fishOfType.Count + " | Average Length: " + this.GetAverageLengthOfFish(fishName) + "\n";
                }
            }

            return ret;
        }

        public Dictionary<string, string> getSyncData()
        {
            Dictionary<string, string> savedata = new Dictionary<string, string>();
            savedata.Add("CurrentFishIDCounter", this.CurrentFishIDCounter.ToString());
            savedata.Add("population", JsonConvert.SerializeObject(this.population));
            return savedata;
        }

        public void sync(Dictionary<string, string> syncData)
        {
            this.CurrentFishIDCounter = int.Parse(syncData["CurrentFishIDCounter"]);
            this.population = JsonConvert.DeserializeObject<Dictionary<string, List<FishModel>>>(syncData["population"]);
        }

        public object getReplacement()
        {
            Chest replacement = new Chest(true);
            return replacement;
        }

        public Dictionary<string, string> getAdditionalSaveData()
        {
            Dictionary<string, string> savedata = new Dictionary<string, string>();
            savedata.Add("CurrentFishIDCounter", this.CurrentFishIDCounter.ToString());
            savedata.Add("population", JsonConvert.SerializeObject(this.population));
            return savedata;
        }

        public void rebuild(Dictionary<string, string> additionalSaveData, object replacement)
        {
            this.CurrentFishIDCounter = int.Parse(additionalSaveData["CurrentFishIDCounter"]);
            this.population = JsonConvert.DeserializeObject<Dictionary<string, List<FishModel>>>(additionalSaveData["population"]);

            this.AllFish = new List<Tuple<int, string, int, int, int>>();

            foreach (KeyValuePair<int, string> item in Game1.content.Load<Dictionary<int, string>>("Data\\Fish"))
            {
                string[] fishFields = item.Value.Split('/');

                if (fishFields[1] != "trap" && fishFields[0] != "Green Algae" && fishFields[0] != "White Algae" && fishFields[0] != "Seaweed")
                {
                    this.AllFish.Add(new Tuple<int, string, int, int, int>(item.Key, fishFields[0] + " ", int.Parse(fishFields[3]), int.Parse(fishFields[4]), 1));
                } else if (fishFields[1] == "trap")
                {
                    this.TrapFish.Add(fishFields[0]);
                }
            }
        }
    }
}