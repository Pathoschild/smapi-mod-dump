using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Locations;
using StardewValley.Monsters;
using StardewValley.Objects;
using xTile.Layers;
using xTile.Tiles;

namespace Question
{
    public class ModEntry : Mod, IAssetEditor
    {
        string MapPath = "";


        public bool CanEdit<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals(@"Data\mail");
        }

        public void Edit<T>(IAssetData asset)
        {
            
           
                IDictionary<string, string> npc = asset.AsDictionary<string, string>().Data;
                npc["birthDayMailWizard"] = "Lets Test out....";
                /*
                asset
                .AsDictionary<string, string>()
                .Set("birthDayMail" + npcz[i], i18n.Get("npc_mail", new { npc_name = npcz[i], npc_gift = npc_gifts[i]}));
                
                *///$"Dear @,^ Tomorrow is {npcz[i]}'s Birthday. You should give them a gift. They would love one of the following: ^^{npc_gifts[i]}."
            
        }

        public override void Entry(IModHelper helper)
        {
            helper.Events.Input.ButtonPressed += Button;
        }

        private void Button(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;
            if (e.IsDown(SButton.F4))
            {
                GameLocation loc = Game1.currentLocation;
                loc.lastQuestionKey = "greenhouse_upgrade";
                loc.createQuestionDialogue("Testing 1 2 3", loc.createYesNoResponses(), ConfirmUpgrade);
            }
            /*
            if (e.IsDown(SButton.NumPad8))
            {
                

                //Lets clean this up some
                string question = Helper.Translation.Get("Wedding_Question");
                List<Response> options = new List<Response>()
                {
                    new Response("On the Beach.",this.Helper.Translation.Get("Wedding_Beach")),
                    new Response("In the Forest, near the river.",this.Helper.Translation.Get("Wedding_Forest")),
                    new Response("In the Deep Woods.",this.Helper.Translation.Get("Wedding_Wood")),
                    new Response("Where the Flower Dance takes place!",this.Helper.Translation.Get("Wedding_Flower"))
                };

                Game1.player.currentLocation.createQuestionDialogue(
                    question,
                    options.ToArray(),
                    answer
                );
            }

            if (e.IsDown(SButton.NumPad7))
            {
                Chest c = new Chest(true);
                if(Game1.currentLocation.isTileLocationTotallyClearAndPlaceable(Game1.player.getTileLocation()))
                    Game1.currentLocation.objects.Add(Game1.player.getTileLocation(), c);
                else
                    Game1.showGlobalMessage("Couldnt place chest");
            }

            if (e.IsDown(SButton.NumPad6))
            {
                Game1.mailbox.Add("birthDayMailWizard");
            }

            if (e.IsDown(SButton.NumPad5))
            {
                ICursorPosition c = Helper.Input.GetCursorPosition();
                int x = Game1.getMouseX();
                int y = Game1.getMouseY();

                //Try adding new monsters to the map.
                //BigSlime bs = new BigSlime(new Vector2(x, y), 1);
                int mineLevel = 1;
                if (Game1.player.CombatLevel >= 10)
                    mineLevel = 140;
                else if (Game1.player.CombatLevel >= 8)
                    mineLevel = 100;
                else if (Game1.player.CombatLevel >= 4)
                    mineLevel = 41;
                IList<NPC> characters = Game1.currentLocation.characters;
                GreenSlime greenSlime = new GreenSlime(c.Tile * 64f, mineLevel) {wildernessFarmMonster = true};
                Bat bat = new Bat(c.Tile * 64f, mineLevel) {wildernessFarmMonster = true};
                BigSlime bs = new BigSlime(c.Tile * 64f, mineLevel) { wildernessFarmMonster = true }; ;
                Bug bug = new Bug(c.Tile * 64f, mineLevel) { wildernessFarmMonster = true };
                Ghost ghost = new Ghost(c.Tile * 64f) { wildernessFarmMonster = true };
                MetalHead metal = new MetalHead(c.Tile * 64f, mineLevel) { wildernessFarmMonster = true };
                Mummy mum = new Mummy(c.Tile * 64f) { wildernessFarmMonster = true };
                //ShadowGirl sgirl = new ShadowGirl(c.Tile * 64f) { wildernessFarmMonster = true };
                //ShadowGuy sguy = new ShadowGuy(c.Tile * 64f) { wildernessFarmMonster = true };
                ShadowShaman ss = new ShadowShaman(c.Tile * 64f) { wildernessFarmMonster = true };
                Skeleton sk = new Skeleton(c.Tile * 64f) { wildernessFarmMonster = true };
                SquidKid skid = new SquidKid(c.Tile * 64f) { wildernessFarmMonster = true, Scale = 15f};
                characters.Add((NPC)greenSlime);
                characters.Add((NPC)bat);
                characters.Add((NPC)bs);
                characters.Add((NPC)bug);
                characters.Add((NPC)ghost);
                characters.Add((NPC)metal);
                characters.Add((NPC)mum);
               //characters.Add((NPC)sgirl);
                //characters.Add((NPC)sguy);
                characters.Add((NPC)ss);
                characters.Add((NPC)sk);
                characters.Add((NPC)skid);
            }

            if (e.IsDown(SButton.NumPad9))
            {
                GameLocation location = Game1.getLocationFromName("BusStop");
                TileSheet tileSheet = location.map.GetTileSheet("spring_outdoorsTileSheet");
                TileSheet tileSheet = location.map.GetTileSheet(nameof(BusStop));

                int tileX = 11;
                int tileY = 23;

                string value = location.doesTileHaveProperty(tileX, tileY, "Diggable", "Back");

                location.setTileProperty(tileX, tileY, "Back", "NoSpawn", "T");

                Layer layer = location.map.GetLayer("Back");
                Tile tile = layer.PickTile(new xTile.Dimensions.Location(tileX, tileY) * Game1.tileSize, Game1.viewport.Size);
                tile.Properties.Remove("Diggable");

            }
            */
        }

        private void ConfirmUpgrade(Farmer who, string whichAnswer)
        {
            Monitor.Log($"{whichAnswer} question 1");
            
            GameLocation loc = Game1.currentLocation;
            loc.afterQuestion = null;
            loc.lastQuestionKey = "greenhouse_upgrade";
            loc.createQuestionDialogue("TestingThis Again 1 2 3", loc.createYesNoResponses(), ConfirmUpgrade1);
        }

        private void ConfirmUpgrade1(Farmer who, string whichAnswer)
        {
            Monitor.Log($"{whichAnswer} Question 2");

            
        }
        private void answer(Farmer who, string ans)
        {
                switch (ans)
                {
                    case "On the Beach.":
                        MapPath = "assets/WeddingBeach.tbin";
                        // Load Beach
                        break;
                    case "In the Forest, near the river.":
                        MapPath = "assets/WeddingForest.tbin";
                    // Load Forest
                    break;
                    case "In the Deep Woods.":
                        MapPath = "assets/WeddingWoods.tbin";
                    // Load Wood
                    break;
                    case "Where the Flower Dance takes place!":
                        MapPath = "assets/WeddingFlower.tbin";

                    // Load Flower Dance
                    break;
                }
            Game1.showGlobalMessage(MapPath);
        }
    }
}
