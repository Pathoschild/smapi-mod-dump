namespace InputFix
{
    /// <summary>
    /// The ITextBox interface can be use on LINUX and Windows, which enables TextBox's caret move and text insert
    /// </summary>
    public interface ITextBox
    {
        /// <summary>
        /// Clear and Set TextBox's Text
        /// </summary>
        /// <param name="str"></param>
        void SetText(string str);

        /// <summary>
        /// Get plain text
        /// </summary>
        /// <param name="str"></param>
        string GetText();

        /// <summary>
        /// The SetSelection method selects text within the TextBox.
        /// </summary>
        /// <param name="acpStart"></param>
        /// <param name="acpEnd"></param>
        void SetSelection(int acpStart, int acpEnd);

        /// <summary>
        /// Set Selection state
        /// </summary>
        /// <returns></returns>
        void SetSelState(SelState state);

        /// <summary>
        /// Get Selection
        /// </summary>
        /// <returns></returns>
        Acp GetSelection();

        /// <summary>
        /// Get Selection state
        /// </summary>
        /// <returns></returns>
        SelState GetSelState();

        /// <summary>
        /// Replace TextBox's Text base on its current selection
        /// </summary>
        /// <param name="text">Replace text</param>
        void ReplaceSelection(string text);

        /// <summary>
        /// Get TextBox's Text Length
        /// </summary>
        /// <returns>Text Length</returns>
        int GetTextLength();

        /// <summary>
        /// Get Text ACP Position by Screen pos Rect
        /// </summary>
        /// <param name="rect"></param>
        /// <returns>
        /// acpStart = acpEnd = -1 means rect not overlap TextBox
        /// if overlap TextBox and the left pos > string's right pos, return acpStart = acpEnd = text.length
        /// if overlap TextBox and the right pos < string's left pos, return acpStart = acpEnd = 0
        /// </returns>
        Acp GetAcpByRange(RECT rect);

        /// <summary>
        /// The GetTextExt method returns the bounding box, in world coordinates, of the text at a specified character position.
        /// </summary>
        /// <param name="acp">
        /// Specifies the character position of the text to get in the document.
        /// </param>
        /// <returns>
        /// the bounding box in screen coordinates of the text at the specified character positions.
        /// </returns>
        RECT GetTextExt(Acp acp);

        /// <summary>
        /// Allow IME start composition
        /// </summary>
        bool AllowIME { get; }
    }

    public struct Acp
    {
        public int Start;
        public int End;

        public Acp(int start, int end)
        {
            Start = start;
            End = end;
        }

        public Acp(Acp acp)
        {
            Start = acp.Start;
            End = acp.End;
        }
    };

    public struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    };

    public enum SelState
    {
        /// <summary>
        /// Nothing selected
        /// </summary>
        SEL_AE_NONE,

        /// <summary>
        /// Selection start at acp.Start
        /// </summary>
        SEL_AE_START,

        /// <summary>
        /// Selection start at acp.End
        /// </summary>
        SEL_AE_END
    }
}