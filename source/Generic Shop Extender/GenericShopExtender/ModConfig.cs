using System.Collections.Generic;

namespace GenericShopExtender
{
    /// <summary>The mod configuration model.</summary>
    public class ModConfig
    {
        /// <summary>The item IDs and prices to add for each shopkeeper.</summary>
        public Dictionary<string, int[,]> Shopkeepers { get; set; } = new Dictionary<string, int[,]>
        {
            ["Marnie"] = new[,]
            {
                { 174, 300 },
                { 182, 300 },
                { 186, 300 },
                { 438, 500 },
                { 440, 500 },
                { 442, 500 },
                { 444, 2000 },
                { 446, 2000 }
            }
        };
    }
}
