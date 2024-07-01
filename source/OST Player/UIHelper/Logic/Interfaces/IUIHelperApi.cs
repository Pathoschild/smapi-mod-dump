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
using StardewValley.Menus;

namespace UIHelper
{
    public interface IUIHelperApi
    {
        /// <summary>
        /// Method to initialize your mod's UI. Your UI won't work if you don't call it.<br/>
        /// A fixed background will be added, wich all pages use as disposition reference and every component will be clamped inside of it.<br/>
        /// It also adds a default page.
        /// </summary>
        /// <param name="bgWidth">The width of the main background. Allowed range: 1000 - viewport width.</param>
        /// <param name="bgHeight">The height of the main background. Allowed range: 500 - viewport height.</param>
        /// <param name="color">The inner color of the main backgound. Wheat by default.</param>
        /// <param name="borderColor">The border color of the main backgound. Burlywood by default.</param>
        /// <param name="defaultPageHasScroll">Defines if the default page has scroll. When you add the first page it will be treated with or without scroll as specified here.</param>
        /// <param name="defElementSpacing">The element spacing of the default page.</param>
        /// <remarks>If you call it when your UI was previously initialized, it'll reset every configuration.</remarks>
        public void InitUIFeatures(int bgWidth, int bgHeight, Color? color = null, Color? borderColor = null, bool defaultPageHasScroll = false, int defElementSpacing = 25);

        /// <summary>
        /// Method to open the configured UI.
        /// </summary>
        /// <param name="playSound">Defines if an opening sound will be played or not when showing the UI.</param>
        public void OpenUI(bool playSound = true);

        /// <summary>
        /// Method to add a new page to the UI and a tab with an icon (you can add up to 10 pages excluding default page).<br/>
        /// If the default page already contains components, these are kept there and only the tab is added above the background.
        /// </summary>
        /// <param name="tabIcon">Icon to display on the tab. If null, tab will show without icon.</param>
        /// <param name="sourceRect">The source rectangle that represents the spot of the icon to be drawn.
        /// If null, it will draw the whole icon.
        /// </param>
        /// <param name="elementSpacing">Vertical separation between components.</param>
        /// <param name="hasScroll">Defines if the page will have scroll or not. Cannot be changed once set.</param>
        public void AddPage(Texture2D tabIcon = null, Rectangle? sourceRect = null, int elementSpacing = 25, bool hasScroll = false);

        /// <summary>
        /// Method to add a title to a specific page. Might change and center position of your page background.<br/>
        /// You only can call this once per page.
        /// </summary>
        /// <param name="title">The horizontally centered text that will be displayed on top of the page.
        /// If it exceeds your background width, it will be cropped to fit.</param>
        /// <param name="pageIdx">The page index wich will contain the title.</param>
        /// <param name="textColor">The color of the text. Black by default.</param>
        /// <param name="drawBox">Chooses whether or not the title will be drawn inside a box.</param>
        /// <param name="boxColor">The inner color of the title box. Wheat by default.</param>
        /// <param name="borderColor">The border color of the title box. Burlywood by default.</param>
        public void AddTitleToPage(string title, int pageIdx = 0, Color? textColor = null, bool drawBox = true, Color? boxColor = null, Color? borderColor = null);

        /// <summary>Method to configure the scroll of a page. If the page has no scroll, this will not take effect.</summary>
        /// <param name="scrollOwnBG">Defines if scroll has its own background. If false, scroll area will be invisible and it will only show the scroll bar.</param>
        /// <param name="showScrollBtns">Defines whether the scroll bar will show up and down buttons or not.</param>
        /// <param name="pageIdx">The index of the page on wich the scroll is being configured.</param>
        /// <param name="scrollColor">The inner color of the scroll area. Wheat by default.</param>
        /// <param name="scrollBorderColor">The border color of the scroll area. Burlywood by default.</param>
        public void ConfigPageScrollArea(bool scrollOwnBG, bool showScrollBtns, int pageIdx = 0, Color? scrollColor = null, Color? scrollBorderColor = null);


