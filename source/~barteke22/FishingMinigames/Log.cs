/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/barteke22/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using System.Collections;

namespace FishingMinigames
{
    class Log
    {
        public static IMonitor Monitor;

        public static void Error(params object[] texts)
        {
            Monitor.Log(MakeLog(texts), LogLevel.Error);
        }
        public static void ErrorOnce(params object[] texts)
        {
            Monitor.LogOnce(MakeLog(texts), LogLevel.Error);
        }
        public static void Alert(params object[] texts)
        {
            Monitor.Log(MakeLog(texts), LogLevel.Alert);
        }
        public static void AlertOnce(params object[] texts)
        {
            Monitor.LogOnce(MakeLog(texts), LogLevel.Alert);
        }


        private static string MakeLog(params object[] texts)
        {
            string s = "";
            if (texts?.Length > 0)
            {
                s += Convert(texts[0]);
                for (int i = 1; i < texts.Length; i++)
                {
                    s += " | " + Convert(texts[i]);
                }
            }
            return s;
        }
        private static string Convert(object text)
        {
            string s = "";
            if (text == null) return "null";
            //else if (text is Vector2 vector) return vector.X + "," + vector.Y;//V2 has a ToString()
            else if (text is Point p) return p.X + "," + p.Y;
            else if (text is Rectangle r) return r.X + "," + r.Y + " ," + r.Width + "," + r.Height;
            else if (text is IList list)
            {
                if (list?.Count > 0)
                {
                    s += list[0];
                    for (int i = 1; i < list.Count; i++)
                    {
                        s += ", " + list[i];
                    }
                }
            }
            else if (text is IDictionary dic)
            {
                if (dic?.Count > 0)
                {
                    foreach (var item in dic.Keys)
                    {
                        s += ", " + item + ":" + dic[item];
                    }
                    s = s.Remove(0, 2);
                }
            }
            else return text.ToString();
            return s;
        }




        public static Texture2D ColorizeTexture2D(Texture2D texture, Color newColor, Rectangle? area = null, Color? oldColor = null, int oldThreshold = 0)
        {
            float tol = oldThreshold / 100f;
            int width = texture.Width;
            int height = texture.Height;

            int aX = 0, aY = 0, aW = 0, aH = 0;
            if (area != null)
            {
                aX = area.Value.X;
                aY = area.Value.Y;
                aW = area.Value.Width;
                aH = area.Value.Height;
            }

            Color[] tcolor = new Color[width * height];
            texture.GetData<Color>(tcolor);

            for (int i = 0; i < tcolor.Length; i++)
            {
                if (tcolor[i].A > 0)
                {
                    if (area != null)//within specific area
                    {
                        int x = i % width;
                        if (!(i > aY * width && x > aX && x < aX + aW && i < (aY + aH) * width)) continue;
                    }
                    if (oldColor != null)//replace specific colour within a treshold
                    {
                        int rgb1 = tcolor[i].R + tcolor[i].G + tcolor[i].B;
                        int rgb2 = oldColor.Value.R + oldColor.Value.G + oldColor.Value.B;

                        if (rgb1 > rgb2 * (1f + tol) || rgb1 < rgb2 * (1f - tol)) continue;
                    }

                    tcolor[i].R = (byte)((tcolor[i].R) * newColor.R / 255);
                    tcolor[i].G = (byte)((tcolor[i].G) * newColor.G / 255);
                    tcolor[i].B = (byte)((tcolor[i].B) * newColor.B / 255);
                }
            }

            Texture2D t2 = new Texture2D(Game1.graphics.GraphicsDevice, texture.Width, texture.Height);//if you want to edit the imported source directly, there's no need to copy and return it - just setData on texture
            t2.SetData<Color>(tcolor);
            return t2;
        }
    }
}
