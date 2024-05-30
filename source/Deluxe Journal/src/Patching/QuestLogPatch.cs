/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace DeluxeJournal.Patching
{
    /// <summary>Patches for <see cref="QuestLog"/>.</summary>
    internal class QuestLogPatch : PatchBase<QuestLogPatch>
    {
        public QuestLogPatch(IMonitor monitor) : base(monitor)
        {
            Instance = this;
        }

        private static bool Prefix_draw(QuestLog __instance, SpriteBatch b)
        {
            try
            {
                // !!! DO NOT DRAW THIS QUESTLOG AS THE ACTIVE MENU !!!
                // ----------------------------------------------------
                // 1) Prevents handling jittery frames being drawn while giving other mods a chance to replace the QuestLog.
                // 2) No logic should be done within draw(), so this SHOULD NOT impact a modded QuestLog.
                // 3) We only want to draw the QuestLog from within the QuestLogPage anyway.
                if (Game1.activeClickableMenu == __instance)
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Instance.LogError(ex, nameof(Prefix_draw));
            }

            return true;
        }

        public override void Apply(Harmony harmony)
        {
            Patch(harmony,
                original: AccessTools.Method(typeof(QuestLog), nameof(QuestLog.draw), new[] { typeof(SpriteBatch) }),
                prefix: new HarmonyMethod(typeof(QuestLogPatch), nameof(QuestLogPatch.Prefix_draw))
            );
        }
    }
}
