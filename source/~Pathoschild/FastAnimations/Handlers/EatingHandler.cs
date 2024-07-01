/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pathoschild.Stardew.Common;
using Pathoschild.Stardew.FastAnimations.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace Pathoschild.Stardew.FastAnimations.Handlers
{
    /// <summary>Handles the eating animation.</summary>
    /// <remarks>See game logic in <see cref="Game1.pressActionButton"/> (opens confirmation dialogue), <see cref="Farmer.showEatingItem"/> (main animation logic), and <see cref="FarmerSprite"/>'s private <c>animateOnce(GameTime)</c> method (runs animation + some logic).</remarks>
    internal sealed class EatingHandler : BaseAnimationHandler
    {
        /*********
        ** Fields
        *********/
        /// <summary>The temporary animations showing the item thrown into the air.</summary>
        private readonly HashSet<TemporaryAnimatedSprite> ItemAnimations = [];

        /// <summary>Whether to disable the confirmation dialogue before eating or drinking.</summary>
        private readonly bool DisableConfirmation;


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="multiplier">The animation speed multiplier to apply.</param>
        /// <param name="disableConfirmation">Whether to disable the confirmation dialogue before eating or drinking.</param>
        public EatingHandler(float multiplier, bool disableConfirmation)
            : base(multiplier)
        {
            this.DisableConfirmation = disableConfirmation;
        }

        /// <inheritdoc />
        public override bool TryApply(int playerAnimationId)
        {
            // disable confirmation
            bool skippedConfirmation = false;
            if (this.DisableConfirmation && this.IsConfirmationShown(out DialogueBox? eatMenu))
            {
                // When the animation starts, the game shows a yes/no dialogue asking the player to
                // confirm they really want to eat the item. This code answers 'yes' and closes the
                // dialogue.
                Response yes = eatMenu.responses[0];
                Game1.currentLocation.answerDialogue(yes);
                eatMenu.closeDialogue();

                skippedConfirmation = true;
            }

            // speed up animation
            if (this.Multiplier > 1 && Context.IsWorldReady && Game1.player.isEating && Game1.player.Sprite.CurrentAnimation != null)
            {
                // The farmer eating animation spins off two main temporary animations: the item being
                // held (at index 1) and the item being thrown into the air (at index 2). The drinking
                // animation only has one temporary animation (at index 1). This code runs after each
                // one is spawned, and adds it to the list of temporary animations to handle.
                int indexInAnimation = Game1.player.FarmerSprite.currentAnimationIndex;
                if (indexInAnimation <= 1)
                    this.ItemAnimations.Clear();
                if ((indexInAnimation == 1 || (indexInAnimation == 2 && playerAnimationId == FarmerSprite.eat)) && Game1.player.itemToEat is Object obj && obj.QualifiedItemId != Object.stardropQID)
                {
                    var data = ItemRegistry.GetDataOrErrorItem(Game1.player.itemToEat.QualifiedItemId);
                    Texture2D texture = data.GetTexture();
                    Rectangle sourceRect = data.GetSourceRect();
                    TemporaryAnimatedSprite? tempAnimation = Game1.player.currentLocation.TemporarySprites.LastOrDefault(p => p.Texture == texture && p.sourceRect == sourceRect);
                    if (tempAnimation != null)
                        this.ItemAnimations.Add(tempAnimation);
                }

                // speed up animations
                GameTime gameTime = Game1.currentGameTime;
                GameLocation location = Game1.player.currentLocation;
                return this.ApplySkips(() =>
                {
                    // temporary item animations
                    foreach (TemporaryAnimatedSprite animation in this.ItemAnimations.ToArray())
                    {
                        bool animationDone = animation.update(gameTime);
                        if (animationDone)
                        {
                            this.ItemAnimations.Remove(animation);
                            location.TemporarySprites.Remove(animation);
                        }
                    }

                    // eating animation
                    Game1.player.Update(gameTime, location);
                });
            }

            return skippedConfirmation;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Get whether the eat/drink confirmation is being shown.</summary>
        /// <param name="menu">The confirmation menu.</param>
        private bool IsConfirmationShown([NotNullWhen(true)] out DialogueBox? menu)
        {
            if (Game1.player.itemToEat != null && Game1.activeClickableMenu is DialogueBox dialogue)
            {
                string? actualLine = dialogue.dialogues.FirstOrDefault();
                bool isConfirmation =
                    actualLine == GameI18n.GetString("Strings\\StringsFromCSFiles:Game1.cs.3159", Game1.player.itemToEat.DisplayName) // drink
                    || actualLine == GameI18n.GetString("Strings\\StringsFromCSFiles:Game1.cs.3160", Game1.player.itemToEat.DisplayName); // eat

                if (isConfirmation)
                {
                    menu = dialogue;
                    return true;
                }
            }

            menu = null;
            return false;
        }
    }
}
