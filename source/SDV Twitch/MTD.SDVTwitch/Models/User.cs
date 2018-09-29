using Newtonsoft.Json;

namespace MTD.SDVTwitch
{
    public class User
    {
        [JsonProperty("display_name")]
        public string DisplayName { get; set; }
    }
}
