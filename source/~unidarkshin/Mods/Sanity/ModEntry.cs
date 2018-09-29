using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Tools;
using StardewValley.Menus;
using StardewValley.BellsAndWhistles;
using StardewValley.Objects;
using System.Timers;
using StardewValley.TerrainFeatures;
using StardewValley.Monsters;
using StardewValley.Locations;
using System.Collections;
using System.IO;
using Netcode;
using xTile.ObjectModel;

namespace Sanity
{
    /// <summary>The mod entry point.</summary>
    /// 
    public class ModEntry : Mod
    {
        public static Random rnd;
        public static Mod instance;

        public static ModData config;
        public double sanity;
        public NetArray<int, NetInt> oXP;
        public Texture2D aRainTex;

        public Dictionary<Vector2, TerrainFeature> ters;

        public ModEntry()
        {
            instance = this;
        }

        public override void Entry(IModHelper helper)
        {
            rnd = new Random();
            aRainTex = this.Helper.Content.Load<Texture2D>(@"blrain.png", ContentSource.ModFolder);
            terfs = new List<ResourceClump>();
            ters = new Dictionary<Vector2, TerrainFeature>();

            InputEvents.ButtonPressed += InputEvents_ButtonPressed;
            GameEvents.OneSecondTick += GameEvents_OneSecondTick;
            SaveEvents.AfterLoad += SaveEvents_AfterLoad;
            SaveEvents.BeforeSave += SaveEvents_BeforeSave;
            GraphicsEvents.OnPostRenderEvent += GraphicsEvents_OnPostRenderEvent;
            TimeEvents.TimeOfDayChanged += TimeEvents_TimeOfDayChanged;
            TimeEvents.AfterDayStarted += TimeEvents_AfterDayStarted;

            helper.ConsoleCommands.Add("set_sanity", "Sets your sanity. Syntax: set_sanity <Int>", this.set_sanity);
        }

        private void TimeEvents_AfterDayStarted(object sender, EventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            no_mons = false;
        }

        private void TimeEvents_TimeOfDayChanged(object sender, EventArgsIntChanged e)
        {
            if (!Context.IsWorldReady)
                return;

            int change = e.PriorInt - e.NewInt;

            if (Math.Abs(change) > 100)
            {
                //if (change >= 0)
                    //sanity = Math.Max(sanity - 15.0, 0);
                //else
                    //sanity = Math.Min(sanity + 5.0, 100.0);
            }

        }

        private void set_sanity(string arg1, string[] arg2)
        {
            if (!Context.IsWorldReady || !Game1.player.isInBed.Value || Game1.IsMultiplayer)
                return;

            if (int.TryParse(arg2[0], out int r) && r >= 0)
            {
                sanity = r;
                return;
            }

            Monitor.Log($"Improper syntax for cmd: {arg1}.");
        }

        private void SaveEvents_BeforeSave(object sender, EventArgs e)
        {
            config.sanity = sanity;

            instance.Helper.WriteJsonFile<ModData>($"Data/{Constants.SaveFolderName}.json", config);

            rem_mons();
        }

        private void GraphicsEvents_OnPostRenderEvent(object sender, EventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            Color c;

            if (sanity <= 20.0)
                c = Color.Red;
            else if (sanity <= 50.0)
                c = Color.Yellow;
            else
                c = Color.DeepSkyBlue;

            drawBar(Game1.viewport.Width - 50, Game1.viewport.Height - 520, sanity / 100.0, c);
        }

        private int barW = 30;
        private int barH = 200;
        private bool ignoreBP = false;

        int time = 200;
        Color col = new Color(170, 87, 254, 255);
        private void drawBar(int x, int y, double p, Color c)
        {
            SpriteBatch spriteBatch = Game1.spriteBatch;

            foreach (KeyValuePair<Vector2, TerrainFeature> ter in Game1.currentLocation.terrainFeatures.Pairs)
            {

                ter.Value.draw(spriteBatch, ter.Key);
            }

            Game1.player.draw(spriteBatch);

            if (sanity <= 50.0)
            {
                float perc = ((50.0f - (float)sanity) / 100.0f);



                /* if (time <= 0)
                 {
                     col = new Color(rnd.Next(0, 255), rnd.Next(0, 255), rnd.Next(0, 255));
                     Monitor.Log($"color: {col}");
                     time = 200;
                 }*/

                spriteBatch.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, col * (perc * 0.70f));
                spriteBatch.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * (perc + 0.1f));