        /* THE FOLLOWING METHODS WILL ADD THE SPECIFIED ITEM BASED ON THE CURRENT VERTICAL POSITION 
        (WICH ACCUMULATES WITH THE PREVIOUSLY SET ELEMENT SPACING AND THE HEIGHT OF THE ITEM). 
        IF YOU HAVE NOT SET A SCROLL, THEY WILL BE ADDED UNTIL THE BOTTOM OF THE 
        PAGE IS REACHED. */

        /// <summary>
        /// Adds some vertical space to the page.
        /// </summary>
        /// <param name="pageIdx">The index of the page.</param>
        /// <param name="height">The number of pixels to add vertically.</param>
        public void AddBlankSpace(int pageIdx = 0, int height = 20);

        /// <summary>
        /// Adds a horizontal line to the page.
        /// </summary>
        /// <param name="pageIdx">The index of the page</param>
        /// <param name="color">The color of the line. Burlywood by default.</param>
        public void AddSeparator(int pageIdx = 0, Color? color = null);

        /// <summary>
        /// Add an accept button to the background to apply changes to every UI component.
        /// </summary>
        /// <param name="closesMenu">Define if you want to close the menu when clicking the button.</param>
        /// <param name="action">Asociate a method to invoke when clicking the button.<br/>
        /// The dictionary parameter will contain all the components ids with their corresponding original value.
        /// </param>
        public void AddAcceptButton(bool closesMenu = true, Action<Dictionary<string, object>> action = null);

        /// <summary>
        /// Add a cancel button to the background to discard the changes of every UI component.
        /// </summary>
        /// <param name="closesMenu">Define if you want to close the menu when clicking the button.</param>
        /// <param name="action">Asociate a method to invoke when clicking the button.<br/>
        /// The dictionary parameter will contain all the components ids with their corresponding new value.
        /// </param>
        public void AddCancelButton(bool closesMenu = true, Action<Dictionary<string, object>> action = null);


        /********************************PREDEFINED COMPONENTS********************************/
        /* All the components in this section have their own behaviour by default and only the
        parameters given can be modified. Each one of them will be clamped to fit in background 
        (or in scroll area if there is any). Also note that you can give them an ID, it is used
        everytime an action is performed on the component and gives you the possibility to access
        its specific value. Otherwise a random ID is generated and you won't be able to know which
        component is the owner of the returned value.
        If you want to define more behaviours, try CUSTOM COMPONENTS section below. */


        /// <summary>
        /// Add a label to show informative text to a page.
        /// </summary>
        /// <param name="text">Text to display. If it is larger than bg/scroll it will be cropped.</param>
        /// <param name="hasBox">Chooses whether or not the text is drawn inside a box.</param>
        /// <param name="pageIdx">The index of the page</param>
        /// <param name="align">Defines the horizontal alignment of the component on the area.<br/>
        /// Valid values:<br/> C or any other char -> Center alignment<br/> L -> Left alignment<br/> R -> Right alignment
        /// </param>
        /// <param name="textColor">The color of the text. Black by default.</param>
        /// <param name="innerColor">The inner color of the label. Wheat by default.</param>
        /// <param name="borderColor">The border color of the label. Burlywood by default.</param>
        public void AddLabel(string text, bool hasBox, int pageIdx = 0, char align = 'C', Color? textColor = null, Color? innerColor = null, Color? borderColor = null);

        /// <summary>
        /// Add a label with an image to a page.
        /// </summary>
        /// <param name="icon">Image to display. If it is larger than bg/scroll it will be cropped.</param>
        /// <param name="width">Destination width of the image.</param>
        /// <param name="height">Destination height of the image.</param>
        /// <param name="hasBox">Chooses whether or not the image is drawn inside a box.</param>
        /// <param name="pageIdx">The index of the page</param>
        /// <param name="align">Defines the horizontal alignment of the component on the area.<br/>
        /// Valid values:<br/> C or any other char -> Center alignment<br/> L -> Left alignment<br/> R -> Right alignment
        /// </param>
        /// <param name="borderColor">The border color of the label. Burlywood by default.</param>
        /// <param name="id">The ID of the label.</param>
        /// <param name="sourceRect">The source rectangle that represents the spot of the icon to be drawn.
        /// If null, it will draw the whole icon.
        /// </param>
        public void AddTextureLabel(Texture2D icon, int width, int height, bool hasBox, int pageIdx = 0, char align = 'C', Color? borderColor = null, Rectangle? sourceRect = null);

