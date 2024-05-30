/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ApryllForever/NuclearBombLocations
**
*************************************************/

using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using xTile.Dimensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using xTile;
using StardewModdingAPI.Utilities;
using StardewValley.Network;
using StardewValley.Objects;
using xTile.Tiles;
using StardewValley.Menus;
using Object = StardewValley.Object;
using StardewValley.Monsters;
using static StardewValley.Event;

namespace NuclearBombLocations
{
    [XmlType("Mods_ApryllForever_RestStopLocations_NuclearSunkenShip")]
    public class NuclearSunkenShip : NuclearLocation
    {


        [XmlElement("addedMermaidGhostiesToday")]
        private readonly NetBool addedMermaidGhostiesToday = new NetBool();

        [XmlElement("addedMermaidCGhostiesToday")]
        private readonly NetBool addedMermaidCGhostiesToday = new NetBool();

        [XmlElement("addedMermaidPGhostiesToday")]
        private readonly NetBool addedMermaidPGhostiesToday = new NetBool();

        [XmlElement("addedMermaidZombieToday")]
        private readonly NetBool addedMermaidZombieToday = new NetBool();

        public static string EnterSubQuestion = "Do you wish to enter Marisol's Nuclear Submarine?";



        internal static IModHelper ModHelper { get; set; }


        public NuclearSunkenShip() { }
        public NuclearSunkenShip(IModContentHelper content)
        : base(content, "NuclearSunkenShip", "NuclearSunkenShip")
        {
            if (!Game1.IsMultiplayer)
            {
                base.ExtraMillisecondsPerInGameMinute = 1000;
            }

        }

        public NuclearSunkenShip(IModHelper helper) 
        {
            //MermaidDugoutHouse.ModHelper = IModHelper;
            //helper.Events.GameLoop.DayEnding += OnDayEnding;
        }

        protected override void resetLocalState()
        {
            base.resetLocalState();
            Game1.changeMusicTrack("ApryllForever.NuclearBomb_MermaidMigration");

            // if (Game1.currentLocation == this)
            // {
            //   Game1.ambientLight = Color.BlueViolet;
            // }

            {
                Vector2 position = new Vector2(20, 7);
            
                Chest chest = new Chest(playerChest: false, position);
                chest.dropContents.Value = true;
                chest.synchronized.Value = true;
                chest.type.Value = "interactive";
                chest.SetBigCraftableSpriteIndex(227);
                chest.addItem(ItemRegistry.Create("(O)288", 5));
                if (this.netObjects.ContainsKey(position))
                    this.netObjects.Remove(position);
                if (this.CanItemBePlacedHere(position))
                this.netObjects.Add(position, chest);
            }

            {
                Vector2 position = new Vector2(37, 10);

                Chest chest = new Chest(playerChest: false, position);
                chest.dropContents.Value = true;
                chest.synchronized.Value = true;
                chest.type.Value = "interactive";
                chest.SetBigCraftableSpriteIndex(227);
                chest.addItem(ItemRegistry.Create("(O)288", 5));
                if (this.netObjects.ContainsKey(position))
                    this.netObjects.Remove(position);
                if (this.CanItemBePlacedHere(position))
                    this.netObjects.Add(position, chest);
            }

            {
                Vector2 position = new Vector2(28, 6);

                Chest chest = new Chest(playerChest: false, position);
                chest.dropContents.Value = true;
                chest.synchronized.Value = true;
                chest.type.Value = "interactive";
                chest.SetBigCraftableSpriteIndex(227);
                chest.addItem(ItemRegistry.Create("(O)288", 5));
                if (this.netObjects.ContainsKey(position))
                    this.netObjects.Remove(position);
                if (this.CanItemBePlacedHere(position))
                    this.netObjects.Add(position, chest);
            }

            {
                Vector2 position = new Vector2(7, 5);
                Chest chest = new Chest(playerChest: false, position);
                chest.dropContents.Value = true;
                chest.synchronized.Value = true;
                chest.type.Value = "interactive";
                chest.SetBigCraftableSpriteIndex(227);
                chest.addItem(ItemRegistry.Create("(O)441", 50));
                if (this.netObjects.ContainsKey(position))
                    this.netObjects.Remove(position);
                if (this.CanItemBePlacedHere(position))
                    this.netObjects.Add(position, chest);
            }

            {
                Vector2 position = new Vector2(38, 4);
                Chest chest = new Chest(playerChest: false, position);
                chest.dropContents.Value = true;
                chest.synchronized.Value = true;
                chest.type.Value = "interactive";
                chest.SetBigCraftableSpriteIndex(227);
                chest.addItem(ItemRegistry.Create(((Game1.random.NextDouble() < 0.2) ? "(O)797" : "(O)166"), 1));
                if (this.netObjects.ContainsKey(position))
                    this.netObjects.Remove(position);
                if (this.CanItemBePlacedHere(position))
                    this.netObjects.Add(position, chest);
            }

            {
                Vector2 position = new Vector2(22, 4);
                Chest chest = new Chest(playerChest: false, position);
                chest.dropContents.Value = true;
                chest.synchronized.Value = true;
                chest.type.Value = "interactive";
                chest.SetBigCraftableSpriteIndex(227);
                chest.addItem(ItemRegistry.Create(((Game1.random.NextDouble() < 0.2) ? "(O)797" : "(O)166"), 1));
                if (this.netObjects.ContainsKey(position))
                    this.netObjects.Remove(position);
                if (this.CanItemBePlacedHere(position))
                    this.netObjects.Add(position, chest);
            }

            {
                Vector2 position = new Vector2(40, 8);
                Chest chest = new Chest(playerChest: false, position);
                chest.dropContents.Value = true;
                chest.synchronized.Value = true;
                chest.type.Value = "interactive";
                chest.SetBigCraftableSpriteIndex(227);
                chest.addItem(ItemRegistry.Create(((Game1.random.NextDouble() < 0.2) ? "(O)797" : "(O)166"), 1));
                if (this.netObjects.ContainsKey(position))
                    this.netObjects.Remove(position);
                if (this.CanItemBePlacedHere(position))
                    this.netObjects.Add(position, chest);
            }

            //((Game1.random.NextDouble() < 0.2) ? "(O)797" : "(O)393")

        }
        public override void cleanupBeforePlayerExit()
        {
            base.cleanupBeforePlayerExit();
        
            Game1.changeMusicTrack("none");
        }

