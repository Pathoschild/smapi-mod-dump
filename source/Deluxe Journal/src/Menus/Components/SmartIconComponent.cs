/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData.Characters;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.Tools;
using DeluxeJournal.Task;
using DeluxeJournal.Task.Tasks;
using DeluxeJournal.Framework.Data;

using static StardewValley.Menus.ClickableComponent;

namespace DeluxeJournal.Menus.Components
{
    public class SmartIconComponent
    {
        /// <summary>Measure of the inner icon source in pixels.</summary>
        private const int InnerIconPixels = 9;

        /// <summary>Measure of the outer/background icon source in pixels.</summary>
        private const int OuterIconPixels = 14;

        /// <summary>Measure of the border width on the background icon in pixels.</summary>
        private const int BorderPixels = 2;

        /// <summary>Width of the strip of pixels to test for the top of an NPC's head.</summary>
        private const int HeadDetectionSampleWidth = 2;

        /// <summary>Size of the source rect to capture the NPC's head.</summary>
        private const int HeadSize = 12;

        internal static Dictionary<string, int>? AnimalIconIds;
        internal static Dictionary<string, BuildingIconData>? BuildingIconData;

        private readonly static IReadOnlyList<SmartIconFlags> OrderedIconFlags = Enum.GetValues(typeof(SmartIconFlags)).Cast<SmartIconFlags>().Reverse().ToList();

        private readonly ClickableComponent? _taskTypeIcon;
        private readonly IList<ClickableComponent> _taskTargetIcons;
        private readonly TaskParser _taskParser;
        private readonly SmartIconFlags _mask;
        private Rectangle _bounds;
        private bool _visible;

        private Texture2D? _npcSpriteSheet;
        private Rectangle _npcSourceRect;
        private string _loadedNpcName;

        public Point Location
        {
            get => _bounds.Location;

            set
            {
                Point translate = value - _bounds.Location;

                if (translate == Point.Zero)
                {
                    return;
                }

                _bounds.Location = value;

                if (_taskTypeIcon != null)
                {
                    _taskTypeIcon.bounds.Location += translate;
                }

                foreach (var target in _taskTargetIcons)
                {
                    target.bounds.Location += translate;
                }
            }
        }

        public bool Visible
        {
            get => _visible;

            set
            {
                if (_visible && !(_visible = value))
                {
                    foreach (var target in _taskTargetIcons)
                    {
                        target.visible = false;
                    }
                }

                if (_taskTypeIcon != null)
                {
                    _taskTypeIcon.visible = value;
                }
            }
        }

        /// <summary>Automatic vertical snapping constructor.</summary>
        public SmartIconComponent(Rectangle bounds, TaskParser taskParser, int myId, SmartIconFlags mask = SmartIconFlags.All, int maxTargets = 2, bool showType = true)
            : this(bounds, taskParser, myId, SNAP_AUTOMATIC, SNAP_AUTOMATIC, mask, maxTargets, showType)
        {
        }

        public SmartIconComponent(Rectangle bounds, TaskParser taskParser, int myId, int upNeighborId, int downNeighborId, SmartIconFlags mask = SmartIconFlags.All, int maxTargets = 2, bool showType = true)
        {
            _taskTargetIcons = new List<ClickableComponent>();
            _taskParser = taskParser;
            _mask = mask;
            _bounds = bounds;
            _visible = true;
            _loadedNpcName = string.Empty;

            if (showType)
            {
                _taskTypeIcon = new ClickableComponent(bounds, "smart_type")
                {
                    myID = myId,
                    upNeighborID = upNeighborId,
                    downNeighborID = downNeighborId,
                    rightNeighborID = SNAP_AUTOMATIC,
                    leftNeighborID = SNAP_AUTOMATIC,
                    fullyImmutable = true,
                    visible = false
                };

                bounds.X += bounds.Width + 4;
            }
            else
            {
                myId -= 1;
            }

            for (int i = 1; i <= Math.Min(maxTargets, OrderedIconFlags.Count); i++)
            {
                _taskTargetIcons.Add(new ClickableComponent(bounds, $"smart_target_{i}")
                {
                    myID = myId + i,
                    upNeighborID = upNeighborId,
                    downNeighborID = downNeighborId,
                    rightNeighborID = SNAP_AUTOMATIC,
                    leftNeighborID = SNAP_AUTOMATIC,
                    fullyImmutable = true,
                    visible = false
                });

                bounds.X += bounds.Width + 4;
            }
        }

        public IEnumerable<ClickableComponent> GetClickableComponents()
        {
            if (_taskTypeIcon != null)
            {
                yield return _taskTypeIcon;
            }

            foreach (var targetIcon in _taskTargetIcons)
            {
                yield return targetIcon;
            }
        }

