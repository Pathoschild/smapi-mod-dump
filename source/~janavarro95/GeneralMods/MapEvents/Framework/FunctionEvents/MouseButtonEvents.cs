namespace EventSystem.Framework.FunctionEvents
{
    /// <summary>Used to handle mouse interactions with button clicks and scrolling the mouse wheel.</summary>
    public class MouseButtonEvents
    {
        /// <summary>Function that runs when the user left clicks.</summary>
        public functionEvent onLeftClick;

        /// <summary>Function that runs when the user right clicks.</summary>
        public functionEvent onRightClick;

        /// <summary>Function that runs when the user scrolls the mouse wheel.</summary>
        public functionEvent onMouseScroll;

        /// <summary>A constructor used to set a single function to a mouse button.</summary>
        /// <param name="clickFunction"></param>
        /// <param name="leftClick">If true the function is set to the left click. If false the function is set to the right click.</param>
        public MouseButtonEvents(functionEvent clickFunction, bool leftClick)
        {
            if (leftClick) this.onLeftClick = clickFunction;
            else this.onRightClick = clickFunction;
        }

        /// <summary>A constructor used to map functions to mouse clicks.</summary>
        /// <param name="OnLeftClick">A function to be ran when the mouse left clicks this position.</param>
        /// <param name="OnRightClick">A function to be ran when the mouse right clicks this position.</param>
        public MouseButtonEvents(functionEvent OnLeftClick, functionEvent OnRightClick)
        {
            this.onLeftClick = OnLeftClick;
            this.onRightClick = OnRightClick;
        }

        /// <summary>A constructor used to map functions to mouse clicks and scrolling the mouse wheel.</summary>
        /// <param name="OnLeftClick">A function to be ran when the mouse left clicks this position.</param>
        /// <param name="OnRightClick">A function to be ran when the mouse right clicks this position.</param>
        /// <param name="OnMouseScroll">A function to be ran when the user scrolls the mouse</param>
        public MouseButtonEvents(functionEvent OnLeftClick, functionEvent OnRightClick, functionEvent OnMouseScroll)
        {
            this.onLeftClick = OnLeftClick;
            this.onRightClick = OnRightClick;
            this.onMouseScroll = OnMouseScroll;
        }
    }
}
