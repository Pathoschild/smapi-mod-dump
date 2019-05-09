using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using xTile.Tiles;

namespace SadisticBundles
{

    public class CommunityCenterManager
    {
        private readonly IModHelper Helper;
        private readonly IMonitor Monitor;
        private readonly BundleInjector bundler;

        public CommunityCenterManager(IModHelper helper, IMonitor monitor, BundleInjector bundler)
        {
            Helper = helper;
            Monitor = monitor;
            this.bundler = bundler;
            helper.Events.Player.Warped += (o, e) => Warped(e.NewLocation);
            helper.Events.Input.ButtonPressed += ButtonPressed;
        }

        private Point notePos = new Point(32, 15);

        private void ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            var cc = Game1.currentLocation as CommunityCenter;
            if (cc == null) return;
            if (!e.Button.IsActionButton()) return;

            var gt = e.Cursor.GrabTile;
            var p = Game1.MasterPlayer;

            // Master note
            if (isMasterNoteVisible(cc) && gt.X == notePos.X && gt.Y == notePos.Y)
            {
                Helper.Input.Suppress(e.Button);
                Func<string, Translation> t = Helper.Translation.Get;
                Func<string, string> rText = x => Game1.content.LoadString("Strings\\UI:JunimoNote_" + x);
                if (GameState.Current.Activated)
                {
                    if (p.mailForTomorrow.Any(x => x.StartsWith("cc")))
                    {
                        var db = new DialogueBox(t("noteRewardsAlreadyWorking"));
                        Game1.activeClickableMenu = db;
                    }
                    else
                    {
                        GameState.Current.LookingAtVanillaRewards = true;
                        ModEntry.InvalidateCache();
                        var responses = new List<MyResponse>();
                        if (!p.hasOrWillReceiveMail("ccPantry"))
                        {
                            var text = rText("RewardPantry") + " (70,000g)";
                            responses.Add(new MyResponse(text, () => buildUpgrade("ccPantry", 70000)));
                        }
                        if (!p.hasOrWillReceiveMail("ccCraftsRoom"))
                        {
                            var text = rText("RewardCrafts") + " (50,000g)";
                            responses.Add(new MyResponse(text, () => buildUpgrade("ccCraftsRoom", 50000)));
                        }
                        if (!p.hasOrWillReceiveMail("ccFishTank"))
                        {
                            var text = rText("RewardFishTank") + " (40,000g)";
                            responses.Add(new MyResponse(text, () => buildUpgrade("ccFishTank", 40000)));
                        }
                        if (!p.hasOrWillReceiveMail("ccBoilerRoom"))
                        {
                            var text = rText("RewardBoiler") + " (30,000g)";
                            responses.Add(new MyResponse(text, () => buildUpgrade("ccBoilerRoom", 30000)));
                        }
                        if (!p.hasOrWillReceiveMail("ccVault"))
                        {
                            var text = rText("RewardVault") + " (80,000g)";
                            responses.Add(new MyResponse(text, () => buildUpgrade("ccVault", 80000)));
                        }
                        GameState.Current.LookingAtVanillaRewards = false;
                        ModEntry.InvalidateCache();
                        var menu = new MyDialogueBox(t("noteRewards"), responses, Helper);
                        Game1.activeClickableMenu = menu;
                    }
                }
                else
                {
                    var menu = new MyDialogueBox(t("noteActivate"), new List<MyResponse> {
                        new MyResponse(t("noteActivateY"), () => ActivateMod(cc)),
                        new MyResponse(t("noteActivateN"), () => DeclineMod(cc))},
                       Helper);
                    Game1.activeClickableMenu = menu;
                }
                return;
            }

            // Bulletin Board
            var tile = cc.map.GetLayer("Buildings").Tiles[(int)gt.X, (int)gt.Y]?.TileIndex ?? -1;
            if (tile == 1799)
            {
                Helper.Input.Suppress(e.Button);
                Helper.Reflection.GetMethod(cc, "checkBundle").Invoke(5);
                return;
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

        private bool isMasterNoteVisible(CommunityCenter cc)
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
            // TODO: check for unpurchased upgrades
            return true;
        }

        private void ActivateMod(CommunityCenter cc)
        {
            GameState.Current.Activated = true;
            ModEntry.InvalidateCache();
            bundler.FixBundles(true);
            Warped(cc);
        }

        private void DeclineMod(CommunityCenter cc)
        {
            GameState.Current.Declined = true;
            Warped(cc);
        }

        private void Warped(GameLocation gl)
        {
            var cc = gl as CommunityCenter;
            if (cc == null) return;

            if (isMasterNoteVisible(cc))
            {
                var layer = cc.map.GetLayer("Buildings");
                var frames = new int[] { 1825, 1826, 1827, 1828, 1829, 1830, 1831, 1832, 1833, 1833, 1833, 1833, 1833, 1833, 1833, 1833, 1833, 1833, 1832, 1824 };
                StaticTile[] tileFrames = frames.Select(x => new StaticTile(layer, cc.map.TileSheets[0], BlendMode.Alpha, x)).ToArray();
                layer.Tiles[notePos.X, notePos.Y] = new AnimatedTile(layer, tileFrames, 70);
                Game1.currentLightSources.Add(new LightSource(4, new Vector2(notePos.X * 64, notePos.Y * 64), 1f));
            }
            else
            {
                var layer = cc.map.GetLayer("Buildings");
                layer.Tiles[notePos.X, notePos.Y] = null;
            }

            if (GameState.Current.Activated)
            {
                // remove event handlers that trigger junimo rewards for area completion.
                // you must buy that stuff.
                Helper.Reflection.GetField<NetEvent1Field<int, NetInt>>(cc, "areaCompleteRewardEvent").SetValue(new NetEvent1Field<int, NetInt>());  

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
    }
}