        public bool TryGetHoverText(int x, int y, ITranslationHelper translation, out string text)
        {
            if (_taskTypeIcon?.containsPoint(x, y) == true)
            {
                text = translation.Get($"task.{_taskParser.ID}");
                return true;
            }

            SmartIconFlags flag = SmartIconFlags.All;

            foreach (ClickableComponent targetIcon in _taskTargetIcons)
            {
                if (targetIcon.containsPoint(x, y))
                {
                    if (ComparePriority(ref flag, SmartIconFlags.Npc, _mask) && _taskParser.ShouldShowSmartIcon(SmartIconFlags.Npc))
                    {
                        text = _taskParser.NpcDisplayName;
                    }
                    else if (ComparePriority(ref flag, SmartIconFlags.Animal, _mask) && _taskParser.ShouldShowSmartIcon(SmartIconFlags.Animal))
                    {
                        text = _taskParser.FarmAnimalDisplayName;
                    }
                    else if (ComparePriority(ref flag, SmartIconFlags.Building, _mask) && _taskParser.ShouldShowSmartIcon(SmartIconFlags.Building))
                    {
                        text = _taskParser.BuildingDisplayName;
                    }
                    else if (ComparePriority(ref flag, SmartIconFlags.Item, _mask) && _taskParser.ShouldShowSmartIcon(SmartIconFlags.Item))
                    {
                        text = _taskParser.ProxyItemDisplayName;
                    }
                    else
                    {
                        break;
                    }

                    return true;
                }

                flag = (SmartIconFlags)((int)flag >> 1);
            }

            text = string.Empty;
            return false;
        }

        public void Draw(SpriteBatch b, Color color, int quality = 0, bool showCount = true, bool shadow = false)
        {
            if (Visible = _taskParser.MatchFound())
            {
                SmartIconFlags flag = SmartIconFlags.All;
                int parsedCount = showCount && _taskParser.ShouldShowCount() ? _taskParser.Count : 0;
                int targetIconIndex = 0;

                if (_taskTypeIcon != null)
                {
                    TaskRegistry.GetTaskIcon(_taskParser.ID).DrawIcon(b, _taskTypeIcon.bounds, color);
                }

                for (; targetIconIndex < _taskTargetIcons.Count; targetIconIndex++)
                {
                    ClickableComponent targetIcon = _taskTargetIcons[targetIconIndex];

                    if (ComparePriority(ref flag, SmartIconFlags.Npc, _mask, true) && _taskParser.ShouldShowSmartIcon(SmartIconFlags.Npc))
                    {
                        string npcName = _taskParser.NpcName;

                        DrawIconBackground(b, targetIcon.bounds, 1, color, shadow);

                        if (LoadNpcSpriteSheet(npcName) is Texture2D texture)
                        {
                            b.Draw(texture, ConvertInnerBounds(targetIcon.bounds), _npcSourceRect, color);
                        }
                    }
                    else if (ComparePriority(ref flag, SmartIconFlags.Animal, _mask, true) && _taskParser.ShouldShowSmartIcon(SmartIconFlags.Animal))
                    {
                        DrawIconWithBackground(b,
                            DeluxeJournalMod.AnimalIconsTexture,
                            ConvertInnerBounds(targetIcon.bounds),
                            targetIcon.bounds,
                            AnimalIconIds?.GetValueOrDefault(_taskParser.FarmAnimals?.FirstOrDefault() ?? string.Empty) ?? 0,
                            0,
                            color,
                            parsedCount,
                            shadow: shadow);
                    }
                    else if (ComparePriority(ref flag, SmartIconFlags.Building, _mask, true) && _taskParser.ShouldShowSmartIcon(SmartIconFlags.Building))
                    {
                        if (BuildingIconData?.GetValueOrDefault(_taskParser.BuildingType) is BuildingIconData buildingIconData)
                        {
                            DrawIcon(b,
                                DeluxeJournalMod.BuildingIconsTexture,
                                targetIcon.bounds,
                                buildingIconData.SpriteIndex,
                                OuterIconPixels,
                                color,
                                parsedCount,
                                buildingIconData.Tier,
                                shadow);
                        }
                        else
                        {
                            DrawIconBackground(b, targetIcon.bounds, 2, color, shadow);
                        }
                    }
                    else if (ComparePriority(ref flag, SmartIconFlags.Item, _mask, true) && _taskParser.ShouldShowSmartIcon(SmartIconFlags.Item) && _taskParser.ProxyItem is Item item)
                    {
                        DrawIconBackground(b, targetIcon.bounds, 1, color, shadow);
                        item.drawInMenu(b,
                            new Vector2(targetIcon.bounds.X - 4, targetIcon.bounds.Y - (item is WateringCan ? -4 : 4)),
                            0.75f, 1.0f, 0.9f,
                            StackDrawType.Hide,
                            color,
                            false);

                        DrawQuality(b, quality, targetIcon.bounds, color);

                        if (item is not Ring || _taskParser.ID == TaskTypes.Buy || _taskParser.ID == TaskTypes.Sell)
                        {
                            DrawDigits(b, parsedCount, targetIcon.bounds, color);
                        }
                    }
                    else
                    {
                        break;
                    }

                    targetIcon.visible = true;
                }

                for (;  targetIconIndex < _taskTargetIcons.Count; targetIconIndex++)
                {
                    _taskTargetIcons[targetIconIndex].visible = false;
                }
            }
        }

