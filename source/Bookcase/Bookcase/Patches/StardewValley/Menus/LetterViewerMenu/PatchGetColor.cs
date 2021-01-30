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
using StardewValley.Menus;
using System;
using System.Reflection;

namespace Bookcase.Patches {

    class PatchGetColor : IGamePatch {

        public Type TargetType => typeof(LetterViewerMenu);

        public MethodBase TargetMethod => TargetType.GetMethod("getTextColor");

        public static bool Prefix(LetterViewerMenu __instance, ref int __result) {

            // Read mail ID from the gui field using reflection.
            String mailTitle = BookcaseMod.reflection.GetField<String>(__instance, "mailTitle").GetValue();

            if (mailTitle != null) {

                // Convert mail ID to Bookcase's ID format. Ignoring validation warnings.
                Identifier id = new Identifier(mailTitle, false);

                if (Registries.Mail.HasKey(id)) {

                    Letter currentLetter = Registries.Mail.Get(id);

                    // If letter exists, and has a text color, modify the returned value.
                    if (currentLetter != null && currentLetter.TextColor != -1) {

                        __result = currentLetter.TextColor;
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
