namespace LovedLabels.Framework
{
    /// <summary>The mod settings that can be configured by the user.</summary>
    internal class LoveLabelConfig
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The tooltip text to show if an animal has already been petted.</summary>
        public string AlreadyPettedLabel { get; set; } = "Is Loved";

        /// <summary>The tooltip text to show if an animal hasn't been petted yet.</summary>
        public string NeedsToBePettedLabel { get; set; } = "Needs Love";
    }
}
