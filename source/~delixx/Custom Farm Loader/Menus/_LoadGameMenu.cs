/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley-custom-farm-loader
**
*************************************************/

using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using StardewValley;
using System.Xml;
using StardewModdingAPI.Events;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Reflection;
using Custom_Farm_Loader.Lib;
using StardewValley.Menus;
using StardewValley.GameData;
using System.Xml.Linq;
using System.IO;
using System.Text.RegularExpressions;

namespace Custom_Farm_Loader.Menus
{
    public class _LoadGameMenu
    {
        public static Mod Mod;
        private static IMonitor Monitor;
        private static IModHelper Helper;

        private static List<ModFarmType> ModFarms = new List<ModFarmType>();
        private static LoadGameMenu LoadGameMenuParent = null;
        private static FieldInfo _currentItemIndex = typeof(LoadGameMenu).GetField("currentItemIndex", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        private static Dictionary<int, string> CachedFarmTypes = new Dictionary<int, string>();
        private static Dictionary<string, Texture2D> CachedFarmTypeIcons = new Dictionary<string, Texture2D>();
        private static Dictionary<string, string> CachedFarmTypeNames = new Dictionary<string, string>();

        private static string GamepadHoverName = "";
        private static Texture2D MissingMapIcon = null;

        #region "FieldInfos"
        private static FieldInfo _ScrollbarRunnerField = typeof(LoadGameMenu).GetField("scrollBarRunner", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        private static FieldInfo _MenuSlots = typeof(LoadGameMenu).GetField("menuSlots", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        private static FieldInfo _CurrentItemIndex = typeof(LoadGameMenu).GetField("currentItemIndex", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        private static MethodInfo _SetScrollBarToCurrentIndex = typeof(LoadGameMenu).GetMethod("setScrollBarToCurrentIndex", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        #endregion
        public static void Initialize(Mod mod)
        {
            Mod = mod;
            Monitor = mod.Monitor;
            Helper = mod.Helper;
            MissingMapIcon = ModEntry._Helper.ModContent.Load<Texture2D>("assets/MissingMapIcon.png");

            var harmony = new Harmony(mod.ModManifest.UniqueID);

            harmony.Patch(
               original: AccessTools.Method(typeof(LoadGameMenu), nameof(LoadGameMenu.draw), new[] { typeof(SpriteBatch) }),
               prefix: new HarmonyMethod(typeof(_LoadGameMenu), nameof(_LoadGameMenu.draw_Prefix))
            );

            harmony.Patch(
               original: AccessTools.Method(typeof(LoadGameMenu), nameof(LoadGameMenu.draw), new[] { typeof(SpriteBatch) }),
               postfix: new HarmonyMethod(typeof(_LoadGameMenu), nameof(_LoadGameMenu.draw_Postfix))
            );

            harmony.Patch(
               original: AccessTools.Method(typeof(LoadGameMenu.SaveFileSlot), nameof(LoadGameMenu.SaveFileSlot.Draw), new[] { typeof(SpriteBatch), typeof(int) }),
               postfix: new HarmonyMethod(typeof(_LoadGameMenu), nameof(_LoadGameMenu.draw_SaveFileSlot_Postfix))
            );

            harmony.Patch(
               original: AccessTools.Constructor(typeof(LoadGameMenu)),
               postfix: new HarmonyMethod(typeof(_LoadGameMenu), nameof(_LoadGameMenu.LoadGameMenu_Postfix))
            );

            harmony.Patch(
               original: AccessTools.Method(typeof(LoadGameMenu), "deleteFile"),
               postfix: new HarmonyMethod(typeof(_LoadGameMenu), nameof(_LoadGameMenu.deleteFile_Postfix))
            );

            harmony.Patch(
               original: AccessTools.Method(typeof(LoadGameMenu), "setScrollBarToCurrentIndex"),
               prefix: new HarmonyMethod(typeof(_LoadGameMenu), nameof(_LoadGameMenu.setScrollBarToCurrentIndex_Prefix))
            );

            Helper.Events.Content.LocaleChanged += (s, e) => CachedFarmTypeNames.Clear();
        }

        public static bool setScrollBarToCurrentIndex_Prefix(LoadGameMenu __instance)
        {
            if (__instance is CoopMenu)
                return true;

            int menuSlotsCount = ((List<LoadGameMenu.MenuSlot>)_MenuSlots.GetValue(__instance)).Count();
            var scrollbarRunner = (Rectangle)_ScrollbarRunnerField.GetValue(__instance);
            var currentItemIndex = (int)_CurrentItemIndex.GetValue(__instance);

            if (menuSlotsCount > 0) {
                __instance.scrollBar.bounds.Y = (int)((float)(scrollbarRunner.Height - __instance.scrollBar.bounds.Height) / Math.Max(1, menuSlotsCount - 4) * currentItemIndex + __instance.upArrow.bounds.Bottom + 4);
                if (currentItemIndex == menuSlotsCount - 4) {
                    __instance.scrollBar.bounds.Y = __instance.downArrow.bounds.Y - __instance.scrollBar.bounds.Height - 4;
                }
            }

            return false;
        }

        public static bool draw_Prefix(LoadGameMenu __instance, SpriteBatch b)
        {
            GamepadHoverName = "";
            return true;
        }

        public static void draw_Postfix(LoadGameMenu __instance, SpriteBatch b)
        {
            if (GamepadHoverName != "") {
                int x = 84;
                int y = Game1.uiViewport.Height - 64;
                int w = 0;
                int h = 64;
                Utility.makeSafe(ref x, ref y, w, h);
                StardewValley.BellsAndWhistles.SpriteText.drawStringWithScrollBackground(b, GamepadHoverName, x, y);
            }

        }
        public static void draw_SaveFileSlot_Postfix(LoadGameMenu.SaveFileSlot __instance, SpriteBatch b, int i)
        {
            drawFarmTypeIcon(__instance, b, i);
        }

        private static void drawFarmTypeIcon(LoadGameMenu.SaveFileSlot __instance, SpriteBatch b, int i)
        {
            if (__instance.Farmer.slotName == null) //Happens when you join someones game and are asked to select a character.
                return;

            int currentItemIndex = (int)_currentItemIndex.GetValue(LoadGameMenuParent) + i;

            Texture2D icon = getFarmTypeIcon(__instance.Farmer.slotName, currentItemIndex);
            Rectangle bounds = LoadGameMenuParent.slotButtons[i].bounds;
            Rectangle iconBounds = __instance.GetType().Name == "SaveFileSlot" //SaveFileSlot = Load, HostFileSlot = Host
                ? new Rectangle(bounds.X + 20, bounds.Y + 78, (int)(72 * ModEntry.Config.LoadMenuIconScale), (int)(80 * ModEntry.Config.LoadMenuIconScale))
                : new Rectangle(bounds.X + 12, bounds.Y + 12, (int)(54 * ModEntry.Config.CoopMenuIconScale), (int)(60 * ModEntry.Config.CoopMenuIconScale));

            if (icon == null)
                icon = MissingMapIcon;

            b.Draw(icon, iconBounds, Color.White);

            if (Game1.options.SnappyMenus && bounds.Contains(Game1.input.GetMouseState().Position))
                GamepadHoverName = getFarmName(__instance.Farmer.slotName, currentItemIndex);
            else if (iconBounds.Contains(Game1.input.GetMouseState().Position))
                IClickableMenu.drawHoverText(b, getFarmName(__instance.Farmer.slotName, currentItemIndex), Game1.dialogueFont, 0, -80);
        }

        private static string getFarmName(string slotName, int i)
        {
            string whichFarm = getFarmType(slotName, i);

            if (CachedFarmTypeNames.ContainsKey(whichFarm))
                return CachedFarmTypeNames[whichFarm];

            string name = "";

            if (int.TryParse(whichFarm, out int id)) {
                name = id switch {
                    0 => Game1.content.LoadString("Strings\\UI:Character_FarmStandard"),
                    1 => Game1.content.LoadString("Strings\\UI:Character_FarmFishing"),
                    2 => Game1.content.LoadString("Strings\\UI:Character_FarmForaging"),
                    3 => Game1.content.LoadString("Strings\\UI:Character_FarmMining"),
                    4 => Game1.content.LoadString("Strings\\UI:Character_FarmCombat"),
                    5 => Game1.content.LoadString("Strings\\UI:Character_FarmFourCorners"),
                    6 => Game1.content.LoadString("Strings\\UI:Character_FarmBeach"),
                    _ => ""
                };

                name = name.Split("_").First();
                CachedFarmTypeNames.Add(whichFarm, name);
                return name;
            }

            var farm = CustomFarm.get(whichFarm);

            if (farm != null)
                name = farm.Name;
            else if (ModFarms.Exists(el => el.ID == whichFarm))
                name = ModFarms.Find(el => el.ID == whichFarm).MapName.Replace("_", " ");
            else if (new Regex(".+\\..+\\/.+").IsMatch(whichFarm)) //If it's a CFL type we take only the name part
                name = whichFarm.Split("/").Last();
            else
                name = whichFarm;

            CachedFarmTypeNames.Add(whichFarm, name);
            return name;
        }

        private static Texture2D getFarmTypeIcon(string slotName, int i)
        {

            string whichFarm = getFarmType(slotName, i);

            if (CachedFarmTypeIcons.ContainsKey(whichFarm))
                return CachedFarmTypeIcons[whichFarm];

            Texture2D icon = null;

            if (int.TryParse(whichFarm, out int id)) {
                icon = id switch {
                    0 => UtilityMisc.createSubTexture(Game1.mouseCursors, new Rectangle(2, 324, 18, 20)),
                    1 => UtilityMisc.createSubTexture(Game1.mouseCursors, new Rectangle(24, 324, 19, 20)),
                    2 => UtilityMisc.createSubTexture(Game1.mouseCursors, new Rectangle(46, 324, 18, 20)),
                    3 => UtilityMisc.createSubTexture(Game1.mouseCursors, new Rectangle(68, 324, 18, 20)),
                    4 => UtilityMisc.createSubTexture(Game1.mouseCursors, new Rectangle(90, 324, 18, 20)),
                    5 => UtilityMisc.createSubTexture(Game1.mouseCursors, new Rectangle(2, 345, 18, 20)),
                    6 => UtilityMisc.createSubTexture(Game1.mouseCursors, new Rectangle(24, 345, 18, 20)),
                    _ => null
                };

                CachedFarmTypeIcons.Add(whichFarm, icon);
                return icon;
            }

            var farm = ModFarms.Find(el => el.ID == whichFarm);

            if (farm == null || farm.IconTexture == "") {
                CachedFarmTypeIcons.Add(whichFarm, null);
                return null;
            }

            try {
                icon = CustomFarmSelection.loadCroppedIcon(farm);
            } catch (Exception) { Monitor.LogOnce($"Unable to load icon asset '{farm.IconTexture}' for farm {whichFarm}"); };

            CachedFarmTypeIcons.Add(whichFarm, icon);
            return icon;
        }
        private static string getFarmType(string saveFile, int i)
        {
            if (CachedFarmTypes.ContainsKey(i))
                return CachedFarmTypes[i];

            string whichFarm = FarmTypeCache.getFarmType(saveFile);

            CachedFarmTypes.Add(i, whichFarm);
            return whichFarm;
        }

        public static void LoadGameMenu_Postfix(LoadGameMenu __instance)
        {
            ModFarms = Game1.content.Load<List<ModFarmType>>("Data\\AdditionalFarms");
            LoadGameMenuParent = __instance;
            CachedFarmTypes = new Dictionary<int, string>();
            CustomFarm.getAll().ForEach(farm => farm.reloadTextures());
        }

        public static void deleteFile_Postfix(LoadGameMenu __instance, int which) => LoadGameMenu_Postfix(__instance);
    }
}