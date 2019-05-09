using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using System;
using System.Collections.Generic;

namespace PermanentSigns
{
    public class ModEntry : Mod
    {
        static ModEntry instance;
        public override void Entry(IModHelper helper)
        {
            instance = this;
            var harmony = HarmonyInstance.Create("captncraig.stardew.mod.signs");
            harmony.Patch(
                original: AccessTools.Method(typeof(Sign), nameof(Sign.draw), new Type[] { typeof(SpriteBatch), typeof(int), typeof(int), typeof(float) }),
                postfix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.Draw_Postfix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(Sign), nameof(Sign.checkForAction)),
                prefix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.CheckForAction_Prefix))
            );
        }

        static List<string> npcs = new List<string>
        {
            // ORDER MATTERS. Internal ids on sign are derived from order here. From -1 downward.
            "Abigail",
            "Emily",
            "Haley",
            "Leah",
            "Maru",
            "Penny",

            "Alex",
            "Elliott",
            "Harvey",
             "Sam",
             "Sebastian",
             "Shane",

             "Caroline",
             "Clint",
             "Demetrius",
             "Dwarf",
             "Evelyn",
             "George",
             "Gus",

             "Jas",
             "Jodi",
             "Kent",
             "Krobus",
             "Lewis",
             "Linus",
             "Marnie",

             "Pam",
             "Pierre",
             "Robin",
             "Sandy",
             "Vincent",
             "Willy",
             "Wizard",

             "Bouncer",
             "Gil",
             "Governor",
             "Grandpa",
             "Gunther",
             "Henchman",
             "Marlon",
             "Morris",
             "MrQi",
        };

        static int idOfNPC(string s)
        {
            if (!npcs.Contains(s)) return 0;
            return -1 * (npcs.IndexOf(s)+1);
        }

        static void Draw_Postfix(Sign __instance, SpriteBatch spriteBatch, int x, int y, float alpha)
        {
            var val = __instance.displayType.Value;
            var idx = (val * -1) - 1;
            if (val >= 0 || idx >= npcs.Count) return;

            var name = npcs[idx];

            var portrait = instance.Helper.Content.Load<Texture2D>($"Portraits/{name}", ContentSource.GameContent);

            Vector2 position = Game1.GlobalToLocal(Game1.viewport, new Vector2((x * 64), (y * 64 - 64)));
            Rectangle destination = new Rectangle((int)position.X + 4, (int)position.Y + 26, 56, 56);

            var parentDepth = Math.Max(0f, (float)((y + 1) * 64 - 20) / 10000f) + (float)x * 1E-05f;
            spriteBatch.Draw(portrait, destination, new Rectangle(0, 0, 64, 64), Color.White, 0, Vector2.Zero, SpriteEffects.None, parentDepth);

        }

        static bool CheckForAction_Prefix(Sign __instance, Farmer who, bool justCheckingForActivity, ref bool __result)
        {
            // Pass through the passive checks
            if (justCheckingForActivity)
            {
                return true;
            }
            // if the sign has anything on it, never allow changes
            if (__instance.displayType.Value != 0 || __instance.displayItem.Value != null)
            {
                return false;
            }
            // if clicking with empty hands, allow more options
            if (who.CurrentItem == null)
            {
                var menu = new ChooseFromListMenu(npcs, (s) =>
                {
                    instance.Monitor.Log($"!!!!!!{s}");
                    __instance.displayType.Value = idOfNPC(s);
                }, false);
                Game1.activeClickableMenu = menu;
                __result = true;
                return false;
            }
            // otherwise pass through
            return true;
        }

    }

}
