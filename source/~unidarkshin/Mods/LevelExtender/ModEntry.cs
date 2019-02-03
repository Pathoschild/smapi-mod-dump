using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using System.Reflection;
using StardewValley.Menus;
using StardewValley.Monsters;
using StardewValley.Locations;
using StardewValley.Characters;
using System.Collections.Generic;
using System.Collections;
using System.Timers;
using System.Linq;

namespace LevelExtender
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod, IAssetEditor
    {
        public static Mod instance;
        private static System.Timers.Timer aTimer;
        int[] oldXP = { 0, 0, 0, 0, 0 };
        int[] newXP = { 0, 0, 0, 0, 0 };
        bool[] old = { false, false, false, false, false };
        //int[] addedXP = { 0, 0, 0, 0, 0 };
        int[] sLevs = { 0, 0, 0, 0, 0 };
        int[] max = { 100, 100, 100, 100, 100 };
        bool firstFade = false;
        ModData config = new ModData();
        public static Random rand = new Random(Guid.NewGuid().GetHashCode());
        int[] origLevs = { 0, 0, 0, 0, 0 };
        int[] origExp = { 0, 0, 0, 0, 0 };
        bool wm = false;
        bool pres_comp = false;
        int[] oldLevs = { 0, 0, 0, 0, 0 };
        int[] newLevs = { 0, 0, 0, 0, 0 };
        bool[] olev = { false, false, false, false, false };
        bool[] shLev = { true, true, true, true, true };
        double xp_mod = 1.0;

        bool no_mons = false;

        private LEModApi API;

        public LEEvents LEE;

        public EXP addedXP;
        private int total_m;
        private double s_mod;

        public MPModApi mpmod;
        private bool mpload;
        private double mpMult;

        private Timer aTimer2 = new Timer();

        public ModEntry()
        {
            instance = this;
            LEE = new LEEvents();
            addedXP = new EXP(LEE);
            total_m = 0;
            s_mod = -1.0;
            mpload = false;
            mpMult = 1.0;
        }

        public override object GetApi()
        {
            return API = new LEModApi(this);
        }

        public bool CanEdit<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals(@"Data\Fish");
        }

        /// <summary>Edit a matched asset.</summary>
        /// <param name="asset">A helper which encapsulates metadata about an asset and enables changes to it.</param>
        public void Edit<T>(IAssetData asset)
        {
            IDictionary<int, string> data = asset.AsDictionary<int, string>().Data;
            foreach (var pair in data.ToArray())
            {
                string[] fields = pair.Value.Split('/');
                if (int.TryParse(fields[1], out int val))
                {
                    int x = (val - rand.Next(0, (int)(Game1.player.FishingLevel / 4)));
                    if (x < 1)
                        x = rand.Next(1, val);
                    fields[1] = x.ToString();
                    data[pair.Key] = string.Join("/", fields);
                }
            }
        }


        public override void Entry(IModHelper helper)
        {
            //instance = this;
            helper.Events.GameLoop.OneSecondUpdateTicked += this.GameEvents_OneSecondTick;
            helper.Events.GameLoop.UpdateTicked += this.GameEvents_QuarterSecondTick;
            helper.Events.GameLoop.GameLaunched += this.GameEvents_FirstUpdateTick;
            helper.Events.GameLoop.SaveLoaded += this.SaveEvents_AfterLoad;
            helper.Events.GameLoop.Saving += this.SaveEvents_BeforeSave;
            helper.Events.GameLoop.ReturnedToTitle += this.SaveEvents_AfterReturnToTitle;
            helper.Events.Display.MenuChanged += Display_MenuChanged;
            helper.Events.Input.ButtonPressed += this.ControlEvent_KeyPressed;
            helper.Events.GameLoop.DayStarted += this.TimeEvent_AfterDayStarted;

            helper.ConsoleCommands.Add("xp", "Tells the players current XP above or at level 10.", this.TellXP);
            helper.ConsoleCommands.Add("lev", "Sets the player's level: lev <type> <number>", this.SetLev);
            helper.ConsoleCommands.Add("wm_toggle", "Toggles monster spawning: wm_toggle", this.WmT);
            helper.ConsoleCommands.Add("xp_m", "Changes the xp modifier for levels 10 and after: xp_m <decimal 0.0 -> ANY> : 1.0 is default.", this.XpM);
            helper.ConsoleCommands.Add("spawn_modifier", "Forcefully changes mosnter spawn rate to specified decimal value: spawn_modifier <decimal(percent)> : -1.0 to not have any effect.", this.SM);
            helper.ConsoleCommands.Add("xp_table", "Displays the xp table for every level.", this.XPT);

            this.Helper.Content.InvalidateCache("Data/Fish");
            //LEE.OnXPChanged += LEE_OnXPChanged;
            

        }

        private void GameEvents_FirstUpdateTick(object sender, EventArgs e)
        {
            
        }

        private void SetTimer(int time)
        {
            // Create a timer with a two second interval.
            aTimer2 = new System.Timers.Timer(time);
            // Hook up the Elapsed event for the timer. 
                aTimer2.Elapsed += OnTimedEvent2;

            aTimer2.AutoReset = false;
            aTimer2.Enabled = true;


        }

        private void OnTimedEvent2(object sender, ElapsedEventArgs e)
        {
            mpMult = mpmod.Exp_Rate();
            aTimer2.Enabled = false;
        }

        private void XPT(string arg1, string[] arg2)
        {
            Monitor.Log("Level:  |  Experience:");
            for (int i = 10; i < 100; i++)
            {
                Monitor.Log($"     {i} | {Math.Round((1000 * i + (i * i * i * 0.33)) * xp_mod)}");
            }
        }

        private void SM(string command, string[] args)
        {
            if (args.Length < 1 || args[0] == null || !double.TryParse(args[0], out double n))
            {
                Monitor.Log("No decimal value found.");
                return;
            }

            s_mod = n;
            Monitor.Log($"Modifier set to {n * 100}%.");

        }

        private void LEE_OnXPChanged(object sender, EventArgs e)
        {
            Monitor.Log("XP Changed");
        }

        private void TellXP(string command, string[] args)
        {

            int[] temp = { Game1.player.FarmingLevel, Game1.player.FishingLevel, Game1.player.ForagingLevel, Game1.player.MiningLevel, Game1.player.CombatLevel };
            this.Monitor.Log($"XP: Farming  Fishing  Foraging  Mining  Combat |");
            for (int i = 0; i < addedXP.Length; i++)
            {
                if (temp[i] >= 10)
                    this.Monitor.Log($"XP: {addedXP[i]}");
                else
                    this.Monitor.Log($"XP: Empty.");
            }
            this.Monitor.Log($"\n");
            for (int i = 0; i < sLevs.Length; i++)
            {
                if (temp[i] >= 10)
                    this.Monitor.Log($"NL: {Math.Round((1000 * temp[i] + (temp[i] * temp[i] * temp[i] * 0.33)) * xp_mod)}");
                else
                    this.Monitor.Log($"XP: Empty.");
            }
            for (int i = 0; i < 5; i++)
            {
                this.Monitor.Log($"Current XP - Default: {Game1.player.experiencePoints[i]}.");
            }

        }
        private void SetLev(string command, string[] args)
        {
            if (args[0] == null || args[1] == null || !int.TryParse(args[1], out int n))
            {
                this.Monitor.Log($"Function Failed!");
                return;
            }
            n = int.Parse(args[1]);
            if (n < 1 || n > 100)
            {
                this.Monitor.Log($"Function Failed!");
                return;
            }

            if (args[0].ToLower() == "farming")
            {
                Game1.player.FarmingLevel = n;
                if (n < 10)
                {
                    sLevs[0] = 10;
                    addedXP[0] = 0;
                    config.FaLV = 10;
                    config.FaXP = 0;
                    oldXP[0] = 0;
                    newXP[0] = 0;
                    old[0] = false;
                }
                else
                {
                    sLevs[0] = n;
                    addedXP[0] = 0;
                    oldXP[0] = 0;
                    newXP[0] = 0;
                    old[0] = false;
                }
            }
            else if (args[0].ToLower() == "fishing")
            {
                Game1.player.FishingLevel = n;
                if (n < 10)
                {
                    sLevs[1] = 10;
                    addedXP[1] = 0;
                    config.FLV = 10;
                    config.FXP = 0;
                    oldXP[1] = 0;
                    newXP[1] = 0;
                    old[1] = false;
                }
                else
                {
                    sLevs[1] = n;
                    addedXP[1] = 0;
                    oldXP[1] = 0;
                    newXP[1] = 0;
                    old[1] = false;
                }
            }
            else if (args[0].ToLower() == "foraging")
            {
                Game1.player.ForagingLevel = n;
                if (n < 10)
                {
                    sLevs[2] = 10;
                    addedXP[2] = 0;
                    config.FoLV = 10;
                    config.FoXP = 0;
                    oldXP[2] = 0;
                    newXP[2] = 0;
                    old[2] = false;
                }
                else
                {
                    sLevs[2] = n;
                    addedXP[2] = 0;
                    oldXP[2] = 0;
                    newXP[2] = 0;
                    old[2] = false;
                }
            }
            else if (args[0].ToLower() == "mining")
            {
                Game1.player.MiningLevel = n;
                if (n < 10)
                {
                    sLevs[3] = 10;
                    addedXP[3] = 0;
                    config.MLV = 10;
                    config.MXP = 0;
                    oldXP[3] = 0;
                    newXP[3] = 0;
                    old[3] = false;
                }
                else
                {
                    sLevs[3] = n;
                    addedXP[3] = 0;
                    oldXP[3] = 0;
                    newXP[3] = 0;
                    old[3] = false;
                }
            }
            else if (args[0].ToLower() == "combat")
            {
                Game1.player.CombatLevel = n;
                if (n < 10)
                {
                    sLevs[4] = 10;
                    addedXP[4] = 0;
                    config.CLV = 10;
                    config.CXP = 0;
                    oldXP[4] = 0;
                    newXP[4] = 0;
                    old[4] = false;
                }
                else
                {
                    sLevs[4] = n;
                    addedXP[4] = 0;
                    oldXP[4] = 0;
                    newXP[4] = 0;
                    old[4] = false;
                }
            }
            this.Helper.Content.InvalidateCache("Data/Fish");
        }
        private void WmT(string command, string[] args)
        {
            if (!wm)
            {
                wm = true;
                this.Monitor.Log($"Monsters -> ON.");
            }
            else
            {
                wm = false;
                this.Monitor.Log($"Monsters -> OFF.");
            }
        }
        private void XpM(string command, string[] args)
        {
            if (double.TryParse(args[0], out double x) && x > 0.0)
            {
                xp_mod = x;
                this.Monitor.Log($"Modifier set to: {x}");
            }
            else
            {
                this.Monitor.Log($"Valid decimal not used; refer to help command.");
            }
        }
        private void Display_MenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (pres_comp)
            {
                if (e.OldMenu.GetType().FullName == "SkillPrestige.Menus.PrestigeMenu")
                {
                    //closing();
                    SetTimer();
                }
                else if (e.OldMenu.GetType().FullName == "StardewValley.Menus.GameMenu")
                {
                    //closing();
                    SetTimer();
                }
                else if (e.OldMenu.GetType().FullName == "SkillPrestige.Menus.SettingsMenu")
                {
                    //closing();
                    SetTimer();
                }
                else if (e.OldMenu.GetType().FullName == "SkillPrestige.Menus.Dialogs.WarningDialog")
                {
                    //closing();
                    SetTimer();
                }
            }
        }
        public void Closing()
        {

            if (Game1.player.FarmingLevel >= 10 && shLev[0])
            {
                Game1.player.FarmingLevel = origLevs[0];

            }
            else if (!shLev[0])
            {
                if (origLevs[0] - 10 >= 10)
                    sLevs[0] = origLevs[0] - 10;
                else
                    sLevs[0] = 10;
                Game1.player.FarmingLevel = origLevs[0] - 10;
                addedXP[0] = 0;
                oldXP[0] = 0;
                newXP[0] = 0;
                old[0] = false;

            }
            if (Game1.player.FishingLevel >= 10 && shLev[1])
            {
                Game1.player.FishingLevel = origLevs[1];

            }
            else if (!shLev[1])
            {

                if (origLevs[1] - 10 >= 10)
                    sLevs[1] = origLevs[1] - 10;
                else
                    sLevs[1] = 10;
                Game1.player.FishingLevel = origLevs[1] - 10;
                addedXP[1] = 0;
                oldXP[1] = 0;
                newXP[1] = 0;
                old[1] = false;

            }
            if (Game1.player.ForagingLevel >= 10 && shLev[2])
            {
                Game1.player.ForagingLevel = origLevs[2];

            }
            else if (!shLev[2])
            {
                if (origLevs[2] - 10 >= 10)
                    sLevs[2] = origLevs[2] - 10;
                else
                    sLevs[2] = 10;
                Game1.player.ForagingLevel = origLevs[2] - 10;
                addedXP[2] = 0;
                oldXP[2] = 0;
                newXP[2] = 0;
                old[2] = false;

            }
            if (Game1.player.MiningLevel >= 10 && shLev[3])
            {
                Game1.player.MiningLevel = origLevs[3];

            }
            else if (!shLev[3])
            {
                if (origLevs[3] - 10 >= 10)
                    sLevs[3] = origLevs[3] - 10;
                else
                    sLevs[3] = 10;
                Game1.player.MiningLevel = origLevs[3] - 10;
                addedXP[3] = 0;
                oldXP[3] = 0;
                newXP[3] = 0;
                old[3] = false;

            }
            if (Game1.player.CombatLevel >= 10 && shLev[4])
            {
                Game1.player.CombatLevel = origLevs[4];

            }
            else if (!shLev[4])
            {
                if (origLevs[4] - 10 >= 10)
                    sLevs[4] = origLevs[4] - 10;
                else
                    sLevs[4] = 10;
                Game1.player.CombatLevel = origLevs[4] - 10;
                addedXP[4] = 0;
                oldXP[4] = 0;
                newXP[4] = 0;
                old[4] = false;

            }
            origLevs = new int[] { 0, 0, 0, 0, 0 };
            origExp = new int[] { 0, 0, 0, 0, 0 };
            pres_comp = false;
            shLev = new bool[] { true, true, true, true, true };
        }
        private int GetExp(int x)
        {
            switch (x)
            {
                case 0:
                    x = 0;
                    break;
                case 1:
                    x = 100;
                    break;
                case 2:
                    x = 380;
                    break;
                case 3:
                    x = 770;
                    break;
                case 4:
                    x = 1300;
                    break;
                case 5:
                    x = 2150;
                    break;
                case 6:
                    x = 3300;
                    break;
                case 7:
                    x = 4800;
                    break;
                case 8:
                    x = 6900;
                    break;
                case 9:
                    x = 10000;
                    break;
                default:
                    x = 15001;
                    break;
            }
            return x;
        }
        private void ControlEvent_KeyPressed(object sender, ButtonPressedEventArgs e)
        {

            if (Game1.activeClickableMenu is GameMenu && e.Button == SButton.P && !pres_comp)
            {
                //this.Monitor.Log($"{Game1.player.name} pressed P--.");
                pres_comp = true;
                if (Game1.player.FarmingLevel > 10)
                {
                    origLevs[0] = Game1.player.FarmingLevel;
                    Game1.player.FarmingLevel = 10;

                }
                if (Game1.player.FishingLevel > 10)
                {
                    origLevs[1] = Game1.player.FishingLevel;
                    Game1.player.FishingLevel = 10;

                }
                if (Game1.player.ForagingLevel > 10)
                {
                    origLevs[2] = Game1.player.ForagingLevel;
                    Game1.player.ForagingLevel = 10;

                }
                if (Game1.player.MiningLevel > 10)
                {
                    origLevs[3] = Game1.player.MiningLevel;
                    Game1.player.MiningLevel = 10;

                }
                if (Game1.player.CombatLevel > 10)
                {
                    origLevs[4] = Game1.player.CombatLevel;
                    Game1.player.CombatLevel = 10;

                }
            }
        }
        private void GameEvents_OneSecondTick(object sender, EventArgs e)
        {
            int[] temp = { Game1.player.FarmingLevel, Game1.player.FishingLevel, Game1.player.ForagingLevel, Game1.player.MiningLevel, Game1.player.CombatLevel };
            if (Context.IsWorldReady) // save is loaded
            {

                for (int i = 0; i < temp.Length; i++)
                {
                    if (temp[i] >= 10 && !old[i])
                    {
                        oldXP[i] = Game1.player.experiencePoints[i];
                        old[i] = true;
                    }
                    else if (old[i])
                    {
                        newXP[i] = Game1.player.experiencePoints[i];

                        if (newXP[i] - oldXP[i] > 0 && !pres_comp)
                        {
                            old[i] = false;
                            AddFishingXP(newXP[i] - oldXP[i], i);
                        }

                    }
                }


            }
            if (Context.IsWorldReady && pres_comp)
            {

                for (int i = 0; i < temp.Length; i++)
                {
                    if (temp[i] >= 10 && !olev[i])
                    {
                        oldLevs[i] = temp[i];
                        olev[i] = true;

                    }
                    else if (olev[i])
                    {
                        newLevs[i] = temp[i];

                        if (newLevs[i] - oldLevs[i] < 0)
                        {
                            olev[i] = false;
                            if (newLevs[i] - oldLevs[i] == -10 && oldLevs[i] == 10)
                            {
                                shLev[i] = false;
                            }
                        }
                    }
                }
            }
            if (Context.IsWorldReady && !no_mons && wm && Game1.player.currentLocation.IsOutdoors && Game1.activeClickableMenu == null && rand.NextDouble() <= S_R())
            {

                Vector2 loc = Game1.player.currentLocation.getRandomTile();
                while (!(Game1.player.currentLocation.isTileLocationTotallyClearAndPlaceable(loc)))
                {
                    loc = Game1.player.currentLocation.getRandomTile();
                }

                int tier = rand.Next(0, 8);

                Monster m = GetMonster(tier, loc * (float)Game1.tileSize);

                if (tier == 8)
                {
                    tier = 5;
                    m.resilience.Value += 20;
                    m.Slipperiness += rand.Next(10) + 5;
                    m.coinsToDrop.Value = rand.Next(10) * 50;
                    m.startGlowing(new Color(rand.Next(0, 255), rand.Next(0, 255), rand.Next(0, 255)), true, 1.0f);

                    var data = Game1.content.Load<Dictionary<int, string>>("Data\\ObjectInformation");
                    //Item item = new StardewValley.Object(rand.Next(data.Count), 1);

                    m.objectsToDrop.Add(rand.Next(data.Count));

                }
                else
                {
                    tier = 1;
                }

                m.DamageToFarmer = (int)(m.DamageToFarmer / 1.5) + (int)(Game1.player.CombatLevel / 3);
                m.Health = (int)(m.Health / 1.5) + ((Game1.player.CombatLevel / 2) * (m.Health / 10));
                m.focusedOnFarmers = true;
                m.wildernessFarmMonster = true;
                m.Speed += rand.Next(3 + Game1.player.CombatLevel);
                m.resilience.Set(m.resilience.Value + (Game1.player.CombatLevel / 10));
                m.ExperienceGained += ((10 + (Game1.player.CombatLevel * 2)) * tier);

                IList<NPC> characters = Game1.currentLocation.characters;
                characters.Add((NPC)m);
                
                total_m++;
            }

        }
        public double S_R()
        {
            if (Game1.player.CombatLevel == 0)
            {
                return 0.0;
            }

            if (s_mod != -1.0)
            {
                return s_mod;
            }
            else if (API.overSR != -1.0)
            {
                return API.overSR;
            }

            if (Game1.isDarkOut() || Game1.isRaining)
            {
                return (0.010 + (Game1.player.CombatLevel * 0.0001)) * 2;
            }

            return (0.010 + (Game1.player.CombatLevel * 0.0001));

        }
        private void GameEvents_QuarterSecondTick(object sender, UpdateTickedEventArgs e)
        {
            
            if (Context.IsWorldReady && e.IsMultipleOf(8) && Game1.player.FishingLevel > 10)
            {
                if (Game1.activeClickableMenu is BobberBar && !firstFade)
                {


                    firstFade = true;
                    this.Monitor.Log($"{this.Helper.Reflection.GetField<int>(Game1.activeClickableMenu, "bobberBarHeight").GetValue()} -SIZE.");
                    this.Helper.Reflection.GetField<int>(Game1.activeClickableMenu, "bobberBarHeight").SetValue(176);
                    this.Helper.Reflection.GetField<float>(Game1.activeClickableMenu, "bobberBarPos").SetValue((float)(568 - 176));


                }
                else if (!(Game1.activeClickableMenu is BobberBar) && firstFade)
                {
                    firstFade = false;
                    //this.Monitor.Log($"Fade set false. -SIZE.");
                }
                else if (Game1.activeClickableMenu is BobberBar && firstFade)
                {
                    bool bobberInBar = this.Helper.Reflection.GetField<bool>(Game1.activeClickableMenu, "bobberInBar").GetValue();
                    if (!bobberInBar)
                    {
                        float dist = this.Helper.Reflection.GetField<float>(Game1.activeClickableMenu, "distanceFromCatching").GetValue();
                        this.Helper.Reflection.GetField<float>(Game1.activeClickableMenu, "distanceFromCatching").SetValue(dist + ((float)(Game1.player.FishingLevel - 10) / 22000.0f));

                    }
                }
            }

        }
        private void TimeEvent_AfterDayStarted(object sender, EventArgs e)
        {
            if (!mpload && this.Helper.ModRegistry.IsLoaded("f1r3w477.Level_Extender"))
            {
                mpmod = this.Helper.ModRegistry.GetApi<MPModApi>("f1r3w477.Level_Extender");
                mpload = true;
                SetTimer(1000);
            }
            else if (mpload)
            {
                SetTimer(1000);
            }
                no_mons = false;
        }

        public void Rem_mons()
        {

            no_mons = true;

            //Monitor.Log("Removed Monsters.");

            int x = 0;
            int y;
            foreach (GameLocation location in Game1.locations)
            {
                y = location.characters.Count;

                location.characters.Filter(f => !(f is Monster monster) || !monster.wildernessFarmMonster);

                x += (y - location.characters.Count);
            }

            Monitor.Log($"Removed | {x} | / | {total_m} | monsters.");

            total_m = 0;
        }

        private Monster GetMonster(int x, Vector2 loc)
        {
            Monster m;
            switch (x)
            {
                case 0:
                    m = new DustSpirit(loc);
                    break;
                case 1:
                    m = new Grub(loc);
                    break;
                case 2:
                    m = new Skeleton(loc);
                    break;
                case 3:
                    m = new RockCrab(loc);
                    break;
                case 4:
                    m = new Ghost(loc);
                    break;
                case 5:
                    m = new GreenSlime(loc);
                    break;
                case 6:
                    m = new RockGolem(loc);
                    break;
                case 7:
                    m = new ShadowBrute(loc);
                    break;
                case 8:
                    int y = rand.Next(1, 6);

                    //m = new Monster();

                    if (y == 1)
                        m = new RockCrab(loc, "Iridium Crab");
                    else if (y == 2)
                        m = new Ghost(loc, "Carbon Ghost");
                    else if (y == 3)
                        m = new LavaCrab(loc);
                    //else if (y == 4)
                    //m = new Bat(loc, Math.Max(Game1.player.CombatLevel * 5, 50));
                    else if (y == 4)
                        m = new GreenSlime(loc, Math.Max(Game1.player.CombatLevel * 5, 50));
                    else if (y == 5)
                        m = new BigSlime(loc, Math.Max(Game1.player.CombatLevel * 5, 50));
                    else
                        m = new Mummy(loc);

                    break;

                default:
                    m = new Monster();
                    break;
            }

            return m;
        }
        private void SaveEvents_AfterLoad(object sender, EventArgs e)
        {
            //object[] temp = { Game1.player.FarmingLevel, Game1.player.FishingLevel, Game1.player.ForagingLevel, Game1.player.MiningLevel, Game1.player.CombatLevel };
            var config_t = this.Helper.Data.ReadJsonFile<ModData>($"data/{Constants.SaveFolderName}.json") ?? new ModData();
            addedXP[0] = config_t.FaXP;
            sLevs[0] = config_t.FaLV;
            addedXP[1] = config_t.FXP;
            sLevs[1] = config_t.FLV;
            addedXP[2] = config_t.FoXP;
            sLevs[2] = config_t.FoLV;
            addedXP[3] = config_t.MXP;
            sLevs[3] = config_t.MLV;
            addedXP[4] = config_t.CXP;
            sLevs[4] = config_t.CLV;
            wm = config_t.WorldMonsters;
            xp_mod = config_t.Xp_modifier;

            config = config_t;

            if (Game1.player.FarmingLevel >= 10)
            {
                Game1.player.FarmingLevel = sLevs[0];

                this.Monitor.Log($"{Game1.player.Name} loaded farming level {Game1.player.FarmingLevel}!");
            }
            if (Game1.player.FishingLevel >= 10)
            {
                Game1.player.FishingLevel = sLevs[1];

                this.Monitor.Log($"{Game1.player.Name} loaded fishing level {Game1.player.FishingLevel}!");
            }
            if (Game1.player.ForagingLevel >= 10)
            {
                Game1.player.ForagingLevel = sLevs[2];

                this.Monitor.Log($"{Game1.player.Name} loaded foraging level {Game1.player.ForagingLevel}!");
            }
            if (Game1.player.MiningLevel >= 10)
            {
                Game1.player.MiningLevel = sLevs[3];

                this.Monitor.Log($"{Game1.player.Name} loaded mining level {Game1.player.MiningLevel}!");
            }
            if (Game1.player.CombatLevel >= 10)
            {
                Game1.player.CombatLevel = sLevs[4];

                this.Monitor.Log($"{Game1.player.Name} loaded combat level {Game1.player.CombatLevel}!");
            }

            this.Helper.Content.InvalidateCache("Data/Fish");
        }
        private void SaveEvents_BeforeSave(object sender, EventArgs e)
        {

            if (Game1.player.FarmingLevel >= 10)
            {

                Game1.player.FarmingLevel = sLevs[0];
                config.FaXP = addedXP[0];
                config.FaLV = sLevs[0];

                this.Monitor.Log($"{Game1.player.Name} saved farming level {Game1.player.FarmingLevel}!");
            }
            if (Game1.player.FishingLevel >= 10)
            {

                Game1.player.FishingLevel = sLevs[1];
                config.FXP = addedXP[1];
                config.FLV = sLevs[1];

                this.Monitor.Log($"{Game1.player.Name} saved fishing level {Game1.player.FishingLevel}!");
            }
            if (Game1.player.ForagingLevel >= 10)
            {

                Game1.player.ForagingLevel = sLevs[2];
                config.FoXP = addedXP[2];
                config.FoLV = sLevs[2];

                this.Monitor.Log($"{Game1.player.Name} saved foraging level {Game1.player.ForagingLevel}!");
            }
            if (Game1.player.MiningLevel >= 10)
            {

                Game1.player.MiningLevel = sLevs[3];
                config.MXP = addedXP[3];
                config.MLV = sLevs[3];

                this.Monitor.Log($"{Game1.player.Name} saved mining level {Game1.player.MiningLevel}!");
            }
            if (Game1.player.CombatLevel >= 10)
            {

                Game1.player.CombatLevel = sLevs[4];
                config.CXP = addedXP[4];
                config.CLV = sLevs[4];

                this.Monitor.Log($"{Game1.player.Name} saved combat level {Game1.player.CombatLevel}!");
            }
            config.WorldMonsters = wm;
            config.Xp_modifier = xp_mod;

            this.Helper.Data.WriteJsonFile<ModData>($"data/{Constants.SaveFolderName}.json", config);



            if (!no_mons)
            {
                Rem_mons();
            }

        }
        private void SaveEvents_AfterReturnToTitle(object sender, EventArgs e)
        {
            oldXP = new int[] { 0, 0, 0, 0, 0 };
            newXP = new int[] { 0, 0, 0, 0, 0 };
            old = new bool[] { false, false, false, false, false };
            addedXP.Values = new int[] { 0, 0, 0, 0, 0 };
            sLevs = new int[] { 0, 0, 0, 0, 0 };
            max = new int[] { 100, 100, 100, 100, 100 };
            firstFade = false;
            config = new ModData();

            origLevs = new int[] { 0, 0, 0, 0, 0 };
            origExp = new int[] { 0, 0, 0, 0, 0 };

            wm = new bool();
            pres_comp = false;
            oldLevs = new int[] { 0, 0, 0, 0, 0 };
            newLevs = new int[] { 0, 0, 0, 0, 0 };
            olev = new bool[] { false, false, false, false, false };
            shLev = new bool[] { true, true, true, true, true };
            xp_mod = 1.0;
        }
        private void AddFishingXP(int xp, int i)
        {
            int[] temp = { Game1.player.FarmingLevel, Game1.player.FishingLevel, Game1.player.ForagingLevel, Game1.player.MiningLevel, Game1.player.CombatLevel };
            this.Monitor.Log($"Adding XP: {xp * mpMult}");
            addedXP[i] += (int)(xp * mpMult);
            if ((addedXP[i] >= Math.Round((1000 * temp[i] + (temp[i] * temp[i] * temp[i] * 0.33)) * xp_mod)) && sLevs[i] < max[i])
            {
                addedXP[i] = 0;
                sLevs[i] += 1;

                if (i == 0)
                {
                    Game1.player.FarmingLevel = sLevs[i];
                }
                else if (i == 1)
                {
                    Game1.player.FishingLevel = sLevs[i];
                }
                else if (i == 2)
                {
                    Game1.player.ForagingLevel = sLevs[i];
                }
                else if (i == 3)
                {
                    Game1.player.MiningLevel = sLevs[i];
                }
                else if (i == 4)
                {
                    Game1.player.CombatLevel = sLevs[i];
                }

                this.Monitor.Log($"{Game1.player.Name} leveled {i}(index) to {sLevs[i]}!");
            }
            else
            {
                if (i == 0)
                {
                    Game1.player.FarmingLevel = sLevs[i];
                }
                else if (i == 1)
                {
                    Game1.player.FishingLevel = sLevs[i];
                }
                else if (i == 2)
                {
                    Game1.player.ForagingLevel = sLevs[i];
                }
                else if (i == 3)
                {
                    Game1.player.MiningLevel = sLevs[i];
                }
                else if (i == 4)
                {
                    Game1.player.CombatLevel = sLevs[i];
                }
                this.Monitor.Log($"{Game1.player.Name} retained {i}(index) at {sLevs[i]}.");
            }
        }
        private void SetTimer()
        {
            // Create a timer with a two second interval.
            aTimer = new System.Timers.Timer(1100);
            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = false;
            aTimer.Enabled = true;
        }
        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            this.Closing();
        }

        public int[] GetCurXP()
        {
            return addedXP.Values;
        }

        public int[] GetReqXP()
        {
            int[] temp = { Game1.player.FarmingLevel, Game1.player.FishingLevel, Game1.player.ForagingLevel, Game1.player.MiningLevel, Game1.player.CombatLevel };
            int[] xTemp = { 0, 0, 0, 0, 0 };

            for (int i = 0; i < temp.Length; i++)
            {
                xTemp[i] = (int)(Math.Round((1000 * temp[i] + (temp[i] * temp[i] * temp[i] * 0.33)) * xp_mod));
            }

            return xTemp;
        }


    }

    public class LEEvents
    {
        public event EventHandler OnXPChanged;

        public void RaiseEvent()
        {
            if (OnXPChanged != null)
            {
                { OnXPChanged(this, EventArgs.Empty); }
            }
        }
    }

    public class EXP
    {
        private int[] axp;
        LEEvents LEE;

        public EXP(LEEvents lee)
        {
            axp = new int[] { 0, 0, 0, 0, 0 };
            LEE = lee;
        }

        public int this[int key]
        {
            get { return axp[key]; }

            set
            {
                if (axp[key] != value)
                {
                    axp[key] = value;
                    LEE.RaiseEvent();
                }

            }
        }

        public int Length
        {
            get { return axp.Length; }
        }

        public int[] Values
        {
            get { return axp; }
            set
            {

                for (int i = 0; i < axp.Length; i++)
                {
                    if (axp[i] != value[i])
                    {
                        LEE.RaiseEvent();
                        break;
                    }
                }
                axp = value;
            }
        }

    }
}

