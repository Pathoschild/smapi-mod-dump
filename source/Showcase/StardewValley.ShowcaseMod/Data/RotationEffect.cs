using Igorious.StardewValley.DynamicApi2.Constants;
using Newtonsoft.Json;

namespace Igorious.StardewValley.ShowcaseMod.Data
{
    public class RotationEffect
    {
        public RotationEffect() { }

        public RotationEffect(CategoryID category, int id, int n) : this(id, n)
        {
            Category = category;
        }

        public RotationEffect(int id, int n)
        {
            ID = id;
            N = n;
        }

        public RotationEffect(CategoryID category, string subCategory, int n) : this(category, n)
        {
            SubCategory = subCategory;
        }

        public RotationEffect(CategoryID category, int n)
        {
            Category = category;
            N = n;
        }

        public int? ID { get; set; }
        public CategoryID? Category {get; set; }
        public string SubCategory { get; set; }

        [JsonProperty(Required = Required.Always, DefaultValueHandling = DefaultValueHandling.Include)]
        public int N { get; set; }
    }
}