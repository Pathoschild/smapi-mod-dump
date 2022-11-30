/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using StardewValley;
using StardewValley.Locations;
using StardewRoguelike.VirtualProperties;
using System;
using Microsoft.Xna.Framework;
using StardewRoguelike.ChallengeFloors;
using System.Collections.Generic;
using StardewModdingAPI.Events;
using System.Reflection;
using System.Linq;
using xTile.Dimensions;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;

namespace StardewRoguelike
{
    public class ChallengeFloor
    {
        public static readonly List<ChallengeType> History = new();

        #pragma warning disable format
        public enum ChallengeType
        {
            KOTH         = 1,
            TimedKills   = 2,
            HotSpring    = 3,
            DwarfChests  = 4,
            Race         = 5,
            SlingshotAim = 6,
            EggHunt      = 7,
            JOTPK        = 8,
            HiddenBoss   = 9,
            PickAPath    = 10,
            Speedrun     = 11,
            GambaChests  = 12
        }
        #pragma warning restore format

        /// <summary>
        /// Finds a MineShaft with the specified level.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <returns>The found MineShaft, if it exists. Otherwise null.</returns>
        public static MineShaft? GetMineFromLevel(int level)
        {
            foreach (MineShaft mine in MineShaft.activeMines)
            {
                if (Roguelike.GetLevelFromMineshaft(mine) == level)
                    return mine;
            }

            return null;
        }

        /// <summary>
        /// Checks if there has been a challenge floor since the last merchant
        /// that has appeared since the specified level.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <returns>true if there has been, false otherwise</returns>
        public static bool HadChallengeFloorSinceLastMerchant(int level)
        {
            int count = 0;

            while (!Merchant.IsMerchantFloor(level + count))
            {
                MineShaft? mine = GetMineFromLevel(level + count);
                if (mine is not null && IsChallengeFloor(mine))
                    return true;

                count--;
            }

            return false;
        }

        /// <summary>
        /// Calculates the number floors until the next boss appears, starting
        /// from the specified level.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <returns>The number of floors until the next boss.</returns>
        public static int FloorsUntilNextBoss(int level)
        {
            int count = 0;

            while (!BossFloor.IsBossFloor(level + count))
                count++;

            return count;
        }

        /// <summary>
        /// Checks whether or not the specified level should be a challenge floor.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <returns>true if it should be, false otherwise.</returns>
        public static bool ShouldDoChallengeFloor(int level)
        {
            if (DebugCommands.ForcedChallengeIndex != -1)
                return true;

            if (level == 8)
                return false;

            if (HadChallengeFloorSinceLastMerchant(level))
                return false;

            return Roguelike.FloorRng.NextDouble() < ((float)1 / FloorsUntilNextBoss(level));
        }

        /// <summary>
        /// Checks whether or not a specified MineShaft is a challenge floor.
        /// This data is stored internally on MineShaft instances.
        /// </summary>
        /// <param name="mine">The mine.</param>
        /// <returns>true if it is, false otherwise.</returns>
        public static bool IsChallengeFloor(MineShaft mine)
        {
            return mine.get_MineShaftIsChallengeFloor().Value;
        }

        /// <summary>
        /// Gets the map path to load for a specified MineShaft.
        /// If the type of challenge that the MineShaft is doesn't
        /// have any map paths specified, load one of the default maps.
        /// </summary>
        /// <param name="mine">The mine.</param>
        /// <returns>The map path to load.</returns>
        public static string GetMapPath(MineShaft mine)
        {
            ChallengeBase challenge = mine.get_MineShaftChallengeFloor();

            return
                challenge.MapPaths?[Roguelike.FloorRng.Next(challenge.MapPaths.Count)] ??
                Roguelike.ValidMineMaps[Roguelike.FloorRng.Next(Roguelike.ValidMineMaps.Count)];
        }

        /// <summary>
        /// Gets the music tracks for a specified MineShaft.
        /// </summary>
        /// <param name="mine">The mine.</param>
        /// <returns>A list of valid music tracks to play.</returns>
        public static List<string> GetMusicTracks(MineShaft mine)
        {
            ChallengeBase challenge = mine.get_MineShaftChallengeFloor();
            return challenge.GetMusicTracks(mine);
        }

