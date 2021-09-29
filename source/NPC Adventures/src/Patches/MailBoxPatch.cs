/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/PurrplingMod
**
*************************************************/

using HarmonyLib;
using NpcAdventure.Events;
using PurrplingCore.Patching;
using StardewValley;
using System;
using System.Linq;

namespace NpcAdventure.Patches
{
    internal class MailBoxPatch : Patch<MailBoxPatch>
    {
        private SpecialModEvents Events { get; set; }
        public override string Name => nameof(MailBoxPatch);

        /// <summary>
        /// Creates instance of mailbox game patch
        /// </summary>
        /// <param name="events"></param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public MailBoxPatch(SpecialModEvents events)
        {
            this.Events = events ?? throw new ArgumentNullException(nameof(events));
            Instance = this;
        }

        /// <summary>
        /// This patches mailbox read method on gamelocation and allow call custom logic 
        /// for NPC Adventures mail letters only. For other mails call vanilla logic.
        /// </summary>
        /// <param name="__instance">Game location</param>
        /// <returns></returns>
        private static bool Before_mailbox(ref GameLocation __instance)
        {
            try
            {
                if (Game1.mailbox.Count <= 0)
                {
                    return true;
                }

                var letter = Game1.mailbox.First();

                if (letter != null && letter.StartsWith("npcAdventures."))
                {
                    string[] parsedLetter = letter.Split('.');
                    MailEventArgs args = new MailEventArgs
                    {
                        FullLetterKey = letter,
                        LetterKey = parsedLetter.Length > 1 ? parsedLetter[1] : null,
                        Mailbox = Game1.player.mailbox,
                        Player = Game1.player,
                    };

                    Instance.Events.FireMailOpen(__instance, args);

                    return false;
                }
            }
            catch (Exception ex)
            {
                Instance.LogFailure(ex, nameof(Before_mailbox));
            }

            return true;
        }

        protected override void Apply(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.mailbox)),
                prefix: new HarmonyMethod(typeof(MailBoxPatch), nameof(MailBoxPatch.Before_mailbox))
            );
        }
    }
}
