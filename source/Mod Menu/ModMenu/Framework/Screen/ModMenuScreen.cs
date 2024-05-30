/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Enaium-StardewValleyMods/ModMenu
**
*************************************************/

using System.Diagnostics;
using System.Reflection;
using EnaiumToolKit.Framework.Extensions;
using EnaiumToolKit.Framework.Screen;
using EnaiumToolKit.Framework.Screen.Components;
using EnaiumToolKit.Framework.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModMenu.Framework.Entity;
using Newtonsoft.Json;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace ModMenu.Framework.Screen;

public class ModMenuScreen : GuiScreen
{
    private Slot<ModInfoSlot> _slot = null!;
    private Button _updateButton = null!;
    private Button _settingButton = null!;
    private Button _homePageButton = null!;
    private Button _issuesButton = null!;

    protected override void Init()
    {
        var mods = new List<IModInfo>();
        mods.AddRange(ModEntry.GetInstance().Helper.ModRegistry.GetAll());
        mods.Sort((m1, m2) => m2.Manifest.Name.Length - m1.Manifest.Name.Length);

        var measureString = Game1.dialogueFont.MeasureString(mods[0].Manifest.Name);
        var slotHeight = (int)(measureString.Y * 2);
        _slot = new Slot<ModInfoSlot>("", "", 10, 50,
            (int)measureString.X + 30,
            (Game1.graphics.GraphicsDevice.Viewport.Height - 100) / slotHeight * slotHeight, slotHeight);


        foreach (var variable in ModEntry.GetInstance().Helper.ModRegistry.GetAll())
        {
            ModMenuEntity? modMenuEntity = null;

            if (variable.Manifest.ExtraFields.TryGetValue("Custom", out var field))
            {
                var custom =
                    JsonConvert.DeserializeObject<CustomEntity>(field.ToString()!);
                if (custom != null)
                {
                    modMenuEntity = custom.ModMenu;
                }
            }


            _slot.AddEntry(new ModInfoSlot(variable, modMenuEntity));
        }

        _slot.SelectedEntry = _slot.Entries[0];

        _homePageButton = new Button(GetTranslation("button.homePage"), "",
            Game1.graphics.GraphicsDevice.Viewport.Width - _slot.X - 220,
            _slot.Height - 200, 200, 80);
        _issuesButton = new Button(GetTranslation("button.issues"), "",
            Game1.graphics.GraphicsDevice.Viewport.Width - _slot.X - 440,
            _slot.Height - 200, 200, 80);
        _updateButton = new Button(GetTranslation("button.update"), "",
            Game1.graphics.GraphicsDevice.Viewport.Width - _slot.X - 220,
            _slot.Height - 100, 200, 80);
        _settingButton = new Button(GetTranslation("button.setting"), "",
            Game1.graphics.GraphicsDevice.Viewport.Width - _slot.X - 440,
            _slot.Height - 100, 200, 80);
        AddComponentRange(_homePageButton, _issuesButton, _updateButton, _settingButton, _slot);
        base.Init();
    }