        /// <summary>Load and cache the sprite sheet of an NPC along with the corresponding icon source rect.</summary>
        /// <param name="npcName">Name of the NPC to load.</param>
        private Texture2D? LoadNpcSpriteSheet(string npcName)
        {
            if (npcName != _loadedNpcName)
            {
                string textureName = $"Characters\\{NPC.getTextureNameForCharacter(npcName)}";

                if (Game1.content.DoesAssetExist<Texture2D>(textureName) && Game1.characterData.TryGetValue(npcName, out var data))
                {
                    _npcSpriteSheet = Game1.content.Load<Texture2D>(textureName);
                    _npcSourceRect = GetNpcSourceRect(_npcSpriteSheet, data.Size.X, data.Size.Y);
                }
                else
                {
                    _npcSpriteSheet = null;
                }

                _loadedNpcName = npcName;
            }

            return _npcSpriteSheet;
        }

        /// <summary>Get the source rect for an NPC portrait icon.</summary>
        /// <param name="texture">The sprite sheet texture.</param>
        /// <param name="spriteWidth">The character sprite width.</param>
        /// <param name="spriteHeight">The character sprite height.</param>
        private static Rectangle GetNpcSourceRect(Texture2D texture, int spriteWidth, int spriteHeight)
        {
            int sampleWidth = Math.Min(HeadDetectionSampleWidth, spriteWidth);
            int size = sampleWidth * spriteHeight;
            Rectangle region = new(Math.Max((spriteWidth - sampleWidth) / 2, 0), 0, sampleWidth, spriteHeight);
            Color[] pixels = new Color[size];

            texture.GetData(0, region, pixels, 0, size);

            for (int i = 0; i < size; i++)
            {
                if (pixels[i].PackedValue > 0)
                {
                    int headY = Math.Min(i / sampleWidth, Math.Max(spriteHeight - HeadSize, 0));
                    int hairOffset = (headY + HeadSize + 2) > spriteHeight ? 0 : 2;
                    return new Rectangle(Math.Max(spriteWidth - HeadSize, 0) / 2, headY + hairOffset, HeadSize, HeadSize);
                }
            }

            return new(0, 0, HeadSize, HeadSize);
        }

        private static void DrawIcon(SpriteBatch b, Texture2D? texture, Rectangle bounds, int iconIndex, int iconSize, Color color, int count = -1, int tier = -1, bool shadow = false)
        {
            if (texture != null)
            {
                Rectangle source = new Rectangle(
                    (iconIndex * iconSize) % texture.Width,
                    (iconIndex * iconSize / texture.Width) * iconSize,
                    iconSize,
                    iconSize);

                if (shadow)
                {
                    Vector2 position = new(bounds.X + bounds.Width / 2, bounds.Y + bounds.Height / 2);
                    Vector2 origin = new(source.Width / 2, source.Height / 2);

                    Utility.drawWithShadow(b, texture, position, source, color, 0, origin, 4f);
                }
                else
                {
                    b.Draw(texture, bounds, source, color);
                }

                DrawDigits(b, count, bounds, color);
                DrawBadge(b, tier, bounds, color);
            }
        }

        private static void DrawIconWithBackground(SpriteBatch b, Texture2D? texture, Rectangle iconBounds, Rectangle backgroundBounds, int iconIndex, int backgroundIndex, Color color, int count = -1, int tier = -1, bool shadow = false)
        {
            DrawIconBackground(b, backgroundBounds, backgroundIndex, color, shadow);
            DrawIcon(b, texture, iconBounds, iconIndex, InnerIconPixels, color);
            DrawDigits(b, count, backgroundBounds, color);
            DrawBadge(b, tier, backgroundBounds, color);
        }

