using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using xTile.Tiles;

namespace HardcoreBundles
{
    public class CommunityCenterManager
    {
        private readonly IModHelper helper;
        private readonly IMonitor monitor;

        public CommunityCenterManager(IModHelper helper, IMonitor monitor)
        {
            this.helper = helper;
            this.monitor = monitor;
            helper.Events.Player.Warped += (o,e) => Warped(e.NewLocation);

        }

        private Point bookPos = new Point(32, 15);
        private Point bookPos2 = new Point(32, 14);

        private void Warped(GameLocation loc)
        {
            var cc = loc as CommunityCenter;
            if (cc == null)
            {
                helper.Events.Input.ButtonPressed -= ButtonPressed;
                return;
            }

            helper.Events.Input.ButtonPressed += ButtonPressed;
            var bldgs = cc.map.GetLayer("Buildings");
            var front = cc.map.GetLayer("Front");
            if (isMasterBookVisible(cc))
            {
                bldgs.Tiles[bookPos.X, bookPos.Y] = new StaticTile(bldgs, cc.map.TileSheets[0], BlendMode.Alpha, 2175);

                front.Tiles[bookPos2.X, bookPos2.Y] = new StaticTile(front, cc.map.TileSheets[0], BlendMode.Alpha, 2143);
                Game1.currentLightSources.Add(new LightSource(4, new Vector2(bookPos2.X * 64, bookPos2.Y - 1 * 64), 1f));
            }
            else
            {
                bldgs.Tiles[bookPos.X, bookPos.Y] = null;
                front.Tiles[bookPos2.X, bookPos2.Y] = null;
            }

            if (GameState.Current.Activated)
            {
                // remove event handlers that trigger junimo rewards for area completion.
                // you must buy that stuff.
                helper.Reflection.GetField<NetEvent1Field<int, NetInt>>(cc, "areaCompleteRewardEvent").SetValue(new NetEvent1Field<int, NetInt>());

                // activate all areas from the start.
                for (int i = 0; i < cc.areasComplete.Count; i++)
                {
                    if (!cc.isJunimoNoteAtArea(i) && !cc.areasComplete[i])
                    {
                        cc.addJunimoNote(i);
                        // for animated version:
                        //cc.addJunimoNoteViewportTarget(i);
                    }
                }
            }
        }

        private void ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            var cc = Game1.currentLocation as CommunityCenter;
            if (cc == null) return;
            if (!e.Button.IsActionButton()) return;

            var gt = e.Cursor.GrabTile;
            var p = Game1.MasterPlayer;

            // Master book
            if (isMasterBookVisible(cc) && (cc.map.GetLayer("Front").Tiles[(int)gt.X, (int)gt.Y]?.TileIndex == 2143 || cc.map.GetLayer("Buildings").Tiles[(int)gt.X, (int)gt.Y]?.TileIndex == 2175))
            {
                helper.Input.Suppress(e.Button);
                handleMasterBook(cc);
            }

            var tile = cc.map.GetLayer("Buildings").Tiles[(int)gt.X, (int)gt.Y]?.TileIndex ?? -1;
            if (tile == 1799)
            {
                helper.Input.Suppress(e.Button);
                helper.Reflection.GetMethod(cc, "checkBundle").Invoke(5);
                return;
            }
        }

        private void handleMasterBook(CommunityCenter cc)
        {
            helper.Content.Load<Dictionary<string,string>>("Strings/UI", ContentSource.GameContent);
            Func<string, Translation> t = helper.Translation.Get;
            if (!GameState.Current.Activated)
            {
                // initial choice to use mod or dimiss it forever.
                var menu = new MyDialogueBox(t("noteActivate"), new List<MyResponse> {
                        new MyResponse(t("noteActivateY"), () => ActivateMod(cc)),
                        new MyResponse(t("noteActivateN"), () => DeclineMod(cc))},
                       helper);
                Game1.activeClickableMenu = menu;

            }
            else
            {
                // are we already working on a reward?
                if (Game1.MasterPlayer.mailForTomorrow.Any(x => x.StartsWith("cc")))
                {
                    var db = new DialogueBox(t("noteRewardsAlreadyWorking"));
                    Game1.activeClickableMenu = db;
                }
                else
                {
                    var responses = new List<MyResponse>();
                    var rooms = new Dictionary<string, int>
                    {
                        {"Pantry", 70000},
                        {"CraftsRoom", 50000 },
                        {"Vault", 80000 },
                        {"FishTank", 40000 },
                        {"BoilerRoom", 30000 },
                    };
                    foreach(var kvp in rooms)
                    {
                        if (!Game1.MasterPlayer.hasOrWillReceiveMail($"cc{kvp.Key}"))
                        {
                            var text = ModEntry.VanillaRewards[kvp.Key] + $" ({kvp.Value:n0}g)";
                            responses.Add(new MyResponse(text, () => buildUpgrade($"cc{kvp.Key}", kvp.Value)));
                        }
                    }
                   
                    var menu = new MyDialogueBox(t("noteRewards"), responses, helper);
                    Game1.activeClickableMenu = menu;
                }
            }
        }

        private void buildUpgrade(string name, int cost)
        {
            var p = Game1.MasterPlayer;
            if (p.money < cost)
            {
                var db = new DialogueBox(Game1.content.LoadString("Strings\\UI:NotEnoughMoney3"));
                Game1.activeClickableMenu = db;
                return;
            }
            p.money -= cost;
            p.mailForTomorrow.Add(name + "%&NL&%");
        }

        private void ActivateMod(CommunityCenter cc)
        {
            GameState.Current.Activated = true;
            ModEntry.Instance.Invalidate();
            Bundles.Fix(true);
            Warped(cc);
        }

        private void DeclineMod(CommunityCenter cc)
        {
            GameState.Current.Declined = true;
            Warped(cc);
        }

        private bool isMasterBookVisible(CommunityCenter cc)
        {
            // do nothing if cc is finished or not yet seen wizard to get skills.
            if (Game1.MasterPlayer.mailReceived.Contains("JojaMember") || cc.areAllAreasComplete() || !Game1.MasterPlayer.hasOrWillReceiveMail("canReadJunimoText"))
            {
                return false;
            }
            // They can opt-out entirely.
            if (GameState.Current.Declined) { return false; }
            // if finished any full rooms in vanilla, they can't play hard mode anymore
            if (!GameState.Current.Activated && cc.areasComplete.Any(x => x)) { return false; }
            // TODO: If they have finished buying from the menu, don't show it anymore
            return true;
        }
    }
}
