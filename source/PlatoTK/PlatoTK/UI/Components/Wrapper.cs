/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/PlatoTK
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PlatoTK.UI.Styles;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PlatoTK.UI.Components
{
    public class Wrapper : Component, IWrapper
    {
        protected Dictionary<string, object> Data { get; } = new Dictionary<string, object>();
        protected Dictionary<string, List<StyleDefinition>> StyleDefinitionsTable { get; } = new Dictionary<string, List<StyleDefinition>>();

        public override string ComponentName => "Wrapper";

        public virtual bool WasLeftMouseDown { get; protected set; } = false;
        public virtual bool IsLeftMouseDown { get; protected set; } = false;

        public virtual bool WasRightMouseDown { get; protected set; } = false;
        public virtual bool IsRightMouseDown { get; protected set; } = false;


        public virtual Point LastMousePosition { get; protected set; } = Point.Zero;
        public virtual Point CurrentMousePosition { get; protected set; } = Point.Zero;

        public virtual int LastMouseWheelState { get; protected set; } = 0;
        public virtual int CurrentMouseWheelState { get; protected set; } = 0;

        public bool TryLoadText(string value, IComponent component, out string text)
        {
            bool data = value.ToLower().StartsWith("data>");
            bool call = !data && value.ToLower().StartsWith("call>");
            bool i18n = !data && !call && value.ToLower().StartsWith("i18n>");
            text = value;

            if (!data && !call && !i18n)
                return true;

            if (data)
            {
                value = value.Substring("data>".Length);
                if (TryGet(value, out string t))
                {
                    text = t;
                    return true;
                }
            }
            else if (call)
            {
                value = value.Substring("call>".Length);
                if (TryCall<string>(value, out string t))
                {
                    text = t;
                    return true;
                }
            }
            else
            {
                if (i18n)
                    value = value.Substring("i18n>".Length);

                text = value;

                if (Helper.ModHelper.Translation.Get(value) is Translation t && t.HasValue())
                {
                    text = t;
                    return true;
                }
            }

            return false;
        }

        public bool TryLoadTexture(string value, IComponent component, out Texture2D texture)
       {
            bool data = value.ToLower().StartsWith("data>");
            bool call = !data && value.ToLower().StartsWith("call>");
            bool content = !data && !call && value.ToLower().StartsWith("content>");
            texture = null;

            if (data)
            {
                value = value.Substring("data>".Length);
                if (TryGet(value, out Texture2D t))
                    texture = t;
            }
            else if (call)
            {
                value = value.Substring("call>".Length);
                if (TryCall<Texture2D>(value, out Texture2D t))
                    texture = t;
            }
            else
            {
                if (content)
                    value = value.Substring("content>".Length);

                texture = Helper.ModHelper.Content.Load<Texture2D>(value, content ? StardewModdingAPI.ContentSource.GameContent : StardewModdingAPI.ContentSource.ModFolder);
            }

            return texture is Texture;
        }

        public bool TryParseIntValue(string value, int relative, IComponent component, out int intValue)
        {
            value = value.Trim();
            intValue = int.MaxValue;

            foreach (string s in value.Split(' ').Select(p => p.Trim()))
            {
                bool data = s.ToLower().StartsWith("data>");
                bool call = !data && s.ToLower().StartsWith("call>");
                string v = s;

                if (data)
                {
                    value = value.Substring("data>".Length);
                    if (TryGet(value, out string ds))
                        v = ds;
                    else if (TryGet(value, out int i))
                        v = i.ToString();
                }
                else if (call)
                {
                    value = value.Substring("call>".Length);
                    if (TryCall<string>(value, out string ds))
                        v = ds;
                    else if (TryCall<int>(value, out int di))
                        v = di.ToString();
                }

                if (v.EndsWith("%") && float.TryParse(v.Substring(0, v.Length - 1).Trim(), out float fvalue))
                    intValue = (int)((fvalue / 100f) * relative);
                else if (v.Contains('x') && v.Split('x') is string[] frac && frac.Length > 1 && float.TryParse(frac[0].Trim(), out float m) && float.TryParse(frac[1].Trim(), out float d))
                    intValue = (int)(m * (relative / d));
                else if (int.TryParse(v.Trim(), out int ivalue))
                    intValue = ivalue;

                intValue = intValue >= 0 ? intValue : relative + intValue;

                if (intValue != int.MaxValue)
                    relative = intValue;
            }


            return intValue != int.MaxValue;
        }

        public virtual bool TryGet<T>(string key, out T value)
        {
            if (Data.ContainsKey(key) && Data[key] is T tvalue)
            {
                value = tvalue;
                return true;
            }

            value = default;
            return false;
        }

        public virtual bool TryCall(string key, IComponent component)
        {
            if (Data.ContainsKey(key) && Data[key] is Delegate dValue)
            {
                dValue.DynamicInvoke(component);
                return true;
            }

            return false;
        }

        public virtual bool TryCall<T>(string key, out T value)
        {
            if (Data.ContainsKey(key) && Data[key] is Delegate dValue && dValue.DynamicInvoke() is T dv)
            {
                value = dv;
                return true;
            }

            value = default;
            return false;
        }

        public virtual bool TryCall<T>(string key, out T value, IComponent component)
        {
            if (Data.ContainsKey(key) && Data[key] is Delegate dValue && dValue.DynamicInvoke(component) is T dv)
            {
                value = dv;
                return true;
            }

            value = default;
            return false;
        }

        public override void UpdateAbsoluteBounds()
        {
            AbsoluteBounds = new Rectangle(0, 0, Game1.viewport.Width, Game1.viewport.Height);
        }

        public override void Compose(IComponent parent)
        {
            UpdateAbsoluteBounds();
            Bounds = AbsoluteBounds;
         
            foreach (IComponent child in Children)
                child.Compose(this);
        }

        public override void Update(IComponent parent)
        {
            WasLeftMouseDown = IsLeftMouseDown;
            IsLeftMouseDown = Helper.ModHelper.Input.IsDown(SButton.MouseLeft);

            WasRightMouseDown = IsRightMouseDown;
            IsRightMouseDown = Helper.ModHelper.Input.IsDown(SButton.MouseRight);

            LastMousePosition = CurrentMousePosition; 
            var pos = Helper.ModHelper.Input.GetCursorPosition().ScreenPixels;
            CurrentMousePosition = new Point((int)pos.X, (int)pos.Y);

            var mouse = Mouse.GetState();
            LastMouseWheelState = CurrentMouseWheelState;
            CurrentMouseWheelState = mouse.ScrollWheelValue;

            bool recompose = ShouldRecompose;
                ShouldRecompose = false;

                if (recompose)
                    Compose(Parent);

                foreach (IComponent child in Children)
                {
                    if (recompose)
                        child.Recompose();

                    child.Update(this);
                }

                
                if (ShouldRedraw || recompose)
                {
                    ComponentRenderTarget = null;
                    ShouldRedraw = false;
                }
        }

        public virtual void Set(string key, object value)
        {
            if (Data.ContainsKey(key))
                Data[key] = value;
            else
                Data.Add(key, value);
        }

        public Wrapper(IPlatoHelper helper)
            : base(helper)
        {
            Bounds = new Rectangle(0, 0, Game1.viewport.Width, Game1.viewport.Height);
            DefaultBounds = Bounds;
            helper.ModHelper.Events.Display.WindowResized += Display_WindowResized;
        }

        private void Display_WindowResized(object sender, StardewModdingAPI.Events.WindowResizedEventArgs e)
        {
            ShouldRecompose = true;
        }

        public override void Setup()
        {
            base.Setup();
            Bounds = new Rectangle(0, 0, Game1.viewport.Width, Game1.viewport.Height);
        }

        public override void Dispose()
        {
            Helper.ModHelper.Events.Display.WindowResized -= Display_WindowResized;
            base.Dispose();
        }

        public IEnumerable<StyleDefinition> GetStyleDefinitions(string[] tags)
        {
            foreach(string tag in tags)
                if (StyleDefinitionsTable.ContainsKey(tag))
                    foreach (var sd in StyleDefinitionsTable[tag])
                        yield return sd;
        }

       
    }
}
