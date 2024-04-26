/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/CJBok/SDV-Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using CJBCheatsMenu.Framework.Components;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using Object = StardewValley.Object;

namespace CJBCheatsMenu.Framework.Cheats.FarmAndFishing
{
    /// <summary>A cheat which automatically fills animal feed troughs.</summary>
    internal class AutoFeedAnimalsCheat : BaseCheat
    {
        /*********
        ** Public methods
        *********/
        /// <summary>Get the config UI fields to show in the cheats menu.</summary>
        /// <param name="context">The cheat context.</param>
        public override IEnumerable<OptionsElement> GetFields(CheatContext context)
        {
            yield return new CheatsOptionsCheckbox(
                label: I18n.Farm_AutoFeedAnimals(),
                value: context.Config.AutoFeed,
                setValue: value => context.Config.AutoFeed = value
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
            needsUpdate = context.Config.AutoFeed;
            needsRendering = false;
        }

        /// <summary>Handle a game update if <see cref="ICheat.OnSaveLoaded"/> indicated updates were needed.</summary>
        /// <param name="context">The cheat context.</param>
        /// <param name="e">The update event arguments.</param>
        public override void OnUpdated(CheatContext context, UpdateTickedEventArgs e)
        {
            if (!e.IsOneSecond || !Context.IsWorldReady)
                return;

            Utility.ForEachLocation(
                location =>
                {
                    switch (location)
                    {
                        case AnimalHouse animalHouse:
                            {
                                int animalCount = Math.Min(animalHouse.animalsThatLiveHere.Count, animalHouse.animalLimit.Value);
                                if (animalHouse.numberOfObjectsWithName("Hay") >= animalCount)
                                    break;

                                int tileX = animalHouse.Name.Contains("Barn")
                                    ? 8
                                    : 6;

                                for (int i = 0; i < animalCount; i++)
                                {
                                    Vector2 tile = new(tileX + i, 3);
                                    if (!animalHouse.objects.ContainsKey(tile))
                                    {
                                        Object hay = GameLocation.GetHayFromAnySilo(animalHouse) ?? ItemRegistry.Create<Object>("(O)178"); // if no hay is available, this is a cheat mod so spawn some anyway
                                        animalHouse.objects.Add(tile, hay);
                                    }
                                }
                            }
                            break;

                        case SlimeHutch slimeHutch:
                            for (int i = 0; i < slimeHutch.waterSpots.Length; i++)
                                slimeHutch.waterSpots[i] = true;
                            break;
                    }

                    return true;
                }
            );
        }
    }
}