        /// <summary>
        /// Add a checkbox with a description to a page.
        /// </summary>
        /// <param name="text">The text to provide a description to the checkbox.</param>
        /// <param name="pageIdx">The index of the page</param>
        /// <param name="align">Defines the horizontal alignment of the component on the area.<br/>
        /// Valid values:<br/> C or any other char -> Center alignment<br/> L -> Left alignment<br/> R -> Right alignment
        /// </param>
        /// <param name="isChecked">Initialize the checkbox checked or unchecked.</param>
        /// <param name="textColor">The color of the description text.</param>
        /// <param name="id">The ID of the label.</param>
        public void AddCheckbox(string text, int pageIdx = 0, char align = 'C', bool isChecked = false, Color? textColor = null, string id = null);

        /// <summary>
        /// Add a slider to select the desired value within certain range.
        /// </summary>
        /// <param name="initValue">The initial value of the slider. Must be numeric, otherwise the slider wont show. If
        /// its floating point type, the values will be shown with 2 digits to the right of the point.<br/>
        /// If null, the starting value will be clamped within range.</param>
        /// <param name="pageIdx">The index of the page.</param>
        /// <param name="align">Defines the horizontal alignment of the component on the area.<br/>
        /// Valid values:<br/> C or any other char -> Center alignment<br/> L -> Left alignment<br/> R -> Right alignment
        /// </param>
        /// <param name="minVal">The minimum value of the slider. If null, it will be 0.</param>
        /// <param name="maxVal">The maximum value of the slider. If null, it will be 9999.</param>
        /// <param name="width">The width of the slider. The valid range is: 220 - background width minus the displayed value.</param>
        /// <param name="textColor">The color of the shown values. Black by default.</param>
        /// <param name="id">The ID of the slider.</param>
        public void AddSlider(object initValue, int pageIdx = 0, char align = 'C', object minVal = null, object maxVal = null, int width = 220, Color? textColor = null, string id = null);

        /// <summary>
        /// Adds a writtable textbox to a page.
        /// </summary>
        /// <param name="width">The width of the textbox. It will determine the max size of the written text.</param>
        /// <param name="pageIdx">The index of the page.</param>
        /// <param name="align">Defines the horizontal alignment of the component on the area.<br/>
        /// Valid values:<br/> C or any other char -> Center alignment<br/> L -> Left alignment<br/> R -> Right alignment
        /// </param>
        /// <param name="textColor">The color of the text. Black by default.</param>
        /// <param name="initialText">The initial text to add to the textbox. If it doesn't fit on the width, it will be cropped.</param>
        /// <param name="id">The ID of the textbox</param>
        public void AddTextBox(int width, int pageIdx = 0, char align = 'C', Color? textColor = null, string initialText = "",string id = null);

        /// <summary>
        /// Adds a button with text in it on a page to perform an action and/or get the value of the component with a provided id.
        /// </summary>
        /// <param name="text">The text to display in the button. Will determine the width. If null, the text will be "Button".</param>
        /// <param name="pageIdx">The index of the page.</param>
        /// <param name="align">Defines the horizontal alignment of the component on the area.<br/>
        /// Valid values:<br/> C or any other char -> Center alignment<br/> L -> Left alignment<br/> R -> Right alignment
        ///</param>
        /// <param name="asociatedId">The id of the component from wich you want to get the value.<br/>
        /// The value will be returned in the ValueChanged action that you defined (if any).</param>
        /// <param name="customAction">A custom action that will be invoked when you click the button.</param>
        /// <param name="textColor">The color of the text. Black by default.</param>
        public void AddTextButton(string text, int pageIdx = 0, char align = 'C', string asociatedId = null, Action customAction = null, Color? textColor = null);

        //public void AddDropDown();


