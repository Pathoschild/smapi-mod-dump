/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MercuriusXeno/EquivalentExchange
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquivalentExchange
{
    public class SaveDataModel
    {
        public Dictionary<long, int> AlchemyLevel { get; set; }
        public Dictionary<long, int> AlchemyExperience { get; set; }
        public Dictionary<long, float> AlkahestryMaxEnergy { get; set; }
        public Dictionary<long, float> AlkahestryCurrentEnergy { get; set; }
        public Dictionary<long, int> TotalValueTransmuted { get; set; }        
        public Dictionary<long, bool> IsSlimeGivenToWizard { get; set; }


        public SaveDataModel()
        {            
            AlchemyLevel = new Dictionary<long, int>();
            AlchemyExperience = new Dictionary<long, int>();
            AlkahestryMaxEnergy = new Dictionary<long, float>();
            AlkahestryCurrentEnergy = new Dictionary<long, float>();
            TotalValueTransmuted = new Dictionary<long, int>();
            IsSlimeGivenToWizard = new Dictionary<long, bool>();

        }        
    }
}
