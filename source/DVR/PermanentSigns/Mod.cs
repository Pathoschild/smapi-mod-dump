using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PermanentSigns
{
    public interface IJsonAssetsApi
    {
        void LoadAssets(string path);
        int GetObjectId(string name);
    }
    public class ModEntry : Mod
    {
        private IJsonAssetsApi JsonAssets;
        private static int giftID;
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
            harmony.Patch(
               original: AccessTools.Method(typeof(NPC), nameof(NPC.checkAction)),
               prefix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.NPCCheckForAction_Prefix)),
               postfix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.NPCCheckForAction_Postfix))
            );
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.DayStarted += DayStarted;
            helper.Events.Display.Rendered += RenderBubbles;
            helper.Events.GameLoop.UpdateTicked += Tick;
        }

        private void Tick(object sender, UpdateTickedEventArgs e)
        {
            Bubbles.Tick();
        }

        private void RenderBubbles(object sender, RenderedEventArgs e)
        {
            Bubbles.Render(e.SpriteBatch);            
        }

        bool showBubble;

        private void DayStarted(object sender, DayStartedEventArgs e)
        {
            showBubble = true;
            giftID = JsonAssets.GetObjectId("Gift");
            ScanAllSigns();
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // load Json Assets API
            this.JsonAssets = this.Helper.ModRegistry.GetApi<IJsonAssetsApi>("spacechase0.JsonAssets");
            if (this.JsonAssets == null)
            {
                this.Monitor.Log("Can't access the Json Assets API. Is the mod installed correctly?", LogLevel.Error);
                return;
            }

            // inject Json Assets content pack
            this.JsonAssets.LoadAssets(Path.Combine(this.Helper.DirectoryPath, "assets"));
            giftID = JsonAssets.GetObjectId("Gift");

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
             "Stardrop",
        };

        static int idOfNPC(string s)
        {
            if (!npcs.Contains(s)) return 0;
            return -1 * (npcs.IndexOf(s) + 1);
        }

        private static IDictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();

        static void Draw_Postfix(Sign __instance, SpriteBatch spriteBatch, int x, int y, float alpha)
        {
            var val = __instance.displayType.Value;
            var idx = (val * -1) - 1;
            if (val >= 0 || idx >= npcs.Count) return;

            var name = npcs[idx];

            Texture2D portrait;

            if (textures.ContainsKey(name))
            {
                portrait = textures[name];
            }
            else
            {
                portrait = instance.Helper.Content.Load<Texture2D>($"Portraits/{name}", ContentSource.GameContent);
                textures[name] = portrait;
            }

            Vector2 position = Game1.GlobalToLocal(Game1.viewport, new Vector2((x * 64), (y * 64 - 64)));
            Rectangle destination = new Rectangle((int)position.X + 4, (int)position.Y + 26, 56, 56);

            var parentDepth = Math.Max(0f, (float)((y + 1) * 64 - 20) / 10000f) + (float)x * 1E-05f;
            spriteBatch.Draw(portrait, destination, new Rectangle(0, 0, 64, 64), Color.White, 0, Vector2.Zero, SpriteEffects.None, parentDepth);

        }

        static StardewValley.Object temp;
        static GiftToGive givingGift;

        static bool NPCCheckForAction_Prefix(NPC __instance, Farmer who, ref bool __result)
        {
            if (who.ActiveObject?.ParentSheetIndex == giftID)
            {
                temp = who.ActiveObject;
                var fd = who.friendshipData;
                if (fd.ContainsKey(__instance.Name) && fd[__instance.Name].GiftsToday != 0)
                {
                    who.ActiveObject = null;
                }
                else
                {
                    // ok, we gave somebody a gift, let's see what we got for them
                    givingGift = getGift(__instance);
                    if (givingGift == null)
                    {
                        // TODO: message here
                        return false;
                    }
                    var giveObj = new StardewValley.Object(givingGift.Obj.ParentSheetIndex, 1, quality: givingGift.Obj.Quality);
                    who.ActiveObject = giveObj;
                    Bubbles.Add(__instance, giveObj.ParentSheetIndex);
                }
            }
            return true;
        }

        static void NPCCheckForAction_Postfix(NPC __instance, Farmer who, bool __result)
        {
            if (temp != null)
            {
                if (givingGift != null && who.ActiveObject == null)
                {
                    // if it got consumed we remove from chest
                    removeFromChest(givingGift);
                }
                who.ActiveObject = temp;
                temp = null;
            }
        }
        static IDictionary<int, IList<GiftSignInfo>> allSigns = new Dictionary<int, IList<GiftSignInfo>>();
        static void removeFromChest(GiftToGive gg)
        {
            for (int i = 0; i < gg.Chest.items.Count; i++)
            {
                var obj = gg.Chest.items[i] as StardewValley.Object;
                if (obj != null && obj.ParentSheetIndex == gg.Obj.ParentSheetIndex && obj.Quality == gg.Obj.Quality)
                {
                    if (obj.Stack == 1)
                    {
                        gg.Chest.items.RemoveAt(i);
                    }
                    else
                    {
                        obj.Stack--;
                        gg.Chest.items[i] = obj;
                    }
                    break;
                }
            }
        }
        static void ScanAllSigns()
        {
            allSigns = new Dictionary<int, IList<GiftSignInfo>>();
            foreach (var loc in Game1.locations)
            {
                ScanLocation(loc);
            }
            foreach (var bld in Game1.getFarm().buildings)
            {
                ScanLocation(bld.indoors.Value);
            }
        }
        static void ScanLocation(GameLocation loc)
        {
            foreach (var obj in loc.Objects.Pairs)
            {
                var sign = obj.Value as Sign;
                if (sign != null && (displayId(sign) < 0 || displayId(sign) == giftID))
                {
                    addSign(loc, obj.Key, sign);
                }
            }
        }

        private static int displayId(Sign sign)
        {
            if (sign.displayType.Value < 0)
            {
                return sign.displayType.Value;
            }
            if (sign.displayType.Value == 1 && (sign.displayItem.Value as StardewValley.Object)?.ParentSheetIndex == giftID)
            {
                return giftID;
            }
            return 0;
        }

        static void addSign(GameLocation loc, Vector2 pos, Sign sign)
        {
            var id = displayId(sign);
            if (!allSigns.ContainsKey(id))
            {
                allSigns[id] = new List<GiftSignInfo>();
            }
            allSigns[id].Add(new GiftSignInfo
            {
                Location = loc,
                Tile = pos,
            });
        }

        static GiftToGive getGift(NPC npc)
        {
            foreach (var chest in getChests(npc.Name))
            {
                var gifts = chest.items
                    .Where(x => (x as StardewValley.Object) != null)
                    .Cast<StardewValley.Object>()
                    .Where(x => x.canBeGivenAsGift())
                    .OrderBy(x => npc.getGiftTasteForThisItem(x))
                    .ThenByDescending(x => x.Quality);
                if (gifts.Any())
                {
                    return new GiftToGive { Chest = chest, Obj = gifts.First() };
                }
            }
            return null;
        }

        static IEnumerable<Chest> getChests(string npc)
        {
            var list = new List<Chest>();
            var id = idOfNPC(npc);
            var signs = new List<GiftSignInfo>();
            if (allSigns.ContainsKey(id))
            {
                signs = signs.Union(allSigns[id]).ToList();
            }
            if (allSigns.ContainsKey(giftID))
            {
                signs = signs.Union(allSigns[giftID]).ToList();
            }
            foreach (var info in signs)
            {
                var pos = info.Tile;
                var sign = (info.Location.getObjectAtTile((int)pos.X, (int)pos.Y)) as Sign;
                var chest = (info.Location.getObjectAtTile((int)pos.X, (int)pos.Y + 1)) as Chest;
                if (chest != null)
                {
                    list.Add(chest);
                }
            }
            return list;
        }

        private class GiftSignInfo
        {
            public GameLocation Location;
            public Vector2 Tile;
        }

        private class GiftToGive
        {
            public Chest Chest;
            public StardewValley.Object Obj;
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

                    __instance.displayType.Value = idOfNPC(s);
                    Game1.activeClickableMenu = null;
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
