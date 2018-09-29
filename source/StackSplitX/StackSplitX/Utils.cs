using System;
using System.Diagnostics;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using Microsoft.Xna.Framework.Input;

namespace StackSplitX
{
    public static class LogExtensions
    {
        public static void Log(this IMonitor monitor, bool condition, string message, LogLevel level = LogLevel.Trace)
        {
            if (condition)
                monitor.Log(message, level);
        }

        public static void DebugLog(this IMonitor monitor, string message, LogLevel level = LogLevel.Trace)
        {
        #if DEBUG
            monitor.Log(message, level);
        #endif // DEBUG
        }
    }

    public class Utils
    {
        public static string ArrayToString<T>(T[] array)
        {
            string s = "";
            int i = 0;
            foreach (T item in array)
            {
                s += item.ToString() + ((++i < array.Length) ? ", " : "");
            }
            return s;
        }

        public static T[] ConcatArrays<T>(T[] a, T[] b)
        {
            T[] c = new T[a.Length + b.Length];
            Array.Copy(a, c, a.Length);
            Array.Copy(b, c, b.Length);
            return c;
        }

        public static int[] StringToIntArray(string[] array, int defaultVal=0)
        {
            int[] output = new int[array.Length];
            for (int i = 0; i < array.Length; ++i)
            {
                try
                {
                    output[i] = Int32.Parse(array[i]);
                }
                catch (Exception)
                {
                    output[i] = defaultVal;
                }
            }
            return output;
        }

        public static T GetNativeField<T, Instance>(Instance instance, string fieldName)
        {
            FieldInfo fieldInfo = typeof(Instance).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            return (T)fieldInfo.GetValue(instance);
        }

        public static FieldInfo[] GetNativeFields<T>()
        {
            return typeof(T).GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
        }

        public static FieldInfo GetNativeFieldInfoByName<T>(string name)
        {
            return typeof(T).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic);
        }

        public static bool IsType<T>(object o)
        {
            if (o != null)
            {
                return (o.GetType() == typeof(T));
            }
            return false;
        }

        public static int GetTileSheetIndexFromID(int id)
        {
            if (id == 0)
                return 0;

            const int spriteSize = 16; // each sprite on this sheet is 16x16
            int x = (int)Math.Floor((float)(id / 24.0f));
            int y = id % spriteSize;
            return (y * spriteSize) + x;
        }

        public static int Clamp(int val, int min, int max)
        {
            return Math.Max(Math.Min(val, max), min);
        }

        public static bool IsAnyKeyDown(KeyboardState state, Keys[] keys)
        {
            foreach (var key in keys)
            {
                if (state.IsKeyDown(key))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsKeyDown(KeyboardState state, Keys key)
        {
            return state.IsKeyDown(key);
        }

        public static bool WasPressedThisFrame(KeyboardState prior, KeyboardState current, Keys key)
        {
            return (prior.IsKeyUp(key) && current.IsKeyDown(key));
        }

        public static bool WasPressedThisFrame(ButtonState prior, ButtonState current)
        {
            //return (current == ButtonState.Released && prior == ButtonState.Pressed);
            return (current == ButtonState.Pressed && prior == ButtonState.Released);
        }
    }
}
