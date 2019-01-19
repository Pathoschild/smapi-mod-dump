using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley;
using StardewValley.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace MTN2.Patches.FarmPatches
{
    /// <summary>
    /// REASON FOR PATCHING: The draw function of the class contains hardcode values
    /// pertaining where things should be rendered. This must be altered to allow
    /// flexibility and customization.
    /// 
    /// 
    /// Patches the method Farm.draw to adjust for the movement
    /// of various structures that were previously static in nature.
    /// (Farmhouse, Greenhouse, Mailbox, Grandpa Shrine)
    /// </summary>
    public class drawPatch {
        private static ICustomManager customManager;

        /// <summary>
        /// Constructor. Awkward method of setting references needed. However, Harmony patches
        /// are required to be static. Thus we must break good Object Orientated practices.
        /// </summary>
        /// <param name="CustomManager">The class controlling information pertaining to the customs (and the loaded customs).</param>
        public drawPatch(ICustomManager customManager) {
            drawPatch.customManager = customManager;
        }

        /// <summary>
        /// Transpiles the CLI to remove operations pertaining to the drawing of the Farmhouse, Greenhouse, Mailbox,
        /// and the note on the grandpa shrine. 
        /// </summary>
        /// <param name="instructions">Code Instructions (in CLI)</param>
        /// <returns></returns>
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
            int i;
            var codes = new List<CodeInstruction>(instructions);

            // TO-DO: Refactor. Simply remove the code instead of replacing with Nops.
            for (i = 48; i < 215; i++) {
                codes[i].opcode = OpCodes.Nop;
            }

            for (i = 226; i < 249; i++) {
                codes[i].opcode = OpCodes.Nop;
            }

            return codes.AsEnumerable();
        }

        /// <summary>
        /// Reimplements the operations pertaining to the drawing of the Farmhouse, Greenhouse, Mailbox, and
        /// the note on the grandpa shrine to accomidate for repositioning based on the information provided by
        /// the custom farm's content pack.
        /// </summary>
        /// <param name="__instance">The instance of the Farm</param>
        /// <param name="b">From original method. The spritebatch used to draw.</param>
        public static void Postfix(Farm __instance, SpriteBatch b) {
            int x;
            if (__instance.Name != "Farm") return;
            NetRectangle house = (NetRectangle)Traverse.Create(__instance).Field("houseSource").GetValue();
            NetRectangle greenhouse = (NetRectangle)Traverse.Create(__instance).Field("greenhouseSource").GetValue();

            //Farmhouse & Farmhouse Shadows 
            b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, customManager.FarmHouseCoords(64f, 568f)), new Rectangle?(Building.leftShadow), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1E-05f);
            for (x = 2; x < 9; x++) {
                b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, customManager.FarmHouseCoords(x * 64f, 568f)), new Rectangle?(Building.middleShadow), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1E-05f);
            }
            b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, customManager.FarmHouseCoords(x * 64, 568f)), new Rectangle?(Building.rightShadow), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1E-05f);
            b.Draw(Farm.houseTextures, Game1.GlobalToLocal(Game1.viewport, customManager.FarmHouseCoords()), new Rectangle?(house), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, customManager.FarmHouseLayerDepth());

            //Green House
            b.Draw(Farm.houseTextures, Game1.GlobalToLocal(Game1.viewport, customManager.GreenHouseCoords()), new Rectangle?(greenhouse), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, customManager.GreenHouseLayerDepth());

            //Mailbox Notification ("!" symbol when new mail arrives).
            if (Game1.mailbox.Count > 0) {
                float yOffset = 4f * (float)Math.Round(Math.Sin(DateTime.Now.TimeOfDay.TotalMilliseconds / 250.0), 2);
                b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, customManager.MailboxNotification(0f, (2.25f * -64f) + yOffset, false)), new Rectangle?(new Rectangle(141, 465, 20, 24)), Color.White * 0.75f, 0f, Vector2.Zero, 4f, SpriteEffects.None, customManager.MailBoxNotifyLayerDepth(false));
                b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, customManager.MailboxNotification(0.5626f * 64f, (1.5f * -64f) + yOffset, true)), new Rectangle?(new Rectangle(189, 423, 15, 13)), Color.White, 0f, new Vector2(7f, 6f), 4f, SpriteEffects.None, customManager.MailBoxNotifyLayerDepth(true));
            }

            //Shrine note
            if (!__instance.hasSeenGrandpaNote) {
               b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, customManager.GrandpaShrineCoords()), new Rectangle?(new Rectangle(575, 1972, 11, 8)), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.0448009968f);
            }
            return;
        }
    }
}
