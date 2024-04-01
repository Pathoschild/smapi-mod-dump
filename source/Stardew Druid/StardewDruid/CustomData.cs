/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using StardewModdingAPI.Utilities;
using System.Collections.Generic;

namespace StardewDruid
{

    public class CustomData
    {

        public List<string> colourPreferences { get; set; }

        public Dictionary<int,int> slotAttunement { get; set; }

        public List<int> roughageItems { get; set; }

        public List<int> lunchItems { get; set; }

        public CustomData()
        {

            colourPreferences = new(){ "Red", "Blue", "Purple", "Black" };

            slotAttunement = new()
            {
                [1] = 0,
                [2] = 1,
                [3] = 2,
                [4] = 3,
                [5] = 4,
            };

            roughageItems = new()
            {
                92, // Sap
                766, // Slime
                311, // PineCone
                310, // MapleSeed
                309, // Acorn
                292, // Mahogany
                767, // BatWings
                420, // RedMushroom
                831, // Taro Tuber
            };

            lunchItems = new()
            {
                399, // SpringOnion
                403, // Snackbar
                404, // FieldMushroom
                257, // Morel
                281, // Chanterelle
                152, // Seaweed
                153, // Algae
                157, // white Algae
                78, // Carrot
                227, // Sashimi
                296, // Salmonberry
                410, // Blackberry
                424, // Cheese
                24, // Parsnip
                851, // Magma Cap
                196, // Salad
                349, // Tonic

            };

        }

    }

}