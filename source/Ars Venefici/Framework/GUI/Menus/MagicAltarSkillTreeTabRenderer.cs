/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/HeyImAmethyst/Ars-Venefici
**
*************************************************/

using ArsVenefici.Framework.Skill;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpaceCore;
using StardewValley;
using StardewValley.Menus;
using System;
using static SpaceCore.Skills;
using System.Collections.Generic;
using ArsVenefici.Framework.Util;
using StardewValley.Locations;
using System.Text;
using ArsVenefici.Framework.Spells.Components;
using static StardewValley.Minigames.CraneGame;
using System.Runtime.Intrinsics;
using Netcode;
using StardewValley.ItemTypeDefinitions;
using static System.Net.Mime.MediaTypeNames;
using StardewValley.BellsAndWhistles;
using StardewValley.Buffs;
using StardewValley.Enchantments;
using StardewValley.Objects;
using StardewValley.Tools;
using Object = StardewValley.Object;

namespace ArsVenefici.Framework.GUI.Menus
{
    public class MagicAltarSkillTreeTabRenderer : MagicAltarTabRenderer
    {
        private static readonly float SKILL_SIZE = 32f;
        private float SCALE = 1f;
        private int lastMouseX = 0;
        private int lastMouseY = 0;
        private float offsetX = 0;
        private float offsetY = 0;
        private SpellPartSkill hoverItem = null;
        bool isHoveringSkill = false;

        HashSet<SpellPartSkill> skills;
        Matrix transformMatrix;

        Dictionary<Rectangle, SpellPartSkill> spellPartSkillSpriteLocations = new Dictionary<Rectangle, SpellPartSkill>();

        StringBuilder learnedDescription = new StringBuilder();
        StringBuilder HoverTextStringBuilder = new StringBuilder();

        public MagicAltarSkillTreeTabRenderer(MagicAltarTab magicOrbTab, MagicAltarMenu parent) : base(magicOrbTab, parent)
        {
            //Init(magicOrbTab.GetWidth() / 2, magicOrbTab.GetHeight() / 2, parent.width, parent.height, parent.xPositionOnScreen, parent.yPositionOnScreen);
            Init(magicOrbTab.GetWidth() / 2, magicOrbTab.GetHeight() / 2, parent.width, parent.height, parent.xPositionOnScreen - 150, parent.yPositionOnScreen - 65);
        }

        protected override void Init()
        {
            SCALE = 2;

            //offsetX = (magicOrbTab.GetStartX() - width / 2f);
            offsetX = (magicAltarTab.GetStartX() - width / 2f) + 450;

            if (offsetX < 0)
                offsetX = 0;

            if (offsetX > textureWidth - width)
                offsetX = textureWidth - width;

            offsetY = magicAltarTab.GetStartY() - width / 2f;

            if (offsetY < 0)
                offsetY = 0;

            if (offsetY > textureHeight - height)
                offsetY = textureHeight - height;

            ModEntry modEntry = parent.modEntry;
            Farmer player = Game1.player;

            SpellPartSkillHelper helper = SpellPartSkillHelper.Instance();

            skills = modEntry.spellPartSkillManager.spellPartSkills.Values.Where(skill => skill.GetOcculusTab().Equals(magicAltarTab)).ToHashSet();
            skills.RemoveWhere(skill => skill.IsHidden() && !helper.Knows(modEntry, player, skill));

            foreach (SpellPartSkill skill in skills)
            {
                Rectangle rect = new Rectangle((int)skill.GetX(), (int)skill.GetY(), (int)SKILL_SIZE, (int)SKILL_SIZE);

                if (!spellPartSkillSpriteLocations.ContainsKey(rect))
                    spellPartSkillSpriteLocations.Add(rect, skill);
            }
        }

        protected override void RenderBg(SpriteBatch spriteBatch, int mouseX, int mouseY, float partialTicks)
        {
            float scaledOffsetX = offsetX * SCALE;
            float scaledOffsetY = offsetY * SCALE;

            float scaledWidth = width * (1 / SCALE);
            float scaledHeight = height * (1 / SCALE);

            if (parent.Dragging)
            {
                offsetX = Math.Clamp(offsetX - (mouseX - lastMouseX), 0, textureWidth - scaledWidth);
                offsetY = Math.Clamp(offsetY - (mouseY - lastMouseY), 0, textureHeight - scaledHeight);
            }

            lastMouseX = mouseX;
            lastMouseY = mouseY;

            Rectangle sourceRect = new Rectangle((int)offsetX, (int)offsetY, (int)scaledWidth, (int)scaledHeight);

            IClickableMenu.drawTextureBox(spriteBatch, bounds.X - 10, bounds.Y - 10, bounds.Width + 20, bounds.Height + 20, Color.White);

            spriteBatch.Draw(magicAltarTab.GetBackground(), bounds, sourceRect, Color.White);
        }