        public override void draw(SpriteBatch b)
        {
            base.draw(b);          
        }

        public override bool checkAction(Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
        {

           
            {
                switch (base.getTileIndexAt(tileLocation, "Buildings"))
                {
                    case 941:

                        createQuestionDialogue(EnterSubQuestion, createYesNoResponses(), "EnterSub");

                        return true;
                }
            }

            return base.checkAction(tileLocation, viewport, who);
        }




        protected override void initNetFields()
        {
            base.initNetFields();
            base.NetFields.AddField(this.addedMermaidGhostiesToday);
            base.NetFields.AddField(this.addedMermaidCGhostiesToday);
            base.NetFields.AddField(this.addedMermaidPGhostiesToday);
            base.NetFields.AddField(this.addedMermaidZombieToday);

        }

        public override bool answerDialogue(Response answer)
        {
            if (lastQuestionKey != null && afterQuestion == null)
            {
                string qa = lastQuestionKey.Split(' ')[0] + "_" + answer.responseKey;
                switch (qa)
                {
                    case "EnterSub_Yes":
                        performTouchAction("Warp " + "Custom_MermaidNuclearSub 8 8", Game1.player.Tile);
                        return true;


                       
                }
            }

            return base.answerDialogue(answer);
        }

        public override bool performAction(string action, Farmer who, Location tileLocation)
        {
            
            if (action == "RS.Hospital")
            {
               

            }

            return base.performAction(action, who, tileLocation);
        }

      
        public override void performTouchAction(string actionStr, Vector2 tileLocation)
        {
            string[] split = actionStr.Split(' ');
            string action = split[0];
            int tx = (int)tileLocation.X;
            int ty = (int)tileLocation.Y;

            if (action == "RS.Diveboard")
            {
                Game1.player.jump();
                Game1.player.xVelocity = -16f;
                Game1.player.swimTimer = 800;
                Game1.player.swimming.Value = true;
                Game1.playSound("pullItemFromWater");
                Game1.playSound("bubbles");
            }

            base.performTouchAction(actionStr, tileLocation);
        }


        protected override void resetSharedState()
        {
            base.resetSharedState();
           
            if ((bool)this.addedMermaidGhostiesToday.Value && (bool)this.addedMermaidCGhostiesToday.Value
                && (bool)this.addedMermaidPGhostiesToday.Value
                && (bool)this.addedMermaidZombieToday.Value)
            {
                return;
            }
            this.addedMermaidGhostiesToday.Value = true;
            Random rando;
            rando = Utility.CreateRandom(Game1.stats.DaysPlayed, Game1.uniqueIDForThisGame, 12.0);
            Microsoft.Xna.Framework.Rectangle spawnAreaa;
            spawnAreaa = new Microsoft.Xna.Framework.Rectangle(25, 4, 16, 8);
            for (int tries = 6; tries > 0; tries--)
            {
                Vector2 tile;
                tile = Utility.getRandomPositionInThisRectangle(spawnAreaa, rando);
                if (this.CanItemBePlacedHere(tile))
                {
                    {
                        Ghost a;
                        a = new Ghost(tile * 64f);
                        a.focusedOnFarmers = false;
                        base.characters.Add(a);
                        //return;
                    }
                }
            }

            if ((bool)this.addedMermaidCGhostiesToday.Value && (bool)this.addedMermaidPGhostiesToday.Value
                && (bool)this.addedMermaidZombieToday.Value)
            {
                return;
            }
            this.addedMermaidCGhostiesToday.Value = true;
            Random rand1;
            rand1 = Utility.CreateRandom(Game1.stats.DaysPlayed, Game1.uniqueIDForThisGame, 12.0);
            Microsoft.Xna.Framework.Rectangle spawnArea1;
            spawnArea1 = new Microsoft.Xna.Framework.Rectangle(25, 4, 16, 8);
            for (int tries = 6; tries > 0; tries--)
            {
                Vector2 tile;
                tile = Utility.getRandomPositionInThisRectangle(spawnArea1, rand1);
                if (this.CanItemBePlacedHere(tile))
                {
                    {

                        Ghost a;
                        a = new Ghost(tile * 64f, "Carbon Ghost");
                        a.focusedOnFarmers = false;
                        base.characters.Add(a);
                    }
                }
            }



            if ((bool)this.addedMermaidPGhostiesToday.Value
                && (bool)this.addedMermaidZombieToday.Value)
            {
                return;
            }
            this.addedMermaidPGhostiesToday.Value = true;
            Random rand2;
            rand2 = Utility.CreateRandom(Game1.stats.DaysPlayed, Game1.uniqueIDForThisGame, 12.0);
            Microsoft.Xna.Framework.Rectangle spawnArea2;
            spawnArea2 = new Microsoft.Xna.Framework.Rectangle(25, 4, 16, 8);
            for (int tries = 10; tries > 0; tries--)
            {
                Vector2 tile;
                tile = Utility.getRandomPositionInThisRectangle(spawnArea2, rand2);
                if (this.CanItemBePlacedHere(tile))
                {
                    {

                        Ghost a;
                        a = new Ghost(tile * 64f, "Putrid Ghost");
                        a.focusedOnFarmers = false;
                        base.characters.Add(a);
                    }
                }
            }

            if ((bool)this.addedMermaidZombieToday.Value)
            {
                return;
            }
            this.addedMermaidZombieToday.Value = true;

            rand2 = Utility.CreateRandom(Game1.stats.DaysPlayed, Game1.uniqueIDForThisGame, 12.0);

            spawnArea2 = new Microsoft.Xna.Framework.Rectangle(1, 4, 40, 8);
            for (int tries = 37; tries > 0; tries--)
            {
                Vector2 tile;
                tile = Utility.getRandomPositionInThisRectangle(spawnArea2, rand2);
                if (this.CanItemBePlacedHere(tile))
                {
                    {
                        RockGolem a;
                        a = new RockGolem(tile * 64f, 37);
                        base.characters.Add(a);
                    }
                }
            }








        }


        public override void DayUpdate(int dayOfMonth)
        {
            base.DayUpdate(dayOfMonth);

            for (int j = 0; j < base.characters.Count; j++)
            {
                if (base.characters[j] is Monster)
                {
                    base.characters.RemoveAt(j);
                    j--;
                }
            }
           
            this.addedMermaidGhostiesToday.Value = false;
            this.addedMermaidCGhostiesToday.Value = false;
            this.addedMermaidPGhostiesToday.Value = false;
            this.addedMermaidZombieToday.Value = false;

            /*

            Random rand1;
            rand1 = Utility.CreateRandom(Game1.stats.DaysPlayed, Game1.uniqueIDForThisGame, 12.0);
            Microsoft.Xna.Framework.Rectangle spawnArea1;
            spawnArea1 = new Microsoft.Xna.Framework.Rectangle(1, 4, 40, 8);
            for (int tries = 6; tries > 0; tries--)
            {
                string id;
                id = ((Game1.random.NextDouble() < 0.2) ? "(O)348" : "(O)166");
                Vector2 tile;
                tile = Utility.getRandomPositionInThisRectangle(spawnArea1, rand1);
                if (this.CanItemBePlacedHere(tile))
                {
                    {

                        this.dropObject(ItemRegistry.Create<Object>(id), tile * 64f, Game1.viewport, initialPlacement: true);
                    }
                }
            }

            for (int tries = 6; tries > 0; tries--)
            {
                string id2;
                id2 = ((Game1.random.NextDouble() < 0.3333) ? "(O)797" : "(O)348");
                Vector2 tile;
                tile = Utility.getRandomPositionInThisRectangle(spawnArea1, rand1);
                if (this.CanItemBePlacedHere(tile))
                {
                    {

                        this.dropObject(ItemRegistry.Create<Object>(id2), tile * 64f, Game1.viewport, initialPlacement: true);
                    }
                }
            }

           


            for (int tries = 6; tries > 0; tries--)
            {
                string id3;
                id3 = ((Game1.random.NextDouble() < 0.2) ? "(O)797" : "(O)393");
                Vector2 tile;
                tile = Utility.getRandomPositionInThisRectangle(spawnArea1, rand1);
                if (this.CanItemBePlacedHere(tile))
                {
                    {

                        this.dropObject(ItemRegistry.Create<Object>(id3), tile * 64f, Game1.viewport, initialPlacement: true);
                    }
                }
            }
            */





            float chance;
     
           
            Microsoft.Xna.Framework.Rectangle seaweedShore;
            seaweedShore = new Microsoft.Xna.Framework.Rectangle(1, 4, 40, 8);
            chance = 0.5f;
            while (Game1.random.NextDouble() < (double)chance)
            {
                if (Game1.random.NextDouble() < 0.15)
                {
                    Vector2 position2;
                    position2 = new Vector2(Game1.random.Next(seaweedShore.X, seaweedShore.Right), Game1.random.Next(seaweedShore.Y, seaweedShore.Bottom));
                    if (this.CanItemBePlacedHere(position2))
                    {
                        this.dropObject(ItemRegistry.Create<Object>("(O)152"), position2 * 64f, Game1.viewport, initialPlacement: true);
                    }
                }
                else if (Game1.random.NextDouble() < 0.37)
                {

                    Vector2 position2;
                    position2 = new Vector2(Game1.random.Next(seaweedShore.X, seaweedShore.Right), Game1.random.Next(seaweedShore.Y, seaweedShore.Bottom));
                    if (this.CanItemBePlacedHere(position2))
                    {
                        this.dropObject(ItemRegistry.Create<Object>("(O)348"), position2 * 64f, Game1.viewport, initialPlacement: true);
                    }


                }
                chance /= 2f;
            }
            for (int i = 0; i < 11; i++)
            {
                this.spawnObjects();
            }
         /*
            chance = 0.13f;
            while (Game1.random.NextDouble() < (double)chance)
            {
                string id2;
                id2 = ((Game1.random.NextDouble() < 0.2) ? "(O)797" : "(O)393");
                Vector2 position3;
                position3 = base.getRandomTile();
                position3.Y /= 2f;
                string prop;
                prop = this.doesTileHaveProperty((int)position3.X, (int)position3.Y, "Type", "Back");
                if (this.CanItemBePlacedHere(position3) && (prop == null || !prop.Equals("Wood")))
                {
                    this.dropObject(ItemRegistry.Create<Object>(id2), position3 * 64f, Game1.viewport, initialPlacement: true);
                }
                chance /= 1.1f;
            } */


        }














    }
}