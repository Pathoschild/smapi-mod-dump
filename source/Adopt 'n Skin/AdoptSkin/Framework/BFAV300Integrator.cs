using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StardewModdingAPI;

namespace AdoptSkin.Framework
{
    public interface IModApiBFAVBeta
    {
        Dictionary<string, List<string>> GetFarmAnimalCategories();
    }


    /// <summary>
    /// Upon instantiation, checks whether Better Farm Animal Variety is a loaded mod, then allows skin control for BFAV added animals
    /// ** THIS IS ONLY FOR THE CURRENT BFAV PUBLISHED VERSION, VERSION 2.2.6 **
    /// </summary>
    class BFAV300Integrator
    {
        internal IModApiBFAVBeta BFAVApi;

        public BFAV300Integrator()
        {
            if (ModEntry.SHelper.ModRegistry.IsLoaded("Paritee.BetterFarmAnimalVariety"))
            {
                BFAVApi = ModEntry.SHelper.ModRegistry.GetApi<IModApiBFAVBeta>("Paritee.BetterFarmAnimalVariety");
                RegisterBFAVAnimals();
            }
        }


        public bool BFAVIsLoaded()
        {
            if (BFAVApi != null)
                return true;
            return false;
        }


        public void RegisterBFAVAnimals()
        {
            Dictionary<string, List<string>> bfavAnimals = BFAVApi.GetFarmAnimalCategories();

            foreach (KeyValuePair<string, List<string>> pair in bfavAnimals)
            {
                foreach (string type in pair.Value)
                {

                    // Only register types that are not already registered
                    if (!ModApi.IsRegisteredType(type))
                    {
                        string newType = ModEntry.Sanitize(pair.Key);
                        if (newType.Contains("dinosaur"))
                            ModApi.RegisterAnimalType(type, false, false);
                        else if (newType.Contains("sheep"))
                            ModApi.RegisterAnimalType(type, true, true);
                        else
                            ModApi.RegisterAnimalType(type);
                    }
                }
            }

        }



    }
}