    private readonly IDictionary<string, string> _modUrls =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["GitHub"] = "https://github.com/{0}/releases",
            ["Chucklefish"] = "https://community.playstarbound.com/resources/{0}",
            ["ModDrop"] = "https://www.moddrop.com/stardew-valley/mods/{0}",
            ["Nexus"] = "https://www.nexusmods.com/stardewvalley/mods/{0}",
            ["CurseForge"] = "https://www.curseforge.com/projects/{0}"
        };

    public override void draw(SpriteBatch b)
    {
        var background = new Rectangle(_slot.X - 10, _slot.Y - 10,
            Game1.graphics.GraphicsDevice.Viewport.Width - _slot.X + 10,
            _slot.Height + 20);
        b.DrawWindowTexture(background.X, background.Y, background.Width, background.Height, Color.White);
        if (_slot.SelectedEntry != null)
        {
            var texts = new List<string>
            {
                $"{GetTranslation("name")}:{_slot.SelectedEntry.ModInfo.Manifest.Name}",
                $"{GetTranslation("author")}:{_slot.SelectedEntry.ModInfo.Manifest.Author}",
                $"{GetTranslation("version")}:{_slot.SelectedEntry.ModInfo.Manifest.Version}",
                $"{GetTranslation("description")}:{_slot.SelectedEntry.ModInfo.Manifest.Description}"
            };
            if (_slot.SelectedEntry.ModInfo.Manifest.Dependencies.Length > 0)
            {
                texts.Add(
                    $"{GetTranslation("dependencies")}:{string.Join(",", _slot.SelectedEntry.ModInfo.Manifest.Dependencies.Select(element => element.UniqueID.Split('.').Last()))}"
                );
            }

            var y = _slot.Y;
            foreach (var variable in texts)
            {
                b.DrawString(Game1.parseText(variable, Game1.dialogueFont, background.Width - _slot.Width),
                    _slot.X + _slot.Width, y);
                y += (int)Game1.dialogueFont.MeasureString(variable).Y;
            }
        }

        if (_slot.SelectedEntry != null)
        {
            var updateUrls = _slot.SelectedEntry.ModInfo.Manifest.UpdateKeys;
            if (updateUrls.Length > 0)
            {
                var updateUrl = GetPageUrl(updateUrls);
                if (updateUrl != null)
                {
                    _updateButton.OnLeftClicked = () =>
                    {
                        Process.Start(new ProcessStartInfo(updateUrl)
                        {
                            UseShellExecute = true
                        });
                    };
                    _updateButton.Visibled = true;
                }
                else
                {
                    _updateButton.Visibled = false;
                }
            }
            else
            {
                _updateButton.Visibled = false;
            }

            var selectedEntryModMenu = _slot.SelectedEntry?.ModMenu;

            if (selectedEntryModMenu != null)
            {
                if (selectedEntryModMenu.Setting != null)
                {
                    _settingButton.OnLeftClicked = () =>
                    {
                        var type = AppDomain.CurrentDomain.GetAssemblies()
                            .Where(a =>
                            {
                                try
                                {
                                    return a.GetTypes().Any();
                                }
                                catch (ReflectionTypeLoadException)
                                {
                                    return false;
                                }
                            })
                            .SelectMany(t => t.GetTypes()).Where(t =>
                                t.IsClass && selectedEntryModMenu.Setting.Equals(t.FullName)).ToArray();

                        if (type.Length == 1)
                        {
                            if (Activator.CreateInstance(type[0]) is IClickableMenu menu)
                            {
                                OpenScreenGui(menu);
                            }
                        }
                        else
                        {
                            ModEntry.GetInstance().Monitor.Log("Not Found Setting:" + selectedEntryModMenu.Setting,
                                LogLevel.Error);
                        }
                    };
                    _settingButton.Visibled = true;
                }
                else
                {
                    _settingButton.Visibled = false;
                }

                if (selectedEntryModMenu.Contact != null)
                {
                    if (selectedEntryModMenu.Contact.HomePage != null)
                    {
                        _homePageButton.OnLeftClicked = () =>
                        {
                            Process.Start(new ProcessStartInfo(selectedEntryModMenu.Contact.HomePage)
                            {
                                UseShellExecute = true
                            });
                        };
                        _homePageButton.Visibled = true;
                    }
                    else
                    {
                        _homePageButton.Visibled = false;
                    }


                    if (selectedEntryModMenu.Contact.Issues != null)
                    {
                        _issuesButton.OnLeftClicked = () =>
                        {
                            Process.Start(new ProcessStartInfo(selectedEntryModMenu.Contact.Issues)
                            {
                                UseShellExecute = true
                            });
                        };
                        _issuesButton.Visibled = true;
                    }
                    else
                    {
                        _issuesButton.Visibled = false;
                    }
                }
                else
                {
                    _homePageButton.Visibled = false;
                    _issuesButton.Visibled = false;
                }
            }
            else
            {
                _settingButton.Visibled = false;
                _homePageButton.Visibled = false;
                _issuesButton.Visibled = false;
            }
        }

        base.draw(b);
    }

    private string? GetPageUrl(IEnumerable<string> updateKeys)
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
        public readonly IModInfo ModInfo;
        public readonly ModMenuEntity? ModMenu;

        public ModInfoSlot(IModInfo modInfo, ModMenuEntity? modMenu)
        {
            ModInfo = modInfo;
            ModMenu = modMenu;
        }

        public override void Render(SpriteBatch b, int x, int y)
        {
            Hovered = new Rectangle(x, y, Width, Height).Contains(Game1.getMouseX(), Game1.getMouseY());
            b.DrawString(ModInfo.Manifest.Name, new Vector2(x + 15, y + 10));
            var desc = Game1.parseText(ModInfo.Manifest.Description, Game1.smallFont, Width - 30).Split('\n')[0];
            b.DrawString(desc, new Vector2(x + 15, y + Height - Game1.smallFont.MeasureString(desc).Y - 10),
                font: Game1.smallFont);
        }
    }
}