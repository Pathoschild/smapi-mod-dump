/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/unidarkshin/Stardew-Mods
**
*************************************************/

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
using Harmony;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using Netcode;
using Newtonsoft.Json;
using System.IO;

namespace LevelExtender
{
    /// <summary>The mod entry point.</summary>

    public class ModEntry : Mod, IAssetEditor
    {
        public static Mod instance;
        private static System.Timers.Timer aTimer;
        //int[] oldXP = { 0, 0, 0, 0, 0 };
        //int[] newXP = { 0, 0, 0, 0, 0 };
        //bool[] old = { false, false, false, false, false };
        //int[] addedXP = { 0, 0, 0, 0, 0 };
        //int[] sLevs = { 0, 0, 0, 0, 0 };
        //int[] max = { 100, 100, 100, 100, 100 };
        bool firstFade = false;
        public static ModData config = new ModData();
        public static Random rand = new Random(Guid.NewGuid().GetHashCode());
        //int[] origLevs = { 0, 0, 0, 0, 0 };
        //int[] origExp = { 0, 0, 0, 0, 0 };
        bool wm = false;
        bool pres_comp = false;
        //int[] oldLevs = { 0, 0, 0, 0, 0 };
        //int[] newLevs = { 0, 0, 0, 0, 0 };
        //bool[] olev = { false, false, false, false, false };
        //bool[] shLev = { true, true, true, true, true };
        //double xp_mod = 1.0;

        float oStamina = 0.0f;
        public bool initialtooluse = false;



        bool no_mons = false;

        private LEModApi API;

        public LEEvents LEE;

        public ModEntry LE;

        //public EXP addedXP;
        private int total_m;
        private double s_mod;

        public MPModApi mpmod;
        private bool mpload;
        private double mpMult;

        private Timer aTimer2 = new Timer();

        //private Timer xpBarTimer = new Timer();

        //int[] dxp = { 0, 0, 0, 0, 0 };

        List<Timer> xpBarTimers = new List<Timer>();

        //int skillCount = 5;

        List<XPBar> xpBars = new List<XPBar>();

        public static List<string> snames = new List<string>();

        HarmonyInstance harmony;

        public static List<Monster> monsters = new List<Monster>();

        public List<Skill> skills = new List<Skill>();
        //public static List<Skill> tskills = new List<Skill>();
        public static List<int[]> categories = new List<int[]>();

        JsonSerializer serializer = new JsonSerializer();

        public static List<int> skillLevs = new List<int>();


