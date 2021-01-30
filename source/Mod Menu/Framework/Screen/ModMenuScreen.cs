/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Enaium-StardewValleyMods/ModMenu
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using EnaiumToolKit.Framework.Screen;
using EnaiumToolKit.Framework.Screen.Components;
using EnaiumToolKit.Framework.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;

namespace ModMenu.Framework.Screen
{
    public class ModMenuScreen : GuiScreen
    {
        private Slot<ModInfoSlot> _slot;
        private Button _websiteButton;

        protected override void Init()
        {
            var mods = new List<IModInfo>();
            mods.AddRange(ModEntry.GetInstance().Helper.ModRegistry.GetAll());
            mods.Sort((m1, m2) => FontUtils.GetWidth(m2.Manifest.Name) - FontUtils.GetWidth(m1.Manifest.Name));

            _slot = new Slot<ModInfoSlot>("", "", 10, 50, FontUtils.GetWidth(mods[0].Manifest.Name),
                (Game1.viewport.Height - 100) / 80 * 80, 80);


            foreach (var variable in ModEntry.GetInstance().Helper.ModRegistry.GetAll())
            {
                _slot.AddEntry(new ModInfoSlot(variable));
            }

            _slot.SelectedEntry = _slot.Entries[0];

            _websiteButton = new Button(GetTranslation("button.website"), "", Game1.viewport.Width - _slot.X - 220,
                _slot.Height - 80, 200, 80);
            AddComponent(_websiteButton);
            AddComponent(_slot);
            base.Init();
        }

        private readonly IDictionary<string, string> _modUrls =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["GitHub"] = "https://github.com/{0}/releases",
                ["Chucklefish"] = "https://community.playstarbound.com/resources/{0}",
                ["ModDrop"] = "https://www.moddrop.com/stardew-valley/mods/{0}",
                ["Nexus"] = "https://www.nexusmods.com/stardewvalley/mods/{0}"
            };

        public override void draw(SpriteBatch b)
        {
            var background = new Rectangle(_slot.X - 10, _slot.Y - 10, Game1.viewport.Width - _slot.X + 10,
                _slot.Height + 20);
            Render2DUtils.DrawBound(b, background.X, background.Y, background.Width, background.Height, Color.White);
            if (_slot.SelectedEntry != null)
            {
                var texts = new List<string>
                {
                    $"{GetTranslation("name")}:{_slot.SelectedEntry.ModInfo.Manifest.Name}",
                    $"{GetTranslation("author")}:{_slot.SelectedEntry.ModInfo.Manifest.Author}",
                    $"{GetTranslation("version")}:{_slot.SelectedEntry.ModInfo.Manifest.Version}",
                    $"{GetTranslation("description")}:"
                };
                texts.AddRange(Game1.parseText(_slot.SelectedEntry.ModInfo.Manifest.Description, Game1.dialogueFont,
                    background.Width - _slot.Width).Split('\n'));
                if (_slot.SelectedEntry.ModInfo.Manifest.Dependencies != null)
                {
                    texts.Add(
                        $"{GetTranslation("dependencies")}:{string.Join(",", _slot.SelectedEntry.ModInfo.Manifest.Dependencies.Select(element => element.UniqueID.Split('.').Last()))}"
                    );
                }

                var y = _slot.Y;
                foreach (var variable in texts)
                {
                    FontUtils.Draw(b, variable, _slot.X + _slot.Width, y);
                    y += FontUtils.GetHeight(variable);
                }
            }

            if (_slot.SelectedEntry != null)
            {
                var updateUrls = _slot.SelectedEntry.ModInfo.Manifest.UpdateKeys;
                if (updateUrls != null)
                {
                    var updateUrl = GetUpdateUrl(updateUrls);
                    if (updateUrl != null)
                    {
                        _websiteButton.OnLeftClicked = () => { Process.Start(updateUrl); };
                        _websiteButton.Visibled = true;
                    }
                    else
                    {
                        _websiteButton.Visibled = false;
                    }
                }
                else
                {
                    _websiteButton.Visibled = false;
                }
            }

            base.draw(b);
        }

        private string GetUpdateUrl(IEnumerable<string> updateKeys)
        {
            foreach (var updateKey in updateKeys)
            {
                var strArray = updateKey.Split(':');
                if (strArray.Length != 2)
                    return null;
                var key = strArray[0].ToLower().Trim();
                var str = strArray[1].ToLower().Trim();
                return _modUrls.TryGetValue(key, out var format) ? string.Format(format, str) : null;
            }

            return null;
        }

        private string GetTranslation(string key)
        {
            return ModEntry.GetInstance().Helper.Translation.Get("modMenu." + key);
        }

        private class ModInfoSlot : Slot<ModInfoSlot>.Entry
        {
            public IModInfo ModInfo;

            public ModInfoSlot(IModInfo modInfo)
            {
                ModInfo = modInfo;
            }

            public override void Render(SpriteBatch b, int x, int y)
            {
                Hovered = Render2DUtils.IsHovered(Game1.getMouseX(), Game1.getMouseY(), x, y, Width, Height);
                FontUtils.Draw(b, ModInfo.Manifest.Name, x + 15, y + 10);
                var desc = Game1.parseText(ModInfo.Manifest.Description, Game1.smallFont, Width - 15).Split('\n')[0];

                Utility.drawTextWithShadow(b, desc, Game1.smallFont,
                    new Vector2(x + 15, y + 10 + 30), Game1.textColor, 1f,
                    -1f,
                    -1, -1, 0.0f);
            }
        }
    }
}