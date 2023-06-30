/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TMThong/Stardew-Mods
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using StardewValley.Quests;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.Xna.Framework;
using StardewModdingAPI;

namespace FindObjectMod
{
    public static class Utilities
    {
        internal static List<OptionsElement> options(this OptionsPage p)
        {
            bool isAndroid = Utilities.IsAndroid;
            if (isAndroid)
            {
                List<OptionsElement> o = p.GetPrivateFieldValue<List<OptionsElement>>("options");
                bool flag = o != null;
                if (flag)
                {
                    return o;
                }
            }
            return p.options;
        }


        public static void DrawRectangle(this Rectangle rectangle, SpriteBatch b, Color color, bool empty = false, bool left = true, bool right = true, bool top = true, bool bottom = true)
        {
            bool flag = !empty;
            if (flag)
            {
                for (int i = rectangle.Y; i < rectangle.Y + rectangle.Height; i++)
                {
                    Utility.drawLineWithScreenCoordinates(rectangle.X, i, rectangle.X + rectangle.Width, i, b, color, 1f);
                }
            }
            else
            {
                if (top)
                {
                    rectangle.DrawTop(b, color);
                }
                if (bottom)
                {
                    rectangle.DrawBottom(b, color);
                }
                if (left)
                {
                    rectangle.DrawLeft(b, color);
                }
                if (right)
                {
                    rectangle.DrawRight(b, color);
                }
            }
        }

        public static void DrawTop(this Rectangle rectangle, SpriteBatch b, Color color)
        {
            Utility.drawLineWithScreenCoordinates(rectangle.X, rectangle.Y, rectangle.X + rectangle.Width, rectangle.Y, b, color, 1f);
        }

        public static void DrawBottom(this Rectangle rectangle, SpriteBatch b, Color color)
        {
            Utility.drawLineWithScreenCoordinates(rectangle.X, rectangle.Y + rectangle.Height, rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height, b, color, 1f);
        }
        public static void DrawLeft(this Rectangle rectangle, SpriteBatch b, Color color)
        {
            Utility.drawLineWithScreenCoordinates(rectangle.X, rectangle.Y, rectangle.X, rectangle.Y + rectangle.Height, b, color, 1f);
        }

        
        public static void DrawRight(this Rectangle rectangle, SpriteBatch b, Color color)
        {
            Utility.drawLineWithScreenCoordinates(rectangle.X + rectangle.Width, rectangle.Y, rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height, b, color, 1f);
        }

         
        public static void DrawArea(List<StardewValley.Object> objects_, SpriteBatch b, Color color)
        {
            IEnumerable<StardewValley.Object> objects = from p in objects_
                                                        where Utilities.CanDrawArea(p)
                                                        select p;
            foreach (StardewValley.Object o in objects)
            {
                Utilities.DrawArea(o, objects.ToArray<StardewValley.Object>(), b, color);
            }
        }

        
        public static void DrawArea(StardewValley.Object o, StardewValley.Object[] objects, SpriteBatch b, Color color)
        {
            bool flag = objects.ToList<StardewValley.Object>().Any((StardewValley.Object p) => p.name != o.name);
            if (flag)
            {
                Vector2 i = o.getLocalPosition(Game1.viewport);
                Rectangle rectangle = new Rectangle((int)i.X, (int)i.Y, 64, 64);
                rectangle.DrawRectangle(b, color * 0.3f, false, true, true, true, true);
                rectangle.DrawRectangle(b, color, true, true, true, true, true);
            }
            else
            {
                Vector2 j = o.getLocalPosition(Game1.viewport);
                Rectangle rectangle2 = new Rectangle((int)j.X, (int)j.Y, 64, 64);
                rectangle2.DrawRectangle(b, color * 0.3f, false, true, true, true, true);
                rectangle2.DrawRectangle(b, color, true, !Utilities.Left(o, objects.ToList<StardewValley.Object>()), !Utilities.Right(o, objects.ToList<StardewValley.Object>()), !Utilities.Top(o, objects.ToList<StardewValley.Object>()), !Utilities.Bottom(o, objects.ToList<StardewValley.Object>()));
            }
        }

        
        public static bool Left(StardewValley.Object One, StardewValley.Object Two)
        {
            return One.tileLocation.X - 1f == Two.tileLocation.X && One.name == Two.name;
        }

         
        public static bool Right(StardewValley.Object One, StardewValley.Object Two)
        {
            return One.tileLocation.X + 1f == Two.tileLocation.X && One.name == Two.name;
        }

         
        public static bool Top(StardewValley.Object One, StardewValley.Object Two)
        {
            return One.tileLocation.Y - 1f == Two.tileLocation.Y && One.name == Two.name;
        }

        
        public static bool Bottom(StardewValley.Object One, StardewValley.Object Two)
        {
            return One.tileLocation.Y + 1f == Two.tileLocation.Y && One.name == Two.name;
        }

        
        public static bool Left(StardewValley.Object One, List<StardewValley.Object> Two)
        {
            return Two.Any((StardewValley.Object p) => Utilities.Left(One, p));
        }

        
        public static bool Right(StardewValley.Object One, List<StardewValley.Object> Two)
        {
            return Two.Any((StardewValley.Object p) => Utilities.Right(One, p));
        }

         
        public static bool Top(StardewValley.Object One, List<StardewValley.Object> Two)
        {
            return Two.Any((StardewValley.Object p) => Utilities.Top(One, p));
        }
         
