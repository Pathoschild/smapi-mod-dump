using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using Netcode;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using StardewValley.Buildings;
using System.Reflection.Emit;

namespace MTN.Patches.FarmPatch
{
    //[HarmonyPatch(typeof(Farm))]
    //[HarmonyPatch("draw")]
    public class drawPatch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            int i;
            var codes = new List<CodeInstruction>(instructions);


            for (i = 48; i < 215; i++)
            {
                codes[i].opcode = OpCodes.Nop;
            }

            for (i = 226; i < 249; i++)
            {
                codes[i].opcode = OpCodes.Nop;
            }

            return codes.AsEnumerable();
        }

        public static void Postfix(Farm __instance, SpriteBatch b)
        {
            int x;
            if (__instance.Name != "Farm") return;
            NetRectangle house = (NetRectangle)Traverse.Create(__instance).Field("houseSource").GetValue();
            NetRectangle greenhouse = (NetRectangle)Traverse.Create(__instance).Field("greenhouseSource").GetValue();

            //Farmhouse & Farmhouse Shadows 
            //For Custom
            if (Memory.isCustomFarmLoaded && Memory.isFarmHouseRelocated)
            {
                b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, Memory.loadedFarm.getFarmHouseRenderPosition(64, 568)), new Rectangle?(Building.leftShadow), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1E-05f);
                for (x = 2; x < 9; x++)
                {
                    b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, Memory.loadedFarm.getFarmHouseRenderPosition(x * 64f, 568)), new Rectangle?(Building.middleShadow), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1E-05f);
                }
                b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, Memory.loadedFarm.getFarmHouseRenderPosition(x * 64f, 568)), new Rectangle?(Building.rightShadow), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1E-05f);
                b.Draw(Farm.houseTextures, Game1.GlobalToLocal(Game1.viewport, Memory.loadedFarm.getFarmHouseRenderPosition()), new Rectangle?(house), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, ((Memory.loadedFarm.farmHousePorchY() - 5 + 3) * 64) / 10000f);
            }
            else
            //For Canon & Customs with Canon Position
            {
                b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(3776f, 1088f)), new Rectangle?(Building.leftShadow), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1E-05f);
                for (x = 1; x < 8; x++)
                {
                    b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(3776 + x * 64), 1088f)), new Rectangle?(Building.middleShadow), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1E-05f);
                }
                b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(4288f, 1088f)), new Rectangle?(Building.rightShadow), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1E-05f);
                b.Draw(Farm.houseTextures, Game1.GlobalToLocal(Game1.viewport, new Vector2(3712f, 520f)), new Rectangle?(house), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.075f);
            }

            //Actual Farmhouse & Green House
            //For Custom
            if (Memory.isCustomFarmLoaded && Memory.isGreenHouseRelocated)
            {
                b.Draw(Farm.houseTextures, Game1.GlobalToLocal(Game1.viewport, Memory.loadedFarm.getGreenHouseRenderPosition()), new Rectangle?(greenhouse), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, ((Memory.loadedFarm.greenHousePorchY() - 7 + 2) * 64f) / 10000f);
            }
            //For Canon & Customs with Canon Position
            else
            {
                b.Draw(Farm.houseTextures, Game1.GlobalToLocal(Game1.viewport, new Vector2(1600f, 384f)), new Rectangle?(greenhouse), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.0704f);
            }


            //Mailbox "!" when new mail arrived.
            if (Game1.mailbox.Count > 0)
            {
                float yOffset = 4f * (float)Math.Round(Math.Sin(DateTime.Now.TimeOfDay.TotalMilliseconds / 250.0), 2);
                
                //For Custom
                if (Memory.isCustomFarmLoaded && Memory.isMailBoxRelocated)
                {
                    b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, Memory.loadedFarm.getMailBoxNotificationRenderPosition(0, (2.25f * -64f) + yOffset)), new Rectangle?(new Rectangle(141, 465, 20, 24)), Color.White * 0.75f, 0f, Vector2.Zero, 4f, SpriteEffects.None, (((Memory.loadedFarm.mailBoxPointY() + 2) * 64f) / 10000f) + 0.000401f);
                    b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, Memory.loadedFarm.getMailBoxNotificationRenderPosition(0.5626f * 64f, (1.5f * -64f) + yOffset)), new Rectangle?(new Rectangle(189, 423, 15, 13)), Color.White, 0f, new Vector2(7f, 6f), 4f, SpriteEffects.None, (((Memory.loadedFarm.mailBoxPointY() + 2) * 64f) / 10000f) + 0.00041f);
                }
                else
                //For Canon & Custom with Canon Position
                {
                    b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(4352f, 880f + yOffset)), new Rectangle?(new Rectangle(141, 465, 20, 24)), Color.White * 0.75f, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.115601f);
                    b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(4388f, 928f + yOffset)), new Rectangle?(new Rectangle(189, 423, 15, 13)), Color.White, 0f, new Vector2(7f, 6f), 4f, SpriteEffects.None, 0.11561f);
                }
            }

            //Shrine note
            if (!__instance.hasSeenGrandpaNote)
            {
                //For Custom 
                if (Memory.isCustomFarmLoaded && Memory.isShrineRelocated)
                {
                    b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, Memory.loadedFarm.getgrandpaShrineRenderPosition()), new Rectangle?(new Rectangle(575, 1972, 11, 8)), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.0448009968f);
                }
                //For Canon & Custom with Canon Position
                else
                {
                    b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(576f, 448f)), new Rectangle?(new Rectangle(575, 1972, 11, 8)), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.0448009968f);
                }
            }
        }
    }
}