        public ModEntry()
        {
            instance = this;
            LE = this;
            LEE = new LEEvents();
            //addedXP = new EXP(LEE);
            total_m = 0;
            s_mod = -1.0;
            mpload = false;
            mpMult = 1.0;

            serializer.NullValueHandling = NullValueHandling.Ignore;
            serializer.Formatting = Formatting.Indented;
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
                    int x = Math.Max(val - rand.Next(0, (int)(Game1.player.fishingLevel.Value / 4)), val / 2);
                    //if (x < 1)
                    //    x = rand.Next(1, val);
                    fields[1] = x.ToString();
                    data[pair.Key] = string.Join("/", fields);
                }
            }
        }


        public override void Entry(IModHelper helper)
        {
            Initialize(instance.Monitor);

            harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);
            Type[] types1 = { typeof(Microsoft.Xna.Framework.Rectangle), typeof(int), typeof(int), typeof(bool), typeof(float), typeof(int), typeof(float), typeof(float), typeof(bool), typeof(Farmer) };
            Type[] types2 = { typeof(Item) };
            Type[] types3 = { typeof(Item), typeof(bool) };

            harmony.Patch(
                    original: AccessTools.Method(typeof(StardewValley.GameLocation), "damageMonster", types1), //nameof(this.Helper.Reflection.GetMethod(typeof(StardewValley.Tools.FishingRod), "doPullFishFromWater"))),
                    prefix: new HarmonyMethod(typeof(ModEntry), nameof(this.DM))
                );
            harmony.Patch(
                    original: AccessTools.Method(typeof(StardewValley.Farmer), "addItemToInventoryBool", types3), //nameof(this.Helper.Reflection.GetMethod(typeof(StardewValley.Tools.FishingRod), "doPullFishFromWater"))),
                    prefix: new HarmonyMethod(typeof(ModEntry), nameof(this.AITI2))
                );


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
            helper.Events.Input.ButtonReleased += this.ControlEvent_KeyReleased;
            helper.Events.Display.Rendered += this.Display_Rendered;
            helper.Events.Player.Warped += this.Player_Warped;

            //LEE.OnXPChanged += LEE;

            helper.ConsoleCommands.Add("xp", "Displays the xp table for your current skill levels.", this.XPT);
            helper.ConsoleCommands.Add("lev", "Sets the player's level: lev <skill name> <number>", this.SetLev);
            helper.ConsoleCommands.Add("wm_toggle", "Toggles monster spawning: wm_toggle", this.WmT);
            helper.ConsoleCommands.Add("xp_m", "Changes the xp modifier for a given skill: xp_m <skill name> <decimal 0.0 -> ANY>: 1.0 is default. Must restart game to take effect", this.XpM);
            helper.ConsoleCommands.Add("spawn_modifier", "Forcefully changes monster spawn rate to specified decimal value: spawn_modifier <decimal(percent)> : -1.0 to not have any effect.", this.SM);
            helper.ConsoleCommands.Add("xp_table", "Displays the XP table for a given skill: xp_table <skill name>", this.TellXP);
            helper.ConsoleCommands.Add("set_xp", "Sets your current XP for a given skill: set_xp <skill name> <XP: int 0 -> ANY>", this.SetXP);
            helper.ConsoleCommands.Add("draw_bars", "Sets whether the XP bars should be drawn or not: draw_bars <bool>, Default; true.", this.DrawBars);
            helper.ConsoleCommands.Add("draw_ein", "Sets whether the extra item notifications should be drawn or not: draw_ein <bool>, Default; true.", this.DrawEIN);
            helper.ConsoleCommands.Add("min_ein_price", "Sets the minimum price threshold for extra item notifications: min_ein_price <int>, Default; 50", this.MinEINP);
            //helper.ConsoleCommands.Add("LE_cmds", "Displays the xp table for your current levels.", this.XPT);

            this.Helper.Content.InvalidateCache("Data/Fish");
            LEE.OnXPChanged += this.OnXPChanged;


        }

        private void MinEINP(string arg1, string[] arg2)
        {
            if (!int.TryParse(arg2[0], out int val))
                return;

            config.minItemPriceForNotifications = val;
            Monitor.Log($"You successfully set the minimum price threshold for extra item notifications to {val}.");
        }

        private void DrawEIN(string arg1, string[] arg2)
        {
            if (!bool.TryParse(arg2[0], out bool val))
                return;

            config.drawExtraItemNotifications = val;
            Monitor.Log($"You successfully set draw extra item notifications to {val}.");
        }

        private void DrawBars(string arg1, string[] arg2)
        {
            if (!bool.TryParse(arg2[0], out bool val))
                return;

            config.drawBars = val;
            Monitor.Log($"You successfully set draw XP bars to {val}.");
        }

        public static bool AITI2(Item item, bool makeActiveObject)
        {
            try
            {
                if (item == null || item.HasBeenInInventory)
                    return true;

                //Monitor.Log($"Picking up item {item.DisplayName}");
                int cat = item.Category;
                string str = "";

                int tstack = item.Stack;

                //tskills = skills
                int i = 0;

                foreach (int[] cats in categories)
                {
                    //Monitor.Log($"LE cats: {cats}, item cat {cat}, {i}, cont {cats.Contains(cat)}, drawein {config.drawExtraItemNotifications}");
                    if (cats.Contains(cat) && ShouldDup(i))
                    {
                        item.Stack += 1;

                        while (ShouldDup(i))
                        {
                            item.Stack += 1;
                        }

                        if (config.drawExtraItemNotifications)
                            str = $"Your {snames[i]} level allowed you to obtain {item.Stack - tstack} extra {item.DisplayName}!";

                        break;
                    }

                    i++;
                }

                if (str.Length > 0 && item.salePrice() >= config.minItemPriceForNotifications)
                {
                    //Game1.chatBox.addMessage(str, Color.DeepSkyBlue);
                    Game1.addHUDMessage(new HUDMessage(str, Color.DeepSkyBlue, 3000, true));
                }
                //item.HasBeenInInventory = true;

                return true;

            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(AITI2)}:\n{ex}", LogLevel.Error);
                return true;
            }
        }

        private void Player_Warped(object sender, WarpedEventArgs e)
        {

        }

        public static bool DM(
      Microsoft.Xna.Framework.Rectangle areaOfEffect,
      int minDamage,
      int maxDamage,
      bool isBomb,
      float knockBackModifier,
      int addedPrecision,
      float critChance,
      float critMultiplier,
      bool triggerMonsterInvincibleTimer,
      Farmer who)
        {
            try
            {
                GameLocation cl = Game1.currentLocation;
                if (!cl.IsFarm ) //&& !cl.IsOutdoors)
                    return true;

                int dmg = 0;

                bool flag1 = false;

                for (int index = cl.characters.Count - 1; index >= 0; --index)
                {
                    if (index < cl.characters.Count && cl.characters[index] is Monster character && (character.IsMonster && character.Health > 0) && character.TakesDamageFromHitbox(areaOfEffect))
                    {
                        if (!monsters.Contains(character))
                            continue;

                        if (character.currentLocation == null)
                            character.currentLocation = cl;
                        if (!character.IsInvisible && !character.isInvincible() && (isBomb || instance.Helper.Reflection.GetMethod(Game1.currentLocation, "isMonsterDamageApplicable").Invoke<bool>(who, character, true) || instance.Helper.Reflection.GetMethod(Game1.currentLocation, "isMonsterDamageApplicable").Invoke<bool>(who, character, false)))
                        {
                            bool flag2 = !isBomb && who != null && (who.CurrentTool != null && who.CurrentTool is MeleeWeapon) && (int)(NetFieldBase<int, NetInt>)(who.CurrentTool as MeleeWeapon).type == 1;
                            bool flag3 = false;
                            if (flag2 && MeleeWeapon.daggerHitsLeft > 1)
                                flag3 = true;
                            if (flag3)
                                triggerMonsterInvincibleTimer = false;
                            flag1 = true;
                            //if (Game1.currentLocation == this)
                            //Rumble.rumble(0.1f + (float)(Game1.random.NextDouble() / 8.0), (float)(200 + Game1.random.Next(-50, 50)));
                            Microsoft.Xna.Framework.Rectangle boundingBox = character.GetBoundingBox();
                            Vector2 trajectory = Utility.getAwayFromPlayerTrajectory(boundingBox, who);
                            if ((double)knockBackModifier > 0.0)
                                trajectory *= knockBackModifier;
                            else
                                trajectory = new Vector2(character.xVelocity, character.yVelocity);
                            if (character.Slipperiness == -1)
                                trajectory = Vector2.Zero;
                            bool flag4 = false;
                            if (who != null && who.CurrentTool != null && character.hitWithTool(who.CurrentTool))
                                return true;
                            if (who.professions.Contains(25))
                                critChance += critChance * 0.5f;
                            int amount1;
                            if (maxDamage >= 0)
                            {
                                int num = Game1.random.Next(minDamage, maxDamage + 1);
                                if (who != null && Game1.random.NextDouble() < (double)critChance + (double)who.LuckLevel * ((double)critChance / 40.0))
                                {
                                    flag4 = true;
                                    //this.playSound("crit");
                                }
                                int amount2 = Math.Max(1, (flag4 ? (int)((double)num * (double)critMultiplier) : num) + (who != null ? who.attack * 3 : 0));
                                if (who != null && who.professions.Contains(24))
                                    amount2 = (int)Math.Ceiling((double)amount2 * 1.10000002384186);
                                if (who != null && who.professions.Contains(26))
                                    amount2 = (int)Math.Ceiling((double)amount2 * 1.14999997615814);
                                if (who != null & flag4 && who.professions.Contains(29))
                                    amount2 = (int)((double)amount2 * 2.0);
                                if (who != null)
                                {
                                    foreach (BaseEnchantment enchantment in who.enchantments)
                                        enchantment.OnCalculateDamage(character, character.currentLocation, who, ref amount2);
                                }
                                //amount1 = character.takeDamage(amount2, (int)trajectory.X, (int)trajectory.Y, isBomb, (double)addedPrecision / 10.0, who);
                                dmg = Math.Max(1, amount2 - (int)(NetFieldBase<int, NetInt>)character.resilience);
                                //this.seenPlayer.Value = true;
                                if (Game1.random.NextDouble() < (double)(NetFieldBase<double, NetDouble>)character.missChance - (double)(NetFieldBase<double, NetDouble>)character.missChance * addedPrecision)
                                {
                                    //num = -1;
                                    return true;
                                }
                            }
                            if (character.Health - dmg <= 0)
                            {

                                who.gainExperience(4, character.ExperienceGained);

                            }

                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(DM)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        DateTime otime;

        private void Display_Rendered(object sender, RenderedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;
            if (otime == null)
                otime = DateTime.Now;

            for (int i = 0; i < xpBars.Count; i++)
            {

                try
                {
                    float bscale = 1.0f;

                    if (xpBars[i] == null)
                        continue;

                    Skill skill = xpBars[i].skill;
                    string name = String.Join(" ", skill.name.ToCharArray());
                    //string[] skills = { "F a r m i n g", "F i s h i n g", "F o r a g i n g", "M i n i n g", "C o m b a t" };
                    //int[] skillLevs = { Game1.player.FarmingLevel, Game1.player.FishingLevel, Game1.player.ForagingLevel, Game1.player.MiningLevel, Game1.player.CombatLevel };
                    int startX = 8;
                    int startY = 8;
                    int sep = (int)(30 * bscale);
                    int barSep = (int)(60 * bscale);


                    int key = skill.key;
                    int xp = skill.xp;
                    int xpc = skill.xpc;
                    int lev = skill.level;
                    int startXP = skill.getReqXP(lev - 1);
                    double deltaTime = DateTime.Now.Subtract(xpBars[i].time).TotalMilliseconds;
                    float transp;

                    /*if (i == 0)
                    {

                        using (System.IO.StreamWriter file =
                        new System.IO.StreamWriter(@"C:\Users\lematd\Desktop\deltatime.txt", true))
                        {
                            file.WriteLine($"{DateTime.Now} - {xpBars[i].time} = {deltaTime}");
                        }
                    }*/

                    if (deltaTime >= 0 && deltaTime <= 1000)
                    {
                        transp = ((float)deltaTime) / 1200.0f;
                    }
                    else if (deltaTime > 1000 && deltaTime <= 4000)
                    {
                        transp = 0.833f;
                    }
                    else
                    {
                        transp = ((float)(5000 - deltaTime)) / 1200.0f;
                    }


                    int curXP = xp;


                    int maxXP = skill.getReqXP(lev);

                    if (startXP > 0)
                    {
                        maxXP = maxXP - startXP;
                        curXP = curXP - startXP;
                        startXP = 0;
                    }

                    int iWidth = (int)(198 * bscale);
                    double mod = iWidth / (maxXP * 1.0);
                    int bar2w = (int)Math.Round(xpc * mod) + 1;
                    int bar1w = (int)Math.Round(curXP * mod) - bar2w;


                    if (i == 0 && xpBars[i].ych < 0)
                    {
                        double ms = (DateTime.Now - otime).TotalMilliseconds;
                        double addv = (xpBars[i].ych + (ms / 15.625 * bscale));
                        xpBars[i].ych = (addv >= 0 ? 0 : addv);
                        //Monitor.Log($"NEG yChange Value: {xpBars[i].ych} -> {deltaTime}");
                    }
                    else if (i == 0 && deltaTime >= 4000)
                    {
                        double addv = (deltaTime - 4000) / 15.625 * bscale;
                        xpBars[i].ych = (addv >= 64 ? 64 : addv);
                        //Monitor.Log($"yChange Value: {xpBars[i].ych} -> {deltaTime}");
                    }


                    xpBars[i].ych = xpBars[0].ych;


                    if (config.drawBars)
                    {
                        Vector2 r1d = new Vector2((float)Math.Round(214 * bscale), (float)Math.Round(64 * bscale));
                        Vector2 r2d = new Vector2((float)Math.Round(210 * bscale), (float)Math.Round(60 * bscale));
                        Vector2 r3d = new Vector2((float)Math.Round(200 * bscale), (float)Math.Round(20 * bscale));
                        Vector2 r4d = new Vector2(bar1w, (float)Math.Round(18 * bscale));
                        Vector2 r5d = new Vector2(bar2w, (float)Math.Round(18 * bscale));

                        Game1.spriteBatch.Draw(Game1.staminaRect, new Rectangle(startX - 7, startY + (barSep * i) - 7 - xpBars[i].ychi, (int)r1d.X, (int)r1d.Y), Color.DarkRed * transp);
                        Game1.spriteBatch.Draw(Game1.staminaRect, new Rectangle(startX - 5, startY + (barSep * i) - 5 - xpBars[i].ychi, (int)r2d.X, (int)r2d.Y), new Color(210, 173, 85) * transp);
                        //Game1.spriteBatch.DrawString(Game1.dialogueFont, $"{skills[key]}", new Vector2((int)Math.Round(((startX - 7 + 214) / 2.0) - ((skills[key].Length / 2.0) * 12)), startY - 3 + (barSep * i) - xpBars[i].ychi), Color.Black * transp, 0.0f, Vector2.Zero, (float)(Game1.pixelZoom / 6f), SpriteEffects.None, 0.5f);
                        //Game1.spriteBatch.DrawString(Game1.dialogueFont, $"{skills[key]}", new Vector2((int)Math.Round(((startX - 7 + 214) / 2.0) - ((skills[key].Length / 2.0) * 12)) + 1, startY - 3 + (barSep * i) + 1 - xpBars[i].ychi), Color.Black * transp, 0.0f, Vector2.Zero, (float)(Game1.pixelZoom / 6f), SpriteEffects.None, 0.5f);
                        //Vector2 sn1loc = new Vector2((int)Math.Round(((startX - 7 + r1d.X) / 2.0) - (Game1.dialogueFont.MeasureString(xpBars[i].skill.name).X * (Game1.pixelZoom / 6.0f / 2.0f * bscale))), startY - 3 + (barSep * i) - xpBars[i].ychi);
                        //Utility.drawTextWithColoredShadow(Game1.spriteBatch, skills[key], Game1.dialogueFont, sn1loc, Color.Black * (transp), new Color(90, 35, 0) * transp, Game1.pixelZoom / 6f, 0.5f);

                        Game1.spriteBatch.DrawString(Game1.dialogueFont, $"{name}", new Vector2((int)Math.Round(((startX - 7 + r1d.X) / 2.0) - (Game1.dialogueFont.MeasureString(name).X * (Game1.pixelZoom / 6.0f / 2.0f)* bscale)), (startY - 3 + (barSep * i) - xpBars[i].ychi) * bscale), new Color(30, 3, 0) * (transp * 1.1f), 0.0f, Vector2.Zero, (float)(Game1.pixelZoom / 6f * bscale), SpriteEffects.None, 0.5f);
                        Game1.spriteBatch.DrawString(Game1.dialogueFont, $"{name}", new Vector2((int)Math.Round(((startX - 7 + r1d.X) / 2.0) - (Game1.dialogueFont.MeasureString(name).X * (Game1.pixelZoom / 6.0f / 2.0f)* bscale)) + 1, (startY - 3 + (barSep * i) - xpBars[i].ychi + 1) * bscale), new Color(90, 35, 0) * (transp), 0.0f, Vector2.Zero, (float)(Game1.pixelZoom / 6.0f * bscale), SpriteEffects.None, 0.5f);

                        Game1.spriteBatch.Draw(Game1.staminaRect, new Rectangle(startX, startY + (barSep * i) + sep - xpBars[i].ychi, (int)r3d.X, (int)r3d.Y), Color.Black * transp);
                        Game1.spriteBatch.Draw(Game1.staminaRect, new Rectangle(startX + 1, startY + (barSep * i) + sep + 1 - xpBars[i].ychi, bar1w, (int)r4d.Y), Color.SeaGreen * transp);
                        Game1.spriteBatch.Draw(Game1.staminaRect, new Rectangle(startX + 1 + bar1w, startY + (barSep * i) + sep + 1 - xpBars[i].ychi, bar2w, (int)r5d.Y), Color.Turquoise * transp);

                        Vector2 mPos = new Vector2(Game1.getMouseX(), Game1.getMouseY());
                        Vector2 bCenter = new Vector2(startX + (200 / 2), startY + (barSep * i) + sep + (20 / 2) - xpBars[i].ychi);
                        float dist = Vector2.Distance(mPos, bCenter);

                        if (dist <= 250f)
                        {

                            float f = Math.Min(25f / dist, 1.0f);

                            string xpt = $"{curXP} / {maxXP}";

                            Game1.spriteBatch.DrawString(Game1.dialogueFont, xpt, new Vector2((int)Math.Round(((startX + 200) / 2.0) - (Game1.dialogueFont.MeasureString(xpt).X * (Game1.pixelZoom / 10.0f / 2.0f))), startY + (barSep * i) + sep + 1 - xpBars[i].ychi), Color.White * f * (transp + 0.05f), 0.0f, Vector2.Zero, (Game1.pixelZoom / 10f), SpriteEffects.None, 0.5f);
                        }

                    }
                }

                catch (Exception ex)
                {
                    Monitor.Log($"Non-Serious draw violation: {ex.Message}");
                    continue;
                }

            }

            otime = DateTime.Now;

            //Game1.spriteBatch.Draw(Game1.mouseCursors, new Vector2(Game1.getMouseX(), Game1.getMouseY()), Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, Game1.options.SnappyMenus ? 44 : 0, 16, 16), Color.White * Game1.mouseCursorTransparency, 0.0f, Vector2.Zero, Game1.pixelZoom + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 0.1f);
        }

        private void SetXP(string command, string[] arg)
        {
            if (!Context.IsWorldReady || arg.Length < 2 || !int.TryParse(arg[1], out int xp))
            {
                Monitor.Log("No skill name entered or the xp was not a whole number.");
                return;
            }

            //int[] skillLevs = { Game1.player.FarmingLevel, Game1.player.FishingLevel, Game1.player.ForagingLevel, Game1.player.MiningLevel, Game1.player.CombatLevel };
            Skill skill = skills.SingleOrDefault(sk => sk.name.ToLower() == arg[0].ToLower());

            if (skill == null)
            {
                Monitor.Log($"Invalid skill name: {arg[0]}");
                return;
            }

            if (skill.key < 5)
                Game1.player.experiencePoints[skill.key] = xp; //Math.Min(xp, skill.getReqXP(skill.level) - 2);
            else
                skill.xp = xp;

            
        }

        private new static IMonitor Monitor;

        public static void Initialize(IMonitor monitor)
        {
            Monitor = monitor;
        }

        private void ControlEvent_KeyReleased(object sender, ButtonReleasedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

        }

        private void GameEvents_FirstUpdateTick(object sender, EventArgs e)
        {

        }

        List<DateTime> xpBarStartTime = new List<DateTime>();

        private void SetTimer(int time, int index)
        {

            if (index == 0)
            {
                // Create a timer with a two second interval.
                aTimer = new System.Timers.Timer(1100);
                // Hook up the Elapsed event for the timer. 
                aTimer.Elapsed += OnTimedEvent;
                aTimer.AutoReset = false;
                aTimer.Enabled = true;

            }
            else if (index == 1)
            {
                // Create a timer with a two second interval.
                aTimer2 = new System.Timers.Timer(time);
                // Hook up the Elapsed event for the timer. 
                aTimer2.Elapsed += OnTimedEvent2;

                aTimer2.AutoReset = false;
                aTimer2.Enabled = true;

            }
            else if (index == 2)
            {
                shouldDraw = new System.Timers.Timer(time);
                // Hook up the Elapsed event for the timer. 
                shouldDraw.Elapsed += sDrawEnd;

                shouldDraw.AutoReset = false;
                shouldDraw.Enabled = true;
            }



        }

        private void sDrawEnd(object sender, ElapsedEventArgs e)
        {
            shouldDraw.Enabled = false;
        }

        public void EndXPBar(int key)
        {

            //xpBars[0] = null;
            //xpBars.RemoveAt(0);
            //pushElementsToZero(xpBars);

            var bar = xpBars.SingleOrDefault(x => x.skill.key == key);
            double yval = bar.ych;

            xpBars.Remove(bar);

            pushElementsToZero(xpBars);

            xpBars[0].ych = 0;
            //setYchVals(yval * -1);

        }

        private void pushElementsToZero(List<int> list)
        {
            List<int> temp = new List<int>();

            foreach (var item in list)
            {
                temp.Add(item);
            }

            list = temp;
        }
        private void pushElementsToZero(List<Timer> list)
        {
            List<Timer> temp = new List<Timer>();

            foreach (var item in list)
            {
                temp.Add(item);
            }

            list = temp;
        }
        private void pushElementsToZero(List<XPBar> list)
        {
            List<XPBar> temp = new List<XPBar>();

            foreach (var item in list)
            {
                temp.Add(item);
            }

            list = temp;
        }

        private void OnTimedEvent2(object sender, ElapsedEventArgs e)
        {
            mpMult = mpmod.Exp_Rate();
            aTimer2.Enabled = false;
        }

        private void XPT(string arg1, string[] arg2)
        {

            Monitor.Log("Skill:  | Level:  |  Current Experience:  | Experience Needed:", LogLevel.Info);

            for (int i = 0; i < skills.Count; i++)
            {
                int xpn = skills[i].getReqXP(skills[i].level);
                Monitor.Log($"{skills[i].name} | {skills[i].level} | {skills[i].xp} | {xpn}", LogLevel.Info);
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

        private void OnXPChanged(object sender, EXPEventArgs e)
        {
            XPBar bar = xpBars.SingleOrDefault(b => b.skill.key == e.key);
            Skill skill = skills.SingleOrDefault(sk => sk.key == e.key);

            if (skill == null || skill.xpc < 0 || skill.xpc > 100001 || shouldDraw.Enabled)
                return;

            //Monitor.Log($"XP Changed: index {e.key}, EXP {e.xp}");

            //bool exists = false;


            if (bar != null)
            {
                bar.timer.Stop();
                bar.timer.Start();
                bar.time = DateTime.Now;
                //xpBars[i].xpc = e.xp;
                //exists = true;

                /*if (i == 0)
                {
                    //xpBars[i].ych = -1 * xpBars[i].ych;
                    xpBars[i].movedir = -1;
                    Monitor.Log("reset on xpchange");
                }*/
                double val = bar.ych * -1;

                //Monitor.Log($"xpchanged, val: {val}");

                setYchVals(val);


                sortByTime();

                //break;                   
            }
            else
            {
                xpBars.Add(new XPBar(skill));
            }

        }

        public void sortByTime()
        {
            List<XPBar> SortedList = xpBars.OrderBy(o => o.time).ToList();

            xpBars = SortedList;
        }
        public void setYchVals(double val)
        {
            foreach (var bar in xpBars)
            {
                bar.ych = val;
            }
        }

        private void TellXP(string command, string[] args)
        {
            if (args.Length < 1)
                return;

            Skill skill = skills.SingleOrDefault(sk => sk.name.ToLower() == args[0].ToLower());

            if (skill == null)
            {
                Monitor.Log("Could not find a match for given skill name.");
                return;
            }

            string str = $"{skill.name}: ";
            int count = 0;

            foreach (int xp in skill.xp_table)
            {
                str += $"{count} -> {xp}, ";
                count++;

                if (count % 5 == 0)
                    str += "\n";
            }

            Monitor.Log(str);


        }

        private void SetLev(string command, string[] args)
        {
            if (args[0] == null || args[1] == null || !int.TryParse(args[1], out int n))
            {
                Monitor.Log($"Function Failed!");
                return;
            }
            //n = int.Parse(args[1]);
            if (n < 0 || n > 100)
            {
                Monitor.Log($"Function Failed!");
                return;
            }

            Skill skill = skills.SingleOrDefault(sk => sk.name.ToLower() == args[0].ToLower());

            if (skill == null)
                return;

            skill.level = n;

            if (skill.key == 1)
                this.Helper.Content.InvalidateCache("Data/Fish");
        }
        private void WmT(string command, string[] args)
        {
            if (!wm)
            {
                wm = true;
                Monitor.Log($"Overworld Monster Spawning -> ON.");
            }
            else
            {
                wm = false;
                Monitor.Log($"Overworld Monster Spawning -> OFF.");
            }
        }
        private void XpM(string command, string[] args)
        {
            if (args.Length > 1 && double.TryParse(args[1], out double x) && x > 0.0)
            {
                //xp_mod = x;
                Skill skill = skills.SingleOrDefault(sk => sk.name.ToLower() == args[0].ToLower());

                if (skill == null)
                    return;

                skill.xp_mod = x;
                Monitor.Log($"The XP modifier for {skill.name} was set to: {x}");
            }
            else
            {
                Monitor.Log($"Valid decimal not used; refer to help command.");
            }
        }
        private void Display_MenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (!Context.IsWorldReady || e.OldMenu == null)
                return;

            /*if (pres_comp)
            {
                if (e.OldMenu.GetType().FullName == "SkillPrestige.Menus.PrestigeMenu")
                {
                    //closing();
                    SetTimer(1100, 0);
                }
                else if (e.OldMenu.GetType().FullName == "StardewValley.Menus.GameMenu")
                {
                    //closing();
                    SetTimer(1100, 0);
                }
                else if (e.OldMenu.GetType().FullName == "SkillPrestige.Menus.SettingsMenu")
                {
                    //closing();
                    SetTimer(1100, 0);
                }
                else if (e.OldMenu.GetType().FullName == "SkillPrestige.Menus.Dialogs.WarningDialog")
                {
                    //closing();
                    SetTimer(1100, 0);
                }
            }*/
        }
        public void Closing()
        {

            /*if (Game1.player.farmingLevel.Value >= 10 && shLev[0])
            {
                Game1.player.farmingLevel.Value = origLevs[0];

            }
            else if (!shLev[0])
            {
                if (origLevs[0] - 10 >= 10)
                    sLevs[0] = origLevs[0] - 10;
                else
                    sLevs[0] = 10;
                Game1.player.farmingLevel.Value = origLevs[0] - 10;
                addedXP[0] = 0;
                oldXP[0] = 0;
                newXP[0] = 0;
                old[0] = false;

            }
            if (Game1.player.fishingLevel.Value >= 10 && shLev[1])
            {
                Game1.player.fishingLevel.Value = origLevs[1];

            }
            else if (!shLev[1])
            {

                if (origLevs[1] - 10 >= 10)
                    sLevs[1] = origLevs[1] - 10;
                else
                    sLevs[1] = 10;
                Game1.player.fishingLevel.Value = origLevs[1] - 10;
                addedXP[1] = 0;
                oldXP[1] = 0;
                newXP[1] = 0;
                old[1] = false;

            }
            if (Game1.player.foragingLevel.Value >= 10 && shLev[2])
            {
                Game1.player.foragingLevel.Value = origLevs[2];

            }
            else if (!shLev[2])
            {
                if (origLevs[2] - 10 >= 10)
                    sLevs[2] = origLevs[2] - 10;
                else
                    sLevs[2] = 10;
                Game1.player.foragingLevel.Value = origLevs[2] - 10;
                addedXP[2] = 0;
                oldXP[2] = 0;
                newXP[2] = 0;
                old[2] = false;

            }
            if (Game1.player.miningLevel.Value >= 10 && shLev[3])
            {
                Game1.player.miningLevel.Value = origLevs[3];

            }
            else if (!shLev[3])
            {
                if (origLevs[3] - 10 >= 10)
                    sLevs[3] = origLevs[3] - 10;
                else
                    sLevs[3] = 10;
                Game1.player.miningLevel.Value = origLevs[3] - 10;
                addedXP[3] = 0;
                oldXP[3] = 0;
                newXP[3] = 0;
                old[3] = false;

            }
            if (Game1.player.combatLevel.Value >= 10 && shLev[4])
            {
                Game1.player.combatLevel.Value = origLevs[4];

            }
            else if (!shLev[4])
            {
                if (origLevs[4] - 10 >= 10)
                    sLevs[4] = origLevs[4] - 10;
                else
                    sLevs[4] = 10;
                Game1.player.combatLevel.Value = origLevs[4] - 10;
                addedXP[4] = 0;
                oldXP[4] = 0;
                newXP[4] = 0;
                old[4] = false;

            }
            origLevs = new int[] { 0, 0, 0, 0, 0 };
            origExp = new int[] { 0, 0, 0, 0, 0 };
            pres_comp = false;
            shLev = new bool[] { true, true, true, true, true };
            */


        }

        public List<int> defReqXPs = new List<int>{ 100, 380, 770, 1300, 2150, 3300, 4800, 6900, 10000, 15000 };


        private void ControlEvent_KeyPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            /*if (Game1.activeClickableMenu is GameMenu && e.Button == SButton.P && !pres_comp)
            {
                //Monitor.Log($"{Game1.player.name} pressed P--.");
                pres_comp = true;
                if (Game1.player.farmingLevel.Value > 10)
                {
                    origLevs[0] = Game1.player.farmingLevel.Value;
                    Game1.player.farmingLevel.Value = 10;

                }
                if (Game1.player.fishingLevel.Value > 10)
                {
                    origLevs[1] = Game1.player.fishingLevel.Value;
                    Game1.player.fishingLevel.Value = 10;

                }
                if (Game1.player.foragingLevel.Value > 10)
                {
                    origLevs[2] = Game1.player.foragingLevel.Value;
                    Game1.player.foragingLevel.Value = 10;

                }
                if (Game1.player.miningLevel.Value > 10)
                {
                    origLevs[3] = Game1.player.miningLevel.Value;
                    Game1.player.miningLevel.Value = 10;

                }
                if (Game1.player.combatLevel.Value > 10)
                {
                    origLevs[4] = Game1.player.combatLevel.Value;
                    Game1.player.combatLevel.Value = 10;

                }
            }*/

        }
        private void GameEvents_OneSecondTick(object sender, OneSecondUpdateTickedEventArgs e)
        {

            if (!Context.IsWorldReady)
                return;

            
            if (e.IsMultipleOf(3600))
            {
                List<Monster> tmons = new List<Monster>();

                foreach (Monster mon in monsters)
                {
                    if (mon == null || mon.Health <= 0 || mon.currentLocation == null)
                    {
                        //monsters.Remove(mon);
                        tmons.Add(mon);
                    }
                }
                foreach (Monster mon in tmons)
                {
                    monsters.Remove(mon);
                }

            }

            if (e.IsMultipleOf(1800))
            {
                for (int i = 0; i < skillLevs.Count; i++)
                {
                    skillLevs[i] = skills[i].level;
                }
            }

            if (skills.Count > 4)
            {

                int[] temp = { Game1.player.farmingLevel.Value, Game1.player.fishingLevel.Value, Game1.player.foragingLevel.Value, Game1.player.miningLevel.Value, Game1.player.combatLevel.Value };

                for (int i = 0; i < temp.Length; i++)
                {

                    Skill skill = skills.SingleOrDefault(sk => sk.key == i);

                    if (skill == null)
                    {
                        Monitor.Log($"LE ERROR - Skill {snames[i]} not registered properly for exp gain, please restart and/or report if no change.");
                    }


                    if (skill.xp != Game1.player.experiencePoints[i])
                    {
                        skill.xp = Game1.player.experiencePoints[i];
                        //oldXP[i] = -1;
                    }
                }
            }

            
            /*if (pres_comp)
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
            }*/


            if (!no_mons && wm && Game1.player.currentLocation.IsOutdoors && Game1.activeClickableMenu == null && rand.NextDouble() <= S_R())
            {

                Vector2 loc = Game1.player.currentLocation.getRandomTile();
                while (!(Game1.player.currentLocation.isTileLocationTotallyClearAndPlaceable(loc)))
                {
                    loc = Game1.player.currentLocation.getRandomTile();
                }

                int tier = rand.Next(0, 9);

                Monster m = GetMonster(tier, loc * (float)Game1.tileSize);

                if (tier == 8)
                {
                    tier = 5;
                    m.resilience.Value += 20;
                    m.Slipperiness += rand.Next(10) + 5;
                    m.coinsToDrop.Value = rand.Next(10) * 50;
                    m.startGlowing(new Color(rand.Next(0, 255), rand.Next(0, 255), rand.Next(0, 255)), true, 1.0f);
                    m.Health *= 1 + (rand.Next(Game1.player.CombatLevel / 2, Game1.player.CombatLevel));

                    var data = Game1.content.Load<Dictionary<int, string>>("Data\\ObjectInformation");
                    //Item item = new StardewValley.Object(rand.Next(data.Count), 1);

                    m.objectsToDrop.Add(rand.Next(data.Count));
                    m.displayName += ": LE BOSS";
                    m.Scale = m.Scale * (float)(1 + (rand.NextDouble() * Game1.player.CombatLevel / 25.0));
                }
                else
                {
                    tier = 1;
                }

                m.DamageToFarmer = (int)(m.DamageToFarmer / 1.5) + (int)(Game1.player.combatLevel.Value / 3);
                m.Health *= 1 + (Game1.player.CombatLevel / 4);
                //Monitor.Log($"{m.Name} Health: {lovepower} > {m.Health}");
                m.focusedOnFarmers = true;
                m.wildernessFarmMonster = true;
                m.Speed += rand.Next((int)Math.Round((Game1.player.combatLevel.Value / 10.0)));
                m.resilience.Set(m.resilience.Value + (Game1.player.combatLevel.Value / 10));
                m.experienceGained.Value += (int)(m.Health / 100.0) + ((10 + (Game1.player.combatLevel.Value * 2)) * tier);

                IList<NPC> characters = Game1.currentLocation.characters;
                characters.Add((NPC)m);

                total_m++;

                if (tier == 5)
                    Game1.chatBox.addMessage($"A boss has spawned in your current location!", Color.DarkRed);

                monsters.Add(m);
            }

        }
        public double S_R()
        {
            if (Game1.player.combatLevel.Value == 0)
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
                return (0.01 + (Game1.player.combatLevel.Value * 0.0001)) * 1.5;
            }

            return (0.01 + (Game1.player.combatLevel.Value * 0.0001));

        }
        private void GameEvents_QuarterSecondTick(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (Game1.player.UsingTool && initialtooluse == false)
            {
                oStamina = Game1.player.Stamina;
                initialtooluse = true;
            }
            else if (!Game1.player.UsingTool && initialtooluse == true)
            {
                if (Game1.player.Stamina > oStamina)
                {
                    Game1.player.Stamina = Math.Max(oStamina - 0.5f, 0.0f);
                }
                else
                {

                }

                oStamina = 0.0f;
                initialtooluse = false;

            }


            if (e.IsMultipleOf(8)) // && Game1.player.FishingLevel > 10)
            {
                if (Game1.activeClickableMenu is BobberBar && !firstFade)
                {
                    //Monitor.Log($"Fishing Level:  {Game1.player.fishingLevel.Value}");
                    //Monitor.Log($"Modified Fishing Level:  {Game1.player.FishingLevel}");
                    //Monitor.Log($"Buffs: {this.Helper.Reflection.GetField<string>(Game1.buffsDisplay, "buffs").GetValue()}");
                    int bobberBonus = 0;
                    Tool tool = Game1.player.CurrentTool;
                    bool beginnersRod;


                    beginnersRod = tool != null && tool is FishingRod && (int)(NetFieldBase<int, NetInt>)tool.upgradeLevel == 1;


                    foreach (var attachment in tool.attachments.Where(n => n != null))
                    {
                        if (attachment.name == "Cork Bobber")
                        {
                            bobberBonus = 24;
                        }
                    }
                    //Item check = this.Helper.Reflection.
                    if (Game1.player.FishingLevel > 99)
                        bobberBonus += 8;
                    else if (Game1.player.FishingLevel > 74)
                        bobberBonus += 6;
                    else if (Game1.player.FishingLevel > 49)
                        bobberBonus += 4;
                    else if (Game1.player.FishingLevel > 24)
                        bobberBonus += 2;

                    int bobberBarSize;

                    if (!(this.Helper.ModRegistry.IsLoaded("DevinLematty.ExtremeFishingOverhaul")))
                    {
                        if (beginnersRod)
                            bobberBarSize = 80 + (5 * 9);
                        else if (Game1.player.FishingLevel < 11)
                            bobberBarSize = 80 + bobberBonus + (int)(Game1.player.FishingLevel * 9);
                        else
                            bobberBarSize = 165 + bobberBonus + (int)(Game1.player.FishingLevel * (0.5 + (rand.NextDouble() / 2.0)));
                    }
                    else
                    {
                        if (beginnersRod)
                            bobberBarSize = 80 + (5 * 7);
                        else if (Game1.player.FishingLevel < 11)
                            bobberBarSize = 80 + bobberBonus + (int)(Game1.player.FishingLevel * 7);
                        else if (Game1.player.FishingLevel > 10 && Game1.player.FishingLevel < 20)
                            bobberBarSize = 150 + bobberBonus + (int)(Game1.player.FishingLevel);
                        else
                            bobberBarSize = 170 + bobberBonus + (int)(Game1.player.FishingLevel * 0.8 * (0.5 + (rand.NextDouble() / 2.0)));
                    }


                    firstFade = true;
                    //Monitor.Log($"{this.Helper.Reflection.GetField<int>(Game1.activeClickableMenu, "bobberBarHeight").GetValue()} -SIZE.");
                    this.Helper.Reflection.GetField<int>(Game1.activeClickableMenu, "bobberBarHeight").SetValue(bobberBarSize);
                    this.Helper.Reflection.GetField<float>(Game1.activeClickableMenu, "bobberBarPos").SetValue((float)(568 - bobberBarSize));


                }
                else if (!(Game1.activeClickableMenu is BobberBar) && firstFade)
                {
                    firstFade = false;
                    //Monitor.Log($"Fade set false. -SIZE.");
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

            if (e.IsMultipleOf(15))
            {
                

            }

        }


        //public static double[] dupRates = { 0.002, 0.002, 0.002, 0.002, 0.002 };

        public static bool ShouldDup(int index)
        {
            double drate = 0.002;

            if (index == 0 || index == 2)
                drate = 0.002 / 2.0;

            //int[] skillLevs = { Game1.player.FarmingLevel, Game1.player.FishingLevel, Game1.player.ForagingLevel, Game1.player.MiningLevel, Game1.player.CombatLevel };

            if (rand.NextDouble() <= (skillLevs[index] * drate))
            {
                return true;
            }

            return false;
        }


        private void TimeEvent_AfterDayStarted(object sender, EventArgs e)
        {
            if (LocalMultiplayer.IsLocalMultiplayer())
            {
                Monitor.Log("LE: Splitscreen Multiplayer is not currently supported. Mod will not load.");
                System.Environment.Exit(1);
            }

            //List<HoeDirt> list = new List<HoeDirt>();
            Farm farm = Game1.getFarm();
            double gchance = Game1.player.FarmingLevel * 0.0002;
            double pchance = Game1.player.FarmingLevel * 0.001;
            foreach (Vector2 key in farm.terrainFeatures.Keys)
            {
                if (farm.terrainFeatures[key] is HoeDirt tf && tf.crop != null && rand.NextDouble() < gchance)
                {
                    tf.crop.growCompletely();
                    //tf.crop.minHarvest.Value = rand.Next(5, 100);
                    //Monitor.Log("LE: hit gchance!");
                    //tf.crop.tintColor.Value = Color.Lavender;
                }
                else if (farm.terrainFeatures[key] is HoeDirt tf2 && tf2.crop != null && rand.NextDouble() < pchance)
                {

                    tf2.crop.currentPhase.Value = Math.Min(tf2.crop.currentPhase.Value + 1, tf2.crop.phaseDays.Count - 1);
                    //tf2.crop.tintColor.Value = Color.DarkBlue;
                }

            }


            if (!mpload && this.Helper.ModRegistry.IsLoaded("f1r3w477.Level_Extender"))
            {
                mpmod = this.Helper.ModRegistry.GetApi<MPModApi>("f1r3w477.Level_Extender");
                mpload = true;
                SetTimer(1000, 1);
            }
            else if (mpload)
            {
                SetTimer(1000, 1);
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

                location.characters.Filter(f => !(f.IsMonster));

                /*foreach (var ch in location.characters)
                {
                    if(ch.IsMonster)
                    location.characters.Remove(ch);
                }*/


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

        Timer shouldDraw;

        private void SaveEvents_AfterLoad(object sender, SaveLoadedEventArgs e)
        {

            SetTimer(2000, 2);

            try
            {

                Monitor.Log("Starting skill load for LE");

                var config_t = this.Helper.Data.ReadJsonFile<ModData>($"data/{Constants.SaveFolderName}.json") ?? new ModData();

                int[] sdlevs = { Game1.player.FarmingLevel, Game1.player.FishingLevel, Game1.player.ForagingLevel, Game1.player.MiningLevel, Game1.player.CombatLevel };
                string[] sdnames = { "Farming", "Fishing", "Foraging", "Mining", "Combat" };

                int[] cats0 = { -16, -74, -75, -79, -80, -81 };
                int[] cats1 = { -4 };
                int[] cats2 = cats0;
                int[] cats3 = { -2, -12, -15 };
                int[] cats4 = { -28, -29, -95, -96, -98 };

                List<int[]> cats = new List<int[]>();

                cats.Add(cats0);
                cats.Add(cats1);
                cats.Add(cats2);
                cats.Add(cats3);
                cats.Add(cats4);

                int count = 0;

                foreach (string str in config_t.skills)
                {
                    Monitor.Log($"skill load - {str}");
                    //Skill sk = JsonConvert.DeserializeObject<Skill>(str);
                    string[] vals = str.Split(',');
                    Skill sk = new Skill(LE, vals[0], int.Parse(vals[1]), double.Parse(vals[2]), new List<int>(defReqXPs), cats[count]);
                    skills.Add(sk);
                    snames.Add(sk.name);
                    categories.Add(sk.cats);
                    skillLevs.Add(sk.level);
                    count++;
                }

                for (int i = count; i < 5; i++)
                {
                    Monitor.Log($"adding skills - {i}, dxp: {Game1.player.experiencePoints[i]}");
                    Skill sk = new Skill(LE, sdnames[i], Game1.player.experiencePoints[i], 1.0, new List<int>(defReqXPs), cats[i]);
                    skills.Add(sk);
                    snames.Add(sk.name);
                    categories.Add(sk.cats);
                    skillLevs.Add(sk.level);
                }


                wm = config_t.WorldMonsters;
                //xp_mod = config_t.Xp_modifier;

                config = config_t;





                //config = this.Helper.Data.ReadJsonFile<ModData>($"data/{Constants.SaveFolderName}.json") ?? new ModData();
            }
            catch (Exception ex)
            {
                Monitor.Log($"LE failed loading skills, mod will not start: {ex.Message}", LogLevel.Trace);
                System.Environment.Exit(1);
            }


            this.Helper.Content.InvalidateCache("Data/Fish");
        }
        private void SaveEvents_BeforeSave(object sender, EventArgs e)
        {
            config.skills = new List<string>();

            foreach (Skill skill in skills)
            {
                //serializer.Converters.Add(new JavaScriptDateTimeConverter());
                //config.skills = new List<string>();

                config.skills.Add($"{skill.name},{skill.xp},{skill.xp_mod}");
                
            }
            

            //this.Helper.Data.WriteJsonFile<ModData>($"data/{Constants.SaveFolderName}.json", config);

            config.WorldMonsters = wm;
            //config.Xp_modifier = xp_mod;
            

            this.Helper.Data.WriteJsonFile<ModData>($"data/{Constants.SaveFolderName}.json", config);

            if (!no_mons)
            {
                Rem_mons();
            }

        }
        private void SaveEvents_AfterReturnToTitle(object sender, EventArgs e)
        {
            /*oldXP = new int[] { 0, 0, 0, 0, 0 };
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
            shLev = new bool[] { true, true, true, true, true };*/
            wm = new bool();
            pres_comp = false;
            firstFade = false;
            config = new ModData();
            //xp_mod = 1.0;

            skills = new List<Skill>();
            snames = new List<string>();
            categories = new List<int[]>();
            skillLevs = new List<int>();
        }
        

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            this.Closing();
        }

        /*public int[] GetCurXP()
        {
            return addedXP.Values;
        }*/


        public dynamic TalkToSkill(string[] args)
        {
            if (args.Length < 3)
                return -3;

            string arg0 = args[0].ToLower();
            string arg1 = args[1].ToLower();
            string arg2 = args[2].ToLower();
            string arg3 = "";
            if (args.Length > 3)
            arg3 = args[3].ToLower();

            Skill s = skills.SingleOrDefault(sk => sk.name.ToLower() == arg1);
            if (s == null)
            {
                return -2;
            }           

            if (arg0 == "get")
            {
                if (arg2 == "xp")
                {
                    return s.xp;
                }
                else if (arg2 == "level")
                {
                    return s.level;
                }
                else
                {
                    return -2;
                }

            }
            else if (arg0 == "set")
            {
                if (!int.TryParse(arg3, out int r))
                    return -2;

                if (arg2 == "xp")
                {
                    s.xp = r;
                    return r;
                }
                else if (arg2 == "level")
                {
                    s.level = r;
                    return r;
                }
                else
                {
                    return -2;
                }

            }

            return -1;
        }

        public int initializeSkill(string name, int xp, double? xp_mod = null, List<int> xp_table = null, int[] cats = null)
        {
            Skill sk = new Skill(LE, name, xp, xp_mod, xp_table, cats);

            if (sk == null)
            {
                return -1;
            }

            skills.Add(sk);

            return 0;
        }

        //public dynamic notifySkill()
    }

    public class LEEvents
    {
        public event EventHandler<EXPEventArgs> OnXPChanged;

        public void RaiseEvent(EXPEventArgs args)
        {
            if (OnXPChanged != null)
            {
                { OnXPChanged(this, args); }
            }
        }
    }

    /*public class EXP
    {
        private int[] axp;
        LEEvents LEE;
        EXPEventArgs args;

        public EXP(LEEvents lee)
        {
            axp = new int[] { 0, 0, 0, 0, 0 };
            LEE = lee;
            args = new EXPEventArgs();
        }

        public int this[int key]
        {
            get { return axp[key]; }

            set
            {
                if (axp[key] != value)
                {
                    args.key = key;
                    //args.xp = value - axp[key];
                    axp[key] = value;

                    LEE.RaiseEvent(args);
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
                        args.key = i;
                        //args.xp = value[i] - axp[i];
                        axp[i] = value[i];


                        //LEE.RaiseEvent();
                        //break;
                    }
                }
                axp = value;
            }
        }

        //dummy events for levels below 10

    }*/

    public class EXPEventArgs : EventArgs
    {
        public int key { get; set; }
    }

    public class XPBar
    {
        public Skill skill;
        public Timer timer;
        public DateTime time;

        public double ych = 0;
        public bool startmove = false;
        public int movedir = 0;


        public XPBar(Skill skill)
        {
            this.skill = skill;
            this.timer = new Timer(5000);
            timer.Elapsed += delegate { skill.LE.EndXPBar(skill.key); };

            timer.AutoReset = false;
            timer.Enabled = true;

            time = DateTime.Now;

        }

        public int ychi
        {
            get => (int)Math.Round(ych);
            set { ychi = value; }
        }

    }

    public class Skill
    {
        
        public string name;
        public int key;
        public bool lvlbyxp = false;
        private int Level;
        public int level
        {
            get { return Level; }
            set
            {
                //LE.Monitor.Log($"LE Level: {level}, XP: {xp}, KEY: {key}");
                //int rxp = getReqXP(level);
                Level = value;

                if (key < 5)
                {
                    //LE.Monitor.Log($"LE Level Initialize: {level}, XP: {xp}, KEY: {key}");

                    if (key == 0)
                        Game1.player.FarmingLevel = level;
                    else if (key == 1)
                        Game1.player.FishingLevel = level;
                    else if (key == 2)
                        Game1.player.ForagingLevel = level;
                    else if (key == 3)
                        Game1.player.MiningLevel = level;
                    else if (key == 4)
                        Game1.player.CombatLevel = level;


                }
                if (!lvlbyxp)
                {
                    int reqxp = getReqXP(level - 1);

                    if (key < 5)
                        Game1.player.experiencePoints[key] = reqxp;

                    XP = reqxp;
                }


                lvlbyxp = false;
            }
        }
        public int xpc;
        public EXPEventArgs args;
        public ModEntry LE;
        public List<int> xp_table;
        public double xp_mod;
        private int XP;
        public int xp
        {
            get { return XP; }
            set
            {
                if (xp != value)
                {
                    xpc = value - xp;
                    XP = value;
                    checkForLevelUp();
                    LE.LEE.RaiseEvent(args);
                }
            }

        }

        public int[] cats;

        int bmaxxp = 0;

        public Skill(ModEntry LE, string name, int xp, double? xp_mod = null, List<int> xp_table = null, int[] cats = null)
        {
            if (xp_table != null && xp_table.Count > 0)
            {
                bmaxxp = xp_table.Max();
            }

            //LE.Monitor.Log($"LE XP_MOD: {xp_mod}");

            this.LE = LE;
          
            this.name = name;
            this.key = LE.skills.Count;
            args = new EXPEventArgs();
            args.key = key;
            this.xp_table = xp_table ?? new List<int>();
            if (xp_mod == null)
                this.xp_mod = 1.0;
            else
                this.xp_mod = xp_mod.Value;

            

            this.cats = cats ?? new int[0];

            if (key == 0)
                Level = Game1.player.FarmingLevel;
            else if (key == 1)
                Level = Game1.player.FishingLevel;
            else if (key == 2)
                Level = Game1.player.ForagingLevel;
            else if (key == 3)
                Level = Game1.player.MiningLevel;
            else if (key == 4)
                Level = Game1.player.CombatLevel;
            else
                Level = 0;

            generateTable(101); 

            if (key < 5)
            {
                Game1.player.experiencePoints[key] = xp;
                XP = xp;
            }
            else
                XP = xp;

            checkForLevelUp();
        }
        public void checkForLevelUp()
        {
            //int reqxp = getReqXP(level);

            int l = getLevByXP();

            if (l != level)
            {
                lvlbyxp = true;
                level = l;

            }

        }

        public int getReqXP(int lev)
        {
            if (xp_table.Count > lev)
                return xp_table[lev];
            else
                generateTable(lev);



            return xp_table[lev];

        }

        public void generateTable(int lev)
        {

            for (int i = xp_table.Count; i <= lev; i++)
            {
                int exp = getXPByLev(i);

                xp_table.Add(exp);
            }
        }

        public int getXPByLev(int i)
        {
            if (xp_table.Count > i)
                return xp_table[i];

            //LE.Monitor.Log($"LE XP_MOD before calcs: {xp_mod}");

            if (i < 45)
                return getXPByLev(i - 1) + 300 + (int)Math.Round(1000 * i * xp_mod);
            else
                return getXPByLev(i - 1) + 300 + (int)Math.Round(((i * i * i * 0.5)) * xp_mod);
        }

        public int getLevByXP()
        {
            int l = 0;
            if (xp_table.Count > 0 && xp > xp_table.Max())
                l = xp_table.Count - 1;

            for (int i = 0; i < xp_table.Count; i++)
            {
            
                if (xp <= xp_table[i])
                {
                    l = i;
                    break;
                }
            }
            //LE.Monitor.Log($"LE, lvlbyxp: {name}, key: {key}, found lev: {l}");
            return l;
        }

        /* private int GetDefStartXP()
         {
             if (key > 4)
                 return 0;

             int exp;

             if (level == 0)
             {
                 exp = 0;
             }
             else if (level > 0 && level < 11)
             {
                 exp = LE.defReqXPs[level - 1];
             }
             else
             {
                 exp = 15000;
             }


             return exp;
         }

         public int StartXP()
         {
             int xp = 0;

             if ()

             if ((level > 0 && level < 10) && key < 5)
             {
                 xp = LE.defReqXPs[level - 1];
             }

             return xp;

         }
         */

    }

}
