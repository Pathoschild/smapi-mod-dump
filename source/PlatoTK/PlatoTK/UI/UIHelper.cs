/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/PlatoTK
**
*************************************************/

using BmFont;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PlatoTK.UI.Components;
using PlatoTK.UI.Styles;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace PlatoTK.UI
{
    internal class UIHelper : InnerHelper, IUIHelper
    {
        private static HashSet<IStyle> StylesLoaded { get; } = new HashSet<IStyle>();
        private static HashSet<IComponent> ComponentsLoaded { get; } = new HashSet<IComponent>();

        public UIHelper(IPlatoHelper platoHelper)
            : base(platoHelper)
        {
            RegisterStyle(new BackgroundStyle(platoHelper));
            RegisterStyle(new PreRenderStyle(platoHelper));
            RegisterStyle(new BoundsStyle(platoHelper));
            RegisterStyle(new AlignementStyle(platoHelper));
            RegisterStyle(new GridStyle(platoHelper));
            RegisterStyle(new OnClickStyle(platoHelper));
            RegisterStyle(new ColorStyle(platoHelper));

            RegisterComponent(new Component(platoHelper));
            RegisterComponent(new Wrapper(platoHelper));
            RegisterComponent(new TextComponent(platoHelper));
        }

        public IWrapper LoadFromFile(string layoutPath, string id = "")
        {
            IWrapper wrapper = new Wrapper(Plato);
            wrapper.ParseAttribute("Id", id);
            string xml = File.ReadAllText(layoutPath);
            XDocument doc = XDocument.Parse(xml);
            foreach(var element in doc.Elements())
                ParseElements(wrapper, element);
            return wrapper;
        }

        public IClickableMenu OpenMenu(IWrapper wrapper)
        {
            return new UIMenu(Plato, wrapper);
        }

        private IComponent ParseElements(IComponent parent, XElement element)
        {
            if (Plato.UI.TryGetComponent(element.Name.LocalName, out IComponent component))
            {
                parent.AddChild(component);
                foreach (var attr in element.Attributes())
                    foreach(var attrEntry in attr.Value.Split(','))
                    component.ParseAttribute(attr.Name.LocalName, attrEntry.Trim());

                foreach (var child in element.Elements())
                    ParseElements(component, child);
            }

            return parent;
        }

        public void RegisterStyle(IStyle style)
        {
            if (!StylesLoaded.Contains(style))
                StylesLoaded.Add(style);
        }

        public void RegisterComponent(IComponent component)
        {
            if (!ComponentsLoaded.Contains(component))
                ComponentsLoaded.Add(component);
        }

        public bool TryGetStyle(string propertyName, out IStyle style, string option = "")
        {
            if (StylesLoaded.FirstOrDefault(s => s.PropertyNames.Any(p => p.ToLower() == propertyName.ToLower())) is IStyle loaded)
            {
                style = loaded.New(Plato, option);
                return true;
            }

            style = null;
            return false;
        }

        public bool TryGetComponent(string componentName, out IComponent component)
        {
            if (ComponentsLoaded.FirstOrDefault(s => s.ComponentName == componentName) is IComponent loaded)
            {
                component = loaded.New(Plato);
                return true;
            }

            component = null;
            return false;        
        }



        public List<Texture2D> LoadFontPages(FontFile fontFile, string assetName)
        {
            //return fontFile.Pages.Select(p => Plato.ModHelper.Content.Load<Texture2D>(p.File,StardewModdingAPI.ContentSource.GameContent)).ToList();
            return fontFile.Pages.Select(p => Plato.ModHelper.Content.Load<Texture2D>(Path.Combine(Path.GetDirectoryName(assetName), p.File))).ToList();
        }

        public FontFile LoadFontFile(string assetName)
        {
            //return Plato.ModHelper.Content.Load<FontFile>(assetName);//
            return FontLoader.Parse(File.ReadAllText(Path.Combine(Plato.ModHelper.DirectoryPath,assetName)));
        }

        public Dictionary<char, FontChar> ParseCharacterMap(FontFile fontFile)
        {
            return fontFile.Chars.ToDictionary(c => (char)c.ID, c => c);
        }

        public SpriteFont LoadSpriteFont(string assetName)
        {
            //return Plato.ModHelper.Content.Load<SpriteFont>(assetName);
            Texture2D texture = Plato.ModHelper.Content.Load<Texture2D>(Path.Combine(Path.GetDirectoryName(assetName), Path.GetFileNameWithoutExtension(assetName) + ".png"));
            SpriteFontData data = Plato.ModHelper.Content.Load<SpriteFontData>(assetName);
            object[] parameter = new object[]{
                    texture,
                    data.Glyphs.Values
                    .OrderBy(g => data.Characters.IndexOf(g.Character))
                    .Select(g => g.BoundsInTexture).ToList(),
                    data.Glyphs.Values
                    .OrderBy(g => data.Characters.IndexOf(g.Character))
                    .Select(g => g.Cropping).ToList(),
                    data.Characters,
                    data.LineSpacing,
                    data.Spacing,
                    data.Glyphs.Values
                    .OrderBy(g => data.Characters.IndexOf(g.Character))
                    .Select(g => new Vector3(g.LeftSideBearing, g.Width, g.RightSideBearing)).ToList(),
                    data.DefaultCharacter 
            };

            return (SpriteFont)AccessTools.Constructor(
                  typeof(SpriteFont), new[] {
                      typeof(Texture2D),
                      typeof(List<Rectangle>),
                      typeof(List<Rectangle>),
                      typeof(List<char>),
                      typeof(int),
                      typeof(float),
                      typeof(List<Vector3>),
                      typeof(char?)}
                ).Invoke(parameter);
        }

    }
}
