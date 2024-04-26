/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jamespfluger/Stardew-NoFriendshipDecayReborn
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace NoFriendshipDecayReborn
{
    public class NoFriendshipDecayRebornEntry : Mod
    {
        /// <summary>
        /// Used to track who the player has friendships with and what their friendship values are
        /// </summary>
        private readonly PerScreen<Dictionary<string, int>> _friendshipData = new();

        /// <summary>
        /// Entrance to the No Friendship Decay - Reborn
        /// </summary>
        public override void Entry(IModHelper helper)
        {
            _friendshipData.Value = new Dictionary<string, int>();

            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
        }

        /// <summary>
        /// When a save is loaded, we clear the existing friendship data because it could be coming from another save
        /// </summary>
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs args)
        {
            _friendshipData.Value.Clear();
        }

        /// <summary>
        /// Every second, reset the friendship decay and store the values
        /// </summary>
        private void OnUpdateTicked(object sender, UpdateTickedEventArgs args)
        {
            if (args.IsOneSecond && Context.IsWorldReady)
            {
                ResetFriendshipDecay();
            }
        }

        /// <summary>
        /// Resets friendship losses and then updates what is tracked
        /// </summary>
        private void ResetFriendshipDecay()
        {
            // Reset friendship decreases back to what they were originally
            ResetFriendships();

            // Update the data we're using to track friendship values
            UpdateFriendshipData();
        }

        /// <summary>
        /// Updates the friendship data we track from the player
        /// </summary>
        private void UpdateFriendshipData()
        {
            _friendshipData.Value.Clear();

            foreach ((string name, NetRef<Friendship> friendship) in Game1.player.friendshipData.FieldDict)
            {
                _friendshipData.Value[name] = friendship.Value.Points;
            }
        }

        /// <summary>
        /// Resets the player's friendship data to use what we have stored
        /// </summary>
        private void ResetFriendships()
        {
            if (_friendshipData.Value.Any())
            {
                foreach (string key in Game1.player.friendshipData.Keys)
                {
                    Friendship friendship = Game1.player.friendshipData[key];

                    if (_friendshipData.Value.TryGetValue(key, out int oldPoints) && oldPoints > friendship.Points)
                    {
                        friendship.Points = oldPoints;
                    }
                }
            }
        }
    }
}