        protected override void RenderFg(SpriteBatch spriteBatch, int mouseX, int mouseY, float partialTicks)
        {
            ModEntry modEntry = parent.modEntry;
            Farmer player = Game1.player;

            if (player == null)
                return;

            spriteBatch.End();

            transformMatrix = Matrix.CreateTranslation(-offsetX, -offsetY, 0) * Matrix.CreateScale(SCALE, SCALE, 0);

            spriteBatch.Begin(SpriteSortMode.Immediate, blendState: BlendState.AlphaBlend,null, null, rasterizerState: new RasterizerState { ScissorTestEnable = true }, null, transformMatrix);

            SpellPartSkillHelper helper = SpellPartSkillHelper.Instance();

            Rectangle clippingRectangle = spriteBatch.GraphicsDevice.ScissorRectangle;
            spriteBatch.GraphicsDevice.ScissorRectangle = bounds;

            foreach (SpellPartSkill skill in skills)
            {
                Vector2 skillPos = new Vector2(bounds.X + skill.GetX(), bounds.Y + skill.GetY());
                //Vector2 skillPos = new Vector2(bounds.X + skill.GetX(), bounds.Y + skill.GetY()) * (1f / Game1.options.zoomLevel);
                //Vector2 skillPos = new Vector2(bounds.X + skill.GetX(), bounds.Y + skill.GetY()) * Game1.options.zoomLevel;

                bool knows = helper.Knows(modEntry, player, skill);

                //325

                //float cX = (skill.GetX() + SKILL_SIZE / 2 + 1);
                //float cY = (skill.GetY() + SKILL_SIZE / 2 + 1);

                float cX = (skillPos.X + SKILL_SIZE / 2 + 1);
                float cY = (skillPos.Y + SKILL_SIZE / 2 + 1);

                bool hasPrereq = helper.CanLearn(modEntry, player, skill) || knows;

                foreach (var parentSkill in skill.Parents())
                {
                    if(parent == null)
                        continue;

                    Vector2 parentSkillPos = new Vector2(bounds.X + parentSkill.GetX(), bounds.Y + parentSkill.GetY());
                    //Vector2 parentSkillPos = new Vector2(bounds.X + parentSkill.GetX(), bounds.Y + parentSkill.GetY()) * (1f / Game1.options.zoomLevel);
                    //Vector2 parentSkillPos = new Vector2(bounds.X + parentSkill.GetX(), bounds.Y + parentSkill.GetY()) * Game1.options.zoomLevel;

                    //float parentCX = (parentSkill.GetX() + SKILL_SIZE / 2 + 1);
                    //float parentCY = (parentSkill.GetY() + SKILL_SIZE / 2 + 1);

                    float parentCX = (parentSkillPos.X + SKILL_SIZE / 2 + 1);
                    float parentCY = (parentSkillPos.Y + SKILL_SIZE / 2 + 1);

                    uint color;
                    int offset;

                    //Color KNOWS_COLOR = new Color(0x00ff00);
                    //Color UNKNOWN_SKILL_LINE_COLOR_MASK = new Color(0x999999);

                    Color KNOWS_COLOR = Color.Blue;
                    Color UNKNOWN_SKILL_LINE_COLOR_MASK = new Color(KNOWS_COLOR.PackedValue * Color.DarkGray.PackedValue * Color.DarkGray.PackedValue * Color.DarkGray.PackedValue * Color.DarkGray.PackedValue * Color.DarkGray.PackedValue);

                    if (hasPrereq)
                    {
                        //color = knows ? KNOWS_COLOR.PackedValue : (KNOWS_COLOR.PackedValue & UNKNOWN_SKILL_LINE_COLOR_MASK.PackedValue | 0xFF000000);
                        color = knows ? KNOWS_COLOR.PackedValue : (KNOWS_COLOR.PackedValue);
                        offset = 1;
                    }
                    else
                    {
                        color = (UNKNOWN_SKILL_LINE_COLOR_MASK.PackedValue | 0xFF000000);
                        //color = Color.White.PackedValue;
                        offset = 0;
                    }

                    if (cX != parentCX)
                    {
                        DrawSprite.DrawLine(spriteBatch, new Vector2(parentCX, cY), new Vector2(cX, cY), new Color(color), 4);
                    }

                    if (cY != parentCY)
                    {
                        DrawSprite.DrawLine(spriteBatch, new Vector2(parentCX, parentCY), new Vector2(parentCX, cY), new Color(color), 4);
                    }
                }
            }

            foreach (SpellPartSkill skill in skills)
            {
                Vector2 skillPos = new Vector2(bounds.X + skill.GetX(), bounds.Y + skill.GetY());
                //Vector2 skillPos = new Vector2(bounds.X + skill.GetX(), bounds.Y + skill.GetY()) * (1f / Game1.options.zoomLevel);
                //Vector2 skillPos = new Vector2(bounds.X + skill.GetX(), bounds.Y + skill.GetY()) * Game1.options.zoomLevel;

                bool knows = helper.Knows(modEntry, player, skill);
                bool hasPrereq = helper.CanLearn(modEntry, player, skill) || knows;

                Color color = Color.Blue;

                if (!hasPrereq)
                {
                    color = new Color(125, 125, 125);
                    //color = Color.Black;
                }
                else if (!knows)
                {
                    color = Color.CornflowerBlue;
                }
                else
                {
                    color = Color.White;
                }

                Texture2D skillTexture = modEntry.spellPartIconManager.GetSprite(skill.GetId());
                
                spriteBatch.Draw(skillTexture, new Rectangle((int)skillPos.X, (int)skillPos.Y, (int)SKILL_SIZE, (int)SKILL_SIZE), color);
            }

            spriteBatch.GraphicsDevice.ScissorRectangle = clippingRectangle;

            spriteBatch.End();

            spriteBatch.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);

            if (hoverItem != null)
            {
                DrawHoverToolTip(modEntry, player, helper, spriteBatch);
            }

            if (!isHoveringSkill)
            {
                hoverItem = null;
            }

        }

        public override void MouseHover(float mouseX, float mouseY)
        {
            ModEntry modEntry = parent.modEntry;
            Farmer player = Game1.player;

            SpellPartSkillHelper helper = SpellPartSkillHelper.Instance();

            foreach (SpellPartSkill skill in skills)
            {
                //Vector2 skillPos = new Vector2(bounds.X + skill.GetX(), bounds.Y + skill.GetY()) * (1f / Game1.options.zoomLevel);
                Vector2 skillPos = new Vector2(bounds.X + skill.GetX(), bounds.Y + skill.GetY());

                Vector2 vector = Vector2.Transform(skillPos, transformMatrix);

                Rectangle rect = new Rectangle((int)vector.X, (int)vector.Y, (int)SKILL_SIZE + 16, (int)SKILL_SIZE + 16);

                if (rect.Contains(mouseX, mouseY))
                {
                    hoverItem = skill;
                    isHoveringSkill = true;
                }
                else
                {
                    //hoverItem = null;
                    isHoveringSkill = false;
                }
            }
        }

