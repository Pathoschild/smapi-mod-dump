using Newtonsoft.Json;

namespace MTD.SDVTwitch.Models
{
    public class Follow
    {
        [JsonProperty("user")]
        public User User { get; set; }
    }
}
