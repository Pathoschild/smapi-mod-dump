using MTD.SDVTwitch.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace MTD.SDVTwitch
{
    public class TwitchClient
    {
        internal static Config _config;

        public TwitchClient(Config config)
        {
            _config = config;
        }

        public List<User> GetUsersByName(string name)
        {
            var users = new List<User>();
            var response = GetFollowersByName(name);

            foreach (var follow in response.Follows)
            {
                users.Add(follow.User);
            }

            return users;
        }

        public FollowerResponse GetFollowersByName(string name)
        {
            var request = (HttpWebRequest)WebRequest.Create("https://api.twitch.tv/kraken/channels/" + name + "/follows");
            request.Headers["Client-Id"] = _config.ClientId;
            var response = request.GetResponse();
            var responseText = "";

            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                responseText = sr.ReadToEnd();
            }

            return JsonConvert.DeserializeObject<FollowerResponse>(responseText);
        }
    }
}
