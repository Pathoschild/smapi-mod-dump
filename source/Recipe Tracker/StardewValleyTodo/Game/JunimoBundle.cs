/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NoxChimaera/StardewValleyTODO
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;

namespace StardewValleyTodo.Game {
    public class JunimoBundle {
        public int RoomId { get; }
        public string Name { get; }
        public int Slots { get; set; }
        public bool IsComplete { get; set; }
        public List<JunimoBundleIngredient> Ingredients { get; }

        public JunimoBundle(int roomId, string name, int slots) {
            RoomId = roomId;
            Name = name;
            Slots = slots;
            Ingredients = new List<JunimoBundleIngredient>();
        }

        /// <summary>
        /// Adds an ingredient into bundle description.
        /// </summary>
        /// <param name="ingredient">Ingredient</param>
        public void AddIngredient(JunimoBundleIngredient ingredient) {
            Ingredients.Add(ingredient);
        }

        public int CountEmptySlots() {
            return Math.Max(0, Slots - Ingredients.Count(x => x.IsDonated));
        }
    }
}