                //time--;
            }

            if (sanity <= 30 && Game1.player.currentLocation.IsOutdoors)
            {
                for (int j = 0; j < Game1.rainDrops.Length; j++)
                {
                    spriteBatch.Draw(aRainTex, Game1.rainDrops[j].position, Game1.getSourceRectForStandardTileSheet(aRainTex, Game1.rainDrops[j].frame, -1, -1), Color.White);
                }
            }

            if (Game1.activeClickableMenu != null)
                Game1.activeClickableMenu.draw(spriteBatch);

            Rectangle destinationRectangle = new Rectangle(x, y, barW, barH);
            spriteBatch.Draw(Game1.staminaRect, destinationRectangle, new Rectangle(0, 0, barW, barH), Color.DarkGray);
            int percentageHeight = (int)((float)(destinationRectangle.Height - 2 * 2) * p);
            destinationRectangle.Y = destinationRectangle.Y + destinationRectangle.Height - percentageHeight - 2;
            destinationRectangle.Height = percentageHeight;
            int percentageWidth = (int)((float)(destinationRectangle.Width - 2 * 2));
            destinationRectangle.X = destinationRectangle.X + destinationRectangle.Width - percentageWidth - 2;
            destinationRectangle.Width = percentageWidth;
            spriteBatch.Draw(Game1.staminaRect, destinationRectangle, new Rectangle(0, 0, percentageWidth, percentageHeight), c);

            

            if ((float)Game1.getOldMouseX() >= x && (float)Game1.getOldMouseY() >= y && (float)Game1.getOldMouseX() <= x + barW)
            {
                Game1.drawWithBorder((int)Math.Round(sanity) + "/" + 100, Color.WhiteSmoke, Color.Red, new Vector2(x - (barW * 4), (int)(y + (barH / 2.0))));
            }

