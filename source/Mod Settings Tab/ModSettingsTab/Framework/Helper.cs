using System;
using System.Timers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace ModSettingsTab.Framework
{
    public static class Helper
    {
        private static IModHelper _helper;

        public static void Init(IModHelper helper, IMonitor monitor)
        {
            _helper = helper;
            Console = monitor;
            Events = _helper.Events;
            Input = _helper.Input;
            Data = _helper.Data;
            Content = _helper.Content;
            ModRegistry = _helper.ModRegistry;
            I18N = _helper.Translation;
            DirectoryPath = _helper.DirectoryPath;
        }

        public static string DirectoryPath { get; private set; }
        public static ITranslationHelper I18N { get; private set; }
        public static IContentHelper Content { get; private set; }
        public static IDataHelper Data { get; private set; }
        public static IInputHelper Input { get; private set; }
        public static IModEvents Events { get; private set; }
        public static IMonitor Console { get; private set; }
        public static IModRegistry ModRegistry { get; private set; }

        public static void Error(this IMonitor console, object message)
        {
            console.Log(message.ToString(), LogLevel.Error);
        }

        public static void Info(this IMonitor console, object message)
        {
            console.Log(message.ToString(), LogLevel.Info);
        }

        public static void Warn(this IMonitor console, object message)
        {
            console.Log(message.ToString(), LogLevel.Warn);
        }

        public static void Alert(this IMonitor console, object message)
        {
            console.Log(message.ToString(), LogLevel.Alert);
        }

        public static IReflectedField<T> GetReflectedField<T>(object obj, string fieldName)
        {
            return _helper.Reflection.GetField<T>(obj, fieldName);
        }

        public static IReflectedMethod GetReflectedMethod(object obj, string methodName)
        {
            return _helper.Reflection.GetMethod(obj, methodName);
        }

        public static void DrawTextureBox(
            SpriteBatch b,
            Texture2D texture,
            Rectangle sourceRect,
            int x,
            int y,
            int width,
            int height,
            Color color,
            float scale = 1f,
            bool drawShadow = true,
            float layerDepth = 0.76f)
        {
            var num = sourceRect.Width / 3;
            if (drawShadow)
            {
                b.Draw(texture,
                    new Vector2(x + width - (int) (num * scale) - 8, y + 8),
                    new Rectangle(sourceRect.X + num * 2, sourceRect.Y, num, num), Color.Black * 0.4f,
                    0.0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth - 0.0001f);
                b.Draw(texture,
                    new Vector2(x - 8, y + height - (int) (num * scale) + 8),
                    new Rectangle(sourceRect.X, num * 2 + sourceRect.Y, num, num), Color.Black * 0.4f,
                    0.0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth - 0.0001f);
                b.Draw(texture,
                    new Vector2(x + width - (int) (num * scale) - 8,
                        y + height - (int) (num * scale) + 8),
                    new Rectangle(sourceRect.X + num * 2, num * 2 + sourceRect.Y, num, num),
                    Color.Black * 0.4f, 0.0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth - 0.0001f);
                b.Draw(texture,
                    new Rectangle(x + (int) (num * scale) - 8, y + 8,
                        width - (int) (num * scale) * 2, (int) (num * scale)),
                    new Rectangle(sourceRect.X + num, sourceRect.Y, num, num), Color.Black * 0.4f, 0.0f,
                    Vector2.Zero, SpriteEffects.None, layerDepth - 0.0001f);
                b.Draw(texture,
                    new Rectangle(x + (int) (num * scale) - 8,
                        y + height - (int) (num * scale) + 8,
                        width - (int) (num * scale) * 2, (int) (num * scale)),
                    new Rectangle(sourceRect.X + num, num * 2 + sourceRect.Y, num, num),
                    Color.Black * 0.4f, 0.0f, Vector2.Zero, SpriteEffects.None, layerDepth - 0.0001f);
                b.Draw(texture,
                    new Rectangle(x - 8, y + (int) (num * scale) + 8,
                        (int) (num * scale), height - (int) (num * scale) * 2),
                    new Rectangle(sourceRect.X, num + sourceRect.Y, num, num), Color.Black * 0.4f, 0.0f,
                    Vector2.Zero, SpriteEffects.None, layerDepth - 0.0001f);
                b.Draw(texture,
                    new Rectangle(x + width - (int) (num * scale) - 8,
                        y + (int) (num * scale) + 8, (int) (num * scale),
                        height - (int) (num * scale) * 2),
                    new Rectangle(sourceRect.X + num * 2, num + sourceRect.Y, num, num),
                    Color.Black * 0.4f, 0.0f, Vector2.Zero, SpriteEffects.None, layerDepth - 0.0001f);
                b.Draw(texture,
                    new Rectangle((int) (num * scale / 2.0) + x - 8,
                        (int) (num * scale / 2.0) + y + 8,
                        width - (int) (num * scale), height - (int) (num * scale)),
                    new Rectangle(num + sourceRect.X, num + sourceRect.Y, num, num), Color.Black * 0.4f,
                    0.0f, Vector2.Zero, SpriteEffects.None, layerDepth - 0.0001f);
            }

            b.Draw(texture,
                new Rectangle((int) (num * scale) + x, (int) (num * scale) + y,
                    width - (int) (num * scale * 2.0),
                    height - (int) (num * scale * 2.0)),
                new Rectangle(num + sourceRect.X, num + sourceRect.Y, num, num), color, 0.0f,
                Vector2.Zero, SpriteEffects.None, layerDepth);
            b.Draw(texture, new Vector2(x, y),
                new Rectangle(sourceRect.X, sourceRect.Y, num, num), color, 0.0f, Vector2.Zero, scale,
                SpriteEffects.None, layerDepth);
            b.Draw(texture, new Vector2(x + width - (int) (num * scale), y),
                new Rectangle(sourceRect.X + num * 2, sourceRect.Y, num, num), color, 0.0f,
                Vector2.Zero, scale, SpriteEffects.None,
                layerDepth);
            b.Draw(texture, new Vector2(x, y + height - (int) (num * scale)),
                new Rectangle(sourceRect.X, num * 2 + sourceRect.Y, num, num), color, 0.0f,
                Vector2.Zero, scale, SpriteEffects.None,
                layerDepth);
            b.Draw(texture,
                new Vector2(x + width - (int) (num * scale),
                    y + height - (int) (num * scale)),
                new Rectangle(sourceRect.X + num * 2, num * 2 + sourceRect.Y, num, num), color, 0.0f,
                Vector2.Zero, scale, SpriteEffects.None,
                layerDepth);
            b.Draw(texture,
                new Rectangle(x + (int) (num * scale), y,
                    width - (int) (num * scale) * 2, (int) (num * scale)),
                new Rectangle(sourceRect.X + num, sourceRect.Y, num, num), color, 0.0f, Vector2.Zero,
                SpriteEffects.None, layerDepth);
            b.Draw(texture,
                new Rectangle(x + (int) (num * scale),
                    y + height - (int) (num * scale),
                    width - (int) (num * scale) * 2, (int) (num * scale)),
                new Rectangle(sourceRect.X + num, num * 2 + sourceRect.Y, num, num), color, 0.0f,
                Vector2.Zero, SpriteEffects.None, layerDepth);
            b.Draw(texture,
                new Rectangle(x, y + (int) (num * scale), (int) (num * scale),
                    height - (int) (num * scale) * 2),
                new Rectangle(sourceRect.X, num + sourceRect.Y, num, num), color, 0.0f, Vector2.Zero,
                SpriteEffects.None, layerDepth);
            b.Draw(texture,
                new Rectangle(x + width - (int) (num * scale),
                    y + (int) (num * scale), (int) (num * scale),
                    height - (int) (num * scale) * 2),
                new Rectangle(sourceRect.X + num * 2, num + sourceRect.Y, num, num), color, 0.0f,
                Vector2.Zero, SpriteEffects.None, layerDepth);
        }

        public static T ReadConfig<T>() where T : class, new()
        {
            return _helper.ReadConfig<T>();
        }

        public static void Reset(this Timer timer)
        {
            timer.Stop();
            timer.Start();
        }
        public static string Get(this JObject obj, string fieldName)
        {
            var jToken = obj.GetValue(fieldName, StringComparison.InvariantCultureIgnoreCase);
            return jToken == null ? "" : jToken.Value<string>();
        }
    }
}