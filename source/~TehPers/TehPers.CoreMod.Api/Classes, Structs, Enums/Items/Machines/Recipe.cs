using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewValley;
using TehPers.CoreMod.Api.Extensions;
using TehPers.CoreMod.Api.Structs;
using Object = StardewValley.Object;

namespace TehPers.CoreMod.Api.Items.Machines {
    public readonly struct Recipe {
        public IReadOnlyList<Ingredient> Input { get; }
        public IReadOnlyList<Ingredient> Output { get; }
        public STimeSpan Delay { get; }

        public Recipe(IEnumerable<Ingredient> input, IEnumerable<Ingredient> output, STimeSpan delay) {
            this.Input = input.ToImmutableArray();
            this.Output = output.ToImmutableArray();
            this.Delay = delay;
        }

        public Recipe AddIngredient(int index, int quantity) => this.AddIngredient(new Ingredient(() => index, quantity));
        public Recipe AddIngredient(Func<int> index, int quantity) => this.AddIngredient(new Ingredient(index, quantity));
        public Recipe AddIngredient(in Ingredient ingredient) {
            return new Recipe(this.Input.Append(ingredient), this.Output, this.Delay);
        }

        public Recipe AddResult(int index, int quantity) => this.AddResult(new Ingredient(() => index, quantity));
        public Recipe AddResult(Func<int> index, int quantity) => this.AddResult(new Ingredient(index, quantity));
        public Recipe AddResult(in Ingredient ingredient) {
            return new Recipe(this.Input, this.Output.Append(ingredient), this.Delay);
        }

        public Recipe SetDelay(STimeSpan delay) {
            return new Recipe(this.Input, this.Output, delay);
        }

        /// <summary>Generates the payload required for this recipe's input containing a request specifically for the held object if it is an ingredient in this recipe.</summary>
        /// <param name="heldObject">The <see cref="Object"/> being held by the farmer.</param>
        /// <param name="requests">The <see cref="ObjectRequest"/>s that must be fulfilled to satisfy this recipe's requrements. This will always contain the requested items, but will only contain a request for the held object if it is an ingredient in this recipe.</param>
        /// <returns>True if the held object is an ingredient, false otherwise.</returns>
        public bool GetHeldRequests(Object heldObject, out IEnumerable<ObjectRequest> requests) {
            // Check if one of the ingredients is currently being held
            Ingredient heldIngredient = this.Input.FirstOrDefault(i => i.Index() == heldObject.ParentSheetIndex);

            if (heldIngredient != null) {
                // Request the held ingredient
                requests = (from ingredient in this.Input
                            where ingredient != heldIngredient
                            select new ObjectRequest(new Object(Vector2.Zero, ingredient.Index(), ingredient.Quantity)))
                    .Prepend(new ObjectRequest(heldObject, heldIngredient.Quantity));
                return true;
            } else {
                requests = this.GetRequests();
                return false;
            }
        }

        /// <summary>Generates the payload required for this recipe's input.</summary>
        /// <returns>The <see cref="ObjectRequest"/>s that must be fulfilled to satisfy this recipe's requirements.</returns>
        public IEnumerable<ObjectRequest> GetRequests() {
            return this.Input.Select(ingredient => new ObjectRequest(new Object(Vector2.Zero, ingredient.Index(), ingredient.Quantity)));
        }

        public class Ingredient {
            /// <summary>The <see cref="Item.ParentSheetIndex"/> of the item.</summary>
            public Func<int> Index { get; }

            /// <summary>The quantity of the item.</summary>
            public int Quantity { get; }

            public Ingredient(Func<int> index, int quantity) {
                this.Index = index;
                this.Quantity = quantity;
            }
        }
    }
}
