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


        public static void Initialize(Mod mod)
        {
            Mod = mod;
            Monitor = mod.Monitor;
            Helper = mod.Helper;

            var harmony = new Harmony(mod.ModManifest.UniqueID);

            harmony.Patch(
               original: AccessTools.Method(typeof(LoadGameMenu.SaveFileSlot), nameof(LoadGameMenu.SaveFileSlot.Draw), new[] { typeof(SpriteBatch), typeof(int) }),
               postfix: new HarmonyMethod(typeof(_LoadGameMenu), nameof(_LoadGameMenu.draw_Postfix))
            );

            harmony.Patch(
               original: AccessTools.Constructor(typeof(LoadGameMenu)),
               postfix: new HarmonyMethod(typeof(_LoadGameMenu), nameof(_LoadGameMenu.LoadGameMenu_Postfix))
            );
        }

        public static void draw_Postfix(LoadGameMenu.SaveFileSlot __instance, SpriteBatch b, int i)
        {
            drawFarmTypeIcon(__instance, b, i);
        }

        private static void drawFarmTypeIcon(LoadGameMenu.SaveFileSlot __instance, SpriteBatch b, int i)
        {
            if (__instance.Farmer.slotName == null) //Happens when you join someones game and are asked to select a character. Haven't though of a cleaner way yet
                return;

            int currentItemIndex = (int)_currentItemIndex.GetValue(LoadGameMenuParent) + i;

            Texture2D icon = getFarmTypeIcon(__instance.Farmer.slotName, currentItemIndex);
            Rectangle bounds = LoadGameMenuParent.slotButtons[i].bounds;

            if (icon == null)
                return;

            if (__instance.GetType().Name == "SaveFileSlot") //SaveFileSlot = Load, HostFileSlot = Host
                b.Draw(icon, new Rectangle(bounds.X + 20, bounds.Y + 78, 72, 80), Color.White);
            else
                b.Draw(icon, new Rectangle(bounds.X + 12, bounds.Y + 12, 54, 60), Color.White);
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
                icon = Helper.GameContent.Load<Texture2D>(farm.IconTexture);
            } catch (Exception) { Monitor.LogOnce($"Unable to load icon asset '{farm.IconTexture}' for farm {whichFarm}"); };

            CachedFarmTypeIcons.Add(whichFarm, icon);
            return icon;
        }

        private static long _old = 0;
        private static long _new = 0;
        private static string getFarmType(string saveFile, int i)
        {
            if (CachedFarmTypes.ContainsKey(i))
                return CachedFarmTypes[i];

            //Explanation: The FarmType is not part of the small SaveGameInfo, but instead the massive general purpose save file
            //In order to display the farm type icon it is required to read the whichFarm xml node.
            //Parsing those 2.5-5 mb large save data files to read that node is very draining on the performance
            //I noticed that the whichFarm node is somewhere in the last ~7k characters.
            //I leave 2x leeway and gamble that I can skip the first 3-5 million characters

            string fullFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "StardewValley", "Saves", saveFile, saveFile);
            string whichFarm = "";
            char chr;

            var fileInfo = new FileInfo(fullFilePath);

            char[] target = "<whichFarm>".ToCharArray();
            int k = 0;

            using (var stream = File.OpenRead(fullFilePath)) {
                stream.Seek(fileInfo.Length - 15000, SeekOrigin.Begin);

                using (StreamReader sr = new StreamReader(stream))
                    while (sr.Peek() >= 0) {
                        chr = (char)sr.Read();

                        if (k != 11)
                            if (chr == target[k])
                                k++;
                            else
                                k = 0;

                        else {
                            if (chr == '<')
                                break;
                            whichFarm += chr;
                        }
                    }
            }

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
    }
}