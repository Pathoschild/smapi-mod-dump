/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/maxvollmer/DeepWoodsMod
**
*************************************************/


using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using xTile.Tiles;
using static DeepWoodsMod.DeepWoodsSettings;

namespace DeepWoodsMod.Stuff
{
    public class DeepWoodsMineCart
    {
        public static Vector2 MineCartLocation => new Vector2(Settings.Map.RootLevelEnterLocation.X - Settings.Map.ExitRadius - 2 - 6, Settings.Map.RootLevelEnterLocation.Y + 5);

        public static void AddDeepWoodsMineCart(DeepWoods location)
        {
            AddMinecartTile(location, MineCartLocation, 0, -1, 933);    // top of minecart
            AddMinecartTile(location, MineCartLocation, 0, 0, 958);     // bottom of minecart
            AddMinecartTile(location, MineCartLocation, 1, 0, 1080);    // left of minecart controls
            AddMinecartTile(location, MineCartLocation, 2, 0, 1081);    // right of minecart controls

            // TODO: Steam
            //AddSteamTile(location, tileLocation, 2, 0, 1081);       // right of minecart controls

            for (int i = 0; i < MineCartLocation.Y; i++)
            {
                AddTrackTile(location, MineCartLocation, 0, -i, 1076);  // track
            }
            AddTrackTile(location, MineCartLocation, 0, 1, 1075);       // end of track

            // If MinecartPatcher is installed, ensure this minecart is loaded
            // (we then inject it in DeepWoodsContentAdder)
            if (ModEntry.GetHelper().ModRegistry.IsLoaded("mod.kitchen.minecartpatcher"))
            {
                var mod = ModEntry.GetHelper().ModRegistry.Get("mod.kitchen.minecartpatcher");
                if (mod != null)
                {
                    // mod is actually of type ModMetadata
                    var modField = mod.GetType()?.GetField("Mod", BindingFlags.Instance | BindingFlags.Public);
                    if (modField != null)
                    {
                        var modEntry = modField.GetValue(mod);
                        if (modEntry != null)
                        {
                            var loadDataMethod = modEntry.GetType()?.GetMethod("LoadData", BindingFlags.Instance | BindingFlags.Public);
                            loadDataMethod?.Invoke(modEntry, null);
                        }
                    }
                }
            }
        }

        private static Tile GetOrCreateTile(DeepWoods location, string layerName, int tileX, int tileY, int tileIndex)
        {
            var layer = location.map.GetLayer(layerName);
            var tile = layer.Tiles[tileX, tileY];
            if (tile == null)
            {
                tile = new StaticTile(layer, location.map.GetTileSheet(DeepWoodsGlobals.DEFAULT_OUTDOOR_TILESHEET_ID), BlendMode.Alpha, tileIndex);
                layer.Tiles[tileX, tileY] = tile;
            }
            return tile;
        }

        private static void AddTrackTile(DeepWoods location, Vector2 tileLocation, int x, int y, int tileIndex)
        {
            int tileX = (int)(tileLocation.X + x);
            int tileY = (int)(tileLocation.Y + y);
            var tile = GetOrCreateTile(location, "Back", tileX, tileY, tileIndex);
            tile.TileIndex = tileIndex;
        }

        private static void AddMinecartTile(DeepWoods location, Vector2 tileLocation, int x, int y, int tileIndex)
        {
            int tileX = (int)(tileLocation.X + x);
            int tileY = (int)(tileLocation.Y + y);
            var tile = GetOrCreateTile(location, "Buildings", tileX, tileY, tileIndex);
            tile.Properties.Remove("Action");
            tile.Properties.Add("Action", "MinecartTransport");
            tile.TileIndex = tileIndex;
        }

        public static void InjectDeepWoodsMineCartIntoGame()
        {
            if (Game1.MasterPlayer.mailReceived.Contains("ccBoilerRoom")
                && DeepWoodsState.LowestLevelReached >= 1
                && Game1.dialogueUp
                && Game1.currentLocation.lastQuestionKey == "Minecart"
                && Game1.activeClickableMenu is DialogueBox dialogBox
                && dialogBox is not PatchedDialogueBox
                && IsMineCartOptions(dialogBox.responses))
            {
                if (Game1.currentLocation is DeepWoods)
                {
                    Game1.activeClickableMenu = new PatchedDialogueBox(dialogBox, "Mines", Game1.content.LoadString("Strings\\Locations:MineCart_Destination_Mines"));
                }
                else
                {
                    Game1.activeClickableMenu = new PatchedDialogueBox(dialogBox, "DeepWoods", I18N.DeepWoodsMineCartText);
                }
            }
        }

        private static bool IsMineCartOptions(List<Response> responses)
        {
            if (responses.Count < 3)
            {
                return false;
            }

            bool canHasQuarry = Game1.MasterPlayer.mailReceived.Contains("ccCraftsRoom");

            bool hasMines = responses.Where(r => r.responseKey == "Mines").Any();
            bool hasTown = responses.Where(r => r.responseKey == "Town").Any();
            bool hasBus = responses.Where(r => r.responseKey == "Bus").Any();

            if (canHasQuarry)
            {
                bool hasQuarry = responses.Where(r => r.responseKey == "Quarry").Any();
                int count = CountTrue(hasMines, hasTown, hasBus, hasQuarry);
                if (count < 3)
                {
                    return false;
                }
            }
            else
            {
                int count = CountTrue(hasMines, hasTown, hasBus);
                if (count < 2)
                {
                    return false;
                }
            }

            return true;
        }

        private static int CountTrue(params bool[] args)
        {
            return args.Count(t => t);
        }

        private class PatchedDialogueBox : DialogueBox
        {
            public PatchedDialogueBox(DialogueBox original, string newOptionKey, string newOptionText)
                : base(original.dialogues[0], InsertResponse(original.responses.ToList(), new Response(newOptionKey, newOptionText)))
            {
            }

            private static List<Response> InsertResponse(List<Response> responses, Response newResponse)
            {
                responses.Insert(responses.Count - 1, newResponse);
                return responses;
            }

            public override void receiveLeftClick(int x, int y, bool playSound = true)
            {
                if (transitioning)
                {
                    return;
                }

                if (safetyTimer > 0)
                {
                    return;
                }

                if (selectedResponse < 0 ||selectedResponse >= responses.Count)
                {
                    return;
                }

                if (responses[selectedResponse].responseKey != "DeepWoods")
                {
                    base.receiveLeftClick(x, y, playSound);
                    return;
                }

                questionFinishPauseTimer = (Game1.eventUp ? 600 : 200);
                transitioning = true;
                transitionInitialized = false;
                transitioningBigger = true;
                Game1.dialogueUp = false;
                Game1.player.Halt();
                Game1.player.freezePause = 700;
                Game1.warpFarmer("DeepWoods", 105, 80, 1);
                Game1.warpFarmer("DeepWoods", (int)MineCartLocation.X, ((int)MineCartLocation.Y) + 1, 2);
                DeepWoodsManager.currentWarpRequestName = "DeepWoods";
                DeepWoodsManager.currentWarpRequestLocation = new Vector2(MineCartLocation.X * 64, (MineCartLocation.Y + 1) * 64);
                Game1.playSound("smallSelect");
                selectedResponse = -1;
                if (Game1.activeClickableMenu != null && Game1.activeClickableMenu.Equals(this))
                {
                    beginOutro();
                }
            }
        }
    }
}
