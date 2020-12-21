/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Gathouria/Adopt-Skin
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using StardewValley;
using StardewValley.Characters;

namespace AdoptSkin.Framework
{

    class Stray
    {
        /// <summary>RNG for selecting randomized aspects</summary>
        private readonly Random Randomizer = new Random();

        /// <summary>Structures constructors for pet types</summary>
        internal static Dictionary<Type, Func<Pet>> PetConstructors = new Dictionary<Type, Func<Pet>>
        {
            { typeof(Dog), () => new Dog() },
            { typeof(Cat), () => new Cat() }
        };

        /// <summary>The GameLocation for Marnie's house</summary>
        internal static readonly GameLocation Marnies = Game1.getLocationFromName("AnimalShop");
        /// <summary>Warp location for a potential pet at the beginning of the day</summary>
        internal static Vector2 CreationLocation = new Vector2(16, 16);

        /// <summary>The identifying number for a potential pet</summary>
        internal static readonly int StrayID = 8000;

        internal Pet PetInstance;
        internal string PetType;
        internal int SkinID;


        /// <summary>Creates a new Stray</summary>
        internal Stray()
        {
            // Create Stray traits
            PetType = ModEntry.PetTypeMap.Keys.ToList()[Randomizer.Next(0, ModEntry.PetTypeMap.Count)];
            SkinID = ModEntry.GetRandomSkin(PetType);

            // Create Pet instance
            PetInstance = PetConstructors[ModEntry.PetTypeMap[PetType]]();
            PetInstance.Manners = StrayID;
            PetInstance.Name = "Stray";
            PetInstance.displayName = "Stray";
            PetInstance.farmerPassesThrough = true;

            int[] info = ModApi.GetSpriteInfo(PetInstance);
            if (SkinID != 0)
                PetInstance.Sprite = new AnimatedSprite(ModEntry.GetSkin(PetType, SkinID).AssetKey, info[0], info[1], info[2]);

            // Put that thing where it belongs
            PetInstance.currentLocation = Marnies;
            Game1.warpCharacter(PetInstance, Marnies, CreationLocation);

            if (ModEntry.Config.NotifyStraySpawn)
            {
                string message = $"A stray pet is available to adopt at Marnie's!";
                ModEntry.SMonitor.Log(message, LogLevel.Debug);
                Game1.chatBox.addInfoMessage(message);
            }
        }


        /// <summary>Remove this Stray's Pet instance from its map</summary>
        internal void RemoveFromWorld()
        {
            Game1.removeThisCharacterFromAllLocations(this.PetInstance);
        }
    }
}