        public static bool Bottom(StardewValley.Object One, List<StardewValley.Object> Two)
        {
            return Two.Any((StardewValley.Object p) => Utilities.Bottom(One, p));
        }

        
        public static void DrawArea(NPC npc, SpriteBatch b, Color color)
        {
            Vector2 i = Game1.GlobalToLocal(Game1.viewport, npc.Position);
            Rectangle rectangle = new Rectangle((int)i.X, (int)i.Y, 64, 64);
            rectangle.DrawRectangle(b, color * 0.3f, false, true, true, true, true);
            rectangle.DrawRectangle(b, color, true, true, true, true, true);
        }

        
        public static bool CanDrawArea(StardewValley.Object gameObject)
        {
            int width = Game1.viewport.Width;
            int height = Game1.viewport.Height;
            Vector2 i = gameObject.getLocalPosition(Game1.viewport);
            bool flag = i.X > (float)width || i.Y > (float)height || i.X + 64f < 0f || i.Y + 64f < 0f;
            return !flag;
        }

       
        public static bool CanDrawArea(NPC npc)
        {
            int width = Game1.viewport.Width;
            int height = Game1.viewport.Height;
            Vector2 i = Game1.GlobalToLocal(Game1.viewport, npc.Position);
            bool flag = i.X > (float)width || i.Y > (float)height || i.X + 64f < 0f || i.Y + 64f < 0f;
            return !flag;
        }

        
        public static bool isQuestObject(StardewValley.Object gameObject)
        {
            return Game1.player.questLog.Any(delegate (Quest p)
            {
                List<int> types = new List<int>();
                types.AddRange(p.questType);
                bool flag = types.Any((int t) => t == 3);
                if (flag)
                {
                    bool flag2 = p.currentObjective == gameObject.name;
                    if (flag2)
                    {
                        return true;
                    }
                }
                return false;
            });
        }

        
        public static bool HasQuestObject(StardewValley.Object[] gameObjects)
        {
            return gameObjects.ToList<StardewValley.Object>().Any((StardewValley.Object p) => Utilities.isQuestObject(p));
        }

         
        public static bool isObjectCanFind(ModConfig config, StardewValley.Object gameObject)
        {
            return config.ObjectToFind[Utilities.SaveKey].ContainsKey(gameObject.Name);
        }

        
        public static bool HasObjectCanFind(ModConfig config, StardewValley.Object[] gameObjects)
        {
            return gameObjects.ToList<StardewValley.Object>().Any((StardewValley.Object p) => Utilities.isObjectCanFind(config, p));
        }

         
        public static bool isNpcCanFind(ModConfig config, NPC nPC)
        {
            return config.FindCharacter[Utilities.SaveKey].Keys.ToList<string>().Any((string p) => p == nPC.name);
        }

         
        public static bool HasNpcCanFind(ModConfig config, NPC[] nPCs)
        {
            return nPCs.ToList<NPC>().Any((NPC p) => Utilities.isNpcCanFind(config, p));
        }

         
        public static bool isClick(int x, int y, StardewValley.Object gameObject)
        {
            Vector2 i = gameObject.getLocalPosition(Game1.viewport);
            Rectangle rectangle = new Rectangle((int)i.X, (int)i.Y, 64, 64);
            return rectangle.Contains(x, y);
        }

        
        public static bool isClick(int x, int y, StardewValley.Object[] gameObjects)
        {
            return gameObjects.ToList<StardewValley.Object>().Any((StardewValley.Object p) => Utilities.isClick(x, y, p));
        }

         
        public static StardewValley.Object[] GetObjects(GameLocation location = null)
        {
            List<StardewValley.Object> gameObj = new List<StardewValley.Object>();
            GameLocation i = (location != null) ? location : Game1.currentLocation;
            foreach (Vector2 a in i.objects.Keys)
            {
                gameObj.Add(i.objects[a]);
            }
            return gameObj.ToArray();
        }

        
        public static List<NPC> GetNpcs(GameLocation location = null)
        {
            GameLocation i = (location != null) ? location : Game1.player.currentLocation;
            List<NPC> npcs = new List<NPC>();
            npcs.AddRange(i.characters);
            return npcs;
        }

        
        public static bool isClick(int x, int y, NPC nPC)
        {
            Vector2 i = Game1.GlobalToLocal(Game1.viewport, nPC.Position);
            Rectangle rectangle = new Rectangle((int)i.X, (int)i.Y - 64, 64, 128);
            return rectangle.Contains(x, y);
        }

         
        public static bool isClick(int x, int y, NPC[] npcs)
        {
            return npcs.ToList<NPC>().Any((NPC p) => Utilities.isClick(x, y, p));
        }

        
        public static bool IsAndroid
        {
            get
            {
                return Constants.TargetPlatform == GamePlatform.Android;
            }
        }

        
        public static void InitializationScrollBoxAndroid(this OptionsPage page, int x, int y, int width, int height)
        {
            bool flag = !Utilities.IsAndroid;
            if (!flag)
            {
                page.SetPrivateFieldValue("newScrollbar", Utilities.GetTypeStardewValley("StardewValley.Menus.MobileScrollbar").CreateInstance<object>(new object[]
                {
                    x + width - 24 - 32,
                    y + 16,
                    24,
                    height - 32,
                    16,
                    16,
                    false
                }));
                int num = 50;
                int num2 = height - 16;
                page.SetPrivateFieldValue("scrollArea", Utilities.GetTypeStardewValley("StardewValley.Menus.MobileScrollbox").CreateInstance<object>(new object[]
                {
                    x,
                    y,
                    width,
                    num2,
                    page.GetPrivatePropertyValue<int>("ContentHeight") - num2 + num,
                    new Rectangle(16, y + 16, Game1.viewport.Width - 32, height - 32),
                    page.GetPrivateFieldValue<object>("newScrollbar")
                }));
            }
        }
        #region Reflection
        public static T GetPrivatePropertyValue<T>(this object obj, string propName)
        {
            if (obj == null) throw new ArgumentNullException("obj"); PropertyInfo pi = obj.GetType().GetProperty(propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (pi == null) throw new ArgumentOutOfRangeException("propName", string.Format("Property {0} was not found in Type {1}", propName, obj.GetType().FullName));
            return (T)pi.GetValue(obj, null);
        }
        public static T GetPrivateFieldValue<T>(this object obj, string propName)
        {
            if (obj == null) throw new ArgumentNullException("obj");
            Type t = obj.GetType(); FieldInfo fi = null; while (fi == null && t != null) { fi = t.GetField(propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance); t = t.BaseType; }
            if (fi == null) throw new ArgumentOutOfRangeException("propName", string.Format("Field {0} was not found in Type {1}", propName, obj.GetType().FullName)); return (T)fi.GetValue(obj);
        }

        public static void SetPrivatePropertyValue<T>(this object obj, string propName, T val)
        {
            Type t = obj.GetType();
            if (t.GetProperty(propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance) == null) throw new ArgumentOutOfRangeException("propName", string.Format("Property {0} was not found in Type {1}", propName, obj.GetType().FullName)); t.InvokeMember(propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.SetProperty | BindingFlags.Instance, null, obj, new object[] { val });
        }

        public static void SetPrivateFieldValue<T>(this object obj, string propName, T val)
        {
            if (obj == null) throw new ArgumentNullException("obj");
            Type t = obj.GetType(); FieldInfo fi = null; while (fi == null && t != null) { fi = t.GetField(propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance); t = t.BaseType; }
            if (fi == null) throw new ArgumentOutOfRangeException("propName", string.Format("Field {0} was not found in Type {1}", propName, obj.GetType().FullName)); fi.SetValue(obj, val);
        }
        public static T GetPrivateMethod<T>(this Type type, string name, BindingFlags bindingFlags, object[] obj)
        {
            var method = type.GetMethod(name, bindingFlags);
            return (T)method.Invoke(null, obj);
        }

        public static T GetPrivateMethodStatic<T>(this Type type, string name, object[] obj) => GetPrivateMethod<T>(type, name, BindingFlags.Static | BindingFlags.NonPublic, obj);
        public static T CreateInstanceProtected<T>(this Type type, object[] obj)
        {
            ConstructorInfo constructor = type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, obj.ToTypes(), null);
            object o = constructor.Invoke(obj);
            if (o is T t)
            {
                return t;
            }
            return default(T);
        }
        public static T CreateInstanceInternal<T>(this Type type, object[] obj)
        {
            object o = Activator.CreateInstance(type, obj);
            if (o is T t)
            {
                return t;
            }
            return default(T);
        }
        public static T CreateInstance<T>(this Type type, object[] obj)
        {
            ConstructorInfo constructor = type.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, obj.ToTypes(), null);
            object o = constructor.Invoke(obj);
            if (o is T t)
            {
                return t;
            }
            return default(T);
        }
        public static T CreateInstance<T>(this Type type, object[] obj, Type[] types)
        {
            ConstructorInfo constructor = type.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, types, null);
            object o = constructor.Invoke(obj);
            if (o is T t)
            {
                return t;
            }
            return default(T);
        }


        public static Type[] ToTypes<T>(this T[] value)
        {
            List<Type> types = new List<Type>();
            for (int i = 0; i < value.Length; i++)
            {
                types.Add(value[i].GetType());
            }
            return types.ToArray();
        }


        #endregion

        public static Type GetTypeStardewValley(string t)
        {
            return typeof(Game1).Assembly.GetType(t);
        }

        internal static string SaveKey;

        internal static List<OptionsElement> OptionsElements = new List<OptionsElement>();
    }
}