        /*******************CUSTOM COMPONENT (ONLY IF YOU KNOW WHAT YOU ARE DOING)***********************/
        /*This component will be more flexible, as you can define many of its own behaviours. When you pass the
        ClickableTextureComponent or its son, the UI will use its following methods as you implemented them:
            -tryHover(int x, int y, float maxScaleIncrease = <the one you define>)
            -draw(SpriteBatch b) (make sure to draw everything relative to your CTC's bounds and that everything its
            drawn within them to avoid issues)
            -containsPoint(int x, int y)
        The rest of the methods have to be passed in the proper params. Also the Y position in the screen won't be the
        same as the one on your bounds, it will adjusted depending on the current element spot (your X position will be
        clamped within the limits).
        */

        /// <summary>
        /// Adds a custom component to a page.
        /// </summary>
        /// <param name="component">The defined ClickableTextureComponent.</param>
        /// <param name="initValue">The initial value of your component. You must manipulate it through SetComponentValue method below.<br/>
        /// It will be returned in ValueChangedAction when set. Also, will be returned with the rest of the UI component
        /// values in Accept and Cancel buttons actions.
        /// </param>
        /// <param name="pageIdx">The index of the page.</param>
        /// <param name="align">Defines the horizontal alignment of the component on the area.<br/>
        /// Valid values:<br/> C or any other char -> Center alignment<br/> L -> Left alignment<br/> R -> Right alignment<br/> N -> None (use your bounds's X and clamp it in the screen)
        ///</param>
        /// <param name="id">The id of the custom component.</param>
        public void AddCustomComponent(ClickableTextureComponent component, object initValue, int pageIdx = 0, char align = 'N', string id = null);

        /// <summary>
        /// Asign an action to perform when left click button is just pressed. Only for custom components.
        /// </summary>
        /// <param name="id">The id of the custom component.</param>
        /// <param name="action">The action to perform. Will receive the x and y coords on screen.</param>
        public void SetCustomLeftClickAction(string id, Action<int, int> action);

        /// <summary>
        /// Asign an action to perform when left click button is held down. Only for custom components.
        /// </summary>
        /// <param name="id">The id of the custom component.</param>
        /// <param name="action">The action to perform. Will receive the x and y coords on screen.</param>
        public void SetCustomLeftClickHeldAction(string id, Action<int, int> action);

        /// <summary>
        /// Asign an action to perform when left click button is just released. Only for custom components.
        /// </summary>
        /// <param name="id">The id of the custom component.</param>
        /// <param name="action">The action to perform. Will receive the x and y coords on screen.</param>
        public void SetCustomLeftClickReleasedAction(string id, Action<int, int> action);

        /// <summary>
        /// Asign an action to perform when the mouse scroll wheel moves. Only for custom components.
        /// </summary>
        /// <param name="id">The id of the custom component.</param>
        /// <param name="action">The action to perform. Will receive the direction of the scroll wheel.</param>
        public void SetCustomScrollWheelAction(string id, Action<int> action);

        /// <summary>
        /// Asign an action to perform when a key is just pressed. Only for custom components.
        /// </summary>
        /// <param name="id">The id of the custom component.</param>
        /// <param name="action">The action to perform. Will receive pressed key.</param>
        public void SetCustomKeyPressedAction(string id, Action<Keys> action);

        /// <summary>
        /// Asign an action to perform when right click button is just pressed. Only for custom components.
        /// </summary>
        /// <param name="id">The id of the custom component.</param>
        /// <param name="action">The action to perform. Will receive the x and y coords on screen.</param>
        public void SetCustomRightClickAction(string id, Action<int, int> action);

        /*******************************    UTILITY METHODS     ****************************************/

        /// <summary>
        /// Change the current value of a component.
        /// </summary>
        /// <param name="id">The id of the component.</param>
        /// <param name="newValue">The new value of the component. Must be a valid type for the component.<br/>
        /// -Checkbox: bool<br/>
        /// -Slider: float (if the slider was initialized with int, it will be rounded)<br/>
        /// -Textbox: string <br/>
        /// -Custom Component: depends on how you initialized it (do it wrong at your own risk!)
        /// </param>
        public void SetComponentValue(string id, object newValue);

        /// <summary>
        /// Asign a method to an action that will be invoked when any component changes its value. Must be called once per UI.
        /// </summary>
        /// <param name="action">Your method. Its params will be the id of the component and the new value.</param>
        public void SetValueChangedAction(Action<string, object> action);

    }
}
