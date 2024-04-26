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
using CJBCheatsMenu.Framework.Components;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Characters;
using StardewValley.Locations;
using StardewValley.Menus;

namespace CJBCheatsMenu.Framework.Cheats.FarmAndFishing
{
    /// <summary>A cheat which automatically pets pet animals.</summary>
    internal class AutoPetPetsCheat : BaseCheat
    {
        /*********
        ** Public methods
        *********/
        /// <summary>Get the config UI fields to show in the cheats menu.</summary>
        /// <param name="context">The cheat context.</param>
        public override IEnumerable<OptionsElement> GetFields(CheatContext context)
        {
            yield return new CheatsOptionsCheckbox(
                label: I18n.Farm_AutoPetPets(),
                value: context.Config.AutoPetPets,
                setValue: value => context.Config.AutoPetPets = value
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
            needsUpdate = context.Config.AutoPetPets;
            needsRendering = false;
        }

        /// <summary>Handle the player loading a save file.</summary>
        /// <param name="context">The cheat context.</param>
        /// <summary>Handle a game update if <see cref="ICheat.OnSaveLoaded"/> indicated updates were needed.</summary>
        /// <param name="e">The update event arguments.</param>
        public override void OnUpdated(CheatContext context, UpdateTickedEventArgs e)
        {
            if (!e.IsOneSecond || !Context.IsWorldReady)
                return;

            // Some mods (like A New Dream) add custom pets outside the farm which react to being pet. Only pet pets in
            // the standard locations (i.e. farm and farmhouses) to avoid issues like repeating dialogues.
            Farm farm = Game1.getFarm();
            this.ApplyInLocation(farm);
            foreach (Building building in farm.buildings)
            {
                if (building.GetIndoors() is FarmHouse home)
                    this.ApplyInLocation(home);
            }
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Pet all pets in a location.</summary>
        /// <param name="location">The location to search for pets.</param>
        private void ApplyInLocation(GameLocation location)
        {
            if (location.characters.Count > 0)
            {
                foreach (NPC character in location.characters)
                {
                    if (character is Pet pet && (!pet.lastPetDay.TryGetValue(Game1.player.UniqueMultiplayerID, out int lastPetDay) || lastPetDay != Game1.Date.TotalDays))
                        pet.checkAction(Game1.player, pet.currentLocation);
                }
            }
        }
    }
}
