/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ProfeJavix/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace UIHelper
{
    public class ModApi : IUIHelperApi
    {
        private string modUniqueId;
        private readonly Dictionary<string, ModUIFeatures> modsFeatures = new();

        public ModApi(string modUniqueId, Dictionary<string, ModUIFeatures> modsFeatures)
        {
            this.modUniqueId = modUniqueId;
            this.modsFeatures = modsFeatures;
        }


        /// <inheritdoc/>
        public void InitUIFeatures(int bgWidth, int bgHeight, Color? color = null, Color? borderColor = null, bool defaultPageHasScroll = false, int defElementSpacing = 25)
        {
            ModUIFeatures ui = new ModUIFeatures(bgWidth, bgHeight, color, borderColor, defaultPageHasScroll, defElementSpacing);
            bool added = modsFeatures.TryAdd(
                modUniqueId,
                ui
            );
            if (!added){
                modsFeatures[modUniqueId] = ui;
                Console.Log($"The UI for the mod {modUniqueId} was reset.", LogLevel.Info);
            }

        }

        ///<inheritdoc/>
        public void OpenUI(bool playSound = true)
        {
            if (Game1.activeClickableMenu is not ModUIFeatures)
            {
                TryGetUI().OpenUI();
                if (playSound)
                    Game1.playSound("select");
            }
        }

        /// <inheritdoc/>
        public void AddPage(Texture2D tabIcon = null, Rectangle? sourceRect = null, int elementSpacing = 25, bool hasScroll = false)
        {
            ModUIFeatures ui = TryGetUI();
            if (ui != null)
            {
                ui.AddPage(tabIcon, sourceRect, elementSpacing, hasScroll);
            }
        }

        /// <inheritdoc/>
        public void AddTitleToPage(string title, int pageIdx = 0, Color? textColor = null, bool drawBox = true, Color? boxColor = null, Color? borderColor = null)
        {
            ModUIFeatures ui = TryGetUI();
            if (ui != null)
            {
                ui.SetTitleToPage(title, pageIdx, textColor, drawBox, boxColor, borderColor);
            }
        }

        /// <inheritdoc/>
        public void ConfigPageScrollArea(bool scrollOwnBG, bool showScrollBtns, int pageIdx = 0, Color? scrollColor = null, Color? scrollBorderColor = null)
        {
            ModUIFeatures ui = TryGetUI();
            if (ui != null)
            {
                ui.ConfigPageScroll(scrollOwnBG, showScrollBtns, pageIdx, scrollColor, scrollBorderColor);
            }
        }

        /// <inheritdoc/>
        public void AddBlankSpace(int pageIdx = 0, int height = 10)
        {
            ModUIFeatures ui = TryGetUI();
            if (ui != null)
            {
                ui.AddBlankSpace(pageIdx, Math.Max(height, 0));
            }
        }

        /// <inheritdoc/>
        public void AddSeparator(int pageIdx = 0, Color? color = null)
        {
            ModUIFeatures ui = TryGetUI();
            if (ui != null)
            {
                ui.AddSeparator(pageIdx, color);
            }
        }

        /// <inheritdoc/>
        public void AddLabel(string text, bool hasBox, int pageIdx = 0, char align = 'C', Color? textColor = null, Color? innerColor = null, Color? borderColor = null)
        {

            if (text == null)
            {
                Console.Log("Text cannot be null.");
                return;
            }

            Vector2 textMeasure = Game1.smallFont.MeasureString(text);
            int width = FitWidthInBG((int)textMeasure.X + 32 + 10, pageIdx, out ModUIFeatures ui);
            Vector2 pos = GetAlignedPos(width, pageIdx, align, ui);

            Rectangle bounds = new((int)pos.X, (int)pos.Y, width, (int)textMeasure.Y + 32 + 10);
            UILabel label = new(text, bounds, hasBox, textColor, innerColor, borderColor, (EAlign)char.ToUpper(align));
            ui.AddComponent(label, pageIdx);
        }

        /// <inheritdoc/>
        public void AddTextureLabel(Texture2D icon, int width, int height, bool hasBox, int pageIdx = 0, char align = 'C', Color? borderColor = null, Rectangle? sourceRect = null)
        {
            if (icon == null)
            {
                Console.Log("Image cannot be null.");
                return;
            }

            int wdth = FitWidthInBG(width, pageIdx, out ModUIFeatures ui);
            Vector2 pos = GetAlignedPos(wdth, pageIdx, align, ui);
            Rectangle bounds = new((int)pos.X, (int)pos.Y, wdth, height);
            UITextureLabel label = new(icon, bounds, hasBox, borderColor, sourceRect, (EAlign)char.ToUpper(align));
            ui.AddComponent(label, pageIdx);
        }

        /// <inheritdoc/>
        public void AddCheckbox(string text, int pageIdx = 0, char align = 'C', bool isChecked = false, Color? textColor = null, string id = null)
        {
            string desc = text ?? "NULL TEXT";
            Vector2 textMeasure = Game1.smallFont.MeasureString(desc);
            int wdth = FitWidthInBG((int)textMeasure.X + 36 + 10, pageIdx, out ModUIFeatures ui);
            Vector2 pos = GetAlignedPos(wdth, pageIdx, align, ui);

            Rectangle bounds = new((int)pos.X, (int)pos.Y, wdth, Math.Max((int)textMeasure.Y, 36));
            UICheckbox checkbox = new(bounds, isChecked, textColor, desc, (EAlign)char.ToUpper(align));
            ui.AddComponent(checkbox, pageIdx, id);
        }

        /// <inheritdoc/>
        public void AddSlider(object initValue, int pageIdx = 0, char align = 'C', object minVal = null, object maxVal = null, int width = 220, Color? textColor = null, string id = null)
        {
            decimal decVal = 0m, decMin = 0m, decMax = 9999.99m;
            if (initValue != null && !decimal.TryParse(Convert.ToString(initValue), out decVal))
            {
                Console.Log("You must pass null or a numerical value in initValue.");
                return;
            }
            if (minVal != null && !decimal.TryParse(Convert.ToString(minVal), out decMin))
            {
                Console.Log("You must pass null or a numerical value in minVal.");
                return;
            }
            if (maxVal != null && !decimal.TryParse(Convert.ToString(maxVal), out decMax))
            {
                Console.Log("You must pass null or a numerical value in maxVal.");
                return;
            }
            bool IsFloat = initValue != null && initValue is not int;
            float min = (minVal != null) ? Math.Clamp((float)Math.Round(decMin, 2), -9999.99f, 9998.99f) : 0f;
            float max = (maxVal != null) ? Math.Clamp((float)Math.Round(decMax, 2), min + 1, 9999.99f) : 9999.99f;
            float val = Math.Clamp((float)Math.Round(decVal, 2), min, max);

            if(!IsFloat){
                min = (float)Math.Truncate(Convert.ToDouble(min));
                max = (float)Math.Truncate(Convert.ToDouble(max));
                val = (float)Math.Truncate(Convert.ToDouble(val));;
            }

            int wdth = Math.Max(220, width);
            Vector2 valSize = Game1.smallFont.MeasureString("9999.99");
            wdth = FitWidthInBG(wdth + (int)valSize.X, pageIdx, out ModUIFeatures ui);
            Vector2 pos = GetAlignedPos(wdth, pageIdx, align, ui);

            Rectangle bounds = new((int)pos.X, (int)pos.Y, wdth, 24 + 10 + (int)valSize.Y);
            UISlider slider = new(bounds, val, IsFloat, min, max, textColor, (EAlign)char.ToUpper(align));
            ui.AddComponent(slider, pageIdx, id);
        }

        ///<inheritdoc/>
        public void AddTextBox(int width, int pageIdx = 0, char align = 'C', Color? textColor = null, string initialText = "", string id = null)
        {
            string ph = initialText ?? "";
            int wdth = FitWidthInBG(width + 10 + 24, pageIdx, out ModUIFeatures ui);
            Vector2 pos = GetAlignedPos(wdth, pageIdx, align, ui);

            Rectangle bounds = new((int)pos.X, (int)pos.Y, wdth, (int)Game1.smallFont.MeasureString("ABC").Y + 24);
            ph = UIUtils.FitTextInArea(ph, wdth, Game1.smallFont);
            UITextbox textbox = new(bounds, ph, (EAlign)char.ToUpper(align), textColor);
            ui.AddComponent(textbox, pageIdx, id);
        }

        ///<inheritdoc/>
        public void AddTextButton(string text, int pageIdx = 0, char align = 'C', string asociatedId = null, Action customAction = null, Color? textColor = null)
        {
            string txt = text ?? "Button";
            Vector2 textSize = Game1.smallFont.MeasureString(text);
            int wdth = FitWidthInBG((int)textSize.X + 60, pageIdx, out ModUIFeatures ui);
            Vector2 pos = GetAlignedPos(wdth, pageIdx, align, ui);

            Rectangle bounds = new((int)pos.X, (int)pos.Y, wdth, (int)textSize.Y+40);
            UITextButton button = new(bounds, txt, (EAlign)char.ToUpper(align), textColor);
            button.action += customAction;

            if(asociatedId != null)
                ui.AsignComponentValueToButton(button, asociatedId);

            ui.AddComponent(button, pageIdx);
        }

        ///<inheritdoc/>
        public void AddAcceptButton(bool closesMenu = true, Action<Dictionary<string, object>> action = null)
        {
            ModUIFeatures ui = TryGetUI();
            if (ui != null)
            {
                ui.AddBGButton(EButtonType.Accept, closesMenu, action);
            }
        }

        ///<inheritdoc/>
        public void AddCancelButton(bool closesMenu = true, Action<Dictionary<string, object>> action = null)
        {
            ModUIFeatures ui = TryGetUI();
            if (ui != null)
            {
                ui.AddBGButton(EButtonType.Cancel, closesMenu, action);
            }
        }

        ///<inheritdoc/>
        public void AddCustomComponent(ClickableTextureComponent component, object initValue, int pageIdx = 0, char align = 'N', string id = null){

            int wdth = FitWidthInBG(component.bounds.Width, pageIdx, out ModUIFeatures ui);
            Vector2 pos = GetAlignedPos(wdth, pageIdx, align, ui, true, component.bounds.X);

            component.bounds.X = (int)pos.X;
            component.bounds.Y = (int)pos.Y;
            component.bounds.Width = wdth;

            UICustomComponent customComp = new(component, initValue, (EAlign)char.ToUpper(align));
            ui.AddComponent(customComp, pageIdx, id);

        }

        ///<inheritdoc/>
        public void SetComponentValue(string id, object newValue)
        {
            if (id == null || id == "")
            {
                Console.Log($"{id} is not a valid ID. Value set failed.");
                return;
            }
            ModUIFeatures ui = TryGetUI();
            if (ui != null)
            {
                ui.SetComponentValue(id, newValue);
            }
        }

        ///<inheritdoc/>
        public void SetValueChangedAction(Action<string, object> action){
            if(action == null){
                Console.Log("The method for ValueChangedAction cannot be null.");
                return;
            }
            ModUIFeatures ui = TryGetUI();
            if (ui != null)
            {
                ui.SetValueChangedAction(action);
            }
        }

        ///<inheritdoc/>
        public void SetCustomLeftClickAction(string id, Action<int, int> action)
        {
            if(id == null || id == ""){
                Console.Log("You have to provide a valid component id to asign an action.");
                return;
            }
            ModUIFeatures ui = TryGetUI();
            if (ui != null)
            {
                UICustomComponent c = ui.GetCustomComponentById(id);
                c.leftClickAction += action;
            }
        }

        ///<inheritdoc/>
        public void SetCustomLeftClickHeldAction(string id, Action<int, int> action)
        {
            if(id == null || id == ""){
                Console.Log("You have to provide a valid component id to asign an action.");
                return;
            }
            ModUIFeatures ui = TryGetUI();
            if (ui != null)
            {
                UICustomComponent c = ui.GetCustomComponentById(id);
                c.leftClickHeldAction += action;
            }
        }

        ///<inheritdoc/>
        public void SetCustomLeftClickReleasedAction(string id, Action<int, int> action)
        {
            if(id == null || id == ""){
                Console.Log("You have to provide a valid component id to asign an action.");
                return;
            }
            ModUIFeatures ui = TryGetUI();
            if (ui != null)
            {
                UICustomComponent c = ui.GetCustomComponentById(id);
                c.releaseLeftClickAction += action;
            }
        }

        ///<inheritdoc/>
        public void SetCustomScrollWheelAction(string id, Action<int> action)
        {
            if(id == null || id == ""){
                Console.Log("You have to provide a valid component id to asign an action.");
                return;
            }
            ModUIFeatures ui = TryGetUI();
            if (ui != null)
            {
                UICustomComponent c = ui.GetCustomComponentById(id);
                c.scrollWheelAction += action;
            }
        }

        ///<inheritdoc/>
        public void SetCustomKeyPressedAction(string id, Action<Keys> action)
        {
            if(id == null || id == ""){
                Console.Log("You have to provide a valid component id to asign an action.");
                return;
            }
            ModUIFeatures ui = TryGetUI();
            if (ui != null)
            {
                UICustomComponent c = ui.GetCustomComponentById(id);
                c.keyPressAction += action;
            }
        }

        ///<inheritdoc/>
        public void SetCustomRightClickAction(string id, Action<int, int> action)
        {
            if(id == null || id == ""){
                Console.Log("You have to provide a valid component id to asign an action.");
                return;
            }
            ModUIFeatures ui = TryGetUI();
            if (ui != null)
            {
                UICustomComponent c = ui.GetCustomComponentById(id);
                c.rightClickAction += action;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////
        private int FitWidthInBG(int width, int pageIdx, out ModUIFeatures ui)
        {
            ui = TryGetUI();
            if (ui != null)
            {
                return ui.FitComponentWidthInPage(width, pageIdx);
            }
            throw new Exception("Something went wrong with the text width.");
        }

        private Vector2 GetAlignedPos(int width, int pageIdx, char align, ModUIFeatures ui, bool customComp = false, int x = 0)
        {
            int xAligned = customComp && char.ToUpper(align) != 'N'? ui.GetXClamped(x, width, pageIdx) : ui.GetXAlignedInPage(width, pageIdx, (EAlign)char.ToUpper(align));
            return new(
                xAligned,
                ui.LasElementPosInPage(pageIdx)
            );
        }

        private ModUIFeatures TryGetUI()
        {
            ModUIFeatures ui = modsFeatures.GetValueOrDefault(modUniqueId);
            if (ui == null)
                Console.Log("You have not initialized your mod's UI.");

            return ui;
        }
    }
}
