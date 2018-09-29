using MTD.SDVTwitch.Models;
using StardewModdingAPI;
using StardewValley;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MTD.SDVTwitch
{
    public class SDVTwitch : Mod
    {
        internal static Config _config;
        private List<User> _followersAtStart = new List<User>();
        private TwitchClient _client;

        public override void Entry(IModHelper helper)
        {
            _config = helper.ReadJsonFile<Config>("config.json");
            _client = new TwitchClient(_config);
            _followersAtStart = _client.GetUsersByName(_config.TwitchName);

            new Thread(TwitchLoop).Start();
        }

        private void TwitchLoop()
        {
            while (true)
            {
                if (Context.IsWorldReady)
                {
                    var users = _client.GetUsersByName(_config.TwitchName);

                    foreach (var user in users)
                    {
                        if (_followersAtStart.FirstOrDefault(u => u.DisplayName == user.DisplayName) == null)
                        {
                            _followersAtStart.Add(user);

                            // 1 - Star
                            // 2 - !
                            // 3 - X
                            // 4 - Green Cross
                            // 5 - Red Cross
                            Game1.addHUDMessage(new HUDMessage($"New Follower: {user.DisplayName}!", 2));
                        }
                    }
                }

                Thread.Sleep(15000);
            }
        }
    }
}
