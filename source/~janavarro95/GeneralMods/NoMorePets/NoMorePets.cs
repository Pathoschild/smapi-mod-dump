using System.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Characters;

namespace Omegasis.NoMorePets
{
    /// <summary>The mod entry point.</summary>
    public class NoMorePets : Mod
    {
        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the player loads a save slot and the world is initialised.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        public void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            foreach (Pet pet in Game1.player.currentLocation.getCharacters().OfType<Pet>().ToArray())
                pet.currentLocation.characters.Remove(pet);
        }
    }
}