            Helper.Reflection.GetMethod(Game1.game1, "drawHUD").Invoke();



            
            //Game1.spriteBatch.Draw(Game1.mouseCursors, new Vector2(Game1.getMouseX(), Game1.getMouseY()), Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, Game1.options.SnappyMenus ? 44 : 0, 16, 16), Color.White * Game1.mouseCursorTransparency, 0.0f, Vector2.Zero, Game1.pixelZoom + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 0.1f);
        }

        private Timer aTimer = new Timer();
        private int oHealth;
        private int oSpeed;
        private Timer aTimer2;
        private bool no_mons = false;
        private Timer aTimer3;

        private void InputEvents_ButtonPressed(object sender, EventArgsInput e)
        {
            if (!Context.IsWorldReady)
                return;

            if (e.Button == config.teleport)
            {
                Game1.player.health -= 10;
                Game1.player.Stamina -= 20;
                sanity = Math.Max(sanity - 15.0, 0.0);

                Game1.warpHome();

            }
            /* if(e.Button == config.force_new_day && Game1.player.IsMainPlayer)
             {
                 foreach(Farmer f in Game1.getOnlineFarmers())
                 {
                     f.clearBackpack();

                 }
                 Game1.player.clearBackpack();
                 Game1.player.Money = Math.Max(Game1.player.Money - 100000, 0);
                 Game1.showEndOfNightStuff();
             } */


            if (ignoreBP)
                return;

            double s = 0.0;

            if (e.IsUseToolButton && Game1.player.CurrentTool is Axe)
                s -= 0.1;
            else if (e.IsUseToolButton && Game1.player.CurrentTool is WateringCan)
                s += 0.2;
            else if (e.Button == SButton.MouseLeft && Game1.currentLocation.isCharacterAtTile(e.Cursor.GrabTile) != null)
                s += 0.4;

            sanity = Math.Min(Math.Max(sanity + s, 0), 100.0);

            SetTimer(600);

        }

        private void SetTimer(int time)
        {
            ignoreBP = true;
            // Create a timer with a two second interval.
            aTimer = new System.Timers.Timer(time);
            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += OnTimedEvent;

            aTimer.AutoReset = false;
            aTimer.Enabled = true;


        }

        private void SetTimer2(int time, int type)
        {

            // Create a timer with a two second interval.
            aTimer2 = new System.Timers.Timer(time);
            // Hook up the Elapsed event for the timer. 
            aTimer2.Elapsed += OnTimedEvent2;

            aTimer2.AutoReset = false;
            aTimer2.Enabled = true;


        }

        private void SetTimer3(int time, int amt)
        {

            // Create a timer with a two second interval.
            aTimer3 = new System.Timers.Timer(time);
            // Hook up the Elapsed event for the timer. 
            aTimer3.Elapsed += OnTimedEvent3;

            aTimer3.AutoReset = true;
            aTimer3.Enabled = true;

            c1 = amt;
        }
        int c1 = 0;
        private void OnTimedEvent3(object sender, ElapsedEventArgs e)
        {
            GameLocation loc = Game1.player.currentLocation;
            Utility.addSmokePuff(loc, loc.getRandomTile());

            c1--;

            if (c1 <= 0)
            {
                aTimer3.Enabled = false;
                aTimer3.AutoReset = false;
            }
        }

        private void SetTimer4(int time, int amt)
        {

            // Create a timer with a two second interval.
            aTimer4 = new System.Timers.Timer(time);
            // Hook up the Elapsed event for the timer. 
            aTimer4.Elapsed += OnTimedEvent4;

            aTimer4.AutoReset = true;
            aTimer4.Enabled = true;

            c2 = amt;
        }
        int c2 = 0;
        private Timer aTimer4;
        private List<ResourceClump> terfs;

        private void OnTimedEvent4(object sender, ElapsedEventArgs e)
        {
            GameLocation loc = Game1.player.currentLocation;
            Utility.drawLightningBolt(Game1.player.getTileLocation() + new Vector2(rnd.Next(-10, 10), rnd.Next(-10, 10)), loc);

            c2--;

            if (c2 <= 0)
            {
                aTimer4.Enabled = false;
                aTimer4.AutoReset = false;
            }
        }

        private void OnTimedEvent2(object sender, ElapsedEventArgs e)
        {
            Game1.player.addedSpeed = oSpeed;

            aTimer2.Enabled = false;
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            ignoreBP = false;

            aTimer.Enabled = false;
        }

        private void SaveEvents_AfterLoad(object sender, EventArgs e)
        {
            config = instance.Helper.ReadJsonFile<ModData>($"Data/{Constants.SaveFolderName}.json") ?? new ModData();

            sanity = config.sanity;

            oXP = Game1.player.experiencePoints;
            oHealth = Game1.player.health;
            oSpeed = Game1.player.addedSpeed;

        }

        private void GameEvents_OneSecondTick(object sender, EventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            isCollidingPosition();

            while (rnd.NextDouble() <= (0.25 - (sanity / 200.0))) // 0.25
            {
                applyRandomEffect();
            }

            sanity = Math.Min(Math.Max(sanity + getSanityChange(), 0), 100.0);


        }

        private void applyRandomEffect()
        {
            try
            {

                int x = (int)Math.Round(rnd.NextDouble() * 26);
                //int y = 52;
                GameLocation loc = Game1.player.currentLocation;

                switch (x)
                {
                    case 0:
                        Game1.player.health = Math.Max(Game1.player.health - 10, 0);
                        break;
                    case 1:
                        if (loc.characters.Count > 0)
                            loc.characters.RemoveAt(rnd.Next(0, loc.characters.Count - 1));
                        break;
                    case 2:
                        if (loc.characters.Count > 0)
                            loc.characters[(rnd.Next(0, loc.characters.Count - 1))].setTileLocation(loc.getRandomTile());
                        break;
                    case 3:
                        if (loc.characters.Count > 0)
                            loc.characters[(rnd.Next(0, loc.characters.Count - 1))].setTrajectory(new Vector2(rnd.Next(-20, 20), rnd.Next(-20, 20)));
                        break;
                    case 4:
                        if (rnd.NextDouble() < 0.3)
                        {
                            GameLocation loc2 = Game1.locations[rnd.Next(0, Game1.locations.Count - 1)];
                            Vector2 tile = loc2.getRandomTile();

                            while (!loc2.isTileLocationTotallyClearAndPlaceable(tile))
                            {
                                tile = loc2.getRandomTile();
                            }
                            Game1.warpFarmer(loc2.Name, (int)tile.X, (int)tile.Y, true);
                            sanity -= 5.0;
                        }
                        break;
                    case 5:
                        loc.terrainFeatures.Remove(loc.getRandomTile());
                        break;
                    case 6:
                        Vector2 tile2 = loc.getRandomTile();
                        loc.removeEverythingFromThisTile((int)tile2.X, (int)tile2.Y);
                        break;
                    case 7:
                        List<Item> i2 = new List<Item>();
                        foreach (Item it in Game1.player.Items)
                        {
                            if (it != null)
                                i2.Add(it);
                        }
                        Game1.player.removeItemFromInventory(i2[rnd.Next(0, i2.Count - 1)]);
                        break;
                    case 8:
                        Game1.timeOfDay += 100;
                        break;
                    case 9:
                        /*var data = Game1.content.Load<Dictionary<int, string>>("Data\\ObjectInformation");
                        Vector2 tile3 = loc.getRandomTile();
                        Game1.currentLocation.setObject(tile3, new StardewValley.Object(rnd.Next(0, data.Count - 1), 1));*/
                        if (rnd.NextDouble() < 0.3)
                        {
                            if (no_mons)
                                break;

                            Monster m = new Ghost(loc.getRandomTile() * (float)Game1.tileSize);
                            m.DamageToFarmer += (int)(((51.0 - sanity) * (51.0 - sanity)) / 26.0);
                            m.Health = m.Health + (m.Health * (int)(((51.0 - sanity) * (51.0 - sanity)) / 260.0));
                            m.focusedOnFarmers = true;
                            m.wildernessFarmMonster = true;
                            m.Speed += rnd.Next(3 + Game1.player.CombatLevel);
                            m.resilience.Set(m.resilience.Value + (int)(((51.0 - sanity) * (51.0 - sanity)) / 50.0));
                            m.ExperienceGained += (int)(m.Health * (sanity / 50.0));

                            m.Scale = Math.Max((int)(((51.0 - sanity) * (51.0 - sanity)) / 50.0), m.Scale);

                            IList<NPC> characters = Game1.currentLocation.characters;
                            characters.Add((NPC)m);
                        }

                        break;
                    case 10:
                        List<Item> items = new List<Item>();

                        if (!Game1.player.areAllItemsNull())
                        {
                            foreach (Item item in Game1.player.Items)
                            {
                                if (item != null)
                                    items.Add(item);
                            }

                            if (Game1.player.CurrentItem == null)
                            {
                                Item i3 = items[rnd.Next(0, items.Count - 1)];
                                this.Helper.Reflection.GetField<Item>(Game1.player, "CurrentItem").SetValue(i3);
                            }

                            if (Game1.player.CurrentItem is StardewValley.Object)
                                Game1.player.eatHeldObject();
                            else
                                Game1.player.dropActiveItem();

                        }
                        break;
                    case 11:
                        Game1.player.Stamina = Math.Max(Game1.player.Stamina - 20, 0);
                        break;
                    case 12:
                        Game1.player.takeDamage(rnd.Next(5, 20), true, new Monster());
                        break;
                    case 13:
                        if (rnd.NextDouble() < 0.25)
                            Game1.weatherForTomorrow = 3;
                        break;
                    case 14:
                        if(loc.IsOutdoors)
                        loc.addCharacterAtRandomLocation(Utility.getRandomTownNPC());
                        break;
                    case 15:
                        if (Game1.activeClickableMenu != null)
                            Game1.exitActiveMenu();
                        break;
                    case 16:
                        Game1.fadeBlack();
                        Game1.fadeScreenToBlack();
                        break;
                    case 17:
                        Game1.player.setTrajectory(new Vector2(rnd.Next(-30, 30), rnd.Next(-30, 30)));
                        break;
                    case 51:
                        List<NPC> n2 = new List<NPC>();

                        foreach (NPC npc in Utility.getAllCharacters())
                        {
                            n2.Add(npc);
                        }

                        Game1.player.changeFriendship(-1, n2[rnd.Next(0, n2.Count - 1)]);
                        break;
                    case 18:

                        loc.addCritter(new Cloud(loc.getRandomTile()));
                        loc.addCritter(new Rabbit(loc.getRandomTile(), true));

                        Vector2 tile5 = loc.getRandomTile();
                        loc.addCritter(new Birdie((int)tile5.X, (int)tile5.Y));

                        break;
                    case 19:
                        Vector2 tile4 = loc.getRandomTile();
                        loc.explode(tile4, rnd.Next(3, 10), Game1.player);
                        break;
                    case 20:
                        sanity -= 5.0;
                        break;
                    case 21:
                        for (int i = 0; i < (int)((100.0 - sanity) / 10.0); i++)
                        {
                            if (rnd.NextDouble() <= 0.5)
                            {
                                Vector2 tile7 = loc.getRandomTile();
                                loc.addCritter(new Crow((int)tile7.X, (int)tile7.Y));
                            }
                        }

                        break;
                    case 22:
                        Game1.player.addedSpeed = (int)(Game1.player.addedSpeed * rnd.NextDouble() * -1.0);
                        double x2 = (100.0 - sanity) / 90.0;
                        SetTimer2(rnd.Next((int)(5000 * x2), (int)(10000 * x2)), 1);

                        break;
                    case 26:
                        Vector2 tile6 = loc.getRandomTile();
                        while (loc.terrainFeatures.ContainsKey(tile6))
                            tile6 = loc.getRandomTile();

                        loc.removeEverythingExceptCharactersFromThisTile((int)tile6.X, (int)tile6.Y);
                        TerrainFeature ter = getRandomTF(tile6);

                        //ters.Add(tile6, ter);
                        loc.terrainFeatures.Add(tile6, ter);

                        //Monitor.Log($"----------------->TF at {tile6}");


                        break;
                    case 23:
                        if (rnd.NextDouble() < .01)
                        {
                            loc.debris.Add(new Debris((Item)(new StardewValley.Object(434, 1)), Utility.getRandomAdjacentOpenTile(loc.getRandomTile(), loc)));
                        }
                        break;
                    case 24:
                        SetTimer3(rnd.Next(500, 1000), (int)((52.0 - sanity) / 2.0));
                        break;
                    case 25:
                        SetTimer4(rnd.Next(500, 1000), (int)((52.0 - sanity) / 2.0));
                        break;

                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                Monitor.Log($"Error: {e.Message} ::: {e.StackTrace}.");
            }
        }

        public TerrainFeature getRandomTF(Vector2 tile6)
        {
            TerrainFeature ter = null;

            int x3 = rnd.Next(0, 4);
            if (x3 == 0)
            {
                ter = new FruitTree(rnd.Next(628, 633));
            }
            else if (x3 == 1)
            {
                double y = rnd.NextDouble();
                if (y <= 0.33)
                    ter = new GiantCrop(190, tile6);
                else if (y <= 0.66)
                    ter = new GiantCrop(254, tile6);
                else
                    ter = new GiantCrop(276, tile6);
            }
            else if (x3 == 2)
            {
                ter = new Grass(rnd.Next(1, 4), rnd.Next(1, 4));
            }
            else if (x3 == 3)
            {
                int x4 = rnd.Next(0, 7);
                if (x4 == 0)
                    ter = new ResourceClump(600, 2, 2, tile6);
                if (x4 == 1)
                    ter = new ResourceClump(602, 2, 2, tile6);
                if (x4 == 2)
                    ter = new ResourceClump(622, 2, 2, tile6);
                if (x4 == 3)
                    ter = new ResourceClump(672, 2, 2, tile6);
                if (x4 == 4)
                    ter = new ResourceClump(752, 2, 2, tile6);
                if (x4 == 5)
                    ter = new ResourceClump(754, 2, 2, tile6);
                if (x4 == 6)
                    ter = new ResourceClump(756, 2, 2, tile6);
                if (x4 == 7)
                    ter = new ResourceClump(758, 2, 2, tile6);
            }
            else if (x3 == 4)
            {
                ter = new Tree(rnd.Next(1, 7));
            }

            return ter;
        }


        public void rem_mons()
        {

            no_mons = true;

            Monitor.Log("Removed Monsters.");
            for (int j = 0; j < Game1.locations.Count; j++)
            {
                //Game1.locations[j].cleanupBeforeSave();

                IList<NPC> characters = Game1.locations[j].characters;
                for (int i = 0; i < characters.Count; i++)
                {
                    if (characters[i] is Monster && (characters[i] as Monster).wildernessFarmMonster)
                    {
                        characters.RemoveAt(i);


                    }
                }
            }
        }

        private double getSanityChange()
        {
            double s = 0.0;

            for (int i = 0; i < Game1.player.experiencePoints.Count - 1; i++)
            {
                if (oXP[i] != Game1.player.experiencePoints[i])
                {
                    switch (i)
                    {
                        case 0:
                            s += 0.5;
                            break;
                        case 1:
                            s += 0.3;
                            break;
                        case 2:
                            s += 0.4;
                            break;
                        case 3:
                            s -= 0.1;
                            break;
                        case 4:
                            s -= 0.5;
                            break;
                        default:
                            break;
                    }

                }

                oXP = Game1.player.experiencePoints;
            }

            if (Game1.isDarkOut())
                s -= 0.3;
            if (Game1.isRaining && Game1.player.currentLocation.IsOutdoors)
                s -= 0.1;
            if (Game1.currentLocation is MineShaft)
                s -= 0.2;
            if (Game1.isLightning && Game1.player.currentLocation.IsOutdoors)
                s -= 0.5;

            bool alone = true;
            foreach (Farmer f in Game1.getOnlineFarmers())
            {
                if (Game1.player.currentLocation == f.currentLocation && Vector2.Distance(Game1.player.getTileLocation(), f.getTileLocation()) <= 20.0f)
                {
                    s += 0.3;

                    alone = false;
                }

            }
            foreach (NPC f in Game1.currentLocation.characters)
            {
                if (Vector2.Distance(Game1.player.getTileLocation(), f.getTileLocation()) <= 10.0f && !(f is Monster))
                {
                    s += 0.2;

                    alone = false;
                }
                else if (Vector2.Distance(Game1.player.getTileLocation(), f.getTileLocation()) <= 10.0f && (f is Monster) && (f as Monster).wildernessFarmMonster)
                {
                    s -= 0.2;

                }

            }

            if (alone)
                s -= 0.2;
            if (Game1.player.isEating)
                s += 0.5;
            if (Game1.player.isDivorced())
                s -= 0.2;
            if (Game1.player.isEngaged())
                s += 0.2;
            if (Game1.player.isInBed.Value)
                s += 0.2;
            if (Game1.player.isMoving())
                s -= 0.1;
            if (Game1.player.Stamina <= (Game1.player.Stamina / 4.0f))
                s -= 0.5;
            if (Game1.player.health <= (Game1.player.health / 4.0f))
                s -= 0.5;
            if (Game1.player.hasPet())
                s += 0.2;
            if (Game1.player.hasUnlockedSkullDoor)
                s -= 0.5;

            foreach (Item i in Game1.player.Items)
            {
                if (i == null)
                {

                }

                else if (i.Category == StardewValley.Object.flowersCategory || i.Category == StardewValley.Object.GemCategory || i.Category == StardewValley.Object.diamondIndex)
                    s += 0.1;
                else if (i.Category == StardewValley.Object.iridiumBar)
                    s += 0.5;
                else if (i.Category == StardewValley.Object.stardrop && Game1.player.CurrentItem.Name.ToLower().Contains("stardrop"))
                    s += 1.0;
                else if (i.Category == StardewValley.Object.monsterLootCategory)
                    s -= 0.2;
            }

            if (Game1.player.addedSpeed < 0)
                s -= 0.2;

            if (oHealth - Game1.player.health > 0)
                s -= 0.4;

            oHealth = Game1.player.health;

            foreach (NPC npc in Utility.getAllCharacters())
            {
                if (Game1.player.getFriendshipHeartLevelForNPC(npc.Name) >= 5)
                    s += 0.1;
            }

            if (!Game1.player.currentLocation.IsOutdoors)
                s += 0.3;
            if (Game1.activeClickableMenu != null && Game1.activeClickableMenu is BobberBar)
                s += 0.2;
            if (sanity <= 30.0)
                s -= 0.4;
            if (sanity <= 50.0 && sanity > 30.0)
                s -= 0.2;

            return s;
        }

        public void isCollidingPosition()
        {
            bool flag = false;

            Rectangle bounds = Game1.player.GetBoundingBox();
            bounds.Width -= 60;
            bounds.Height -= 60;
            bounds.X += 30;
            bounds.Y += 30;
            foreach (KeyValuePair<Vector2, TerrainFeature> ters in Game1.currentLocation.terrainFeatures.Pairs)
            {
                if (ters.Value.getBoundingBox(ters.Key).Intersects(bounds))
                {
                    Game1.player.health = Math.Max(Game1.player.health - 10, 0);
                    Game1.player.addedSpeed = Game1.player.Speed * -2;
                    sanity -= 5.0;

                    //Game1.chatBox.addMessage("Cannot pass!", Color.Red);
                    if (!flag)
                        flag = true;
                }

            }
            if (!flag)
            {
                Game1.player.addedSpeed = oSpeed;
            }
        }
    }

    public class ModData
    {
        public double sanity { get; set; } = 100.0;
        public SButton teleport { get; set; } = SButton.Home;
        //public SButton force_new_day { get; set; } = SButton.End;
    }
}