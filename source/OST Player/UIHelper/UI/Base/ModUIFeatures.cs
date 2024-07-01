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
    public class ModUIFeatures : IClickableMenu
    {

        private Rectangle bgRect = Rectangle.Empty;
        private Color bgColor, bgBorderColor;
        internal bool BGSet { get; private set; } = false;
        private HashSet<UIPage> pages = new HashSet<UIPage>();
        private const int maxPages = 10;
        private int shownPage = 0;
        private const int tabSpacing = 20;
        private bool firstPageAdded = false;
        internal readonly int titleMinSpace = 32 + (int)Game1.dialogueFont.MeasureString("ABC").Y + 10 + 48 + 16;

        private List<BGButton> btns = new(){null, null};
        private Action<Dictionary<string, object>> newValuesAction, oldValuesAction;
        private Action<string, object> valChanged = null;

        internal bool ShowTabs { get; private set; } = false;

        internal ModUIFeatures(int bgWigth, int bgHeight, Color? color = null, Color? borderColor = null, bool defaultPageHasScroll = false, int defElementSpacing = 25)
        {
            SetBG(bgWigth, bgHeight, color, borderColor, defaultPageHasScroll, defElementSpacing);
        }

        internal void OpenUI()
        {
            Game1.activeClickableMenu = this;
            ReadjustMenu();
        }

        internal void ReadjustMenu()
        {
            ClampBG(bgRect.Width, bgRect.Height);
            int i = 0;
            foreach (UIPage pg in pages)
            {
                pg.ReadjustPage(bgRect);
                i++;
            }
            SetTabsBounds();
            pages.ElementAt(shownPage).ReadjustBtns(btns);
        }

        public override void draw(SpriteBatch spriteBatch)
        {
            using (SpriteBatch b = new SpriteBatch(Game1.graphics.GraphicsDevice))
            {
                b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
                b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
                DrawPageTitle(b);
                DrawTabs(b);
                DrawBG(b);
                b.End();
            }

            DrawPageComponents();
            drawMouse(spriteBatch);
        }

        private void DrawBG(SpriteBatch b)
        {
            UIUtils.DrawBox(b, bgRect, bgColor, 255, 0, true);
            UIUtils.DrawBox(b, bgRect, bgBorderColor, 0, 255, true);
            
            btns.ForEach(btn => btn?.draw(b));
        }

        private void DrawPageComponents()
        {
            if (shownPage >= 0 && shownPage < pages.Count)
            {
                pages.ElementAt(shownPage).DrawComponents();
            }
        }

        private void DrawPageTitle(SpriteBatch b)
        {
            if (shownPage >= 0 && shownPage < pages.Count)
            {
                pages.ElementAt(shownPage).DrawTitle(b);
            }
        }

        private void DrawTabs(SpriteBatch b)
        {
            if (ShowTabs)
                foreach (UIPage page in pages)
                {
                    page.drawTab(b);
                }
        }

        public override void receiveKeyPress(Keys key)
        {
            UIPage pg = pages.ElementAt(shownPage);
            if (key.ToSButton() == SButton.Escape)
            {
                if (pg.GetFocusComponent() == null)
                {
                    exitThisMenu();
                    return;
                }
                pg.UnfocusElements();
                return;
            }
            pg.receiveKeyPress(key);

        }

        public override void receiveGamePadButton(Buttons b)
        {
            UIPage pg = pages.ElementAt(shownPage);
            if (b == Buttons.B)
            {
                if (pg.GetFocusComponent() == null)
                {
                    exitThisMenu();
                    return;
                }
                pg.UnfocusElements();
                return;
            }
            pg.receiveGamePadButton(b);
        }

        public override void gamePadButtonHeld(Buttons b)
        {
            if (b == Buttons.RightThumbstickUp)
                receiveScrollWheelAction(1);
            else if (b == Buttons.RightThumbstickDown)
                receiveScrollWheelAction(-1);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {

            if (pages.ElementAt(shownPage).IsTitlePointed(x, y))
                return;

            int clickedTabIdx = GetClickedTab(x, y);
            if (!pages.ElementAt(shownPage).bgRect.Contains(x, y) && clickedTabIdx == -1)
            {
                exitThisMenu();
            }
            else
            {
                if (clickedTabIdx != -1)
                {
                    ChangeActiveTab(shownPage, clickedTabIdx);
                    return;
                }

                BGButton btn = btns.Find(b => b != null && b.containsPoint(x, y));
                if(btn != null){
                    btn.Highlighted = true;
                    btn.Clicked = true;
                    Game1.playSound("select");
                    return;
                }

                pages.ElementAt(shownPage).receiveLeftClick(x, y);
            }
        }

        public override void leftClickHeld(int x, int y)
        {
            pages.ElementAt(shownPage).leftClickHeld(x, y);
        }

        public override void releaseLeftClick(int x, int y)
        {
            pages.ElementAt(shownPage).releaseLeftClick(x, y);
            
            BGButton btn = btns.Find(b => b != null && b.containsPoint(x,y));
            if(btn != null){
                btn.performLeftClick();
            }

            btns.ForEach(b => b?.Unclick());
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            btns.ForEach(b => b?.Unclick());
            pages.ElementAt(shownPage).receiveRightClick(x, y);
        }

        public override void receiveScrollWheelAction(int direction)
        {
            pages.ElementAt(shownPage).receiveScrollWheelAction(direction);
        }

        private void ChangeActiveTab(int prevTab, int newTab)
        {
            Game1.playSound("drumkit6");
            if(prevTab == newTab)
                return;

            shownPage = newTab;
            pages.ElementAt(prevTab).TabActive = false;
            pages.ElementAt(newTab).TabActive = true;

            pages.ElementAt(newTab).ReadjustPage(bgRect);
            SetTabsBounds();
            pages.ElementAt(newTab).ReadjustBtns(btns);
        }

        private void SetTabsBounds(){
            Rectangle bgRect = pages.ElementAt(shownPage).bgRect;
            int xPos = bgRect.Left + tabSpacing;
            foreach(UIPage pg in pages){
                pg.tabRect = new(xPos, bgRect.Top - 48 - (pg.TabActive? 15: 0),64, 64);
                xPos += 64 + tabSpacing;
            }
        }

        private int GetClickedTab(int x, int y)
        {
            int idx = -1;
            if (ShowTabs)
                for (int i = 0; i < pages.Count; i++)
                {
                    if (pages.ElementAt(i).IsTabPointed(x, y))
                    {
                        idx = i;
                        break;
                    }
                }
            return idx;
        }

        private void SetBG(int width, int height, Color? color, Color? borderColor, bool defaultPageHasScroll, int defElementSpacing)
        {
            if (!BGSet)
            {
                ClampBG(width, height);
                bgColor = color ?? Color.Wheat;
                bgBorderColor = borderColor ?? Color.BurlyWood;

                AddPage(elementSpacing: defElementSpacing, hasScroll: defaultPageHasScroll, defPage: true);
                BGSet = true;
            }
            else
            {
                Console.Log("Background already set.");
            }
        }

        private void ClampBG(int width, int height)
        {
            int wdth = Math.Clamp(width, 1000, Game1.uiViewport.Width - 32);
            int hght = Math.Clamp(height, 500, Game1.uiViewport.Height - titleMinSpace - 16);
            int topLeftX = (int)Utility.getTopLeftPositionForCenteringOnScreen(wdth, hght).X;
            int topLeftY = Game1.uiViewport.Y + titleMinSpace;
            bgRect = new(topLeftX, topLeftY, wdth, hght);
        }

        internal void AddPage(Texture2D tabIcon = null, Rectangle? sourceRect = null, int elementSpacing = 25, bool hasScroll = false, bool defPage = false)
        {
            if (!BGSet && !defPage)
            {
                Console.Log("You have not set the background.");
                return;
            }

            if (pages.Count >= maxPages)
            {
                Console.Log($"You have reached the max number of pages allowed: {maxPages}.");
                return;
            }

            UIPage page;
            if (defPage || firstPageAdded)
            {
                page = new UIPage(elementSpacing, bgRect, tabIcon, hasScroll);
            }
            else //Only when adding first page
            {
                page = pages.ElementAt(0);
                pages.Clear();
                firstPageAdded = true;
                if (elementSpacing != page.elementSpacing)
                    Console.Log("You can't change the element spacing set to the default page.");
            }

            page.SetTabIcon(tabIcon, sourceRect);
            pages.Add(page);

            if (!defPage)
            {
                ShowTabs = true;
            }else{
                pages.ElementAt(0).TabActive = true;
            }
        }

        public override void performHoverAction(int x, int y)
        {
            pages.ElementAt(shownPage).performHoverAction(x, y);
            btns.ForEach(b => b?.performHoverAction(x, y));
        }

        internal void SetTitleToPage(string title, int pageIdx, Color? textColor, bool drawBox, Color? titleColor, Color? borderColor)
        {
            TryAccessPage(pageIdx, out UIPage pg);

            if (!pg.TitleSet)
                pg.SetTitle(title, textColor, drawBox, titleColor, borderColor);
            else
                Console.Log($"Title already set to page {pageIdx}");
        }

        internal void ConfigPageScroll(bool scrollOwnBG, bool showScrollBtns, int pageIdx, Color? scrollColor = null, Color? scrollBorderColor = null)
        {
            TryAccessPage(pageIdx, out UIPage pg);
            if (pg.HasScrollBar)
                pg.ConfigScroll(scrollOwnBG, showScrollBtns, scrollColor, scrollBorderColor);
            else
                Console.Log($"Page at index {pageIdx} has no scroll to configure.");
        }

        internal void AddBlankSpace(int pageIdx, int height)
        {
            TryAccessPage(pageIdx, out UIPage pg);
            pg.AddBlankSpace(height);

        }

        internal void AddSeparator(int pageIdx, Color? color)
        {
            TryAccessPage(pageIdx, out UIPage pg);
            pg.AddSeparator(color);
        }

        internal void AddComponent(UIComponent component, int pageIdx = 0, string id = null)
        {
            TryAccessPage(pageIdx, out UIPage pg);
            IUniqueIdGen gen = new UniqueIdGen();
            string componentId = ValidId(id) ? id : gen.Generate();
            component.ValueChangedAction += GetValueChange;
            pg.AddComponent(component, componentId);
        }

        internal void SetValueChangedAction(Action<string, object> action){
            if(valChanged == null){
                valChanged = action;
            }else{
                Console.Log("You've already set a ValueSetAction.");
            }
        }

        private bool ValidId(string id)
        {
            if (id == null || id.Trim() == "")
                return false;
            string component = getAllComponentIds().FirstOrDefault(s => s == id);
            if (component != null)
            {
                Console.Log($"UI component -{id}- already exists. Setting generated id.");
                return false;
            }
            return true;
        }

        private List<string> getAllComponentIds(){
            List<string> ids = new();
            foreach(UIPage pg in pages){
                ids = [.. ids, .. pg.GetAllComponentIds()];
            }
            return ids;
        }

        internal void AddBGButton(EButtonType type, bool closesMenu, Action<Dictionary<string, object>> action){

            switch (type)
            {
                case EButtonType.Accept:
                    if(btns[1] != null){
                        Console.Log("Accept Button already defined.");
                        return;
                    }
                    newValuesAction += action;
                    btns[1] = new BGAcceptButton(Rectangle.Empty, closesMenu? exitThisMenu: null, ReturnNewValues);
                    break;
                case EButtonType.Cancel:
                    if(btns[0] != null){
                        Console.Log("Cancel Button already defined.");
                        return;
                    }
                    oldValuesAction += action;
                    btns[0] = new BGCancelButton(Rectangle.Empty, closesMenu? exitThisMenu: null, ReturnOldValues);
                    break;
            }
            pages.ElementAt(shownPage).ReadjustBtns(btns);
        }

        internal void GetValueChange(string id, object val){
            valChanged?.Invoke(id, val);
        }

        private void ReturnNewValues(){
            SaveAllValues();
            newValuesAction?.Invoke(GetAllValues());
        }
        private void ReturnOldValues(){
            DiscardAllValues();
            oldValuesAction?.Invoke(GetAllValues());
        }

        private void DiscardAllValues(){
            foreach (UIComponent c in GetAllInteractibleComponents())
            {
                c.Value = c.PrevValue;
            }
        }
        private void SaveAllValues(){
            foreach (UIComponent c in GetAllInteractibleComponents())
            {
                c.PrevValue = c.Value;
            }
        }

        private HashSet<UIComponent> GetAllInteractibleComponents(){
            HashSet<UIComponent> comps = new();
            foreach(UIPage pg in pages){
                foreach(UIComponent c in pg.GetInteractiveComponents()){
                    comps.Add(c);
                }
            }
            return comps;
        }

        private void exitThisMenu(){
            base.exitThisMenu();
        }

        private Dictionary<string, object> GetAllValues(){
            Dictionary<string, object> vals = new();
            foreach(UIPage pg in pages){
                foreach(UIComponent c in pg.GetInteractiveComponents()){
                    vals.Add(c.Id, c.Value);
                }
            }
            return vals;
        }

        internal void SetComponentValue(string id, object newValue){
            UIComponent c = GetComponentById(id);
            if(c == null){
                Console.Log($"There is no component with id {id}.");
                return;
            }

            if(!ValidValueForComponent(c, newValue))
                return;

            c.Value = newValue;

        }

        internal void AsignComponentValueToButton(UITextButton btn, string compId){
            if(compId == ""){
                Console.Log("You must bind a valid id for your button to get its value.");
                return;
            }

            UIComponent c = GetComponentById(compId);
            if(c != null){
                btn.action += c.InvokeValueChange;
            }else
                Console.Log($"{compId} does not exist. Can't bind button");
        }

        private static bool ValidValueForComponent(UIComponent c, object newVal){

            if(c is UICheckbox){
                if(newVal is not bool){
                    Console.Log("The new value for a checkbox must be a bool.");
                    return false;
                }
                return true;
            }
            if(c is UISlider){
                if(newVal != null && !decimal.TryParse(Convert.ToString(newVal), out decimal decVal)){
                    Console.Log("The new value for a slider must be numeric.");
                    return false;
                }
                return true;
            }
            if(c is UITextbox){
                if(newVal is not string){
                    Console.Log("The new value for a textbox must be a string.");
                    return false;
                }
                return true;
            }
            if(c is UICustomComponent){
                return true;
            }
            Console.Log("The id passed does not exist (this message should not appear).");
            return false;
        }

        private UIComponent GetComponentById(string id){
            UIComponent component = null;
            foreach(UIPage pg in pages){
                foreach(UIComponent c in pg.GetInteractiveComponents()){
                    if(c.Id == id){
                        component = c;
                        break;
                    }
                }
                if(component != null)
                    break;
            }
            return component;
        }

        internal UICustomComponent GetCustomComponentById(string id){
            UIComponent c = GetComponentById(id);
            if(c != null){
                if(c is UICustomComponent)
                    return (UICustomComponent)c;
                else{
                    Console.Log($"Component {id} is not a custom component.");
                    return null;
                }
            }else
                Console.Log($"There is no component with id {id}.");
            
            return null;
        }

        internal int LasElementPosInPage(int pageIdx)
        {
            TryAccessPage(pageIdx, out UIPage pg);
            return pg.lastElementPos;
        }

        internal int GetXAlignedInPage(int width, int pageIdx, EAlign alignment)
        {

            TryAccessPage(pageIdx, out UIPage pg);
            return pg.GetXAligned(width, alignment);
        }

        internal int GetXClamped(int x, int width, int pageIdx){

            TryAccessPage(pageIdx, out UIPage pg);
            return pg.GetXClamped(x, width);
        }

        internal int FitComponentWidthInPage(int width, int pageIdx)
        {
            TryAccessPage(pageIdx, out UIPage pg);
            return pg.FitComponentWidth(width);
        }

        private void TryAccessPage(int pageIdx, out UIPage? pg)
        {
            if (!BGSet)
            {
                pg = null;
                throw new Exception("You have not set the background.");
            }
            pg = pages.ElementAtOrDefault(pageIdx);
            if (pg == null)
            {
                throw new Exception($"Available page indexes are from 0 to {pages.Count - 1}");
            }
        }
    }
}
