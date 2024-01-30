/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jamespfluger/Stardew-MisophoniaAccessibility
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MisophoniaAccessibility.UserInterface;

namespace MisophoniaAccessibility.Config
{
    public class Sounds
    {
        [GameSound(codeName: "eat", displayName: "Disable eat sound")]
        public bool DisableEatSound { get; set; } = true;

        [GameSound(codeName: "gulp", displayName: "Disable drink sound")]
        public bool DisableDrinkSound { get; set; } = false;

        [GameSound(codeName: "bubbles", displayName: "Disable keg sound")]
        public bool DisableBubblesSound { get; set; } = false;
    }
}
