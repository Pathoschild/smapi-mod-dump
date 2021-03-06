/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lisamreynolds/MusicalCellar
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System.Linq;

namespace MusicalCellar
{
    public class ModEntry : Mod
    {
        private const string CELLAR_NAME = "Cellar";
        private const string FARMHOUSE_NAME = "FarmHouse";

        private string currentlyPlaying = "";
        private string trackSource = "";

        public override void Entry(IModHelper helper)
        {
            Helper.Events.Player.Warped += Warped;
            Helper.Events.GameLoop.DayEnding += DayEnding;
            Helper.Events.GameLoop.UpdateTicked += UpdateTicked;
        }

        private void UpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (Game1.currentLocation?.Name != FARMHOUSE_NAME && Game1.currentLocation?.Name != CELLAR_NAME) return;
            if (!Game1.locations.Any(location => location.Name == CELLAR_NAME)) return;

            GameLocation room = Game1.currentLocation;
            string otherRoomName = (room.Name == FARMHOUSE_NAME) ? CELLAR_NAME : FARMHOUSE_NAME;
            GameLocation otherRoom = Game1.locations.First(l => l.Name == otherRoomName);

            if (!string.IsNullOrEmpty(currentlyPlaying) &&
                (Game1.player.isInBed ||
                 Game1.timeOfDay >= 2600 ||
                 Game1.player.stamina <= -15f))
            {
                // The player is in bed or exhausted
                StopJukeboxMusic();
                // There will be no music
                return;
            }
            if (!string.IsNullOrEmpty(currentlyPlaying) && room.Name == trackSource && !room.IsMiniJukeboxPlaying())
            {
                // The player has turned the jukebox off
                StopJukeboxMusic();
                // There may be other music to play
            }

            if (room.IsMiniJukeboxPlaying())
            {
                if (room.miniJukeboxTrack != currentlyPlaying)
                {
                    // The jukebox is on but the track has changed
                    StartJukeboxMusic(room);
                }
            }
            else if (otherRoom.IsMiniJukeboxPlaying())
            {
                if (otherRoom.miniJukeboxTrack != currentlyPlaying)
                {
                    // Overhear the music from the other room
                    StartJukeboxMusic(otherRoom);
                }
            }
        }

        private void DayEnding(object sender, DayEndingEventArgs e)
        {
            if (string.IsNullOrEmpty(currentlyPlaying)) return;
            StopJukeboxMusic();
        }

        private void Warped(object sender, WarpedEventArgs e)
        {
            if (e.OldLocation.Name == FARMHOUSE_NAME && e.NewLocation.Name != CELLAR_NAME)
            {
                LeftFarmhouse();
            }
        }

        /// <summary>
        /// Manage music when the player leaves the FarmHouse, but does not go into the Cellar
        /// </summary>
        public void LeftFarmhouse()
        {
            Monitor.Log("Left the house");
            if (string.IsNullOrEmpty(currentlyPlaying)) return;
            StopJukeboxMusic();
            StartOutsideMusic();
        }

        /// <summary>
        /// Manually start the FarmHouse "jukebox" music
        /// </summary>
        public void StartJukeboxMusic(GameLocation room)
        {
            Monitor.Log("Starting music");
            Game1.changeMusicTrack(room.miniJukeboxTrack);
            currentlyPlaying = room.miniJukeboxTrack;
            trackSource = room.Name;
        }

        /// <summary>
        /// Manually stop the FarmHouse "jukebox" music
        /// </summary>
        public void StopJukeboxMusic()
        {
            Monitor.Log("Stopping music");
            Game1.changeMusicTrack("none", track_interruptable: true);
            currentlyPlaying = "";
        }

        /// <summary>
        /// Start music (again) when the player leaves the FarmHouse
        /// </summary>
        public void StartOutsideMusic()
        {
            Game1.currentLocation.checkForMusic(Game1.currentGameTime);
            Game1.currentLocation.resetForPlayerEntry();
        }
    }
}
