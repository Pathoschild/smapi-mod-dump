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
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Characters;
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

            foreach (Pet pet in this.GetAllPets())
            {
                if (!pet.lastPetDay.TryGetValue(Game1.player.UniqueMultiplayerID, out int lastPetDay) || lastPetDay < Game1.Date.TotalDays)
                    pet.checkAction(Game1.player, pet.currentLocation);
            }
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Get all pets in the game.</summary>
        /// <remarks>Derived from <see cref="Farmer.getPet"/>.</remarks>
        private IEnumerable<Pet> GetAllPets()
        {
            return
                Game1.getFarm().characters.OfType<Pet>()
                .Concat(
                    from player in Game1.getAllFarmers()
                    from pet in Utility.getHomeOfFarmer(player).characters.OfType<Pet>()
                    select pet
                );
        }
    }
}