        private static void DrawIconBackground(SpriteBatch b, Rectangle bounds, int iconIndex, Color color, bool shadow = false)
        {
            Rectangle source = new(iconIndex * OuterIconPixels, 110, OuterIconPixels, OuterIconPixels);

            if (shadow)
            {
                Vector2 position = new(bounds.X + bounds.Width / 2, bounds.Y + bounds.Height / 2);
                Vector2 origin = new(source.Width / 2, source.Height / 2);

                Utility.drawWithShadow(b, DeluxeJournalMod.UiTexture, position, source, color, 0, origin, 4f);
            }
            else
            {
                b.Draw(DeluxeJournalMod.UiTexture, bounds, source, color);
            }
        }

        private static void DrawBadge(SpriteBatch b, int tier, Rectangle bounds, Color color)
        {
            if (tier > 0)
            {
                b.Draw(DeluxeJournalMod.UiTexture,
                    new Rectangle(bounds.X + bounds.Width - 22, bounds.Y + 6, 16, 20),
                    new Rectangle(64 + (tier - 1) * 8, 80, 8, 10),
                    color);
            }
        }

        private static void DrawQuality(SpriteBatch b, int quality, Rectangle bounds, Color color)
        {
            if (quality > 0)
            {
                b.Draw(Game1.mouseCursors,
                    new Rectangle(bounds.X + 2, bounds.Y + bounds.Height - 20, 16, 16),
                    quality <= 2 ? new Rectangle(338 + (quality - 1) * 8, 400, 8, 8) : new Rectangle(346, 392, 8, 8),
                    color);
            }
        }

        private static void DrawDigits(SpriteBatch b, int number, Rectangle bounds, Color color)
        {
            if (number > 1)
            {
                int digitWidth = GetWidthOfTinyDigitString(number, 2f);
                bool showMaxPlus = digitWidth > bounds.Width - 12f;

                if (showMaxPlus)
                {
                    number = 9999;
                }

                Vector2 digitPosition = new Vector2(
                    bounds.X + bounds.Width - GetWidthOfTinyDigitString(number, 2f) - 6f,
                    bounds.Y + bounds.Height - 20f);

                Utility.drawTinyDigits(number, b, digitPosition, 2f, 1f, color);

                if (showMaxPlus)
                {
                    b.Draw(DeluxeJournalMod.UiTexture,
                        new Rectangle((int)digitPosition.X - 10, (int)digitPosition.Y + 2, 10, 10),
                        new Rectangle(88, 80, 5, 5),
                        color);
                }
            }
        }

        /// <summary>
        /// Wrap <see cref="Utility.getWidthOfTinyDigitString"/> and correct for pixel drift bug
        /// in <see cref="Utility.drawTinyDigits"/>.
        /// </summary>
        private static int GetWidthOfTinyDigitString(int number, float scale)
        {
            int log10;

            if (number < 10)
            {
                log10 = 0;
            }
            else if (number < 100)
            {
                log10 = 1;
            }
            else if (number < 1000)
            {
                log10 = 2;
            }
            else
            {
                log10 = 3;
            }

            return Utility.getWidthOfTinyDigitString(number, scale) - log10;
        }

        /// <summary>
        /// Check if the <paramref name="left"/> flag is of equal or higher priority than the
        /// <paramref name="right"/> flag.
        /// </summary>
        /// <param name="left">Left operand.</param>
        /// <param name="right">Right operand.</param>
        /// <param name="mask">Mask to apply to the <paramref name="right"/> flag. Enables or disables icons.</param>
        /// <param name="shift">Reduce the priority of the <paramref name="left"/> flag if higher priority.</param>
        private static bool ComparePriority(ref SmartIconFlags left, SmartIconFlags right, SmartIconFlags mask = SmartIconFlags.All, bool shift = false)
        {
            if ((right & mask) == SmartIconFlags.None && (right | mask) != SmartIconFlags.None)
            {
                return false;
            }

            bool geq = left >= right;

            if (geq && shift)
            {
                left = (SmartIconFlags)((int)right >> 1);
            }

            return geq;
        }

        /// <summary>Convert the outer bounds of an icon into the inner bounds.</summary>
        /// <param name="bounds">Outer bounds.</param>
        private static Rectangle ConvertInnerBounds(Rectangle outerBounds)
        {
            return new Rectangle(
                outerBounds.X + BorderPixels * 4,
                outerBounds.Y + BorderPixels * 4,
                outerBounds.Width * (OuterIconPixels - BorderPixels * 2) / OuterIconPixels,
                outerBounds.Height * (OuterIconPixels - BorderPixels * 2) / OuterIconPixels);
        }
    }
}
