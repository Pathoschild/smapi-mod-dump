/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/CJBok/SDV-Mods
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using CJBCheatsMenu.Framework.Components;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace CJBCheatsMenu.Framework.Cheats.Relationships
{
    /// <summary>A cheat which prevents NPC friendships from decaying.</summary>
    internal class NoFriendshipDecayCheat : BaseCheat
    {
        /*********
        ** Fields
        *********/
        /// <summary>The minimum friendship points to maintain for each NPC.</summary>
        private readonly Dictionary<string, int> PreviousFriendships = new();


        /*********
        ** Public methods
        *********/
        /// <summary>Get the config UI fields to show in the cheats menu.</summary>
        /// <param name="context">The cheat context.</param>
        public override IEnumerable<OptionsElement> GetFields(CheatContext context)
        {
            yield return new CheatsOptionsCheckbox(
                label: I18n.Relationships_NoDecay(),
                value: context.Config.NoFriendshipDecay,
                setValue: value => context.Config.NoFriendshipDecay = value
            );
        }

        /// <summary>Handle the cheat options being loaded or changed.</summary>
        /// <param name="context">The cheat context.</param>
        /// <param name="needsUpdate">Whether the cheat should be notified of game updates.</param>
        /// <param name="needsInput">Whether the cheat should be notified of button presses.</param>
        /// <param name="needsRendering">Whether the cheat should be notified of render ticks.</param>
        public override void OnConfig(CheatContext context, out bool needsInput, out bool needsUpdate, out bool needsRendering)
        {
            needsInput = false;
            needsUpdate = context.Config.NoFriendshipDecay;
            needsRendering = false;
        }

        /// <summary>Handle the player loading a save file.</summary>
        /// <param name="context">The cheat context.</param>
        public override void OnSaveLoaded(CheatContext context)
        {
            this.PreviousFriendships.Clear();
        }

        /// <summary>Handle a game update if <see cref="ICheat.OnSaveLoaded"/> indicated updates were needed.</summary>
        /// <param name="context">The cheat context.</param>
        /// <param name="e">The update event arguments.</param>
        public override void OnUpdated(CheatContext context, UpdateTickedEventArgs e)
        {
            if (!e.IsOneSecond || !Context.IsWorldReady)
                return;

            // undo any friendships decreases
            if (this.PreviousFriendships.Any())
            {
                foreach (string key in Game1.player.friendshipData.Keys)
                {
                    Friendship friendship = Game1.player.friendshipData[key];
                    if (this.PreviousFriendships.TryGetValue(key, out int oldPoints) && oldPoints > friendship.Points)
                        friendship.Points = oldPoints;
                }
            }

            // update friendship tracking
            this.PreviousFriendships.Clear();
            foreach ((string name, NetRef<Friendship> friendship) in Game1.player.friendshipData.FieldDict)
                this.PreviousFriendships[name] = friendship.Value.Points;
        }

        /// <summary>Update the tracked friendship points for an NPC.</summary>
        /// <param name="npc">The NPC whose friendship to update.</param>
        /// <param name="points">The new friendship points value.</param>
        public void UpdateFriendship(NPC npc, int points)
        {
            this.PreviousFriendships[npc.Name] = points;
        }
    }
}