        public override void MouseClicked(float mouseX, float mouseY)
        {
            ModEntry modEntry = parent.modEntry;
            Farmer player = Game1.player;
            SpellPartSkillHelper helper = SpellPartSkillHelper.Instance();
            
            if (player != null && hoverItem != null && !helper.Knows(modEntry, player, hoverItem))
            {
                if (helper.CanLearn(modEntry, player, hoverItem))
                {
                    foreach (var item in hoverItem.Cost())
                    {
                        player.Items.ReduceId(item.Key.QualifiedItemId, item.Value);
                    }

                    helper.Learn(modEntry, player, hoverItem);
                }
            }
        }

        public void DrawHoverToolTip(ModEntry modEntry, Farmer player, SpellPartSkillHelper helper, SpriteBatch spriteBatch)
        {

            string spellPartNameText = modEntry.Helper.Translation.Get($"spellpart.{hoverItem.GetId()}.name");
            string spellPartDescriptionText = modEntry.Helper.Translation.Get($"spellpart.{hoverItem.GetId()}.description");

            int val1 = 272;
            if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.fr)
                val1 = 384;
            if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.tr)
                val1 = 336;

            int value = Math.Max(val1, (int)Game1.dialogueFont.MeasureString(spellPartNameText == null ? "" : spellPartNameText).X);

            StringBuilder s = new StringBuilder();
            s.AppendLine(Game1.parseText(spellPartDescriptionText, Game1.smallFont, value));
            //s.AppendLine("");

            if (spellPartNameText != null && spellPartDescriptionText != null)
                drawSkillToolTip(spriteBatch, s.ToString(), spellPartNameText, null, false, -1, 0, null, -1, hoverItem);
        }

        public void drawSkillToolTip(SpriteBatch b, string hoverText, string hoverTitle, Item hoveredItem, bool heldItem = false, int healAmountToDisplay = -1, int currencySymbol = 0, string extraItemToShowIndex = null, int extraItemToShowAmount = -1, SpellPartSkill craftingIngredients = null, int moneyAmountToShowAtBottom = -1)
        {
            Object @object = hoveredItem as Object;
            bool flag = @object != null && (int)@object.Edibility != -300;
            string[] array = null;
            if (flag && Game1.objectData.TryGetValue(hoveredItem.ItemId, out var value))
            {
                BuffEffects buffEffects = new BuffEffects();
                int num = int.MinValue;
                foreach (Buff item in Object.TryCreateBuffsFromData(value, hoveredItem.Name, hoveredItem.DisplayName, 1f, hoveredItem.ModifyItemBuffs))
                {
                    buffEffects.Add(item.effects);
                    if (item.millisecondsDuration == -2 || (item.millisecondsDuration > num && num != -2))
                    {
                        num = item.millisecondsDuration;
                    }
                }

                if (buffEffects.HasAnyValue())
                {
                    array = buffEffects.ToLegacyAttributeFormat();
                    if (num != -2)
                    {
                        array[12] = " " + Utility.getMinutesSecondsStringFromMilliseconds(num);
                    }
                }
            }

            drawSkillHoverText(b, hoverText, Game1.smallFont, heldItem ? 40 : 0, heldItem ? 40 : 0, moneyAmountToShowAtBottom, hoverTitle, flag ? ((int)(hoveredItem as Object).edibility) : (-1), array, hoveredItem, currencySymbol, extraItemToShowIndex, extraItemToShowAmount, -1, -1, 1f, craftingIngredients);
        }

        public void drawSkillHoverText(SpriteBatch b, string text, SpriteFont font, int xOffset = 0, int yOffset = 0, int moneyAmountToDisplayAtBottom = -1, string boldTitleText = null, int healAmountToDisplay = -1, string[] buffIconsToDisplay = null, Item hoveredItem = null, int currencySymbol = 0, string extraItemToShowIndex = null, int extraItemToShowAmount = -1, int overrideX = -1, int overrideY = -1, float alpha = 1f, SpellPartSkill craftingIngredients = null, IList<Item> additional_craft_materials = null, Texture2D boxTexture = null, Rectangle? boxSourceRect = null, Color? textColor = null, Color? textShadowColor = null, float boxScale = 1f, int boxWidthOverride = -1, int boxHeightOverride = -1)
        {
            learnedDescription.Clear();
            HoverTextStringBuilder.Clear();
            HoverTextStringBuilder.Append(text);
            drawSkillHoverText(b, HoverTextStringBuilder, font, xOffset, yOffset, moneyAmountToDisplayAtBottom, boldTitleText, healAmountToDisplay, buffIconsToDisplay, hoveredItem, currencySymbol, extraItemToShowIndex, extraItemToShowAmount, overrideX, overrideY, alpha, craftingIngredients, additional_craft_materials, boxTexture, boxSourceRect, textColor, textShadowColor, boxScale, boxWidthOverride, boxHeightOverride);
        }

        public void drawSkillHoverText(SpriteBatch b, StringBuilder text, SpriteFont font, int xOffset = 0, int yOffset = 0, int moneyAmountToDisplayAtBottom = -1, string boldTitleText = null, int healAmountToDisplay = -1, string[] buffIconsToDisplay = null, Item hoveredItem = null, int currencySymbol = 0, string extraItemToShowIndex = null, int extraItemToShowAmount = -1, int overrideX = -1, int overrideY = -1, float alpha = 1f, SpellPartSkill craftingIngredients = null, IList<Item> additional_craft_materials = null, Texture2D boxTexture = null, Rectangle? boxSourceRect = null, Color? textColor = null, Color? textShadowColor = null, float boxScale = 1f, int boxWidthOverride = -1, int boxHeightOverride = -1)
        {
            boxTexture = boxTexture ?? Game1.menuTexture;
            boxSourceRect = boxSourceRect ?? new Rectangle(0, 256, 60, 60);
            textColor = textColor ?? Game1.textColor;
            textShadowColor = textShadowColor ?? Game1.textShadowColor;
            if (text == null || text.Length == 0)
            {
                return;
            }

            if (moneyAmountToDisplayAtBottom <= -1 && currencySymbol == 0 && hoveredItem != null && Game1.player.stats.Get("Book_PriceCatalogue") != 0 && !(hoveredItem is Furniture) && hoveredItem.CanBeLostOnDeath() && !(hoveredItem is Clothing) && !(hoveredItem is Wallpaper) && (!(hoveredItem is Object) || !(hoveredItem as Object).bigCraftable.Value) && hoveredItem.sellToStorePrice(-1L) > 0)
            {
                moneyAmountToDisplayAtBottom = hoveredItem.sellToStorePrice(-1L) * hoveredItem.Stack;
            }

            string text2 = null;
            if (boldTitleText != null && boldTitleText.Length == 0)
            {
                boldTitleText = null;
            }

            int num = Math.Max((healAmountToDisplay != -1) ? ((int)font.MeasureString(healAmountToDisplay + "+ Energy" + 32).X) : 0, Math.Max((int)font.MeasureString(text).X, (boldTitleText != null) ? ((int)Game1.dialogueFont.MeasureString(boldTitleText).X) : 0)) + 32;
            int num2 = Math.Max(20 * 3, (int)font.MeasureString(text).Y + 32 + (int)((moneyAmountToDisplayAtBottom > -1) ? Math.Max(font.MeasureString(moneyAmountToDisplayAtBottom.ToString() ?? "").Y + 4f, 44f) : 0f) + (int)((boldTitleText != null) ? (Game1.dialogueFont.MeasureString(boldTitleText).Y + 16f) : 0f));
            if (extraItemToShowIndex != null)
            {
                ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem("(O)" + extraItemToShowIndex);
                string displayName = dataOrErrorItem.DisplayName;
                Rectangle sourceRect = dataOrErrorItem.GetSourceRect();
                string text3 = Game1.content.LoadString("Strings\\UI:ItemHover_Requirements", extraItemToShowAmount, (extraItemToShowAmount > 1) ? Lexicon.makePlural(displayName) : displayName);
                int num3 = sourceRect.Width * 2 * 4;
                num = Math.Max(num, num3 + (int)font.MeasureString(text3).X);
            }

            if (buffIconsToDisplay != null)
            {
                foreach (string text4 in buffIconsToDisplay)
                {
                    if (!text4.Equals("0") && text4 != "")
                    {
                        num2 += 39;
                    }
                }

                num2 += 4;
            }

            //if (craftingIngredients != null && Game1.options.showAdvancedCraftingInformation && craftingIngredients.getCraftCountText() != null)
            //{
            //    num2 += (int)font.MeasureString("T").Y + 2;
            //}

            string text5 = null;
            if (hoveredItem != null)
            {
                if (hoveredItem is FishingRod)
                {
                    if (hoveredItem.attachmentSlots() == 1)
                    {
                        num2 += 68;
                    }
                    else if (hoveredItem.attachmentSlots() > 1)
                    {
                        num2 += 136;
                    }
                }
                else
                {
                    num2 += 68 * hoveredItem.attachmentSlots();
                }

                text5 = hoveredItem.getCategoryName();
                if (text5.Length > 0)
                {
                    num = Math.Max(num, (int)font.MeasureString(text5).X + 32);
                    num2 += (int)font.MeasureString("T").Y;
                }

                int num4 = 9999;
                int num5 = 92;
                Point extraSpaceNeededForTooltipSpecialIcons = hoveredItem.getExtraSpaceNeededForTooltipSpecialIcons(font, num, num5, num2, text, boldTitleText, moneyAmountToDisplayAtBottom);
                num = ((extraSpaceNeededForTooltipSpecialIcons.X != 0) ? extraSpaceNeededForTooltipSpecialIcons.X : num);
                num2 = ((extraSpaceNeededForTooltipSpecialIcons.Y != 0) ? extraSpaceNeededForTooltipSpecialIcons.Y : num2);
                MeleeWeapon meleeWeapon = hoveredItem as MeleeWeapon;
                if (meleeWeapon != null)
                {
                    if (meleeWeapon.GetTotalForgeLevels() > 0)
                    {
                        num2 += (int)font.MeasureString("T").Y;
                    }

                    if (meleeWeapon.GetEnchantmentLevel<GalaxySoulEnchantment>() > 0)
                    {
                        num2 += (int)font.MeasureString("T").Y;
                    }
                }

                Object @object = hoveredItem as Object;
                if (@object != null && (int)@object.Edibility != -300)
                {
                    healAmountToDisplay = @object.staminaRecoveredOnConsumption();
                    num2 = ((healAmountToDisplay == -1) ? (num2 + 40) : (num2 + 40 * ((healAmountToDisplay <= 0 || @object.healthRecoveredOnConsumption() <= 0) ? 1 : 2)));
                    if (Game1.content.GetCurrentLanguage() == LocalizedContentManager.LanguageCode.zh)
                    {
                        num2 += 16;
                    }

                    num = (int)Math.Max(num, Math.Max(font.MeasureString(Game1.content.LoadString("Strings\\UI:ItemHover_Energy", num4)).X + (float)num5, font.MeasureString(Game1.content.LoadString("Strings\\UI:ItemHover_Health", num4)).X + (float)num5));
                }

                if (buffIconsToDisplay != null)
                {
                    for (int j = 0; j < buffIconsToDisplay.Length; j++)
                    {
                        if (!buffIconsToDisplay[j].Equals("0") && j <= 12)
                        {
                            num = (int)Math.Max(num, font.MeasureString(Game1.content.LoadString("Strings\\UI:ItemHover_Buff" + j, num4)).X + (float)num5);
                        }
                    }
                }
            }

            Vector2 vector = Vector2.Zero;
            if (craftingIngredients != null)
            {
                //if (Game1.options.showAdvancedCraftingInformation)
                //{
                //    int craftableCount = craftingIngredients.getCraftableCount(additional_craft_materials);
                //    if (craftableCount > 1)
                //    {
                //        text2 = " (" + craftableCount + ")";
                //        vector = Game1.smallFont.MeasureString(text2);
                //    }
                //}

                num = (int)Math.Max(Game1.dialogueFont.MeasureString(boldTitleText).X + vector.X + 12f, 384f);
                //num2 += craftingIngredients.getDescriptionHeight(num + 4 - 8) - 32;

                int w = num + 4 - 8;

                if (craftingIngredients.Parents().Any())
                {
                    learnedDescription.AppendLine("Learned:");

                    foreach (SpellPartSkill parentSkill in craftingIngredients.Parents())
                    {
                        learnedDescription.AppendLine(parentSkill.GetId());
                    }

                    learnedDescription.AppendLine("");
                }


                learnedDescription.AppendLine("");

                num2 += (int)(Game1.smallFont.MeasureString(Game1.parseText(learnedDescription.ToString(), Game1.smallFont, w)).Y + (float)(craftingIngredients.Cost().Count * 36) + (float)(int)Game1.smallFont.MeasureString(Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.567")).Y + 21f) - 40;
                
                if (craftingIngredients != null && hoveredItem != null && hoveredItem.getDescription().Equals(text.ToString()))
                {
                    num2 -= (int)font.MeasureString(text.ToString()).Y;
                }

                if (craftingIngredients != null && Game1.content.GetCurrentLanguage() == LocalizedContentManager.LanguageCode.zh)
                {
                    num2 += 8;
                }
            }
            else if (text2 != null && boldTitleText != null)
            {
                vector = Game1.smallFont.MeasureString(text2);
                num = (int)Math.Max(num, Game1.dialogueFont.MeasureString(boldTitleText).X + vector.X + 12f);
            }

            int x = Game1.getOldMouseX() + 32 + xOffset;
            int num6 = Game1.getOldMouseY() + 32 + yOffset;
            if (overrideX != -1)
            {
                x = overrideX;
            }

            if (overrideY != -1)
            {
                num6 = overrideY;
            }

            if (x + num > Utility.getSafeArea().Right)
            {
                x = Utility.getSafeArea().Right - num;
                num6 += 16;
            }

            if (num6 + num2 > Utility.getSafeArea().Bottom)
            {
                x += 16;
                if (x + num > Utility.getSafeArea().Right)
                {
                    x = Utility.getSafeArea().Right - num;
                }

                num6 = Utility.getSafeArea().Bottom - num2;
            }

            num += 4;
            int num7 = ((boxWidthOverride != -1) ? boxWidthOverride : (num + ((craftingIngredients != null) ? 21 : 0)));
            int num8 = ((boxHeightOverride != -1) ? boxHeightOverride : num2);
            IClickableMenu.drawTextureBox(b, boxTexture, boxSourceRect.Value, x, num6, num7, num8, Color.White * alpha, boxScale);
            if (boldTitleText != null)
            {
                Vector2 vector2 = Game1.dialogueFont.MeasureString(boldTitleText);
                IClickableMenu.drawTextureBox(b, boxTexture, boxSourceRect.Value, x, num6, num + ((craftingIngredients != null) ? 21 : 0), (int)Game1.dialogueFont.MeasureString(boldTitleText).Y + 32 + (int)((hoveredItem != null && text5.Length > 0) ? font.MeasureString("asd").Y : 0f) - 4, Color.White * alpha, 1f, drawShadow: false);
                b.Draw(Game1.menuTexture, new Rectangle(x + 12, num6 + (int)Game1.dialogueFont.MeasureString(boldTitleText).Y + 32 + (int)((hoveredItem != null && text5.Length > 0) ? font.MeasureString("asd").Y : 0f) - 4, num - 4 * ((craftingIngredients != null) ? 1 : 6), 4), new Rectangle(44, 300, 4, 4), Color.White);
                b.DrawString(Game1.dialogueFont, boldTitleText, new Vector2(x + 16, num6 + 16 + 4) + new Vector2(2f, 2f), textShadowColor.Value);
                b.DrawString(Game1.dialogueFont, boldTitleText, new Vector2(x + 16, num6 + 16 + 4) + new Vector2(0f, 2f), textShadowColor.Value);
                b.DrawString(Game1.dialogueFont, boldTitleText, new Vector2(x + 16, num6 + 16 + 4), textColor.Value);
                if (text2 != null)
                {
                    Utility.drawTextWithShadow(b, text2, Game1.smallFont, new Vector2((float)(x + 16) + vector2.X, (int)((float)(num6 + 16 + 4) + vector2.Y / 2f - vector.Y / 2f)), Game1.textColor);
                }

                num6 += (int)Game1.dialogueFont.MeasureString(boldTitleText).Y;
            }

            if (hoveredItem != null && text5.Length > 0)
            {
                num6 -= 4;
                Utility.drawTextWithShadow(b, text5, font, new Vector2(x + 16, num6 + 16 + 4), hoveredItem.getCategoryColor(), 1f, -1f, 2, 2);
                num6 += (int)font.MeasureString("T").Y + ((boldTitleText != null) ? 16 : 0) + 4;
                Tool tool = hoveredItem as Tool;
                if (tool != null && tool.GetTotalForgeLevels() > 0)
                {
                    string text6 = Game1.content.LoadString("Strings\\UI:Item_Tooltip_Forged");
                    Utility.drawTextWithShadow(b, text6, font, new Vector2(x + 16, num6 + 16 + 4), Color.DarkRed, 1f, -1f, 2, 2);
                    int totalForgeLevels = tool.GetTotalForgeLevels();
                    if (totalForgeLevels < tool.GetMaxForges() && !tool.hasEnchantmentOfType<DiamondEnchantment>())
                    {
                        Utility.drawTextWithShadow(b, " (" + totalForgeLevels + "/" + tool.GetMaxForges() + ")", font, new Vector2((float)(x + 16) + font.MeasureString(text6).X, num6 + 16 + 4), Color.DimGray, 1f, -1f, 2, 2);
                    }

                    num6 += (int)font.MeasureString("T").Y;
                }

                MeleeWeapon meleeWeapon2 = hoveredItem as MeleeWeapon;
                if (meleeWeapon2 != null && meleeWeapon2.GetEnchantmentLevel<GalaxySoulEnchantment>() > 0)
                {
                    GalaxySoulEnchantment enchantmentOfType = meleeWeapon2.GetEnchantmentOfType<GalaxySoulEnchantment>();
                    string text7 = Game1.content.LoadString("Strings\\UI:Item_Tooltip_GalaxyForged");
                    Utility.drawTextWithShadow(b, text7, font, new Vector2(x + 16, num6 + 16 + 4), Color.DarkRed, 1f, -1f, 2, 2);
                    int level = enchantmentOfType.GetLevel();
                    if (level < enchantmentOfType.GetMaximumLevel())
                    {
                        Utility.drawTextWithShadow(b, " (" + level + "/" + enchantmentOfType.GetMaximumLevel() + ")", font, new Vector2((float)(x + 16) + font.MeasureString(text7).X, num6 + 16 + 4), Color.DimGray, 1f, -1f, 2, 2);
                    }

                    num6 += (int)font.MeasureString("T").Y;
                }
            }
            else
            {
                num6 += ((boldTitleText != null) ? 16 : 0);
            }

            if (hoveredItem != null && craftingIngredients == null)
            {
                hoveredItem.drawTooltip(b, ref x, ref num6, font, alpha, text);
            }
            else if (text != null && text.Length != 0 && (text.Length != 1 || text[0] != ' ') && (craftingIngredients == null || hoveredItem == null || !hoveredItem.getDescription().Equals(text.ToString())))
            {
                if (text.ToString().Contains("[line]"))
                {
                    string[] array = text.ToString().Split("[line]");
                    b.DrawString(font, array[0], new Vector2(x + 16, num6 + 16 + 4) + new Vector2(2f, 2f), textShadowColor.Value * alpha);
                    b.DrawString(font, array[0], new Vector2(x + 16, num6 + 16 + 4) + new Vector2(0f, 2f), textShadowColor.Value * alpha);
                    b.DrawString(font, array[0], new Vector2(x + 16, num6 + 16 + 4) + new Vector2(2f, 0f), textShadowColor.Value * alpha);
                    b.DrawString(font, array[0], new Vector2(x + 16, num6 + 16 + 4), textColor.Value * 0.9f * alpha);
                    num6 += (int)font.MeasureString(array[0]).Y - 16;
                    Utility.drawLineWithScreenCoordinates(x + 16 - 4, num6 + 16 + 4, x + 16 + num - 28, num6 + 16 + 4, b, textShadowColor.Value);
                    Utility.drawLineWithScreenCoordinates(x + 16 - 4, num6 + 16 + 5, x + 16 + num - 28, num6 + 16 + 5, b, textShadowColor.Value);
                    if (array.Length > 1)
                    {
                        num6 -= 16;
                        b.DrawString(font, array[1], new Vector2(x + 16, num6 + 16 + 4) + new Vector2(2f, 2f), textShadowColor.Value * alpha);
                        b.DrawString(font, array[1], new Vector2(x + 16, num6 + 16 + 4) + new Vector2(0f, 2f), textShadowColor.Value * alpha);
                        b.DrawString(font, array[1], new Vector2(x + 16, num6 + 16 + 4) + new Vector2(2f, 0f), textShadowColor.Value * alpha);
                        b.DrawString(font, array[1], new Vector2(x + 16, num6 + 16 + 4), textColor.Value * 0.9f * alpha);
                        num6 += (int)font.MeasureString(array[1]).Y;
                    }

                    num6 += 4;
                }
                else
                {
                    b.DrawString(font, text, new Vector2(x + 16, num6 + 16 + 4) + new Vector2(2f, 2f), textShadowColor.Value * alpha);
                    b.DrawString(font, text, new Vector2(x + 16, num6 + 16 + 4) + new Vector2(0f, 2f), textShadowColor.Value * alpha);
                    b.DrawString(font, text, new Vector2(x + 16, num6 + 16 + 4) + new Vector2(2f, 0f), textShadowColor.Value * alpha);
                    b.DrawString(font, text, new Vector2(x + 16, num6 + 16 + 4), textColor.Value * 0.9f * alpha);
                    num6 += (int)font.MeasureString(text).Y + 4;
                }
            }

            if (craftingIngredients != null)
            {
                //craftingIngredients.drawRecipeDescription(b, new Vector2(x + 16, num6 - 8), num, additional_craft_materials);
                //num6 += craftingIngredients.getDescriptionHeight(num - 8);

                drawKnowegeCostDescription(b, new Vector2(x + 16, num6 - 8), num, craftingIngredients);
            }

            if (healAmountToDisplay != -1)
            {
                int num9 = (hoveredItem as Object).staminaRecoveredOnConsumption();
                if (Game1.content.GetCurrentLanguage() == LocalizedContentManager.LanguageCode.zh)
                {
                    num6 += 8;
                }

                if (num9 >= 0)
                {
                    int num10 = (hoveredItem as Object).healthRecoveredOnConsumption();
                    if (num9 > 0)
                    {
                        Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2(x + 16 + 4, num6 + 16), new Rectangle((num9 < 0) ? 140 : 0, 428, 10, 10), Color.White, 0f, Vector2.Zero, 3f, flipped: false, 0.95f);
                        Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\UI:ItemHover_Energy", ((num9 > 0) ? "+" : "") + num9), font, new Vector2(x + 16 + 34 + 4, num6 + 16), Game1.textColor);
                        num6 += 34;
                    }

                    if (num10 > 0)
                    {
                        Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2(x + 16 + 4, num6 + 16), new Rectangle(0, 438, 10, 10), Color.White, 0f, Vector2.Zero, 3f, flipped: false, 0.95f);
                        Utility.drawTextWithShadow(b, (num10 >= 999) ? " 100%" : Game1.content.LoadString("Strings\\UI:ItemHover_Health", ((num10 > 0) ? "+" : "") + num10), font, new Vector2(x + 16 + 34 + 4, num6 + 16), Game1.textColor);
                        num6 += 34;
                    }
                }
                else if (num9 != -300)
                {
                    Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2(x + 16 + 4, num6 + 16), new Rectangle(140, 428, 10, 10), Color.White, 0f, Vector2.Zero, 3f, flipped: false, 0.95f);
                    Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\UI:ItemHover_Energy", num9.ToString() ?? ""), font, new Vector2(x + 16 + 34 + 4, num6 + 16), Game1.textColor);
                    num6 += 34;
                }
            }

            if (buffIconsToDisplay != null)
            {
                num6 += 16;
                b.Draw(Game1.staminaRect, new Rectangle(x + 12, num6 + 6, num - ((craftingIngredients != null) ? 4 : 24), 2), new Color(207, 147, 103) * 0.8f);
                for (int k = 0; k < buffIconsToDisplay.Length; k++)
                {
                    if (buffIconsToDisplay[k].Equals("0") || !(buffIconsToDisplay[k] != ""))
                    {
                        continue;
                    }

                    if (k == 12)
                    {
                        Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2(x + 16 + 4, num6 + 16), new Rectangle(410, 501, 9, 9), Color.White, 0f, Vector2.Zero, 3f, flipped: false, 0.95f);
                        Utility.drawTextWithShadow(b, buffIconsToDisplay[k], font, new Vector2(x + 16 + 34 + 4, num6 + 16), Game1.textColor);
                    }
                    else
                    {
                        Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2(x + 16 + 4, num6 + 16), new Rectangle(10 + k * 10, 428, 10, 10), Color.White, 0f, Vector2.Zero, 3f, flipped: false, 0.95f);
                        string text8 = ((Convert.ToDouble(buffIconsToDisplay[k]) > 0.0) ? "+" : "") + buffIconsToDisplay[k] + " ";
                        if (k <= 11)
                        {
                            text8 = Game1.content.LoadString("Strings\\UI:ItemHover_Buff" + k, text8);
                        }

                        Utility.drawTextWithShadow(b, text8, font, new Vector2(x + 16 + 34 + 4, num6 + 16), Game1.textColor);
                    }

                    num6 += 39;
                }

                num6 -= 8;
            }

            if (hoveredItem != null && hoveredItem.attachmentSlots() > 0)
            {
                hoveredItem.drawAttachments(b, x + 16, num6 + 16);
                if (moneyAmountToDisplayAtBottom > -1)
                {
                    num6 += 68 * hoveredItem.attachmentSlots();
                }
            }

            if (moneyAmountToDisplayAtBottom > -1)
            {
                b.Draw(Game1.staminaRect, new Rectangle(x + 12, num6 + 22 - ((healAmountToDisplay <= 0) ? 6 : 0), num - ((craftingIngredients != null) ? 4 : 24), 2), new Color(207, 147, 103) * 0.5f);
                string text9 = moneyAmountToDisplayAtBottom.ToString();
                int num11 = 0;
                if ((buffIconsToDisplay != null && buffIconsToDisplay.Length > 1) || healAmountToDisplay > 0 || craftingIngredients != null)
                {
                    num11 = 8;
                }

                b.DrawString(font, text9, new Vector2(x + 16, num6 + 16 + 4 + num11) + new Vector2(2f, 2f), textShadowColor.Value);
                b.DrawString(font, text9, new Vector2(x + 16, num6 + 16 + 4 + num11) + new Vector2(0f, 2f), textShadowColor.Value);
                b.DrawString(font, text9, new Vector2(x + 16, num6 + 16 + 4 + num11) + new Vector2(2f, 0f), textShadowColor.Value);
                b.DrawString(font, text9, new Vector2(x + 16, num6 + 16 + 4 + num11), textColor.Value);
                switch (currencySymbol)
                {
                    case 0:
                        b.Draw(Game1.debrisSpriteSheet, new Vector2((float)(x + 16) + font.MeasureString(text9).X + 20f, num6 + 16 + 20 + num11), Game1.getSourceRectForStandardTileSheet(Game1.debrisSpriteSheet, 8, 16, 16), Color.White, 0f, new Vector2(8f, 8f), 4f, SpriteEffects.None, 0.95f);
                        break;
                    case 1:
                        b.Draw(Game1.mouseCursors, new Vector2((float)(x + 8) + font.MeasureString(text9).X + 20f, num6 + 16 - 5 + num11), new Rectangle(338, 400, 8, 8), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
                        break;
                    case 2:
                        b.Draw(Game1.mouseCursors, new Vector2((float)(x + 8) + font.MeasureString(text9).X + 20f, num6 + 16 - 7 + num11), new Rectangle(211, 373, 9, 10), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
                        break;
                    case 4:
                        b.Draw(Game1.objectSpriteSheet, new Vector2((float)(x + 8) + font.MeasureString(text9).X + 20f, num6 + 16 - 7 + num11), Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 858, 16, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
                        break;
                }

                num6 += 48;
            }

            if (extraItemToShowIndex != null)
            {
                if (moneyAmountToDisplayAtBottom == -1)
                {
                    num6 += 8;
                }

                ParsedItemData dataOrErrorItem2 = ItemRegistry.GetDataOrErrorItem(extraItemToShowIndex);
                string displayName2 = dataOrErrorItem2.DisplayName;
                Texture2D texture = dataOrErrorItem2.GetTexture();
                Rectangle sourceRect2 = dataOrErrorItem2.GetSourceRect();
                string text10 = Game1.content.LoadString("Strings\\UI:ItemHover_Requirements", extraItemToShowAmount, displayName2);
                float num12 = Math.Max(font.MeasureString(text10).Y + 21f, 96f);
                IClickableMenu.drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60), x, num6 + 4, num + ((craftingIngredients != null) ? 21 : 0), (int)num12, Color.White);
                num6 += 20;
                b.DrawString(font, text10, new Vector2(x + 16, num6 + 4) + new Vector2(2f, 2f), textShadowColor.Value);
                b.DrawString(font, text10, new Vector2(x + 16, num6 + 4) + new Vector2(0f, 2f), textShadowColor.Value);
                b.DrawString(font, text10, new Vector2(x + 16, num6 + 4) + new Vector2(2f, 0f), textShadowColor.Value);
                b.DrawString(Game1.smallFont, text10, new Vector2(x + 16, num6 + 4), textColor.Value);
                b.Draw(texture, new Vector2(x + 16 + (int)font.MeasureString(text10).X + 21, num6), sourceRect2, Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
            }

            //if (craftingIngredients != null && Game1.options.showAdvancedCraftingInformation)
            //{
            //    Utility.drawTextWithShadow(b, craftingIngredients.getCraftCountText(), font, new Vector2(x + 16, num6 + 16 + 4), Game1.textColor, 1f, -1f, 2, 2);
            //    num6 += (int)font.MeasureString("T").Y + 4;
            //}
        }

        public virtual void drawKnowegeCostDescription(SpriteBatch b, Vector2 position, int width, SpellPartSkill knowlege)
        {
            int num = ((LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ko) ? 8 : 0);
            
            if(knowlege.Cost().Any())
            {

                b.Draw(Game1.staminaRect, new Rectangle((int)(position.X + 8f), (int)(position.Y + 32f + Game1.smallFont.MeasureString("Ing!").Y) - 4 - 2 - (int)((float)num * 1.5f), width - 32, 2), Game1.textColor * 0.35f);
                Utility.drawTextWithShadow(b, Game1.content.LoadString("Strings\\StringsFromCSFiles:CraftingRecipe.cs.567"), Game1.smallFont, position + new Vector2(8f, 28f), Game1.textColor * 0.75f);


                int num2 = -1;

                foreach (KeyValuePair<Item, int> cost in knowlege.Cost())
                {
                    num2++;

                    int ingredientAmount = cost.Value;
                    string ingredientID = cost.Key.QualifiedItemId;

                    int itemCount = Game1.player.Items.CountId(ingredientID);
                    int num3 = 0;
                    int num4 = ingredientAmount - itemCount;

                    string nameFromIndex;

                    ParsedItemData dataOrErrorItem1 = ItemRegistry.GetDataOrErrorItem(ingredientID);

                    if (dataOrErrorItem1 != null)
                    {
                        nameFromIndex = dataOrErrorItem1.DisplayName;
                    }
                    else 
                    { 
                        nameFromIndex = ItemRegistry.GetErrorItemName(); 
                    }

                    Color color = ((num4 <= 0) ? Game1.textColor : Color.Red);
                    ParsedItemData dataOrErrorItem2 = ItemRegistry.GetDataOrErrorItem(ingredientID);
                    Texture2D texture = dataOrErrorItem2.GetTexture();
                    Rectangle sourceRect = dataOrErrorItem2.GetSourceRect();
                    float num5 = 2f;

                    if (sourceRect.Width > 0 || sourceRect.Height > 0)
                    {
                        num5 *= 16f / (float)Math.Max(sourceRect.Width, sourceRect.Height);
                    }

                    b.Draw(texture, new Vector2(position.X + 16f, position.Y + 64f + (float)(num2 * 64 / 2) + (float)(num2 * 4) + 16f), sourceRect, Color.White, 0f, new Vector2(sourceRect.Width / 2, sourceRect.Height / 2), num5, SpriteEffects.None, 0.86f);
                    Utility.drawTinyDigits(ingredientAmount, b, new Vector2(position.X + 32f - Game1.tinyFont.MeasureString(ingredientAmount.ToString() ?? "").X, position.Y + 64f + (float)(num2 * 64 / 2) + (float)(num2 * 4) + 21f), 2f, 0.87f, Color.AntiqueWhite);
                    Vector2 vector = new Vector2(position.X + 32f + 8f, position.Y + 64f + (float)(num2 * 64 / 2) + (float)(num2 * 4) + 4f);
                    Utility.drawTextWithShadow(b, nameFromIndex, Game1.smallFont, vector, color);
                    if (Game1.options.showAdvancedCraftingInformation)
                    {
                        vector.X = position.X + (float)width - 40f;
                        b.Draw(Game1.mouseCursors, new Rectangle((int)vector.X, (int)vector.Y + 2, 22, 26), new Rectangle(268, 1436, 11, 13), Color.White);
                        Utility.drawTextWithShadow(b, (itemCount + num3).ToString() ?? "", Game1.smallFont, vector - new Vector2(Game1.smallFont.MeasureString(itemCount + num3 + " ").X, 0f), color);
                    }
                }
            }

            if (knowlege.Parents().Any())
            {
                learnedDescription.AppendLine("Learned:");

                b.Draw(Game1.staminaRect, new Rectangle((int)(position.X + 8f), (int)(position.Y + 76 + knowlege.Cost().Count * 36 + num + Game1.smallFont.MeasureString("Ing!").Y) - 4 - 2 - (int)((float)num * 1.5f), width - 32, 2), Game1.textColor * 0.35f);
                Utility.drawTextWithShadow(b, Game1.parseText("Learned:", Game1.smallFont, width - 8), Game1.smallFont, position + new Vector2(8f, 76 + knowlege.Cost().Count * 36 + num), Game1.textColor * 0.75f);

                int g = 0;

                foreach (SpellPartSkill parentSkill in knowlege.Parents())
                {
                    //learnedDescription.AppendLine(parentSkill.GetId());
                    learnedDescription.AppendLine(parent.modEntry.Helper.Translation.Get($"spellpart.{parentSkill.GetId()}.name"));
                    Utility.drawTextWithShadow(b, Game1.parseText(parent.modEntry.Helper.Translation.Get($"spellpart.{parentSkill.GetId()}.name"), Game1.smallFont, width - 8), Game1.smallFont, position + new Vector2(8f, 76 + knowlege.Cost().Count * 36 + num + 50 + g), SpellPartSkillHelper.Instance().Knows(parent.modEntry, Game1.player, parentSkill) ? Color.Green : Color.Red);
                    g += 20;
                }
            }
        }
    }
}
