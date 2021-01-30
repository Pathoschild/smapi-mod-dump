/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Stardew-Valley-Modding/Bookcase
**
*************************************************/

using Bookcase.Mail;
using Bookcase.Registration;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Reflection;

namespace Bookcase.Patches {

    public class PatchConstructor : IGamePatch {

        public Type TargetType => typeof(LetterViewerMenu);

        public MethodBase TargetMethod => TargetType.GetConstructor(new Type[] { typeof(string), typeof(string), typeof(bool)});

        public static void Prefix(ref string mail, string mailTitle, bool fromCollection) {

            // Convert mail ID to Bookcase's ID format. Ignoring validation warnings.
            Identifier id = new Identifier(mailTitle, false);

            // Check if letter being opened exists in the mail registry.
            if (Registries.Mail.HasKey(id)) {

                Letter currentLetter = Registries.Mail.Get(id);

                // If the letter actually exists, and has a pre processor, run it!
                if (currentLetter != null && currentLetter.PreProcessor != null) {

                    // Sets mail to results of the pre processor.
                    mail = currentLetter.PreProcessor.Invoke(currentLetter, mail);
                }
            }
        }

        public static void Postfix(LetterViewerMenu __instance, string mail, string mailTitle, bool fromCollection, ref int ___moneyIncluded, ref Texture2D ___letterTexture) {

            // Convert mail ID to Bookcase's ID format. Ignoring validation warnings.
            Identifier id = new Identifier(mailTitle, false);

            if (Registries.Mail.HasKey(id)) {

                Letter currentLetter = Registries.Mail.Get(id);

                if (currentLetter != null) {

                    // Set the background if the letter has one.
                    if (currentLetter.Background != null) {

                        ___letterTexture = currentLetter.Background;
                    }

                    // Add additional items if the letter has some.
                    currentLetter.Gifts.ForEach((Action<Item>)(i => __instance.itemsToGrab.Add(new ClickableComponent(new Rectangle(__instance.xPositionOnScreen + __instance.width / 2 - 48, __instance.yPositionOnScreen + __instance.height - 32 - 96, 96, 96), i.getOne()) {
                        myID = 104,
                        leftNeighborID = 101,
                        rightNeighborID = 102
                    })));

                    // Applys the current letter callback.
                    if (currentLetter.Callback != null) {

                        currentLetter.Callback.Invoke(currentLetter, __instance);
                    }
                }
            }
        }
    }
}