        /// <summary>
        /// Finds where the spawn location is for a specified MineShaft.
        /// If a spawn location isn't specified in the ChallengeBase,
        /// use the game default.
        /// </summary>
        /// <param name="mine">The mine.</param>
        /// <returns>The spawn tile location.</returns>
        public static Vector2 GetSpawnLocation(MineShaft mine)
        {
            Vector2? tileBeneathLadder = (Vector2?)mine.GetType().GetProperty("tileBeneathLadder", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(mine);

            ChallengeBase challenge = mine.get_MineShaftChallengeFloor();
            return challenge.GetSpawnLocation(mine) ?? tileBeneathLadder ?? new(0, 0);
        }

        /// <summary>
        /// Gets a random challenge for a level from the enum of <see cref="ChallengeType"/>.
        /// The returned ChallengeBase instance is ready to be assigned to the MineShaft.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <returns>A random challenge instance.</returns>
        public static ChallengeBase GetRandomChallenge(int level)
        {
            var values = Enum.GetValues<ChallengeType>().ToList();

            ChallengeType type;
            if (DebugCommands.ForcedChallengeIndex != -1)
                type = values[DebugCommands.ForcedChallengeIndex];
            else
            {
                float healthRemaining = Game1.player.health / (float)Game1.player.maxHealth;
                if (FloorsUntilNextBoss(level) > 1 || (!Context.IsMultiplayer && healthRemaining >= 0.9f))
                    values.Remove(ChallengeType.HotSpring);

                if (level < 6)
                {
                    values.Remove(ChallengeType.PickAPath);
                    values.Remove(ChallengeType.HotSpring);
                }
                else if (level < 18)
                    values.Remove(ChallengeType.GambaChests);

                while (History.Count >= values.Count && History.Count > 0)
                    History.RemoveAt(0);

                values.RemoveAll(value => History.Contains(value));

                type = values[Roguelike.FloorRng.Next(values.Count)];
                History.Add(type);
            }

            return type switch
            {
                ChallengeType.KOTH => new KOTH(),
                ChallengeType.TimedKills => new TimedKills(),
                ChallengeType.HotSpring => new HotSpring(),
                ChallengeType.DwarfChests => new DwarfChests(),
                ChallengeType.Race => new Race(),
                ChallengeType.SlingshotAim => new SlingshotAim(),
                ChallengeType.EggHunt => new EggHunt(),
                ChallengeType.JOTPK => new JOTPK(),
                ChallengeType.HiddenBoss => new HiddenBoss(),
                ChallengeType.PickAPath => new PickAPath(),
                ChallengeType.Speedrun => new Speedrun(),
                ChallengeType.GambaChests => new GambaChests(),
                _ => throw new NotImplementedException()
            };
        }

        /// <summary>
        /// Event handler for when the local player warps.
        /// This event is passed down to the ChallengeBase instance.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="WarpedEventArgs"/> instance containing the event data.</param>
        public static void PlayerWarped(object? sender, WarpedEventArgs e)
        {
            if (!e.IsLocalPlayer)
                return;

            if (e.OldLocation is MineShaft && IsChallengeFloor((MineShaft)e.OldLocation))
            {
                ChallengeBase challenge = ((MineShaft)e.OldLocation).get_MineShaftChallengeFloor();
                challenge.PlayerLeft((MineShaft)e.OldLocation);
            }
            if (e.NewLocation is MineShaft && IsChallengeFloor((MineShaft)e.NewLocation))
            {
                ChallengeBase challenge = ((MineShaft)e.NewLocation).get_MineShaftChallengeFloor();
                challenge.PlayerEntered((MineShaft)e.NewLocation);
            }
        }

        /// <summary>
        /// Event handler for when dialogue is answered by the local player.
        /// This event is passed down to the ChallengeBase instance.
        /// </summary>
        /// <param name="mine">The mine.</param>
        /// <param name="questionAndAnswer">The question and answer.</param>
        /// <param name="questionParams">The question parameters.</param>
        /// <returns></returns>
        public static bool AnswerDialogueAction(MineShaft mine, string questionAndAnswer, string[] questionParams)
        {
            ChallengeBase challenge = mine.get_MineShaftChallengeFloor();
            if (challenge is null)
                return false;

            return challenge.AnswerDialogueAction(mine, questionAndAnswer, questionParams);
        }

        public static void DrawBeforeLocation(MineShaft mine, SpriteBatch b)
        {
            ChallengeBase challenge = mine.get_MineShaftChallengeFloor();
            if (challenge is null)
                return;

            challenge.DrawBeforeLocation(mine, b);
        }

        public static void DrawAfterLocation(MineShaft mine, SpriteBatch b)
        {
            ChallengeBase challenge = mine.get_MineShaftChallengeFloor();
            if (challenge is null)
                return;

            challenge.DrawAfterLocation(mine, b);
        }

        public static bool CheckForCollision(MineShaft mine, Microsoft.Xna.Framework.Rectangle position, Farmer who)
        {
            ChallengeBase challenge = mine.get_MineShaftChallengeFloor();
            if (challenge is null)
                return false;

            return challenge.CheckForCollision(mine, position, who);
        }

        public static bool CheckAction(MineShaft mine, Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
        {
            ChallengeBase challenge = mine.get_MineShaftChallengeFloor();
            if (challenge is null)
                return false;

            return challenge.CheckAction(mine, tileLocation, viewport, who);
        }

        public static bool PerformAction(MineShaft mine, string action, Farmer who, Location tileLocation)
        {
            ChallengeBase challenge = mine.get_MineShaftChallengeFloor();
            if (challenge is null)
                return false;

            return challenge.PerformAction(mine, action, who, tileLocation);
        }

        /// <summary>
        /// Event handler for every update ticked.
        /// This event is passed down to the ChallengeBase instance.
        /// </summary>
        /// <param name="mine">The mine.</param>
        /// <param name="time">The time.</param>
        public static void DoUpdate(MineShaft mine, GameTime time)
        {
            if (!IsChallengeFloor(mine))
                return;

            ChallengeBase challenge = mine.get_MineShaftChallengeFloor();
            challenge.Update(mine, time);
        }
    }
}
